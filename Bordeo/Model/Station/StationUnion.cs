using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Bordeo.Model.Station
{
    public enum StationUnion
    {
        /// <summary>
        /// La estación no reconece la unión
        /// </summary>
        None = 0,
        /// <summary>
        /// La estación es de tipo I
        /// </summary>
        I = 1,
        /// <summary>
        /// La estación es de tipo L a 90°
        /// </summary>
        L90 = 2,
        /// <summary>
        /// La estación es de tipo L a 135
        /// </summary>
        L135 = 3
    }
}
