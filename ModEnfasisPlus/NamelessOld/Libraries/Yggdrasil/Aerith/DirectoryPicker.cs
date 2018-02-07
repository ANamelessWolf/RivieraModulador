using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.Windows.Forms;
using static System.Environment;

namespace NamelessOld.Libraries.Yggdrasil.Aerith
{
    public class DirectoryPicker : NamelessObject
    {
        /// <summary>
        /// True if the file picker allows multiple selection
        /// </summary>
        public Boolean AllowMultipleSelection;

        /// <summary>
        /// Creates a new directory picker
        /// </summary>
        /// <param name="allowMultipleSelection">True if the picker will allow multiple directory selection</param>
        public DirectoryPicker()
        {
        }
        /// <summary>
        /// La ruta de inicio para seleccionar los directorios
        /// </summary>
        public SpecialFolder InitialDirectory;

        /// <summary>
        /// Gets the Directory path from the windows open file dialog
        /// </summary>
        /// <param name="dialogDescription">The file dialog title</param>
        /// <returns>The path of the directory, empty if the selection is cancelled</returns>
        public Boolean PickPath(String dialogDescription, out String selPath)
        {
            Boolean flag;
            FolderBrowserDialog oDialog = new FolderBrowserDialog();
            selPath = String.Empty;
            oDialog.Description = dialogDescription;
            oDialog.RootFolder = InitialDirectory;
            flag = oDialog.ShowDialog() == DialogResult.OK;
            if (flag)
                selPath = oDialog.SelectedPath;
            return flag;
        }

    }
}
