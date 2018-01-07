using DaSoft.Riviera.Modulador.Bordeo.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DaSoft.Riviera.Modulador.Bordeo.Assets.Strings;
namespace DaSoft.Riviera.Modulador.Bordeo.Controller
{
    /// <summary>
    /// Defines the bordeo class utils
    /// Size Utility
    /// </summary>
    public static partial class BordeoUtils
    {
        /// <summary>
        /// Gets the size of the linear drawing.
        /// </summary>
        /// <param name="nominal">The nominal value.</param>
        /// <returns>The linear double value</returns>
        public static Double GetLinearDrawingSize(Double nominal)
        {
            double val;
            if (nominal == 18d)
                val = 0.457d;
            else if (nominal == 24d)
                val = 0.61d;
            else if (nominal == 30d)
                val = 0.762d;
            else if (nominal == 36d)
                val = 0.915d;
            else if (nominal == 42d)
                val = 1.067d;
            else if (nominal == 48d)
                val = 1.22d;
            else
                throw new BordeoException(String.Format(ERR_UNKNOWN_DRAWING_SIZE, nominal));
            return val;
        }
    }
}
