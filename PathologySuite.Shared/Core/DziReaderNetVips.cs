using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using PathologySuite.Shared.Core.Interfaces;

namespace PathologySuite.Shared.Core
{
    public class DziReaderNetVips : IDziReader
    {
        public DziValuesWrapperNetVips ReadDziFile(string path)
        {
            XDocument xdoc = XDocument.Load(path);

            return new DziValuesWrapperNetVips(xdoc.Root.Attribute("xmlns").Value,
                xdoc.Root.Attribute("Format").Value,
                xdoc.Root.Attribute("Overlap").Value,
                xdoc.Root.Attribute("TileSize").Value,
                xdoc.Descendants().Where(o => o.Name.LocalName == "Size").FirstOrDefault().Attribute("Height").Value,
                xdoc.Descendants().Where(o => o.Name.LocalName == "Size").FirstOrDefault().Attribute("Width").Value);
        }
    }
}
