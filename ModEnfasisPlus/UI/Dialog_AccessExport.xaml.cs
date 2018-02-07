using DaSoft.Riviera.OldModulador.Runtime.DaNTe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static DaSoft.Riviera.OldModulador.Assets.Strings;
namespace DaSoft.Riviera.OldModulador.UI
{
    /// <summary>
    /// Interaction logic for Dialog_AccessExport.xaml
    /// </summary>
    public partial class Dialog_AccessExport : Window
    {

        public DaNTeExportStatus Result;
        /// <summary>
        /// Accede a la opción de cuantificación agrupada
        /// </summary>
        public Boolean GroupedQuantification
        {
            get { return this.grpQuantification.IsChecked.Value; }
            set { this.grpQuantification.IsChecked = value; }
        }

        /// <summary>
        /// Accede a la opción de mezclar cuantificación
        /// </summary>
        public Boolean MergeQuantification
        {
            get { return this.mergeQuantification.IsChecked.Value; }
            set { this.mergeQuantification.IsChecked = value; }
        }

        /// <summary>
        /// El nombre de la cuantificación realizada por el usuario
        /// </summary>
        public String QuantificationName
        {
            get { return this.qName.Text; }
            set { this.qName.Text = value; }
        }
        /// <summary>
        /// El nombre de la zona
        /// </summary>
        public String Zone
        {
            get { return this.qZone.SelectedItem.ToString(); }

        }
        /// <summary>
        /// Los comentarios de la cuantificación
        /// </summary>
        public String Comments
        {
            get { return this.qComments.Text; }

        }

        public Action SendAction;

        /// <summary>
        /// La lista de las zonas disponibles
        /// </summary>
        public List<String> Zones;
        /// <summary>
        /// La lista de cuantificaciones realizadas
        /// </summary>
        public static List<String> DoneQuantifications;
        /// <summary>
        /// Crea un controlador nuevo para exportar a Access
        /// </summary>
        /// <param name="zones">La lista de zonas disponibles</param>
        public Dialog_AccessExport(List<string> doneQuantifications, params String[] zones)
        {
            InitializeComponent();
            Result = DaNTeExportStatus.None;
            this.Zones = zones.ToList();
            DoneQuantifications = doneQuantifications.ToList();
        }
        /// <summary>
        /// La opción seleccionado
        /// </summary>
        private void DialogAction_Click(object sender, RoutedEventArgs e)
        {
            if (this.button_New.Name == (sender as FrameworkElement).Name)
                Result = DaNTeExportStatus.New;
            else if (this.button_Save.Name == (sender as FrameworkElement).Name)
                Result = DaNTeExportStatus.Save;
            else if (this.button_Cancel.Name == (sender as FrameworkElement).Name)
                Result = DaNTeExportStatus.Cancel;
            if ((Result == DaNTeExportStatus.Save || Result == DaNTeExportStatus.New) && this.QuantificationName == string.Empty)
                Dialog_MessageBox.Show(ERR_QNAME_NEEDED, MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                if (this.qName.Text != "" && !DoneQuantifications.Contains(this.qName.Text))
                {
                    DoneQuantifications.Add(this.qName.Text);
                    this.listOfCuantificaciones.Items.Add(this.qName.Text);
                }
                if (Result == DaNTeExportStatus.Cancel)
                    this.Close();
                else
                    SendAction();
            }
        }
        /// <summary>
        /// Focus the window
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
            this.qZone.Items.Add("Todo");
            foreach (String zone in this.Zones.OrderBy(x => x))
                this.qZone.Items.Add(zone);
            this.qZone.SelectedIndex = 0;
            foreach (String q in DoneQuantifications.OrderBy(x => x))
                this.listOfCuantificaciones.Items.Add(q);
        }
        /// <summary>
        /// Updates the name of the qua
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Selection_Changed(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).SelectedIndex > 0)
                this.qName.Text = ((ComboBox)sender).SelectedItem.ToString();
        }
    }
}
