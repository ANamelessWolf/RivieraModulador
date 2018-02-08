using DaSoft.Riviera.OldModulador.Model;
using NamelessOld.Libraries.DB.Mikasa.Model;
using NamelessOld.Libraries.DB.Misa.Model;
using NamelessOld.Libraries.Yggdrasil.Alice;
using NamelessOld.Libraries.Yggdrasil.Medaka;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using IoDirectory = System.IO.Directory;
using IoDirectoryInfo = System.IO.DirectoryInfo;
using IoFile = System.IO.File;
using IoFileInfo = System.IO.FileInfo;
using IoPath = System.IO.Path;

namespace DaSoft.Riviera.OldModulador.Runtime
{
    public class RivieraApplication : NamelessApplication
    {
        #region Constants
        const string APP_NAME = "Addin_App";
        const string FOLDER_NAME_BLOCKS_DELTA = "Delta";
        const string FOLDER_NAME_BLOCKS_2D = "2D";
        const string FOLDER_NAME_BLOCKS_3D = "3D";
        //Configuration nodes
        const string CAT_ROOT = "Riviera";
        const string CAT_DANTE = "DaNTe";
        const String CAT_LOGIN = "Credentials";
        const string ORACLE_CONN = "oracleOld.conn";
        const string ACCESS_CONN = "accessOld.conn";
        //Options
        const String PROP_PROJECT_NAME = "Project";
        const String PROP_3D_MODE_ENABLED = "ViewMode3D";
        const String PROP_ARNES_ENABLED = "UsarArnes";
        const String PROP_CRED_USERNAME = "DB_Username";
        const String PROP_CRED_PASSWORD = "DB_Password";
        const String PROP_CRED_COMPANY = "DB_Company";
        const String PROP_CRED_PROJECTID = "DB_ProjectId";
        const String PROP_DAN_MDB_PATH = "DaNTePath";
        const String PROP_DAN_ASOC_PATH = "AsocDirPath";
        const String PROP_DAN_MOD_PATH = "ModulesDirPath";
        const String PROP_DAN_ASOC = "SelectAsocPath";
        const String PROP_DAN_UNITS = "Unidades";
        const String PROP_ENABLE_LOG = "LogStatus";
        #endregion
        #region Configuration File Nodes
        //Configuration nodes
        public string[] categories = new string[] { CAT_ROOT, CAT_LOGIN, CAT_DANTE };
        //Configuration properties
        string[][] properties = new string[][] {  //El primer nodo es el de Root
                                                   new string[]
                                                   {
                                                       PROP_PROJECT_NAME, PROP_3D_MODE_ENABLED, PROP_ARNES_ENABLED,
                                                       PROP_ENABLE_LOG
                                                   },
                                                   new string[]
                                                   {
                                                       PROP_CRED_USERNAME, PROP_CRED_PASSWORD, PROP_CRED_COMPANY,
                                                       PROP_CRED_PROJECTID
                                                   },
                                                   new string[]
                                                   {
                                                       PROP_DAN_MDB_PATH, PROP_DAN_ASOC_PATH, PROP_DAN_ASOC,
                                                       PROP_DAN_MOD_PATH, PROP_DAN_UNITS
                                                   },
                                               };
        //Configuration node properties default value
        string[][] propertiesDefaultData = new string[][] { 
                                                   //El primer nodo es el de Root
                                                   new string[]
                                                   {
                                                       "", false.ToString(), true.ToString(),
                                                       true.ToString()
                                                   },
                                                   new string[]
                                                   {
                                                       "", "", "",
                                                       ""
                                                   },
                                                   new string[]
                                                   {
                                                       "", "", "",
                                                       "", "0"
                                                   },
                                               };

        #endregion
        public OracleConnectionBuilder ConnectionBuilder;
        /// <summary>
        /// The DaNTe current template
        /// </summary>
        public FileInfo DaNTeDBTemplate
        {
            get
            {
                return new FileInfo(Path.Combine(this.AppDirectory.FullName, "bases[Default].mdb"));
            }
        }
        /// <summary>
        /// La ruta al archivo DaNTe MDB
        /// </summary>
        public FileInfo DaNTeMDB
        {
            get
            {
                string pth = this.Configuration[CAT_DANTE][PROP_DAN_MDB_PATH];
                if (pth != String.Empty && File.Exists(pth))
                    return new FileInfo(pth);
                else
                    return null;
            }
            set
            {
                this.Configuration[CAT_DANTE][PROP_DAN_MDB_PATH] = value.FullName.ToString();
            }
        }
        /// <summary>
        /// La ruta al archivo Bases Modulos MDB
        /// </summary>
        public FileInfo ModulosMDB
        {
            get
            {
                if (DaNTeMDB != null)
                {
                    string pth = Path.Combine(this.DaNTeMDB.Directory.FullName, "BASES MODULOS.mdb");
                    if (pth != String.Empty && File.Exists(pth))
                        return new FileInfo(pth);
                    else
                        return null;
                }
                else
                    return null;
            }
        }
        /// <summary>
        /// La ruta al archivo Asociado MDB seleccionado
        /// </summary>
        public FileInfo AsociadoMDB
        {
            get
            {
                string mdb = this.Configuration[CAT_DANTE][PROP_DAN_ASOC],
                    pth = String.Empty;
                if (Asociados != null)
                    pth = Path.Combine(Asociados.FullName, mdb);
                if (pth != String.Empty && File.Exists(pth))
                    return new FileInfo(pth);
                else
                    return null;
            }
            set { this.Configuration[CAT_DANTE][PROP_DAN_ASOC] = value.Name; }
        }
        /// <summary>
        /// La ruta al directorio de asociados de DaNTe
        /// </summary>
        public DirectoryInfo Asociados
        {
            get
            {
                string pth = this.Configuration[CAT_DANTE][PROP_DAN_ASOC_PATH];
                if (pth != String.Empty && Directory.Exists(pth))
                    return new DirectoryInfo(pth);
                else
                    return null;
            }
            set { this.Configuration[CAT_DANTE][PROP_DAN_ASOC_PATH] = value.FullName.ToString(); }
        }
        /// <summary>
        /// Las unidades usadas en la aplicación
        /// </summary>
        public DaNTeUnits Units
        {
            get
            {
                string strNum = this.Configuration[CAT_DANTE][PROP_DAN_UNITS];
                int num;
                return (DaNTeUnits)(int.TryParse(strNum, out num) ? num : -1);
            }
            set { this.Configuration[CAT_DANTE][PROP_DAN_UNITS] = ((int)value).ToString(); }
        }
        /// <summary>
        /// La ruta al directorio de modulos de DaNTe
        /// </summary>
        public DirectoryInfo Modules
        {
            get
            {
                string pth = this.Configuration[CAT_DANTE][PROP_DAN_MOD_PATH];
                if (pth != String.Empty && Directory.Exists(pth))
                    return new DirectoryInfo(pth);
                else
                    return null;
            }
            set { this.Configuration[CAT_DANTE][PROP_DAN_MOD_PATH] = value.FullName.ToString(); }
        }
        /// <summary>
        /// La ruta al directorio de modulos dwg 2D
        /// </summary>
        public DirectoryInfo Modules2D
        {
            get
            {
                if (Modules != null)
                {
                    if (!Directory.Exists(Path.Combine(Modules.FullName, "dwg2d")))
                        Directory.CreateDirectory(Path.Combine(Modules.FullName, "dwg2d"));
                    return new DirectoryInfo(Path.Combine(Modules.FullName, "dwg2d"));
                }
                else
                    return null;
            }
        }
        /// <summary>
        /// La ruta al directorio de modulos dwg 3D
        /// </summary>
        public DirectoryInfo Modules3D
        {
            get
            {
                if (Modules != null)
                {
                    if (!Directory.Exists(Path.Combine(Modules.FullName, "dwg3d")))
                        Directory.CreateDirectory(Path.Combine(Modules.FullName, "dwg3d"));
                    return new DirectoryInfo(Path.Combine(Modules.FullName, "dwg3d"));
                }
                else
                    return null;
            }
        }
        /// <summary>
        /// Connection Object
        /// </summary>
        public ConnectionData Connection
        {
            get
            {
                if (ConnectionBuilder != null)
                    return new ConnectionData(this.Key, this.IV, this.ConnectionFile, new RivieraConnectionContent(ConnectionBuilder));
                else
                    return null;
            }
        }
        /// <summary>
        /// The access connection file
        /// </summary>
        public FileInfo AccessConnectionFile { get { return new FileInfo(ACCESS_CONN); } }
        /// <summary>
        /// Get the application credentials
        /// </summary>
        public UserCredential Credentials
        {
            get
            {
                string pass;
                int com, pId;
                try { pass = this.Decrypt(this.Configuration[CAT_LOGIN][PROP_CRED_PASSWORD]); }
                catch (Exception) { pass = String.Empty; }
                return new UserCredential()
                {
                    Company = (RivieraCompany)(int.TryParse(this.Configuration[CAT_LOGIN][PROP_CRED_COMPANY], out com) ? com : -1),
                    Username = this.Configuration[CAT_LOGIN][PROP_CRED_USERNAME],
                    Password = pass,
                    ProjectId = int.TryParse(this.Configuration[CAT_LOGIN][PROP_CRED_PROJECTID], out pId) ? pId : -1,
                };
            }
            set
            {
                this.Configuration[CAT_LOGIN][PROP_CRED_COMPANY] = ((int)value.Company).ToString();
                this.Configuration[CAT_LOGIN][PROP_CRED_USERNAME] = value.Username.ToString();
                this.Configuration[CAT_LOGIN][PROP_CRED_PASSWORD] = this.Encrypt(value.Password);
                this.Configuration[CAT_LOGIN][PROP_CRED_PROJECTID] = value.ProjectId.ToString();
            }
        }
        /// <summary>
        /// Gets the application DB file
        /// </summary>
        public IoFileInfo ConnectionFile { get { return new IoFileInfo(IoPath.Combine(this.AppDirectory.FullName, ORACLE_CONN)); } }

        /// <summary>
        /// 2D Blocks
        /// </summary>
        public IoFileInfo[] Delta2D
        {
            get
            {
                String pth = IoPath.Combine(this.AppDirectory.FullName, FOLDER_NAME_BLOCKS_DELTA, FOLDER_NAME_BLOCKS_2D);
                if (IoDirectory.Exists(pth))
                    return new IoDirectoryInfo(pth).GetFiles();
                else
                    return new IoFileInfo[0];
            }
        }
        /// <summary>
        /// Gets the application DB file
        /// </summary>
        public IoFileInfo[] Delta3D
        {
            get
            {
                String pth = IoPath.Combine(this.AppDirectory.FullName, FOLDER_NAME_BLOCKS_DELTA, FOLDER_NAME_BLOCKS_3D);
                if (IoDirectory.Exists(pth))
                    return new IoDirectoryInfo(pth).GetFiles();
                else
                    return new IoFileInfo[0];
            }
        }
        /// <summary>
        /// Gets or sets the 3d view mode enabled status
        /// </summary>
        public bool Is3DEnabled
        {
            get { return Boolean.Parse(this.Configuration[CAT_ROOT][PROP_3D_MODE_ENABLED]); }
            set { this.Configuration[CAT_ROOT][PROP_3D_MODE_ENABLED] = value.ToString(); }
        }

        /// <summary>
        /// Activa o desactiva la opción para usar un arnes
        /// </summary>
        public bool IsArnesEnabled
        {
            get { return Boolean.Parse(this.Configuration[CAT_ROOT][PROP_ARNES_ENABLED]); }
            set { this.Configuration[CAT_ROOT][PROP_ARNES_ENABLED] = value.ToString(); }
        }

        /// <summary>
        /// Activa o desactiva el uso del log de la aplicación
        /// </summary>
        public bool LogIsEnabled
        {
            get { return Boolean.Parse(this.Configuration[CAT_ROOT][PROP_ENABLE_LOG]); }
            set
            {
                this.Configuration[CAT_ROOT][PROP_ENABLE_LOG] = value.ToString();
                this.Log.GhostMode = value;
            }
        }
        /// <summary>
        /// Para pruebas esta bandera habilita las herramientas de desarrollo
        /// </summary>
        public bool DeveloperMode;

        /// <summary>
        /// Creates a new Riviera application
        /// </summary>
        /// <param name="applicationName">The name of the application</param>
        public RivieraApplication(String applicationName) :
            base(APP_NAME)
        {
            this.AppVersion = Assembly.GetAssembly(typeof(RivieraApplication)).GetName().Version.ToString(4);
            this.Last_Access_Date = new IoFileInfo(Assembly.GetAssembly(typeof(RivieraApplication)).Location).LastWriteTime;
            App.DB = new Database();
            this.Log.GhostMode = LogIsEnabled;
            //Activa la bandera para el modo de desarrollador
            this.DeveloperMode = false;
            if (IoFile.Exists(ConnectionFile.FullName))
            {
                KeyValuePair<String, String>[] fileData;
                ConnectionData.ExtractConnData(new Caterpillar(this.Key, this.IV), this.ConnectionFile, out fileData);
                OracleConnectionContent content = new OracleConnectionContent(fileData);
                this.ConnectionBuilder = content.AsBuilder;
            }
        }


        /// <summary>
        /// Initialize the configuration file
        /// </summary>
        /// <param name="conf">The configuration object</param>
        protected override void InitConfiguration(ref ConfigMedaka conf)
        {
            base.InitConfiguration(ref conf);
            this.Fill(ref conf, this.categories, this.properties, this.propertiesDefaultData, false);
        }
        /// <summary>
        /// Updates the configuration file
        /// </summary>
        /// <param name="conf">The configuration object</param>
        protected override void UpdateConfiguration(ref ConfigMedaka conf)
        {
            base.UpdateConfiguration(ref conf);
            this.Fill(ref conf, this.categories, this.properties, this.propertiesDefaultData, true);
        }
    }
}
