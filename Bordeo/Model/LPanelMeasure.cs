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
    /// Defines the measure for a L panel
    /// </summary>
    /// <seealso cref="DaSoft.Riviera.Modulador.Core.Model.RivieraMeasure" />
    public class LPanelMeasure : RivieraMeasure
    {
        /// <summary>
        /// The Panel "frente inicial" size
        /// </summary>
        public RivieraSize FrenteStart => this[KEY_START_FRONT];
        /// <summary>
        /// The Panel "frente final" size
        /// </summary>
        public RivieraSize FrenteEnd => this[KEY_END_FRONT];
        /// <summary>
        /// The Panel "alto" size
        /// </summary>
        public RivieraSize Alto => this[KEY_HEIGHT];
        /// <summary>
        /// Initializes a new instance of the <see cref="LPanelMeasure"/> class.
        /// </summary>
        /// <param name="frenteStart">The panel "frente inicial" size.</param>
        /// <param name="frenteEnd">The panel "frente final" size.</param>
        /// <param name="alto">The panel "alto" size.</param>
        public LPanelMeasure(RivieraSize frenteStart, RivieraSize frenteEnd, RivieraSize alto)
            : base(frenteStart, frenteEnd, alto)
        {

        }
        /// <summary>
        /// Inverts the front.
        /// </summary>
        /// <returns>A size with the inverted fronts</returns>
        public LPanelMeasure InvertFront()
        {
            RivieraSize s0 = new RivieraSize() { Measure = this.FrenteStart.Measure, Nominal = this.FrenteEnd.Nominal, Real = this.FrenteEnd.Real },
                        s1 = new RivieraSize() { Measure = this.FrenteEnd.Measure, Nominal = this.FrenteStart.Nominal, Real = this.FrenteStart.Real };
            return new LPanelMeasure(s0, s1, this.Alto);
        }
    }
}