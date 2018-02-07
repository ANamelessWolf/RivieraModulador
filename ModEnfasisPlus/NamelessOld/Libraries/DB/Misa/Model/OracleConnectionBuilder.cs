using NamelessOld.Libraries.DB.Mikasa.Model;
using System;

namespace NamelessOld.Libraries.DB.Misa.Model
{
    public class OracleConnectionBuilder : DBConnectionBuilder
    {
        /// <summary>
        /// Oracle connection type
        /// SID, TNS, Servicename
        /// </summary>
        public Connection_Type ConnectionType;
        /// <summary>
        /// The name of the TNS connection
        /// </summary>
        public String TNS;
        /// <summary>
        /// The service name connection
        /// </summary>
        public String Servicename;
        /// <summary>
        /// The SID connection
        /// </summary>
        public String SID;
        /// <summary>
        /// Gets the data into array, used to create an Oracle Connection content
        /// </summary>
        public override String[] Data
        {
            get
            {
                return new String[]{
                        ((int)ConnectionInterface.Oracle).ToString(),
                        this.Username != null ? this.Username : String.Empty,
                        this.Password != null ? this.Password : String.Empty,
                        this.Host != null ? this.Host : String.Empty,
                        this.Port.ToString(),
                        ((int)ConnectionType).ToString(),
                        this.TNS != null ? this.TNS : String.Empty,
                        this.Servicename != null ? this.Servicename : String.Empty,
                        this.SID != null ? this.SID : String.Empty,
                        this.TimeOut.ToString(),
                        PersistSecurity.ToString()
                };
            }
        }
    }
}
