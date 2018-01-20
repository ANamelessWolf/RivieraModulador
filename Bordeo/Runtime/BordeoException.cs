using DaSoft.Riviera.Modulador.Core.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Bordeo.Runtime
{
    public class BordeoException : RivieraException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BordeoException"/> class.
        /// </summary>
        /// <param name="msg">The exception message.</param>
        public BordeoException(string msg) : base(msg)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BordeoException"/> class.
        /// </summary>
        /// <param name="msg">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public BordeoException(string msg, Exception innerException) : base(msg, innerException)
        {
        }
    }
}
