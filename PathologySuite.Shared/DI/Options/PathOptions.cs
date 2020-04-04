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
        public PathOptions()
        {
            WsiBaseUri = new Uri($@"http://localhost:5000");
            WsiBaseFolderName = "histo";
        }
        public Uri WsiBaseUri{get; set;}
        public String WsiBaseFolderName { get; set; }
    }
}
