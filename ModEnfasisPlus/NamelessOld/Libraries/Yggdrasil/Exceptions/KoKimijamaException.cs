using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.Yggdrasil.Exceptions
{
    public class KoKimijamaException : NamelessException
    {
        #region Constructores
        /// <summary>
        /// Creates a new KoKimijamaException (Threading, Client-Server) Exception
        /// </summary>
        /// <param name="msg">Specify the text for the current exception</param>
        public KoKimijamaException(string msg)
            : base(msg) { }
        /// <summary>
        /// Creates a new KoKimijamaException (Threading, Client-Server) Exception
        /// </summary>
        /// <param name="msg">Specify the text for the current exception</param>
        /// <param name="innerException">Specify the Inner Exception for this exception</param>
        public KoKimijamaException(string msg, System.Exception innerException)
            : base(msg, innerException) { }
        #endregion
    }
}
