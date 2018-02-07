using NamelessOld.Libraries.DB.Mikasa;
using NamelessOld.Libraries.DB.Mikasa.Model;
using NamelessOld.Libraries.DB.Misa.Exceptions;
using NamelessOld.Libraries.DB.Misa.Model;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using static NamelessOld.Libraries.DB.Mikasa.STRINGS;
namespace NamelessOld.Libraries.DB.Misa
{
    /// <summary>
    /// Creates a Riviera transaction to Riviera DB.
    /// </summary>
    /// <typeparam name="Output">The type value returned from the transaction.</typeparam>
    public class BlankOracle_Transaction<Output>
    {
        #region Variables
        /// <summary>
        /// The delegate of a transaction action
        /// </summary>
        /// <param name="conn">The connection data</param>
        /// <returns>The typed valued object in the delegate.</returns>
        public delegate Output ActionTransaction(Oracle_Connector conn);
        /// <summary>
        /// The delegate of a transaction with a Background transaction
        /// </summary>
        /// <param name="conn">The connection data</param>
        /// <param name="bgWorker">The background worker assign to this transaction.</param>
        /// <returns>The typed valued object in the delegate.</returns>
        public delegate Output ActionBgTransaction(Oracle_Connector conn, ref BackgroundWorker bgWorker);
        /// <summary>
        /// The action for the current transaction.
        /// </summary>
        ActionTransaction trAction;
        /// <summary>
        /// The action for the current transaction.
        /// </summary>
        ActionBgTransaction trBgAction;
        /// <summary>
        /// Sql connection data
        /// </summary>
        ConnectionData Data;
        /// <summary>
        /// The Oracle task
        /// </summary>
        OracleTask Task;
        #endregion
        #region Constructor
        /// <summary>
        /// Creates an Sql transaction
        /// </summary>
        /// <param name="connData">Sql connection data</param>
        /// <param name="action">The action for the current transaction.</param>
        public BlankOracle_Transaction(ConnectionData connData, ActionTransaction action)
        {
            this.Data = connData;
            this.trAction = action;
        }
        /// <summary>
        /// Creates an Sql transaction
        /// Background mode
        /// </summary>
        /// <param name="connData">Sql connection data</param>
        /// <param name="action">The background transaction action.</param>
        public BlankOracle_Transaction(ConnectionData connData, ActionBgTransaction action)
        {
            this.Data = connData;
            this.trBgAction = action;
        }
        #endregion
        #region Action
        /// <summary>
        /// Excecute the current action.
        /// </summary>
        /// <returns>The typed valued returned in the transaction.</returns>
        public Output Run()
        {
            Output returnValue = default(Output);
            try
            {
                using (Oracle_Connector conn = new Oracle_Connector(this.Data))
                {
                    try
                    {
                        returnValue = this.trAction(conn);
                    }
                    catch (System.Exception exc)
                    {
                        throw new ShinigamiException(exc.Message, exc);
                    }
                }
            }
            catch (ShinigamiException exc)
            {
                throw new ShinigamiException(String.Format("{0}: {1}", StringConstants.TRANSACTION_ERROR, exc.Message), exc);
            }
            return returnValue;
        }
        /// <summary>
        /// Excecute the current action.
        /// </summary>
        /// <returns>The typed valued returned in the transaction.</returns>
        public void RunBGWorker(OracleTask task)
        {
            this.Task = task;
            OracleTask.Current = 0;
            this.Task.BgWorker.DoWork += BgWorker_DoWork;
            this.Task.BgWorker.RunWorkerAsync(new Object[] { this.Data });
        }
        /// <summary>
        /// In case the task is from a background process
        /// </summary>
        /// <param name="sender">The background sender</param>
        /// <param name="e">The background args</param>
        private void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Object[] args = e.Argument as Object[];
                ConnectionData data = args[0] as ConnectionData;
                using (Oracle_Connector conn = new Oracle_Connector(data))
                {
                    try
                    {
                        e.Result = this.trBgAction(conn, ref this.Task.BgWorker);
                    }
                    catch (Exception exc)
                    {
                        conn.Dispose();
                        throw new ShinigamiException(String.Format("{0}: {1}", StringConstants.TRANSACTION_ERROR, exc.Message));
                    }
                }
            }
            catch (Exception exc)
            {
                e.Result = new ShinigamiException(String.Format("{0}: {1}", StringConstants.TRANSACTION_ERROR, exc.Message));
            }
        }
        #endregion
    }
}