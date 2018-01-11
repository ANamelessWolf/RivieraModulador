using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using DaSoft.Riviera.Modulador.Core.Controller;
using DaSoft.Riviera.Modulador.Core.Model;
using Nameless.Libraries.HoukagoTeaTime.Ritsu.Utils;
using Nameless.Libraries.HoukagoTeaTime.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BordeoGeometryUtils = DaSoft.Riviera.Modulador.Core.Controller.GeometryUtils;
namespace DaSoft.Riviera.Modulador.Bordeo.Controller.Transactions
{
    /// <summary>
    /// Defines the bordeo drawing transactions
    /// </summary>
    public static partial class BordeoAutoCADTransactions
    {
        /// <summary>
        /// Draws the arrows.
        /// </summary>
        /// <param name="sowEntity">The sow entity.</param>
        /// <param name="rivObject">The riviera object.</param>
        /// <param name="atStartPoint">if set to <c>true</c> draw arrows [at start point].</param>
        /// <param name="atEndPoint">if set to <c>true</c> draw arrows [at end point].</param>
        /// <returns></returns>
        public static ObjectIdCollection DrawArrows(ISowable sowEntity, RivieraObject rivObject, Boolean atStartPoint = false, Boolean atEndPoint = false)
        {
            return new TransactionWrapper<Object, ObjectIdCollection>(BordeoAutoCADTransactions.DrawArrows).Run(sowEntity, atStartPoint, atEndPoint, rivObject);
        }
        /// <summary>
        /// Defines the transaction that draws the arrows of a Bordeo Entity
        /// </summary>
        /// <param name="doc">The active document.</param>
        /// <param name="tr">The active transaction.</param>
        /// <param name="input">The transaction input, needs a <see cref="ISowable"/>,a <see cref="RivieraObject"/>, start point flag as <see cref="Boolean"/> and
        /// end point flag as <see cref="Boolean"/>.</param>
        /// <returns>The arrows drew ids</returns>
        private static ObjectIdCollection DrawArrows(Document doc, Transaction tr, params Object[] input)
        {
            ISowable sowEnt = (ISowable)input[0];
            Boolean atStart = (Boolean)input[1],
                    atEnd = (Boolean)input[2];
            RivieraObject obj = (RivieraObject)input[3];
            ObjectIdCollection ids = new ObjectIdCollection();
            if (atStart)
                foreach (ObjectId id in sowEnt.DrawArrows(BordeoGeometryUtils.IsBack, obj.Start.ToPoint3d(), obj.Angle, tr))
                    ids.Add(id);
            if (atEnd)
                foreach (ObjectId id in sowEnt.DrawArrows(BordeoGeometryUtils.IsFront, obj.End.ToPoint3d(), obj.Angle, tr))
                    ids.Add(id);
            return ids;
        }
    }
}
