using NamelessOld.Libraries.DB.Mikasa.Model;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;

namespace NamelessOld.Libraries.DB.Tessa.Model
{
    public class AccessConnectionContent : ConnectionContent
    {
        const String CONN_USER_ID = "User ID";
        const String FIELD_PASSWORD = "Password";
        const String FIELD_OLEDB_PROVIDER = "OleDbProvider";
        const String FIELD_INTERFACE = "ConnectionInterface";
        const String FIELD_USERNAME = "Username";
        const String FIELD_ACCESS_PATH = "Db_Path";
        const String FIELD_PERSIST_SECURITY = "PersistSecurityInfo";
        /// <summary>
        /// Access the Oledb Provider
        /// </summary>
        public AccessProvider OleDbProvider { get { int prEnum; if (int.TryParse(this[FIELD_OLEDB_PROVIDER], out prEnum)) return new AccessProvider((OledbProviders)prEnum); else return new AccessProvider(OledbProviders.UndefinedProvider); } set { this[FIELD_OLEDB_PROVIDER] = ((int)value.Provider).ToString(); } }
        /// <summary>
        /// Access the path of the Access database file.
        /// </summary>
        public String Access_DB_File { get { return this[FIELD_ACCESS_PATH]; } set { this[FIELD_ACCESS_PATH] = value; } }
        /// <summary>
        /// Access the persist security
        /// </summary>
        public Boolean PersistSecurity { get { Boolean flag; if (Boolean.TryParse(this[FIELD_PERSIST_SECURITY], out flag)) return flag; else return false; } set { this[FIELD_PERSIST_SECURITY] = value.ToString(); } }

        /// <summary>
        /// Creates a new Access connection content
        /// </summary>
        /// <param name="values">
        /// The connection values in this Order
        /// </param>
        public AccessConnectionContent(params String[] values)
            : base(new KeyValuePair<String, String>(FIELD_INTERFACE, values[0]),
                   new KeyValuePair<String, String>(FIELD_USERNAME, values[1]),
                   new KeyValuePair<String, String>(FIELD_PASSWORD, values[2]),
                   new KeyValuePair<String, String>(FIELD_OLEDB_PROVIDER, values[3]),
                   new KeyValuePair<String, String>(FIELD_ACCESS_PATH, values[4]),
                   new KeyValuePair<String, String>(FIELD_PERSIST_SECURITY, values[5]))
        {

        }
        /// <summary>
        /// Creates a new Access connection content
        /// </summary>
        /// <param name="filePath">The file path</param>
        public AccessConnectionContent(FileInfo filePath, String user = "", String pass = "")
            : this(((int)ConnectionInterface.Access).ToString(), //Interface
                    user,
                    pass,
                    ((int)(new AccessProvider(OledbProviders.Microsoft_ACE_OleDb_12)).Provider).ToString(), //Access Provider
                    filePath.FullName, //Access file path
                    "true")
        {

        }
        /// <summary>
        /// Creates a new Access connection content
        /// </summary>
        /// <param name="content">The connection content data</param>
        public AccessConnectionContent(params KeyValuePair<String, String>[] content)
            : base(content)
        {

        }
        /// <summary>
        /// Creates the Access Connection String
        /// </summary>
        /// <returns>The connection string to connect to oracle</returns>
        public override string GenerateConnectionString()
        {
            OleDbConnectionStringBuilder conStr = new OleDbConnectionStringBuilder();
            conStr.DataSource = this.Access_DB_File;
            conStr[CONN_USER_ID] = this.Username;
            conStr[FIELD_PASSWORD] = this.Password;
            conStr.Provider = this.OleDbProvider.ToString();
            conStr.PersistSecurityInfo = this.PersistSecurity;
            return conStr.ToString();
        }
    }
}
