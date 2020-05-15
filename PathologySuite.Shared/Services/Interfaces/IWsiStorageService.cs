using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathologySuite.Shared.Services.Interfaces
{
    public interface IWsiStorageService
    {
        Task SaveAsync(IFormFile file);
        Task<bool> DeleteAsync(string filename);
        Task<bool> NotifyUploadCompleteAsync(string filename);
    }
}
