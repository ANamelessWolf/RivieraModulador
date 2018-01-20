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
    public class PanelMeasure : RivieraMeasure
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
        /// Initializes a new instance of the <see cref="PanelMeasure"/> class.
        /// </summary>
        /// <param name="frente">The panel "frente" size.</param>
        /// <param name="alto">The panel "alto" size.</param>
        public PanelMeasure(RivieraSize frente, RivieraSize alto)
            : base(frente, alto)
        {

        }
    }
}
