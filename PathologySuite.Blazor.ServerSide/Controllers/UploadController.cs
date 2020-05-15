using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using PathologySuite.Shared.Core.Interfaces;
using PathologySuite.Shared.Options;
using PathologySuite.Shared.Models;
using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PathologySuite.Blazor.ServerSide.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : Controller
    {
        private IWebHostEnvironment _hostingEnv;
        private IWsiProcessor _wsiProcessor;
        private IWebHostEnvironment _webHostEnvironment;
        private PathOptions _pathOptions;

        public UploadController(IWebHostEnvironment env, IWsiProcessor wsiProcessor, IWebHostEnvironment webHostEnvironment, PathOptions pathOptions)
        {
            _hostingEnv = env;
            _wsiProcessor = wsiProcessor;
            _webHostEnvironment = webHostEnvironment;
            _pathOptions = pathOptions;

            if (!System.IO.Directory.Exists(_pathOptions.WsiBasePath))
            {
                System.IO.Directory.CreateDirectory(_pathOptions.WsiBasePath);
            }
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
                try
                {
                    var filename = ContentDispositionHeaderValue
                                        .Parse(file.ContentDisposition)
                                        .FileName
                                        .Trim('"');
                    var filepath =  $@"{_pathOptions.WsiBasePath}/{filename}";

                    string fileURL = $@"{_pathOptions.WsiBaseUri}/{_pathOptions.WsiBaseFolderName}/{filename}";
                    string filenameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
                    string thumbURL = $@"{_pathOptions.WsiBasePath}/{filenameWithoutExtension}-thumbnail.jpg";
                    string deleteURL = $@"{_pathOptions.WsiBaseUri}api/upload/DeleteBlueimp/?filename={filename}";
                    string doneURL = $@"{_pathOptions.WsiBaseUri}api/upload/OnAfterSuccessfulUploadBlueimp/?filename={filename}";
                    responseModels.Add(new BlueimpUploadJsonResponseModel(filename, file.Length, fileURL, thumbURL, deleteURL, doneURL, "DELETE", ""));


                    if (!System.IO.File.Exists(filepath))
                    {
                        using (FileStream fs = new FileStream(filepath,
                                                                FileMode.Create,
                                                                FileAccess.Write,
                                                                FileShare.ReadWrite,
                                                                bufferSize: 4096,
                                                                useAsync: true))
                        {
                            await file.CopyToAsync(fs);
                            await fs.FlushAsync();
                        }
                    }
                    else
                    {
                        using (FileStream fs = new FileStream(filepath,
                                                                FileMode.Append,
                                                                FileAccess.Write,
                                                                FileShare.ReadWrite,
                                                                bufferSize: 4096,
                                                                useAsync: true))
                        {
                            await file.CopyToAsync(fs);
                            await fs.FlushAsync();
                        }
                    }
                }

                catch (Exception e)
                {
                    responseModels.Last().Error = e.Message;
                }
            }

            return Json(responseModels);
        }

        [HttpGet("[action]")]
        public JsonResult OnAfterSuccessfulUploadBlueimp(string filename)
        {
            try
            {
                var filepath = $@"{_pathOptions.WsiBasePath}/{filename}";
                if (System.IO.File.Exists(filepath))
                {
                    //fire and forget method in new task
                    _wsiProcessor.GenerateThumbnail(filepath);
                    //_wsiProcessor.GenerateDzi(filepath);
                }
            }
            catch (Exception e)
            {
                //TODO
                throw e;
            }

            return Json(new BlueimpDoneJsonResponseModel(filename, true));
        }

        [HttpGet("[action]")]
        public async Task<JsonResult> DeleteBlueimp(string filename)
        {
            filename = Path.GetFileNameWithoutExtension(filename);
            bool success = true;
            try
            {
                foreach (string file in System.IO.Directory.GetFiles($@"{ _pathOptions.WsiBasePath}/"))
                {
                    if (file.Contains(filename))
                    {
                        System.IO.File.Delete(file);

                        foreach(string dir in System.IO.Directory.GetDirectories($@"{ _pathOptions.WsiBasePath}/"))
                        {
                            if (dir.Contains(filename))
                            {
                                System.IO.Directory.Delete(dir);
                            }
                        }
                    }
                }

                //var filepath = _hostingEnv.ContentRootPath + $@"{_basePath}\{UploadFiles[0].FileName}";
                //if (System.IO.File.Exists(filepath))
                //{
                //    System.IO.File.Delete(filepath);
                //}
            }
            catch (Exception e)
            {

                return Json(new PathologySuite.Shared.Models.BlueimpDeleteJsonResponseModel(filename, false));
                //Response.Clear();
                //Response.StatusCode = 200;
                //Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "File removed successfully";
                //Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = e.Message;

            }

            return Json(new PathologySuite.Shared.Models.BlueimpDeleteJsonResponseModel(filename, success));
        }
    }
}
