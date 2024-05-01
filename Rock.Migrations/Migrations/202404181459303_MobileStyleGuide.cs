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
namespace Rock.Migrations
{
    /// <summary>
    ///
    /// </summary>
    public partial class MobileStyleGuide : Rock.Migrations.RockMigration
    {
        /// <summary>
        /// The standard icon to use for new templates.
        /// </summary>
        private const string _standardIconSvg = "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiIHN0YW5kYWxvbmU9Im5vIj8+CjwhRE9DVFlQRSBzdmcgUFVCTElDICItLy9XM0MvL0RURCBTVkcgMS4xLy9FTiIgImh0dHA6Ly93d3cudzMub3JnL0dyYXBoaWNzL1NWRy8xLjEvRFREL3N2ZzExLmR0ZCI+Cjxzdmcgd2lkdGg9IjEwMCUiIGhlaWdodD0iMTAwJSIgdmlld0JveD0iMCAwIDY0MCAyNDAiIHZlcnNpb249IjEuMSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIiB4bWxuczp4bGluaz0iaHR0cDovL3d3dy53My5vcmcvMTk5OS94bGluayIgeG1sOnNwYWNlPSJwcmVzZXJ2ZSIgeG1sbnM6c2VyaWY9Imh0dHA6Ly93d3cuc2VyaWYuY29tLyIgc3R5bGU9ImZpbGwtcnVsZTpldmVub2RkO2NsaXAtcnVsZTpldmVub2RkO3N0cm9rZS1saW5lam9pbjpyb3VuZDtzdHJva2UtbWl0ZXJsaW1pdDoyOyI+CiAgICA8ZyB0cmFuc2Zvcm09Im1hdHJpeCgxLjEwMTU1LDAsMCwxLC0zMC44NDM0LC0zMSkiPgogICAgICAgIDxyZWN0IHg9IjI4IiB5PSIzMSIgd2lkdGg9IjU4MSIgaGVpZ2h0PSIxOCIgc3R5bGU9ImZpbGw6cmdiKDIzMSwyMzEsMjMxKTsiLz4KICAgIDwvZz4KICAgIDxnIHRyYW5zZm9ybT0ibWF0cml4KDAuOTY1NTc3LDAsMCwxLC0yNy4wMzYxLDEyKSI+CiAgICAgICAgPHJlY3QgeD0iMjgiIHk9IjMxIiB3aWR0aD0iNTgxIiBoZWlnaHQ9IjE4IiBzdHlsZT0iZmlsbDpyZ2IoMjMxLDIzMSwyMzEpOyIvPgogICAgPC9nPgogICAgPGcgdHJhbnNmb3JtPSJtYXRyaXgoMS4wMjA2NSwwLDAsMSwtMjguNTc4Myw1NSkiPgogICAgICAgIDxyZWN0IHg9IjI4IiB5PSIzMSIgd2lkdGg9IjU4MSIgaGVpZ2h0PSIxOCIgc3R5bGU9ImZpbGw6cmdiKDIzMSwyMzEsMjMxKTsiLz4KICAgIDwvZz4KICAgIDxnIHRyYW5zZm9ybT0ibWF0cml4KDAuOTg0NTA5LDAsMCwxLC0yNy41NjYzLDk4KSI+CiAgICAgICAgPHJlY3QgeD0iMjgiIHk9IjMxIiB3aWR0aD0iNTgxIiBoZWlnaHQ9IjE4IiBzdHlsZT0iZmlsbDpyZ2IoMjMxLDIzMSwyMzEpOyIvPgogICAgPC9nPgogICAgPGcgdHJhbnNmb3JtPSJtYXRyaXgoMS4wNTY4LDAsMCwxLC0yOS41OTA0LDE0MSkiPgogICAgICAgIDxyZWN0IHg9IjI4IiB5PSIzMSIgd2lkdGg9IjU4MSIgaGVpZ2h0PSIxOCIgc3R5bGU9ImZpbGw6cmdiKDIzMSwyMzEsMjMxKTsiLz4KICAgIDwvZz4KICAgIDxnIHRyYW5zZm9ybT0ibWF0cml4KDEuMDc5MTcsMCwwLDEsLTMwLjIxNjksMTg0KSI+CiAgICAgICAgPHJlY3QgeD0iMjgiIHk9IjMxIiB3aWR0aD0iNTgxIiBoZWlnaHQ9IjE4IiBzdHlsZT0iZmlsbDpyZ2IoMjMxLDIzMSwyMzEpOyIvPgogICAgPC9nPgo8L3N2Zz4K";

        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            // TODO:
            // 1. For all existing mobile sites:
            //    a. Ensure the "Enable Legacy Mobile Styles" setting is enabled.
            //    b. This should be automatic, but verify that the templates are still using 
            //       the legacy.
            //
            // For all of the blocks that we update templates for,
            // we need to:
            // a) Add the new template as the default value.
            // b) Update the block to use the new template for the default value (in the block attribute).
            // c) Rename the old template to have a different name ("Legacy").
            // 
            ConnectionTypeListBlockUp();
        }

        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
            ConnectionTypeListBlockDown();
        }

        /// <summary>
        /// Called to update the ConnectionTypeListBlock default templates.
        /// </summary>
        private void ConnectionTypeListBlockUp()
        {
            //
            // Add the new default template.
            //
            RockMigrationHelper.AddOrUpdateTemplateBlockTemplate(
                "F9F29166-A080-4179-A210-AE42CC473D6F",
                Rock.SystemGuid.DefinedValue.BLOCK_TEMPLATE_MOBILE_CONNECTION_CONNECTION_TYPE_LIST,
                "Default",
                _connectionTypeListTemplate,
                _standardIconSvg,
                "standard-template.svg",
                "image/svg+xml" );

            //
            // Update the legacy template to have a new name.
            //
            RockMigrationHelper.AddOrUpdateTemplateBlockTemplate(
                "E0D00422-7895-4081-9C06-16DE9BF48E1A",
                Rock.SystemGuid.DefinedValue.BLOCK_TEMPLATE_MOBILE_CONNECTION_CONNECTION_TYPE_LIST,
                "Legacy",
                _connectionTypeListLegacyTemplate,
                _standardIconSvg,
                "standard-template.svg",
                "image/svg+xml" );
        }

        /// <summary>
        /// Called to downgrade the ConnectionTypeListBlock default templates.
        /// </summary>
        private void ConnectionTypeListBlockDown()
        {
            RockMigrationHelper.DeleteTemplateBlockTemplate( "F9F29166-A080-4179-A210-AE42CC473D6F" );
            RockMigrationHelper.DeleteTemplateBlockTemplate( "E0D00422-7895-4081-9C06-16DE9BF48E1A" );

            //
            // Add back the new default legacy template.
            // Need to do it this way since the block now points to the new guid as the default.
            //
            RockMigrationHelper.AddOrUpdateTemplateBlockTemplate(
                "F9F29166-A080-4179-A210-AE42CC473D6F",
                Rock.SystemGuid.DefinedValue.BLOCK_TEMPLATE_MOBILE_CONNECTION_CONNECTION_TYPE_LIST,
                "Default",
                _connectionTypeListLegacyTemplate,
                _standardIconSvg,
                "standard-template.svg",
                "image/svg+xml" );
        }

        #region New Templates

        private const string _connectionTypeListTemplate = @"<Rock:StyledBorder StyleClass=""border, border-interface-soft, bg-interface-softest, rounded, p-16"">
    <VerticalStackLayout>
        {% for type in ConnectionTypes %}    
            <Grid RowDefinitions=""Auto, Auto, Auto""
                  ColumnDefinitions=""Auto, *""
                  StyleClass=""gap-column-8"">
    
                <!-- Icon -->
                <Rock:Icon StyleClass=""text-interface-strong""
                           IconClass=""{{ type.IconCssClass | Replace:'fa fa-','' }}""
                           FontSize=""36""
                           HorizontalOptions=""Center""
                           VerticalOptions=""Center""
                           Grid.Row=""0""
                           Grid.Column=""0"" 
                           Grid.RowSpan=""2"" />
    
                <Grid ColumnDefinitions=""*, 24""
                      Grid.Row=""0""
                      Grid.Column=""1"">

                      <Label Text=""{{ type.Name | Escape }}""
                             StyleClass=""body, bold, text-interface-stronger""
                             MaxLines=""1""
                             LineBreakMode=""TailTruncation""
                             HorizontalOptions=""Start""
                             Grid.Row=""0""
                             Grid.Column=""0"" />

                      <Border HeightRequest=""20""
                              WidthRequest=""20""
                              StrokeShape=""RoundRectangle 10""
                              Grid.Row=""0""
                              Grid.Column=""1""
                              StyleClass=""bg-info-strong"">
                           <Label Text=""{{ ConnectionRequestCounts[type.Id].AssignedToYouCount }}""
                                  StyleClass=""text-interface-softer, caption2""
                                  HorizontalOptions=""Center""
                                  VerticalOptions=""Center"" />
                      </Border>
                </Grid>
                    
                <Label Text=""{{ type.Description | Escape }}""
                       StyleClass=""footnote, text-interface-strong""
                       MaxLines=""2""
                       LineBreakMode=""TailTruncation""
                       Grid.Row=""1""
                       Grid.Column=""1"" />

                {% unless forloop.last %}
                    <BoxView HeightRequest=""1""
                             Grid.Row=""2"" 
                             Grid.Column=""0"" 
                             Grid.ColumnSpan=""2""
                             StyleClass=""my-8, bg-interface-soft"" />
                {% endunless %}
                
                {% if DetailPage != null %}
                    <Grid.GestureRecognizers>
                        <TapGestureRecognizer Command=""{Binding PushPage}""
                                              CommandParameter=""{{ DetailPage }}"" />
                    </Grid.GestureRecognizers>
                {% endif %}
            </Grid>
        {% endfor %}
    </VerticalStackLayout>    
</Rock:StyledBorder>";
        private const string _connectionTypeListLegacyTemplate = @"<StackLayout Spacing=""0"">
    {% for type in ConnectionTypes %}    
        <Frame StyleClass=""connection-type""
            HasShadow=""false"">
            <Grid ColumnDefinitions=""50,*,Auto""
                RowDefinitions=""Auto,Auto""
                RowSpacing=""0"">
                {% if type.IconCssClass != null and type.IconCssClass != '' %}
                    <Rock:Icon StyleClass=""connection-type-icon""
                        IconClass=""{{ type.IconCssClass | Replace:'fa fa-','' }}""
                        HorizontalOptions=""Center""
                        VerticalOptions=""Center""
                        Grid.RowSpan=""2"" />
                {% endif %}
                
                <Label StyleClass=""connection-type-name""
                    Text=""{{ type.Name | Escape }}""
                    Grid.Column=""1"" />

                <Label StyleClass=""connection-type-description""
                    Text=""{{ type.Description | Escape }}""
                    MaxLines=""2""
                    LineBreakMode=""TailTruncation""
                    Grid.Column=""1""
                    Grid.Row=""1""
                    Grid.ColumnSpan=""2"" />

                <Rock:Tag StyleClass=""connection-type-count""
                    Text=""{{ ConnectionRequestCounts[type.Id].AssignedToYouCount }}""
                    Type=""info""
                    Grid.Column=""2"" />
            </Grid>

            {% if DetailPage != null %}            
                <Frame.GestureRecognizers>
                    <TapGestureRecognizer Command=""{Binding PushPage}"" CommandParameter=""{{ DetailPage }}?ConnectionTypeGuid={{ type.Guid }}"" />
                </Frame.GestureRecognizers>
            {% endif %}
        </Frame>
    {% endfor %}
</StackLayout>";

        #endregion
    }
}
