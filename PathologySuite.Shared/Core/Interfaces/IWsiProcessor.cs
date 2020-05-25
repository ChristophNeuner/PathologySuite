using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PathologySuite.Shared.Core.Interfaces
{
    public interface IWsiProcessor
    {
        void GenerateThumbnail(string wsiPath);
        void GenerateDzi(string wsiPath);
        Task<byte[]> GenerateThumbnail(byte[] buffer);
        Task<byte[]> GenerateDzi(byte[] buffer);
    }
}
