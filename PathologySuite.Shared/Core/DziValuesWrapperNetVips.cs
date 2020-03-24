using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PathologySuite.Shared.Core
{
    public struct DziValuesWrapperNetVips
    {
        private string _xmlns;
        private string _format;
        private string _overlap;
        private string _tileSize;
        private string _height;
        private string _width;
        public DziValuesWrapperNetVips(string xmlns, string format, string overlap, string tileSize, string height, string width)
        {
            _xmlns = xmlns;
            _format = format;
            _overlap = overlap;
            _tileSize = tileSize;
            _height = height;
            _width = width;
        }

        public string Xmlns { get => _xmlns; }
        public string Format { get => _format; }
        public string Overlap { get => _overlap; }
        public string TileSize { get => _tileSize; }
        public string Height { get => _height; }
        public string Width { get => _width; }

    }
}
