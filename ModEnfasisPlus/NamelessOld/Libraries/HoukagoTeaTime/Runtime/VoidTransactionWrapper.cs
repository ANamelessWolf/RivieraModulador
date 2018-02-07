using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using NamelessOld.Libraries.HoukagoTeaTime.Assets;
using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using AcadExc = Autodesk.AutoCAD.Runtime.Exception;
namespace NamelessOld.Libraries.HoukagoTeaTime.Runtime
{
    /// <summary>
    /// Creates a transaction wrapper to access or modify the AutoCAD
    /// document Object.
    /// </summary>
    /// <typeparam name="input">The input type of the parameters received by the transaction moment.</typeparam>
    public class VoidTransactionWrapper<input> : NamelessObject
    {
        #region Propiedades
        /// <summary>
        /// The delegate of a transaction action
        /// </summary>
        /// <param name="doc">The AutoCAD active document.</param>
        /// <param name="tr">The transaction manager object.</param>
        /// <param name="trParameters">The params used in the transaction</param>
        public delegate void ActionTransaction(Document doc, Transaction tr, params input[] trParameters);
        /// <summary>
        /// The action for the current transaction.
        /// </summary>
        ActionTransaction trAction;
        #endregion
        #region Constructor
        /// <summary>
        /// Creates a new transaction
        /// </summary>
        /// <param name="action">The action for the current transaction moment.</param>
        public VoidTransactionWrapper(ActionTransaction action)
        {
            this.trAction = action;
        }
        #endregion
        /// <summary>
        /// Excecute the current transaction action.
        /// </summary>
        /// <param name="trParameters">The parameters involved in the transaction</param>
        /// <returns>The typed valued returned in the transaction.</returns>
        public void Run(params input[] trParameters)
        {
            Document currentDoc = AcadApp.DocumentManager.MdiActiveDocument;
            using (Transaction tr = currentDoc.Database.TransactionManager.StartTransaction())
            {
                try
                {
                    this.trAction(currentDoc, tr, trParameters);
                    tr.Commit();
                }
                catch (RomioException exc)
                {
                    tr.Abort();
                    throw new RomioException(String.Format("{0}: {1}", Errors.Transaction, exc.Message), exc);
                }
                catch (AcadExc exc)
                {
                    tr.Abort();
                    throw new RomioException(String.Format("{0}: {1}", Errors.Transaction, exc.Message), exc);
                }
                catch (System.Exception exc)
                {
                    tr.Abort();
                    throw new RomioException(String.Format("{0}: {1}", Errors.Transaction, exc.Message), exc);
                }
            }
        }
    }
}
