using DaSoft.Riviera.Modulador.Core.Model.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Bordeo.UI
{
    /// <summary>
    /// The bridge selection result
    /// </summary>
    public class BridgeSelectionResult
    {
        /// <summary>
        /// The selected code
        /// </summary>
        public string SelectedCode;
        /// <summary>
        /// The acabado puentes
        /// </summary>
        public RivieraAcabado AcabadoPuentes;
        /// <summary>
        /// The acabado pazo
        /// </summary>
        public RivieraAcabado AcabadoPazo;
    }
}
