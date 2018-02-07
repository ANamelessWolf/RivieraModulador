using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.Yggdrasil.Exceptions
{
    public class ErgoProxyException : NamelessException
    {
        #region Constructores
        /// <summary>
        /// Creates a new Ergo Proxy Exception ReL (Net Tools API) Exception
        /// </summary>
        /// <param name="msg">Specify the text for the current exception</param>
        public ErgoProxyException(string msg)
            : base(msg) { }
        /// <summary>
        /// Creates a new Ergo Proxy Exception ReL (Net Tools API) Exception
        /// </summary>
        /// <param name="msg">Specify the text for the current exception</param>
        /// <param name="innerException">Specify the Inner Exception for this exception</param>
        public ErgoProxyException(string msg, System.Exception innerException)
            : base(msg, innerException) { }
        #endregion
    }
}
