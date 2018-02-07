using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.Yggdrasil.Frau.Threading
{
    public abstract class FrauThread
    {
        /// <summary>
        /// True if the thread worker is busy
        /// </summary>
        public Boolean IsBusy { get { return this.Bg.IsBusy; } }
        /// <summary>
        /// The thread input data
        /// </summary>
        public Object InputData;
        /// <summary>
        /// The process id
        /// </summary>
        public int PID;
        /// <summary>
        /// The process id
        /// </summary>
        public Object Result;
        /// <summary>
        /// True if the thread is allow to report progress
        /// </summary>
        public Boolean ReportProgress { get { return Bg.WorkerReportsProgress; } set { Bg.WorkerReportsProgress = value; } }
        /// <summary>
        /// True if the thread is allow support cancellation
        /// </summary>
        public Boolean SupportCancellation;
        /// <summary>
        /// Defines the measurable goal the thread has to reach
        /// </summary>
        public Double Goal;
        /// <summary>
        /// Defines the current task progress
        /// </summary>
        public Double Current;
        /// <summary>
        /// Gets the current thread progress percent
        /// </summary>
        public Double Progress { get { return this.Current / this.Goal; } }
        /// <summary>
        /// Gets the current thread progress percent in String format
        /// </summary>
        public String Percent { get { return String.Format("{0:P2}", this.Progress); } }
        /// <summary>
        /// The task to report progress
        /// </summary>
        public abstract void ReportProgressTask(object bgWorker, ProgressChangedEventArgs e);
        /// <summary>
        /// The task to do the work action
        /// </summary>
        public abstract void DoWorkAction(object bgWorker, DoWorkEventArgs e);
        /// <summary>
        /// The task to run once the work action has finished
        /// </summary>
        public abstract void WorkTask_Completed(object sender, RunWorkerCompletedEventArgs e);
        /// <summary>
        /// The background worker
        /// </summary>
        BackgroundWorker Bg;
        /// <summary>
        /// Creates a new Frau thread setting manager
        /// </summary>
        /// <param name="frauThreadSettings">The template frau thread settings</param>
        public FrauThread()
        {
            Bg = new BackgroundWorker();
            Bg.DoWork += this.DoWorkAction;
            Bg.ProgressChanged += this.ReportProgressTask;
            Bg.WorkerSupportsCancellation = this.SupportCancellation;
            Bg.RunWorkerCompleted += this.WorkTask_Completed;
        }
        /// Run the task and repeat it self after finished
        /// </summary>
        /// <param name="restart">Restart flag</param>
        public void Run()
        {
            this.Bg.RunWorkerAsync(this.InputData);
        }
        /// <summary>
        /// Stops the background worker
        /// </summary>
        public void Stop()
        {
            this.Bg.CancelAsync();
        }
        /// <summary>
        /// Get the pool thread template
        /// </summary>
        /// <returns>The pool thread template</returns>
        public override string ToString()
        {
            return String.Format("PID:{0}, Progress:{1}", this.PID, this.Percent);
        }
    }
}
