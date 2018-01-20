using Nameless.Libraries.Yggdrasil.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Core.Runtime
{
    /// <summary>
    /// Defines a Riviera Application Exception
    /// </summary>
    public class RivieraException : NamelessException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RivieraException"/> class.
        /// </summary>
        /// <param name="msg">The exception message.</param>
        public RivieraException(string msg) : base(msg)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="RivieraException"/> class.
        /// </summary>
        /// <param name="msg">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public RivieraException(string msg, Exception innerException) : base(msg, innerException)
        {
        }
    }
}
