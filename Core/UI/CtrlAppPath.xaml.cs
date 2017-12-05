using DaSoft.Riviera.Modulador.Core.Controller;
using DaSoft.Riviera.Modulador.Core.Model;
using DaSoft.Riviera.Modulador.Core.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IOPath = System.IO.Path;
using IODirectory = System.IO.Directory;
using IOFileInfo = System.IO.FileInfo;
using IOFile = System.IO.File;
using static DaSoft.Riviera.Modulador.Core.Assets.Strings;
using static DaSoft.Riviera.Modulador.Core.Controller.ApplicationUtils;
using Nameless.Libraries.Yggdrasil.Aerith;
using Nameless.Libraries.Yggdrasil.Aerith.Filter;
using DaSoft.Riviera.Modulador.Core.Controller.UI;
using static System.Environment;

namespace DaSoft.Riviera.Modulador.Core.UI
{
    /// <summary>
    /// Lógica de interacción para CtrlAppPath.xaml
    /// </summary>
    public partial class CtrlAppPath : UserControl
    {
        public string DaNTeDir { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CtrlAppPath"/> class.
        /// </summary>
        public CtrlAppPath()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Handles the Loaded event of the UserControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.Riviera != null)
            {
                string danteDFTL = IOPath.Combine(CheckQuantifyPath(), APP_DANTE_BASES);
                this.tboBases.Text = App.Riviera.GetPath(DaNTePath.DANTE_BASES, danteDFTL);
                this.tboDirAsoc.Text = App.Riviera.GetPath(DaNTePath.ASOC_DIR, CheckAsocPath());
                this.tboDirMod.Text = App.Riviera.GetPath(DaNTePath.MOD_DIR, new IOFileInfo(danteDFTL).DirectoryName);
                if (IODirectory.Exists(this.tboDirAsoc.Text))
                    this.UpdateCboAsoc(this.tboDirAsoc.Text);
                this.Save();
            }
        }
        /// <summary>
        /// Saves this instance.
        /// </summary>
        private void Save()
        {
            string danteDFTL = IOPath.Combine(CheckQuantifyPath(), APP_DANTE_BASES);
            if (IOFile.Exists(this.tboBases.Text))
                App.Riviera.SetPath(DaNTePath.DANTE_BASES, this.tboBases.Text);
            if (IODirectory.Exists(this.tboDirAsoc.Text))
                App.Riviera.SetPath(DaNTePath.ASOC_DIR, this.tboDirAsoc.Text);
            if (IODirectory.Exists(this.tboDirMod.Text))
                App.Riviera.SetPath(DaNTePath.MOD_DIR, this.tboDirMod.Text);
            string asocMDB = IOPath.Combine(this.tboDirAsoc.Text, cboAsoc.SelectedIndex != -1 ? (cboAsoc.SelectedItem as ItemFile).FilePath : String.Empty);
            if (IOFile.Exists(asocMDB))
                App.Riviera.SetPath(DaNTePath.ASOC_MDB, asocMDB);
            App.Riviera.Save();
        }

        /// <summary>
        /// Updates the asociated combo box list
        /// </summary>
        /// <param name="asocDir">The asoc directory.</param>
        private void UpdateCboAsoc(string asocDir)
        {
            AerithScanner scn = new AerithScanner(asocDir, true, true);
            scn.Find(new MicrosoftAccessFilter());
            this.cboAsoc.ItemsSource = scn.Files.Select(x => new ItemFile(x)).OrderBy(y => y.ItemName);
            if (this.cboAsoc.Items.Count > 0)
            {
                string asoc = App.Riviera.GetPath(DaNTePath.ASOC_MDB);
                if (IOFile.Exists(asoc))
                {
                    IOFileInfo file = new IOFileInfo(asoc);
                    ItemFile item = this.cboAsoc.ItemsSource.OfType<ItemFile>().FirstOrDefault(x => x.ItemName == file.Name);
                    if (item != null)
                        this.cboAsoc.SelectedIndex = this.cboAsoc.ItemsSource.OfType<ItemFile>().ToList().IndexOf(item);
                    else
                        this.cboAsoc.SelectedIndex = 0;
                }
                else
                    this.cboAsoc.SelectedIndex = 0;
            }
        }
        /// <summary>
        /// Handles the SelectionChanged event of the cboAsoc control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void cboAsoc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedIndex != -1)
            {
                ItemFile item = this.cboAsoc.SelectedItem as ItemFile;
                String pth = item.FilePath;
                if (IOFile.Exists(pth))
                    App.Riviera.SetPath(DaNTePath.ASOC_MDB, pth);
                App.Riviera.Save();
            }
        }
        /// <summary>
        /// BTNs the select path.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnSelectPath(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            if (btnBases.Name == btn.Name && IODirectory.Exists(this.DaNTeDir))
                this.DaNTeDir.PickFile(TIT_SEL_DANTE, CAPTION_DAN, (String filePath) =>
                {
                    App.Riviera.SetPath(DaNTePath.DANTE_BASES, filePath);
                    this.tboBases.Text = filePath;
                }, "mdb");
            else
            {
                string msg;
                Action<String> task;
                if (btn.Name == btnAsoc.Name)
                {
                    msg = MSG_PICK_ASOC_DIR;
                    task = (String dirPath) =>
                    {
                        App.Riviera.SetPath(DaNTePath.ASOC_DIR, dirPath);
                        this.tboDirAsoc.Text = dirPath;
                        this.UpdateCboAsoc(dirPath);
                    };
                }
                else
                {
                    msg = MSG_PICK_MOD_DIR;
                    task = (String dirPath) =>
                    {
                        App.Riviera.SetPath(DaNTePath.MOD_DIR, dirPath);
                        this.tboDirMod.Text = dirPath;
                    };
                }
                SpecialFolder.MyComputer.PickDirectory(msg, task);
            }
            App.Riviera.Save();
        }
    }
}
