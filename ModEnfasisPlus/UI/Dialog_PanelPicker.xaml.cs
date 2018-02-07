using DaSoft.Riviera.OldModulador.Controller;
using DaSoft.Riviera.OldModulador.Model;
using DaSoft.Riviera.OldModulador.Model.Delta;
using DaSoft.Riviera.OldModulador.Runtime;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static DaSoft.Riviera.OldModulador.Controller.PanelExtender;
namespace DaSoft.Riviera.OldModulador.UI
{
    /// <summary>
    /// Interaction logic for Dialog_PanelPicker.xaml
    /// </summary>
    public partial class Dialog_PanelPicker : MetroWindow
    {
        /// <summary>
        /// El resultado de la selección
        /// </summary>
        public PanelAction Result;
        /// <summary>
        /// El resultado del dialago
        /// </summary>
        public Boolean ResultIsOk;
        /// <summary>
        /// Checa si se trata de un nuevo panel Stack
        /// </summary>
        public bool IsNewPanelStack;
        /// <summary>
        /// La lista de paneles disponibles para el frente seleccionado
        /// </summary>
        public List<PanelData> Paneles;
        /// <summary>
        /// La lista de paneles disponibles para el frente seleccionado
        /// para la parte superior
        /// </summary>
        public List<PanelData> UpperPaneles;
        /// <summary>
        /// La lista de biombos disponibles para el frente seleccionado
        /// </summary>
        public List<PanelData> Biombos;
        /// <summary>
        /// Habilita el modo para uso de paneles triples
        /// </summary>
        public Boolean TriplePanelMode;
        /// <summary>
        /// La mampara de la izquierda
        /// </summary>
        public Mampara LeftMampara;
        /// <summary>
        /// La mampara de la derecha
        /// </summary>
        public Mampara RightMampara;
        /// <summary>
        /// La mampara de la perpendicular
        /// </summary>
        public Mampara TopMampara;
        /// <summary>
        /// La unión de los paneles
        /// </summary>
        JointObject Joint;
        /// <summary>
        /// La colección con la relación altura nominal, tamaño del nivel
        /// </summary>
        /// <value>
        /// La colección de alturas-niveles
        /// </value>
        Dictionary<int, String> PanelHeights
        {
            get
            {
                Dictionary<int, String> pH = new Dictionary<int, string>();
                int[] h = new int[] { 6, 12, 18, 24, 30, 36, 42, 48 };
                String[] lv = new String[] { "1/2", "1", "1 1/2", "2", "2 1/2", "3", "3 1/2", "4" };
                for (int i = 0; i < h.Length; i++)
                    pH.Add(h[i], lv[i]);
                return pH;
            }
        }
        /// <summary>
        /// Obtiene el valor del panel seleccionado
        /// </summary>
        /// <value>
        /// El panel seleccionado.
        /// </value>
        public Ctrl_PanelDataItem SelectedPanel
        {
            get
            {
                return this.listOfPanels.SelectedIndex != -1 ? ((Ctrl_PanelDataItem)this.listOfPanels.SelectedItem) : null;
            }
        }
        /// <summary>
        /// Obtiene el valor del biombo seleccionado
        /// </summary>
        /// <value>
        /// El biombo seleccionado.
        /// </value>
        public Ctrl_PanelDataItem SelectedBiombo
        {
            get
            {
                return this.listOfBiombos.SelectedIndex != -1 ? ((Ctrl_PanelDataItem)this.listOfBiombos.SelectedItem) : null;
            }
        }
        /// <summary>
        /// Obtiene el valor del panel seleccionado, para superior
        /// </summary>
        /// <value>
        /// El panel seleccionado.
        /// </value>
        public Ctrl_PanelDataItem SelectedUpperPanel
        {
            get
            {
                return this.listOfUpperPanels.SelectedIndex != -1 ? ((Ctrl_PanelDataItem)this.listOfUpperPanels.SelectedItem) : null;
            }
        }
        /// <summary>
        /// Accede a la información del panel seleccionado
        /// </summary>
        /// <value>
        /// La información del panel seleccionado.
        /// </value>
        public PanelData PData
        {
            get
            {
                return this.SelectedPanel != null ? this.SelectedPanel.Data : null;
            }
        }
        /// <summary>
        /// Accede a la información del biombo seleccionado
        /// </summary>
        /// <value>
        /// La información del biombo seleccionado.
        /// </value>
        public PanelData BData
        {
            get
            {
                return this.SelectedBiombo != null ? this.SelectedBiombo.Data : null;
            }
        }
        /// <summary>
        /// Accede a la información del panel superior seleccionado
        /// </summary>
        /// <value>
        /// La información del panel seleccionado.
        /// </value>
        public PanelData UpperPData
        {
            get
            {
                return this.SelectedUpperPanel != null ? this.SelectedUpperPanel.Data : null;
            }
        }
        /// <summary>
        /// Obtiene el acabado default del panel seleccionado
        /// </summary>
        /// <value>
        /// El código de acabado
        /// </value>
        public String Acabado
        {
            get
            {
                return this.PData != null && _Acabado == null ? App.DB.Acabados.Where(x => x.Code == this.PData.Code).FirstOrDefault()?.Acabados.FirstOrDefault().Item1 : _Acabado != null ? _Acabado : null;
            }
        }
        String _Acabado;
        /// <summary>
        /// Obtiene el acabado default del biombo seleccionado
        /// </summary>
        /// <value>
        /// El código del biombo seleccionado
        /// </value>
        public String AcabadoBiombo
        {
            get
            {
                return this.BData != null && _AcabadoBiombo == null ? App.DB.Acabados.Where(x => x.Code == this.BData.Code).FirstOrDefault()?.Acabados.FirstOrDefault().Item1 : _AcabadoBiombo != null ? _AcabadoBiombo : null;
            }
        }
        String _AcabadoBiombo;
        /// <summary>
        /// Obtiene el acabado default del panel seleccionado
        /// </summary>
        /// <value>
        /// El código de acabado
        /// </value>
        public String AcabadoUpper
        {
            get
            {
                return this.UpperPData != null && _AcabadoUpper == null ? App.DB.Acabados.Where(x => x.Code == this.UpperPData.Code).FirstOrDefault()?.Acabados.FirstOrDefault().Item1 : _AcabadoUpper != null ? _AcabadoUpper : null;
            }
        }
        String _AcabadoUpper;
        /// <summary>
        /// Obtiene la información cruda del lado izquierdo 
        /// del arreglo de paneles dobles
        /// </summary>
        /// <value>
        /// El arreglo de paneles dobles
        /// </value>
        public PanelRaw LeftPanel
        {
            get
            {
                if (this.SelectedPanel != null)
                {
                    return new PanelRaw()
                    {
                        Code = this.GetPanelCode(this.LeftMampara, this.TriplePanelMode),
                        Direction = ArrowDirection.Left_Back,
                        Height = this.PData.Tipo == "PP" ? 0 : 0.128d,
                        Block = App.DB.Description.Where(x => x.Code.Contains(this.SelectedPanel.Code)).FirstOrDefault().Bloque,
                        Nivel = this.PData.Heights.Last().Key,
                        APiso = this.PData.Tipo == "PP",
                        Acabado = Acabado,
                        Side = PanelSide.Lado_A,
                    };
                }
                else
                    return new PanelRaw();
            }
        }
        /// <summary>
        /// Obtiene la información cruda del lado derecho de la parte trasera 
        /// del arreglo de paneles dobles
        /// </summary>
        /// <value>
        /// El arreglo de paneles dobles
        /// </value>
        public PanelRaw RightPanel
        {
            get
            {
                if (this.SelectedPanel != null)
                {
                    return new PanelRaw()
                    {
                        Code = this.GetPanelCode(this.RightMampara, this.TriplePanelMode),
                        Direction = ArrowDirection.Right_Back,
                        Height = this.PData.Tipo == "PP" ? 0 : 0.128d,
                        Block = App.DB.Description.Where(x => x.Code.Contains(this.SelectedPanel.Code)).FirstOrDefault().Bloque,
                        Nivel = this.PData.Heights.Last().Key,
                        APiso = this.PData.Tipo == "PP",
                        Acabado = Acabado,
                        Side = PanelSide.Lado_B,
                    };
                }
                else
                    return new PanelRaw();
            }
        }

        /// <summary>
        /// Obtiene la información cruda del lado doble 
        /// </summary>
        /// <value>
        /// El arreglo de paneles dobles
        /// </value>
        public PanelRaw DoublePanel
        {
            get
            {
                if (this.SelectedPanel != null)
                {
                    PanelData panel = this.PData;
                    string code = String.Format("{0}{1}{2:00}", panel.Code, PanelRaw.GetPanelDoubleFrente(this.LeftMampara, this.RightMampara), panel.Heights.Last().Value);
                    return new PanelRaw()
                    {
                        Code = code,
                        Direction = ArrowDirection.Front,
                        Height = this.PData.Tipo == "PP" ? 0 : 0.128d,
                        Block = App.DB.Description.Where(x => x.Code.Contains(this.SelectedPanel.Code)).FirstOrDefault().Bloque,
                        Nivel = this.PData.Heights.Last().Key,
                        APiso = this.PData.Tipo == "PP",
                        Acabado = Acabado,
                        Side = PanelSide.Lado_AB

                    };
                }
                else
                    return new PanelRaw();
            }
        }
        /// <summary>
        /// Obtiene la información cruda del lado doble 
        /// </summary>
        /// <value>
        /// El arreglo de paneles dobles
        /// </value>
        public PanelRaw DoubleBiombo
        {
            get
            {
                if (this.SelectedBiombo != null)
                {
                    PanelData panel = this.BData;
                    string nivel = this.listOfBiombosLevels.SelectedIndex != -1 ? this.listOfBiombosLevels.SelectedItem.ToString() : panel.Niveles.FirstOrDefault(),
                        code = String.Format("{0}{1}{2:00}", panel.Code, PanelRaw.GetPanelDoubleFrente(this.LeftMampara, this.RightMampara), panel.Heights[nivel]);
                    return new PanelRaw()
                    {
                        Code = code,
                        Direction = ArrowDirection.Same,
                        Height = this.LeftMampara.Alto,
                        Block = App.DB.Description.Where(x => x.Code.Contains(this.SelectedBiombo.Code)).FirstOrDefault().Bloque,
                        Nivel = nivel,
                        APiso = false,
                        Acabado = AcabadoBiombo,
                        Side = PanelSide.Lado_AB

                    };
                }
                else
                    return new PanelRaw();
            }
        }
        /// <summary>
        /// Obtiene la información cruda del lado doble 
        /// </summary>
        /// <value>
        /// El arreglo de paneles dobles
        /// </value>
        public PanelRaw DoublePanelBack
        {
            get
            {
                if (this.SelectedPanel != null)
                {
                    PanelData panel = this.PData;
                    string code = String.Format("{0}{1}{2:00}", panel.Code, PanelRaw.GetPanelDoubleFrente(this.LeftMampara, this.RightMampara), panel.Heights.Last().Value);
                    return new PanelRaw()
                    {
                        Code = code,
                        Direction = ArrowDirection.Back,
                        Height = this.PData.Tipo == "PP" ? 0 : 0.128d,
                        Block = App.DB.Description.Where(x => x.Code.Contains(this.SelectedPanel.Code)).FirstOrDefault().Bloque,
                        Nivel = this.PData.Heights.Last().Key,
                        APiso = this.PData.Tipo == "PP",
                        Acabado = Acabado,
                        Side = PanelSide.Lado_AB,
                    };
                }
                else
                    return new PanelRaw();
            }
        }
        /// <summary>
        /// Obtiene la información cruda del lado doble 
        /// </summary>
        /// <value>
        /// El arreglo de paneles dobles
        /// </value>
        public PanelRaw DoublePanelUpperBack
        {
            get
            {
                if (this.SelectedUpperPanel != null)
                {
                    PanelData panel = this.UpperPData;
                    string code = String.Format("{0}{1}{2:00}", panel.Code, PanelRaw.GetPanelDoubleFrente(this.LeftMampara, this.RightMampara), this.UpperHeightAsNominalString);
                    string bloque = App.DB.Description.Where(x => x.Code.Contains(this.SelectedUpperPanel.Code)).FirstOrDefault().Bloque;
                    return new PanelRaw()
                    {
                        Code = code,
                        Direction = ArrowDirection.Back,
                        Height = this.UpperHeight,
                        Block = bloque,
                        Nivel = this.UpperPData.Heights.Last().Key,
                        APiso = false,
                        Acabado = this.AcabadoUpper,
                        Side = PanelSide.Lado_AB,
                    };
                }
                else
                    return new PanelRaw();
            }
        }
        /// <summary>
        /// Devuelve la altura del nivel inferior en formato 
        /// nominal
        /// </summary>
        /// <value>
        /// La altura nominal como cadena
        /// </value>
        public String LowerHeightAsNominalString
        {
            get
            {
                var heights = PanelHeights;
                string[] h = { "54", "48", "42", "36", "30" },
                        lv = { "4", "3 1/2", "3", "2 1/2", "2" };
                return this.PData.Heights[lv[h.ToList().IndexOf(this.TopMampara.Code.Substring(8, 2))]].ToString();
            }
        }
        /// <summary>
        /// Devuelve la altura del nivel superior en formato 
        /// nominal
        /// </summary>
        /// <value>
        /// La altura nominal como cadena
        /// </value>
        public String UpperHeightAsNominalString
        {
            get
            {
                return String.Format("{0:00}", this.LeftMampara.RivSize.Nominal.Alto - this.TopMampara.RivSize.Nominal.Alto);
            }
        }
        /// <summary>
        /// La altura inicial para el panel superior.
        /// La altura está en metros.
        /// </summary>
        /// <value>
        /// El valor de la altura en metros.
        /// </value>
        public Double UpperHeight
        {
            get
            {
                string[] hs = { "54", "48", "42", "36", "30" },
                        lvs = { "4", "3 1/2", "3", "2 1/2", "2" };
                var heights = this.PanelHeights;
                var pData = this.PData;
                int height;
                String nivelStr;
                Double h;
                double altoNominal;
                PanelSize spSze;
                height = int.Parse(this.TopMampara.Code.Substring(8, 2));
                nivelStr = lvs[hs.ToList().IndexOf(this.TopMampara.Code.Substring(8, 2))];
                h = pData.Heights[nivelStr];
                altoNominal = App.DB.Alto_Nivel.Where(x => x.Type == ElementType.Panel && x.Nivel == nivelStr && x.Alto == h).FirstOrDefault().Alto;
                var frente = PanelRaw.GetPanelDoubleFrente(this.LeftMampara, this.RightMampara);

                if (altoNominal == 53)
                {
                    altoNominal = 41;
                    spSze = App.DB.Panel_Size.Where(x => x.Nominal.Alto == altoNominal && x.Nominal.Frente.ToString() == frente && pData.Code == x.Code).FirstOrDefault();
                }
                else
                    spSze = App.DB.Panel_Size.Where(x => x.Nominal.Alto == altoNominal && x.Nominal.Frente.ToString() == frente && pData.Code == x.Code).FirstOrDefault();
                if (this.PData.Tipo == "PP")
                    return (spSze.Real.Alto).ConvertUnits(Unit_Type.mm, Unit_Type.m);
                else
                    return (spSze.Real.Alto).ConvertUnits(Unit_Type.mm, Unit_Type.m) + 0.0040d;
            }
        }


        private string GetFrente(Mampara mampara)
        {
            return mampara.Code.Substring(6, 2);
        }
        /// <summary>
        /// Obtiene el código de panel formateado, sin acabados.
        /// </summary>
        /// <param name="mampara">La mampara seleccionada.</param>
        /// <returns>El código del panel</returns>
        private String GetPanelCode(Mampara mampara, Boolean uselowerHeight = false)
        {
            PanelData panel = this.PData;
            if (panel != null)
                return String.Format("{0}{1}{2:00}", panel.Code, this.GetFrente(mampara), uselowerHeight ? LowerHeightAsNominalString : panel.Heights.Last().Value.ToString());
            else
                return String.Empty;
        }

        public Dialog_PanelPicker(MamparaJoint joint, Mampara left, Mampara right, Mampara top, List<PanelData> paneles, List<PanelData> panelesUpper, List<PanelData> biombos)
        {
            this.Joint = joint;
            this.TopMampara = top;
            this.LeftMampara = left;
            this.RightMampara = right;
            this.Paneles = paneles;
            this.UpperPaneles = panelesUpper;
            this.Biombos = biombos;
            if (left.RivSize.Real.Alto >= 1348 || (top.RivSize.Real.Alto < left.RivSize.Real.Alto))
                this.TriplePanelMode = true;
            //Se apaga la opción de paneles triples cuando el frente es de 18 y la diferencia es de 6"
            if (left.RivSize.Nominal.Frente == 18 &&
                Math.Abs(left.RivSize.Nominal.Alto - top.RivSize.Nominal.Alto) == 6 &&
                left.RivSize.Nominal.Alto > top.RivSize.Nominal.Alto)
                this.TriplePanelMode = false;

            InitializeComponent();

        }

        private void HideTripleMode()
        {
            this.tagPaneles.Text = "Paneles";
            this.row.Height = new GridLength(0, GridUnitType.Pixel);
        }

        private void listOfPanels_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (this.listOfPanels.SelectedIndex != -1)
            {
                Ctrl_PanelDataItem panelDataItem = (Ctrl_PanelDataItem)this.listOfPanels.SelectedItem;
                PanelData panel = panelDataItem.Data;
                this.descPanel.Text = panel.Description;
            }
        }

        private void listOfUpperPanels_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (this.listOfUpperPanels.SelectedIndex != -1)
            {
                Ctrl_PanelDataItem panelDataItem = (Ctrl_PanelDataItem)this.listOfUpperPanels.SelectedItem;
                PanelData panel = panelDataItem.Data;
                this.descPanel.Text = panel.Description;
            }
        }
        private void listOfBiombos_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (this.listOfBiombos.SelectedIndex != -1)
            {
                Ctrl_PanelDataItem panelDataItem = (Ctrl_PanelDataItem)this.listOfBiombos.SelectedItem;
                PanelData panel = panelDataItem.Data;
                this.descPanel.Text = panel.Description;
                this.listOfBiombosLevels.Items.Clear();
                foreach (String lv in panel.Niveles)
                    this.listOfBiombosLevels.Items.Add(lv);
                if (listOfBiombosLevels.Items.Count > 0)
                    this.listOfBiombosLevels.SelectedIndex = 0;
            }
        }

        private void listOfPanels_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            foreach (Ctrl_PanelDataItem pItem in this.listOfPanels.Items)
                pItem.JustCode = false;
        }

        private void listOfPanels_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            foreach (Ctrl_PanelDataItem pItem in this.listOfPanels.Items)
                pItem.JustCode = true;
        }

        private void listOfPanelsUpper_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            foreach (Ctrl_PanelDataItem pItem in this.listOfUpperPanels.Items)
                pItem.JustCode = false;
        }

        private void listOfPanelsUpper_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            foreach (Ctrl_PanelDataItem pItem in this.listOfUpperPanels.Items)
                pItem.JustCode = true;
        }
        private void listOfBiombos_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            foreach (Ctrl_PanelDataItem pItem in this.listOfBiombos.Items)
                pItem.JustCode = false;
        }

        private void listOfBiombos_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            foreach (Ctrl_PanelDataItem pItem in this.listOfBiombos.Items)
                pItem.JustCode = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.ResultIsOk = false;
            this.Result = PanelAction.None;
            this.Close();
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            this.ResultIsOk = true;
            this.Result = PanelAction.Delete;
            this.Close();
        }
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.ResultIsOk = true;
            if (!this.Joint.HasDoublePanels)
                this.Result = PanelAction.Create;
            else
                this.Result = PanelAction.Update;
            if (chBiombo.IsChecked.Value && (this.listOfBiombosLevels.SelectedIndex == -1 || this.listOfBiombosLevels.SelectedIndex == -1))
            {
                Dialog_MessageBox.Show("Debe seleccionar un biombo con un nivel si habilito la opción de biombos para poder aplicar los cambios", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (this.listOfPanels.SelectedIndex != -1 && (this.row.Height.Value == 0 || (this.row.Height.Value > 0 && this.listOfUpperPanels.SelectedIndex != -1)))
                this.Close();
            else
                Dialog_MessageBox.Show("Debe seleccionar un panel para poder aplicar los cambios", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.biomboRow.Height = new GridLength(0);
            //Borra la lista de paneles actuales
            this.listOfPanels.Items.Clear();
            foreach (PanelData panel in this.Paneles)
                if (panel.Tipo != null)
                    this.listOfPanels.Items.Add(new Ctrl_PanelDataItem(panel) { JustCode = false });
            this.listOfUpperPanels.Items.Clear();
            foreach (PanelData panel in this.UpperPaneles)
                if (panel.Tipo != null)
                    this.listOfUpperPanels.Items.Add(new Ctrl_PanelDataItem(panel) { JustCode = false });
            this.listOfBiombos.Items.Clear();
            foreach (PanelData panel in this.Biombos.Where(x => x.Niveles.Count > 0))
                if (panel.Tipo != null)
                    this.listOfBiombos.Items.Add(new Ctrl_PanelDataItem(panel) { JustCode = false });
            if (!TriplePanelMode)
                this.HideTripleMode();
        }

        private void SetAcabado_Click(object sender, RoutedEventArgs e)
        {
            string sName = (sender as FrameworkElement).Name;
            if (sName == btnAcabado.Name && this.listOfPanels.SelectedIndex != -1)
            {
                Ctrl_PanelDataItem panelDataItem = (Ctrl_PanelDataItem)this.listOfPanels.SelectedItem;
                PanelData panel = panelDataItem.Data;
                String acabado;
                panelDataItem.SelectAcabado(out acabado);
                this._Acabado = acabado;
            }
            else if (sName == btnUpperAcabado.Name && this.listOfUpperPanels.SelectedIndex != -1)
            {
                Ctrl_PanelDataItem panelDataItem = (Ctrl_PanelDataItem)this.listOfUpperPanels.SelectedItem;
                PanelData panel = panelDataItem.Data;
                String acabado;
                panelDataItem.SelectAcabado(out acabado);
                this._AcabadoUpper = acabado;
            }
            else if (sName == btnBiomboAcabado.Name && this.listOfBiombos.SelectedIndex != -1)
            {
                Ctrl_PanelDataItem panelDataItem = (Ctrl_PanelDataItem)this.listOfBiombos.SelectedItem;
                PanelData panel = panelDataItem.Data;
                String acabado;
                panelDataItem.SelectAcabado(out acabado);
                this._AcabadoBiombo = acabado;
            }
        }

        private void chBiombo_Checked(object sender, RoutedEventArgs e)
        {
            if (this.chBiombo.IsChecked.Value)
                this.biomboRow.Height = new GridLength(72);
            else
                this.biomboRow.Height = new GridLength(0);
        }
    }
}
