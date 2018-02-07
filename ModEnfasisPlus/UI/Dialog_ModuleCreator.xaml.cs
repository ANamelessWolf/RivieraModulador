using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.OldModulador.Model;
using DaSoft.Riviera.OldModulador.Runtime;
using MahApps.Metro.Controls;
using NamelessOld.Libraries.DB.Mikasa.Model;
using NamelessOld.Libraries.DB.Tessa;
using NamelessOld.Libraries.DB.Tessa.Model;
using NamelessOld.Libraries.HoukagoTeaTime.Yui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace DaSoft.Riviera.OldModulador.UI
{
    /// <summary>
    /// Interaction logic for Dialog_ModuleCreator.xaml
    /// </summary>
    public partial class Dialog_ModuleCreator : MetroWindow
    {
        /// <summary>
        /// La acción del modulo
        /// </summary>
        public ModuleAction Action;
        /// <summary>
        /// El punto de inserción del bloque a crear
        /// </summary>
        public Point3d Point;

        /// <summary>
        /// El nombre del módulo
        /// </summary>
        public String ModuleName
        {
            get { return this.mName.Text; }
        }
        /// <summary>
        /// Las claves seleccionadas
        /// </summary>
        public List<Tuple<string, int>> Claves;
        /// <summary>
        /// El bitmap de la imagen
        /// </summary>
        public ImageSource Source;
        /// <summary>
        /// Crea un nuevo dialogo
        /// </summary>
        public Dialog_ModuleCreator(List<Tuple<string, int>> claves, ImageSource src)
        {
            this.Claves = claves;
            this.Source = src;
            InitializeComponent();

        }
        /// <summary>
        /// Ejecuta una acción de la aplicación
        /// </summary>
        private void DialogAction_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string name = (sender as Button).Name;
            if (name == this.button_Sel.Name)
            {
                this.Action = ModuleAction.Select;
                this.Hide();
            }
            else if (name == this.button_Ok.Name && this.ModuleName != String.Empty)
            {
                AccessConnectionBuilder aBuilder = new AccessConnectionBuilder()
                {
                    Access_DB_File = App.Riviera.ModulosMDB.FullName,
                    OleDbProvider = new AccessProvider(OledbProviders.Microsoft_ACE_OleDb_12),
                };
                new VoidAccess_Transaction<Object>(new AccessConnectionContent(aBuilder.Data).GenerateConnectionString(),
                delegate (Access_Connector conn, Object[] trParameters)
                {
                    if (!conn.TableExist(this.ModuleName))
                    {
                        this.Close();
                        this.Action = ModuleAction.Ok;
                    }
                    else if (Dialog_MessageBox.Show(String.Format("El modulo {0} ya existe, ¿esta seguro de querer remplazarlo?", this.ModuleName), System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question) == System.Windows.MessageBoxResult.Yes)
                    {
                        this.Close();
                        this.Action = ModuleAction.Ok;
                    }
                    else
                    {
                        this.Action = ModuleAction.None;
                        this.Hide();
                    }
                }).Run();


            }
            else if (name == this.button_Ok.Name && this.ModuleName == String.Empty)
            {
                this.Action = ModuleAction.Ok;
                Dialog_MessageBox.Show("El nombre del bloque no es válido", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                this.Hide();
            }
            else if (name == this.button_PickPoint.Name)
            {
                this.Hide();
                Selector.Point("Selecciona el punto de inserción del bloque", out this.Point);
                this.ShowDialog();
            }
            else
            {
                this.Action = ModuleAction.Cancel;
                this.Close();
            }


        }
        /// <summary>
        /// Carga la vista del control
        /// </summary>
        public void Load()
        {
            if (this.Claves != null)
            {
                foreach (var clave in Claves.OrderBy(x => x.Item1))
                    table.AddItem(clave.Item1, clave.Item2, clave);
            }

        }
        /// <summary>
        /// Realiza el proceso de inicio de la tabla
        /// </summary>
        /// <param name="claves">Las claves del modulo</param>
        /// <param name="src">La miniatura del modulo</param>
        public void Init(List<Tuple<string, int>> claves, ImageSource src)
        {
            if (claves != null)
            {
                this.Claves = claves;
                this.Load();
            }
            if (src != null)
            {
                this.Source = src;
                blockPreview.Source = this.Source;
            }
        }

        private void MetroWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.Source != null)
                blockPreview.Source = this.Source;
        }

        private void table_Fill(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.Claves != null)
            {
                foreach (var clave in Claves.OrderBy(x => x.Item1))
                    table.AddItem(clave.Item1, clave.Item2, clave);
            }
        }
    }
}
