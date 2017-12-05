using DaSoft.Riviera.Modulador.Core.Model;
using DaSoft.Riviera.Modulador.Core.Runtime;
using Nameless.Libraries.Yggdrasil.Medaka;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Environment;
namespace DaSoft.Riviera.Modulador.Core.Controller
{
    /// <summary>
    /// Defines the application utils class
    /// </summary>
    public static partial class ApplicationUtils
    {
        /****************************/
        /******* App Names **********/
        /****************************/
        const String APP_COMPANY = "DaSoft";
        const String APP_RIVIERA_FOLDER = "Riviera";
        const String APP_APPLICATION_FOLDER = "aplicaciones";
        const String APP_QUANTIFY_FOLDER = "Cuantificaciones";
        const String APP_PRODUCT = "DaNTe";
        public const String APP_DANTE_BASES = "Bases.MDB";
        /// <summary>
        /// Gets the DaNte path.
        /// </summary>
        /// <param name="startDirectory">The start directory.</param>
        /// <param name="subFolder">The sub folder.</param>
        /// <returns>The subfolder path</returns>
        public static string GetDaNTePath(SpecialFolder startDirectory, String subFolder = "")
        {
            if (subFolder != "")
                return Path.Combine(Environment.GetFolderPath(startDirectory), APP_COMPANY, APP_PRODUCT, subFolder);
            else
                return Path.Combine(Environment.GetFolderPath(startDirectory), APP_COMPANY, APP_PRODUCT);
        }
        /// <summary>
        /// Checks the DaNte path.
        /// </summary>
        /// <returns>The DaNTe path</returns>
        public static string CheckDaNTePath()
        {
            if (Directory.Exists(Path.Combine(@"C:\", APP_PRODUCT)))
                return Path.Combine(@"C:\", APP_PRODUCT);
            else if (Directory.Exists(GetDaNTePath(SpecialFolder.ProgramFilesX86)))
                return GetDaNTePath(SpecialFolder.ProgramFilesX86);
            else
                return @"C:\";
        }
        /// <summary>
        /// Checks the DaNte quantification path.
        /// </summary>
        /// <returns>The quantification path</returns>
        public static string CheckQuantifyPath()
        {
            if (Directory.Exists(Path.Combine(@"C:\", APP_PRODUCT, APP_QUANTIFY_FOLDER)))
                return Path.Combine(@"C:\", APP_PRODUCT, APP_QUANTIFY_FOLDER);
            else if (Directory.Exists(GetDaNTePath(SpecialFolder.ProgramFilesX86, APP_QUANTIFY_FOLDER)))
                return GetDaNTePath(SpecialFolder.ProgramFilesX86, APP_QUANTIFY_FOLDER);
            else
                return @"C:\";
        }
        /// <summary>
        /// Checks the DaNte quantification path.
        /// </summary>
        /// <returns>The quantification path</returns>
        public static string CheckAsocPath()
        {
            if (Directory.Exists(Path.Combine(@"C:\", APP_PRODUCT, APP_APPLICATION_FOLDER, APP_RIVIERA_FOLDER)))
                return Path.Combine(@"C:\", APP_PRODUCT, APP_APPLICATION_FOLDER, APP_RIVIERA_FOLDER);
            else if (Directory.Exists(GetDaNTePath(SpecialFolder.ProgramFilesX86, Path.Combine(APP_APPLICATION_FOLDER, APP_RIVIERA_FOLDER))))
                return GetDaNTePath(SpecialFolder.ProgramFilesX86, Path.Combine(APP_APPLICATION_FOLDER, APP_RIVIERA_FOLDER));
            else
                return @"C:\";
        }
        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="path">The application desired path.</param>
        /// <returns>The path as string</returns>
        public static string GetPath(this RivieraApplication app, DaNTePath path, string dftl_pth = null)
        {
            string pth = null;
            Boolean isDirPath = true;
            switch (path)
            {
                case DaNTePath.DANTE_BASES:
                    pth = app[CAT_DANTE][PROP_DAN_MDB_PATH];
                    isDirPath = false;
                    break;
                case DaNTePath.ASOC_DIR:
                    pth = app[CAT_DANTE][PROP_DAN_ASOC_PATH];
                    break;
                case DaNTePath.ASOC_MDB:
                    pth = Path.Combine(app[CAT_DANTE][PROP_DAN_ASOC_PATH], app[CAT_DANTE][PROP_DAN_ASOC]);
                    isDirPath = false;
                    break;
                case DaNTePath.MOD_DIR:
                    pth = app[CAT_DANTE][PROP_DAN_MOD_PATH];
                    break;
                case DaNTePath.DANTE_DIR:
                    pth = new FileInfo(app[CAT_DANTE][PROP_DAN_MDB_PATH]).DirectoryName;
                    break;
            }
            if ((dftl_pth != null) && ((isDirPath && !Directory.Exists(pth)) || (!isDirPath && !File.Exists(pth))))
                return dftl_pth;
            else
                return pth;
        }
        /// <summary>
        /// Sets the path.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="path">The path.</param>
        /// <param name="strPath">The string path.</param>
        public static void SetPath(this RivieraApplication app, DaNTePath path, String strPath)
        {
            switch (path)
            {
                case DaNTePath.DANTE_BASES:
                    app[CAT_DANTE][PROP_DAN_MDB_PATH] = strPath;
                    break;
                case DaNTePath.ASOC_DIR:
                    app[CAT_DANTE][PROP_DAN_ASOC_PATH] = strPath;
                    break;
                case DaNTePath.ASOC_MDB:
                    app[CAT_DANTE][PROP_DAN_ASOC] = new FileInfo(strPath).Name;
                    break;
                case DaNTePath.MOD_DIR:
                    app[CAT_DANTE][PROP_DAN_MOD_PATH] = strPath;
                    break;
            }
        }
    }
}
