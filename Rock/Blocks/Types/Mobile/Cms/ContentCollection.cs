using Rock.Attribute;
using System.ComponentModel;

namespace Rock.Blocks.Types.Mobile.Cms
{
    /// <summary>
    /// A block that displays a search-able collection of content.
    /// </summary>
    /// 
    [DisplayName( "Content Collection" )]
    [Category( "Mobile > Cms" )]
    [Description( "Displays the search results of a particular content collection." )]
    [IconCssClass( "fa fa-book-open" )]
    [SupportedSiteTypes( Model.SiteType.Mobile )]

    [Rock.SystemGuid.EntityTypeGuid( Rock.SystemGuid.EntityType.MOBILE_CMS_CONTENT_COLLECTION_BLOCK_TYPE )]
    [Rock.SystemGuid.BlockTypeGuid( Rock.SystemGuid.BlockType.MOBILE_CMS_CONTENT_COLLECTION )]
    public class ContentCollection : RockBlockType
    {
    }
}
