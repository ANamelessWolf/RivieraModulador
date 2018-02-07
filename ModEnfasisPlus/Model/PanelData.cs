using System;
using System.Collections.Generic;

namespace DaSoft.Riviera.OldModulador.Model
{
    public class PanelData
    {
        /// <summary>
        /// El código del dueño de los paneles
        /// </summary>
        public String HostCode;
        /// <summary>
        /// El código del panel seleccionado
        /// </summary>
        public String Code;
        /// <summary>
        /// El tipo de panel seleccionado
        /// </summary>
        public String Tipo;
        /// <summary>
        /// La altura del contenedor de paneles
        /// </summary>
        public double Host_Height;
        /// <summary>
        /// El frente del contenedor de paneles
        /// </summary>
        public double Host_Front;
        /// <summary>
        /// La lista de niveles disponibles para el panel seleccionado
        /// </summary>
        public List<String> Niveles;
        /// <summary>
        /// La lista de alturas disponibles ordenadas por nombre de nivel
        /// </summary>
        public Dictionary<String, double> Heights;
        /// <summary>
        /// Accede a la descripción del código actual
        /// </summary>
        public String Description;
        /// <summary>
        /// El valor de frente nominal del panel
        /// </summary>
        public String FrenteNominal;
        /// <summary>
        /// Verdadero si el tipo de panel puede ser insertado en dos mamparas
        /// </summary>
        public Boolean CanBeDouble;
        /// <summary>
        /// El valor del panel en cadena
        /// </summary>
        /// <returns>El código del panel</returns>
        public override string ToString()
        {
            return Code;
        }

    }
}
