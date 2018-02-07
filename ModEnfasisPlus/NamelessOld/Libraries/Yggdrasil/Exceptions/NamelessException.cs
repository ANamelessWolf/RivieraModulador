using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.Yggdrasil.Exceptions
{
    /// <summary>
    /// NamelessException are the exceptions of nameless API
    /// Wutai is the home town of Yufi Kisaragi.
    /// </summary>
    public class NamelessException : System.Exception, INameless
    {
        /// <summary>
        /// Creates the Nameless data exception
        /// </summary>
        NamelessObject Nameless;
        #region Constructores
        /// <summary>
        /// Creates a new nameless exception basic Exception
        /// </summary>
        /// <param name="msg">Specify the text for the current exception</param>
        public NamelessException(string msg)
            : base(msg)
        {
            Nameless = new NamelessObject(this.GetType());
        }
        /// <summary>
        /// Creates a new nameless exception basic Exception
        /// </summary>
        /// <param name="msg">Specify the text for the current exception</param>
        /// <param name="innerException">Specify the Inner Exception for this exception</param>
        public NamelessException(string msg, System.Exception innerException)
            : base(msg, innerException)
        {
            Nameless = new NamelessObject(this.GetType());
        }
        #endregion
        #region Actions
        /// <summary>
        /// Gets the nameless object definition
        /// </summary>
        /// <returns>The nameless object</returns>
        public NamelessObject GetNameless()
        {
            return this.Nameless;
        }
        #endregion
    }
}
