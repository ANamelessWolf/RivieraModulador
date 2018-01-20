using Nameless.Libraries.Yggdrasil.Aerith.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DaSoft.Riviera.Modulador.Core.Model
{
    /// <summary>
    /// Defines a filter for Microsoft Access Files
    /// </summary>
    public class MicrosoftAccessFilter : AerithFilter
    {
        /// <summary>
        /// Determines whether [is directory valid] [the specified directory].
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <returns>
        ///   <c>true</c> if [is directory valid] [the specified directory]; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="NotSupportedException">Not needed</exception>
        public override bool IsDirectoryValid(DirectoryInfo directory)
        {
            return true;
        }
        /// <summary>
        /// Determines whether [is file valid] [the specified file].
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>
        ///   <c>true</c> if [is file valid] [the specified file]; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsFileValid(FileInfo file)
        {
            String[] valid_ext = new String[] { ".MDB", ".ACCDB" };
            return valid_ext.Contains(file.Extension.ToUpper());
        }
    }
}
