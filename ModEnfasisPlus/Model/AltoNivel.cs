using System;

namespace DaSoft.Riviera.OldModulador.Model
{
    /// <summary>
    /// Esta estructura guarda la relación entre el alto y nivel.
    /// El campo type, nos define a que elemento pertenece
    /// </summary>
    public struct AltoNivel
    {
        /// <summary>
        /// El tipo al que pertenece la relación
        /// </summary>
        public ElementType Type;
        /// <summary>
        /// El alto en valor nominal
        /// </summary>
        public Double Alto;
        /// <summary>
        /// El nivel asignadao a la altura
        /// </summary>
        public String Nivel;

        /// <summary>
        /// Realiza el parseo de una lectura de una fila seleccionada en la BD
        /// </summary>
        /// <param name="row">La fila seleccionada</param>
        public AltoNivel(ElementType type, string[] row)
        {
            Double alto;
            this.Alto = Double.TryParse(row[0], out alto) ? alto : 0;
            this.Nivel = row[1];
            this.Type = type;
        }
        /// <summary>
        /// Regresa la regla de nivel seleccionada
        /// </summary>
        public override string ToString()
        {
            return String.Format("{0}: {1}-{2}", this.Type, this.Alto, this.Nivel);
        }
    }
}
