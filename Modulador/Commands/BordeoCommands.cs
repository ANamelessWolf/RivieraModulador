﻿using Autodesk.AutoCAD.ApplicationServices;
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
                       ObjectId entId;
                       int state;
                       if (Picker.ObjectId("Selecciona un panel para continuar la inserción", out entId, typeof(Line), typeof(Polyline)))
                       {
                           RivieraObject rivObj = App.Riviera.Database.Objects.FirstOrDefault(x => x.Id == entId);
                           if (rivObj != null)
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
                           else
                               Selector.Ed.WriteMessage("No es un elemento de bordeo");
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
                       ObjectId selPanelStackId;
                       if (Picker.ObjectId("Selecciona el panel a editar", out selPanelStackId, typeof(BlockReference)))
                       {
                           var objs = App.Riviera.Database.Objects;
                           var obj = objs.FirstOrDefault(x => x.Ids.Contains(selPanelStackId));
                           if (obj is BordeoPanelStack || obj is BordeoLPanelStack)
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

        [CommandMethod(BORDEO_PANEL_UPDATE_SIZE)]
        public void BordeoUpdateFrenteSize()
        {
            App.RunCommand(
               delegate ()
               {
                   try
                   {
                       ObjectId selPanelStackId;
                       if (Picker.ObjectId("Selecciona el panel a editar", out selPanelStackId, typeof(BlockReference)))
                       {
                           var objs = App.Riviera.Database.Objects;
                           var obj = objs.FirstOrDefault(x => x.Ids.Contains(selPanelStackId));
                           if (obj is BordeoPanelStack || obj is BordeoLPanelStack)
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
