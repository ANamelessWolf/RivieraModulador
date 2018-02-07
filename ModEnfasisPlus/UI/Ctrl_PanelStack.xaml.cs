using DaSoft.Riviera.OldModulador.Model;
using DaSoft.Riviera.OldModulador.Runtime;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CONST;
using static DaSoft.Riviera.OldModulador.Assets.Strings;
namespace DaSoft.Riviera.OldModulador.UI
{
    /// <summary>
    /// Interaction logic for Ctrl_PanelStack.xaml
    /// </summary>
    public partial class Ctrl_PanelStack : UserControl
    {
        /// <summary>
        /// Códigos de biombos sin clamps
        /// </summary>
        String[] BiombosCodes = new String[] { "DD8001", "DD8002", "DD8003" };
        /// <summary>
        /// Pichoneras
        /// </summary>
        String[] PichonerasCodes = new String[] { "DD7000", "DD7001", "DD7002", "DD7003" };
        /// <summary>
        /// Gabinetes
        /// </summary>
        String[] CajonerasCodes = new String[] { "DD7011", "DD7012" };
        /// <summary>
        /// Códigos de biombos con clamps
        /// </summary>
        String[] BiombosCodesWithClamps = new String[] { "DD8004", "DD8005", "DD8006" };
        /// <summary>
        /// Inicia el llenado de la pila
        /// </summary>

        public event RoutedEventHandler Fill;
        /// <summary>
        /// Define el conjunto de reglas para definir una mampara
        /// </summary>
        public RivieraPanelRules Rules;
        /// <summary>
        /// Regresa la altura disponible de la mampara en pixeles
        /// </summary>
        public Double AvailableHeight
        {
            get
            {
                return TotalHeight -
                    (this.stack.Children.Count > 0 ? this.stack.Children.OfType<Ctrl_PanelItem>().Sum(x => x.Height) : 0);
            }
        }
        /// <summary>
        /// El nivel actual de la mampara
        /// </summary>
        public int CurrentLevel
        {
            get
            {
                double hG = (this.stack.Children.Count > 0 ? this.stack.Children.OfType<Ctrl_PanelItem>().Sum(x => x.Height) : 0);
                if (hG == 0 || hG == FLOOR_OFFSET_PX)
                    return 1;
                else
                {
                    if ((this.stack.Children.OfType<Ctrl_PanelItem>().LastOrDefault().Data as PanelData).Tipo == "PP")
                        hG -= FLOOR_OFFSET_PX;
                    return ((int)(hG / NIVEL_SIZE_PX)) + 1;
                }
            }
        }

        /// <summary>
        /// Regresa la altura total en pixeles
        /// </summary>
        public Double TotalHeight
        {
            get { return Niveles * NIVEL_SIZE_PX + FLOOR_OFFSET_PX; }
        }

        /// <summary>
        /// El número de niveles de la mampara seleccionada
        /// </summary>
        public Double Niveles
        {
            get
            {
                return (Double)GetValue(NivelProperty);
            }
            set
            {
                SetValue(NivelProperty, value);
            }
        }

        /// <summary>
        /// Si esta bandera es verdadera, el primer gajo se inserta desde el suelo. 
        /// En otro caso solo se inserta con el floor offset
        /// </summary>
        public Boolean InsertFromFloor
        {
            get
            {
                return (Boolean)GetValue(InsertFromFloorProperty);
            }
            set
            {
                SetValue(InsertFromFloorProperty, value);
            }
        }
        /// <summary>
        /// El biombo seleccionado
        /// </summary>
        public IBiomboable Biombo
        {
            get { return this.BiomboContainer.Children.OfType<IBiomboable>().FirstOrDefault(); }
        }

        /// <summary>
        /// El panel actualmente seleccionado
        /// </summary>
        public Ctrl_PanelItem SelectedItem;
        /// <summary>
        /// El indice seleccionado
        /// </summary>
        public int SelectedIndex;

        /// <summary>
        /// El total de paneles insertados
        /// </summary>
        public int Count
        {
            get { return this.stack.Children.Count; }
        }

        public static DependencyProperty NivelProperty;
        public static DependencyProperty InsertFromFloorProperty;


        /// <summary>
        /// Constructor estatico
        /// </summary>
        static Ctrl_PanelStack()
        {
            NivelProperty = DependencyProperty.Register("NivelValue", typeof(Double), typeof(Ctrl_PanelStack),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender, Nivel_Changed));
            InsertFromFloorProperty = DependencyProperty.Register("InsertFromFloorValue", typeof(Boolean), typeof(Ctrl_PanelStack),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, InsertFromFloor_Changed));
        }
        /// <summary>
        /// Actualiza el marco de mampara
        /// </summary>
        private static void Nivel_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Ctrl_PanelStack ctr = sender as Ctrl_PanelStack;
            Double res = (Double)e.NewValue;
            ctr.Height = res * NIVEL_SIZE_PX + FLOOR_OFFSET_PX;
        }
        /// <summary>
        /// Actualiza la altura de inserción manual de la mampara
        /// </summary>
        private static void InsertFromFloor_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Ctrl_PanelStack ctr = sender as Ctrl_PanelStack;
            Boolean isFromFloor = (Boolean)e.NewValue;
            ctr.Rules.AvailableHeight = ctr.AvailableHeight;
        }

        /// <summary>
        /// Agregá un biombo
        /// </summary>
        /// <param name="biombodata">La infgormación para insertar el biombo</param>
        public Double AddBiombo(PanelData pData, Object biombodata)
        {
            Double h = 0, hLess;
            try
            {
                if (biombodata is String[])
                {
                    if (this.BiomboContainer.Children.Count > 0)
                    {
                        hLess = RemoveBiombo();
                        this.Height = this.Height - hLess;
                    }
                    String[] data = biombodata as String[];
                    Double size = this.GetBiomboSize(data[1]);
                    h = size;
                    String acabado;
                    if (data[0] != String.Empty)
                    {
                        data[0] = data[2];
                        acabado = App.DB.Acabados.Where(x => x.Code == data[0].Substring(0, 6)).FirstOrDefault() != null ?
                            App.DB.Acabados.Where(x => x.Code == data[0].Substring(0, 6)).FirstOrDefault().Acabados.FirstOrDefault() != null ?
                            App.DB.Acabados.Where(x => x.Code == data[0].Substring(0, 6)).FirstOrDefault().Acabados.FirstOrDefault().Item1 :
                            String.Empty : String.Empty;
                    }
                    else
                        acabado = String.Empty;
                    IBiomboable bItem = null;
                    String code = pData.Code;

                    if (this.BiombosCodesWithClamps.Contains(code))
                        bItem = new Ctrl_BiomboItem() { Text = data[0], Height = size, Data = pData, Acabado = acabado };
                    else if (this.BiombosCodes.Contains(code))
                        bItem = new Ctrl_LinearBiomboItem() { Text = data[0], Height = size, Data = pData, Acabado = acabado };
                    else if (this.PichonerasCodes.Contains(code))
                        bItem = new Ctrl_PichoneraItem() { Text = data[0], Height = size, Data = pData, Acabado = acabado, DivCentral = pData.Host_Front >= 42 };
                    else if (this.CajonerasCodes.Contains(code))
                        bItem = new Ctrl_CajoneraItem() { Text = data[0], Height = size, Data = pData, Acabado = acabado };
                    if (bItem != null)
                    {
                        bItem.Text = bItem.Text + bItem.Acabado;
                        this.BiomboContainer.Children.Add(bItem as UIElement);
                    }
                }
            }
            catch (Exception exc)
            {
                Dialog_MessageBox.Show(String.Format(ERR_ADD_BIOMBO, exc.Message), MessageBoxButton.OK, MessageBoxImage.Error);
                App.Riviera.Log.AppendEntry(exc.Message, NamelessOld.Libraries.Yggdrasil.Lain.Protocol.Error, "AddBiombo", "Panel Editor");
            }
            return h;
        }



        /// <summary>
        /// Obtiene el tamaño en pixeles de un nivel seleccionado
        /// </summary>
        /// <param name="nivel">El nivel seleccionado</param>
        /// <returns>El tamaño del nivel seleccionado</returns>
        public double GetBiomboSize(String nivel)
        {
            Boolean half = nivel.Contains("/");
            Double size = 0, bSize = BIOMBO_SIZE_PX;

            if (half)
                size = nivel.Split(' ').Length == 2 ?
                    (double.Parse(nivel.Split(' ')[0]) + 0.5) * bSize : bSize / 2;
            else
                size = double.Parse(nivel) * bSize;
            return size;
        }

        /// <summary>
        /// Remueve el objeto superior, biombo, cajonera o pichonera
        /// </summary>
        public double RemoveBiombo()
        {
            IBiomboable item = this.BiomboContainer.Children.OfType<IBiomboable>().FirstOrDefault();
            Double h = 0;
            if (item != null)
            {
                h = item.Height;
                this.BiomboContainer.Children.Clear();
            }
            return h;
        }

        /// <summary>
        /// Agrega un nuevo gajo a la colección
        /// </summary>
        /// <param name="code">El código del gajo agregar</param>
        /// <param name="half">Verdadero si se agrega medio gajo</param>
        /// <param name="data">La información del panel</param>
        public void AddGajo(String code, Object data, String nivel, Boolean half = false)
        {
            Ctrl_PanelItem item;
            Double size = RivieraPanelRules.GetSize(nivel);

            PanelData pData = data as PanelData;
            this.Rules.Code = pData.Code;
            this.Rules.Level = this.CurrentLevel;
            String acabado = App.DB.Acabados.Where(x => x.Code == code.Substring(0, 6)).FirstOrDefault() != null ?
                             App.DB.Acabados.Where(x => x.Code == code.Substring(0, 6)).FirstOrDefault().Acabados.FirstOrDefault() != null ?
                             App.DB.Acabados.Where(x => x.Code == code.Substring(0, 6)).FirstOrDefault().Acabados.FirstOrDefault().Item1 :
                             String.Empty : String.Empty;
            if (this.Rules.IsValid())
            {
                item = new Ctrl_PanelItem()
                {
                    Height = size,
                    Text = code,
                    Status = PanelStatus.Ok,
                    Data = data,
                    Id = Dialog_PanelEditor.LastId,
                    Acabado = acabado
                };
                item.codePanel.Text = item.Text + item.Acabado;
                this.Rules.UpdateMargin(item, pData.Tipo == "PP");
                item.Click += Item_Click;
                this.stack.Children.Insert(0, item);
            }
            this.Rules.AvailableHeight = this.AvailableHeight;
            this.Rules.UpdateStatus(this.InsertFromFloor);
        }
        /// <summary>
        /// Realiza la extracción de paneles a un arreglo de paneles
        /// La extracción es una copia, no una referencia
        /// </summary>
        /// <returns>La colección de paneles extraidos</returns>
        public Ctrl_PanelItem[] Extract()
        {
            //Preparamos la memoria
            Ctrl_PanelItem pan;
            Ctrl_PanelItem[] panels = new Ctrl_PanelItem[this.stack.Children.Count];
            for (int i = 0; i < panels.Length; i++)
            {
                pan = (this.stack.Children[i] as Ctrl_PanelItem);
                panels[i] = new Ctrl_PanelItem()
                {
                    Height = pan.Height,
                    Text = pan.Text,
                    Status = pan.Status,
                    Data = pan.Data,
                    Id = pan.Id,
                    Margin = pan.Margin,
                    Acabado = pan.Acabado,
                    Side = pan.Side

                };
                string code = panels[i].Text.Replace(panels[i].Acabado, "");
                panels[i].codePanel.Text = code + panels[i].Acabado;
            }
            return panels;
        }

        /// <summary>
        /// Elimina el gajo seleccionado
        /// </summary>
        public void RemoveGajo()
        {
            if (this.SelectedItem != null)
            {
                Ctrl_PanelItem item = this.SelectedItem;
                int index = this.IndexOf(item);
                this.stack.Children.RemoveAt(index);

                Ctrl_PanelItem last = this.stack.Children.OfType<Ctrl_PanelItem>().LastOrDefault();
                if (last != null && (last.Data as PanelData).Tipo == "PP")
                    last.Margin = new Thickness(0, 0, 0, 0);
                else if (last != null)
                    last.Margin = new Thickness(0, 0, 0, FLOOR_OFFSET_PX);

                this.Rules.AvailableHeight = this.AvailableHeight;
                this.Rules.UpdateStatus(this.InsertFromFloor);
                this.SelectedItem = null;
                this.SelectedIndex = -1;
            }


        }
        /// <summary>
        /// Borra la colección de gajos
        /// </summary>
        public void Clear()
        {
            this.stack.Children.Clear();
        }

        /// <summary>
        /// Selecciona un elemento de la lista
        /// </summary>
        internal void Item_Click(object sender, MouseButtonEventArgs e)
        {
            this.Rules.AvailableHeight = this.AvailableHeight;
            this.Rules.UpdateStatus(this.InsertFromFloor);
            int index = this.IndexOf(sender as Ctrl_PanelItem);
            if (this.SelectedIndex != index)
            {
                this.SelectedIndex = index;
                this.SelectedItem = (sender as Ctrl_PanelItem);
                this.SelectedItem.Status = PanelStatus.Selected;
            }
            else
            {
                this.SelectedItem = null;
                this.SelectedIndex = -1;
            }

        }
        /// <summary>
        /// Devuelve el indice del elemento seleccionado
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(Ctrl_PanelItem item)
        {
            int index = -1;
            for (int i = 0; index == -1 && i < this.stack.Children.Count; i++)
                if (item.Id == (stack.Children[i] as Ctrl_PanelItem).Id)
                    index = i;
            return index;
        }

        /// <summary>
        /// Crea una nueva pila
        /// </summary>
        public Ctrl_PanelStack()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Inicia el llenado del control
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (Fill != null)
            {
                this.Clear();
                Fill(this, e);
            }
            foreach (Ctrl_PanelItem panel in this.stack.Children)
                panel.Status = PanelStatus.Ok;
            this.Rules.AvailableHeight = this.AvailableHeight;
        }

        internal bool IsLast(string nivel)
        {
            Double size = RivieraPanelRules.GetSize(nivel),
                   height = AvailableHeight - size;
            PanelStatus status = height == FLOOR_OFFSET_PX || height == 0 ?
                               PanelStatus.Ok :
                               PanelStatus.Error;
            return status == PanelStatus.Ok;
        }
    }
}
