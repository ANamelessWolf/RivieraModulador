using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.DB.Mikasa.Model
{
    public abstract class DB_Query : NamelessObject
    {
        /// <summary>
        /// Format value to use in the current connection
        /// </summary>
        /// <param name="value">The value to be formated</param>
        /// <returns>The formated value</returns>
        public abstract string FormatValue(string value);
    }
}
