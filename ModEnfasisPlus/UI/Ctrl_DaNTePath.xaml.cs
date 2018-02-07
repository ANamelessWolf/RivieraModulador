using DaSoft.Riviera.OldModulador.Model;
using DaSoft.Riviera.OldModulador.Runtime;
using NamelessOld.Libraries.Yggdrasil.Aerith;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using static DaSoft.Riviera.OldModulador.Assets.Strings;

namespace DaSoft.Riviera.OldModulador.UI
{
    /// <summary>
    /// Interaction logic for Ctrl_DaNTePath.xaml
    /// </summary>
    public partial class Ctrl_DaNTePath : UserControl
    {
        public Ctrl_DaNTePath()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Cambia la tabla de asociados
        /// </summary>
        private void Asoc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedIndex != -1)
            {
                String pth = Path.Combine(App.Riviera.Asociados.FullName, String.Format("{0}.MDB", (sender as ComboBox).SelectedItem.ToString()));
                if (File.Exists(pth))
                    App.Riviera.AsociadoMDB = new FileInfo(pth);
                App.Riviera.Save();
            }
        }
        /// <summary>
        /// Selecciona el archivo de DaNTe
        /// </summary>
        private void SelectDaNTe_Click(object sender, RoutedEventArgs e)
        {
            FilePicker pck = new FilePicker(false, "mdb");

            pck.InitialDirectory = this.GetInitialDir();
            string pth;
            if (pck.PickPath(CAPTION_DAN, TIT_SEL_DANTE, out pth) && File.Exists(pth))
            {
                App.Riviera.DaNTeMDB = new FileInfo(pth);
                this.fieldDaNTePath.Text = pth;
                App.Riviera.Save();
            }
        }
        /// <summary>
        /// Busca la ruta inicial de DaNTe
        /// </summary>
        /// <returns>La ruta inicial de DaNTE</returns>
        private string GetInitialDir()
        {
            String initialDir;
            if (Directory.Exists(@"C:\Dante\Cuantificaciones"))
                initialDir = @"C:\Dante\Cuantificaciones";
            else if (Directory.Exists(@"C:\Dante"))
                initialDir = @"C:\Dante";
            else if (Directory.Exists(Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "DaSoft", "Dante", "Cuantificaciones")))
                initialDir = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "DaSoft", "Dante", "Cuantificaciones");
            else if (Directory.Exists(Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "DaSoft", "Dante")))
                initialDir = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "DaSoft", "Dante");
            else
                initialDir = @"C:\";
            return initialDir;
        }
        /// <summary>
        /// Selecciona el directorio de asociados
        /// </summary>
        private void SelectAsoc_Click(object sender, RoutedEventArgs e)
        {
            DirectoryPicker pck = new DirectoryPicker();
            pck.InitialDirectory = System.Environment.SpecialFolder.MyComputer;
            string pth;
            if (pck.PickPath("Seleccionar directorio asociados", out pth) && Directory.Exists(pth))
            {
                App.Riviera.Asociados = new DirectoryInfo(pth);
                this.fieldAsocPath.Text = pth;
                this.FillAsociados();
                App.Riviera.Save();
            }
        }
        /// <summary>
        /// Realiza la carga del control de la aplicación
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.Riviera.DaNTeMDB != null)
                this.fieldDaNTePath.Text = App.Riviera.DaNTeMDB.FullName;
            if (App.Riviera.Asociados != null)
            {
                this.fieldAsocPath.Text = App.Riviera.Asociados.FullName;
                this.FillAsociados();
            }
            if (App.Riviera.Modules != null)
                this.fieldModules.Text = App.Riviera.Modules.FullName;
            this.appLog.IsChecked = App.Riviera.LogIsEnabled;

        }
        /// <summary>
        /// Llena la lista de tablas de asociados
        /// </summary>
        private void FillAsociados()
        {
            this.listOfAsoc.Items.Clear();
            FileScanner scn = new FileScanner(App.Riviera.Asociados, true);
            scn.Find(new MDBFilter());

            foreach (var file in scn.Files)
                this.listOfAsoc.Items.Add(file.Name.Substring(0, file.Name.IndexOf('.')));
            if (App.Riviera.AsociadoMDB != null)
            {
                string asoc = App.Riviera.AsociadoMDB.Name;
                asoc = asoc.Substring(0, asoc.IndexOf('.'));
                int index = -1;
                for (int i = 0; index == -1 && index < this.listOfAsoc.Items.Count; i++)
                    if ((this.listOfAsoc.Items[i] as string) == asoc)
                        index = i;
                if (index != -1)
                    this.listOfAsoc.SelectedIndex = index;
                else if (this.listOfAsoc.Items.Count > 0)
                    this.listOfAsoc.SelectedIndex = 0;
            }
            else
                this.listOfAsoc.SelectedIndex = 0;
        }
        /// <summary>
        /// Realiza la selección de modulos de la aplicación
        /// </summary>
        private void SelectModule_Click(object sender, RoutedEventArgs e)
        {
            DirectoryPicker pck = new DirectoryPicker();
            pck.InitialDirectory = System.Environment.SpecialFolder.MyComputer;
            string pth;
            if (pck.PickPath("Seleccionar directorio modulos", out pth) && Directory.Exists(pth))
            {
                App.Riviera.Modules = new DirectoryInfo(pth);
                this.fieldModules.Text = pth;
                App.Riviera.Save();
            }
        }

        private void appLog_Checked(object sender, RoutedEventArgs e)
        {
            if (App.Riviera.LogIsEnabled != (sender as CheckBox).IsChecked.Value)
                App.Riviera.LogIsEnabled = (sender as CheckBox).IsChecked.Value;
        }


    }
}
