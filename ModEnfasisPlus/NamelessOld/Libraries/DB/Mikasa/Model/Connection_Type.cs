using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.DB.Mikasa.Model
{
    /// <summary>
    /// Define the type of connection to connect to Oracle
    /// </summary>
    public enum Connection_Type
    {
        /// <summary>
        /// None type selected
        /// </summary>
        None = -1,
        /// <summary>
        /// The connection type use a SID(site identifier)
        /// </summary>
        SID = 0,
        /// <summary>
        /// The connection type use an alias to an INSTANCE (or many instances)
        /// </summary>
        Service_Name = 1,
        /// <summary>
        /// The TNSNames.ora file contains the specific information required to connect to the Oracle instance. 
        /// By default, the TNSNames.ora
        /// </summary>
        TNS = 2
    }
}
