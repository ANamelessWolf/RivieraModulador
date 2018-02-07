using NamelessOld.Libraries.DB.Mikasa;
using NamelessOld.Libraries.DB.Mikasa.Model;
using NamelessOld.Libraries.DB.Tessa.Exceptions;
using NamelessOld.Libraries.DB.Tessa.Model;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using static NamelessOld.Libraries.DB.Mikasa.STRINGS;

namespace NamelessOld.Libraries.DB.Tessa
{
    /// <summary>
    /// Creates a Riviera transaction to Riviera DB.
    /// </summary>
    /// <typeparam name="Output">The type value returned from the transaction.</typeparam>
    public class BlankAccess_Transaction<Output>
    {
        #region Variables
        /// <summary>
        /// A main thread transaction
        /// </summary>
        /// <param name="conn">Microsoft Access connector</param>
        /// <returns>Transaction result</returns>
        public delegate Output ActionTransaction(Access_Connector conn);
        /// <summary>
        /// A background worker transaction
        /// </summary>
        /// <param name="conn">Microsoft Access connector</param>
        /// <param name="bgWorker">Background worker asigned to the transaction</param>
        /// <returns>Transaction result</returns>
        public delegate Output ActionBgTransaction(Access_Connector conn, ref BackgroundWorker bgWorker);
        /// <summary>
        /// Main thread delegate
        /// </summary>
        ActionTransaction trAction;
        /// <summary>
        /// Background worker delegate
        /// </summary>
        ActionBgTransaction trBgAction;
        /// <summary>
        /// Microsoft Access connection data
        /// </summary>
        ConnectionData Data;
        /// <summary>
        /// The Microsoft Access task
        /// </summary>
        AccessTask _Task;
        /// <summary>
        /// Connection String
        /// </summary>
        String ConnectionString;
        #endregion
        #region Constructor
        /// <summary>
        /// Creates an Microsoft Access transaction
        /// Main thread transaction
        /// </summary>
        /// <param name="connData">Microsoft Access connection data</param>
        /// <param name="action">Microsoft Access transaction action</param>
        public BlankAccess_Transaction(ConnectionData connData, ActionTransaction action)
        {
            this.Data = connData;
            this.trAction = action;
        }
        /// <summary>
        /// Creates an Microsoft Access transaction
        /// Background transaction
        /// </summary>
        /// <param name="connData">Microsoft Access connection data</param>
        /// <param name="action">Microsoft Access background transaction action</param>
        public BlankAccess_Transaction(ConnectionData connData, ActionBgTransaction action)
        {
            this.Data = connData;
            this.trBgAction = action;
        }
        /// <summary>
        /// Creates an Microsoft Access transaction
        /// Main thread transaction
        /// </summary>
        /// <param name="connString">Microsoft Access connection string</param>
        /// <param name="action">Microsoft Access transaction action</param>
        public BlankAccess_Transaction(String connString, ActionTransaction action)
        {
            this.ConnectionString = connString;
            this.trAction = action;
        }
        /// <summary>
        /// Creates an Microsoft Access transaction
        /// Background transaction
        /// </summary>
        /// <param name="connString">Microsoft Access connection string</param>
        /// <param name="action">Microsoft Access background transaction action</param>
        public BlankAccess_Transaction(String connString, ActionBgTransaction action)
        {
            this.ConnectionString = connString;
            this.trBgAction = action;
        }
        #endregion
        #region Action
        /// <summary>
        /// Run the transaction as main thread transaction
        /// </summary>
        /// <returns>Transaction result</returns>
        public Output Run()
        {
            Output returnValue = default(Output);
            if (trAction == null)
                throw new MithrilException(String.Format(ERR_DEF_MAIN_TR, MAIN_TR));
            try
            {
                if (this.Data != null)
                {
                    using (Access_Connector conn = new Access_Connector(this.Data))
                    {
                        try
                        {
                            returnValue = this.trAction(conn);
                        }
                        catch (System.Exception exc)
                        {
                            throw new MithrilException(exc.Message, exc);
                        }
                    }
                }
                else if (this.ConnectionString != null)
                {
                    using (Access_Connector conn = new Access_Connector(this.ConnectionString))
                    {
                        try
                        {
                            returnValue = this.trAction(conn);
                        }
                        catch (System.Exception exc)
                        {
                            throw new MithrilException(exc.Message, exc);
                        }
                    }
                }
                else
                    return default(Output);
            }
            catch (MithrilException exc)
            {
                throw new MithrilException(String.Format("{0}: {1}", STRINGS.ERR_TRANSACTION, exc.Message), exc);
            }
            return returnValue;
        }
        /// <summary>
        /// Run the transaction as an async transaction
        /// </summary>
        /// <returns>Transaction result</returns>
        public async Task<Output> RunAsync()
        {
            Output returnValue = default(Output);
            if (trAction == null)
                throw new MithrilException(String.Format(ERR_DEF_MAIN_TR, ASYNC_TR));
            try
            {
                await Task<Output>.Run(() =>
                {
                    if (this.Data != null)
                    {
                        using (Access_Connector conn = new Access_Connector(this.Data))
                        {
                            try
                            {
                                returnValue = this.trAction(conn);
                            }
                            catch (System.Exception exc)
                            {
                                throw new MithrilException(exc.Message, exc);
                            }
                        }
                    }
                    else if (this.ConnectionString != null)
                    {
                        using (Access_Connector conn = new Access_Connector(this.ConnectionString))
                        {
                            try
                            {
                                returnValue = this.trAction(conn);
                            }
                            catch (System.Exception exc)
                            {
                                throw new MithrilException(exc.Message, exc);
                            }
                        }
                    }
                    else
                        returnValue = default(Output);
                });
            }
            catch (MithrilException exc)
            {
                throw new MithrilException(String.Format("{0}: {1}", ERR_TRANSACTION, exc.Message), exc);
            }
            return returnValue;
        }
        /// <summary>
        /// Run the transaction as a background worker transaction, the transaction result is
        /// returned on the AccessTask TaskIsFinished event.
        /// </summary>
        /// <param name="trParameters">Transaction parameters</param>
        public void RunBGWorker(AccessTask task)
        {
            if (trBgAction == null)
                throw new MithrilException(ERR_DEF_BG_TR);
            this._Task = task;
            AccessTask.Current = 0;
            this._Task.BgWorker.DoWork += BgWorker_DoWork;
            if (this.Data != null)
                this._Task.BgWorker.RunWorkerAsync(new Object[] { this.Data });
            else
                this._Task.BgWorker.RunWorkerAsync(new Object[] { this.ConnectionString });
        }
        /// <summary>
        /// Background worker transaction.
        /// If an error is found the result is the generated exception
        /// </summary>
        private void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Object[] args = e.Argument as Object[];
                ConnectionData data = args[0] as ConnectionData;
                if (data != null)
                {
                    using (Access_Connector conn = new Access_Connector(data))
                    {
                        try
                        {
                            e.Result = this.trBgAction(conn, ref this._Task.BgWorker);
                        }
                        catch (Exception exc)
                        {
                            conn.Dispose();
                            throw new MithrilException(String.Format("{0}: {1}", ERR_TRANSACTION, exc.Message, exc.InnerException != null ? exc.InnerException.Message:String.Empty));
                        }
                    }
                }
                else if (args[0] is String)
                {
                    String stringData = args[0] as String;
                    using (Access_Connector conn = new Access_Connector(stringData))
                    {
                        try
                        {
                            e.Result = this.trBgAction(conn, ref this._Task.BgWorker);
                        }
                        catch (Exception exc)
                        {
                            conn.Dispose();
                            throw new MithrilException(String.Format("{0}: {1}", ERR_TRANSACTION, exc.Message, exc.InnerException != null ? exc.InnerException.Message : String.Empty));
                        }
                    }
                }
                else
                    e.Result = null;

            }
            catch (Exception exc)
            {
                e.Result = new MithrilException(String.Format("{0}: {1}\n{2}", ERR_TRANSACTION, exc.Message, exc.InnerException != null ? exc.InnerException.Message : String.Empty));
            }
        }
        #endregion
    }
}
