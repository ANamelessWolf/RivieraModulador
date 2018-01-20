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
    public class BordeoIPanelRow : RivieraMeasureRow
    {
        const string FIELD_ALTO_REAL = "ALTO_REAL_M";
        const string FIELD_ALTO = "ALTO_NOMINAL";
        const string FIELD_FRENTE_REAL = "FRENTE_REAL_M";
        const string FIELD_FRENTE = "FRENTE_NOMINAL";

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <value>
        /// The name of the table.
        /// </value>
        public override string TableName => TABLENAME_PANEL_RECTO;
        /// <summary>
        /// Initializes a new instance of the <see cref="BordeoIPanelRow"/> class.
        /// </summary>
        /// <param name="result">The selection result.</param>
        public BordeoIPanelRow(SelectionResult[] result) :
            base(result)
        { }
        /// <summary>
        /// Creates the meausure.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        protected override RivieraMeasure CreateMeausure(SelectionResult[] result)
        {
            RivieraSize frente = new RivieraSize()
            {
                Measure = KEY_FRONT,
                Nominal = result.ConvertValue<Double>(FIELD_FRENTE),
                Real = result.ConvertValue<Double>(FIELD_FRENTE_REAL)
            },
            alto = new RivieraSize()
            {
                Measure = KEY_HEIGHT,
                Nominal = result.ConvertValue<Double>(FIELD_ALTO),
                Real = result.ConvertValue<Double>(FIELD_ALTO_REAL)
            };
            return new PanelMeasure(frente, alto);
        }
    }
}
