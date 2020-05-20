using MongoDB.Bson.Serialization.Attributes;
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
        private string _id;
        private string _filename;
        private string _fileExtension;
        private string _physicalPathWsi;
        private string _physcialPathThumbnail;
        //private string _webRootPathWsi;
        //private string _webRootPathThumbnail;
        private bool _completelyUploaded;

        public WholeSlideImage(string id,
                                string filename,
                                string fileExtension,
                                string physicalPathWsi,
                                string physcialPathThumbnail,
                                //string webRootPathWsi,
                                //string webRootPathThumbnail,
                                bool completelyUploaded)
        {
            _id = id ?? throw new ArgumentNullException(nameof(id));
            _filename = filename ?? throw new ArgumentNullException(nameof(filename));
            _fileExtension = fileExtension ?? throw new ArgumentNullException(nameof(fileExtension));
            _physicalPathWsi = physicalPathWsi ?? throw new ArgumentNullException(nameof(physicalPathWsi));
            _physcialPathThumbnail = physcialPathThumbnail ?? throw new ArgumentNullException(nameof(physcialPathThumbnail));
            //_webRootPathWsi = webRootPathWsi ?? throw new ArgumentNullException(nameof(webRootPathWsi));
            //_webRootPathThumbnail = webRootPathThumbnail ?? throw new ArgumentNullException(nameof(webRootPathThumbnail));
            _completelyUploaded = completelyUploaded;
        }

        [BsonId]
        public string Id { get { return _id; } set { _id = value; } }

        [BsonElement("Name")]
        public string FileName { get { return _filename; } set { _filename = value; } }

        /// <summary>
        /// e.g. ".ndpi"
        /// </summary>
        public string FileExtension { get { return _fileExtension; } set { _fileExtension = value; } }

        public string PhysicalPathWsi { get { return _physicalPathWsi; } set { _physicalPathWsi = value; } }
        public string PhysicalPathThumbnail { get { return _physcialPathThumbnail; } set { _physcialPathThumbnail = value; } }

        ///// <summary>
        ///// part of the full path that comes after WebRootPath (by default everything after ~/wwwroot/)
        ///// </summary>
        //public string WebRootPathWsi { get { return _webRootPathWsi; } set { _webRootPathWsi = value; } }

        ///// <summary>
        ///// part of the full path that comes after WebRootPath (by default everything after ~/wwwroot/)
        ///// </summary>
        //public string WebRootPathThumbnail { get { return _webRootPathThumbnail; } set { _webRootPathThumbnail = value; } }

        public bool CompletelyUploaded { get { return _completelyUploaded; } set { _completelyUploaded = value; } }
    }
}
