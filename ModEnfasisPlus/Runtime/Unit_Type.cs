using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.OldModulador.Runtime
{
    /// <summary>
    /// Un enumerador que define el tipo de unidades que actualmente usa la aplicación
    /// </summary>
    public enum Unit_Type
    {
        /// <summary>
        /// Ningun tipo de unidad, adimensional
        /// </summary>
        None = -1,
        /// <summary>
        /// Metros líneales
        /// </summary>
        m = 1,
        /// <summary>
        /// Milimetros líneales
        /// </summary>
        mm = 2,
        /// <summary>
        /// Centimetros líneales
        /// </summary>
        cm = 3,
        /// <summary>
        /// Pulgadas líneales
        /// </summary>
        inches = 4,
        /// <summary>
        /// Pies líneales
        /// </summary>
        feet = 5,
        /// <summary>
        /// Yardas líneales
        /// </summary>
        yards = 6,
        /// <summary>
        /// Unides gráficas
        /// </summary>
        GraphicUnits = 7,
        /// <summary>
        /// Metros cuadrados
        /// </summary>
        m2 = 8,
        /// <summary>
        /// Pies cuadrados
        /// </summary>
        ft2 = 9,
        /// <summary>
        /// Unides gráficas cuadradas
        /// </summary>
        GraphicSquareUnits = 10,
    }
}
