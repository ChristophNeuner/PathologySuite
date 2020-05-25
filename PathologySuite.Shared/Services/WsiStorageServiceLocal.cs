using Microsoft.AspNetCore.Http;
using PathologySuite.Shared.Options;
using System;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PathologySuite.Shared.Core.Interfaces;
using PathologySuite.Shared.Models;

namespace PathologySuite.Shared.Services.Interfaces
{
    public class WsiStorageServiceLocal : IWsiStorageService
    {
        private readonly PathOptions _pathOptions;
        private readonly IWsiProcessor _wsiProcessor;
        private readonly WsiDbService _wsiDbService;

        public WsiStorageServiceLocal(IServiceProvider serviceProvider)
        {
            _pathOptions = serviceProvider.GetService<PathOptions>();
            _wsiProcessor = serviceProvider.GetService<IWsiProcessor>();
            _wsiDbService = serviceProvider.GetService<WsiDbService>();

            if (!System.IO.Directory.Exists(_pathOptions.WsiBasePath))
            {
                System.IO.Directory.CreateDirectory(_pathOptions.WsiBasePath);
            }
        }

        public async Task DeleteAsync(string filename)
        {

            string filenameWithoutExt = Path.GetFileNameWithoutExtension(filename);
            try
            {
                foreach (string file in System.IO.Directory.GetFiles($@"{ _pathOptions.WsiBasePath}/"))
                {
                    if (file.Contains(filenameWithoutExt))
                    {
                        System.IO.File.Delete(file);

                        foreach (string dir in System.IO.Directory.GetDirectories($@"{ _pathOptions.WsiBasePath}/"))
                        {
                            if (dir.Contains(filenameWithoutExt))
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
        }

        public async Task<WholeSlideImage> NotifyUploadCompleteAsync(string filename)
        {
            try
            {
                string wsiPath = Path.Combine(new string[] { _pathOptions.WsiBasePath, filename });
                if (System.IO.File.Exists(wsiPath))
                {
                    byte[] thumbnail = await _wsiProcessor.GenerateThumbnail(await File.ReadAllBytesAsync(wsiPath));
                    string thumbPath = Path.Combine(new string[] { _pathOptions.WsiBasePath, Utils.GetThumbnailName(filename) });
                    await File.WriteAllBytesAsync(thumbPath, thumbnail);
                }
            }
            catch (Exception e)
            {
                //TODO
                throw e;
            }

            return new WholeSlideImage(id: Utils.GetGuidFromFilename(filename, _pathOptions.GuidSeparator),
                filename: filename,
                fileExtension: Path.GetExtension(filename),
                physicalPathWsi: Path.Combine(new string[] { _pathOptions.WsiBasePath, filename }),
                physcialPathThumbnail: Path.Combine(new string[] { _pathOptions.WsiBasePath, Utils.GetThumbnailName(filename) }),
                completelyUploaded: true);
        }

        public async Task<WholeSlideImage> SaveAsync(IFormFile file)
        {
            var filename = Utils.GetFilenameFromIFormFile(file);
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

            return new WholeSlideImage(id: Utils.GetGuidFromFilename(filename, _pathOptions.GuidSeparator),
                            filename: filename,
                            fileExtension: Path.GetExtension(filename),
                            physicalPathWsi: Path.Combine(new string[] { _pathOptions.WsiBasePath, filename }),
                            physcialPathThumbnail: Path.Combine(new string[] { _pathOptions.WsiBasePath, Utils.GetThumbnailName(filename) }),
                            completelyUploaded: false);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="wsi"></param>
        /// <returns>byte[] of the thumbnail image or null if the thumbnail generation has not finished yet</returns>
        public async Task<byte[]?> GetThumbnailAsync(WholeSlideImage wsi)
        {
            try
            {
                return await File.ReadAllBytesAsync(wsi.PhysicalPathThumbnail);
            }
            catch(Exception e)
            {
                Console.WriteLine($"thumbnail of {wsi.Filename} not completely generated");
                return null;
            }
        }
    }
}
