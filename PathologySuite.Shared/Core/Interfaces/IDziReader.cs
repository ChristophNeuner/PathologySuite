using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PathologySuite.Shared.Core;

namespace PathologySuite.Shared.Core.Interfaces
{
    public interface IDziReader
    {
        DziValuesWrapperNetVips ReadDziFile(string path);
    }
}
