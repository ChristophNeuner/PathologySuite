using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PathologySuite.Shared.Core.Interfaces
{
    public interface IWsiProcessor
    {
        void GenerateThumbnailAndDzi(string path);
    }
}
