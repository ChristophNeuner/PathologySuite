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
using PathologySuite.Shared;
using PathologySuite.Shared.Services;

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
        private WsiDbService _wsiDbService;

        public UploadController(IWebHostEnvironment env, IWebHostEnvironment webHostEnvironment, PathOptions pathOptions, IWsiStorageService wsiStorageService, WsiDbService wsiDbService)
        {
            _hostingEnv = env;
            _webHostEnvironment = webHostEnvironment;
            _pathOptions = pathOptions;
            _wsiStorageService = wsiStorageService;
            _wsiDbService = wsiDbService;
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
                var filename = Utils.GetFilenameFromIFormFile(file);

                string deleteURL = $@"{_pathOptions.AppBaseUri}api/upload/DeleteBlueimp/?filename={filename}";
                string doneURL = $@"{_pathOptions.AppBaseUri}api/upload/OnAfterSuccessfulUploadBlueimp/?filename={filename}";
                responseModels.Add(new BlueimpUploadJsonResponseModel(filename, file.Length, deleteURL, doneURL, error: ""));

                try
                {
                    WholeSlideImage wsi =  await _wsiStorageService.SaveAsync(file);
                    await _wsiDbService.CreateOrUpdateAsync(Utils.GetGuidFromFilename(filename, _pathOptions.GuidSeparator), wsi);
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
            bool success = true;
            try
            {
                WholeSlideImage wsi = await _wsiStorageService.NotifyUploadCompleteAsync(filename);
                await _wsiDbService.CreateOrUpdateAsync(Utils.GetGuidFromFilename(filename, _pathOptions.GuidSeparator), wsi);
            }
            catch(Exception e)
            {
                //TODO
                Console.WriteLine(e.Message);
                success = false;
            }

            Debug.Assert(success);

            return Json(new BlueimpDoneJsonResponseModel(filename, success));
        }

        [HttpGet("[action]")]
        public async Task<JsonResult> DeleteBlueimp(string filename)
        {
            bool success = true;
            try
            {
                WholeSlideImage wsi = await _wsiStorageService.DeleteAsync(filename);
                await _wsiDbService.RemoveAsync(Utils.GetGuidFromFilename(filename, _pathOptions.GuidSeparator));
            }
            catch(Exception e)
            {
                //TODO
                Console.WriteLine(e.Message);
                success = false;
            }

            Debug.Assert(success);

            return Json(new PathologySuite.Shared.Models.BlueimpDeleteJsonResponseModel(filename, success));
        }
    }
}
