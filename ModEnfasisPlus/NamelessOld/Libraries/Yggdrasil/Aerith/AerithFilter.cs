using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.Yggdrasil.Aerith
{
    public abstract class AerithFilter : NamelessObject
    {
        /// <summary>
        /// Check if the file info is valid by the Aerith filter
        /// </summary>
        /// <param name="file">The file to filter</param>
        /// <returns>True if the file pass the filter</returns>
        public abstract Boolean IsFileValid(FileInfo file);
        /// <summary>
        /// Check if the directory info is valid by the Aerith filter
        /// </summary>
        /// <param name="directory">The directory to filter</param>
        /// <returns>True if the directory pass the filter</returns>
        public abstract Boolean IsDirectoryValid(DirectoryInfo directory);
    }
}
