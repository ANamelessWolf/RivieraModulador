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
    /// Defines the measure for a puente
    /// </summary>
    /// <seealso cref="DaSoft.Riviera.Modulador.Core.Model.RivieraMeasure" />
    public class PuenteMeasure : RivieraMeasure
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
        /// The Panel "alto" size
        /// </summary>
        public RivieraSize Alto => this[KEY_HEIGHT];
        /// <summary>
        /// Initializes a new instance of the <see cref="PuenteMeasure"/> class.
        /// </summary>
        /// <param name="frente">The panel "frente" size.</param>
        /// <param name="fondo">The panel "fondo" size.</param>
        /// <param name="fondo">The panel "alto" size.</param>
        public PuenteMeasure(RivieraSize frente, RivieraSize fondo, RivieraSize alto)
            : base(frente, fondo, alto)
        {

        }
    }
}