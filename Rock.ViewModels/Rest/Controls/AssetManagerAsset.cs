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


using System.Text.RegularExpressions;

namespace Rock.ViewModels.Rest.Controls
{
    /// <summary>
    /// TODO
    /// </summary>
    public class AssetManagerAsset
    {
        public int? ProviderId { get; set; }

        public string EncryptedRoot { get; set; } = string.Empty;

        public string Root { get; set; } = string.Empty;

        public string SubPath { get; set; } = string.Empty;

        public string FullPath
        {
            get
            {
                return Regex.Replace( Root + SubPath, "[" + Regex.Escape( string.Concat( System.IO.Path.GetInvalidPathChars() ) ) + "]", string.Empty, RegexOptions.CultureInvariant ).Replace( '\\', '/' );
            }
        }

        public string FullDirectoryPath
        {
            get
            {
                return System.IO.Path.GetDirectoryName( FullPath ).Replace( '\\', '/' );
            }
        }

        public string FileName
        {
            get
            {
                var fileName = System.IO.Path.GetFileName( FullPath );
                return Regex.Replace( fileName, "[" + Regex.Escape( string.Concat( System.IO.Path.GetInvalidFileNameChars() ) ) + "]", string.Empty, RegexOptions.CultureInvariant );
            }
        }

        public bool isRoot
        {
            get
            {
                return Root != null && Root != string.Empty && ( SubPath == null || SubPath == string.Empty );
            }
        }
    }
}
