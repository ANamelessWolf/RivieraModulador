using DaSoft.Riviera.OldModulador.Runtime.Delta;
using System;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CONST;
using static DaSoft.Riviera.OldModulador.Assets.Strings;
namespace DaSoft.Riviera.OldModulador.Model
{
    public class PanelSize : ElementSize
    {
        /// <summary>
        /// El código del elemento con formato válido
        /// </summary>
        public override String CodeFormatted
        {
            get
            {
                return String.Format("{0}{1}{2}", this.Code,
                    FrenteNominal != String.Empty ? this.FrenteNominal : ((int)this.Nominal.Frente).ToString(),
                    (int)this.Nominal.Alto);
            }
        }
        /// <summary>
        /// El tamaño por default del panel en unidades nominales
        /// </summary>
        public override double Default_Ancho_Nom
        {
            get
            {
                return PAN_ANCHO_NOM;
            }
        }
        /// <summary>
        /// El tamaño por default del panel en milimetros
        /// </summary>
        public override double Default_Ancho_mm
        {
            get
            {
                return PAN_ANCHO_MM;
            }
        }
        /// <summary>
        /// Crea un panel desde la lista obtenida de la BD
        /// Las columnas estan en un arreglo de String[]
        /// Orden
        /// Código, frente_nominal, frente_real, alto_nominal, alto_real
        /// </summary>
        public PanelSize(String[] row) : base(row) { }
        /// <summary>
        /// Crea una mampara vacia
        /// </summary>
        public PanelSize() : base() { }
        /// <summary>
        /// Los paneles llegan a tener frentes nominales de 4 digitos.
        /// </summary>
        public String FrenteNominal;

        /// <summary>
        /// Realiza el parseo de la mampra
        /// </summary>
        /// <param name="row">Las columnas recibidas por el query</param>
        public override void ParseRow(string[] row)
        {
            if (row.Length >= 5)
            {
                Double f, f1, f2, a;
                this.Code = row[0];
                //Valor real
                this.Real = new RivieraSize
                {
                    Frente = Double.TryParse(row[2], out f) ? f : Double.NaN,
                    Alto = Double.TryParse(row[4], out a) ? a : Double.NaN,
                    Ancho = Default_Ancho_mm,
                    Code = this.Code,
                };
                //Realizamos el cálculo del valor nominal
                if (row[1].Length > 2)
                {
                    String f1Str = row[1].Substring(0, 2), f2Str = row[1].Substring(2, 2);
                    f = Double.TryParse(f1Str, out f1) && Double.TryParse(f2Str, out f2) ? f1 + f2 : Double.NaN;
                    FrenteNominal = row[1];
                }
                else
                {
                    f = Double.TryParse(row[1], out f) ? f : Double.NaN;
                    FrenteNominal = String.Empty;
                }
                this.Nominal = new RivieraSize
                {
                    Frente = f,
                    Alto = Double.TryParse(row[3], out a) ? a : Double.NaN,
                    Ancho = Default_Ancho_Nom,
                    Code = this.Code
                };
            }
            else
                throw new DeltaException(BAD_ROW_PANEL);
        }
    }
}