using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.Modulador.Bordeo.Model.Enities;
using DaSoft.Riviera.Modulador.Core.Controller;
using DaSoft.Riviera.Modulador.Core.Model;
using Nameless.Libraries.HoukagoTeaTime.Tsumugi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DaSoft.Riviera.Modulador.Bordeo.Assets.Codes;
using static DaSoft.Riviera.Modulador.Bordeo.Assets.Constants;
using static DaSoft.Riviera.Modulador.Core.Assets.CONST;
using static DaSoft.Riviera.Modulador.Core.Controller.AutoCADUtils;
namespace DaSoft.Riviera.Modulador.Bordeo.Controller
{
    public class BordeoLoader : RivieraLoader
    {
        /// <summary>
        /// Gets or sets the bordeo codes.
        /// </summary>
        /// <value>
        /// The bordeo codes.
        /// </value>
        public override String[] LineCodes => new string[] { CODE_DPANEL_STACK, CODE_PANEL_STACK };
        /// <summary>
        /// Initializes a new instance of the <see cref="BordeoLoader"/> class.
        /// </summary>
        /// <param name="dManager">The dictionary manager.</param>
        public BordeoLoader(ExtensionDictionaryManager dManager)
            : base(dManager) { }
        /// <summary>
        /// Loads the specified code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="loadObject">The load object.</param>
        /// <returns></returns>
        public Boolean Load(String code, Transaction tr, Entity ent, DBDictionary dictionary, out RivieraObject loadObject)
        {
            loadObject = null;
            Point3d start, end;
            ObjectIdCollection ids;
            if (this.IsDesignLineCode(code))
            {
                this.GetLocation(tr, out start, out end);
                switch (code)
                {
                    case CODE_DPANEL_STACK:
                        BordeoLPanel[] doublePanels = this.GetDoublePanels(tr);
                        BordeoLPanelStack pdStack = new BordeoLPanelStack(start, end, doublePanels);
                        doublePanels[0].PanelGeometry = this.GetEntity<Polyline>(tr);
                        ids = this.LoadGeometry(tr);
                        pdStack.Ids = ids;
                        doublePanels[0].Ids = ids;
                        loadObject = pdStack;
                        break;
                    case CODE_PANEL_STACK:
                        BordeoPanel[] panels = this.GetPanels(tr);
                        BordeoPanelStack pStack = new BordeoPanelStack(start, end, panels);
                        pStack.PanelGeometry = this.GetEntity<Line>(tr);
                        ids = this.LoadGeometry(tr);
                        pStack.Ids = ids;
                        panels[0].Ids = ids;
                        loadObject = pStack;
                        break;
                }
                if (loadObject != null)
                {
                    this.LoadConnections(tr, loadObject, ArrowDirection.FRONT.GetArrowDirectionName(), ArrowDirection.BACK.GetArrowDirectionName());
                    this.Load(ref loadObject, tr);
                }
            }
            return loadObject != null;
        }

        private BordeoLPanel[] GetDoublePanels(Transaction tr)
        {
            throw new NotImplementedException();
        }

        private BordeoPanel[] GetPanels(Transaction tr)
        {
            throw new NotImplementedException();
        }
    }
}
