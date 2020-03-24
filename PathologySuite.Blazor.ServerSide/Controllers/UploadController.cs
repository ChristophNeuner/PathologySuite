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
using PathologySuite.Shared.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PathologySuite.Blazor.ServerSide.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : Controller
    {
        private IWebHostEnvironment _hostingEnv;
        private IWsiProcessor _wsiProcessor;
        private string _basePath = @"\wwwroot\histo";

        public UploadController(IWebHostEnvironment env, IWsiProcessor wsiProcessor)
        {
            _hostingEnv = env;
            _wsiProcessor = wsiProcessor;
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
                    var filepath = _hostingEnv.ContentRootPath + $@"{_basePath}\{filename}";

                    // TODO change this to be more generic
                    string baseURL = "http://localhost:5000/histo/";
                    string fileURL = $@"{baseURL}/{filename}";
                    string filenameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
                    string thumbURL = $@"{baseURL}/{filenameWithoutExtension}-thumbnail.jpg";
                    string deleteURL = $@"";
                    responseModels.Add(new BlueimpUploadJsonResponseModel(filename, file.Length, fileURL, thumbURL, deleteURL, "DELETE", ""));


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

        [HttpPost("[action]")]
        public void OnAfterSuccessfulUploadBlueimp(string filename)
        {
            try
            {
                var filepath = _hostingEnv.ContentRootPath + $@"{_basePath}\{filename}";
                if (System.IO.File.Exists(filepath))
                {
                    _wsiProcessor.GenerateThumbnailAndDzi(filepath);
                }
            }
            catch (Exception e)
            {
                //TODO
                throw e;
            }
        }


        //TODO
        [HttpPost("[action]")]
        public void RemoveBlueimp(IList<IFormFile> UploadFiles)
        {
            try
            {
                var filepath = _hostingEnv.ContentRootPath + $@"{_basePath}\{UploadFiles[0].FileName}";
                if (System.IO.File.Exists(filepath))
                {
                    System.IO.File.Delete(filepath);
                }
            }
            catch (Exception e)
            {
                Response.Clear();
                Response.StatusCode = 200;
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "File removed successfully";
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = e.Message;
            }
        }
    }
}
