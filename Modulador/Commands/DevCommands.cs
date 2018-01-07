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
using Nameless.Libraries.HoukagoTeaTime.Mio.Entities;
using DaSoft.Riviera.Modulador.Core.Controller;
using System.IO;

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
                        else if (i == 3)
                            panel = new BordeoL90Panel(SweepDirection.Counterclockwise, start, end, size.InvertFront());
                        else
                            panel = null;
                        if (panel != null)
                            panel.Draw(tr);
                        ed.Regen();
                    }
                }).Run();
        }
        int index = 0;
        [CommandMethod("TestBlockLoad")]
        public void TestBlockLoad()
        {
            new QuickTransactionWrapper((Document doc, Transaction tr) =>
            {
                try
                {
                    string blockPath = @"D:\DaSoft\Proyectos\RivModulador\Modulador\Bordeo\2D\BR2020181815T.dwg";
                    AutoCADUtils._LoadBlock(String.Format("test{0}", index++), blockPath, tr);
                }
                catch (System.Exception exc)
                {
                    Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
                    ed.WriteMessage(exc.Message);
                }

            }).Run();
        }

        [CommandMethod("TestBlockDraw")]
        public void TestBlock()
        {
            new QuickTransactionWrapper((Document doc, Transaction tr) =>
            {
                try
                {
                    Point3d start, end;
                    TabBordeoMenu ctrl = this.Controls.Where(x => x is TabBordeoMenu).FirstOrDefault() as TabBordeoMenu;
                    Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
                    if (Picker.Point("Selecciona el punto inicial", out start))
                    {
                        end = start + new Vector3d(10, 0, 0);
                        var size = ctrl.GetL90Panels().FirstOrDefault() as LPanelMeasure;
                        var panel = new BordeoL90Panel(SweepDirection.Counterclockwise, start, end, size);
                        panel.Draw(tr);
                        this.InitContent(panel, tr);
                        ed.Regen();
                    }
                }
                catch (System.Exception exc)
                {
                    Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
                    ed.WriteMessage(exc.Message);
                }

            }).Run();
        }

        private void InitContent(BordeoL90Panel panel, Transaction tr)
        {
            Dictionary<LBlockType, AutoCADBlock>
                blocks2D = new Dictionary<LBlockType, AutoCADBlock>(),
                blocks3D = new Dictionary<LBlockType, AutoCADBlock>();
            RivieraLBlock rBlock = (panel.Block as RivieraLBlock);
            AutoCADBlock block2d = new AutoCADBlock(String.Format(rBlock.Block2DName, rBlock.BlockName), rBlock.GetBlockFilePath(), tr);
            AutoCADBlock block3d = new AutoCADBlock(String.Format(rBlock.Block3DName, rBlock.BlockName), rBlock.GetBlockFilePath(false), tr);
            AutoCADBlock varBlock2d = new AutoCADBlock(String.Format(SUFFIX_BLOCK2D, rBlock.VariantBlockName), rBlock.GetBlockFilePath(rBlock.VariantBlockName), tr);
            AutoCADBlock varBlock3d = new AutoCADBlock(String.Format(SUFFIX_BLOCK3D, rBlock.VariantBlockName), rBlock.GetBlockFilePath(rBlock.VariantBlockName, false), tr);
            (panel.Block as RivieraLBlock).LoadBlocks(Application.DocumentManager.MdiActiveDocument, tr, out blocks2D, out blocks3D);
            string blockName = block2d.Blockname.Substring(0, block2d.Blockname.Length - 2),
                   variantBlockName = varBlock2d != null ? varBlock2d.Blockname.Substring(0, varBlock2d.Blockname.Length - 2) : null;
            AutoCADBlock blockRecord = blocks2D[LBlockType.LEFT_START_MIN_SIZE];
            blockRecord.Open(tr, OpenMode.ForWrite);
            blockRecord.Clear(tr);
            String code = blockName.Substring(0, 6);
            Double frente1 = 0.3748d,
                frente2 = 0.5278d;
            BlockReference blkRef = block2d.CreateReference(new Point3d(), 0);
            //Se rota 270°
            blkRef.TransformBy(Matrix3d.Rotation(3 * Math.PI / 2, Vector3d.ZAxis, new Point3d()));
            Vector3d offset = new Vector3d(frente2, frente1, 0);
            //Offset BR2020
            if (code == CODE_PANEL_90)
                offset = new Vector3d(offset.X + 0.1002d, offset.Y + 0.1002d, 0);
            //Offset BR2030
            else
                offset = new Vector3d(offset.X + 0.0709d, offset.Y + 0.0293d, 0);
            //Se traslada el punto final al punto inicial
            blkRef.TransformBy(Matrix3d.Displacement(new Vector3d(offset.X, offset.Y, 0)));
            blockRecord.Draw(tr, blkRef);
        }
    }
}
