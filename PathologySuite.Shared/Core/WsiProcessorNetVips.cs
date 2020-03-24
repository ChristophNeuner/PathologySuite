using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NetVips;
using PathologySuite.Shared.Core.Interfaces;

namespace PathologySuite.Shared.Core
{
    public class WsiProcessorNetVips : IWsiProcessor
    {
        public void GenerateThumbnailAndDzi(string path)
        {
            Task.Run(() =>
            {
                string filenameWithoutExtension = Path.GetFileNameWithoutExtension(path);
                string directoryName = Path.GetDirectoryName(path);
                Image img = Image.NewFromFile(path);
                Image thumbnail = img.ThumbnailImage(200);
                thumbnail.Jpegsave($@"{directoryName}\{filenameWithoutExtension}-thumbnail.jpg");
                img.Dzsave($@"{directoryName}\{filenameWithoutExtension}");
            });
        }
    }
}
