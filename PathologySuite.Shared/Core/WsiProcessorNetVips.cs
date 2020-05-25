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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer">byte[] of a whole-slide image</param>
        /// <returns>byte[] of the generated thumbnail image</returns>
        public Task<byte[]> GenerateThumbnail(byte[] wholeSlideImage)
        {
            return Task.Run(() =>
            {
                Image thumbnail = Image.NewFromBuffer(wholeSlideImage).ThumbnailImage(200);
                return thumbnail.JpegsaveBuffer();
            });
        }

        public Task<byte[]> GenerateDzi(byte[] wholeSlideImage)
        {
            throw new NotImplementedException();
        }

        public void GenerateThumbnail(string wsiPath)
        {
            Task.Run(() =>
            {
                string filenameWithoutExtension = Path.GetFileNameWithoutExtension(wsiPath);
                string directoryName = Path.GetDirectoryName(wsiPath);
                Image thumbnail = Image.NewFromFile(wsiPath).ThumbnailImage(200);
                thumbnail.Jpegsave($@"{directoryName}\{filenameWithoutExtension}-thumbnail.jpg");
            });
        }

        public void GenerateDzi(string wsiPath)
        {
            Task.Run(() =>
            {
                string filenameWithoutExtension = Path.GetFileNameWithoutExtension(wsiPath);
                string directoryName = Path.GetDirectoryName(wsiPath);
                Image img = Image.NewFromFile(wsiPath);
                img.Dzsave($@"{directoryName}\{filenameWithoutExtension}");
            });
        }
    }
}
