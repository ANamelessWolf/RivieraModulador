using DaSoft.Riviera.Modulador.Core.Model;
using DaSoft.Riviera.Modulador.Core.Model.DB;
using Nameless.Libraries.DB.Mikasa;
using Nameless.Libraries.DB.Mikasa.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DaSoft.Riviera.Modulador.Core.Assets.CONST;
using static DaSoft.Riviera.Modulador.Bordeo.Model.BordeoDesignDatabase;
namespace DaSoft.Riviera.Modulador.Bordeo.Model.DB
{
    /// <summary>
    /// Defines a Bordeo Panel Row
    /// </summary>
    /// <seealso cref="DaSoft.Riviera.Modulador.Core.Model.DB.RivieraMeasureRow" />
    public class BordeoLPanelRow : RivieraMeasureRow
    {
        const string FIELD_ALTO_REAL = "ALTO_REAL_M";
        const string FIELD_ALTO = "ALTO";
        const string FIELD_FRENTE_1_REAL = "FRENTE_1_REAL_M";
        const string FIELD_FRENTE_1 = "FRENTE_1";
        const string FIELD_FRENTE_2_REAL = "FRENTE_2_REAL_M";
        const string FIELD_FRENTE_2 = "FRENTE_2";
        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <value>
        /// The name of the table.
        /// </value>
        public override string TableName => TABLENAME_PANEL_L;
        /// <summary>
        /// Initializes a new instance of the <see cref="BordeoLPanelRow"/> class.
        /// </summary>
        /// <param name="result">The selection result.</param>
        public BordeoLPanelRow(SelectionResult[] result) :
            base(result)
        { }
        /// <summary>
        /// Creats the meausure.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>
        /// Creates the measure
        /// </returns>
        protected override RivieraMeasure CreateMeausure(SelectionResult[] result)
        {
            RivieraSize frente1 = new RivieraSize()
            {
                Measure = KEY_START_FRONT,
                Nominal = result.ConvertValue<Double>(FIELD_FRENTE_1),
                Real = result.ConvertValue<Double>(FIELD_FRENTE_1_REAL)
            },
            frente2 = new RivieraSize()
            {
                Measure = KEY_END_FRONT,
                Nominal = result.ConvertValue<Double>(FIELD_FRENTE_2),
                Real = result.ConvertValue<Double>(FIELD_FRENTE_2_REAL)
            },
            alto = new RivieraSize()
            {
                Measure = KEY_HEIGHT,
                Nominal = result.ConvertValue<Double>(FIELD_ALTO),
                Real = result.ConvertValue<Double>(FIELD_ALTO_REAL)
            };
            return new LPanelMeasure(frente1, frente2, alto);
        }
    }
}