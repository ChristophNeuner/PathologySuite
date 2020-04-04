using System;
using System.Collections.Generic;
using System.Text;

namespace PathologySuite.Shared.Models
{
    public class BlueimpDeleteJsonResponseModel
    {
        public BlueimpDeleteJsonResponseModel(string name, bool success)
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

