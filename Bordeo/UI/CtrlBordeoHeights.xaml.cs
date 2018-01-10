﻿using DaSoft.Riviera.Modulador.Bordeo.Model;
using DaSoft.Riviera.Modulador.Bordeo.Model.UI;
using DaSoft.Riviera.Modulador.Core.Controller.UI;
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

namespace DaSoft.Riviera.Modulador.Bordeo.UI
{
    /// <summary>
    /// Lógica de interacción para CtrlBordeoHeights.xaml
    /// </summary>
    public partial class CtrlBordeoHeights : UserControl
    {
        /// <summary>
        /// Gets the selected height for the panel.
        /// </summary>
        /// <value>
        /// The selected height panel.
        /// </value>
        public BordeoPanelHeight SelectedHeight
        {
            get
            {
                BordeoPanelHeightItem sel = this.cboPanelHeights.SelectedIndex != -1 ?
                    (BordeoPanelHeightItem)cboPanelHeights.SelectedItem : new BordeoPanelHeightItem() { Height = BordeoPanelHeight.None };
                return sel.Height;
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="CtrlBordeoHeights"/> class.
        /// </summary>
        public CtrlBordeoHeights()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Handles the SelectionChanged event of the cboPanelHeights control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void cboPanelHeights_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboPanelHeights.SelectedIndex != -1)
            {
                BordeoPanelHeightItem sel = (BordeoPanelHeightItem)cboPanelHeights.SelectedItem;
               // var bitmap = new BitmapImage(new Uri(String.Format("pack://application:,,,/riv_bordeo;component/Assets/{0}", sel.ImageName)));
                //var stream = this.GetType().Assembly.GetManifestResourceStream(String.Format("/riv_bordeo;component/Assets/{0}", sel.ImageName));
                //img.Source = String.Format("/riv_bordeo;component/Assets/{0}", sel.ImageName).LoadImage(true);
            }


        }
        /// <summary>
        /// Handles the Loaded event of the UserControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            cboPanelHeights.ItemsSource =
                Enum.GetValues(typeof(BordeoPanelHeight)).OfType<BordeoPanelHeight>().
                Where(x => x != BordeoPanelHeight.None).
                Select(y => new BordeoPanelHeightItem() { Height = y });
            if (cboPanelHeights.Items.Count > 0)
                this.cboPanelHeights.SelectedIndex = 0;
        }
    }
}
