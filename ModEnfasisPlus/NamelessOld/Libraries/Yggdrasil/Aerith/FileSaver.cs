using Microsoft.Win32;
using NamelessOld.Libraries.Yggdrasil.Exceptions;
using NamelessOld.Libraries.Yggdrasil.Lilith;
using NamelessOld.Libraries.Yggdrasil.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.Yggdrasil.Aerith
{
    /// <summary>
    /// Creates a new save dialog.
    /// </summary>
    /// <typeparam name="input">The type of parameters received by the save delegate</typeparam>
    public class FileSaver<input> : NamelessObject
    {
        #region Propiedades
        /// <summary>
        /// The start saving directory.
        /// </summary>
        public String StartDirectory { get; set; }
        /// <summary>
        /// The saved file name
        /// </summary>
        public String SavedFileName { get { return savedfile; } }
        /// <summary>
        /// The saving title dialog
        /// </summary>
        public String SavingTitle { get; set; }
        /// <summary>
        /// Allowed Extensions, write the extension name without the dot.
        /// </summary>
        public string[] AllowedExtensions { get; set; }
        #endregion
        #region Variables
        /// <summary>
        /// The saved file
        /// </summary>
        string savedfile;
        /// <summary>
        /// The delegate for saving a file.
        /// </summary>
        /// <param name="savingParameters">The saving parameters.</param>
        /// <param name="saveName">The name of the saveFile, this value is updated in showDialog action</param>
        /// <returns>True if the saving transaction is succesfull</returns>
        public delegate void SavingTransaction(string saveName, params input[] savingParameters);
        /// <summary>
        /// The action for the current transaction.
        /// </summary>
        SavingTransaction trAction;
        #endregion
        #region Constructor
        /// <summary>
        /// Creates a new saver object.
        /// </summary>
        /// <param name="action">The action for the saving transaction</param>
        /// <param name="allowedExtensions">The allowed extensions, write without the dot</param>
        public FileSaver(SavingTransaction action, params string[] allowedExtensions)
        {
            string vals = String.Empty;
            foreach (string ext in allowedExtensions)
                vals += ext + ", ";
            vals = vals.Substring(0, vals.Length - 2);
            this.AllowedExtensions = allowedExtensions;
            this.trAction = action;
            this.StartDirectory = String.Empty;
            this.savedfile = String.Empty;
        }
        #endregion
        #region Actions
        /// <summary>
        /// Summons the saving dialog.
        /// </summary>
        /// <param name="categoryName">The name of the saving category. Example: Images, Videos, etc</param>
        /// <param name="saveTitle">The title of the saving form dialog.</param>
        /// <param name="savingParameters">The list of parameters needed to save the file.</param>
        /// <returns>True if the file is correctly saved.</returns>
        public void ShowDialog(string categoryName, string saveTitle, params input[] savingParameters)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = FileUtility.CreateFilter(this.AllowedExtensions, categoryName);
                saveDialog.Title = saveTitle;
                if (this.StartDirectory != String.Empty)
                {
                    if (Directory.Exists(this.StartDirectory))
                    {
                        DirectoryInfo dir = new DirectoryInfo(this.StartDirectory);
                        saveDialog.InitialDirectory = dir.FullName;
                    }
                    else
                        throw new BlackMateriaException(String.Format(Errors.MissingDirectory, this.StartDirectory));
                }
                if (saveDialog.ShowDialog().Value && saveDialog.FileName != String.Empty)
                {
                    this.trAction(saveDialog.FileName, savingParameters);
                    this.savedfile = saveDialog.FileName;
                }
            }
            catch (System.Exception exc)
            {
                throw new BlackMateriaException(Errors.SavingFile, exc);
            }
        }
        #endregion
    }
}
