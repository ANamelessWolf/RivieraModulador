﻿using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using NamelessOld.Libraries.HoukagoTeaTime.Assets;
using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using AcadExc = Autodesk.AutoCAD.Runtime.Exception;
namespace NamelessOld.Libraries.HoukagoTeaTime.Runtime
{
    /// <summary>
    /// Creates a transaction wrapper to access or modify the AutoCAD
    /// document Object.
    /// </summary>
    /// <typeparam name="output">The output type value returned from the transaction moment.</typeparam>
    public class BlankTransactionWrapper<output> : NamelessObject
    {
        #region Propiedades
        /// <summary>
        /// The delegate of a transaction action
        /// </summary>
        /// <param name="doc">The AutoCAD active document.</param>
        /// <param name="tr">The transaction manager object.</param>
        /// <returns>The typed valued object in the delegate.</returns>
        public delegate output ActionTransaction(Document doc, Transaction tr);
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
        public BlankTransactionWrapper(ActionTransaction action)
        {
            this.trAction = action;
        }
        #endregion
        /// <summary>
        /// Excecute the current transaction action.
        /// </summary>
        /// <returns>The typed valued returned in the transaction.</returns>
        public output Run()
        {
            output returnValue = default(output);
            Document currentDoc = AcadApp.DocumentManager.MdiActiveDocument;
            using (Transaction tr = currentDoc.Database.TransactionManager.StartTransaction())
            {
                try
                {
                    returnValue = this.trAction(currentDoc, tr);
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