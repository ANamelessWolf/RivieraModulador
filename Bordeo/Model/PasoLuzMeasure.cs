using DaSoft.Riviera.Modulador.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DaSoft.Riviera.Modulador.Core.Assets.CONST;
namespace DaSoft.Riviera.Modulador.Bordeo.Model
{
    /// <summary>
    /// Defines the measure for a pazo de Luz
    /// </summary>
    /// <seealso cref="DaSoft.Riviera.Modulador.Core.Model.RivieraMeasure" />
    public class PasoLuzMeasure : RivieraMeasure
    {
        /// <summary>
        /// The Panel "frente" size
        /// </summary>
        public RivieraSize Frente => this[KEY_FRONT];
        /// <summary>
        /// The Panel "fondo" size
        /// </summary>
        public RivieraSize Fondo => this[KEY_DEPHT];
        /// <summary>
        /// Initializes a new instance of the <see cref="PasoLuzMeasure"/> class.
        /// </summary>
        /// <param name="frente">The panel "frente" size.</param>
        /// <param name="fondo">The panel "fondo" size.</param>
        public PasoLuzMeasure(RivieraSize frente, RivieraSize fondo)
            : base(frente, fondo)
        {

        }
    }
}