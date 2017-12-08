using DaSoft.Riviera.Modulador.Core.Model;
using Nameless.Libraries.DB.Mikasa;
using Nameless.Libraries.DB.Mikasa.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DaSoft.Riviera.Modulador.Core.Assets.CONST;
namespace DaSoft.Riviera.Modulador.Bordeo.Model.DB
{
    /// <summary>
    /// Defines a Bordeo Panel Row
    /// </summary>
    /// <seealso cref="Nameless.Libraries.DB.Mikasa.Model.DBMappingViewObject" />
    public class BordeoPanelRow : DBMappingViewObject
    {
        const string TABLENAME = "VW_BR_PANEL_L";
        const string FIELD_ALTO_REAL = "ALTO_REAL_M";
        const string FIELD_ALTO = "ALTO";
        const string FIELD_FRENTE_1_REAL = "FRENTE_1_REAL_M";
        const string FIELD_FRENTE_1 = "FRENTE_1";
        const string FIELD_FRENTE_2_REAL = "FRENTE_2_REAL_M";
        const string FIELD_FRENTE_2 = "FRENTE_2";
        const string FIELD_CODE = "CODIGO";
        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <value>
        /// The name of the table.
        /// </value>
        public override string TableName => TABLENAME;
        /// <summary>
        /// Gets the primary key.
        /// </summary>
        /// <value>
        /// The primary key.
        /// </value>
        public override string PrimaryKey => FIELD_CODE;
        /// <summary>
        /// The Bordeo panel measure
        /// </summary>
        public RivieraMeasure Measure;
        /// <summary>
        /// The Measure code
        /// </summary>
        public String Code;
        /// <summary>
        /// Parses the object.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <exception cref="NotImplementedException"></exception>
        protected override void ParseObject(SelectionResult[] result)
        {
            RivieraSize frente1 = new RivieraSize()
            {
                Measure = KEY_START_FRONT,
                Nominal = result.GetValue<Double>(FIELD_FRENTE_1),
                Real = result.GetValue<Double>(FIELD_FRENTE_1_REAL)
            },
            frente2 = new RivieraSize()
            {
                Measure = KEY_END_FRONT,
                Nominal = result.GetValue<Double>(FIELD_FRENTE_2),
                Real = result.GetValue<Double>(FIELD_FRENTE_2_REAL)
            },
            alto = new RivieraSize()
            {
                Measure = KEY_HEIGHT,
                Nominal = result.GetValue<Double>(FIELD_ALTO),
                Real = result.GetValue<Double>(FIELD_ALTO_REAL)
            };
            this.Measure = new LPanelMeasure(frente1, frente2, alto);
            this.
        }
    }
}
