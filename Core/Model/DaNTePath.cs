using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Core.Model
{
    /// <summary>
    /// Defines the DaNTe application directories or MDB files
    /// </summary>
    public enum DaNTePath
    {
        /// <summary>
        /// The dante app directory path
        /// </summary>
        DANTE_DIR = 0,
        /// <summary>
        /// The dante app Bases.MDB directory path
        /// </summary>
        DANTE_BASES = 1,
        /// <summary>
        /// Dantes associated directory path
        /// </summary>
        ASOC_DIR = 2,
        /// <summary>
        /// Dantes associated mdb path
        /// </summary>
        ASOC_MDB = 3,
        /// <summary>
        /// Dantes module directory path
        /// </summary>
        MOD_DIR = 4
    }
}
