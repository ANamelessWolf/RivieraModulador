using System;

namespace DaSoft.Riviera.OldModulador.Model
{
    public class QuantifiableObject
    {
        /// <summary>
        /// El código del objeto
        /// </summary>
        public String Code;
        /// <summary>
        /// La zona del objeto seleccionado
        /// </summary>
        public String Zone;
        /// <summary>
        /// El total de objetos a cuantificar
        /// </summary>
        public int Count;
        /// <summary>
        /// El handle del elemento seleccionado
        /// </summary>
        public long Handle;

        /// <summary>
        /// La visibilidad del objeto a cuantificar
        /// </summary>
        public Boolean Visibility;
        /// <summary>
        /// El código del elemento
        /// </summary>
        public override string ToString()
        {
            return String.Format("Code({0}), Count: {1}, Visibilidad: {2}", this.Code, this.Count, Visibility ? "S" : "N");
        }
    }
}
