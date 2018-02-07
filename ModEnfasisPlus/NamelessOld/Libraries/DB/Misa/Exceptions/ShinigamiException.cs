using NamelessOld.Libraries.DB.Mikasa.Exceptions;
using NamelessOld.Libraries.Yggdrasil.Exceptions;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.DB.Misa.Exceptions
{

    public class ShinigamiException : TitanException
    {
        #region Constructores
        /// <summary>
        /// Creates a new ShinigamiException (Oracle Manager) Exception
        /// </summary>
        /// <param name="msg">Specify the text for the current exception</param>
        public ShinigamiException(string msg)
            : base(msg) { }
        /// <summary>
        /// Creates a new ShinigamiException (Oracle Manager) Exception
        /// </summary>
        /// <param name="msg">Specify the text for the current exception</param>
        /// <param name="innerException">Specify the Inner Exception for this exception</param>
        public ShinigamiException(string msg, OracleException innerException)
            : base(msg, innerException) { }
        /// <summary>
        /// Creates a new ShinigamiException (Oracle Manager) Exception
        /// </summary>
        /// <param name="msg">Specify the text for the current exception</param>
        /// <param name="innerException">Specify the Inner Exception for this exception</param>
        public ShinigamiException(string msg, System.Exception innerException)
            : base(msg, innerException) { }
        #endregion
    }
}
