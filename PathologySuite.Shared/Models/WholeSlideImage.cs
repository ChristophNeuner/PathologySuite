using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace PathologySuite.Shared.Models
{
    public class WholeSlideImage
    {
        private string _physicalPathWsi;
        private string _physcialPathThumbnail;
        private string _webRootPathWsi;
        private string _webRootPathThumbnail;

        public WholeSlideImage(string physicalPathWsi, string physcialPathThumbnail, string webRootPathWsi, string webRootPathThumbnail)
        {
            _physicalPathWsi = physicalPathWsi ?? throw new ArgumentNullException(nameof(physicalPathWsi));
            _physcialPathThumbnail = physcialPathThumbnail ?? throw new ArgumentNullException(nameof(physcialPathThumbnail));
            _webRootPathWsi = webRootPathWsi ?? throw new ArgumentNullException(nameof(webRootPathWsi));
            _webRootPathThumbnail = webRootPathThumbnail ?? throw new ArgumentNullException(nameof(webRootPathThumbnail));
        }

        public string PhysicalPathWsi { get { return _physicalPathWsi; } private set { _physicalPathWsi = value; } }
        public string PhysicalPathThumbnail { get { return _physcialPathThumbnail; } private set { _physcialPathThumbnail = value; } }

        /// <summary>
        /// part of the full path that comes after WebRootPath (by default everything after ~/wwwroot/)
        /// </summary>
        public string WebRootPathWsi { get { return _webRootPathWsi; } private set { _webRootPathWsi = value; } }

        /// <summary>
        /// part of the full path that comes after WebRootPath (by default everything after ~/wwwroot/)
        /// </summary>
        public string WebRootPathThumbnail { get { return _webRootPathThumbnail; } private set { _webRootPathThumbnail = value; } }
    }
}
