﻿// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;

namespace Rock.Jobs
{
    /// <summary>
    /// Job to download any scheduled payments that were processed by the payment gateway
    /// </summary>
    [DisplayName( "Download Payments" )]
    [Category( "Finance" )]
    [Description( "Job to download any scheduled payments that were processed by the payment gateway." )]

    #region Job Attributes
    [IntegerField(
        "Days Back",
        Key = AttributeKey.DaysBack,
        Description = "The number of days prior to the current date to use as the start date when querying for scheduled payments that were processed.",
        IsRequired = true,
        DefaultIntegerValue = 7,
        Order = 1 )]

    [TextField(
        "Batch Name Prefix",
        Key = AttributeKey.BatchNamePrefix,
        Description = "The batch prefix name to use when creating a new batch.",
        IsRequired = false,
        DefaultValue = "Online Giving",
        Order = 2 )]

    [SystemCommunicationField( "Receipt Email",
        Key = AttributeKey.ReceiptEmail,
        Description = "The system email to use to send the receipts.",
        IsRequired = false,
        Order = 3 )]

    [SystemCommunicationField(
        "Failed Payment Email",
        Key = AttributeKey.FailedPaymentEmail,
        Description = "The system email to use to send a notice about a scheduled payment that failed.",
        IsRequired = false,
        Order = 4 )]

    [WorkflowTypeField(
        "Failed Payment Workflow",
        Key = AttributeKey.FailedPaymentWorkflow,
        Description = "An optional workflow to start whenever a scheduled payment has failed.",
        AllowMultiple = false,
        IsRequired = false,
        Order = 5 )]

    [FinancialGatewayField(
        "Target Gateway",
        Key = AttributeKey.TargetGateway,
        Description = "By default payments will download from all active financial gateways. Optionally select a single gateway to download scheduled payments from.  You will need to set up additional jobs targeting other active gateways.",
        IsRequired = false,
        Order = 6 )]

    [BooleanField(
        "Verbose Logging",
        Key = AttributeKey.VerboseLogging,
        Description = "If enabled then the job results will include more detailed information that can help troubleshoot problems.",
        IsRequired = true,
        DefaultBooleanValue = true,
        Order = 7 )]

    #endregion Job Attributes
    public class GetScheduledPayments : RockJob
    {
        #region Attribute Keys

        /// <summary>
        /// Keys to use for Block Attributes
        /// </summary>
        private static class AttributeKey
        {
            /// <summary>
            /// The days back
            /// </summary>
            public const string DaysBack = "DaysBack";

            /// <summary>
            /// The batch name prefix
            /// </summary>
            public const string BatchNamePrefix = "BatchNamePrefix";

            /// <summary>
            /// The receipt email
            /// </summary>
            public const string ReceiptEmail = "ReceiptEmail";

            /// <summary>
            /// The failed payment email
            /// </summary>
            public const string FailedPaymentEmail = "FailedPaymentEmail";

            /// <summary>
            /// The failed payment workflow
            /// </summary>
            public const string FailedPaymentWorkflow = "FailedPaymentWorkflow";

            /// <summary>
            /// The target gateway
            /// </summary>
            public const string TargetGateway = "TargetGateway";

            /// <summary>
            /// The verbose logging key
            /// </summary>
            public const string VerboseLogging = "VerboseLogging";
        }

        #endregion Attribute Keys

        /// <summary> 
        /// Empty constructor for job initialization
        /// <para>
        /// Jobs require a public empty constructor so that the
        /// scheduler can instantiate the class whenever it needs.
        /// </para>
        /// </summary>
        public GetScheduledPayments()
        {
        }

        /// <inheritdoc cref="RockJob.Execute()"/>
        public override void Execute()
        {
            var exceptionMsgs = new List<string>();
            var scheduledPaymentsProcessed = 0;

            var receiptEmail = GetAttributeValue( AttributeKey.ReceiptEmail ).AsGuidOrNull();
            var failedPaymentEmail = GetAttributeValue( AttributeKey.FailedPaymentEmail ).AsGuidOrNull();
            var failedPaymentWorkflowType = GetAttributeValue( AttributeKey.FailedPaymentWorkflow ).AsGuidOrNull();
            var daysBack = GetAttributeValue( AttributeKey.DaysBack ).AsIntegerOrNull() ?? 1;
            var verboseLogging = GetAttributeValue( AttributeKey.VerboseLogging ).AsBoolean( true );

            var today = RockDateTime.Today;
            var daysBackTimeSpan = new TimeSpan( daysBack, 0, 0, 0 );

            string batchNamePrefix = GetAttributeValue( AttributeKey.BatchNamePrefix );
            Dictionary<FinancialGateway, string> processedPaymentsSummary = new Dictionary<FinancialGateway, string>();

            using ( var rockContext = new RockContext() )
            {
                var targetGatewayQuery = new FinancialGatewayService( rockContext ).Queryable().Where( g => g.IsActive ).AsNoTracking();

                var targetGatewayGuid = GetAttributeValue( AttributeKey.TargetGateway ).AsGuidOrNull();
                if ( targetGatewayGuid.HasValue )
                {
                    targetGatewayQuery = targetGatewayQuery.Where( g => g.Guid == targetGatewayGuid.Value );
                }

                foreach ( var financialGateway in targetGatewayQuery.ToList() )
                {
                    try
                    {
                        financialGateway.LoadAttributes( rockContext );

                        var gateway = financialGateway.GetGatewayComponent();
                        if ( gateway == null )
                        {
                            continue;
                        }

                        var endDateTime = today.Add( financialGateway.GetBatchTimeOffset() );

                        // If the calculated end time has not yet occurred, use the previous day.
                        endDateTime = RockDateTime.Now.CompareTo( endDateTime ) >= 0 ? endDateTime : endDateTime.AddDays( -1 );

                        var startDateTime = endDateTime.Subtract( daysBackTimeSpan );

                        var errorMessage = string.Empty;
                        var payments = gateway.GetPayments( financialGateway, startDateTime, endDateTime, out errorMessage );

                        if ( string.IsNullOrWhiteSpace( errorMessage ) )
                        {
                            var gatewayProcessPaymentsSummary = FinancialScheduledTransactionService.ProcessPayments( financialGateway, batchNamePrefix, payments, string.Empty, receiptEmail, failedPaymentEmail, failedPaymentWorkflowType, verboseLogging );
                            processedPaymentsSummary.Add( financialGateway, gatewayProcessPaymentsSummary );
                            scheduledPaymentsProcessed += payments.Count();
                        }
                        else
                        {
                            processedPaymentsSummary.Add( financialGateway, errorMessage + Environment.NewLine );
                        }
                    }
                    catch ( Exception ex )
                    {
                        ExceptionLogService.LogException( ex, null );
                        exceptionMsgs.Add( ex.Message );
                        processedPaymentsSummary.Add( financialGateway, ex.Message + Environment.NewLine );
                    }
                }
            }

            var summary = new StringBuilder();

            if ( exceptionMsgs.Any() )
            {
                summary.AppendLine( "\n<i class='fa fa-circle text-warning'></i> Some Financial Gateways have errors. See exception log for details." );
                summary.AppendLine();
            }

            foreach ( var item in processedPaymentsSummary )
            {
                summary.AppendLine( $"Summary for {item.Key.Name}:<br/>{item.Value}" );
            }

            this.Result = summary.ToString();

            if ( exceptionMsgs.Any() )
            {
                throw new RockJobWarningException( "One or more exceptions occurred while downloading transactions..." + Environment.NewLine + exceptionMsgs.AsDelimited( Environment.NewLine ) );
            }
        }
    }
}
