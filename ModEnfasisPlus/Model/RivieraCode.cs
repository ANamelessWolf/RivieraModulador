using System;

namespace DaSoft.Riviera.OldModulador.Model
{
    /// <summary>
    /// Define una estructura que define un código con una 
    /// descripción y un nombre de bloque asociado.
    /// Una de las clases que lo utilizan son los paneles
    /// </summary>
    public struct RivieraCode
    {
        /// <summary>
        /// El código del elemento
        /// </summary>
        public String Code;
        /// <summary>
        /// La descripción del código
        /// </summary>
        public String Description;
        /// <summary>
        /// El nombre del bloque
        /// </summary>
        public String Bloque;
        /// <summary>
        /// El tipo de panel o biombo
        /// </summary>
        public String Tipo;
        /// <summary>
        /// The can be double
        /// </summary>
        public Boolean CanBeDouble;
        /// <summary>
        /// Regresa el código seleccionado
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Code;
        }
    }
}
