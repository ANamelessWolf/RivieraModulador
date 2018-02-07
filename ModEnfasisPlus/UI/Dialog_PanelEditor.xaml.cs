using DaSoft.Riviera.OldModulador.Controller;
using DaSoft.Riviera.OldModulador.Model;
using DaSoft.Riviera.OldModulador.Model.Delta;
using DaSoft.Riviera.OldModulador.Runtime;
using DaSoft.Riviera.OldModulador.Runtime.Delta;
using MahApps.Metro.Controls;
using NamelessOld.Libraries.HoukagoTeaTime.Yui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CODE;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CONST;
using static DaSoft.Riviera.OldModulador.Assets.Strings;
namespace DaSoft.Riviera.OldModulador.UI
{
    /// <summary>
    /// Interaction logic for Dialog_PanelEditor.xaml
    /// </summary>
    public partial class Dialog_PanelEditor : MetroWindow
    {
        /// <summary>
        /// El tipo de reglas actualmente seleccionadas
        /// </summary>
        public Type RulesType;
        /// <summary>
        /// Ignore the close validation
        /// </summary>
        public Boolean IgnoreCloseValidation;
        /// <summary>
        /// El id del último elemento insertado
        /// </summary>
        public static int LastId;
        /// <summary>
        /// La lista de paneles disponibles para el frente seleccionado
        /// </summary>
        public List<PanelData> Paneles;
        /// <summary>
        /// El objeto que define el frente de los paneles
        /// </summary>
        public RivieraObject Frente;
        /// <summary>
        /// El resultado de la selección
        /// </summary>
        public PanelAction Result;
        /// <summary>
        /// El resultado del dialago
        /// </summary>
        public Boolean ResultIsOk;
        Double tmpHeight;
        /// <summary>
        /// Checa si se trata de un nuevo panel Stack
        /// </summary>
        public bool IsNewPanelStack;
        /// <summary>
        /// La colección de paneles seleccionados
        /// </summary>
        public List<PanelRaw> Data
        {
            get
            {
                List<PanelRaw> result = new List<PanelRaw>();
                IEnumerable<Ctrl_PanelItem> panels = this.mamparaSideA.stack.Children.OfType<Ctrl_PanelItem>().OrderByDescending(x => this.mamparaSideA.IndexOf(x));
                tmpHeight = 0;
                foreach (Ctrl_PanelItem panel in panels)
                    result.Add(new PanelRaw()
                    {
                        Code = panel.Text.Replace(panel.Acabado, ""),
                        Direction = ArrowDirection.Left_Front,
                        Height = GetHeight(panel, this.mamparaSideA),
                        Block = App.DB.Description.Where(x => panel.Text.Contains(x.Code)).FirstOrDefault().Bloque,
                        Nivel = (panel.Data as PanelData).Tipo == "PP" ? ((panel.Height - FLOOR_OFFSET_PX) / NIVEL_SIZE_PX).NivelToString() : (panel.Height / NIVEL_SIZE_PX).NivelToString(),
                        APiso = (panel.Data as PanelData).Tipo == "PP",
                        Acabado = panel.Acabado,
                        Side = panel.Side,
                    });
                panels = this.mamparaSideB.stack.Children.OfType<Ctrl_PanelItem>().OrderByDescending(x => this.mamparaSideB.IndexOf(x));
                tmpHeight = 0;
                foreach (Ctrl_PanelItem panel in panels)
                    result.Add(new PanelRaw()
                    {
                        Code = panel.Text.Replace(panel.Acabado, ""),
                        Direction = ArrowDirection.Right_Front,
                        Height = GetHeight(panel, this.mamparaSideB),
                        Block = App.DB.Description.Where(x => panel.Text.Contains(x.Code)).FirstOrDefault().Bloque,
                        Nivel = (panel.Data as PanelData).Tipo == "PP" ? ((panel.Height - FLOOR_OFFSET_PX) / NIVEL_SIZE_PX).NivelToString() : (panel.Height / NIVEL_SIZE_PX).NivelToString(),
                        APiso = (panel.Data as PanelData).Tipo == "PP",
                        Acabado = panel.Acabado,
                        Side = panel.Side
                    });
                return result;
            }
        }

        public PanelRaw BiomboData
        {
            get
            {
                if (this.mamparaSideA.Biombo != null)
                {
                    IBiomboable biombo = (this.mamparaSideA.Biombo.Side == PanelSide.Lado_A || this.mamparaSideA.Biombo.Side == PanelSide.DontCare) ? this.mamparaSideA.Biombo : this.mamparaSideB.Biombo;
                    return new PanelRaw()
                    {
                        Code = biombo.Text.Length >= 10 ? biombo.Text.Substring(0, 10) : String.Empty,
                        Direction = ArrowDirection.None,
                        Height = Frente.Alto,
                        Block = App.DB.Description.Where(x => biombo.Text.Contains(x.Code)).FirstOrDefault().Bloque,
                        Nivel = (biombo.Height / BIOMBO_SIZE_PX).NivelToString(),
                        APiso = false,
                        Acabado = biombo.Acabado,
                        Side = this.mamparaSideA.Biombo.Side
                    };
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Obtiene la altura del panel a insertar
        /// </summary>
        /// <param name="panel">La altura del panel a insertar</param>
        /// <returns>La altura del panel seleccionado</returns>
        private double GetHeight(Ctrl_PanelItem panel, Ctrl_PanelStack stack)
        {
            double nivel = panel.Height == FLOOR_OFFSET_PX ? 0 : panel.Height / NIVEL_SIZE_PX;

            if (nivel != 0 && (panel.Data as PanelData).Tipo == "PP")
            {
                nivel = panel.Height - FLOOR_OFFSET_PX;
                nivel = nivel / NIVEL_SIZE_PX;
            }
            String nivelStr = nivel.ToString().Contains(".") ? String.Format("{0} 1/2", nivel.ToString().Split('.')[0]) : nivel.ToString();
            if (nivelStr.Contains("0 "))
                nivelStr = nivelStr.Replace("0 ", "");
            String[] fParse;
            double frente;
            if (this.frenteSize.Text.Length > 3)
            {
                fParse = this.frenteSize.Text.Split('"');
                frente = Double.Parse(fParse[0]) + Double.Parse(fParse[1]);
            }
            else
                frente = Double.Parse(this.frenteSize.Text.Replace("\"", ""));
            String code = panel.Text.Replace(panel.Acabado, "");
            //Altura
            double num, h = double.TryParse(code.Substring(code.Length - 2), out num) ? num : 0;

            double altoNominal = App.DB.Alto_Nivel.Where(
                x => x.Type == ElementType.Panel &&
                x.Nivel == nivelStr &&
                x.Alto == h).FirstOrDefault().Alto;

            PanelSize spSze = App.DB.Panel_Size.Where(x => x.Nominal.Alto == altoNominal && x.Nominal.Frente == frente && code.Contains(x.Code)).FirstOrDefault();
            double z = nivel == 0 ? 0d : spSze != null ? spSze.Real.Alto : 0,
                   currentHeight = tmpHeight;
            if (spSze == null)
            {
                Selector.Ed.WriteMessage("No existe un tamaño para el panel {0}, con alto {1} y frente {1}", code, code.Substring(code.Length - 2), frente);
                App.Riviera.Log.AppendEntry(String.Format("No existe un tamaño para el panel {0}, con alto {1} y frente {1}", code, code.Substring(code.Length - 2), frente), NamelessOld.Libraries.Yggdrasil.Lain.Protocol.Error, "GetHeight", "PanelEditor");
            }
            if (nivel != 0 && tmpHeight == 0 && (panel.Data as PanelData).Tipo == "PP")
                tmpHeight = (z - 127d).ConvertUnits(Unit_Type.mm, Unit_Type.m);
            else if (tmpHeight == 0)
                tmpHeight = z.ConvertUnits(Unit_Type.mm, Unit_Type.m);
            else
                tmpHeight += z.ConvertUnits(Unit_Type.mm, Unit_Type.m);
            return currentHeight;
        }

        /// <summary>
        /// La pila de paneles seleccionados.
        /// </summary>
        public Ctrl_PanelStack SelectedStack
        {
            get
            {
                if (this.stackOneBorder.Visibility == Visibility.Visible)
                    return this.mamparaSideA;
                else if (this.stackTwoBorder.Visibility == Visibility.Visible)
                    return this.mamparaSideB;
                else
                    return null;
            }
        }
        /// <summary>
        /// Checa si una mampara ha sido seleccionada.
        /// </summary>
        public bool StackSelected
        {
            get
            {
                Boolean flag = this.SelectedStack != null;
                if (!flag)
                    Dialog_MessageBox.Show(MSG_STACK_NOT_SEL, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return flag;
            }
        }

        public bool IsInverted;

        public Dialog_PanelEditor(Mampara mampara, List<PanelData> paneles)
        {
            this.Paneles = paneles;
            this.Frente = mampara;
            LastId = 0;
            this.IsNewPanelStack = true;
            InitializeComponent();
        }

        private void clicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if ((sender as FrameworkElement).Name == mamparaSideA.Name)
            {
                this.stackOneBorder.Visibility = Visibility.Visible;
                this.stackTwoBorder.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.stackOneBorder.Visibility = Visibility.Collapsed;
                this.stackTwoBorder.Visibility = Visibility.Visible;
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.mamparaSideA.Rules = new RivieraPanelRules(this.mamparaSideA.stack);
            this.mamparaSideB.Rules = new RivieraPanelRules(this.mamparaSideB.stack);
            String code;
            if (this.Frente is RivieraObject)
                code = (this.Frente as RivieraObject).Code;
            else
                code = String.Empty;
            //Se actualiza el código seleccionado
            this.codeHost.Text = code;
            this.frenteSize.Text = code != String.Empty ? String.Format("{0}\"", code.Substring(6, 2)) : String.Empty;
            this.heightSize.Text = code != String.Empty ? String.Format("{0}\"", code.Substring(8, 2)) : String.Empty;
            //Fantasmas
            String[] frentes = new String[] { "66\"", "72\"" };
            String[] heights = new String[] { "48\"", "54\"" };
            if (frentes.Contains(this.frenteSize.Text) && heights.Contains(this.heightSize.Text))
                this.codeHost.Text = String.Format("{0}{1}{2} (Fantasma)", DT_MAMPARA_GHOST, this.frenteSize.Text.Replace("\"", ""), this.heightSize.Text.Replace("\"", ""));

            //Se cambia el tamaño de la mampara
            String niv = App.DB.Alto_Nivel.Where(x => x.Type == ElementType.Mampara && x.Alto == double.Parse(code.Substring(8, 2))).FirstOrDefault().Nivel;
            this.mamparaSideA.Niveles = niv.NivelToValue();
            this.mamparaSideB.Niveles = niv.NivelToValue();
            //Borra la lista de paneles actuales
            this.listOfPanels.Items.Clear();
            foreach (PanelData panel in this.Paneles)
                if (panel.Tipo != null)
                    this.listOfPanels.Items.Add(new Ctrl_PanelDataItem(panel) { JustCode = false });
            if (this.listOfPanels.Items.Count > 0)
                this.listOfPanels.SelectedIndex = 1;
            //Se agregá el panel existente


            mamparaNLevels.Text = this.mamparaSideA.Niveles.NivelToString();
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

        private void listOfPanels_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (this.listOfPanels.SelectedIndex != -1)
            {
                //La lista de elementos de paneles

                Ctrl_PanelDataItem panelDataItem = (Ctrl_PanelDataItem)this.listOfPanels.SelectedItem;
                //panelDataItem.JustCode = true;
                PanelData panel = panelDataItem.Data;
                if (panel.Tipo == "PP")
                    this.optApiso.IsChecked = true;
                else
                    this.optApiso.IsChecked = false;

                if (panel.Tipo == "B" || panel.Tipo == "PI" || panel.Tipo == "C")
                    tagLvOrHeights.Text = "Alturas: ";
                else
                    tagLvOrHeights.Text = "Niveles: ";

                String pHost = panel.Host_Front.ToString(),
                       pFNominal = panel.FrenteNominal,
                       mF = this.frenteSize.Text;
                if (pFNominal != String.Empty && String.Format("{0}\"{1}\"", pFNominal.Substring(0, 2), pFNominal.Substring(2, 2)) != mF)
                    this.frenteSize.Text = String.Format("{0}\"{1}\"", pFNominal.Substring(0, 2), pFNominal.Substring(2, 2));
                else if (pFNominal == String.Empty && String.Format("{0}\"", pHost) != mF)
                    this.frenteSize.Text = String.Format("{0}\"", panel.Host_Front.ToString());

                this.listOfLevels.Items.Clear();
                foreach (string nivel in panel.Niveles)
                    if (nivel == "0")
                        this.listOfLevels.Items.Add("Zoclo");
                    else if (panel.Tipo == "B" || panel.Tipo == "PI" || panel.Tipo == "C")
                        this.listOfLevels.Items.Add(panel.Heights[nivel]);
                    else
                        this.listOfLevels.Items.Add(nivel);
                this.listOfLevels.SelectedIndex = 0;
                this.descPanel.Text = panel.Description;

            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (StackSelected)
            {
                LastId++;
                Ctrl_PanelDataItem panelDataItem = (Ctrl_PanelDataItem)this.listOfPanels.SelectedItem;
                PanelData panel = panelDataItem.Data;

                String nivel;
                if (this.listOfLevels.SelectedItem == null)
                    nivel = "1";
                else
                {
                    if (this.listOfLevels.SelectedItem is Double)
                    {
                        nivel = panel.Heights.Where(x => x.Value == (Double)this.listOfLevels.SelectedItem).FirstOrDefault().Key;
                        if (nivel == null || nivel == String.Empty)
                            nivel = "1";
                    }
                    else if (this.listOfLevels.SelectedItem.ToString() == "Zoclo")
                        nivel = "0";
                    else
                        nivel = (String)this.listOfLevels.SelectedItem;

                }
                if (panel.Tipo == "B" || panel.Tipo == "PI" || panel.Tipo == "C")

                {
                    String code = String.Format("{0}{1}{2:00}", panel.Code, this.frenteSize.Text, panel.Heights.Count > 0 ? panel.Heights[nivel] : 0);
                    code = code.Replace("\"", "");
                    //Existen códigos en donde importa el lado en el que sea insertado, para estos casos se valida con el string
                    String[] biomboSideMatters = new string[] { "DD7000" };
                    Double h;
                    if (biomboSideMatters.Contains(code.Substring(0, 6)))
                    {
                        if (this.SelectedStack.Name == this.mamparaSideA.Name)
                        {
                            h = mamparaSideA.AddBiombo(panel, new String[] { panel.Code, nivel, code });
                            mamparaSideB.AddBiombo(panel, new String[] { String.Empty, nivel, code });
                            mamparaSideA.Biombo.Side = PanelSide.Lado_A;
                        }
                        else
                        {
                            h = mamparaSideB.AddBiombo(panel, new String[] { panel.Code, nivel, code });
                            mamparaSideA.AddBiombo(panel, new String[] { String.Empty, nivel, code });
                            mamparaSideA.Biombo.Side = PanelSide.Lado_B;
                        }
                    }
                    else
                    {
                        h = mamparaSideA.AddBiombo(panel, new String[] { panel.Code, nivel, code });
                        mamparaSideB.AddBiombo(panel, new String[] { String.Empty, nivel, code });
                    }
                    mamparaSideA.Height = mamparaSideA.Height + h;
                    mamparaSideB.Height = mamparaSideA.Height;

                }
                else
                {
                    if (panel.Tipo == "PP" && this.SelectedStack.Count > 0)
                        Dialog_MessageBox.Show(String.Format(WAR_PP, panel.Code), MessageBoxButton.OK, MessageBoxImage.Warning);
                    //else if (nivel.Contains("1/2") && !this.SelectedStack.IsLast(nivel))
                    //    Dialog_MessageBox.Show(WAR_MIDDLE_HEIGHT_P, MessageBoxButton.OK, MessageBoxImage.Warning);
                    else
                    {
                        String code, inverseFront = this.frenteSize.Text.Replace("\"", ""), sF1, sF2;

                        if (inverseFront.Length == 4)
                        {
                            sF1 = inverseFront.Substring(0, 2);
                            sF2 = inverseFront.Substring(2, 2);
                            int num,
                                f1 = int.TryParse(sF1, out num) ? num : 0,
                                f2 = int.TryParse(sF2, out num) ? num : 0,
                                fVal = f1 + f2;
                            Boolean is66 = fVal == 66;
                            Boolean is3024 = new String[] { "DD2033", "DD2037", "DD2042", "DD2043", "DD2046", "DD2047" }.Contains(panel.Code);
                            if (this.SelectedStack == this.mamparaSideB && is66)
                            {
                                code = String.Format("{0}{1}{2:00}", panel.Code, this.frenteSize.Text, panel.Heights[nivel]);
                                code = code.Replace("\"", "");
                                this.SelectRules(code, nivel);
                                inverseFront = this.frenteSize.Text.Replace("\"", "");
                                inverseFront = inverseFront.Substring(2, 2) + inverseFront.Substring(0, 2);
                                code = String.Format("{0}{1}{2:00}", panel.Code, inverseFront, panel.Heights[nivel]);
                                this.SelectedStack.AddGajo(code, panel, nivel, nivel.Contains("/"));
                            }
                            else if (fVal == 54 && is3024)
                            {
                                code = String.Format("{0}{1}{2:00}", panel.Code, this.frenteSize.Text, panel.Heights[nivel]);
                                code = code.Replace("\"", "");
                                this.SelectRules(code, nivel);
                                inverseFront = "3024";
                                code = String.Format("{0}{1}{2:00}", panel.Code, inverseFront, panel.Heights[nivel]);
                                this.SelectedStack.AddGajo(code, panel, nivel, nivel.Contains("/"));
                            }
                            else
                            {
                                code = String.Format("{0}{1}{2:00}", panel.Code, this.frenteSize.Text, panel.Heights[nivel]);
                                code = code.Replace("\"", "");
                                this.SelectRules(code, nivel);
                                this.SelectedStack.AddGajo(code, panel, nivel, nivel.Contains("/"));
                            }
                        }
                        else
                        {
                            code = String.Format("{0}{1}{2:00}", panel.Code, this.frenteSize.Text, panel.Heights[nivel]);
                            code = code.Replace("\"", "");
                            this.SelectRules(code, nivel);
                            this.SelectedStack.AddGajo(code, panel, nivel, nivel.Contains("/"));
                        }
                    }
                }
            }

        }
        /// <summary>
        /// Se realiza el proceso de selección para las mamparas seleccionadas
        /// </summary>
        private void SelectRules(String code, String niveles)
        {
            if (RulesType == typeof(MamparaPanelRules))
                this.SelectedStack.Rules = new MamparaPanelRules(RivieraPanelRules.GetSize(niveles), this.SelectedStack.stack, this.SelectedStack.AvailableHeight, code);
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            Ctrl_PanelDataItem panelDataItem = (Ctrl_PanelDataItem)this.listOfPanels.SelectedItem;
            PanelData panel = panelDataItem.Data;
            if (StackSelected && (this.SelectedStack.SelectedItem != null || panel.Tipo == "B" || panel.Tipo == "PI" || panel.Tipo == "C"))
            {

                if (panel.Tipo == "B" || panel.Tipo == "PI" || panel.Tipo == "C")
                {
                    Double h = mamparaSideA.RemoveBiombo();
                    mamparaSideB.RemoveBiombo();
                    mamparaSideA.Height = mamparaSideA.Height - h;
                    mamparaSideB.Height = mamparaSideA.Height;
                }
                else
                {
                    String nivel = (String)this.listOfLevels.SelectedItem;
                    if (nivel == "Zoclo")
                        nivel = "0";
                    String code = String.Format("{0}{1}{2}", panel.Code, this.frenteSize.Text, panel.Heights[nivel]);
                    this.SelectedStack.RemoveGajo();
                }
            }
        }

        private void CopyLeft_Click(object sender, RoutedEventArgs e)
        {
            Ctrl_PanelItem[] panels = this.mamparaSideB.Extract();
            this.mamparaSideA.Clear();
            for (int i = 0; i < panels.Length; i++)
                SwapCode(panels[i]);
            panels.ToList().ForEach(x => this.mamparaSideA.stack.Children.Add(x));

            foreach (Ctrl_PanelItem panel in mamparaSideA.stack.Children)
                panel.Click += mamparaSideA.Item_Click;
        }

        private void Swap_Click(object sender, RoutedEventArgs e)
        {
            Ctrl_PanelStack pivot, second;
            if (this.SelectedStack == null)
                this.stackOneBorder.Visibility = Visibility.Visible;
            //1: Se preparan las listas para el swap
            if (this.SelectedStack.Name == mamparaSideA.Name)
            {
                pivot = mamparaSideA;
                second = mamparaSideB;
            }
            else
            {
                pivot = mamparaSideB;
                second = mamparaSideA;
            }
            //2: Se realiza la clonación de paneles
            Ctrl_PanelItem[] pivotPanels = second.Extract(),
                             secondPanels = pivot.Extract();
            for (int i = 0; i < pivotPanels.Length; i++)
                SwapCode(pivotPanels[i]);
            for (int i = 0; i < secondPanels.Length; i++)
                SwapCode(secondPanels[i]);
            //3: Se intercambia la información
            pivot.Clear();
            pivotPanels.ToList().ForEach(x => pivot.stack.Children.Add(x));
            second.Clear();
            secondPanels.ToList().ForEach(x => second.stack.Children.Add(x));
            //4: Agregamos los eventos
            foreach (Ctrl_PanelItem panel in mamparaSideA.stack.Children)
                panel.Click += mamparaSideA.Item_Click;
            foreach (Ctrl_PanelItem panel in mamparaSideB.stack.Children)
                panel.Click += mamparaSideB.Item_Click;
        }

        private void CopyRight_Click(object sender, RoutedEventArgs e)
        {
            Ctrl_PanelItem[] panels = this.mamparaSideA.Extract();
            for (int i = 0; i < panels.Length; i++)
                SwapCode(panels[i]);
            this.mamparaSideB.Clear();
            panels.ToList().ForEach(x => this.mamparaSideB.stack.Children.Add(x));
            foreach (Ctrl_PanelItem panel in mamparaSideB.stack.Children)
                panel.Click += mamparaSideB.Item_Click;
        }

        private void SwapCode(Ctrl_PanelItem panel)
        {
            string code = panel.Text;
            if (code.Length > 12)
                code = code.Substring(0, 12);
            if (code.Length == 12)
            {
                string sF1 = code.Substring(6, 2),
                       sF2 = code.Substring(8, 2);
                Boolean is3024 = new String[] { "DD2033", "DD2037", "DD2042", "DD2043", "DD2046", "DD2047" }.Contains(code.Substring(0, 6));
                int num,
                    f1 = int.TryParse(sF1, out num) ? num : 0,
                    f2 = int.TryParse(sF2, out num) ? num : 0;
                if (f1 + f2 == 66)
                    panel.Text = String.Format("{0}{1}{2}{3}", code.Substring(0, 6), sF2, sF1, code.Substring(10, 2)) + panel.Acabado;
                else if (f1 + f2 == 54 && is3024)
                    panel.Text = String.Format("{0}{1}{2}{3}", code.Substring(0, 6), 30, 24, code.Substring(10, 2)) + panel.Acabado;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.ResultIsOk = false;
            this.Result = PanelAction.None;
            this.Close();

        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.ResultIsOk = true;
            Boolean ePanelsAreValid = !HasBadElectricPanelOrientation();
            if (this.MamparaAreReady() && ePanelsAreValid)
            {
                this.Close();
                this.Result = IsNewPanelStack ? PanelAction.Create : PanelAction.Update;
            }
            else if (IgnoreCloseValidation)
            {
                this.ResultIsOk = false;
                this.Close();
            }
            else if (!ePanelsAreValid)
                Dialog_MessageBox.Show(ERR_SOP_24IN, MessageBoxButton.OK, MessageBoxImage.Warning);
            else
                Dialog_MessageBox.Show(WAR_INCOMPLETE_PANEL, MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        /// <summary>
        /// Determina si un panel electrico en una mampara de 54 tiene una mala orientación.
        /// El panel electrico no puede estar del lado dede 24"
        /// </summary>
        /// <returns>
        ///   <c>verdadero</c> si [la orientación del panel eléctrico es correcto]; en otro caso, <c>falso</c>.
        /// </returns>
        private bool HasBadElectricPanelOrientation()
        {
            String[] eRgtPan = new String[] { "DD2032", "DD2036", "DD2042", "DD2046" };
            String[] eLftPan = new String[] { "DD2033", "DD2037", "DD2043", "DD2047" };
            Boolean sideAHasBadOrientationNormal = Data.Where(p => p.Direction == ArrowDirection.Left_Front).Count(x => eLftPan.Contains(x.Code.Substring(0, 6))) > 0,
                      sideBHasBadOrientationNormal = Data.Where(p => p.Direction == ArrowDirection.Right_Front).Count(x => eRgtPan.Contains(x.Code.Substring(0, 6))) > 0,
                      sideAHasBadOrientationInverted = Data.Where(p => p.Direction == ArrowDirection.Left_Front).Count(x => eRgtPan.Contains(x.Code.Substring(0, 6))) > 0,
                      sideBHasBadOrientationInverted = Data.Where(p => p.Direction == ArrowDirection.Right_Front).Count(x => eLftPan.Contains(x.Code.Substring(0, 6))) > 0,
                      sideAHasBadOrientation = (sideAHasBadOrientationNormal && !this.IsInverted) || (sideAHasBadOrientationInverted && this.IsInverted),
                      sideBHasBadOrientation = (sideBHasBadOrientationNormal && !this.IsInverted) || (sideBHasBadOrientationInverted && this.IsInverted);
            return this.Frente.Code.Substring(6, 2) == "54" && (sideAHasBadOrientation || sideBHasBadOrientation);
        }

        private bool MamparaAreReady()
        {
            return this.mamparaSideA.stack.Children.Count > 0 && this.mamparaSideA.stack.Children.OfType<Ctrl_PanelItem>().Count(x => x.Status == PanelStatus.Ok || x.Status == PanelStatus.Selected) == this.mamparaSideA.stack.Children.Count &&
                   this.mamparaSideB.stack.Children.Count > 0 && this.mamparaSideB.stack.Children.OfType<Ctrl_PanelItem>().Count(x => x.Status == PanelStatus.Ok || x.Status == PanelStatus.Selected) == this.mamparaSideB.stack.Children.Count;
        }

        private void mamparaSideA_Fill(object sender, RoutedEventArgs e)
        {
            this.mamparaSideA.Clear();
            long id = this.Frente.Children[FIELD_LEFT_FRONT];
            RivieraPanelStack stack = App.DB[id] as RivieraPanelStack;
            if (stack != null && stack.Collection != null)
            {
                foreach (PanelRaw raw in stack.Collection.OrderBy<PanelRaw, Double>(x => x.Height))
                {
                    LastId++;
                    this.mamparaSideA.AddGajo(raw.Code, raw.GetPanelData(this.Paneles), raw.Nivel, raw.Nivel.Contains("/"));
                    this.mamparaSideA.stack.Children.OfType<Ctrl_PanelItem>().FirstOrDefault().Acabado = raw.Acabado;
                    this.mamparaSideA.stack.Children.OfType<Ctrl_PanelItem>().FirstOrDefault().codePanel.Text =
                        this.mamparaSideA.stack.Children.OfType<Ctrl_PanelItem>().FirstOrDefault().Text + this.mamparaSideA.stack.Children.OfType<Ctrl_PanelItem>().FirstOrDefault().Acabado;
                }
                this.IsNewPanelStack = false;
            }
            if (this.Frente is Mampara)
            {
                String b = String.Empty;
                if ((this.Frente as Mampara).BiomboId != 0)
                    b = (App.DB[(this.Frente as Mampara).BiomboId] as RivieraBiombo).PanelData;

                if (b != String.Empty)
                {
                    String[] data = b.Replace("@", "").Split('|');
                    String code = data[0];
                    String nivel = data[3];
                    PanelData panel = this.Paneles.Where(x => code.Substring(0, 6) == x.Code).FirstOrDefault();
                    String[] biomboSideMatters = new string[] { "DD7000" };
                    Double h;
                    if (biomboSideMatters.Contains(code.Substring(0, 6)))
                    {
                        if ((PanelSide)int.Parse(data[7]) == PanelSide.Lado_A)
                        {
                            h = mamparaSideA.AddBiombo(panel, new String[] { panel.Code, nivel, code });
                            mamparaSideB.AddBiombo(panel, new String[] { String.Empty, nivel, this.Frente.Code.Substring(6, 2) });
                            mamparaSideA.Biombo.Side = PanelSide.Lado_A;
                            mamparaSideA.Biombo.Acabado = data[6];
                            mamparaSideA.Biombo.Text = mamparaSideA.Biombo.GetCode() + mamparaSideA.Biombo.Acabado;
                        }
                        else
                        {
                            h = mamparaSideB.AddBiombo(panel, new String[] { panel.Code, nivel, code });
                            mamparaSideA.AddBiombo(panel, new String[] { String.Empty, nivel, code });
                            mamparaSideA.Biombo.Side = PanelSide.Lado_B;
                            mamparaSideB.Biombo.Acabado = data[6];
                            mamparaSideB.Biombo.Text = mamparaSideA.Biombo.GetCode() + mamparaSideA.Biombo.Acabado;
                        }
                    }
                    else
                    {
                        h = mamparaSideA.AddBiombo(panel, new String[] { panel.Code, nivel, code });
                        mamparaSideB.AddBiombo(panel, new String[] { String.Empty, nivel, this.Frente.Code.Substring(6, 2) });
                        mamparaSideA.Biombo.Acabado = data[6];
                        mamparaSideA.Biombo.Text = mamparaSideA.Biombo.GetCode() + mamparaSideA.Biombo.Acabado;
                    }


                    mamparaSideA.Height = mamparaSideA.Height + h;
                    mamparaSideB.Height = mamparaSideA.Height;
                }
            }
        }

        private void mamparaSideB_Fill(object sender, RoutedEventArgs e)
        {
            this.mamparaSideB.Clear();
            long id = this.Frente.Children[FIELD_RIGHT_FRONT];
            RivieraPanelStack stack = App.DB[id] as RivieraPanelStack;
            if (stack != null && stack.Collection != null)
                foreach (PanelRaw raw in stack.Collection.OrderBy<PanelRaw, Double>(x => x.Height))
                {
                    LastId++;
                    this.mamparaSideB.AddGajo(raw.Code, raw.GetPanelData(this.Paneles), raw.Nivel, raw.Nivel.Contains("/"));
                    this.mamparaSideB.stack.Children.OfType<Ctrl_PanelItem>().FirstOrDefault().Acabado = raw.Acabado;
                    this.mamparaSideB.stack.Children.OfType<Ctrl_PanelItem>().FirstOrDefault().codePanel.Text =
                        this.mamparaSideB.stack.Children.OfType<Ctrl_PanelItem>().FirstOrDefault().Text + this.mamparaSideB.stack.Children.OfType<Ctrl_PanelItem>().FirstOrDefault().Acabado;
                }
        }

        private void optApiso_Checked(object sender, RoutedEventArgs e)
        {
            this.mamparaSideA.InsertFromFloor = (sender as CheckBox).IsChecked.Value;
            this.mamparaSideB.InsertFromFloor = (sender as CheckBox).IsChecked.Value;
            this.mamparaSideA.Rules.UpdateStatus(this.mamparaSideA.InsertFromFloor);
            this.mamparaSideB.Rules.UpdateStatus(this.mamparaSideB.InsertFromFloor);
        }

        private void SetAcabado_Click(object sender, RoutedEventArgs e)
        {
            Ctrl_PanelDataItem panelDataItem = (Ctrl_PanelDataItem)this.listOfPanels.SelectedItem;
            PanelData panel = panelDataItem.Data;

            if (StackSelected && (this.SelectedStack.SelectedItem != null || panel.Tipo == "B" || panel.Tipo == "PI" || panel.Tipo == "C"))
            {
                if (panel.Tipo == "B" || panel.Tipo == "PI" || panel.Tipo == "C")
                    mamparaSideA.Biombo.SetAcabado();
                else
                    this.SelectedStack.SelectedItem.SetAcabado();
            }
        }


    }
}
