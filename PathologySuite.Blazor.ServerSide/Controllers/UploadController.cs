using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PathologySuite.Shared.Core.Interfaces;
using PathologySuite.Shared.Options;
using PathologySuite.Shared.Models;
using PathologySuite.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Diagnostics;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PathologySuite.Blazor.ServerSide.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : Controller
    {
        private IWebHostEnvironment _hostingEnv;
        private IWebHostEnvironment _webHostEnvironment;
        private PathOptions _pathOptions;
        private IWsiStorageService _wsiStorageService;

        public UploadController(IWebHostEnvironment env, IWebHostEnvironment webHostEnvironment, PathOptions pathOptions, IWsiStorageService wsiStorageService)
        {
            _hostingEnv = env;
            _webHostEnvironment = webHostEnvironment;
            _pathOptions = pathOptions;
            _wsiStorageService = wsiStorageService;
        }

        [HttpPost("[action]")]
        [DisableRequestSizeLimit]
        [RequestFormLimits(MultipartBodyLengthLimit = 1024 * 1024 * 50)] // this value has also to be set in web.config and jquery.fileupload.js -> maxChunkSize; 
                                                                         //see here http://www.binaryintellect.net/articles/612cf2d1-5b3d-40eb-a5ff-924005955a62.aspx
        public async Task<JsonResult> SaveBlueimp(IList<IFormFile> files)
        {
            List<BlueimpUploadJsonResponseModel> responseModels = new List<BlueimpUploadJsonResponseModel>();

            foreach (var file in files)
            {
                var filename = ContentDispositionHeaderValue
                    .Parse(file.ContentDisposition)
                    .FileName
                    .Trim('"');
                var filepath = $@"{_pathOptions.WsiBasePath}/{filename}";

                string filenameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
                string deleteURL = $@"{_pathOptions.WsiBaseUri}api/upload/DeleteBlueimp/?filename={filename}";
                string doneURL = $@"{_pathOptions.WsiBaseUri}api/upload/OnAfterSuccessfulUploadBlueimp/?filename={filename}";
                responseModels.Add(new BlueimpUploadJsonResponseModel(filename, file.Length, deleteURL, doneURL, error: ""));

                try
                {
                    await _wsiStorageService.SaveAsync(file);
                }

                catch (Exception e)
                {
                    responseModels.Last().Error = e.Message;
                }
            }

            return Json(responseModels);
        }

        [HttpGet("[action]")]
        public async Task<JsonResult> OnAfterSuccessfulUploadBlueimp(string filename)
        {
            bool result = await _wsiStorageService.NotifyUploadCompleteAsync(filename);

            Debug.Assert(result);

            return Json(new BlueimpDoneJsonResponseModel(filename, result));
        }

        [HttpGet("[action]")]
        public async Task<JsonResult> DeleteBlueimp(string filename)
        {
            bool result = await _wsiStorageService.DeleteAsync(filename);

            Debug.Assert(result);

            return Json(new PathologySuite.Shared.Models.BlueimpDeleteJsonResponseModel(filename, result));
        }
    }
}
