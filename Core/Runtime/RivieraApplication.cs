using DaSoft.Riviera.Modulador.Core.Controller;
using Nameless.Libraries.DB.Misa.Model;
using Nameless.Libraries.Yggdrasil.Medaka;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaSoft.Riviera.Modulador.Core.Model;
using Nameless.Libraries.Yggdrasil.Lilith;
using static DaSoft.Riviera.Modulador.Core.Controller.ApplicationUtils;
namespace DaSoft.Riviera.Modulador.Core.Runtime
{
    /// <summary>
    /// Defines the Riviera application
    /// </summary>
    /// <seealso cref="Nameless.Libraries.Yggdrasil.Medaka.NamelessApplication" />
    public class RivieraApplication : NamelessApplication
    {
        /// <summary>
        /// The application name
        /// </summary>
        const string APP_NAME = "RivieraMod";
        /// <summary>
        /// Enables or disables the application log
        /// </summary>
        const Boolean ENABLE_LOG = true;
        /// <summary>
        /// The oracle connection file name
        /// </summary>
        const string ORACLE_CONN = "oracle.conn";
        /// <summary>
        /// The microsoft access connection file name
        /// </summary>
        const string MS_ACCESS_CONN = "access.conn";
        /// <summary>
        /// Gets or sets the application version.
        /// </summary>
        /// <value>
        /// The application version.
        /// </value>
        public override string AppVersion
        {
            get
            {
                SuccubusAssembly assem = new SuccubusAssembly(typeof(RivieraApplication));
                return assem.ShortVersion;
            }
            set => base.AppVersion = value;
        }
        /// <summary>
        /// Gets or sets the last access.
        /// </summary>
        /// <value>
        /// The last access.
        /// </value>
        public override DateTime Last_Access
        {
            get
            {
                SuccubusAssembly assem = new SuccubusAssembly(typeof(RivieraApplication));
                return assem.File.CreationTime;
            }
            set => base.Last_Access = value;
        }
        /// <summary>
        /// Gets or sets the 3d view mode enabled status
        /// </summary>
        public bool Is3DEnabled { get { return Boolean.Parse(this[CAT_ROOT][PROP_3D_MODE_ENABLED]); } set { this[CAT_ROOT][PROP_3D_MODE_ENABLED] = value.ToString(); } }
        /// <summary>
        /// Gets the application configuration node categories
        /// </summary>
        /// <value>
        /// The application node categories.
        /// </value>
        protected override CategoryDefinition[] Categories => this.GetApplicationCategories(base.Categories);
        /// <summary>
        /// Gets the oracle connection file.
        /// </summary>
        /// <value>
        /// The oracle connection file.
        /// </value>
        public FileInfo OracleConnectionFile { get { return new FileInfo(Path.Combine(this.AppDirectory.FullName, ORACLE_CONN)); } }
        /// <summary>
        /// The riviera database
        /// </summary>
        public RivieraDatabase Database;
        /// <summary>
        /// The oracle connection data
        /// </summary>
        public OracleConnectionData OracleConnection;
        /// <summary>
        /// Gets the Microsoft Access connection file.
        /// </summary>
        /// <value>
        /// The ms access connection file.
        /// </value>
        public FileInfo MsAccessConnectionFile { get { return new FileInfo(Path.Combine(this.AppDirectory.FullName, MS_ACCESS_CONN)); } }

        public bool DeveloperMode { get; set; }

        /// <summary>
        /// The Riviera login credentials
        /// </summary>
        public UserCredential Credentials;
        /// <summary>
        /// Initializes a new instance of the <see cref="RivieraApplication"/> class.
        /// </summary>
        public RivieraApplication()
            : base(APP_NAME, ENABLE_LOG)
        {
            try
            {
                //Por default se crea la conexión a la red local de DaSoft
                if (!File.Exists(this.OracleConnectionFile.FullName))
                    this.CreateOracleConnectionFile(DBUtils.LocalRivieraServiceName);
                else
                    this.OracleConnection = new OracleConnectionData(this.OracleConnectionFile.FullName);
                this.SetApplicationInformation();
                this.Database = new RivieraDatabase();
            }
            catch (Exception exc)
            {
                this.Log.AppendEntry(exc, this, true);
            }

        }
    }
}
