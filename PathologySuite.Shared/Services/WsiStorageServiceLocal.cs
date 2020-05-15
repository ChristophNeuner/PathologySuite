﻿using Microsoft.AspNetCore.Http;
using PathologySuite.Shared.Options;
using System;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PathologySuite.Shared.Core.Interfaces;

namespace PathologySuite.Shared.Services.Interfaces
{
    public class WsiStorageServiceLocal : IWsiStorageService
    {
        private readonly PathOptions _pathOptions;
        private readonly IWsiProcessor _wsiProcessor;

        public WsiStorageServiceLocal(IServiceProvider serviceProvider)
        {
            _pathOptions = serviceProvider.GetService<PathOptions>();
            _wsiProcessor = serviceProvider.GetService<IWsiProcessor>();

            if (!System.IO.Directory.Exists(_pathOptions.WsiBasePath))
            {
                System.IO.Directory.CreateDirectory(_pathOptions.WsiBasePath);
            }
        }

        public async Task<bool> DeleteAsync(string filename)
        {

            filename = Path.GetFileNameWithoutExtension(filename);
            try
            {
                foreach (string file in System.IO.Directory.GetFiles($@"{ _pathOptions.WsiBasePath}/"))
                {
                    if (file.Contains(filename))
                    {
                        System.IO.File.Delete(file);

                        foreach (string dir in System.IO.Directory.GetDirectories($@"{ _pathOptions.WsiBasePath}/"))
                        {
                            if (dir.Contains(filename))
                            {
                                System.IO.Directory.Delete(dir);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //TODO
                throw e;
            }

            return true;
        }

        public async Task<bool> NotifyUploadCompleteAsync(string filename)
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

            return true;
        }

        public async Task SaveAsync(IFormFile file)
        {
            var filename = ContentDispositionHeaderValue
                    .Parse(file.ContentDisposition)
                    .FileName
                    .Trim('"');
            var filepath = $@"{_pathOptions.WsiBasePath}/{filename}";

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
    }
}