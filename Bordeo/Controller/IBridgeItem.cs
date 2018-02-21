using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Bordeo.Controller
{
    interface IBridgeItem
    {
        /// <summary>
        /// Sets the code.
        /// </summary>
        /// <param name="code">The code.</param>
        void SetCode(String code);
        /// <summary>
        /// Sets the acabado.
        /// </summary>
        /// <param name="acabdo">The acabdo.</param>
        void SetAcabado(string acabdo);
        /// <summary>
        /// Updates the size.
        /// </summary>
        /// <param name="sizeName">Name of the size.</param>
        /// <param name="sizeValue">The size value.</param>
        void UpdateSize(String sizeName, double sizeValue);
    }
}
