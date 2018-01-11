using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using DaSoft.Riviera.Modulador.Bordeo.Controller;
using DaSoft.Riviera.Modulador.Bordeo.Model.Enities;
using DaSoft.Riviera.Modulador.Bordeo.UI;
using DaSoft.Riviera.Modulador.Core.Controller;
using DaSoft.Riviera.Modulador.Core.Model;
using DaSoft.Riviera.Modulador.Core.Runtime;
using Nameless.Libraries.HoukagoTeaTime.Yui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DaSoft.Riviera.Modulador.Core.Assets.CMDS;
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
                   try
                   {
                       TabBordeoMenu ctrl = this.Controls.Where(x => x is TabBordeoMenu).FirstOrDefault() as TabBordeoMenu;
                       BordeoSower sow = new BordeoSower(ctrl);
                       sow.Sow();
                   }
                   catch (System.Exception exc)
                   {
                       Selector.Ed.WriteMessage(exc.Message);
                   }
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
                   try
                   {
                       ObjectId entId;
                       int state;
                       if (Picker.ObjectId("Selecciona un panel para continuar la inserción", out entId, typeof(Line), typeof(Polyline)))
                       {
                           RivieraObject rivObj = App.Riviera.Database.Objects.FirstOrDefault(x => x.Id == entId);
                           if (rivObj != null)
                           {
                               TabBordeoMenu ctrl = this.Controls.Where(x => x is TabBordeoMenu).FirstOrDefault() as TabBordeoMenu;
                               BordeoSower sow = new BordeoSower(ctrl);
                               if (rivObj is BordeoPanelStack)
                                   state = 1;
                               else if (rivObj is BordeoLPanelStack)
                                   state = 2;
                               else
                                   state = 0;
                               var dir = sow.PickArrow(rivObj as ISowable);
                               sow.Sow(dir, rivObj, state);
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
