using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Bordeo.Model
{
    /// <summary>
    /// Defines the selected Bordeo panel height
    /// </summary>
    public enum BordeoPanelHeight
    {
        None = -1,
        /// <summary>
        /// Two Panels one normal size and one mini size
        /// </summary>
        NormalMini = 0,
        /// <summary>
        /// Two Panels two normal panels
        /// </summary>
        TwoNormals = 1,
        /// <summary>
        /// Three Panels one normal and two mini panels
        /// </summary>
        NormalTwoMinis = 2,
        /// <summary>
        /// Three Panels Two normal and one mini panel
        /// </summary>
        TwoNormalOneMini = 3,
        /// <summary>
        /// Three Panels Two normal and one mini panel 
        /// on the middle
        /// </summary>
        NormalMiniNormal = 4,
        /// <summary>
        /// Four Panels One normal and three mini panels
        /// </summary>
        NormalThreeMini = 5,
        /// <summary>
        /// Three Panels Three normal panels
        /// </summary>
        ThreeNormals = 6,
    }
}
