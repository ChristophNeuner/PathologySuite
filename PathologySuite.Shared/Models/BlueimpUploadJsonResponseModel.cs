using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PathologySuite.Shared.Models
{
    public class BlueimpUploadJsonResponseModel
    {
        public BlueimpUploadJsonResponseModel(string name, long size, string deleteUrl, string doneUrl, string error)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (deleteUrl is null)
            {
                throw new ArgumentNullException(nameof(deleteUrl));
            }

            if(doneUrl is null)
            {
                throw new ArgumentNullException(nameof(doneUrl));
            }

            if (error is null)
            {
                throw new ArgumentNullException(nameof(error));
            }

            Name = name;
            Size = size;
            DeleteUrl = deleteUrl;
            DoneUrl = doneUrl;
            Error = error;
        }

        public string Name { get; set; }
        public long Size { get; set; }
        public string DeleteUrl { get; set; }
        public string DoneUrl { get; private set; }
        public string Error { get; set; }
    }
}
