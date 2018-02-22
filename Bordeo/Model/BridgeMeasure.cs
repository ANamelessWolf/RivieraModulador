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
    /// Defines the measure for a panel
    /// </summary>
    /// <seealso cref="DaSoft.Riviera.Modulador.Core.Model.RivieraMeasure" />
    public class BridgeMeasure : RivieraMeasure
    {
        /// <summary>
        /// The Panel "frente" size
        /// </summary>
        public RivieraSize Frente => this[KEY_FRONT];
        /// <summary>
        /// The Panel "alto" size
        /// </summary>
        public RivieraSize Alto => this[KEY_HEIGHT];
        /// <summary>
        /// The Panel "fondo" size
        /// </summary>
        public RivieraSize Fondo => this[KEY_DEPHT];
        /// <summary>
        /// Initializes a new instance of the <see cref="BridgeMeasure"/> class.
        /// </summary>
        /// <param name="frente">The bridge "frente" size.</param>
        /// <param name="alto">The bridge "alto" size.</param>
        /// <param name="fondo">The bridge "fondo" size.</param>
        public BridgeMeasure(RivieraSize frente, RivieraSize alto, RivieraSize fondo)
            : base(frente, alto,fondo)
        {

        }
    }
}