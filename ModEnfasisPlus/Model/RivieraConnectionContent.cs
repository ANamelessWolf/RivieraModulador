using NamelessOld.Libraries.DB.Mikasa.Model;
using NamelessOld.Libraries.DB.Misa.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.OldModulador.Model
{
    public class RivieraConnectionContent : OracleConnectionContent
    {
        /// <summary>
        /// Creates a new Riviera Connection
        /// </summary>
        /// <param name="builder">The oracle connection builder</param>
        public RivieraConnectionContent(OracleConnectionBuilder builder) : 
            base(builder.Data)
        {

        }

        #region Default Connections
        /// <summary>
        /// Gets the remote Riviera TNS connection
        /// </summary>
        public static OracleConnectionContent Remote_Riviera_TNS
        {
            get
            {
                return new OracleConnectionContent(
                    ((int)ConnectionInterface.Oracle).ToString(),
                    "riviera",
                    "s1na1",
                    "193.3.5.30",
                    "1521",
                    ((int)Connection_Type.TNS).ToString(),
                    "TRAMITES_RIVIERA",
                    String.Empty,
                    String.Empty,
                    "10",
                    "true");
            }
        }
        /// <summary>
        /// Gets the remote Riviera service name connection
        /// </summary>
        public static OracleConnectionContent Remote_Riviera_Servicename
        {
            get
            {
                return new OracleConnectionContent(
                    ((int)ConnectionInterface.Oracle).ToString(),
                    "riviera",
                    "s1na1",
                    "193.3.5.30",
                    "1521",
                    ((int)Connection_Type.Service_Name).ToString(),
                    String.Empty,
                    "tramites.palmas.rivieramex.com",
                    String.Empty,
                    "10",
                    "true");
            }
        }
        /// <summary>
        /// Gets the remote Riviera TNS connection
        /// </summary>
        public static OracleConnectionContent Remote_Riviera_SID
        {
            get
            {
                return new OracleConnectionContent(
                    ((int)ConnectionInterface.Oracle).ToString(),
                    "riviera",
                    "s1na1",
                    "193.3.5.30",
                    "1521",
                    ((int)Connection_Type.SID).ToString(),
                    String.Empty,
                    String.Empty,
                    "TRAMITES",
                    "10",
                    "true");
            }
        }
        /// <summary>
        /// Gets the local Riviera connection
        /// </summary>
        public static OracleConnectionContent Local_Riviera_SID
        {
            get
            {
                return new OracleConnectionContent(
                    ((int)ConnectionInterface.Oracle).ToString(),
                    "riviera",
                    "r4cks",
                    "193.3.5.30",
                    "1521",
                    ((int)Connection_Type.SID).ToString(),
                    String.Empty,
                    String.Empty,
                    "ORCL",
                    "10",
                    "true");
            }
        }
        #endregion
    }
}
