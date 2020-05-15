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
        public PathOptions(String wsiBasePath, String wsiBaseFolderName, Uri wsiBaseUri, string guidSeparator)
        {
            WsiBasePath = wsiBasePath;
            WsiBaseFolderName = wsiBaseFolderName;
            WsiBaseUri = wsiBaseUri;
            GuidSeparator = guidSeparator;
        }
        public String WsiBasePath{get; private set;}
        public String WsiBaseFolderName { get; private set; }

        public Uri WsiBaseUri { get; private set; }
        public string GuidSeparator { get; private set; }
    }
}
