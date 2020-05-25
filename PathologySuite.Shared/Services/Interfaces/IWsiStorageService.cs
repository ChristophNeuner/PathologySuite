using Microsoft.AspNetCore.Http;
using PathologySuite.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathologySuite.Shared.Services.Interfaces
{
    public interface IWsiStorageService
    {
        Task<WholeSlideImage> SaveAsync(IFormFile file);
        Task<WholeSlideImage> NotifyUploadCompleteAsync(string filename);
        Task DeleteAsync(string filename);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wsi"></param>
        /// <returns></returns>
        Task<byte[]?> GetThumbnailAsync(WholeSlideImage wsi);
    }
}
