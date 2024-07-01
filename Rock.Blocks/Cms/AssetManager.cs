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

using System.ComponentModel;

using Rock.Attribute;
using Rock.Model;
using Rock.ViewModels.Blocks.Cms.AssetManager;

namespace Rock.Blocks.Cms
{
    /// <summary>
    /// Manage files stored on a remote server or 3rd party cloud storage
    /// </summary>

    [DisplayName( "Asset Manager" )]
    [Category( "CMS" )]
    [Description( "Browse and manage files on the web server or stored on a remote server or 3rd party cloud storage" )]
    [IconCssClass( "fa fa-question" )]
    // [SupportedSiteTypes( Model.SiteType.Web )]

    #region Block Attributes

    #endregion

    [Rock.SystemGuid.EntityTypeGuid( "e357ad54-1725-48b8-997c-23c2587800fb" )]
    [Rock.SystemGuid.BlockTypeGuid( "535500a7-967f-4da3-8fca-cb844203cb3d" )]
    public class AssetManager : RockBlockType
    {
        #region Keys

        private static class PageParameterKey
        {
            public const string AssetStorageProviderId = "AssetStorageProviderId";
        }

        private static class NavigationUrlKey
        {
            public const string ParentPage = "ParentPage";
        }

        #endregion Keys

        #region Methods

        /// <inheritdoc/>
        public override object GetObsidianBlockInitialization()
        {
            var box = new AssetManagerBag();

            //box.NavigationUrls = GetBoxNavigationUrls();
            //box.Options = GetBoxOptions( box.IsEditable );

            return box;
        }

        #endregion

        #region Block Actions

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="key">The identifier of the entity to be deleted.</param>
        /// <returns>A string that contains the URL to be redirected to on success.</returns>
        [BlockAction]
        public BlockActionResult DoSomething( string key )
        {
            //var entityService = new AssetStorageProviderService( RockContext );

            //if ( !TryGetEntityForEditAction( key, out var entity, out var actionError ) )
            //{
            //    return actionError;
            //}

            //if ( !entityService.CanDelete( entity, out var errorMessage ) )
            //{
            //    return ActionBadRequest( errorMessage );
            //}

            //entityService.Delete( entity );
            //RockContext.SaveChanges();

            return ActionOk( "ok" );
        }

        #endregion
    }
}
