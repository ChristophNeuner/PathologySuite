using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathologySuite.Shared.Options
{
    public class PathOptions
    {
        public PathOptions(String wsiBasePath, String wsiBaseFolderName, Uri appBaseUri, string guidSeparator)
        {
            WsiBasePath = wsiBasePath;
            WsiBaseFolderName = wsiBaseFolderName;
            AppBaseUri = AppBaseUri;
            GuidSeparator = guidSeparator;
        }
        public String WsiBasePath{get; set;}
        public String WsiBaseFolderName { get; set; }

        /// <summary>
        /// e.g. http://localhost:5000/
        /// </summary>
        public Uri AppBaseUri { get; set; }
        public string GuidSeparator { get; set; }
    }
}
