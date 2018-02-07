using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.Yggdrasil.Frau.Threading
{
    public class PoolThread : NamelessObject, ICollection<FrauThread>
    {
        const int REFRESH_TIME_MS = 100;
        /// <summary>
        /// The Action once the Task is finished
        /// </summary>
        /// <param name="input">The input is the result of all threads</param>
        public delegate Object TaskFinished(Object[] input);
        public TaskFinished TaskIsFinished;
        /// <summary>
        /// The Action while the bgIsWorking
        /// </summary>
        /// <param name="threads">The collection of working threads</param>
        public delegate void ReportTaskProgress(IEnumerable<FrauThread> threads);
        public ReportTaskProgress ReportingTaskProgress;
        /// <summary>
        /// The pool Collection
        /// </summary>
        Dictionary<int, FrauThread> Pool;
        /// <summary>
        /// Get the last PID
        /// </summary>
        int LastPID;
        /// <summary>
        /// Get the current Progress
        /// </summary>
        double Progress;
        /// <summary>
        /// Gets the current thread progress percent in String format
        /// </summary>
        public String Percent { get { return String.Format("{0:P2}", this.Progress); } }
        /// <summary>
        /// True if the all tasks are ready
        /// </summary>
        public Boolean TaskReady
        {
            get
            {
                Boolean flag = true;
                foreach (int pid in Pool.Keys)
                    //flag = flag && (!this.Pool[pid].IsBusy && this.Pool[pid].Result != null);
                    flag = flag && (this.Pool[pid].Result != null);
                return flag;
            }
        }
        /// <summary>
        /// Creates a new pool thread task
        /// </summary>
        public PoolThread()
        {
            this.Pool = new Dictionary<int, FrauThread>();
            this.LastPID = 0;
        }
        /// <summary>
        /// Adds a new pool to the thread
        /// </summary>
        /// <param name="thread">The thread to be added to the pool</param>
        public void Add(FrauThread thread)
        {
            this.LastPID++;
            thread.PID = this.LastPID;
            this.Pool.Add(thread.PID, thread);
        }
        /// <summary>
        /// True if the pool has the thread 
        /// The thread is search by its Id
        /// </summary>
        /// <param name="PID">The thread id</param>
        /// <returns>True if the pid is contained</returns>
        public bool HasPID(int PID)
        {
            return this.Pool.ContainsKey(PID);
        }
        /// <summary>
        /// Runs the current task
        /// </summary>
        /// <returns>Get the task result</returns>
        public Object Run()
        {
            //1: Init process ids
            foreach (int PID in this.Pool.Keys)
                this.Pool[PID].Run();
            //2: Process the data
            this.ProcessData();
            //3: Get the task result
            return GetResult();
        }
        /// <summary>
        /// Gets the Task Result
        /// </summary>
        /// <returns>The Result data</returns>
        private Object GetResult()
        {
            if (TaskIsFinished != null)
            {
                Object[] result = new Object[this.Count];
                int index = 0;
                foreach (int PID in this.Pool.Keys)
                {
                    result[index] = this.Pool[PID].Result;
                    index++;
                }
                return this.TaskIsFinished(result);
            }
            else
                return new Object();
        }
        /// <summary>
        /// Refresh the current data;
        /// </summary>
        private void ProcessData()
        {
            TimeSpan t = new TimeSpan(0, 0, 0, 0, REFRESH_TIME_MS);
            DateTime time = DateTime.Now;
            while (!this.TaskReady)
            {
                if ((DateTime.Now - time).Milliseconds >= t.Milliseconds)
                {
                    this.ReportProgress();
                    time = DateTime.Now;
                }
            }
            this.ReportProgress();
        }
        /// <summary>
        /// Reportes the current progress
        /// </summary>
        private void ReportProgress()
        {
            this.Progress = 0;
            foreach (int PID in this.Pool.Keys)
                this.Progress += this.Pool[PID].Progress / (double)this.Count;
          
            if (this.ReportingTaskProgress != null)
                this.ReportingTaskProgress(this.Pool.Values);
        }

        /// <summary>
        /// True if the pool contains a thread
        /// </summary>
        /// <param name="thread">The thread to search</param>
        /// <returns>True if the thread is contained</returns>
        public bool Contains(FrauThread thread)
        {
            return this.Pool.ContainsValue(thread);
        }
        /// <summary>
        /// Gets the list of PIDs
        /// </summary>
        public ICollection<int> PIDs
        {
            get { return this.Pool.Keys; }
        }
        /// <summary>
        /// Gets the list of Threads
        /// </summary>
        public ICollection<FrauThread> Threads
        {
            get { return this.Pool.Values; }
        }
        /// <summary>
        /// Get or Sets a thread by its PID
        /// </summary>
        /// <param name="PID">Th thread PID</param>
        /// <returns>The thread</returns>
        public FrauThread this[int PID]
        {
            get
            {
                return this.Pool[PID];
            }
            set
            {
                this.Pool[PID] = value;
            }
        }
        /// <summary>
        /// Clear the pool thread
        /// </summary>
        public void Clear()
        {
            this.Pool.Clear();
        }
        /// <summary>
        /// Gets the pool thread size
        /// </summary>
        public int Count
        {
            get { return this.Pool.Count; }
        }
        /// <summary>
        /// Copy an array of threads
        /// </summary>
        /// <param name="arrayThread">Thea thread array to copy</param>
        /// <param name="index">The index to start copying</param>
        public void CopyTo(FrauThread[] arrayThread, int index)
        {
            this.Pool.Values.CopyTo(arrayThread, index);
        }
        /// <summary>
        /// Read only
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }
        /// <summary>
        /// Remove a thread 
        /// </summary>
        /// <param name="thread">The thread</param>
        /// <returns>True if the thread is removed</returns>
        public bool Remove(FrauThread thread)
        {
            return this.Pool.Remove(thread.PID);
        }
        /// <summary>
        /// Get the enumarator
        /// </summary>
        /// <returns>The enurator</returns>
        public IEnumerator<FrauThread> GetEnumerator()
        {
            return this.Pool.Values.GetEnumerator();
        }
        /// <summary>
        /// Get the enumarator
        /// </summary>
        /// <returns>The enurator</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.Pool.Values.GetEnumerator();
        }
        /// <summary>
        /// Get the pool thread string
        /// </summary>
        /// <returns>The pool thread string</returns>
        public override string ToString()
        {
            return String.Format("Threads:{0}, Progress:{1}", this.Count, this.Percent);
        }

    }
}
