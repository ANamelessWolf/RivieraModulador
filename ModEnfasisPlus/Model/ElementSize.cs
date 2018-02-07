using System;

namespace DaSoft.Riviera.OldModulador.Model
{
    public abstract class ElementSize
    {
        /// <summary>
        /// El valor por defecto del ancho nominal, pulgadas
        /// </summary>
        public abstract Double Default_Ancho_Nom { get; }
        /// <summary>
        /// El valor por defecto del ancho en milimetros
        /// </summary>
        public abstract Double Default_Ancho_mm { get; }
        /// <summary>
        /// El código del elemento
        /// </summary>
        public String Code;
        /// <summary>
        /// El código del elemento con formato válido
        /// </summary>
        public virtual String CodeFormatted
        {
            get
            {
                return String.Format("{0}{1}{2}", this.Code, (int)this.Nominal.Frente, (int)this.Nominal.Alto);
            }
        }
        /// <summary>
        /// El frente del elemento en valor nominal
        /// </summary>
        public String Frente
        {
            get
            {
                return Nominal.Frente.ToString();
            }
        }
        /// <summary>
        /// El alto del elemento en valor nominal
        /// </summary>
        public String Alto
        {
            get
            {
                return Nominal.Alto.ToString();
            }
        }
        /// <summary>
        /// El valor nominal
        /// </summary>
        public RivieraSize Nominal;
        /// <summary>
        /// El valor real
        /// </summary>
        public RivieraSize Real;
        /// <summary>
        /// Crea una mampara desde la lista obtenida de la BD
        /// Las columnas estan en un arreglo de String[]
        /// Orden
        /// Código, frente_nominal, frente_real, alto_nominal, alto_real
        /// </summary>
        public ElementSize(String[] row)
        {
            this.ParseRow(row);
        }
        /// <summary>
        /// Crea una mampara vacia
        /// </summary>
        public ElementSize()
        {
            this.Code = String.Empty;
            this.Nominal = new RivieraSize() { Frente = 0, Alto = 0, Ancho = Default_Ancho_Nom };
            this.Real = new RivieraSize() { Frente = 0, Alto = 0, Ancho = Default_Ancho_mm };
        }
        /// <summary>
        /// Realiza el parseo del objeto
        /// </summary>
        /// <param name="row">Las columnas obtenidas del query de selección</param>
        public abstract void ParseRow(string[] row);
        /// <summary>
        /// Imprime una descripción de la mampara
        /// </summary>
        /// <returns>La mampara como string</returns>
        public override string ToString()
        {
            return String.Format("{0}: {1}", this.Code, this.Nominal);
        }
    }
}
