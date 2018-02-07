using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.Yggdrasil.Exceptions
{
    /// <summary>
    /// GodModeException are the exception found in Medaka (Nameless Configuration File Manager).
    /// Medaka God Mode is not in controlled, her physical strength is greatly increase but his mind is damage in this state.
    /// </summary>
    public class GodModeException : NamelessException
    {
        #region Constructores
        /// <summary>
        /// Creates a new GodModeException (Configuration Manager) Exception
        /// </summary>
        /// <param name="msg">Specify the text for the current exception</param>
        public GodModeException(string msg)
            : base(msg) { }
        /// <summary>
        /// Creates a new GodModeException (Configuration Manager) Exception
        /// </summary>
        /// <param name="msg">Specify the text for the current exception</param>
        /// <param name="xmlException">Specify the xml Exception</param>
        public GodModeException(string msg, System.Xml.XmlException xmlException)
            : base(msg, xmlException) { }
        /// <summary>
        /// Creates a new GodModeException (Configuration Manager) Exception
        /// </summary>
        /// <param name="msg">Specify the text for the current exception</param>
        /// <param name="innerException">Specify the Inner Exception for this exception</param>
        public GodModeException(string msg, System.Exception innerException)
            : base(msg, innerException) { }
        #endregion
    }
}
