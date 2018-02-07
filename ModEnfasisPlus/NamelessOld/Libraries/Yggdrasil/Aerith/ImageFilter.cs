using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.Yggdrasil.Aerith
{
    public class ImageFilter : AerithFilter
    {
        /// <summary>
        /// The image filter valid extensions
        /// </summary>
        public String[] Extensions
        {
            get
            {
                return new String[]
                {
                    "tif", "tiff", "gif", "jpeg", "jpg",
                    "jif", "jfif", "jp2", "jpx", "j2k",
                    "j2c", "fpx", "pcd", "png", "bmp"
                };
            }
        }
        /// <summary>
        /// Check if the directory has at least one image file
        /// </summary>
        /// <param name="directory">The image directory</param>
        /// <returns>True if the directory has images</returns>
        public override bool IsDirectoryValid(DirectoryInfo directory)
        {
            return directory.GetFiles().Count(x => IsFileValid(x)) > 0;
        }
        /// <summary>
        /// Check if the file is an image
        /// </summary>
        /// <param name="file">The file to validate</param>
        /// <returns>true id the file is valid</returns>
        public override bool IsFileValid(FileInfo file)
        {
            return this.Extensions.Contains(file.Extension.ToLower());
        }
    }
}
