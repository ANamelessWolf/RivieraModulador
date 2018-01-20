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
    public class BordeoPuenteRow : RivieraMeasureRow
    {
        const string FIELD_ALTO_REAL = "ALTO_REAL_M";
        const string FIELD_ALTO = "ALTO_NOMINAL";
        const string FIELD_FONDO_REAL = "FONDO_REAL_M";
        const string FIELD_FONDO = "FONDO_NOMINAL";
        const string FIELD_FRENTE_REAL = "FRENTE_REAL_M";
        const string FIELD_FRENTE = "FRENTE";
        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <value>
        /// The name of the table.
        /// </value>
        public override string TableName => TABLENAME_PUENTE;
        /// <summary>
        /// Initializes a new instance of the <see cref="BordeoPuenteRow"/> class.
        /// </summary>
        /// <param name="result">The selection result.</param>
        public BordeoPuenteRow(SelectionResult[] result) :
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
            RivieraSize frente = new RivieraSize()
            {
                Measure = KEY_FRONT,
                Nominal = result.ConvertValue<Double>(FIELD_FRENTE),
                Real = result.ConvertValue<Double>(FIELD_FRENTE_REAL)
            },
            fondo = new RivieraSize()
            {
                Measure = KEY_HEIGHT,
                Nominal = result.ConvertValue<Double>(FIELD_FONDO),
                Real = result.ConvertValue<Double>(FIELD_FONDO_REAL)
            },
            alto = new RivieraSize()
            {
                Measure = KEY_HEIGHT,
                Nominal = result.ConvertValue<Double>(FIELD_ALTO),
                Real = result.ConvertValue<Double>(FIELD_ALTO_REAL)
            };
            return new PuenteMeasure(frente, fondo, alto);
        }
    }
}