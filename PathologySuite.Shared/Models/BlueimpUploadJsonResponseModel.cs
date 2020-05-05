using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PathologySuite.Shared.Models
{
    public class BlueimpUploadJsonResponseModel
    {
        public BlueimpUploadJsonResponseModel(string name, long size, string url, string thumbnailUrl, string deleteUrl, string doneUrl, string deleteType, string error)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (thumbnailUrl is null)
            {
                throw new ArgumentNullException(nameof(thumbnailUrl));
            }

            if (deleteUrl is null)
            {
                throw new ArgumentNullException(nameof(deleteUrl));
            }

            if(doneUrl is null)
            {
                throw new ArgumentNullException(nameof(doneUrl));
            }

            if (deleteType is null)
            {
                throw new ArgumentNullException(nameof(deleteType));
            }

            if (error is null)
            {
                throw new ArgumentNullException(nameof(error));
            }

            Name = name;
            Size = size;
            Url = url;
            ThumbnailUrl = thumbnailUrl;
            DeleteUrl = deleteUrl;
            DoneUrl = doneUrl;
            DeleteType = deleteType;
            Error = error;
        }

        public string Name { get; set; }
        public long Size { get; set; }
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }
        public string DeleteUrl { get; set; }
        public string DoneUrl { get; private set; }

        public string DeleteType { get { return "DELETE"; } private set { } }
        public string Error { get; set; }
    }
}
