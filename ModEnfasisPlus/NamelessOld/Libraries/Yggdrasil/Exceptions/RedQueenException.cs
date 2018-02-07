using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.Yggdrasil.Exceptions
{

    /// <summary>
    /// RedQueenException are the exception found in Asuna (Cryptography).
    /// The red queen or the queen of hearts is the evilness in wonderland, so if something goes wrong the Queen must be responsible.
    /// </summary>
    public class RedQueenException : NamelessException
    {
        #region Constructores
        /// <summary>
        /// Creates a new RedQueenException (Cryptography) Exception
        /// </summary>
        /// <param name="msg">Specify the text for the current exception</param>
        public RedQueenException(string msg)
            : base(msg) { }
        /// <summary>
        /// Creates a new RedQueenException (Cryptography) Exception
        /// </summary>
        /// <param name="msg">Specify the text for the current exception</param>
        /// <param name="cryptoExc">Specify the cryptographic Exception</param>
        public RedQueenException(string msg, CryptographicException cryptoExc)
            : base(msg, cryptoExc) { }
        /// <summary>
        /// Creates a new RedQueenException (Cryptography) Exception
        /// </summary>
        /// <param name="msg">Specify the text for the current exception</param>
        /// <param name="innerException">Specify the Inner Exception for this exception</param>
        public RedQueenException(string msg, System.Exception innerException)
            : base(msg, innerException) { }
        #endregion
    }
}
