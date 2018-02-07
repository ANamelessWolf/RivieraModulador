using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.Yggdrasil.Medaka
{
    public interface IMedakaApplication
    {
        /// <summary>
        /// Application Directory
        /// </summary>
        DirectoryInfo AppDirectory { get; }
        /// <summary>
        /// Configuration File
        /// </summary>
        FileInfo ConfigurationFile { get; }
        /// <summary>
        /// Application Version
        /// </summary>
        String AppVersion { get; set; }
        /// <summary>
        /// Last Access application date
        /// </summary>
        DateTime Last_Access_Date { get; set; }
    }
}
