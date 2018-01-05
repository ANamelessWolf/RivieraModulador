using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using DaSoft.Riviera.Modulador.Bordeo.Model;
using DaSoft.Riviera.Modulador.Bordeo.Model.Enities;
using DaSoft.Riviera.Modulador.Bordeo.Testing;
using DaSoft.Riviera.Modulador.Bordeo.UI;
using DaSoft.Riviera.Modulador.Core.Model;
using DaSoft.Riviera.Modulador.Core.Runtime;
using Nameless.Libraries.HoukagoTeaTime.Mio.Utils;
using Nameless.Libraries.HoukagoTeaTime.Runtime;
using Nameless.Libraries.HoukagoTeaTime.Yui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using static DaSoft.Riviera.Modulador.Core.Assets.CMDS;
using static DaSoft.Riviera.Modulador.Core.Assets.CONST;
using static DaSoft.Riviera.Modulador.Bordeo.Assets.Codes;
using DaSoft.Riviera.Modulador.Bordeo.Controller;
using Autodesk.AutoCAD.EditorInput;

namespace DaSoft.Riviera.Modulador.Commands
{
    /// <summary>
    /// Defines the application commands
    /// </summary>
    public partial class ModuladorCommands
    {
        /// <summary>
        /// Initializes the delta application.
        /// </summary>
        [CommandMethod("LoadMockingDatabase")]
        public void InitDeltaApplicationAsMock()
        {
            if (App.Riviera == null)
                App.Riviera = new RivieraApplication();
            App.Riviera.Is3DEnabled = false;
            App.Riviera.Database = new RivieraDatabase();
            App.Riviera.Database.LineDB.Add(DesignLine.Bordeo, new BordeoMockingDesignDatabase());
            var doc = Application.DocumentManager.MdiActiveDocument;
            doc.SendStringToExecute(INIT_APP_UI + " ", true, false, false);
        }

        [CommandMethod("TestPanelInsert")]
        public void TestPanelInsert()
        {
            Point3d p0, pf;
            if (Picker.Point("Selecciona el punto inicial", out p0) &&
                Picker.Point("Selecciona el punto final", p0, out pf))
            {
                App.Riviera.Is3DEnabled = true;
                new QuickTransactionWrapper((Document doc, Transaction tr) =>
                {
                    RivieraSize frente = new RivieraSize() { Measure = KEY_FRONT, Nominal = 18, Real = 18 * 0.0254d },
                                alto1 = new RivieraSize() { Measure = KEY_HEIGHT, Nominal = 27, Real = 27 * 0.0254d },
                                alto2 = new RivieraSize() { Measure = KEY_HEIGHT, Nominal = 15, Real = 15 * 0.0254d };
                    PanelMeasure panel = new PanelMeasure(frente, alto1),
                                 panel2 = new PanelMeasure(frente, alto2);
                    BordeoPanelStack stack = new BordeoPanelStack(p0, pf, panel);
                    stack.AddPanel(panel2);
                    stack.Draw(tr);
                }).Run();
            }
        }

        [CommandMethod("TestLPanelInsert")]
        public void TestPanelLInsert()
        {
            Point3d start;
            Point3d end;
            TabBordeoMenu ctrl = this.Controls.Where(x => x is TabBordeoMenu).FirstOrDefault() as TabBordeoMenu;
            BordeoLPanel panel;
            var size = ctrl.GetL90Panels().FirstOrDefault() as LPanelMeasure;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            for (int i = 0; i < 4 && ctrl.SelectHeights.Length > 0; i++)
                new QuickTransactionWrapper((Document doc, Transaction tr) =>
                {
                    if (Picker.Point("Selecciona el punto inicial", out start))
                    {
                        end = start + new Vector3d(10, 0, 0);
                        if (i == 0)
                            panel = new BordeoL90Panel(SweepDirection.Clockwise, start, end, size);
                        else if (i == 1)
                            panel = new BordeoL90Panel(SweepDirection.Counterclockwise, start, end, size);
                        else if (i == 2)
                            panel = new BordeoL90Panel(SweepDirection.Clockwise, start, end, size.InvertFront());
                        else
                            panel = new BordeoL90Panel(SweepDirection.Counterclockwise, start, end, size.InvertFront());
                        panel.Draw(tr);
                        ed.Regen();
                    }
                }).Run();
        }
    }
}
