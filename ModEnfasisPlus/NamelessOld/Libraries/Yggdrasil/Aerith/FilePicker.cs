using Microsoft.Win32;
using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.Yggdrasil.Aerith
{
    public class FilePicker : NamelessObject
    {
        /// <summary>
        /// True if the file picker allows multiple selection
        /// </summary>
        public Boolean AllowMultipleSelection;
        /// <summary>
        /// Get or sets the list of file extensions allowed by the file picker
        /// </summary>
        public String[] ExtensionFilter;

        /// <summary>
        /// Creates a new file picker
        /// </summary>
        /// <param name="allowedExtensions">The allowed file extensions for selection</param>
        /// <param name="allowMultipleSelection">True if the picker will allow multiple file selection</param>
        public FilePicker(bool allowMultipleSelection, params string[] allowedExtensions)
        {
            this.AllowMultipleSelection = allowMultipleSelection;
            this.ExtensionFilter = allowedExtensions;
        }
        /// <summary>
        /// La ruta de inicio para seleccionar el archivo o los archivos
        /// </summary>
        public string InitialDirectory;

        /// <summary>
        /// Gets the file path from the windows open file dialog
        /// </summary>
        /// <param name="catName">The file type category</param>
        /// <param name="fileDialogTitle">The file dialog title</param>
        /// <returns>The path of the selected file, empty if the selection is cancelled</returns>
        public Boolean PickPath(String catName, String fileDialogTitle, out String selPath)
        {
            Boolean flag;
            OpenFileDialog oDialog = new OpenFileDialog();
            selPath = String.Empty;
            oDialog.Filter = FileUtility.CreateFilter(ExtensionFilter, catName);
            oDialog.Title = fileDialogTitle;
            oDialog.Multiselect = this.AllowMultipleSelection;
            if (InitialDirectory != null && Directory.Exists(InitialDirectory))
                oDialog.InitialDirectory = InitialDirectory;
            flag = oDialog.ShowDialog().Value;
            if (flag)
                selPath = oDialog.FileName;
            return flag;
        }
    }
}
