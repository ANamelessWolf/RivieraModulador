using DaSoft.Riviera.Modulador.Bordeo.Model.Enities;
using DaSoft.Riviera.Modulador.Core.Model;
using DaSoft.Riviera.Modulador.Core.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Bordeo.Controller
{
    public static class BordeoElementFinder
    {
        /// <summary>
        /// Gets the Riviera objects that belongs to bordeo group.
        /// </summary>
        /// <param name="obj">The riviera object.</param>
        /// <returns>The list of riviera object</returns>
        public static IEnumerable<RivieraObject> GetGroup(this BordeoPanelStack panel)
        {
            return GetRivieraGroup(panel);
        }
        /// <summary>
        /// Gets the Riviera objects that belongs to bordeo group.
        /// </summary>
        /// <param name="obj">The riviera object.</param>
        /// <returns>The list of riviera object</returns>
        public static IEnumerable<RivieraObject> GetGroup(this BordeoLPanelStack panel)
        {
            return GetRivieraGroup(panel);
        }
        /// <summary>
        /// Gets the Riviera objects that belongs to bordeo group.
        /// </summary>
        /// <param name="obj">The riviera object.</param>
        /// <returns>The list of riviera object</returns>
        private static List<RivieraObject> GetRivieraGroup(RivieraObject obj)
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
                while (obj.Parent > 0)
                    parentObj = parentObj.GetParent(db);
                objs = GetRivieraGroup(parentObj);
            }
            return objs;
        }
        /// <summary>
        /// Gets the Riviera objects that belongs to bordeo group.
        /// </summary>
        /// <param name="obj">The riviera object.</param>
        /// <returns>The list of riviera object</returns>
        private static List<RivieraObject> GetRivieraFront(RivieraObject obj)
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
        /// <param name="obj">The riviera object.</param>
        /// <returns>The list of riviera object</returns>
        private static List<RivieraObject> GetRivieraBack(RivieraObject obj)
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
            return objs;
        }
    }
}
