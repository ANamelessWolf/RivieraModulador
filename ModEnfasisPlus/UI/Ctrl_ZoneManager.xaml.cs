using DaSoft.Riviera.OldModulador.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace DaSoft.Riviera.OldModulador.UI
{
    /// <summary>
    /// Interaction logic for Ctrl_ZoneManager.xaml
    /// </summary>
    public partial class Ctrl_ZoneManager : UserControl
    {

        /// <summary>
        /// Establece el filtro en la definición de zonas
        /// </summary>
        public Boolean EnableFilter { get { return this.enableFilter.IsChecked.Value; } set { this.enableFilter.IsChecked = value; } }

        public Ctrl_ZoneManager()
        {
            InitializeComponent();
        }

        private void ElementAction_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            bool filter = enableFilter.IsChecked.Value;
            if (butt_AddElement.Name == (sender as Button).Name &&
                this.listOfZones.SelectedIndex != -1)
                new ZoneManager().AddElementsToZone(this.listOfZones.SelectedItem as String, filter);
            else if (butt_DelElement.Name == (sender as Button).Name)
                new ZoneManager().RemoveZoneFromElements();
            else if (butt_SnoopElement.Name == (sender as Button).Name)
                new ZoneManager().SnoopElementZone();
            else if (butt_NoZoneElement.Name == (sender as Button).Name)
                new ZoneManager().SnoopElementByZone();

        }

        private void ZoneAction_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (butt_Create.Name == (sender as Button).Name &&
                this.fieldZone.Text != String.Empty &&
                !this.listOfZones.Items.OfType<String>().Contains(this.fieldZone.Text.ToUpper()) &&
                new ZoneManager().AddZone(this.fieldZone.Text.ToUpper()))
                this.listOfZones.Items.Add(this.fieldZone.Text.ToUpper());
            else if (butt_Delete.Name == (sender as Button).Name &&
                this.listOfZones.SelectedIndex != -1 &&
                new ZoneManager().RemoveZone(this.listOfZones.SelectedItem as String))
                this.listOfZones.Items.RemoveAt(this.listOfZones.SelectedIndex);
            else if (butt_Rename.Name == (sender as Button).Name &&
                this.fieldZone.Text != String.Empty &&
                !this.listOfZones.Items.OfType<String>().Contains(this.fieldZone.Text.ToUpper()) &&
                this.listOfZones.SelectedIndex != -1 &&
                new ZoneManager().RenameZone(this.listOfZones.SelectedItem as String, this.fieldZone.Text.ToUpper()))
            {
                this.listOfZones.Items.RemoveAt(this.listOfZones.SelectedIndex);
                this.listOfZones.Items.Add(this.fieldZone.Text.ToUpper());
            }
            else if (butt_Show.Name == (sender as Button).Name &&
                this.listOfZones.SelectedIndex != -1)
                new ZoneManager().SnoopElementByZone(this.listOfZones.SelectedItem as String);

        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            List<String> zones = new ZoneManager().ListZones();
            this.listOfZones.Items.Clear();
            foreach (String zone in zones)
                this.listOfZones.Items.Add(zone);
        }
    }
}

