using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.Modulador.Bordeo.Model;
using DaSoft.Riviera.Modulador.Bordeo.Model.Enities;
using DaSoft.Riviera.Modulador.Core.Controller;
using DaSoft.Riviera.Modulador.Core.Model;
using DaSoft.Riviera.Modulador.Core.Runtime;
using Nameless.Libraries.HoukagoTeaTime.Tsumugi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
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
        public Boolean Load(String code, Transaction tr, out RivieraObject loadObject)
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
                        BordeoLPanel[] doublePanels = GetDoublePanels(this.DManager, start, end, tr);
                        BordeoLPanelStack pdStack = new BordeoLPanelStack(start, end, doublePanels);
                        doublePanels[0].PanelGeometry = this.GetEntity<Polyline>(tr);
                        ids = this.LoadGeometry(tr);
                        pdStack.Ids = ids;
                        doublePanels[0].Ids = ids;
                        loadObject = pdStack;
                        break;
                    case CODE_PANEL_STACK:
                        BordeoPanel[] panels = GetPanels(this.DManager, start, end, tr);
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
        /// <summary>
        /// Gets the panels.
        /// </summary>
        /// <param name="start">The start panel.</param>
        /// <param name="end">The end panel.</param>
        /// <param name="tr">The active transaction.</param>
        /// <returns>The stack panels</returns>
        public static BordeoLPanel[] GetDoublePanels(ExtensionDictionaryManager dMan, Point3d start, Point3d end, Transaction tr)
        {
            String[] contentData = dMan.GetXRecord(KEY_CONTENT, tr).GetDataAsString(tr);
            var db = BordeoUtils.GetDatabase();
            BordeoLPanel[] panels = new BordeoLPanel[contentData.Length];
            string code;
            double frenteStartVal, frenteEndVal, altoVal, elevVal;
            SweepDirection dir;
            int index;
            String[] content;
            List<RivieraMeasure> sizes;
            LPanelMeasure measure = null;
            for (int i = 0; i < contentData.Length; i++)
            {
                content = contentData[i].Split('@');
                code = content[0];
                //Solo se carga un tamaño por stack
                if (measure == null)
                {
                    frenteStartVal = double.Parse(content[1]);
                    frenteEndVal = double.Parse(content[2]);
                    altoVal = double.Parse(content[3]);
                    sizes = db.Sizes[code].Sizes;
                    measure = sizes.Where(x => x is LPanelMeasure).
                        Select(y => y as LPanelMeasure).
                        FirstOrDefault(x => x.FrenteStart.Nominal == frenteStartVal && x.FrenteEnd.Nominal == frenteEndVal && x.Alto.Nominal == altoVal);
                }
                index = int.Parse(content[4]);
                elevVal = double.Parse(content[5]);
                dir = (SweepDirection)int.Parse(content[6]);
                if (code == CODE_PANEL_90)
                    panels[i] = new BordeoL90Panel(dir, start, end, measure);
                else
                    panels[i] = new BordeoL135Panel(dir, start, end, measure);
                panels[i].SetAcabado(index);
                panels[i].Elevation = elevVal;
            }
            return panels;
        }
        /// <summary>
        /// Gets the panels.
        /// </summary>
        /// <param name="start">The start point.</param>
        /// <param name="end">The end point.</param>
        /// <param name="tr">The active transaction.</param>
        /// <returns>The stack panels</returns>
        public static BordeoPanel[] GetPanels(ExtensionDictionaryManager dMan, Point3d start, Point3d end, Transaction tr)
        {
            String[] contentData = dMan.GetXRecord(KEY_CONTENT, tr).GetDataAsString(tr);
            var db = BordeoUtils.GetDatabase();
            BordeoPanel[] panels = new BordeoPanel[contentData.Length];
            string code;
            double frenteVal, altoVal, elevVal;
            int index;
            String[] content;
            List<RivieraMeasure> sizes;
            PanelMeasure measure = null;
            for (int i = 0; i < contentData.Length; i++)
            {
                content = contentData[i].Split('@');
                //Solo se carga un tamaño por stack
                if (measure == null)
                {
                    code = content[0];
                    frenteVal = double.Parse(content[1]);
                    altoVal = double.Parse(content[2]);
                    sizes = db.Sizes[code].Sizes;
                    measure = sizes.Where(x => x is PanelMeasure).
                        Select(y => y as PanelMeasure).
                        FirstOrDefault(x => x.Frente.Nominal == frenteVal && x.Alto.Nominal == altoVal);
                }
                index = int.Parse(content[3]);
                elevVal = double.Parse(content[4]);
                panels[i] = new BordeoPanel(start, end, measure);
                panels[i].SetAcabado(index);
                panels[i].Elevation = elevVal;
            }
            return panels;
        }
    }
}
