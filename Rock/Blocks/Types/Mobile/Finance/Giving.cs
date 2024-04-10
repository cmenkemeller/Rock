using System.ComponentModel;

using Rock.Attribute;

using Rock.Web.UI;

namespace Rock.Blocks.Types.Mobile.Finance
{
    /// <summary>
    /// Native giving block for the mobile application.
    /// </summary>
    /// <seealso cref="Rock.Blocks.RockBlockType" />
    [DisplayName( "Giving" )]
    [Category( "Mobile > Core" )]
    [Description( "Collect donations natively, within your Rock Mobile application." )]
    [IconCssClass( "fa fa-donate" )]
    [ContextAware]
    [SupportedSiteTypes( Model.SiteType.Mobile )]

    #region Block Attributes

    #endregion

    [Rock.SystemGuid.EntityTypeGuid( Rock.SystemGuid.EntityType.MOBILE_FINANCE_GIVING )]
    [Rock.SystemGuid.BlockTypeGuid( Rock.SystemGuid.BlockType.MOBILE_FINANCE_GIVING )]
    public class Giving : RockBlockType
    {
    }
}
