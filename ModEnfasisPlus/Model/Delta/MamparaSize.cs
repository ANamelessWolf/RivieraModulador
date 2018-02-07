using DaSoft.Riviera.OldModulador.Runtime.Delta;
using System;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CONST;
using static DaSoft.Riviera.OldModulador.Assets.Strings;
namespace DaSoft.Riviera.OldModulador.Model.Delta
{
    public class MamparaSize : ElementSize
    {
        /// <summary>
        /// El tamaño por default de la mampara en unidades nominales
        /// </summary>
        public override double Default_Ancho_Nom
        {
            get
            {
                return MAM_ANCHO_NOM;
            }
        }
        /// <summary>
        /// El tamaño por default de la mampara en milimetros
        /// </summary>
        public override double Default_Ancho_mm
        {
            get
            {
                return MAM_ANCHO_MM;
            }
        }
        /// <summary>
        /// Crea una mampara desde la lista obtenida de la BD
        /// Las columnas estan en un arreglo de String[]
        /// Orden
        /// Código, frente_nominal, frente_real, alto_nominal, alto_real
        /// </summary>
        public MamparaSize(String[] row) : base(row) { }
        /// <summary>
        /// Crea una mampara vacia
        /// </summary>
        public MamparaSize() : base() { }
        /// <summary>
        /// Realiza el parseo de la mampra
        /// </summary>
        /// <param name="row">Las columnas recibidas por el query</param>
        public override void ParseRow(string[] row)
        {
            if (row.Length >= 5)
            {
                Double f, a;
                this.Code = row[0];
                this.Real = new RivieraSize
                {
                    Frente = Double.TryParse(row[2], out f) ? f : Double.NaN,
                    Alto = Double.TryParse(row[4], out a) ? a : Double.NaN,
                    Ancho = Default_Ancho_mm,
                    Code = this.Code,
                };
                this.Nominal = new RivieraSize
                {
                    Frente = Double.TryParse(row[1], out f) ? f : Double.NaN,
                    Alto = Double.TryParse(row[3], out a) ? a : Double.NaN,
                    Ancho = Default_Ancho_Nom,
                    Code = this.Code
                };
            }
            else
                throw new DeltaException(BAD_ROW_MAMPARA);
        }
    }
}
