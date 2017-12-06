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
        /******* App Categories *****/
        /****************************/
        public const string CAT_ROOT = "Riviera";
        public const string CAT_DANTE = "DaNTe";
        public const String CAT_LOGIN = "Credentials";
        /****************************/
        /******* App Properties *****/
        /****************************/
        public const String PROP_PROJECT_NAME = "Project";
        public const String PROP_3D_MODE_ENABLED = "ViewMode3D";
        public const String PROP_ARNES_ENABLED = "UsarArnes";
        public const String PROP_CRED_USERNAME = "DB_Username";
        public const String PROP_CRED_PASSWORD = "DB_Password";
        public const String PROP_CRED_COMPANY = "DB_Company";
        public const String PROP_CRED_PROJECTID = "DB_ProjectId";
        public const String PROP_DAN_MDB_PATH = "DaNTePath";
        public const String PROP_DAN_ASOC_PATH = "AsocDirPath";
        public const String PROP_DAN_MOD_PATH = "ModulesDirPath";
        public const String PROP_DAN_ASOC = "SelectAsocPath";
        public const String PROP_DAN_UNITS = "Unidades";
        public const String PROP_ENABLE_LOG = "LogStatus";

        /// <summary>
        /// Gets the application categories.
        /// </summary>
        /// <param name="application">The riviera application.</param>
        /// <param name="baseCategories">The nameless base categories.</param>
        /// <returns>The application categories</returns>
        public static CategoryDefinition[] GetApplicationCategories(this RivieraApplication application, CategoryDefinition[] baseCategories)
        {
            return baseCategories.Union(new CategoryDefinition[]
            {
                new CategoryDefinition(CAT_ROOT, new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>(PROP_PROJECT_NAME, ""),
                    new KeyValuePair<string, string>(PROP_3D_MODE_ENABLED, false.ToString()),
                    new KeyValuePair<string, string>(PROP_ARNES_ENABLED, true.ToString())
                }),
                new CategoryDefinition(CAT_LOGIN, new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>(PROP_CRED_USERNAME, ""),
                    new KeyValuePair<string, string>(PROP_CRED_PASSWORD, ""),
                    new KeyValuePair<string, string>(PROP_CRED_COMPANY, ""),
                    new KeyValuePair<string, string>(PROP_CRED_PROJECTID, ""),
                }),
                new CategoryDefinition(CAT_DANTE, new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>(PROP_DAN_MDB_PATH, ""),
                    new KeyValuePair<string, string>(PROP_DAN_ASOC_PATH, ""),
                    new KeyValuePair<string, string>(PROP_DAN_ASOC, ""),
                    new KeyValuePair<string, string>(PROP_DAN_MOD_PATH, ""),
                    new KeyValuePair<string, string>(PROP_DAN_UNITS, "0"),
                })
            }).ToArray();
        }
     
    }
}
