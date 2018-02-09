using Autodesk.AutoCAD.DatabaseServices;
using DaSoft.Riviera.Modulador.Bordeo.Model.Enities;
using DaSoft.Riviera.Modulador.Core.Controller;
using DaSoft.Riviera.Modulador.Core.Model;
using DaSoft.Riviera.Modulador.Core.Runtime;
using Nameless.Libraries.HoukagoTeaTime.Yui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DaSoft.Riviera.Modulador.Bordeo.Assets.Strings;
namespace DaSoft.Riviera.Modulador.Bordeo.Controller
{
    public static class BordeoElementFinder
    {
        /// <summary>
        /// Picks the bordeo stack, the element must be a <see cref="BordeoPanelStack"/>
        /// or a <see cref="BordeoLPanelStack"/>
        /// </summary>
        /// <param name="msg">The selection message.</param>
        /// <param name="panelStack">As an output parameter the panel stack.</param>
        /// <returns>True if a panel stack is selected.</returns>
        public static Boolean PickBordeoStack(this string msg, out RivieraObject panelStack)
        {
            ObjectId selPanelStackId;
            panelStack = null;
            if (Picker.ObjectId(msg, out selPanelStackId, typeof(BlockReference)))
            {
                var objs = App.Riviera.Database.Objects;
                var obj = objs.FirstOrDefault(x => x.Ids.Contains(selPanelStackId));
                if (obj is BordeoPanelStack || obj is BordeoLPanelStack)
                    panelStack = obj;
                else
                    Selector.Ed.WriteMessage(String.Format(ERR_NOT_BORDERO, "ejecutar el commando"));
            }
            return panelStack != null;
        }

        /// <summary>
        /// Gets the Riviera objects that belongs to bordeo group.
        /// </summary>
        /// <param name="obj">The riviera object.</param>
        /// <returns>The list of riviera object</returns>
        public static IEnumerable<RivieraObject> GetGroup(this BordeoPanelStack panel)
        {
            return GetBordeoGroup(panel);
        }
        /// <summary>
        /// Gets the Riviera objects that belongs to bordeo group.
        /// </summary>
        /// <param name="obj">The riviera object.</param>
        /// <returns>The list of riviera object</returns>
        public static IEnumerable<RivieraObject> GetGroup(this BordeoLPanelStack panel)
        {
            return GetBordeoGroup(panel);
        }
        /// <summary>
        /// Gets the Riviera objects that belongs to bordeo group.
        /// </summary>
        /// <param name="obj">The riviera object.</param>
        /// <returns>The list of riviera object</returns>
        public static List<RivieraObject> GetBordeoGroup(this RivieraObject obj)
        {
            List<RivieraObject> objs = new List<RivieraObject>();
            var db = App.Riviera.Database.Objects;
            if (obj.IsRoot)
            {
                objs.Add(obj);
                var frontObj = obj.GetChildren(db, ArrowDirection.FRONT);
                while (frontObj != null)
                {
                    objs.Add(frontObj);
                    frontObj = frontObj.GetChildren(db, ArrowDirection.FRONT);
                }
                var backObj = obj.GetChildren(db, ArrowDirection.BACK);
                while (backObj != null)
                {
                    objs.Add(backObj);
                    backObj = backObj.GetChildren(db, ArrowDirection.FRONT);
                }
            }
            else
            {
                var parentObj = obj;
                while (!parentObj.IsRoot)
                    parentObj = parentObj.GetParent(db);
                objs = GetBordeoGroup(parentObj);
            }
            return objs;
        }
        /// <summary>
        /// Gets the Riviera objects that belongs to bordeo group.
        /// </summary>
        /// <param name="obj">The riviera object.</param>
        /// <returns>The list of riviera object</returns>
        public static List<RivieraObject> GetRivieraFront(this RivieraObject obj)
        {
            List<RivieraObject> objs = new List<RivieraObject>();
            var db = App.Riviera.Database.Objects;
            var frontObj = obj.GetChildren(db, ArrowDirection.FRONT);
            while (frontObj != null)
            {
                objs.Add(frontObj);
                frontObj = frontObj.GetChildren(db, ArrowDirection.FRONT);
            }
            return objs;
        }
        /// <summary>
        /// Gets the Riviera objects that belongs to bordeo group.
        /// </summary>
        /// <param name="obj">The 
        /// riviera object.</param>
        /// <returns>The list of riviera object</returns>
        public static List<RivieraObject> GetRivieraBack(this RivieraObject obj)
        {
            List<RivieraObject> objs = new List<RivieraObject>();
            var db = App.Riviera.Database.Objects;

            if (obj.IsRoot)
            {
                var back = obj.GetChildren(db, ArrowDirection.BACK);
                while (back != null)
                {
                    objs.Add(back);
                    back = back.GetChildren(db, ArrowDirection.FRONT);
                }
            }
            else
            {
                var back = obj;
                while (back != null && !back.IsRoot)
                {
                    back = back.GetChildren(db, ArrowDirection.BACK);
                    objs.Add(back);
                }
                if (back != null && !back.IsRoot)
                {
                    RivieraObject previous = objs[objs.Count - 2];
                    string fKey = ArrowDirection.FRONT.GetArrowDirectionName();
                    //Previous is Front
                    if (previous.Handle.Value == back.Children[fKey])
                    {
                        back = obj.GetChildren(db, ArrowDirection.BACK);
                        while (back != null)
                        {
                            objs.Add(back);
                            back = obj.GetChildren(db, ArrowDirection.BACK);
                        }
                    }//Previous is Back
                    else
                    {
                        back = obj.GetChildren(db, ArrowDirection.FRONT);
                        while (back != null)
                        {
                            objs.Add(back);
                            back = obj.GetChildren(db, ArrowDirection.FRONT);
                        }
                    }
                }
            }
            return objs;
        }
    }
}
