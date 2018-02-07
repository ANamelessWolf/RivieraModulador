using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.DB.Mikasa.Model
{
    /// <summary>
    /// Gets the Oledb Providers for Access
    /// </summary>
    public enum OledbProviders
    {
        /// <summary>
        /// Undefined Ole DB Provider
        /// </summary>
        UndefinedProvider = -1,
        /// <summary>
        /// Microsoft.Jet.OLEDB.4.0
        /// </summary>
        JetOleDb_4 = 1,
        /// <summary>
        /// Microsoft.ACE.OLEDB.4.0
        /// </summary>
        Microsoft_ACE_OleDb_4 = 2,
        /// <summary>
        /// Microsoft.ACE.OLEDB.12.0
        /// </summary>
        Microsoft_ACE_OleDb_12 = 3,
        /// <summary>
        /// Microsoft.ACE.OLEDB.14.0
        /// </summary>
        Microsoft_ACE_OleDb_14 = 4,
    }

}
