﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathologySuite.Shared
{
    public static class Utils
    {
        /// <summary>
        /// Expects filename of form: f1d5c415-81b3-43bd-b193-c66afe73e21c{guidSeparator}155-19-III-ACTH.ndpi
        /// where $$$==guid==$$$ separates guid and "real" filename
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>filename without guid and separator</returns>
        public static string RemoveGuid(string filename, string guidSeparator)
        {
            return filename.Split(guidSeparator)[1];
        }


        /// <summary>
        /// Expects filename of form: f1d5c415-81b3-43bd-b193-c66afe73e21c{guidSeparator}155-19-III-ACTH.ndpi
        /// where $$$==guid==$$$ separates guid and "real" filename
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>guid from the filename</returns>
        public static string GetGuidFromFilename(string filename, string guidSeparator)
        {
            return filename.Split(guidSeparator)[0];
        }
    }
}
