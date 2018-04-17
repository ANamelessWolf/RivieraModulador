using DaSoft.Riviera.Modulador.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaSoft.Riviera.Modulador.Core.Model.DB;

namespace DaSoft.Riviera.Modulador.Bordeo.Model
{
    public class BridgeGroupCode : RivieraCode
    {
        private RivieraAcabado _AcabadoPuentes;
        private RivieraAcabado _AcabadoPazoLuz;
        public override RivieraAcabado SelectedAcabado => _AcabadoPuentes;
        public RivieraAcabado SelectedPazoLuzAcabado => _AcabadoPazoLuz;
        /// <summary>
        /// Initializes a new instance of the <see cref="BridgeGroupCode"/> class.
        /// </summary>
        /// <param name="groupCode">The group code.</param>
        /// <param name="acabadoPuente">The acabado puente.</param>
        /// <param name="acabadoPazo">The acabado pazo.</param>
        public BridgeGroupCode(String groupCode, RivieraAcabado acabadoPuente, RivieraAcabado acabadoPazo) : base()
        {
            this.Code = groupCode;
            this.Block = groupCode;
            this.Description = "Grupo de puentes";
            this._AcabadoPazoLuz = acabadoPazo;
            this._AcabadoPuentes = acabadoPuente;
            this.ElementType = RivieraElementType.Bridge_Group;
            this.Line = DesignLine.Bordeo;
            this.DoublePanel = false;
        }
    }
}
