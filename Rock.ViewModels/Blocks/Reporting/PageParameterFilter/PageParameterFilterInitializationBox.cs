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

using System.Collections.Generic;

using Rock.ViewModels.Utility;

namespace Rock.ViewModels.Blocks.Reporting.PageParameterFilter
{
    /// <summary>
    /// The box that contains all the initialization information for the page parameter filter block.
    /// </summary>
    public class PageParameterFilterInitializationBox : BlockBox
    {
        /// <summary>
        /// Gets or sets the block title text.
        /// </summary>
        public string BlockTitleText { get; set; }

        /// <summary>
        /// Gets or sets the block title icon CSS class.
        /// </summary>
        public string BlockTitleIconCssClass { get; set; }

        /// <summary>
        /// Gets or sets whether to show the block title.
        /// </summary>
        public bool ShowBlockTitle { get; set; }

        /// <summary>
        /// Gets or sets the filter button text.
        /// </summary>
        public string FilterButtonText { get; set; }

        /// <summary>
        /// Gets or sets the filter button size.
        /// </summary>
        public string FilterButtonSize { get; set; }

        /// <summary>
        /// Gets or sets whether to show the filter button.
        /// </summary>
        public bool ShowFilterButton { get; set; }

        /// <summary>
        /// Gets or sets whether to show the reset filters button.
        /// </summary>
        public bool ShowResetFiltersButton { get; set; }

        /// <summary>
        /// Gets or sets how many filters to display per row.
        /// </summary>
        public int FiltersPerRow { get; set; }

        /// <summary>
        /// Gets or sets the page parameter filters available for selection.
        /// </summary>
        public Dictionary<string, PublicAttributeBag> Filters { get; set; }

        /// <summary>
        /// Gets or sets the page parameter filter value defaults.
        /// </summary>
        public Dictionary<string, string> FilterValueDefaults { get; set; }
    }
}
