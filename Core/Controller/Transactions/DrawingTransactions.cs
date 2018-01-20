using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using DaSoft.Riviera.Modulador.Core.Model;
using Nameless.Libraries.HoukagoTeaTime.Mio.Utils;
using Nameless.Libraries.HoukagoTeaTime.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Core.Controller.Transactions
{
    /// <summary>
    /// Defines the drawing transactions
    /// </summary>
    public static partial class AutoCADTransactions
    {
        /// <summary>
        /// Picks an arrow direction.
        /// After the arrow is selected, the other arrows are erased.
        /// </summary>
        /// <param name="sowEntity">The sow entity.</param>
        /// <param name="arrowsIds">The arrows ids.</param>
        /// <returns>The picked arrow direction</returns>
        public static ArrowDirection PickArrowDirection(ISowable sowEntity, ObjectIdCollection arrowsIds)
        {
            return new TransactionWrapper<Object, ArrowDirection>(AutoCADTransactions.PickAndEraseArrows).Run(sowEntity, arrowsIds);
        }
        /// <summary>
        /// Picks an arrow and then erase the showed arrows.
        /// </summary>
        /// <param name="doc">The active document.</param>
        /// <param name="tr">The active transaction.</param>
        /// <param name="input">The transaction input, needs a <see cref="ISowable"/> and an <see cref="ObjectIdCollection"/>.</param>
        /// <returns>The picked arrow direction</returns>
        private static ArrowDirection PickAndEraseArrows(Document doc, Transaction tr, params Object[] input)
        {
            var sowEntity = input[0] as ISowable;
            var ids = input[1] as ObjectIdCollection;
            ArrowDirection dir = sowEntity.PickDirection(tr);
            ids.Erase(tr);
            return dir;
        }
    }
}
