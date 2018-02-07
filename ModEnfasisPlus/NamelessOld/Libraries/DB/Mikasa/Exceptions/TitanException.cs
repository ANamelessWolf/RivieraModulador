using NamelessOld.Libraries.Yggdrasil.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.DB.Mikasa.Exceptions
{
    public class TitanException : NamelessException
    {
        #region Constructores
        /// <summary>
        /// Creates a new TitanException (DB Manager) Exception
        /// </summary>
        /// <param name="msg">Specify the text for the current exception</param>
        public TitanException(string msg)
            : base(msg) { }
        /// <summary>
        /// Creates a new TitanException (DB Manager) Exception
        /// </summary>
        /// <param name="msg">Specify the text for the current exception</param>
        /// <param name="innerException">Specify the Inner Exception for this exception</param>
        public TitanException(string msg, System.Exception innerException)
            : base(msg, innerException) { }
        #endregion
    }
}
