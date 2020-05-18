using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathologySuite.Shared.Models
{
    public class PathologySuiteDatabaseSettings : IPathologySuiteDatabaseSettings
    {
        public string WSIsCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IPathologySuiteDatabaseSettings
    {
        string WSIsCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
