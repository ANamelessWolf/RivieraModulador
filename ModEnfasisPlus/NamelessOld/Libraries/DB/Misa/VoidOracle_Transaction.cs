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
    /// Creates an Oracle transaction
    /// </summary>
    /// <typeparam name="Input">The type of the parameters received by the transaction.</typeparam>
    public class VoidOracle_Transaction<Input>
    {
        #region Variables
        /// <summary>
        /// A main thread transaction
        /// </summary>
        /// <param name="conn">Oracle connector</param>
        /// <param name="trParameters">Transaction parameters</param>
        public delegate void ActionTransaction(Oracle_Connector conn, params Input[] trParameters);
        /// <summary>
        /// A background worker transaction
        /// </summary>
        /// <param name="conn">Oracle connector</param>
        /// <param name="bgWorker">Background worker asigned to the transaction</param>
        /// <param name="trParameters">Transaction parameters</param>
        public delegate void ActionBgTransaction(Oracle_Connector conn, ref BackgroundWorker bgWorker, Input trParameters);
        /// <summary>
        /// Main thread delegate
        /// </summary>
        ActionTransaction trAction;
        /// <summary>
        /// Background worker delegate
        /// </summary>
        ActionBgTransaction trBgAction;
        /// <summary>
        /// Oracle connection data
        /// </summary>
        ConnectionData Data;
        /// <summary>
        /// The Oracle task
        /// </summary>
        OracleTask _Task;
        /// <summary>
        /// Connection String
        /// </summary>
        String ConnectionString;
        #endregion
        #region Constructor
        /// <summary>
        /// Creates an Oracle transaction
        /// Main thread transaction
        /// </summary>
        /// <param name="connData">Oracle connection data</param>
        /// <param name="action">Oracle transaction action</param>
        public VoidOracle_Transaction(ConnectionData connData, ActionTransaction action)
        {
            this.Data = connData;
            this.trAction = action;
        }
        /// <summary>
        /// Creates an Oracle transaction
        /// Background transaction
        /// </summary>
        /// <param name="connData">Oracle connection data</param>
        /// <param name="action">Oracle background transaction action</param>
        public VoidOracle_Transaction(ConnectionData connData, ActionBgTransaction action)
        {
            this.Data = connData;
            this.trBgAction = action;
        }
        /// <summary>
        /// Creates an Oracle transaction
        /// Main thread transaction
        /// </summary>
        /// <param name="connString">Oracle connection string</param>
        /// <param name="action">Oracle transaction action</param>
        public VoidOracle_Transaction(String connString, ActionTransaction action)
        {
            this.ConnectionString = connString;
            this.trAction = action;
        }
        /// <summary>
        /// Creates an Oracle transaction
        /// Background transaction
        /// </summary>
        /// <param name="connString">Oracle connection string</param>
        /// <param name="action">Oracle background transaction action</param>
        public VoidOracle_Transaction(String connString, ActionBgTransaction action)
        {
            this.ConnectionString = connString;
            this.trBgAction = action;
        }
        #endregion
        #region Action
        /// <summary>
        /// Run the transaction as main thread transaction
        /// </summary>
        /// <param name="trParameters">Transaction parameters</param>
        /// <returns>Transaction result</returns>
        public void Run(params Input[] trParameters)
        {
            if (trAction == null)
                throw new ShinigamiException(String.Format(ERR_DEF_MAIN_TR, MAIN_TR));
            try
            {
                if (this.Data != null)
                {
                    using (Oracle_Connector conn = new Oracle_Connector(this.Data))
                    {
                        try
                        {
                            this.trAction(conn, trParameters);
                        }
                        catch (System.Exception exc)
                        {
                            String err = String.Format("{0}: {1}", exc.Message, exc.InnerException != null ? exc.InnerException.Message : String.Empty);
                            throw new ShinigamiException(err, exc);
                        }
                    }
                }
                else if (this.ConnectionString != null)
                {
                    using (Oracle_Connector conn = new Oracle_Connector(this.ConnectionString))
                    {
                        try
                        {
                            this.trAction(conn, trParameters);
                        }
                        catch (System.Exception exc)
                        {
                            String err = String.Format("{0}: {1}", exc.Message, exc.InnerException != null ? exc.InnerException.Message : String.Empty);
                            throw new ShinigamiException(err, exc);
                        }
                    }
                }
            }
            catch (ShinigamiException exc)
            {
                throw new ShinigamiException(String.Format("{0}: {1}", STRINGS.ERR_TRANSACTION, exc.Message), exc);
            }
        }
        /// <summary>
        /// Run the transaction as an async transaction
        /// </summary>
        /// <param name="trParameters">Transaction parameters</param>
        /// <returns>Transaction result</returns>
        public async void RunAsync(params Input[] trParameters)
        {
            if (trAction == null)
                throw new ShinigamiException(String.Format(ERR_DEF_MAIN_TR, ASYNC_TR));
            try
            {
                await Task.Run(() =>
                {
                    if (this.Data != null)
                    {
                        using (Oracle_Connector conn = new Oracle_Connector(this.Data))
                        {
                            try
                            {
                                this.trAction(conn, trParameters);
                            }
                            catch (System.Exception exc)
                            {
                                throw new ShinigamiException(exc.Message, exc);
                            }
                        }
                    }
                    else if (this.ConnectionString != null)
                    {
                        using (Oracle_Connector conn = new Oracle_Connector(this.ConnectionString))
                        {
                            try
                            {
                                this.trAction(conn, trParameters);
                            }
                            catch (System.Exception exc)
                            {
                                throw new ShinigamiException(exc.Message, exc);
                            }
                        }
                    }
                });
            }
            catch (ShinigamiException exc)
            {
                throw new ShinigamiException(String.Format("{0}: {1}", ERR_TRANSACTION, exc.Message), exc);
            }
        }
        /// <summary>
        /// Run the transaction as a background worker transaction, the transaction result is
        /// returned on the OracleTask TaskIsFinished event.
        /// </summary>
        /// <param name="trParameters">Transaction parameters</param>
        public void RunBGWorker(OracleTask task, params Input[] trParameters)
        {
            if (trBgAction == null)
                throw new ShinigamiException(ERR_DEF_BG_TR);
            this._Task = task;
            OracleTask.Current = 0;
            this._Task.BgWorker.DoWork += BgWorker_DoWork;
            if (this.Data != null)
                this._Task.BgWorker.RunWorkerAsync(new Object[] { this.Data, trParameters });
            else
                this._Task.BgWorker.RunWorkerAsync(new Object[] { this.ConnectionString, trParameters });
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
                Input input = (Input)args[1];
                if (data != null)
                {
                    using (Oracle_Connector conn = new Oracle_Connector(data))
                    {
                        try
                        {
                            e.Result = null;
                            this.trBgAction(conn, ref this._Task.BgWorker, input);
                        }
                        catch (Exception exc)
                        {
                            conn.Dispose();
                            throw new ShinigamiException(String.Format("{0}: {1}", ERR_TRANSACTION, exc.Message));
                        }
                    }
                }
                else if (args[0] is String)
                {
                    String stringData = args[0] as String;
                    using (Oracle_Connector conn = new Oracle_Connector(stringData))
                    {
                        try
                        {
                            e.Result = null;
                            this.trBgAction(conn, ref this._Task.BgWorker, input);
                        }
                        catch (Exception exc)
                        {
                            conn.Dispose();
                            throw new ShinigamiException(String.Format("{0}: {1}", ERR_TRANSACTION, exc.Message));
                        }
                    }
                }
                else
                    e.Result = null;
            }
            catch (Exception exc)
            {
                e.Result = new ShinigamiException(String.Format("{0}: {1}", ERR_TRANSACTION, exc.Message));
            }
        }
        #endregion
    }
}