﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Core.Model.DB
{
    /// <summary>
    /// Defines a Riviera Acabado
    /// </summary>
    public struct RivieraAcabado
    {
        /// <summary>
        /// The Riviera code associated to this "acabado"
        /// </summary>
        public RivieraCode RivCode;
        /// <summary>
        /// The Riviera code for the "acabado"
        /// </summary>
        public String Acabado;
        /// <summary>
        /// The "acabado" description
        /// </summary>
        public String Description;
        /// <summary>
        /// Imprime el código al que tiene asociado los acabados
        /// </summary>
        /// <returns>la colección de acabados.</returns>
        public override string ToString()
        {
            return String.Format("{0}: {1}", this.RivCode, this.Acabado);
        }
    }
}
