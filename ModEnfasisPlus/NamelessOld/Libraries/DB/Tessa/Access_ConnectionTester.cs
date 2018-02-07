using NamelessOld.Libraries.DB.Mikasa.Model;
using NamelessOld.Libraries.DB.Tessa.Model;
using System;
using System.ComponentModel;

namespace NamelessOld.Libraries.DB.Tessa
{
    public class Access_ConnectionTester : DB_ConnectionTester
    {
        /// <summary>
        /// Connect with a background worker transaction
        /// </summary>
        /// <param name="connString">Connection string</param>
        public override void Connect(string connString)
        {
            BlankAccess_Transaction<Boolean> tr = new BlankAccess_Transaction<Boolean>(connString, TestConnection);
            this.Run(tr);
        }
        /// <summary>
        /// Connect with a background worker transaction
        /// </summary>
        /// <param name="data">Connection data</param>
        public override void Connect(ConnectionData data)
        {
            BlankAccess_Transaction<Boolean> tr = new BlankAccess_Transaction<Boolean>(data, TestConnection);
            this.Run(tr);
        }
        /// <summary>
        /// Test connection transaction
        /// </summary>
        /// <param name="conn">Database connector</param>
        /// <param name="bgWorker">background worker</param>
        /// <returns>True if the element is connected</returns>
        private bool TestConnection(Access_Connector conn, ref BackgroundWorker bgWorker)
        {
            return conn.IsConnected;
        }
        /// <summary>
        /// Run the Microsoft Access test transactrion
        /// </summary>
        /// <param name="tr">Microsoft Access transaction</param>
        private void Run(BlankAccess_Transaction<Boolean> tr)
        {
            AccessTask task = new AccessTask();
            task.TaskIsFinished += this.TaskIsFinished;
            tr.RunBGWorker(task);
        }
    }
}
