// <copyright>
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

using Rock.Attribute;
using Rock.Model;
using Rock.Obsidian.UI;
using Rock.ViewModels.Blocks;
using Rock.ViewModels.Blocks.Reporting.PageParameterFilter;
using Rock.ViewModels.Cms;
using Rock.ViewModels.Utility;
using Rock.Web.Cache;

namespace Rock.Blocks.Reporting
{
    /// <summary>
    /// Filter block that passes the filter values as query string parameters.
    /// </summary>

    [DisplayName( "Page Parameter Filter" )]
    [Category( "Reporting" )]
    [Description( "Filter block that passes the filter values as query string parameters." )]
    //[SupportedSiteTypes( Model.SiteType.Web )]

    #region Block Attributes

    [TextField( "Block Title Text",
        Key = AttributeKey.BlockTitleText,
        Description = "The text to display as the block title.",
        Category = "CustomSetting",
        DefaultValue = "" )]

    [TextField( "Block Title Icon CSS Class",
        Key = AttributeKey.BlockTitleIconCssClass,
        Description = "The CSS class name to use for the block title icon.",
        Category = "CustomSetting",
        DefaultValue = "fa fa-filter" )]

    [BooleanField( "Show Block Title",
        Key = AttributeKey.ShowBlockTitle,
        Description = "Determines if the block title should be displayed.",
        Category = "CustomSetting",
        DefaultBooleanValue = true )]

    [TextField( "Filter Button Text",
        Key = AttributeKey.FilterButtonText,
        Description = "The text to display on the filter button.",
        Category = "CustomSetting",
        DefaultValue = "Filter" )]

    [CustomDropdownListField( "Filter Button Size",
        Key = AttributeKey.FilterButtonSize,
        Description = "The size of the filter button.",
        Category = "CustomSetting",
        ListSource = "1^Normal,2^Small,3^Extra Small",
        DefaultValue = "3" )]

    [BooleanField( "Show Filter Button",
        Key = AttributeKey.ShowFilterButton,
        Description = "Determines if the filter button should be displayed.",
        Category = "CustomSetting",
        DefaultBooleanValue = true )]

    [BooleanField( "Show Reset Filters Button",
        Key = AttributeKey.ShowResetFiltersButton,
        Description = "Determines if the reset filters button should be displayed.",
        Category = "CustomSetting",
        DefaultBooleanValue = true )]

    [IntegerField( "Filters Per Row",
        Key = AttributeKey.FiltersPerRow,
        Description = "The number of filters to display per row. Maximum is 12.",
        Category = "CustomSetting",
        DefaultIntegerValue = 2 )]

    [LinkedPage( "Redirect Page",
        Key = AttributeKey.RedirectPage,
        Description = "If set, the filter button will redirect to the selected page.",
        Category = "CustomSetting",
        DefaultValue = "" )]

    //[CustomDropdownListField( "Selection Action",
    //    Key = AttributeKey.DoesSelectionCausePostback,
    //    Description = "Specifies what should happen when a value is changed. Nothing, update page, or update block.",
    //    Category = "CustomSetting",
    //    ListSource = "nothing^,block^Update Block,page^Update Page",
    //    DefaultValue = "nothing" )]

    #endregion Block Attributes

    [Rock.SystemGuid.EntityTypeGuid( "59F94307-B2B0-4383-9C2C-88A4E154C461" )]
    [Rock.SystemGuid.BlockTypeGuid( "842DFBC2-DA38-465D-BFD2-B6C8585AA3BF" )]
    public class PageParameterFilter : RockBlockType, IHasCustomActions
    {
        #region Legacy Block Checklist

        //#region Enums
        //private enum SelectionAction
        //{
        //    Nothing = 0,
        //    UpdateBlock = 1,
        //    UpdatePage = 2,
        //}
        //#endregion

        //#region Properties
        //protected Dictionary<string, object> CurrentParameters { get; set; }
        //#endregion

        //#region Fields

        //private int _blockTypeEntityId;
        //private Block _block;
        //private int _filtersPerRow;
        //private SelectionAction _reloadOnSelection;

        //#endregion

        //#region Base Control Methods

        ///// <summary>
        ///// Restores the view-state information from a previous user control request that was saved by the <see cref="M:System.Web.UI.UserControl.SaveViewState" /> method.
        ///// </summary>
        ///// <param name="savedState">An <see cref="T:System.Object" /> that represents the user control state to be restored.</param>
        //protected override void LoadViewState( object savedState )
        //{
        //    base.LoadViewState( savedState );

        //    CurrentParameters = ViewState["CurrentParameters"] as Dictionary<string, object>;
        //}

        ///// <summary>
        ///// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        ///// </summary>
        ///// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        //protected override void OnInit( EventArgs e )
        //{
        //    base.OnInit( e );

        //    _reloadOnSelection = GetSelectAction();

        //    // This is needed so that we can get the data from the controls after all control events
        //    // have run so that their values are updated.
        //    Page.LoadComplete += Page_LoadComplete;
        //}

        ///// <summary>
        ///// Handles the LoadComplete event of the Page control.
        ///// </summary>
        ///// <param name="sender">The source of the event.</param>
        ///// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        //private void Page_LoadComplete( object sender, EventArgs e )
        //{
        //    // Add postback controls
        //    if ( Page.IsPostBack )
        //    {
        //        if ( _reloadOnSelection == SelectionAction.UpdateBlock )
        //        {
        //            // See if hidden field has 'true' already set
        //            if ( hfPostBack.Value.IsNullOrWhiteSpace() )
        //            {
        //                var control = Page.FindControl( Request.Form["__EVENTTARGET"] );
        //                if ( control != null && control.UniqueID.Contains( "attribute_field_" ) )
        //                {
        //                    // We need to update the form action so that the partial postback call post to the new parameterized URL.
        //                    Page.Form.Action = GetParameterizedUrl();
        //                    hfPostBack.Value = "True";
        //                    ScriptManager.RegisterStartupScript( control, control.GetType(), "Refresh-Controls", @"console.log('Doing Postback');  __doPostBack('" + Request.Form["__EVENTTARGET"] + @"','');", true );
        //                }
        //            }
        //            else
        //            {
        //                // Reset hidden field for next time.
        //                hfPostBack.Value = string.Empty;
        //            }
        //        }
        //        else if ( _reloadOnSelection == SelectionAction.UpdatePage )
        //        {
        //            var control = Page.FindControl( Request.Form["__EVENTTARGET"] );
        //            if ( control != null && control.UniqueID.Contains( "attribute_field_" ) )
        //            {
        //                btnFilter_Click( null, null );
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        ///// </summary>
        ///// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        //protected override void OnLoad( EventArgs e )
        //{
        //    if ( !Page.IsPostBack )
        //    {
        //        CurrentParameters = this.RockPage.PageParameters();

        //        // Get list of attributes with default values (4/12/2022 JME replaces code that read
        //        // this from the DB with the call below that reads from cache.
        //        var attribsWithDefaultValue = AttributeCache.AllForEntityType( _blockTypeEntityId )
        //            .Where( a =>
        //                a.EntityTypeQualifierColumn == "Id"
        //                && a.EntityTypeQualifierValue == _block.Id.ToString()
        //                && a.DefaultValue != null
        //                && a.DefaultValue != string.Empty )
        //            .ToList();

        //        // If we have any filters with default values, we want to load this block with the page parameters already set.
        //        if ( attribsWithDefaultValue.Any() && !this.RockPage.PageParameters().Any() )
        //        {
        //            ResetFilters();
        //        }
        //        else
        //        {
        //            LoadFilters();
        //        }
        //    }
        //    else
        //    {
        //        LoadFilters();
        //    }

        //    base.OnLoad( e );
        //}

        //protected override object SaveViewState()
        //{
        //    ViewState["CurrentParameters"] = CurrentParameters;

        //    return base.SaveViewState();
        //}
        //#endregion

        //#region Settings

        ///// <summary>
        ///// Shows the settings.
        ///// </summary>
        //protected override void ShowSettings()
        //{
        //    ddlSelectionAction.SelectedValue = GetSelectAction().ConvertToInt().ToString();
        //}

        ///// <summary>
        ///// Handles the SaveClick event of the mdSettings control.
        ///// </summary>
        ///// <param name="sender">The source of the event.</param>
        ///// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        //protected void mdSettings_SaveClick( object sender, EventArgs e )
        //{
        //    SetAttributeValue( AttributeKey.DoesSelectionCausePostback, ddlSelectionAction.SelectedValue );

        //    // reload the page to make sure we have a clean load
        //    ResetFilters();
        //    NavigateToCurrentPageReference();
        //}

        //#endregion Settings

        //#region Events

        ///// <summary>
        ///// Handles the BlockUpdated event of the control.
        ///// </summary>
        ///// <param name="sender">The source of the event.</param>
        ///// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        //protected void Block_BlockUpdated( object sender, EventArgs e )
        //{
        //    LoadFilters();
        //}

        ///// <summary>
        ///// Handles the btnFilter event.
        ///// </summary>
        ///// <param name="sender">The source of the event.</param>
        ///// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        //protected void btnFilter_Click( object sender, EventArgs e )
        //{
        //    Response.Redirect( GetParameterizedUrl(), false );
        //}

        ///// <summary>
        ///// Handles the btnResetFilters event.
        ///// </summary>
        ///// <param name="sender">The source of the event.</param>
        ///// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        //protected void btnResetFilters_Click( object sender, EventArgs e )
        //{
        //    ResetFilters();
        //}

        ///// <summary>
        ///// Handles the SelectItem event from an ItemPicker (fake event to register the postback)
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void FilterControl_ItemChanged( object sender, EventArgs e )
        //{
        //    // Hopefully an xhr happens here
        //}

        //#endregion

        //#region Methods

        ///// <summary>
        ///// Loads the filters.
        ///// </summary>
        //private void LoadFilters()
        //{
        //    BuildControls();

        //    if ( _block.Attributes != null )
        //    {
        //        foreach ( var attribute in _block.Attributes )
        //        {
        //            var attributeCache = AttributeCache.Get( attribute.Value.Guid );

        //            Control control = phAttributes.FindControl( string.Format( "attribute_field_{0}", attribute.Value.Id ) );
        //            if ( control != null )
        //            {
        //                var value = PageParameter( attribute.Key );
        //                if ( value.IsNotNullOrWhiteSpace() )
        //                {
        //                    attributeCache.FieldType.Field.SetEditValue( control, null, value );
        //                }
        //                else if ( attribute.Value.DefaultValue.IsNotNullOrWhiteSpace() )
        //                {
        //                    attributeCache.FieldType.Field.SetEditValue( control, null, attribute.Value.DefaultValue );
        //                }

        //                // Enable ListControls postback and Event
        //                if ( control is ListControl listControl && _reloadOnSelection != SelectionAction.Nothing )
        //                {
        //                    listControl.AutoPostBack = true;
        //                }

        //                // Enable ItemPicker postback event
        //                if ( control is ItemPicker itemPicker && _reloadOnSelection != SelectionAction.Nothing )
        //                {
        //                    itemPicker.SelectItem += FilterControl_ItemChanged;
        //                }

        //                // Enable Toggle postback event
        //                if ( control is Toggle toggle && _reloadOnSelection != SelectionAction.Nothing )
        //                {
        //                    toggle.CheckedChanged += FilterControl_ItemChanged;
        //                }
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// Resets the filters to their original state.  Any filters with default values will be set as well.
        ///// </summary>
        //private void ResetFilters()
        //{
        //    BuildControls();

        //    NameValueCollection queryString = GenerateQueryString();

        //    // 4/12/2022 JME
        //    // Updated the redirects to set the endResponse = true (was false). This prevents
        //    // child blocks from fully loading, redirecting and loading again. The child blocks
        //    // are typically SQL so that could mean a very slow initial page load as they would
        //    // be run twice. Not sure why these were originally set to false. 
        //    if ( queryString.AllKeys.Any() )
        //    {
        //        Response.Redirect( $"{Request.UrlProxySafe().AbsolutePath}?{queryString}", true );
        //    }
        //    else
        //    {
        //        Response.Redirect( Request.UrlProxySafe().AbsolutePath, true );
        //    }
        //}

        ///// <summary>
        ///// Generates the query string.
        ///// </summary>
        ///// <returns></returns>
        //private NameValueCollection GenerateQueryString()
        //{
        //    var queryString = HttpUtility.ParseQueryString( String.Empty );

        //    // Don't create a query string if the block's page does not match the current page. This
        //    // would be the case when editing the settings from 'Admin Tools > CMS Settings > Pages'.
        //    // Without this check the block would thrown an exception as CurrentParameters would be
        //    // null. This may not be the _best_ place for this check, but the correct change may
        //    // need a major refactor.
        //    if ( RockPage.PageId != BlockCache.PageId )
        //    {
        //        return queryString;
        //    }

        //    foreach ( var parameter in CurrentParameters )
        //    {
        //        if ( parameter.Key != "PageId" )
        //        {
        //            queryString.Set( parameter.Key, parameter.Value.ToString() );
        //        }
        //    }

        //    _block.LoadAttributes( new RockContext() );

        //    if ( _block.Attributes != null )
        //    {
        //        foreach ( var attribute in _block.Attributes )
        //        {
        //            Control control = phAttributes.FindControl( string.Format( "attribute_field_{0}", attribute.Value.Id ) );
        //            if ( control != null )
        //            {
        //                string value = attribute.Value.FieldType.Field.GetEditValue( control, attribute.Value.QualifierValues );

        //                // If there is no value use the attribute's default value
        //                if ( value.IsNullOrWhiteSpace() )
        //                {
        //                    value = attribute.Value.DefaultValue;
        //                }

        //                if ( value.IsNotNullOrWhiteSpace() )
        //                {
        //                    queryString.Set( attribute.Key, value );
        //                    CurrentPageReference.Parameters.AddOrReplace( attribute.Key, value );
        //                }
        //                else
        //                {
        //                    queryString.Remove( attribute.Key );
        //                    CurrentPageReference.Parameters.Remove( attribute.Key );
        //                }
        //            }
        //        }
        //    }

        //    return queryString;
        //}

        ///// <summary>
        ///// Gets the parameterized URL.
        ///// </summary>
        ///// <returns></returns>
        //private string GetParameterizedUrl()
        //{
        //    var queryString = GenerateQueryString();
        //    var url = Request.UrlProxySafe().AbsolutePath;

        //    var pageGuid = GetAttributeValue( AttributeKey.RedirectPage ).AsGuidOrNull();
        //    if ( pageGuid.HasValue )
        //    {
        //        var page = PageCache.Get( pageGuid.Value );

        //        url = VirtualPathUtility.ToAbsolute( string.Format( "~/page/{0}", page.Id ) );
        //    }

        //    return queryString.AllKeys.Any() ? $"{url}?{queryString}" : url;
        //}

        ///// <summary>
        ///// Gets the select action.
        ///// </summary>
        ///// <returns></returns>
        //private SelectionAction GetSelectAction()
        //{
        //    var attributeValue = GetAttributeValue( AttributeKey.DoesSelectionCausePostback );
        //    if ( attributeValue.IsNullOrWhiteSpace() )
        //    {
        //        return SelectionAction.Nothing;
        //    }

        //    if ( attributeValue.Equals( "false", StringComparison.InvariantCultureIgnoreCase ) )
        //    {
        //        return SelectionAction.Nothing;
        //    }

        //    if ( attributeValue.Equals( "true", StringComparison.InvariantCultureIgnoreCase ) )
        //    {
        //        return SelectionAction.UpdateBlock;
        //    }

        //    return attributeValue.ConvertToEnum<SelectionAction>();
        //}
        //#endregion

        #endregion Legacy Block Checklist

        #region Keys

        private static class AttributeKey
        {
            public const string BlockTitleText = "BlockTitleText";
            public const string BlockTitleIconCssClass = "BlockTitleIconCSSClass";
            public const string ShowBlockTitle = "ShowBlockTitle";
            public const string FilterButtonText = "FilterButtonText";
            public const string FilterButtonSize = "FilterButtonSize";
            public const string ShowFilterButton = "ShowFilterButton";
            public const string ShowResetFiltersButton = "ShowResetFiltersButton";
            public const string FiltersPerRow = "FiltersPerRow";
            public const string RedirectPage = "RedirectPage";
            //public const string DoesSelectionCausePostback = "DoesSelectionCausePostback";
        }

        private static class NavigationUrlKey
        {
            public const string RedirectPage = "RedirectPage";
        }

        #endregion Keys

        #region Fields

        private List<ListItemBag> _filterButtonSizeItems;

        #endregion Fields

        #region Properties

        private List<ListItemBag> FilterButtonSizeItems
        {
            get
            {
                if ( _filterButtonSizeItems == null )
                {
                    _filterButtonSizeItems = new List<ListItemBag>
                    {
                        FilterButtonSize.Normal,
                        FilterButtonSize.Small,
                        FilterButtonSize.ExtraSmall
                    };
                }

                return _filterButtonSizeItems;
            }
        }

        private string DefaultFilterButtonSize => FilterButtonSize.ExtraSmall.Value;

        private int DefaultFiltersPerRow => 2;

        private int AttributeEntityTypeId => EntityTypeCache.Get( Rock.SystemGuid.EntityType.ATTRIBUTE ).Id;

        private int BlockEntityTypeId => EntityTypeCache.Get( Rock.SystemGuid.EntityType.BLOCK ).Id;

        #endregion Properties

        #region Methods

        /// <inheritdoc/>
        public override object GetObsidianBlockInitialization()
        {
            var customSettingsBox = GetCustomSettingsBox();
            var settings = customSettingsBox.Settings;

            var box = new PageParameterFilterInitializationBox
            {
                BlockTitleText = settings.BlockTitleText,
                BlockTitleIconCssClass = settings.BlockTitleIconCssClass,
                ShowBlockTitle = settings.ShowBlockTitle,
                FilterButtonText = settings.FilterButtonText,
                FilterButtonSize = settings.FilterButtonSize,
                ShowFilterButton = settings.ShowFilterButton,
                ShowResetFiltersButton = settings.ShowResetFiltersButton,
                FiltersPerRow = settings.FiltersPerRow,
                Filters = GetPageParameterFilters(),
                FilterValueDefaults = GetPageParameterFilterValueDefaults(),
                SecurityGrantToken = customSettingsBox.SecurityGrantToken,
                NavigationUrls = GetBoxNavigationUrls()
            };

            return box;
        }

        /// <summary>
        /// Gets the custom settings box that contains the current block settings and available options.
        /// </summary>
        /// <returns>The custom settings box that contains the current block settings and available options.</returns>
        private CustomSettingsBox<PageParameterFilterCustomSettingsBag, PageParameterFilterCustomSettingsOptionsBag> GetCustomSettingsBox()
        {
            var options = new PageParameterFilterCustomSettingsOptionsBag
            {
                FilterButtonSizeItems = this.FilterButtonSizeItems,
                FiltersGridDefinition = GetFiltersGridBuilder().BuildDefinition(),
                FiltersReservedKeyNames = GetFiltersReservedKeyNames()
            };

            var filterButtonSize = GetAttributeValue( AttributeKey.FilterButtonSize );
            if ( !this.FilterButtonSizeItems.Any( s => s.Value == filterButtonSize ) )
            {
                filterButtonSize = this.DefaultFilterButtonSize;
            }

            var filtersPerRow = GetAttributeValue( AttributeKey.FiltersPerRow ).AsIntegerOrNull();
            if ( !filtersPerRow.HasValue || filtersPerRow.Value < 1 || filtersPerRow.Value > 12 )
            {
                filtersPerRow = this.DefaultFiltersPerRow;
            }

            var settings = new PageParameterFilterCustomSettingsBag
            {
                BlockTitleText = GetAttributeValue( AttributeKey.BlockTitleText ),
                BlockTitleIconCssClass = GetAttributeValue( AttributeKey.BlockTitleIconCssClass ),
                ShowBlockTitle = GetAttributeValue( AttributeKey.ShowBlockTitle ).AsBoolean( true ),
                FilterButtonText = GetAttributeValue( AttributeKey.FilterButtonText ),
                FilterButtonSize = filterButtonSize,
                ShowFilterButton = GetAttributeValue( AttributeKey.ShowFilterButton ).AsBoolean( true ),
                ShowResetFiltersButton = GetAttributeValue( AttributeKey.ShowResetFiltersButton ).AsBoolean( true ),
                FiltersPerRow = filtersPerRow.Value,
                RedirectPage = GetAttributeValue( AttributeKey.RedirectPage ).ToPageRouteValueBag()
            };

            return new CustomSettingsBox<PageParameterFilterCustomSettingsBag, PageParameterFilterCustomSettingsOptionsBag>
            {
                Settings = settings,
                Options = options,
                SecurityGrantToken = new Rock.Security.SecurityGrant().ToToken()
            };
        }

        /// <summary>
        /// Gets the reserved key names for the current list of filters.
        /// </summary>
        /// <param name="ignoreFilterGuid">The optional filter unique identifier whose key should be ignored.</param>
        /// <returns>The reserved key names for the current list of filters.</returns>
        private List<string> GetFiltersReservedKeyNames( Guid? ignoreFilterGuid = null )
        {
            return AttributeCache.AllForEntityType( this.BlockEntityTypeId )
                .Where( a =>
                    a.EntityTypeQualifierColumn == "Id"
                    && a.EntityTypeQualifierValue == this.BlockId.ToString()
                    && (
                        !ignoreFilterGuid.HasValue
                        || ignoreFilterGuid.Equals( Guid.Empty )
                        || !a.Guid.Equals( ignoreFilterGuid.Value )
                    )
                )
                .Select( a => a.Key )
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// Gets the grid builder for the filters grid.
        /// </summary>
        /// <returns>The grid builder for the filters grid.</returns>
        private GridBuilder<Model.Attribute> GetFiltersGridBuilder()
        {
            return new GridBuilder<Model.Attribute>()
                .AddField( "guid", a => a.Guid )
                .AddTextField( "name", a => a.Name )
                .AddTextField( "description", a => a.Description )
                .AddTextField( "filterType", a => FieldTypeCache.Get( a.FieldTypeId )?.ToString() )
                .AddTextField( "defaultValue", a =>
                {
                    //return ExtensionMethods.GetAttributeCondensedHtmlValue( this.BlockCache, a.Key );
                    return "TODO...";
                } );
        }

        /// <summary>
        /// Gets the page parameter filters available for selection.
        /// </summary>
        /// <returns>The page parameter filters available for selection.</returns>
        private Dictionary<string, PublicAttributeBag> GetPageParameterFilters()
        {
            var orderedFilters = GetOrderedFilters( shouldTrackChanges: false );

            return this.BlockCache.GetPublicAttributesForEdit( this.RequestContext.CurrentPerson, enforceSecurity: true, a => orderedFilters.Any( f => f.Key == a.Key ) );
        }

        /// <summary>
        /// Gets the page parameter filter value defaults.
        /// </summary>
        /// <returns>The page parameter filter value defaults.
        private Dictionary<string, string> GetPageParameterFilterValueDefaults()
        {
            var orderedFilters = GetOrderedFilters( shouldTrackChanges: false );

            return this.BlockCache.GetPublicAttributeValuesForEdit( this.RequestContext.CurrentPerson, enforceSecurity: true, a => orderedFilters.Any( f => f.Key == a.Key ) );
        }

        /// <summary>
        /// Gets the ordered filters.
        /// </summary>
        /// <param name="shouldTrackChanges">Whether Entity Framework should track changes.</param>
        /// <returns>The ordered filters.</returns>
        private List<Model.Attribute> GetOrderedFilters( bool shouldTrackChanges )
        {
            var filtersQry = new AttributeService( this.RockContext )
                .Get( this.BlockEntityTypeId, "Id", this.BlockId.ToString() );

            if ( !shouldTrackChanges )
            {
                filtersQry = filtersQry.AsNoTracking();
            }

            return filtersQry
                .OrderBy( a => a.Order )
                .ToList();
        }

        /// <summary>
        /// Gets the box navigation URLs required for the page to operate.
        /// </summary>
        /// <returns>A dictionary of key names and URL values.</returns>
        private Dictionary<string, string> GetBoxNavigationUrls()
        {
            var urls = new Dictionary<string, string>();

            var redirectPage = GetAttributeValue( AttributeKey.RedirectPage );
            if ( redirectPage.IsNotNullOrWhiteSpace() )
            {
                urls.Add( NavigationUrlKey.RedirectPage, this.GetLinkedPageUrl( AttributeKey.RedirectPage ) );
            }

            return urls;
        }

        #endregion Methods

        #region Block Actions

        /// <summary>
        /// Gets the values and all other required details that will be needed to display the custom settings modal.
        /// </summary>
        /// <returns>A box that contains the custom settings values and additional data.</returns>
        [BlockAction]
        public BlockActionResult GetCustomSettings()
        {
            if ( !this.BlockCache.IsAuthorized( Rock.Security.Authorization.ADMINISTRATE, this.RequestContext.CurrentPerson ) )
            {
                return ActionForbidden( "Not authorized to edit block settings." );
            }

            var customSettingsBox = GetCustomSettingsBox();

            return ActionOk( customSettingsBox );
        }

        /// <summary>
        /// Saves the updates to the custom setting values for this block, for the custom settings modal.
        /// </summary>
        /// <param name="box">The box that contains the setting values.</param>
        /// <returns>A response that indicates if the save was successful or not.</returns>
        [BlockAction]
        public BlockActionResult SaveCustomSettings( CustomSettingsBox<PageParameterFilterCustomSettingsBag, PageParameterFilterCustomSettingsOptionsBag> box )
        {
            if ( !this.BlockCache.IsAuthorized( Rock.Security.Authorization.ADMINISTRATE, this.RequestContext.CurrentPerson ) )
            {
                return ActionForbidden( "Not authorized to edit block settings." );
            }

            var block = new BlockService( this.RockContext ).Get( this.BlockId );
            block.LoadAttributes( this.RockContext );

            box.IfValidProperty( nameof( box.Settings.BlockTitleText ),
                () => block.SetAttributeValue( AttributeKey.BlockTitleText, box.Settings.BlockTitleText ) );

            box.IfValidProperty( nameof( box.Settings.BlockTitleIconCssClass ),
                () => block.SetAttributeValue( AttributeKey.BlockTitleIconCssClass, box.Settings.BlockTitleIconCssClass ) );

            box.IfValidProperty( nameof( box.Settings.ShowBlockTitle ),
                () => block.SetAttributeValue( AttributeKey.ShowBlockTitle, box.Settings.ShowBlockTitle.ToString() ) );

            box.IfValidProperty( nameof( box.Settings.FilterButtonText ),
                () => block.SetAttributeValue( AttributeKey.FilterButtonText, box.Settings.FilterButtonText ) );

            box.IfValidProperty( nameof( box.Settings.FilterButtonSize ), () =>
                {
                    if ( !this.FilterButtonSizeItems.Any( s => s.Value == box.Settings.FilterButtonSize ) )
                    {
                        box.Settings.FilterButtonSize = this.DefaultFilterButtonSize;
                    }

                    block.SetAttributeValue( AttributeKey.FilterButtonSize, box.Settings.FilterButtonSize );
                } );

            box.IfValidProperty( nameof( box.Settings.ShowFilterButton ),
                () => block.SetAttributeValue( AttributeKey.ShowFilterButton, box.Settings.ShowFilterButton.ToString() ) );

            box.IfValidProperty( nameof( box.Settings.ShowResetFiltersButton ),
                () => block.SetAttributeValue( AttributeKey.ShowResetFiltersButton, box.Settings.ShowResetFiltersButton.ToString() ) );

            box.IfValidProperty( nameof( box.Settings.FiltersPerRow ), () =>
            {
                if ( box.Settings.FiltersPerRow < 1 || box.Settings.FiltersPerRow > 12 )
                {
                    box.Settings.FiltersPerRow = this.DefaultFiltersPerRow;
                }

                block.SetAttributeValue( AttributeKey.FiltersPerRow, box.Settings.FiltersPerRow.ToString() );
            } );

            box.IfValidProperty( nameof( box.Settings.RedirectPage ),
                () => block.SetAttributeValue( AttributeKey.RedirectPage, box.Settings.RedirectPage.ToCommaDelimitedPageRouteValues() ) );

            block.SaveAttributeValues( this.RockContext );

            BlockCache.FlushItem( this.BlockId );

            return ActionOk();
        }

        /// <summary>
        /// Gets the filters grid row data, for the custom settings modal.
        /// </summary>
        /// <returns>A bag containing the filters grid row data.</returns>
        [BlockAction]
        public BlockActionResult GetFiltersGridRowData()
        {
            if ( !this.BlockCache.IsAuthorized( Rock.Security.Authorization.ADMINISTRATE, this.RequestContext.CurrentPerson ) )
            {
                return ActionForbidden( "Not authorized to edit block settings." );
            }

            var attributes = GetOrderedFilters( shouldTrackChanges: false );
            var builder = GetFiltersGridBuilder();
            var gridDataBag = builder.Build( attributes );

            return ActionOk( gridDataBag );
        }

        /// <summary>
        /// Gets the information needed to add new or edit existing filter, for the custom settings modal.
        /// </summary>
        /// <param name="filterGuid">The unique identifier of the filter to edit or <see langword="null"/> if adding a new filter.</param>
        /// <returns>A bag containing the editable filter and/or the current reserved key names.</returns>
        [BlockAction]
        public BlockActionResult AddOrEditFilter( Guid? filterGuid )
        {
            if ( !this.BlockCache.IsAuthorized( Rock.Security.Authorization.ADMINISTRATE, this.RequestContext.CurrentPerson ) )
            {
                return ActionForbidden( "Not authorized to edit block settings." );
            }

            var editableFilter = new EditableFilterBag
            {
                FiltersReservedKeyNames = GetFiltersReservedKeyNames( filterGuid )
            };

            if ( filterGuid.HasValue && !filterGuid.Equals( Guid.Empty ) )
            {
                var attribute = new AttributeService( this.RockContext ).Get( filterGuid.Value );
                if ( attribute == null )
                {
                    return ActionBadRequest();
                }

                editableFilter.Filter = PublicAttributeHelper.GetPublicEditableAttributeViewModel( attribute );
            }

            return ActionOk( editableFilter );
        }

        /// <summary>
        /// Saves the filter, for the custom settings modal.
        /// </summary>
        /// <param name="bag">The information needed to save the filter.</param>
        /// <returns>A grid row object containing information about the saved filter.</returns>
        [BlockAction]
        public BlockActionResult SaveFilter( PublicEditableAttributeBag bag )
        {
            if ( !this.BlockCache.IsAuthorized( Rock.Security.Authorization.ADMINISTRATE, this.RequestContext.CurrentPerson ) )
            {
                return ActionForbidden( "Not authorized to edit block settings." );
            }

            if ( bag == null )
            {
                return ActionBadRequest();
            }

            // Prevent duplicate key values.
            var filtersReservedKeyNames = GetFiltersReservedKeyNames( bag.Guid );
            if ( filtersReservedKeyNames.Contains( bag.Key.Trim(), StringComparer.CurrentCultureIgnoreCase ) )
            {
                return ActionBadRequest( "There is already a filter with the key value you entered. Please enter a different key." );
            }

            // Always assign filter attributes to the "CustomSetting" category.
            bag.Categories = CategoryCache.All()
                .Where( c =>
                    c.Name == "CustomSetting"
                    && c.EntityTypeId == this.AttributeEntityTypeId
                    && c.EntityTypeQualifierColumn == "EntityTypeId"
                    && c.EntityTypeQualifierValue == this.BlockEntityTypeId.ToString()
                )
                .ToListItemBagList();

            var attribute = Helper.SaveAttributeEdits( bag, this.BlockEntityTypeId, "Id", this.BlockId.ToString(), this.RockContext );

            // Attribute will be null if it was not valid.
            if ( attribute == null )
            {
                return ActionBadRequest();
            }

            // Return a grid row representing the attribute so it can be added or updated within the filters grid.
            var builder = GetFiltersGridBuilder();
            var filters = new List<Model.Attribute> { attribute };
            var gridDataBag = builder.Build( filters );

            BlockCache.FlushItem( this.BlockId );

            return ActionOk( gridDataBag.Rows[0] );
        }

        /// <summary>
        /// Changes the ordered position of a single filter, for the custom settings modal.
        /// </summary>
        /// <param name="filterKey">The identifier of the filter that will be moved.</param>
        /// <param name="beforeFilterKey">The identifier of the filter it will be placed before.</param>
        /// <returns>An empty result that indicates if the operation succeeded.</returns>
        [BlockAction]
        public BlockActionResult ReorderFilter( string filterKey, string beforeFilterKey )
        {
            if ( !this.BlockCache.IsAuthorized( Rock.Security.Authorization.ADMINISTRATE, this.RequestContext.CurrentPerson ) )
            {
                return ActionForbidden( "Not authorized to edit block settings." );
            }

            var attributes = GetOrderedFilters( shouldTrackChanges: true );
            if ( !attributes.ReorderEntity( filterKey, beforeFilterKey ) )
            {
                return ActionBadRequest( "Invalid reorder attempt." );
            }

            this.RockContext.SaveChanges();

            BlockCache.FlushItem( this.BlockId );

            return ActionOk();
        }

        /// <summary>
        /// Deletes the specified filter from the database, for the custom settings modal.
        /// </summary>
        /// <param name="filterGuid">The unique identifier of the filter to delete.</param>
        /// <returns>An empty result that indicates if the operation succeeded.</returns>
        [BlockAction]
        public BlockActionResult DeleteFilter( Guid filterGuid )
        {
            if ( !this.BlockCache.IsAuthorized( Rock.Security.Authorization.ADMINISTRATE, this.RequestContext.CurrentPerson ) )
            {
                return ActionForbidden( "Not authorized to edit block settings." );
            }

            var attributeService = new AttributeService( this.RockContext );
            var attribute = attributeService.Get( filterGuid );
            if ( attribute == null )
            {
                return ActionBadRequest( "Filter not found." );
            }

            attributeService.Delete( attribute );
            this.RockContext.SaveChanges();

            BlockCache.FlushItem( this.BlockId );

            return ActionOk();
        }

        #endregion Block Actions

        #region IHasCustomActions

        /// <inheritdoc/>
        List<BlockCustomActionBag> IHasCustomActions.GetCustomActions( bool canEdit, bool canAdministrate )
        {
            var actions = new List<BlockCustomActionBag>();

            if ( canAdministrate )
            {
                actions.Add( new BlockCustomActionBag
                {
                    IconCssClass = "fa fa-edit",
                    Tooltip = "Settings",
                    ComponentFileUrl = "/Obsidian/Blocks/Reporting/pageParameterFilterCustomSettings.obs"
                } );
            }

            return actions;
        }

        #endregion

        #region SupportingMembers

        /// <summary>
        /// A POCO to represent available filter button sizes for the page parameter filter block.
        /// </summary>
        private class FilterButtonSize
        {
            private static readonly ListItemBag _normal = new ListItemBag { Text = "Normal", Value = "1" };
            public static ListItemBag Normal => _normal;

            private static readonly ListItemBag _small = new ListItemBag { Text = "Small", Value = "2" };
            public static ListItemBag Small => _small;

            private static readonly ListItemBag _extraSmall = new ListItemBag { Text = "Extra Small", Value = "3" };
            public static ListItemBag ExtraSmall => _extraSmall;
        }

        #endregion SupportingMembers
    }
}
