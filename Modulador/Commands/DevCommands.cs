using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using DaSoft.Riviera.Modulador.Bordeo.Model;
using DaSoft.Riviera.Modulador.Bordeo.Model.Enities;
using DaSoft.Riviera.Modulador.Bordeo.Testing;
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
using static DaSoft.Riviera.Modulador.Core.Assets.CMDS;
using static DaSoft.Riviera.Modulador.Core.Assets.CONST;
namespace DaSoft.Riviera.Modulador.Commands
{
    /// <summary>
    /// Defines the application commands
    /// </summary>
    public partial class DevCommands
    {
        /// <summary>
        /// Initializes the delta application.
        /// </summary>
        [CommandMethod("LoadMockingDatabase")]
        public void InitDeltaApplication()
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
                Picker.Point("Selecciona el punto final", out pf))
            {
                App.Riviera.Is3DEnabled = true;
                new QuickTransactionWrapper((Document doc, Transaction tr) => 
                {
                    BordeoPanelStack stack = new BordeoPanelStack(p0, pf);
                    RivieraSize frente = new RivieraSize() { Measure = KEY_FRONT, Nominal = 18, Real = 18 * 0.0254d },
                                alto1 = new RivieraSize() { Measure = KEY_HEIGHT, Nominal = 27, Real = 27 * 0.0254d },
                                alto2 = new RivieraSize() { Measure = KEY_HEIGHT, Nominal = 15, Real = 15 * 0.0254d };
                    PanelMeasure panel = new PanelMeasure(frente, alto1),
                                 panel2 = new PanelMeasure(frente, alto2);
                    stack.AddPanel(panel);
                    stack.AddPanel(panel2);
                    stack.Draw(tr);
                }).Run();
            }
        }
    }
}
