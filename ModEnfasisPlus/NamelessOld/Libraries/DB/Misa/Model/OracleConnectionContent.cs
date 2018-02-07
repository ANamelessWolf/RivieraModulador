using NamelessOld.Libraries.DB.Mikasa.Model;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.DB.Misa.Model
{
    public class OracleConnectionContent : ConnectionContent
    {
        const String FIELD_INTERFACE = "ConnectionInterface";
        const String FIELD_CONNECTION_TYPE = "ConnectionType";
        const String FIELD_USERNAME = "Username";
        const String FIELD_PASSWORD = "Password";
        const String FIELD_SERVER = "Server";
        const String FIELD_PORT = "Port";
        const String FIELD_TNS = "Tns";
        const String FIELD_SERVICE_NAME = "ServiceName";
        const String FIELD_SERVICE_ID = "SID";
        const String FIELD_TIME_OUT = "TimeOut";
        const String FIELD_PERSIST_SECURITY = "PersistSecurityInfo";
        /// <summary>
        /// Oracle connection type
        /// SID, TNS, Servicename
        /// </summary>
        public Connection_Type ConnectionType { get { return (Connection_Type)int.Parse(this[FIELD_CONNECTION_TYPE]); } set { this[FIELD_CONNECTION_TYPE] = ((int)value).ToString(); } }
        /// <summary>
        /// Oracle host name or server address
        /// </summary>
        public String Server { get { return this[FIELD_SERVER]; } set { this[FIELD_SERVER] = value; } }
        /// <summary>
        /// Oracle connection port
        /// </summary>
        public int Port { get { int port; if (int.TryParse(this[FIELD_PORT], out port)) return port; else return 1521; } set { this[FIELD_PORT] = value.ToString(); } }
        /// <summary>
        /// The name of the TNS connection
        /// </summary>
        public String TNS { get { return this[FIELD_TNS]; } set { this[FIELD_TNS] = value; } }
        /// <summary>
        /// The SID connection
        /// </summary>
        public String SID { get { return this[FIELD_SERVICE_ID]; } set { this[FIELD_SERVICE_ID] = value; } }
        /// <summary>
        /// The service name connection
        /// </summary>
        public String Service_Name { get { return this[FIELD_SERVICE_NAME]; } set { this[FIELD_SERVICE_NAME] = value; } }
        /// <summary>
        /// Oracle persist security
        /// </summary>
        public Boolean PersistSecurity { get { Boolean flag; if (Boolean.TryParse(this[FIELD_PERSIST_SECURITY], out flag)) return flag; else return false; } set { this[FIELD_PERSIST_SECURITY] = value.ToString(); } }
        /// <summary>
        /// Gets the oracle connection data as builder
        /// </summary>
        public OracleConnectionBuilder AsBuilder
        {
            get
            {
                return new OracleConnectionBuilder()
                {
                    ConnectionType = this.ConnectionType,
                    Host = this.Server,
                    Password = this.Password,
                    PersistSecurity = this.PersistSecurity,
                    Port = this.Port,
                    Servicename = this.Service_Name,
                    SID = this.SID,
                    TimeOut = this.TimeOut,
                    TNS = this.TNS,
                    Username = this.Username
                };
            }
        }


        /// <summary>
        /// Creates a new Oracle connection content
        /// </summary>
        /// <param name="values">The connection values</param>
        public OracleConnectionContent(params String[] values)
            : base(new KeyValuePair<String, String>(FIELD_INTERFACE, values[0]),
                   new KeyValuePair<String, String>(FIELD_USERNAME, values[1]),
                   new KeyValuePair<String, String>(FIELD_PASSWORD, values[2]),
                   new KeyValuePair<String, String>(FIELD_SERVER, values[3]),
                   new KeyValuePair<String, String>(FIELD_PORT, values[4]),
                   new KeyValuePair<String, String>(FIELD_CONNECTION_TYPE, values[5]),
                   new KeyValuePair<String, String>(FIELD_TNS, values[6]),
                   new KeyValuePair<String, String>(FIELD_SERVICE_NAME, values[7]),
                   new KeyValuePair<String, String>(FIELD_SERVICE_ID, values[8]),
                   new KeyValuePair<String, String>(FIELD_TIME_OUT, values[9]),
                                      new KeyValuePair<String, String>(FIELD_PERSIST_SECURITY, values[10]))
        {

        }
        /// <summary>
        /// Creates a new Oracle connection content
        /// </summary>
        /// <param name="content">The connection content data</param>
        public OracleConnectionContent(params KeyValuePair<String, String>[] content)
            : base(content)
        {

        }
        /// <summary>
        /// Creates the Oracle Connection String
        /// </summary>
        /// <returns>The connection string to connect to oracle</returns>
        public override string GenerateConnectionString()
        {
            OracleConnectionStringBuilder connStr = new OracleConnectionStringBuilder();
            if (this.Username.Length > 0)
                connStr.UserID = this.Username;
            if (this.Password.Length > 0)
                connStr.Password = this.Password;
            connStr.PersistSecurityInfo = this.PersistSecurity;
            connStr.ConnectionTimeout = this.TimeOut;
            if (ConnectionType == Connection_Type.Service_Name)
                connStr.DataSource = GetDataSourceAsServiceName();
            else if (ConnectionType == Connection_Type.SID)
                connStr.DataSource = GetDataSourceAsSID();
            else if (ConnectionType == Connection_Type.TNS)
                connStr.DataSource = this.TNS;
            return connStr.ConnectionString;
        }


        /// <summary>
        /// Creates the data source string for a SID connenction
        /// </summary>
        /// <returns>the data source string</returns>
        private string GetDataSourceAsSID()
        {
            return String.Format("(DESCRIPTION = (ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={2})))" +
                                 "(CONNECT_DATA = (SERVER=DEDICATED)(SID={1})))", this.Server, this.SID, this.Port);
        }
        /// <summary>
        /// Creates the data source string for a service name connenction
        /// </summary>
        /// <returns>the data source string</returns>
        private string GetDataSourceAsServiceName()
        {
            return String.Format("(DESCRIPTION = (ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={2})))" +
                                 "(CONNECT_DATA = (SERVER=DEDICATED)(SERVICE_NAME={1})))", this.Server, this.Service_Name, this.Port);
        }
    }
}
