using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathologySuite.Shared.DI.Options
{
    public class PathOptions
    {
        public PathOptions(String wsiBasePath, String wsiBaseFolderName, Uri wsiBaseUri)
        {
            WsiBasePath = wsiBasePath;
            WsiBaseFolderName = wsiBaseFolderName;
            WsiBaseUri = wsiBaseUri;
        }
        public String WsiBasePath{get; private set;}
        public String WsiBaseFolderName { get; private set; }

        public Uri WsiBaseUri { get; private set; }
    }
}
