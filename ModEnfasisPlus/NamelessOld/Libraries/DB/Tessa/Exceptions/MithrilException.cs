using NamelessOld.Libraries.DB.Mikasa.Exceptions;
using NamelessOld.Libraries.Yggdrasil.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.DB.Tessa.Exceptions
{
    /// <summary>
    /// Creates a new Mithril exception for anything that goes wrong in the Access
    /// </summary>
    public class MithrilException : TitanException
    {
        #region Constructores
        /// <summary>
        /// Creates a new MithrilException (Access Manager) Exception
        /// </summary>
        /// <param name="msg">Specify the text for the current exception</param>
        public MithrilException(string msg)
            : base(msg) { }
        /// <summary>
        /// Creates a new MithrilException (Access Manager) Exception
        /// </summary>
        /// <param name="msg">Specify the text for the current exception</param>
        /// <param name="innerException">Specify the Inner Exception for this exception</param>
        public MithrilException(string msg, OleDbException innerException)
            : base(msg, innerException) { }
        /// <summary>
        /// Creates a new MithrilException (Access Manager) Exception
        /// </summary>
        /// <param name="msg">Specify the text for the current exception</param>
        /// <param name="innerException">Specify the Inner Exception for this exception</param>
        public MithrilException(string msg, System.Exception innerException)
            : base(msg, innerException) { }
        #endregion
    }
}
