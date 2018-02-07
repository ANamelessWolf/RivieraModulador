using NamelessOld.Libraries.Yggdrasil.Lilith;
using System.ComponentModel;

namespace NamelessOld.Libraries.DB.Misa
{
    public class OracleTask : NamelessObject
    {
        /// <summary>
        /// The total to achive in the task transaction
        /// </summary>
        public static int Total;
        /// <summary>
        /// The current value of the transaction
        /// </summary>
        public static int Current;
        /// <summary>
        /// Once the task is finished
        /// </summary>
        public event RunWorkerCompletedEventHandler TaskIsFinished;
        /// <summary>
        /// While the task is working
        /// </summary>
        public event ProgressChangedEventHandler TaskReportChanged;
        /// <summary>
        /// The background worker
        /// </summary>
        public BackgroundWorker BgWorker;
        /// <summary>
        /// Creates a new transaction
        /// </summary>
        public OracleTask()
        {
            Current = 0;
            Total = 0;
            BgWorker = new BackgroundWorker();
            BgWorker.WorkerReportsProgress = true;
            BgWorker.ProgressChanged += bg_Changes;
            BgWorker.RunWorkerCompleted += bg_WorkIsComplete;
            BgWorker.WorkerReportsProgress = true;
        }
        /// <summary>
        /// The background action once the task is finished
        /// </summary>
        private void bg_WorkIsComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            if (TaskIsFinished != null)
                TaskIsFinished(sender, e);
        }
        /// <summary>
        /// The background action while the task is working
        /// </summary>
        private void bg_Changes(object sender, ProgressChangedEventArgs e)
        {
            if (TaskReportChanged != null)
                TaskReportChanged(sender, e);
        }

    }
}
