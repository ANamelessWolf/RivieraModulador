using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.OldModulador.Runtime.Delta
{

    /// <summary>
    /// Excepciones generadas por componentes de la línea delta
    /// </summary>
    public class DeltaException : System.Exception
    {
        #region Constructores
        /// <summary>
        /// Crea una nueva excepción con un mensaje de error
        /// </summary>
        /// <param name="msg">El texto de la excepción generada</param>
        public DeltaException(string msg)
            : base(msg)
        {

        }
        /// <summary>
        /// Crea una nueva excepción con un mensaje de error, basada en un excepción generada
        /// </summary>
        /// <param name="msg">El texto de la excepción generada</param>
        /// <param name="innerException">La excepción interna</param>
        public DeltaException(string msg, System.Exception innerException)
            : base(msg, innerException)
        {
        }
        #endregion
    }
}
