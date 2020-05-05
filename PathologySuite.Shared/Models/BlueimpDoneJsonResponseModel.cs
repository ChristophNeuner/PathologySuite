using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathologySuite.Shared.Models
{
    public class BlueimpDoneJsonResponseModel
    {
        public BlueimpDoneJsonResponseModel(string name, bool success)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }


            Name = name;
            Success = success;
        }

        public string Name { get; }
        public bool Success { get; }
    }
}
