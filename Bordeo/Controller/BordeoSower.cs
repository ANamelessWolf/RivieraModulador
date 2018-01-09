using DaSoft.Riviera.Modulador.Bordeo.Model;
using DaSoft.Riviera.Modulador.Core.Model;
using Nameless.Libraries.HoukagoTeaTime.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Nameless.Libraries.HoukagoTeaTime.Yui;
using DaSoft.Riviera.Modulador.Bordeo.UI;
using System.Windows.Controls;
using DaSoft.Riviera.Modulador.Bordeo.Model.Enities;
using DaSoft.Riviera.Modulador.Core.Controller;
using Nameless.Libraries.HoukagoTeaTime.Ritsu.Utils;
using System.IO;
using DaSoft.Riviera.Modulador.Core.Runtime;
using static DaSoft.Riviera.Modulador.Bordeo.Assets.Codes;
using static DaSoft.Riviera.Modulador.Bordeo.Assets.Constants;
using BordeoGeometryUtils = DaSoft.Riviera.Modulador.Core.Controller.GeometryUtils;
using Autodesk.AutoCAD.EditorInput;
using Nameless.Libraries.HoukagoTeaTime.Mio.Utils;
using DaSoft.Riviera.Modulador.Core.Controller.Transactions;
using DaSoft.Riviera.Modulador.Bordeo.Controller.Transactions;

namespace DaSoft.Riviera.Modulador.Bordeo.Controller
{
    /// <summary>
    /// Defines the Bordeo Sower process
    /// </summary>
    public class BordeoSower
    {
        public String MiscFolder = Path.Combine(App.Riviera.AppDirectory.FullName, FOLDER_NAME_BLOCKS_BORDEO);
        /// <summary>
        /// Gets or sets the menu.
        /// </summary>
        /// <value>
        /// The menu.
        /// </value>
        public UserControl Menu { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="BordeoSower"/> class.
        /// </summary>
        public BordeoSower(TabBordeoMenu bordeoMenu)
        {
            this.Menu = bordeoMenu;
        }
        /// <summary>
        /// Sows the specified panel.
        /// </summary>
        /// <param name="panel">The panel measure.</param>
        public void Sow()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            TabBordeoMenu ctrl = this.Menu as TabBordeoMenu;
            var data = ctrl.GetLinearPanels().ToArray();
            BordeoPanelStack panel0 = (BordeoPanelStack)new TransactionWrapper<RivieraMeasure, RivieraObject>(InitSowing).Run(data);
            ed.Regen();
            ArrowDirection dir = this.PickArrow(panel0, panel0, true, true);

        }
        /// <summary>
        /// Sows as Linear panel.
        /// </summary>
        /// <param name="doc">The active document.</param>
        /// <param name="tr">The active transaction.</param>
        protected RivieraObject InitSowing(Document doc, Transaction tr, params RivieraMeasure[] sizes)
        {
            Point3d p0, pf;
            if (Picker.Point("Selecciona el punto inicial", out p0) &&
                Picker.Point("Selecciona el punto final", p0, out pf))
            {
                IEnumerable<PanelMeasure> pSizes = sizes.Select(x => x as PanelMeasure);
                var first = sizes[0] as PanelMeasure;
                BordeoPanelStack stack = new BordeoPanelStack(p0, pf, first);
                for (int i = 1; i < sizes.Length; i++)
                    stack.AddPanel(sizes[i] as PanelMeasure);
                stack.Draw(tr);
                return stack;
            }
            else
                return null;
        }



        /// <summary>
        /// Sows as Linear panel.
        /// </summary>
        /// <param name="doc">The active document.</param>
        /// <param name="tr">The active transaction.</param>
        private ArrowDirection ContinueSowAsLinearPanel(Document doc, Transaction tr, params Object[] input)
        {
            RivieraObject last = input[0] as RivieraObject;
            ArrowDirection dir = (ArrowDirection)input[1];
            RivieraMeasure[] sizes = (RivieraMeasure[])input[2];
            Point3d pf = dir == ArrowDirection.FRONT ? last.End.ToPoint2dByPolar(1, last.Angle).ToPoint3d() : last.Start.ToPoint2dByPolar(1, last.Angle).ToPoint3d(),
                    p0 = dir == ArrowDirection.FRONT ? last.End.ToPoint3d() : last.Start.ToPoint3d();
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            IEnumerable<PanelMeasure> pSizes = sizes.Select(x => x as PanelMeasure);
            var first = sizes[0] as PanelMeasure;
            BordeoPanelStack stack = new BordeoPanelStack(p0, pf, first);
            for (int i = 1; i < sizes.Length; i++)
                stack.AddPanel(sizes[i] as PanelMeasure);
            stack.Draw(tr);
            BordeoPanel panel = stack.FirstOrDefault();
            ObjectIdCollection ids;
            if (dir == ArrowDirection.FRONT)
                ids = stack.DrawArrows(BordeoGeometryUtils.IsFront, panel.End.ToPoint3d(), panel.Angle, tr);
            else
                ids = stack.DrawArrows(BordeoGeometryUtils.IsBack, panel.Start.ToPoint3d(), panel.Angle, tr);
            ed.Regen();
            dir = stack.PickDirection(tr);
            ids.Erase(tr);
            App.Riviera.Database.Objects.Add(stack);
            return dir;
        }


        /// <summary>
        /// Picks the arrow.
        /// </summary>
        /// <param name="sowEntity">The sow entity.</param>
        /// <param name="rivObject">The riv object.</param>
        /// <param name="atStartPoint">if set to <c>true</c> [at start point].</param>
        /// <param name="atEndPoint">if set to <c>true</c> [at end point].</param>
        public ArrowDirection PickArrow(ISowable sowEntity, RivieraObject rivObject, Boolean atStartPoint = false, Boolean atEndPoint = false)
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            var drewIds = BordeoAutoCADTransactions.DrawArrows(sowEntity, rivObject, atStartPoint, atStartPoint);
            ed.Regen();
            rivObject.ZoomAt(2.5d);
            ArrowDirection direction = AutoCADTransactions.PickArrowDirection(sowEntity, drewIds);
            ed.Regen();
            return direction;
        }

    }
}
