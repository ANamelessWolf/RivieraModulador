using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.Yggdrasil.Exceptions
{
    /// <summary>
    /// DarkIllusionException are the exception found in Morrigan (Data Structure API). 
    /// Dark Illusion was the most common attack used by Morrigan Aensland, 
    /// </summary>
    public class DarkIllusionException : NamelessException
    {
        #region Constructores
        /// <summary>
        /// Creates a new DarkIllusionException (Nameless Data Structure API) Exception
        /// </summary>
        /// <param name="msg">Specify the text for the current exception</param>
        public DarkIllusionException(string msg)
            : base(msg)
        { }
        /// <summary>
        /// Creates a new DarkIllusionException (Nameless Data Structure API) Exception
        /// </summary>
        /// <param name="msg">Specify the text for the current exception</param>
        /// <param name="innerException">Specify the Inner Exception for this exception</param>
        public DarkIllusionException(string msg, System.Exception innerException)
            : base(msg, innerException)
        { }
        #endregion
    }

}
