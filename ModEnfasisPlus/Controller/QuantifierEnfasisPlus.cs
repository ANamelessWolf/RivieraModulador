using Autodesk.AutoCAD.DatabaseServices;
using DaSoft.Riviera.OldModulador.Controller.Delta;
using DaSoft.Riviera.OldModulador.Model;
using DaSoft.Riviera.OldModulador.Model.Delta;
using DaSoft.Riviera.OldModulador.Runtime;
using DaSoft.Riviera.OldModulador.UI;
using NamelessOld.Libraries.HoukagoTeaTime.Mio;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu.Shapes2D;
using NamelessOld.Libraries.HoukagoTeaTime.Yui;
using NamelessOld.Libraries.Yggdrasil.Lain;
using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.Collections.Generic;
using System.Linq;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CODE;
using static DaSoft.Riviera.OldModulador.Assets.Strings;

namespace DaSoft.Riviera.OldModulador.Controller
{
    public class QuantifierEnfasisPlus : NamelessObject
    {
        /// <summary>
        /// El cuantificador principal de la aplicación
        /// </summary>
        public Quantifier Quantifier;
        /// <summary>
        /// La base de datos a cuantificar
        /// </summary>
        public List<RivieraObject> Database;
        /// <summary>
        /// La colección de mamparas con paneles
        /// </summary>
        public IEnumerable<RivieraObject> MamparasWithPanels;
        /// <summary>
        /// La mampara actual en el ciclo de cuantificación
        /// </summary>
        Mampara Current;
        /// <summary>
        /// La colección de errores encontrados en la cuantificación
        /// </summary>
        Dictionary<long, MamparaEnfasisQuantificationError> Errors;
        /// <summary>
        /// Frentes que consideran una mampara doble
        /// </summary>
        int[] MamparasWithDoubleFront = new int[] { 54, 60, 66, 72 };
        /// <summary>
        /// Prefijos de paneles electricos
        /// </summary>
        String[] ElectricPanelPrefixes = new String[] { "DD203", "DD204" };
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
        /// Inicializa una nueva instancia de <see cref="QuantifierEnfasisPlus"/> class.
        /// </summary>
        /// <param name="q">El cuantificador principal</param>
        public QuantifierEnfasisPlus(Quantifier q, List<RivieraObject> db)
        {
            this.Quantifier = q;
            this.Database = db;
            this.MamparasWithPanels = this.Database.Where(x => (x is Mampara) && x.Children[FIELD_RIGHT_FRONT] != 0 && x.Children[FIELD_LEFT_FRONT] != 0);
            this.Errors = new Dictionary<long, MamparaEnfasisQuantificationError>();

        }
        /// <summary>
        /// Realiza el proceso de cuantifiación para los
        /// elementos exclusivos de la línea enfasis plus, que no se encuentran dibujados
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        public void Quantify(Transaction tr)
        {
            int frente;
            string zoneName;
            long handle;
            IEnumerable<RivieraObject> stackPanelColl;
            foreach (Mampara obj in this.MamparasWithPanels)
                if (this.GetRivieraPanelStacks(obj, out stackPanelColl))
                {
                    try
                    {
                        this.Current = obj;
                        this.ExtractQuantifyData(obj, stackPanelColl.First(), tr, out frente, out zoneName, out handle);
                        IEnumerable<PanelRaw> panels = this.GetPanels(stackPanelColl);
                        IEnumerable<String> codes = this.GetPanelCodesPrefixes(panels);
                        this.QuantifySoportePanelAPiso(tr, panels, frente, zoneName, handle);
                        this.QuantifyCanaletas(tr, stackPanelColl, panels, codes, frente, zoneName, handle);
                        this.QuantifyPlacas(tr, stackPanelColl, zoneName, handle);
                        this.QuantifyMolduras(tr, frente, zoneName, handle);
                        //Este metodo puede arrojar una excepción
                        if (App.Riviera.IsArnesEnabled)
                            this.QuantifySoportesArnes(tr, stackPanelColl, zoneName, handle);
                    }
                    catch (Exception exc)
                    {
                        Dialog_MessageBox.Show(exc.Message, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                        break;
                    }
                }
        }
        /// <summary>
        /// Realizá la cuantificación de paneles dobles
        /// </summary>
        /// <param name="obj">La unión de la mampara</param>
        /// <param name="tr">La transacción activa</param>
        public void QuantifyDoublePanels(List<RivieraObject> objs, MamparaJoint obj, Transaction tr)
        {
            try
            {
                int frente, soportes;
                long biombo;
                string zoneName;
                RivieraPanel front = objs.Where(x => x.Handle.Value == obj.PanelDoubleId).FirstOrDefault() as RivieraPanel;
                this.ExtractQuantifyData(obj, tr, out frente, out zoneName);
                IEnumerable<string> codes = obj.PanelArray.Panels.Select(x => x.Raw.Code.Substring(0, 5)).Distinct();

                try
                {
                    Mampara t;
                    Mampara[] h;
                    obj.GetMamparaInT(out t, out h);
                    Boolean areEqual = t.Code.Substring(8, 2) == h[0].Code.Substring(8, 2) && h[0].Code.Substring(8, 2) == h[1].Code.Substring(8, 2),
                            vIsLarger = int.Parse(t.Code.Substring(8, 2)) > int.Parse(h[0].Code.Substring(8, 2));
                    //Solo se inserta un relleno intermedio de 6 pulgadas si las mamparas no son iguales y cuando las
                    //mamparas horizontales tienen una mayor altura a la de T
                    if (!areEqual && !vIsLarger)
                    {
                        if (obj.PanelArray.DobleBottom != null && obj.PanelArray.DobleBottom.Raw.Code.Substring(6, 4) == "5206")
                            this.Quantifier.AddItem(String.Format("{0}06S", DT_RELLENO_INTERMEDIO_6_IN), 1, zoneName, obj.PanelDoubleId);
                        if (obj.PanelArray.DobleBottom == null && obj.PanelArray.Left != null && obj.PanelArray.DobleFront != null &&
                            obj.PanelArray.DobleFront.Raw.Code.Substring(8, 2) == obj.PanelArray.Left.Raw.Code.Substring(8, 2))
                            this.Quantifier.AddItem(String.Format("{0}06S", DT_RELLENO_INTERMEDIO_6_IN), 1, zoneName, obj.PanelDoubleId);
                    }
                }
                catch (Exception exc)
                {
                    App.Riviera.Log.AppendEntry(exc.Message);
                    Selector.Ed.WriteMessage("\nError al reportar un DD3030, Detalles: {0}", exc.Message);
                }

                //Soportes a piso
                if (front.Raw.APiso)
                {
                    soportes = frente > 54 ? 3 : 2;
                    soportes *= 2;
                    this.Quantifier.AddItem(String.Format("{0}S", DT_SOPORTE_PANELE_A_PISO), soportes, zoneName, obj.PanelDoubleId);
                }
                //Canaletas
                int p = obj.PanelArray.Panels.Count();
                int numOfCanaletas = p == 3 ? 1 : 2,
                    //Si la opción de arneses esta activa se cuantifica una canaleta más. 
                    //Siempre y cuando no exista un gajo de tipo DD203* o DD204*
                    addMore = (App.Riviera.IsArnesEnabled && ElectricPanelPrefixes.Intersect(codes).Count() == 0) ? 0 : 1;
                this.Quantifier.AddItem(String.Format("{0}{1}S", DT_CANALETA, frente), numOfCanaletas + addMore, zoneName, obj.Handle.Value);
                biombo = obj.BiomboId;
                //Moldura Superior
                if (biombo == 0)
                    this.Quantifier.AddItem(String.Format("{0}{1}S", DT_MOLDURA_CIEGA, frente), 1, zoneName, obj.Handle.Value);
                else
                {
                    RivieraPanelBiombo b = App.DB[obj.BiomboId] as RivieraPanelBiombo;
                    if (b != null)
                    {
                        string code = b.Raw.Code.Substring(0, 6);
                        if (BiombosCodes.Contains(code))
                            this.Quantifier.AddItem(String.Format("{0}{1}S", DT_MOLDURA_BIOMBO_INSERTADO, frente), 1, zoneName, obj.Handle.Value);
                        else if (BiombosCodesWithClamps.Contains(code))
                            this.Quantifier.AddItem(String.Format("{0}{1}S", DT_MOLDURA_BIOMBO_CLAMP, frente), 1, zoneName, obj.Handle.Value);
                        else if (PichonerasCodes.Union(CajonerasCodes).Contains(code))
                            this.Quantifier.AddItem(String.Format("{0}{1}S", DT_MOLDURA_PICHONERA_CAJONERA, frente), 1, zoneName, obj.Handle.Value);
                        else
                            this.Quantifier.AddItem(String.Format("{0}{1}S", DT_MOLDURA_CIEGA, frente), 1, zoneName, obj.Handle.Value);
                    }
                    else
                    {
                        var ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
                        ed.WriteMessage("Error al reportar el panel de la unión de paneles dobles con ID" + obj.Handle.Value);
                    }
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }


        #region Cuantificación para Enfasis Plus
        /// <summary>
        /// Realiza el proceso de cuantificación de soportes de paneles a piso
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        /// <param name="panels">La colección de paneles insertados en la mampara actual</param>
        /// <param name="frente">El frente de la mampara</param>
        /// <param name="zoneName">El nombre de la zona</param>
        /// <param name="handle">El handle del arreglo de paneles</param>
        private void QuantifySoportePanelAPiso(Transaction tr, IEnumerable<PanelRaw> panels, int frente, string zoneName, long handle)
        {
            int soportes;
            Boolean hasFloorPanel;
            try
            {
                hasFloorPanel = panels.Count(x => x.APiso) > 0;
                if (hasFloorPanel)
                {
                    soportes = frente > 54 ? 3 : 2;
                    this.Quantifier.AddItem(String.Format("{0}S", DT_SOPORTE_PANELE_A_PISO), soportes, zoneName, handle);
                }
            }
            catch (Exception exc)
            {
                this.HiglightError(exc.Message, this.MethodName, this.Current);
            }
        }
        /// <summary>
        /// Realiza la cuantificación de canaletas de la línea enfasis plus
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        /// <param name="panels">La colección de paneles insertados en la mampara actual</param>
        /// <param name="codes">La colección de prefijos de códigos de paneles</param>
        /// <param name="stackPanelColl">Los arreglos de paneles</param>
        /// <param name="frente">El frente de la mampara</param>
        /// <param name="zoneName">El nombre de la zona</param>
        /// <param name="handle">El handle del arreglo de paneles</param>
        private void QuantifyCanaletas(Transaction tr, IEnumerable<RivieraObject> stackPanelColl, IEnumerable<PanelRaw> panels, IEnumerable<String> codes,
            int frente, string zoneName, long handle)
        {
            int numOfCanaletas, addMore;
            try
            {
                if (MamparasWithDoubleFront.Contains(frente))
                    this.QuantifyCanaletasDoble(tr, frente, zoneName, handle, stackPanelColl);
                else
                {
                    numOfCanaletas = this.CountCanaletas(stackPanelColl);
                    numOfCanaletas--;
                    addMore = 0;
                    //Si la opción de arneses esta activa se cuantifica una canaleta más. 
                    //Siempre y cuando no exista un gajo de tipo DD203* o DD204*
                    if (App.Riviera.IsArnesEnabled && ElectricPanelPrefixes.Intersect(codes).Count() == 0)
                        addMore++;
                    this.Quantifier.AddItem(String.Format("{0}{1}S", DT_CANALETA, frente), numOfCanaletas + addMore, zoneName, handle);
                }
            }
            catch (Exception exc)
            {
                this.HiglightError(exc.Message, this.MethodName, this.Current);
            }
        }
        /// <summary>
        /// Realiza la cuantificación de soportes de la línea enfasis plus
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        /// <param name="stackPanelColl">Los arreglos de paneles</param>
        /// <param name="zoneName">El nombre de la zona</param>
        /// <param name="handle">El handle del arreglo de paneles</param>
        private void QuantifySoportesArnes(Transaction tr, IEnumerable<RivieraObject> stackPanelColl, string zoneName, long handle)
        {
            PanelStackAnalyser analyser = new PanelStackAnalyser(this.Current, (stackPanelColl.First() as RivieraPanelStack), (stackPanelColl.Last() as RivieraPanelStack));
            try
            {
                analyser.QuantifySoportes(this.Quantifier, zoneName, handle);
            }
            catch (Exception exc)
            {
                this.HiglightError(exc.Message, this.MethodName, this.Current);
                throw new System.Exception(exc.Message);
            }
        }
        /// <summary>
        /// Realiza la cuantificación de placas de la línea enfasis plus
        /// </summary>
        /// <param name="stackPanelColl">Los arreglos de paneles</param>
        /// <param name="zoneName">El nombre de la zona</param>
        /// <param name="handle">El handle del arreglo de paneles</param>
        private void QuantifyPlacas(Transaction tr, IEnumerable<RivieraObject> stackPanelColl, string zoneName, long handle)
        {
            int numOfPlacas;
            try
            {
                //Se realizan la selección de gajos de tipo panel electrico
                //DD203* DD204*
                numOfPlacas = this.CountPlacas(stackPanelColl);
                if (numOfPlacas > 0)
                {
                    if (App.Riviera.IsArnesEnabled)
                        this.Quantifier.AddItem(String.Format("{0}S", DT_PLACA_ARNES), numOfPlacas, zoneName, handle);
                    else
                        this.Quantifier.AddItem(String.Format("{0}S", DT_PLACA_SIN_ARNES), numOfPlacas, zoneName, handle);
                }
            }
            catch (Exception exc)
            {
                this.HiglightError(exc.Message, this.MethodName, this.Current);
            }
        }
        /// <summary>
        /// Realiza la cuantificación de molduras de mamparas
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        /// <param name="frente">El frente de la mampara</param>
        /// <param name="zoneName">El nombre de la zona</param>
        /// <param name="handle">El handle del arreglo de paneles</param>
        private void QuantifyMolduras(Transaction tr, int frente, string zoneName, long handle)
        {
            try
            {
                long mamparaHandle = this.Current.Handle.Value;
                RivieraObject biombo = App.DB[this.Current.BiomboId] as RivieraBiombo;
                if (biombo == null)
                    this.Quantifier.AddItem(String.Format("{0}{1}S", DT_MOLDURA_CIEGA, frente), 1, zoneName, mamparaHandle);
                else
                {
                    this.Quantifier.AddItem(biombo, biombo.GetZone(tr));
                    string code = biombo.Code.Substring(0, 6);
                    if (BiombosCodes.Contains(code))
                        this.Quantifier.AddItem(String.Format("{0}{1}S", DT_MOLDURA_BIOMBO_INSERTADO, frente), 1, zoneName, mamparaHandle);
                    else if (BiombosCodesWithClamps.Contains(code))
                        this.Quantifier.AddItem(String.Format("{0}{1}S", DT_MOLDURA_BIOMBO_CLAMP, frente), 1, zoneName, mamparaHandle);
                    else if (PichonerasCodes.Union(CajonerasCodes).Contains(code))
                        this.Quantifier.AddItem(String.Format("{0}{1}S", DT_MOLDURA_PICHONERA_CAJONERA, frente), 1, zoneName, mamparaHandle);
                    else
                        this.Quantifier.AddItem(String.Format("{0}{1}S", DT_MOLDURA_CIEGA, frente), 1, zoneName, mamparaHandle);
                }
            }
            catch (Exception exc)
            {
                this.HiglightError(exc.Message, this.MethodName, this.Current);
            }
        }
        #endregion
        /// <summary>
        /// Realiza la cuantificación de canaletas con frente mayor a 54 de la línea enfasis plus
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        /// <param name="frente">El frente seleccionado</param>
        /// <param name="zoneName">La zona encontrada</param>
        /// <param name="handle">El handle del arreglo de panel</param>
        /// <param name="stackPanelColl">La colección de paneles </param>
        private void QuantifyCanaletasDoble(Transaction tr, int frente, string zoneName, long handle, IEnumerable<RivieraObject> stackPanelColl)
        {
            int canaletas = this.CountCanaletas(stackPanelColl);
            PanelStackAnalyser analyser = new PanelStackAnalyser(frente, stackPanelColl.First() as RivieraPanelStack, stackPanelColl.Last() as RivieraPanelStack);
            analyser.QuantifyCanaletas(this.Quantifier, canaletas, zoneName, handle);
        }
        /// <summary>
        /// Devueleve los arreglos de paneles siempre y cuando la mampara tenga paneles
        /// </summary>
        /// <param name="mampara">La mampara a extraer sus arreglos de paneles</param>
        /// <param name="stackPanels">La salida son los arreglos de paneles</param>
        /// <returns>Los dos arreglos de paneles de una mampara</returns>
        private Boolean GetRivieraPanelStacks(RivieraObject mampara, out IEnumerable<RivieraObject> stackPanels)
        {
            stackPanels = mampara.Children.Values.Select<long, RivieraObject>(x => App.DB[x]).Where(y => y is RivieraPanelStack);
            return stackPanels.Count() > 0;
        }
        /// <summary>
        /// Extrae información de cuantificación tanto de la mampara como del arreglo de paneles
        /// </summary>
        /// <param name="mampara">La mampara</param>
        /// <param name="stackPanel">El arreglo de paneles</param>
        /// <param name="tr">La transacción activa</param>
        /// <param name="frente">El frente de la mampara</param>
        /// <param name="zoneName">La zona a la que pertenecen los paneles</param>
        /// <param name="handle">El handle del arreglo de panel</param>
        private void ExtractQuantifyData(RivieraObject mampara, RivieraObject stackPanel, Transaction tr, out int frente, out String zoneName, out long handle)
        {
            int num;
            frente = int.TryParse(mampara.Code.Substring(6, 2), out num) ? num : 0;
            zoneName = stackPanel.GetZone(tr);
            handle = stackPanel.Handle.Value;
        }
        /// <summary>
        /// Extrae información de cuantificación tanto de la mampara como del arreglo de paneles
        /// </summary>
        /// <param name="mampara">La unión de la mampara</param>
        /// <param name="tr">La transacción activa</param>
        /// <param name="frente">El frente de la mampara</param>
        /// <param name="zoneName">La zona a la que pertenecen los paneles</param>
        private void ExtractQuantifyData(MamparaJoint obj, Transaction tr, out int frente, out string zoneName)
        {
            try
            {
                int num;
                frente = int.TryParse(obj.PanelArray.Left.Raw.Code.Substring(6, 2), out num) ? num : 0;
                if (frente == 18)
                    frente = 40;
                else if (frente == 24)
                    frente = 52;
                zoneName = obj.GetZone(tr);
            }
            catch (Exception exc)
            {
                throw new Exception(String.Format("Error extrayendo la información de cuantificación, de la unión:{0}\n{1}", obj.Handle.Value, exc.Message));
            }

        }
        /// <summary>
        /// Extrae los paneles contenidos en los arreglos de paneles de la mampara
        /// </summary>
        /// <param name="stackPanels">La colección de paneles</param>
        private IEnumerable<PanelRaw> GetPanels(IEnumerable<RivieraObject> stackPanels)
        {
            return (stackPanels.First() as RivieraPanelStack).Collection.Union((stackPanels.Last() as RivieraPanelStack).Collection);
        }
        /// <summary>
        /// Realiza un conteo sin reglas de canaletas
        /// </summary>
        /// <param name="stackPanels">Las canaletas contadas en la aplicación</param>
        /// <returns>El número de canaletas de la mampara</returns>
        private int CountCanaletas(IEnumerable<RivieraObject> stackPanels)
        {
            return (stackPanels.OrderByDescending<RivieraObject, int>(x => (x as RivieraPanelStack).Collection.Count).First() as RivieraPanelStack).Collection.Count;
        }
        private int CountPlacas(IEnumerable<RivieraObject> stackPanels)
        {
            RivieraPanelStack stackA = stackPanels.First() as RivieraPanelStack,
                              stackB = stackPanels.Last() as RivieraPanelStack;
            return stackA.Collection.Union(stackB.Collection).Select<PanelRaw, String>(x => x.Code).Count(y => IsElectric(y));
        }
        /// <summary>
        /// Determina si un código es de panel electrico
        /// </summary>
        /// <param name="code">El código a válidar</param>
        /// <returns>Verdadero si es un panel electrico</returns>
        private Boolean IsElectric(String code)
        {
            string prefix = code.Substring(0, 5);
            return this.ElectricPanelPrefixes.Contains(prefix);
        }
        /// <summary>
        /// Obtiene los prefijos de los códigos de los paneles insertados 
        /// </summary>
        /// <param name="panels">La colección de paneles insertados</param>
        /// <returns>La colección de prefijos de códigos insertados</returns>
        private IEnumerable<string> GetPanelCodesPrefixes(IEnumerable<PanelRaw> panels)
        {
            return panels.Select<PanelRaw, String>(x => x.Code.Substring(0, 5));
        }
        /// <summary>
        /// Realiza un reporte de error en la cuantificación y crea un reporte en la aplicación
        /// </summary>
        /// <param name="message">El mensaje de error de la aplicación</param>
        /// <param name="mampara">La mampara actual</param>
        private void HiglightError(string message, string method, Mampara mampara)
        {
            App.Riviera.Log.AppendEntry(message, Protocol.Error, this);

            if (this.Errors.ContainsKey(mampara.Handle.Value))
            {
                this.Errors[mampara.Handle.Value].ErrorMethods.Add(method);
                this.Errors[mampara.Handle.Value].ExceptionMessage.Add(method, message);
            }
            else
            {
                this.Errors.Add(mampara.Handle.Value, new MamparaEnfasisQuantificationError(mampara, method, message));
                BoundingBox2D box = new BoundingBox2D(mampara.GeometricExtents[0], mampara.GeometricExtents[1]).Scale(1.1);
                Drawer.Geometry2D(box, Autodesk.AutoCAD.Colors.Color.FromRgb(255, 0, 0));
            }
        }
    }

}
