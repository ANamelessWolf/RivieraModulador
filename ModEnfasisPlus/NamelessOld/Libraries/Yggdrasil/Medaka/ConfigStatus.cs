using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.Yggdrasil.Medaka
{
    public enum ConfigStatus
    {
        /// <summary>
        /// Means the configuration files is empty
        /// </summary>
        Empty = 1,
        /// <summary>
        /// Means the configuration files is damage and needs to be rebuild.
        /// </summary>
        Damage = 2,
        /// <summary>
        /// Means the configuration file is defined correctly.
        /// </summary>
        Ok = 3
    }
}
