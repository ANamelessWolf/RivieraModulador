using DaSoft.Riviera.OldModulador.Model.Delta;
using System;
using System.Collections.Generic;

namespace DaSoft.Riviera.OldModulador.Controller
{
    public class MamparaEnfasisQuantificationError
    {
        /// <summary>
        /// La lista de los metodos que generarón los errores encontrados en la cuantificación
        /// </summary>
        public List<String> ErrorMethods;
        /// <summary>
        /// El mensaje de error asociado a una excecpción
        /// </summary>
        public Dictionary<String, String> ExceptionMessage;
        /// <summary>
        /// La mampara que contiene el error de cuantificación
        /// </summary>
        public Mampara Mampara;
        /// <summary>
        /// Initializes a new instance of the <see cref="MamparaEnfasisQuantificationError"/> class.
        /// </summary>
        public MamparaEnfasisQuantificationError(Mampara mam, String method, String excMsg)
        {
            this.Mampara = mam;
            this.ErrorMethods = new List<string>();
            this.ExceptionMessage = new Dictionary<string, string>();
            this.ExceptionMessage.Add(method, excMsg);
            this.ErrorMethods.Add(method);
        }
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Format("{0}, Errors: {1}", this.Mampara.Code, this.ErrorMethods.Count);
        }
    }
}
