using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.Yggdrasil.Exceptions
{
    /// <summary>
    /// TinaniaException are the exception found in Asuna (XML Manager).
    /// Titania was the avatar of Asuna during the world tree saga in Swor Art Online series
    /// </summary>
    public class TitaniaException : NamelessException
    {
        #region Constructores
        /// <summary>
        /// Creates a new TitaniaException (XML Manager) Exception
        /// </summary>
        /// <param name="msg">Specify the text for the current exception</param>
        public TitaniaException(string msg)
            : base(msg) { }
        /// <summary>
        /// Creates a new TitaniaException (XML Manager) Exception
        /// </summary>
        /// <param name="msg">Specify the text for the current exception</param>
        /// <param name="xmlException">Specify the xml Exception</param>
        public TitaniaException(string msg, System.Xml.XmlException xmlException)
            : base(msg, xmlException) { }
        /// <summary>
        /// Creates a new TitaniaException (XML Manager) Exception
        /// </summary>
        /// <param name="msg">Specify the text for the current exception</param>
        /// <param name="innerException">Specify the Inner Exception for this exception</param>
        public TitaniaException(string msg, System.Exception innerException)
            : base(msg, innerException) { }
        #endregion
    }
}
