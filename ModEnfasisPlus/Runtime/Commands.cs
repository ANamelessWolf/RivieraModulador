using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using DaSoft.Riviera.OldModulador.Controller;
using DaSoft.Riviera.OldModulador.Controller.Delta;
using DaSoft.Riviera.OldModulador.Model;
using DaSoft.Riviera.OldModulador.Model.Delta;
using DaSoft.Riviera.OldModulador.UI;
using NamelessOld.Libraries.HoukagoTeaTime.Mio;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu;
using NamelessOld.Libraries.HoukagoTeaTime.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Yui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static DaSoft.Riviera.OldModulador.Assets.Strings;
using static DaSoft.Riviera.OldModulador.Runtime.CMD;

namespace DaSoft.Riviera.OldModulador.Runtime
{
    public class Commands
    {


        [CommandMethod(START)]
        public void InitDeltaApplication()
        {
            App.Riviera = new RivieraApplication("Modulador");
            App.Riviera.Is3DEnabled = false;
            this.RefreshApp(true);
        }

        [CommandMethod(LOAD_ZONES)]
        public void LoadZones()
        {
            new FastTransactionWrapper(delegate (Document doc, Transaction tr)
            {
                List<String> zones = new List<string>();
                if (App.DB.Objects != null)
                    App.DB.Objects.ForEach(x => zones.Add(x.GetZone(tr)));

                IEnumerable<String> insertedZones = zones.Distinct();
                ZoneManager zoneMan = new ZoneManager();
                List<String> currentZones = zoneMan.ListZones();
                foreach (String zone in insertedZones)
                {
                    if (!currentZones.Contains(zone))
                        zoneMan.AddZone(zone);
                }
            }).Run();
        }



        [CommandMethod(ABOUT)]
        public void AboutEnfasisPlus()
        {
            App.RunCommand(delegate ()
            {
                new Dialog_About().ShowDialog();
            });
        }

        [CommandMethod("NUSEL")]
        public void NuSelect()
        {
            ObjectId id;
            if (Selector.ObjectId("", out id, typeof(Polyline)))
            {
                Point3dCollection pts = (id.OpenEntity() as Polyline).Vertices();
                PromptSelectionResult res = Selector.Ed.SelectCrossingPolygon(pts);
                if (res.Status == PromptStatus.OK)
                    Selector.Ed.SetImpliedSelection(res.Value.OfType<SelectedObject>().Select<SelectedObject, ObjectId>(x => x.ObjectId).ToArray());
            }
        }

        [CommandMethod(INSERT_MODULE)]
        public void InsertModule()
        {
            App.RunCommand(delegate ()
            {
                Dialog_InsertModule d = new Dialog_InsertModule();
                d.ShowDialog();
                do
                {
                    Point3d insPt = new Point3d();
                    if (d.Action == ModuleAction.Ok)
                    {
                        try
                        {
                            ModuleManager mMan = new ModuleManager();
                            while (Selector.Point("Seleccione el punto de inserción del bloque", out insPt, true))
                                mMan.InsertBlock(d.ModuleName, insPt);
                            d.ShowDialog();
                        }
                        catch (System.Exception)
                        {
                            d.Action = ModuleAction.Cancel;
                        }


                    }
                    else
                        d.Action = ModuleAction.Cancel;
                } while (d.Action != ModuleAction.Cancel);
            });
        }

        [CommandMethod(MODULE_CREATOR)]
        public void OpenModuleCreator()
        {
            App.RunCommand(delegate ()
            {
                List<Tuple<string, int>> claves = null;
                System.Windows.Media.ImageSource imgSrc = null;

                Dialog_ModuleCreator d;
                ModuleManager mMan = new ModuleManager();
                FixMTextWidth();
                d = new Dialog_ModuleCreator(claves, imgSrc);
                do
                {
                    d.Init(claves, imgSrc);
                    d.ShowDialog();
                    try
                    {
                        if (d.Action == ModuleAction.Select && d.Point != new Point3d() && mMan.Pick())
                        {
                            mMan.InsPoint = d.Point;
                            claves = mMan.Quantify();
                            imgSrc = mMan.CreateTemporalBlocks();
                        }
                        else if (d.Action == ModuleAction.Select && d.Point == new Point3d() && mMan.Pick())
                            Dialog_MessageBox.Show("Seleccione primero el punto de inserción del módulo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                        else if (d.Action == ModuleAction.Ok && d.ModuleName != String.Empty)
                        {
                            mMan.WriteBlock(d.ModuleName);
                            mMan.SaveModule(d.ModuleName, claves);
                        }
                        else if (d.Action == ModuleAction.Ok && d.ModuleName == String.Empty)
                            d.Action = ModuleAction.None;
                        else
                        {
                            claves = null;
                            imgSrc = null;
                        }
                    }
                    catch (System.Exception exc)
                    {
                        Dialog_MessageBox.Show(exc.Message, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                        d.Action = ModuleAction.None;
                    }
                } while (d.Action != ModuleAction.Ok && d.Action != ModuleAction.Cancel);
            });
        }
        /// <summary>
        /// Fix the MTextWidth
        /// </summary>
        private void FixMTextWidth()
        {
            new FastTransactionWrapper(delegate (Document doc, Transaction tr)
            {
                BlockTable blkTab = (BlockTable)doc.Database.BlockTableId.GetObject(OpenMode.ForRead);
                BlockTableRecord rec;
                foreach (ObjectId recId in blkTab)
                {
                    rec = (BlockTableRecord)recId.GetObject(OpenMode.ForRead);
                    foreach (ObjectId entId in rec)
                    {
                        DBObject obj = entId.GetObject(OpenMode.ForRead);
                        if (obj is MText && ((obj as MText).Width - (obj as MText).Text.Length * (obj as MText).TextHeight * 2) > 2d)
                        {
                            DBText nText = new DBText();
                            nText.TextString = (obj as MText).Text;
                            nText.Height = (obj as MText).TextHeight;
                            nText.Rotation = (obj as MText).Rotation;
                            nText.Position = new Point3d((obj as MText).Location.X, (obj as MText).Location.Y - (obj as MText).TextHeight, (obj as MText).Location.Z);
                            Drawer.Erase(tr, obj.Id);
                            rec.UpgradeOpen();
                            rec.AppendEntity(nText);
                            tr.AddNewlyCreatedDBObject(nText, true);
                        }
                    }
                }
            }).Run();
        }

        [CommandMethod(CONFIG_UI)]
        public void Configuration()
        {

            if (App.Riviera == null)
                App.Riviera = new RivieraApplication("Modulador");
            Dialog_ConnectionSettings d = new Dialog_ConnectionSettings();
            d.ShowDialog();
        }

        [CommandMethod(SWAP_3D_MODE)]
        public void Swap3DMode()
        {
            App.RunCommand(delegate ()
            {
                VoidTransactionWrapper<Boolean> trW = new VoidTransactionWrapper<Boolean>(
                    delegate (Document doc, Transaction tr, Boolean[] input)
                    {
                        ChangeViewer cw = new ChangeViewer(doc, tr, input[0]);
                        cw.Swap();
                    });
                trW.Run(App.Riviera.Is3DEnabled);
                //Se guarda el status del modo de vista actual

                //Se regenera el documento
                Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.Regen();
            });
        }

        [CommandMethod(PRINT_EXTRA)]
        public void SnoopMampara()
        {
            App.RunCommand(delegate ()
            {
                ObjectId id;
                if (Selector.ObjectId(MSG_SEL_OBJ, out id, typeof(BlockReference), typeof(Polyline)))
                {
                    RivieraObject obj = App.DB[id];
                    if (obj != null)
                    {
                        FastTransactionWrapper trW = new FastTransactionWrapper(
                            delegate (Document doc, Transaction tr)
                        {

                            String s = obj.Data[FIELD_EXTRA, tr];
                            if (obj is RivieraPanelStack)
                            {
                                StringBuilder sb = new StringBuilder();
                                String[] lines = s.Split('@');
                                foreach (String line in lines.Where(x => x != String.Empty))
                                    sb.Append(String.Format(
                                        "Panel: {0}\nAltura: {1}\nBloque: {2}.dwg\nNivel: {3}\nDirección: {4}, A piso: {5}\nAcabado: {6}\n\n", line.Split('|')));
                                Selector.Ed.WriteMessage(sb.ToString());
                            }
                            else
                                Selector.Ed.WriteMessage(s);
                        });
                        trW.Run();
                    }
                }
            });
        }

        /// <summary>
        /// Este comando se encarga de volver a dibujar la geometría y asociarla de nuevo
        /// a las mamparas insertadas.
        /// Antes de usar este comando, borrar la geometría existente, que este en la capa
        /// DD2D_Geometry, en el recurso Strings.LAYER_RIVIERA_GEOMETRY
        /// </summary>
        [CommandMethod(REDRAW_GEOMETRY)]
        public void RedrawGeometry()
        {
            App.RunCommand(delegate ()
            {
                List<ObjectId> elNotErased;
                List<RivieraObject> elNotFixed;
                App.DB.CleanGeometry(out elNotErased, out elNotFixed);
                if (elNotErased.Count > 0 || elNotFixed.Count > 0)
                    Selector.Ed.WriteMessage("\nError al regenerar la geometría.");
                else
                    Selector.Ed.WriteMessage("\nGeometría generada de forma correcta.");
                this.RefreshApp(false);
            });
        }
        [CommandMethod(DELTA_FLIP_54_MAMPARA)]
        public void FlipSelectedPolyline()
        {
            App.RunCommand(delegate ()
            {
                try
                {
                    Mampara54Flipper flip = new Mampara54Flipper();
                    new FastTransactionWrapper(
                        (Document doc, Transaction tr) =>
                        {
                            flip.Swap(tr);
                            flip.Regen(tr);


                        }).Run();
                }
                catch (System.Exception exc)
                {
                    Selector.Ed.WriteMessage(exc.Message);
                }
            });
        }
        [CommandMethod(REGEN_IDS)]
        public void RegenIds()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            App.RunCommand(delegate ()
            {
                App.CleanCache();
                new FastTransactionWrapper(
                    delegate (Document doc, Transaction tr)
                    {
                        BlockTableRecord currentSpace = doc.Database.CurrentSpaceId.GetObject(OpenMode.ForRead) as BlockTableRecord;
                        ModuladorIdMapper mapper = new ModuladorIdMapper();
                        DBObject obj;
                        //Se recolecta la información de los ids de los bloques
                        foreach (ObjectId id in currentSpace)
                        {
                            obj = id.GetObject(OpenMode.ForRead);
                            //En caso de necesitar cambiar los ids se agregá a la lista
                            if (mapper.IsRivieraObject(tr, obj))
                                mapper.AddObject(obj);
                        }
                        //Arregla los ids
                        mapper.Fix(tr);
                        App.DB.LoadDrawingData();
                        List<ObjectId> elNotErased;
                        List<RivieraObject> elNotFixed;
                        App.DB.CleanGeometry(out elNotErased, out elNotFixed);
                        if (elNotErased.Count > 0 || elNotFixed.Count > 0)
                            Selector.Ed.WriteMessage("\nError al regenerar la geometría.");
                        else
                            Selector.Ed.WriteMessage("\nGeometría generada de forma correcta.");
                        this.RefreshApp(false);
                    }).Run();
            });
            sw.Stop();
            Selector.Ed.WriteMessage("Tiempo: {0}s, {1}ms", sw.Elapsed.Seconds, sw.Elapsed.Milliseconds);
        }


        /// <summary>
        /// Actualiza el estado de la aplicación
        /// </summary>
        private void RefreshApp(Boolean loadUI)
        {
            if (File.Exists(App.Riviera.ConnectionFile.FullName))
                App.DB.Init(loadUI);
            App.DB.LoadDrawingData();
            App.InitLayers();
            if (loadUI)
                App.CleanCache();
        }
    }
}
