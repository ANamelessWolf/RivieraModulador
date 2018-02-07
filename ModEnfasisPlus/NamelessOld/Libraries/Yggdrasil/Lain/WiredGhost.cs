using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.IO;

namespace NamelessOld.Libraries.Yggdrasil.Lain
{
    /// <summary>
    /// This is the agent that creates a log file for an application, an reports the application
    /// progress. In Serial Experiments Lain, Lain Iwakura was the main character she lacks from emotions
    /// and when she is plugged to a computer she resembles a wired ghost, that can report anything in 
    /// the world.
    /// </summary>
    public class WiredGhost : NamelessObject
    {
        #region Properties
        /// <summary>
        /// Enables or disable the use of the log.
        /// </summary>
        public bool GhostMode { set; get; }
        #endregion
        #region Variables
        FileInfo Log;
        #endregion
        #region Constructor
        /// <summary>
        /// Creates a Wired Ghost to manage programs log
        /// </summary>
        /// <param name="enable">Enable the usage of the log. Its called the ghost mode</param>
        /// <param name="logFile">The path of the log file</param>
        public WiredGhost(FileInfo logFile, Boolean enable = false)
        {
            if (!File.Exists(logFile.FullName))
                File.Create(logFile.FullName).Close();
            this.GhostMode = enable;
            this.Log = logFile;
        }
        #endregion
        #region Action Methods
        /// <summary>
        /// Adds a new entry to the log.
        /// </summary>
        /// <param name="msg">The message of the entry</param>
        /// <param name="type">The type of the entry</param>
        /// <param name="functionName">The name of the function</param>
        /// <param name="specialCaption">Adds a special caption title</param>
        public void AppendEntry(string msg, Protocol type, string functionName, string specialCaption = "")
        {
            if (this.GhostMode)
            {
                string entry, date, entryType, caption;
                entryType = "[" + type.ToString() + "]";
                caption = "[" + specialCaption + "]";
                functionName = "[" + functionName + "]";
                date = "[" + String.Format("{0:d/M/yyyy HH:mm:ss}", DateTime.Now) + "]";
                if (specialCaption == "")
                    entry = string.Format("{0, -10}{1, -30}{2,-35}", entryType, date, functionName);
                else
                    entry = string.Format("{0, -10}{1, -30}{2,-20}{3,-35}", entryType, caption, date, functionName);
                entry += "Detalles: " + msg + "\n";
                try
                {
                    using (StreamWriter w = File.AppendText(this.Log.FullName))
                        SaveLog(entry, w);
                }
                catch (Exception)
                {

                }
            }
        }
        /// <summary>
        /// Adds a new entry to the log.
        /// </summary>
        /// <param name="msg">The message of the entry</param>
        /// <param name="type">The type of the entry</param>
        /// <param name="nameless">The nameless object</param>
        /// <param name="specialCaption">Adds a special caption title</param>
        public void AppendEntry(string msg, Protocol type, NamelessObject nameless)
        {
            if (this.GhostMode)
            {
                string entry, functionName, date, entryType, caption;
                entryType = "[" + type.ToString() + "]";
                caption = "[" + nameless.Class + "]";
                functionName = "[" + nameless.MethodName + "]";
                date = "[" + String.Format("{0:d/M/yyyy HH:mm:ss}", DateTime.Now) + "]";
                entry = string.Format("{0, -10}{1, -30}{2,-20}{3,-35}", entryType, caption, date, functionName);
                entry += "Detalles: " + msg + "\n";
                try
                {
                    using (StreamWriter w = File.AppendText(this.Log.FullName))
                        SaveLog(entry, w);
                }
                catch (Exception)
                {

                }
            }
        }
        /// <summary>
        /// Adds a new entry to the log.
        /// </summary>
        /// <param name="msg">The message of the entry</param>
        /// <param name="specialCaption">Adds a special caption title</param>
        public void AppendEntry(string msg)
        {
            if (this.GhostMode)
            {
                string entry, date;
                date = "[" + String.Format("{0:d/M/yyyy HH:mm:ss}", DateTime.Now) + "]";
                entry = string.Format("{0} {1}\n", date, msg);
                try
                {
                    using (StreamWriter w = File.AppendText(this.Log.FullName))
                        SaveLog(entry, w);
                }
                catch (Exception)
                {

                }
            }
        }
        #endregion
        #region Help Methods
        /// <summary>
        /// Write a new log entry
        /// </summary>
        /// <param name="logMessage">The message line to be append to the log.</param>
        /// <param name="w">The text writer tool to manipulate a log</param>
        private void SaveLog(string logMessage, TextWriter w)
        {
            w.Write(logMessage);
        }
        #endregion
    }
}
