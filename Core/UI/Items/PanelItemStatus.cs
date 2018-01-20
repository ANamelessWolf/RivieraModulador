using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Core.UI.Items
{
    public enum PanelItemStatus
    {
        /// <summary>
        /// Initial panel status
        /// </summary>
        Initial = -1,
        /// <summary>
        /// The selected status
        /// </summary>
        Selected = 0,
        /// <summary>
        /// The panel is in ok status
        /// </summary>
        Ok = 1,
        /// <summary>
        /// The panel is in an error
        /// </summary>
        Error = 2,
    }
}
