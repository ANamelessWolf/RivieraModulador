using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using DaSoft.Riviera.Modulador.Bordeo.Controller;

using DaSoft.Riviera.Modulador.Bordeo.Model.Enities;
using DaSoft.Riviera.Modulador.Bordeo.UI;
using DaSoft.Riviera.Modulador.Core.Controller;
using DaSoft.Riviera.Modulador.Core.Model;
using DaSoft.Riviera.Modulador.Core.Runtime;
using DaSoft.Riviera.Modulador.Core.UI;
using Nameless.Libraries.HoukagoTeaTime.Yui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DaSoft.Riviera.Modulador.Core.Assets.CMDS;
using static DaSoft.Riviera.Modulador.Core.Assets.CONST;
using static DaSoft.Riviera.Modulador.Bordeo.Assets.Codes;
using Nameless.Libraries.HoukagoTeaTime.Runtime;
using System.Windows.Media.Media3D;
using Nameless.Libraries.HoukagoTeaTime.Ritsu.Utils;
using Autodesk.AutoCAD.Geometry;
using Nameless.Libraries.HoukagoTeaTime.Mio.Entities;
using NamelessOld.Libraries.Yggdrasil.Aerith;
using NamelessOld.Libraries.HoukagoTeaTime.Runtime;
using DaSoft.Riviera.Modulador.Bordeo.Model;
using DaSoft.Riviera.Modulador.Bordeo;

namespace DaSoft.Riviera.Modulador.Commands
{
    /// <summary>
    /// Defines the application commands
    /// </summary>
    public partial class ModuladorCommands
    {
        /// <summary>
        /// Initializes the sowing process
        /// </summary>
        [CommandMethod(BORDEO_SOW_START)]
        public void StartSowing()
        {
            App.RunCommand(
               delegate ()
               {
                   TabBordeoMenu ctrl = null;
                   try
                   {
                       ctrl = this.Controls.Where(x => x is TabBordeoMenu).FirstOrDefault() as TabBordeoMenu;
                       ctrl.SetHeightEnableStatus(false);
                       BordeoSower sow = new BordeoSower(ctrl);
                       sow.Sow();
                   }
                   catch (System.Exception exc)
                   {
                       Selector.Ed.WriteMessage(exc.Message);
                   }
                   ctrl?.SetHeightEnableStatus(true);
               });
        }
        /// <summary>
        /// Continue the sowing process
        /// </summary>
        [CommandMethod(BORDEO_SOW_CONTINUE)]
        public void ContinueSowing()
        {
            App.RunCommand(
               delegate ()
               {
                   TabBordeoMenu ctrl = null;
                   try
                   {
                       RivieraObject rivObj;
                       int state;
                       if ("Selecciona un panel para continuar la inserción".PickBordeoStack(out rivObj))
                       {
                           ctrl = this.Controls.Where(x => x is TabBordeoMenu).FirstOrDefault() as TabBordeoMenu;
                           ctrl.SetHeightEnableStatus(false);
                           BordeoSower sow = new BordeoSower(ctrl);
                           if (rivObj is BordeoPanelStack)
                               state = 1;
                           else if (rivObj is BordeoLPanelStack)
                               state = 2;
                           else
                               state = 0;
                           var dir = sow.PickArrow(rivObj as ISowable);
                           sow.Sow(dir, rivObj, state);
                           ctrl.SetHeightEnableStatus(true);
                       }
                   }
                   catch (System.Exception exc)
                   {
                       Selector.Ed.WriteMessage(exc.Message);
                   }
                   ctrl?.SetHeightEnableStatus(true);
               });
        }
        /// <summary>
        /// Initializes the sowing process
        /// </summary>
        [CommandMethod(BORDEO_EDIT_PANELS)]
        public void BordeoEditPanels()
        {
            App.RunCommand(
               delegate ()
               {
                   try
                   {
                       RivieraObject obj;
                       if ("Selecciona el panel a editar".PickBordeoStack(out obj))
                       {
                           WinPanelEditor win = new WinPanelEditor(obj as IBordeoPanelStyler);
                           if (win.ShowDialog().Value)
                           {
                               var connected = obj.GetBordeoGroup();
                               foreach (var objConn in connected)
                               {
                                   if (objConn.Handle.Value == obj.Handle.Value)
                                       (objConn as IBordeoPanelStyler).UpdatePanelStack(win.Heights, win.AcabadosLadoA.ToArray(), win.AcabadosLadoB.ToArray());
                                   else
                                       (objConn as IBordeoPanelStyler).UpdatePanelStack(win.Heights, null, null);
                               }
                           }
                       }
                   }
                   catch (System.Exception exc)
                   {
                       Selector.Ed.WriteMessage(exc.Message);
                   }
               });
        }

        [CommandMethod(BORDEO_PANEL_UPDATE_SIZE)]
        public void BordeoUpdateFrenteSize()
        {
            App.RunCommand(
               delegate ()
               {
                   try
                   {
                       RivieraObject obj;
                       if ("Selecciona el panel a editar".PickBordeoStack(out obj))
                       {
                           ArrowDirection dir = (obj as IBordeoPanelStyler).GetMoveDirection();
                           new QuickTransactionWrapper((Document doc, Transaction tr) =>
                           {
                               if (dir != ArrowDirection.NONE)
                               {
                                   String defFront = (obj as IBordeoPanelStyler).GetDefaultMovingFront(dir);
                                   WinSelectFront win = new WinSelectFront(BordeoUtils.GetDatabase(), CODE_PANEL_RECTO, defFront);
                                   if (win.ShowDialog().Value)
                                       (obj as IBordeoPanelStyler).Move(tr, dir, win.SelectedFront);
                               }
                           }).Run();
                       }
                   }
                   catch (System.Exception exc)
                   {
                       Selector.Ed.WriteMessage(exc.Message);
                   }
               });
        }
        [CommandMethod("BordeoInsertarPuente")]
        public void InsertarPuentes()
        {
            App.RunCommand(
            delegate ()
            {
                try
                {
                    RivieraObject obj;
                    if ("Selecciona el panel a insertar un puente".PickBordeoStack(out obj) &&
                    obj is BordeoPanelStack)
                    {
                        WinBridgeEditor win = new WinBridgeEditor();
                        if (win.ShowDialog().Value)
                        {
                            var panel = (BordeoPanel)(obj as BordeoPanelStack).FirstOrDefault();
                            BridgeSelectionResult bridge = win.SelectedBridge;
                            BordeoBridge b = new BordeoBridge(bridge.SelectedCode, bridge.AcabadoPazo, bridge.AcabadoPuentes, panel.PanelSize, panel.Start.ToPoint3d(), panel.End.ToPoint3d());
                            b.SetElevation((obj as BordeoPanelStack).Sum(x => x.PanelSize.Alto.Nominal));
                            var fTw = new FastTransactionWrapper(
                                (Document doc, Transaction tr) =>
                                {
                                    b.Draw(tr);
                                    var db = App.Riviera.Database.Objects;
                                    if (db.FirstOrDefault(x => x.Handle.Value == b.Handle.Value) == null)
                                        db.Add(b);
                                    b.Save(tr);
                                });
                            fTw.Run();
                        }
                    }
                }
                catch (System.Exception exc)
                {
                    Selector.Ed.WriteMessage(exc.Message);
                }
            });
        }

        [CommandMethod("BordeoRefresh")]
        public void BordeoRefresh()
        {
            App.RunCommand(
            delegate ()
            {
                try
                {
                    var fTw = new FastTransactionWrapper(
                        (Document doc, Transaction tr) =>
                        {
                            BordeoStationBuilder bSB = new BordeoStationBuilder(tr);
                            var stations = App.Riviera.Database.Objects.Where(x => x.Code.Code == CODE_STATION).ToArray();
                            foreach (RivieraObject s in stations)
                                s.Delete(tr);
                            Ed.Regen();
                            bSB.SelectEntities();
                            AutoCADLayer layer = new AutoCADLayer(LAYER_RIVIERA_STATION, tr);
                            Entity ent;
                            List<BordeoStationVertex> objs;
                            BordeoStation station;
                            Point3d start;
                            while (bSB.BordeoEnds.Count > 0)
                            {
                                ent = bSB.BordeoEnds.FirstOrDefault();
                                start = ent is Line ? (ent as Line).StartPoint : (ent is Polyline) ? (ent as Polyline).StartPoint : new Point3d();
                                bSB.BordeoEnds.Remove(ent);
                                objs = bSB.BuildStation(App.Riviera.Database, ent);
                                station = new BordeoStation(start, objs);
                                station.Draw(tr);
                                App.Riviera.Database.Objects.Add(station);
                            }
                        });
                    fTw.Run();
                }
                catch (System.Exception exc)
                {
                    Selector.Ed.WriteMessage(exc.Message);
                }
            });
        }


        [CommandMethod(BORDEO_DELETE_STACK)]
        public void BordeoDeleteStacks()
        {
            App.RunCommand(
             delegate ()
             {
                 try
                 {
                     RivieraObject obj;
                     if ("Selecciona el panel a eliminar".PickBordeoStack(out obj))
                     {
                         if (obj.Children[KEY_DIR_FRONT] > 0 && obj.Children[KEY_DIR_BACK] > 0)
                             throw new System.Exception("El elemento no es final");
                         else
                             new QuickTransactionWrapper((Document doc, Transaction tr) => obj.Delete(tr)).Run();
                         App.Riviera.Database.Clean();
                     }
                 }
                 catch (System.Exception exc)
                 {
                     Selector.Ed.WriteMessage(exc.Message);
                 }
             });
        }

        [CommandMethod(BORDEO_PANEL_COPY_PASTE)]
        public void BordeoClipboard()
        {
            App.RunCommand(
              delegate ()
              {
                  try
                  {
                      ObjectId selPanelStackId;
                      ObjectIdCollection stacksIds;
                      SelectionFilterBuilder sb = new SelectionFilterBuilder(typeof(BlockReference));
                      if (Picker.ObjectId("Selecciona el arreglo de paneles a copiar", out selPanelStackId, typeof(BlockReference)) &&
                          Picker.ObjectIds("Selecciona en donde se pegará el estilo de paneles", out stacksIds, sb.Filter))
                      {
                          var objs = App.Riviera.Database.Objects;
                          var obj = objs.FirstOrDefault(x => x.Ids.Contains(selPanelStackId));
                          if (obj is BordeoPanelStack || obj is BordeoLPanelStack)
                          {
                              var stacks = stacksIds.OfType<ObjectId>().
                              Select(x => objs.FirstOrDefault(y => y.Ids.Contains(x))).
                              Where(z => (obj is BordeoPanelStack || obj is BordeoLPanelStack)).Select(x => x as IBordeoPanelStyler).ToList();
                              var cbstack = (obj as IBordeoPanelStyler);
                              String[] acabA = cbstack.AcabadosLadoA.Select(y => y.Acabado).ToArray(),
                                       acabB = cbstack.AcabadosLadoB.Select(y => y.Acabado).ToArray();
                              stacks.ForEach(x =>
                              {
                                  x.UpdatePanelStack(cbstack.Height, acabA, acabB);

                              });
                          }
                          else
                              Selector.Ed.WriteMessage("No es un elemento de bordeo");
                      }
                  }
                  catch (System.Exception exc)
                  {
                      Selector.Ed.WriteMessage(exc.Message);
                  }
              });
        }
    }
}
