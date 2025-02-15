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
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rock.Constants;

namespace Rock.Web.UI.Controls
{
    /// <summary>
    /// A <see cref="T:System.Web.UI.WebControls.TextBox"/> control with an associated label.
    /// </summary>
    [ToolboxData( "<{0}:RockTextBox runat=server></{0}:RockTextBox>" )]
    public class RockTextBox : TextBox, IRockControl, IDisplayRequiredIndicator
    {
        private RegularExpressionValidator _regexValidator;

        #region IRockControl implementation

        /// <summary>
        /// Gets or sets the label text.
        /// </summary>
        /// <value>
        /// The label text.
        /// </value>
        [
        Bindable( true ),
        Category( "Appearance" ),
        DefaultValue( "" ),
        Description( "The text for the label." )
        ]
        public string Label
        {
            get { return ViewState["Label"] as string ?? string.Empty; }
            set { ViewState["Label"] = value; }
        }

        /// <summary>
        /// Gets or sets the form group class.
        /// </summary>
        /// <value>
        /// The form group class.
        /// </value>
        [
        Bindable( true ),
        Category( "Appearance" ),
        Description( "The CSS class to add to the form-group div." )
        ]
        public string FormGroupCssClass
        {
            get { return ViewState["FormGroupCssClass"] as string ?? string.Empty; }
            set { ViewState["FormGroupCssClass"] = value; }
        }

        /// <summary>
        /// Gets or sets the help text.
        /// </summary>
        /// <value>
        /// The help text.
        /// </value>
        [
        Bindable( true ),
        Category( "Appearance" ),
        DefaultValue( "" ),
        Description( "The help block." )
        ]
        public string Help
        {
            get
            {
                return HelpBlock != null ? HelpBlock.Text : string.Empty;
            }

            set
            {
                if ( HelpBlock != null )
                {
                    HelpBlock.Text = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the warning text.
        /// </summary>
        /// <value>
        /// The warning text.
        /// </value>
        [
        Bindable( true ),
        Category( "Appearance" ),
        DefaultValue( "" ),
        Description( "The warning block." )
        ]
        public string Warning
        {
            get
            {
                return WarningBlock != null ? WarningBlock.Text : string.Empty;
            }

            set
            {
                if ( WarningBlock != null )
                {
                    WarningBlock.Text = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RockTextBox"/> is required.
        /// </summary>
        /// <value>
        ///   <c>true</c> if required; otherwise, <c>false</c>.
        /// </value>
        [
        Bindable( true ),
        Category( "Behavior" ),
        DefaultValue( "false" ),
        Description( "Is the value required?" )
        ]
        public virtual bool Required
        {
            get { return ViewState["Required"] as bool? ?? false; }
            set { ViewState["Required"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show the Required indicator when Required=true
        /// </summary>
        /// <value>
        /// <c>true</c> if [display required indicator]; otherwise, <c>false</c>.
        /// </value>
        public bool DisplayRequiredIndicator
        {
            get { return ViewState["DisplayRequiredIndicator"] as bool? ?? true; }
            set { ViewState["DisplayRequiredIndicator"] = value; }
        }

        /// <summary>
        /// Gets or sets the required error message. If blank, the LabelName name will be used
        /// </summary>
        /// <value>
        /// The required error message.
        /// </value>
        public string RequiredErrorMessage
        {
            get
            {
                return RequiredFieldValidator != null ? RequiredFieldValidator.ErrorMessage : string.Empty;
            }

            set
            {
                if ( RequiredFieldValidator != null )
                {
                    RequiredFieldValidator.ErrorMessage = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RockTextBox"/> will allow emojis or special fonts. This property is meant to be used when dealing with Person names.
        /// </summary>
        /// <value>
        ///   <c>true</c> if emojis and special fonts are not allowed; otherwise, <c>false</c>.
        /// </value>
        [
        Bindable( true ),
        Category( "Behavior" ),
        DefaultValue( "false" ),
        Description( "Are emojis and special fonts allowed?" )
        ]
        public virtual bool NoEmojisOrSpecialFonts
        {
            get
            {
                return ViewState["NoEmojisOrSpecialFonts"] as bool? ?? false;
            }
            set
            {
                ViewState["NoEmojisOrSpecialFonts"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the no emojis or special fonts error message. If blank, the LabelName name will be used
        /// </summary>
        /// <value>
        /// The no emojis or special fonts error message.
        /// </value>
        public string NoEmojisOrSpecialFontsErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RockTextBox"/> will allow special characters. This property is meant to be used when dealing with Person names.
        /// </summary>
        /// <value>
        ///   <c>true</c> if special characters are not allowed; otherwise, <c>false</c>.
        /// </value>
        [
        Bindable( true ),
        Category( "Behavior" ),
        DefaultValue( "false" ),
        Description( "Are special characters allowed?" )
        ]
        public virtual bool NoSpecialCharacters
        {
            get
            {
                return ViewState["NoSpecialCharacters"] as bool? ?? false;
            }
            set
            {
                ViewState["NoSpecialCharacters"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the no special characters error message. If blank, the LabelName name will be used
        /// </summary>
        /// <value>
        /// The no special characters error message.
        /// </value>
        public string NoSpecialCharactersErrorMessage { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsValid
        {
            get
            {
                return ( !Required || RequiredFieldValidator == null || RequiredFieldValidator.IsValid ) && !( this.MaxLength != 0 && this.MaxLength < this.Text.Length );
            }
        }

        /// <summary>
        /// Gets or sets the help block.
        /// </summary>
        /// <value>
        /// The help block.
        /// </value>
        public HelpBlock HelpBlock { get; set; }

        /// <summary>
        /// Gets or sets the warning block.
        /// </summary>
        /// <value>
        /// The warning block.
        /// </value>
        public WarningBlock WarningBlock { get; set; }

        /// <summary>
        /// Gets or sets the required field validator.
        /// </summary>
        /// <value>
        /// The required field validator.
        /// </value>
        public RequiredFieldValidator RequiredFieldValidator { get; set; }

        /// <summary>
        /// Gets or sets the special characters field validator.
        /// </summary>
        /// <value>
        /// The special characters field validator.
        /// </value>
        public RegularExpressionValidator SpecialCharactersValidator { get; set; }

        /// <summary>
        /// Gets or sets the special characters field validator.
        /// </summary>
        /// <value>
        /// The special characters field validator.
        /// </value>
        public RegularExpressionValidator EmojiAndSpecialFontValidator { get; set; }

        /// <summary>
        /// Gets or sets the group of controls for which the <see cref="T:System.Web.UI.WebControls.TextBox" /> control causes validation when it posts back to the server.
        /// </summary>
        /// <returns>The group of controls for which the <see cref="T:System.Web.UI.WebControls.TextBox" /> control causes validation when it posts back to the server. The default value is an empty string ("").</returns>
        public override string ValidationGroup
        {
            get
            {
                return base.ValidationGroup;
            }

            set
            {
                base.ValidationGroup = value;

                EnsureChildControls();

                if ( RequiredFieldValidator != null )
                {
                    RequiredFieldValidator.ValidationGroup = value;
                }

                if ( _regexValidator != null )
                {
                    _regexValidator.ValidationGroup = value;
                }

                if ( SpecialCharactersValidator != null )
                {
                    SpecialCharactersValidator.ValidationGroup = value;
                }

                if ( EmojiAndSpecialFontValidator != null )
                {
                    EmojiAndSpecialFontValidator.ValidationGroup = value;
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the prepend text.
        /// </summary>
        /// <value>
        /// The prepend text.
        /// </value>
        [
        Bindable( false ),
        Category( "Appearance" ),
        DefaultValue( "" ),
        Description( "Text that appears prepended to the front of the text box." )
        ]
        public string PrependText
        {
            get { return ViewState["PrependText"] as string ?? string.Empty; }
            set { ViewState["PrependText"] = value; }
        }

        /// <summary>
        /// Gets or sets the append text.
        /// </summary>
        /// <value>
        /// The append text.
        /// </value>
        [
        Bindable( false ),
        Category( "Appearance" ),
        DefaultValue( "" ),
        Description( "Text that appears appended to the end of the text box." )
        ]
        public string AppendText
        {
            get { return ViewState["AppendText"] as string ?? string.Empty; }
            set { ViewState["AppendText"] = value; }
        }

        /// <summary>
        /// Gets or sets the placeholder text to display inside textbox when it is empty
        /// </summary>
        /// <value>
        /// The placeholder text
        /// </value>
        public string Placeholder
        {
            get { return ViewState["Placeholder"] as string ?? string.Empty; }
            set { ViewState["Placeholder"] = value; }
        }

        /// <summary>
        /// Gets or sets the behavior mode (single-line, multiline, or password) of the <see cref="T:System.Web.UI.WebControls.TextBox" /> control.
        /// NOTE: using TextMode=Password results in the password being included in HTML source on postback.  Make sure you are ok 
        /// with this before using this option!
        /// </summary>
        public override TextBoxMode TextMode
        {
            get
            {
                return base.TextMode;
            }

            set
            {
                base.TextMode = value;
            }
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        protected string Password
        {
            get
            {
                string password = ViewState["Password"] as string;
                return password ?? string.Empty;
            }

            set
            {
                ViewState["Password"] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show the count down
        /// </summary>
        /// <value>
        /// <c>true</c> if [show count down]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowCountDown
        {
            get { return ViewState["ShowCountDown"] as bool? ?? false; }

            set { ViewState["ShowCountDown"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the default text trim behaviour should be disabled
        /// </summary>
        /// <value>
        ///   <c>true</c> if [disable text trim]; otherwise, <c>false</c>.
        /// </value>
        public bool DisableTextTrim
        {
            get { return ViewState["DisableTextTrim"] as bool? ?? false; }
            set { ViewState["DisableTextTrim"] = value; }
        }

        #endregion

        private HiddenField _hfDisableVrm;

        /// <summary>
        /// Initializes a new instance of the <see cref="RockTextBox" /> class.
        /// </summary>
        public RockTextBox()
            : base()
        {
            RockControlHelper.Init( this );
        }

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            Controls.Clear();
            RockControlHelper.CreateChildControls( this, Controls );

            _hfDisableVrm = new HiddenField();
            _hfDisableVrm.ID = this.ID + "_dvrm";
            _hfDisableVrm.Value = "True";
            Controls.Add( _hfDisableVrm );

            _regexValidator = new RegularExpressionValidator();
            _regexValidator.ID = this.ID + "_LengthRE";
            _regexValidator.ControlToValidate = this.ID;
            _regexValidator.Display = ValidatorDisplay.Dynamic;
            _regexValidator.CssClass = "validation-error help-inline";
            _regexValidator.Enabled = false;
            Controls.Add( _regexValidator );

            SpecialCharactersValidator = new RegularExpressionValidator();
            SpecialCharactersValidator.ID = this.ID + "_SpecialCharRE";
            SpecialCharactersValidator.ControlToValidate = this.ID;
            SpecialCharactersValidator.Display = ValidatorDisplay.Dynamic;
            SpecialCharactersValidator.CssClass = "validation-error help-inline";
            SpecialCharactersValidator.Enabled = false;
            Controls.Add( SpecialCharactersValidator );

            EmojiAndSpecialFontValidator = new RegularExpressionValidator();
            EmojiAndSpecialFontValidator.ID = this.ID + "_EmojiAndSpecialFontRE";
            EmojiAndSpecialFontValidator.ControlToValidate = this.ID;
            EmojiAndSpecialFontValidator.Display = ValidatorDisplay.Dynamic;
            EmojiAndSpecialFontValidator.CssClass = "validation-error help-inline";
            EmojiAndSpecialFontValidator.Enabled = false;
            Controls.Add( EmojiAndSpecialFontValidator );
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( System.EventArgs e )
        {
            base.OnLoad( e );

            if ( this.Page.IsPostBack && this.TextMode == TextBoxMode.Password )
            {
                Password = this.Text;
            }
        }

        /// <summary>
        /// Outputs server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter" /> object and stores tracing information about the control if tracing is enabled.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter" /> object that receives the control content.</param>
        public override void RenderControl( HtmlTextWriter writer )
        {
            if ( this.Visible )
            {
                if ( this.MaxLength != 0  && this.ShowCountDown)
                {
                    writer.AddAttribute( "class", "pull-right badge" );
                    writer.AddAttribute( HtmlTextWriterAttribute.Id, this.ClientID + "_em" );
                    writer.RenderBeginTag( HtmlTextWriterTag.Em );
                    writer.RenderEndTag();
                }

                RockControlHelper.RenderControl( this, writer );
            }
        }

        /// <summary>
        /// Renders the base control.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public virtual void RenderBaseControl( HtmlTextWriter writer )
        {
            if ( this.TextMode == TextBoxMode.Password )
            {
                if ( this.Text == string.Empty && Password != string.Empty )
                {
                    this.Text = Password;
                }

                this.Attributes["value"] = this.Text;
            }

            // logic to add input groups for prepend and append labels
            bool renderInputGroup = false;
            string cssClass = this.CssClass;

            if ( !string.IsNullOrWhiteSpace( PrependText ) || !string.IsNullOrWhiteSpace( AppendText ) )
            {
                renderInputGroup = true;
            }

            if ( renderInputGroup )
            {
                writer.AddAttribute( "class", "input-group " + cssClass );
                if ( this.Style[HtmlTextWriterStyle.Display] == "none" )
                {
                    // render the display:none in the inputgroup div instead of the control itself
                    writer.AddStyleAttribute( HtmlTextWriterStyle.Display, "none" );
                    this.Style[HtmlTextWriterStyle.Display] = string.Empty;
                }

                writer.RenderBeginTag( HtmlTextWriterTag.Div );

                this.CssClass = string.Empty;
            }

            if ( !string.IsNullOrWhiteSpace( PrependText ) )
            {
                writer.AddAttribute( "class", "input-group-addon" );
                writer.RenderBeginTag( HtmlTextWriterTag.Span );
                writer.Write( PrependText );
                writer.RenderEndTag();
            }

            ( ( WebControl ) this ).AddCssClass( "form-control" );
            if ( !string.IsNullOrWhiteSpace( Placeholder ) )
            {
                this.Attributes["placeholder"] = Placeholder;
            }

            if ( ValidateRequestMode == System.Web.UI.ValidateRequestMode.Disabled )
            {
                _hfDisableVrm.RenderControl( writer );
            }

            base.RenderControl( writer );

            if ( !string.IsNullOrWhiteSpace( AppendText ) )
            {
                writer.AddAttribute( "class", "input-group-addon" );
                writer.RenderBeginTag( HtmlTextWriterTag.Span );
                writer.Write( AppendText );
                writer.RenderEndTag();
            }

            if ( renderInputGroup )
            {
                writer.RenderEndTag();  // input-group
                this.CssClass = cssClass;
            }

            RenderDataValidator( writer );

            if ( this.MaxLength != 0 && this.ShowCountDown )
            {
                string scriptFormat = string.Format( @"$('#{0}').limit({{maxChars: {1}, counter:'#{2}', normalClass:'badge', warningClass:'badge-warning', overLimitClass: 'badge-danger'}});", this.ClientID, this.MaxLength, this.ClientID + "_em" );
                ScriptManager.RegisterStartupScript( this, this.GetType(), "MaxLengthScript_" + this.ClientID, scriptFormat, true );
            }
        }

        /// <summary>
        /// Renders any data validator.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected virtual void RenderDataValidator( HtmlTextWriter writer )
        {
            if ( this.MaxLength != 0
                 && this.TextMode == TextBoxMode.MultiLine
                 && _regexValidator != null )
            { 
                _regexValidator.Enabled = true;
                _regexValidator.ValidationExpression = @"^((.|\n){0," + this.MaxLength.ToString() + "})$";

                /*
                    8/14/2021 - CWR

                    The TextLengthInvalid method expects a plain, text-only Label, not one with markup or extra spaces.
                    The BulkUpdate page (and possibly others) add HTML to the Label, which TextLengthInvalid does not expect.
                    It will affect the resulting page's style and structure, as noted below from GitHub.
                    Therefore, we will strip HTML out and trim whitespace before passing the label on to the 'length invalid' message.

                    Reason: GitHub Issue #4231 (https://github.com/SparkDevNetwork/Rock/issues/4231)
                */
                _regexValidator.ErrorMessage = Rock.Constants.WarningMessage.TextLengthInvalid( this.Label.StripHtml().Trim(), this.MaxLength );
                _regexValidator.ValidationGroup = this.ValidationGroup;
                _regexValidator.RenderControl( writer );
            }

            if ( SpecialCharactersValidator != null && NoSpecialCharacters )
            {
                SpecialCharactersValidator.Enabled = true;
                SpecialCharactersValidator.ValidationExpression = RegexPatterns.SpecialCharacterPattern;
                if ( NoSpecialCharactersErrorMessage.IsNullOrWhiteSpace() )
                {
                    SpecialCharactersValidator.ErrorMessage = $"{this.Label} cannot contain special characters such as quotes, parentheses, etc.";
                }
                else
                {
                    SpecialCharactersValidator.ErrorMessage = NoSpecialCharactersErrorMessage;
                }
                SpecialCharactersValidator.ValidationGroup = this.ValidationGroup;
                SpecialCharactersValidator.RenderControl( writer );
            }

            if ( EmojiAndSpecialFontValidator != null && NoEmojisOrSpecialFonts )
            {
                EmojiAndSpecialFontValidator.Enabled = true;
                EmojiAndSpecialFontValidator.ValidationExpression = RegexPatterns.EmojiAndSpecialFontPattern;
                if ( NoEmojisOrSpecialFontsErrorMessage.IsNullOrWhiteSpace() )
                {
                    EmojiAndSpecialFontValidator.ErrorMessage = $"{this.Label} cannot contain emojis or special fonts.";
                }
                else
                {
                    EmojiAndSpecialFontValidator.ErrorMessage = NoEmojisOrSpecialFontsErrorMessage;
                }
                EmojiAndSpecialFontValidator.ValidationGroup = this.ValidationGroup;
                EmojiAndSpecialFontValidator.RenderControl( writer );
            }
        }

        /// <summary>
        /// Shows the error message.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        public virtual void ShowErrorMessage( string errorMessage )
        {
            RequiredFieldValidator.ErrorMessage = errorMessage;
            RequiredFieldValidator.IsValid = false;
        }

        /// <summary>
        /// Clears the password.
        /// </summary>
        public void ClearPassword()
        {
            Password = string.Empty;
            this.Text = string.Empty;
            ViewState["Password"] = null;
        }

        /// <summary>
        /// Gets or sets the text content of the <see cref="T:System.Web.UI.WebControls.TextBox" /> control.
        /// </summary>
        /// <returns>The text displayed in the <see cref="T:System.Web.UI.WebControls.TextBox" /> control. The default is an empty string ("").</returns>
        public override string Text
        {
            get
            {
                if ( base.Text == null )
                {
                    return null;
                }
                else
                {
                    if ( DisableTextTrim )
                    {
                        return base.Text;
                    }
                    return base.Text.Trim();
                }
            }

            set
            {
                base.Text = value;
            }
        }

        /// <summary>
        /// Gets the untrimmed text.
        /// </summary>
        /// <value>
        /// The untrimmed text.
        /// </value>
        public string UntrimmedText
        {
            get
            {
                return base.Text;
            }
        }
    }
}