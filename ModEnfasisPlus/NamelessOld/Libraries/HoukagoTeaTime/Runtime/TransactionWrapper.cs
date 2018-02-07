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
    /// <typeparam name="output">The input type value returned from the transaction moment.</typeparam>
    public class TransactionWrapper<input, output> : NamelessObject
    {
        #region Propiedades
        /// <summary>
        /// The delegate of a transaction action
        /// </summary>
        /// <param name="doc">The AutoCAD active document.</param>
        /// <param name="tr">The transaction manager object.</param>
        /// <param name="trParameters">The params used in the transaction</param>
        /// <returns>The typed valued object in the delegate.</returns>
        public delegate output ActionTransaction(Document doc, Transaction tr, params input[] trParameters);
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
        public TransactionWrapper(ActionTransaction action)
        {
            this.trAction = action;
        }
        #endregion
        /// <summary>
        /// Excecute the current transaction action.
        /// </summary>
        /// <param name="trParameters">The parameters involved in the transaction</param>
        /// <returns>The typed valued returned in the transaction.</returns>
        public output Run(params input[] trParameters)
        {
            Transaction tr;
            Document currentDoc = AcadApp.DocumentManager.MdiActiveDocument;
            output returnValue = default(output);
            using (tr = currentDoc.Database.TransactionManager.StartTransaction())
            {
                try
                {
                    returnValue = this.trAction(currentDoc, tr, trParameters);
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
            return returnValue;
        }
    }
}
