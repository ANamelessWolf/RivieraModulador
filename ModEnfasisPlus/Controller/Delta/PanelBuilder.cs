using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using DaSoft.Riviera.OldModulador.Model;
using DaSoft.Riviera.OldModulador.Model.Delta;
using DaSoft.Riviera.OldModulador.Runtime;
using DaSoft.Riviera.OldModulador.Runtime.Delta;
using DaSoft.Riviera.OldModulador.UI;
using DaSoft.Riviera.OldModulador.UI.Delta;
using NamelessOld.Libraries.HoukagoTeaTime.Mio;
using NamelessOld.Libraries.HoukagoTeaTime.MugiChan;
using NamelessOld.Libraries.HoukagoTeaTime.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Yui;
using System;
using System.Collections.Generic;
using System.Linq;
using static DaSoft.Riviera.OldModulador.Assets.Strings;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CONST;
namespace DaSoft.Riviera.OldModulador.Controller.Delta
{
    public class PanelBuilder
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
        /// Contiene la información de paneles asociados 
        /// al lado izquierdo de la mampara
        /// </summary>
        public List<PanelRaw> StackPanelLeftFrontRaw;
        /// <summary>
        /// Contiene la información de paneles asociados 
        /// al lado derecho de la mampara
        /// </summary>
        public List<PanelRaw> StackPanelRightFrontRaw;
        /// <summary>
        /// La información de biombo de la mampara
        /// </summary>
        public PanelRaw BiomboRaw;
        /// <summary>
        /// Valida si la mampara del constructor tiene paneles
        /// </summary>
        public Boolean HasPanels
        {
            get { return StackPanelRightFrontRaw != null && StackPanelLeftFrontRaw != null; }
        }
        /// <summary>
        /// Valida si la mampara del constructor tiene 
        /// biombos
        /// </summary>
        public Boolean HasBiombo
        {
            get { return BiomboRaw != null; }
        }
        /// <summary>
        /// Creates a new panel builder
        /// </summary>
        /// <param name="source">La mampara principal</param>
        public PanelBuilder(Mampara source)
        {
            if (source != null)
            {
                //Información inicial de los paneles de la mampara
                if (source.Children[FIELD_LEFT_FRONT] != 0 && source.Children[FIELD_RIGHT_FRONT] != 0)
                {
                    RivieraPanelStack stackA = App.DB[source.Children[FIELD_LEFT_FRONT]] as RivieraPanelStack,
                                      stackB = App.DB[source.Children[FIELD_RIGHT_FRONT]] as RivieraPanelStack;
                    this.StackPanelLeftFrontRaw = stackA.Collection;
                    this.StackPanelRightFrontRaw = stackB.Collection;
                }
                //Información inicial del biombo de la mampara
                if (source.BiomboId != 0)
                {
                    RivieraBiombo biombo = App.DB[source.BiomboId] as RivieraBiombo;
                    this.BiomboRaw = biombo.RawData;
                }
            }
        }
        /// <summary>
        /// Realiza el proceso de copiado de paneles
        /// </summary>
        public static void Copy()
        {
            ObjectId sourceId;
            try
            {
                if (Selector.ObjectId(MSG_SEL_PANEL_SOURCE, out sourceId, typeof(BlockReference)))
                {
                    //Se realiza la selección de la mampara base
                    Mampara source = App.DB[sourceId] as Mampara;
                    if (source != null && !source.HasDoublePanels)
                    {
                        PanelBuilder pB = new PanelBuilder(source);
                        IEnumerable<Mampara> mamparas;
                        if (pB.SelectMamparasLike(source, out mamparas))
                        {
                            FastTransactionWrapper trW =
                            new FastTransactionWrapper(delegate (Document doc, Transaction tr)
                            {
                                foreach (Mampara m in mamparas)
                                {
                                    try
                                    {
                                        if (!m.HasDoublePanels)
                                            pB.Copy(m, tr);
                                    }
                                    catch (System.Exception exc)
                                    {
                                        Selector.Ed.WriteMessage(exc.Message);
                                    }
                                }
                            });
                            trW.Run();
                        }
                    }
                    else if (source.HasDoublePanels)
                        Dialog_MessageBox.Show("No se puede copiar el estilo de una mampara asociada a un panel doble.", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                }
            }
            catch (System.Exception exc)
            {
                Selector.Ed.WriteMessage("\n{0}", exc.Message);
            }
        }
        /// <summary>
        /// Realiza el proceso de inserción o edición de paneles
        /// </summary>
        public static void EditPanels()
        {
            ObjectId sourceId,
                     sideAId = new ObjectId(),
                     sideBId = new ObjectId();
            try
            {
                if (Selector.ObjectId(MSG_SEL_PANEL_SOURCE, out sourceId, typeof(BlockReference), typeof(Line)))
                {
                    Entity ent = sourceId.OpenEntity();
                    Mampara source;
                    if (ent is BlockReference)
                        //Se realiza la selección de la mampara base
                        source = App.DB[sourceId] as Mampara;
                    else
                        source = App.DB[ent.Handle.Value] as Mampara;
                    source.DrawSideTags(out sideAId, out sideBId);
                    Selector.Ed.Regen();
                    if (source != null && !source.HasDoublePanels)
                    {
                        PanelBuilder pB = new PanelBuilder(source);
                        PanelAction action;
                        List<PanelRaw> paneles;
                        PanelRaw biombo;
                        pB.SummonPanelEditor(source, out action, out paneles, out biombo);
                        if (action == PanelAction.Create)
                            pB.CreatePaneles(source, paneles, biombo);
                        else if (action == PanelAction.Update)
                            pB.UpdatePaneles(source, paneles, biombo);
                    }
                    else if (source.HasDoublePanels)
                        Dialog_MessageBox.Show("No se puede editar los paneles con esta opción. La mampara pertenece a un arreglo de paneles dobles.\nSeleccione la herramienta de edición para paneles dobles.", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                }

            }
            catch (System.Exception exc)
            {
                Selector.Ed.WriteMessage("\n{0}", exc.Message);
            }
            if (sideAId.IsValid && sideBId.IsValid)
                Drawer.Erase(new ObjectIdCollection(new ObjectId[] { sideAId, sideBId }));
            else if (sideAId.IsValid)
                Drawer.Erase(new ObjectIdCollection(new ObjectId[] { sideAId }));
            else if (sideBId.IsValid)
                Drawer.Erase(new ObjectIdCollection(new ObjectId[] { sideBId }));
        }
        /// <summary>
        /// Realiza el proceso de edición de paneles dobles
        /// </summary>
        public static void EditDoublePanels()
        {
            ObjectId sourceId, sideAId = new ObjectId(), sideBId = new ObjectId(), sideABId = new ObjectId();
            try
            {
                if (Selector.ObjectId(MSG_SEL_DOUBLE_PANEL, out sourceId, typeof(DBText), typeof(Polyline)))
                {
                    Entity ent = sourceId.OpenEntity();
                    var source = App.DB[sourceId] as JointObject;
                    if (source.Type == JointType.Joint_T)
                    {
                        Mampara left, right, top;
                        //Se realiza el proceso de selección de la mampara
                        RivieraPanelDoubleLocation.FindMampara(source, out left, out right, out top);
                        if (left.Code != right.Code)
                        {
                            Dialog_MessageBox.Show(WARNING_PDOBLE_DIFF, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                            return;
                        }
                        else if (!"1824".Contains(left.Code.Substring(6, 2)))
                        {
                            Dialog_MessageBox.Show(WARNING_PDOBLE_INVALID_FRONT, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                            return;
                        }
                        RivieraPanelDoubleLocation location =
                            new RivieraPanelDoubleLocation(RivieraPanelDoubleLocation.PickFrente(left), top, left, right, source);
                        Double f = left.RivSize.Alto == "24" ? 52d : 40d;
                        var paneles = App.DB.PanelsFor(left);
                        var biombos = App.DB.Panel_Size.Where(x => x.Nominal.Frente == f);
                        source.DrawSideTags(location, out sideAId, out sideBId, out sideABId);
                        String[] validUpperCodes = new String[] { "DD2011", "DD2013" };
                        var picker = new Dialog_PanelPicker(source as MamparaJoint, left, right, top,
                            paneles.Where(x => x.CanBeDouble).ToList(),
                            paneles.Where(x => validUpperCodes.Contains(x.Code)).ToList(),
                            biombos.ExtractPanelData(source.Code, f, double.Parse(left.Code.Substring(8, 2))).Where(x => x.Tipo == "B").ToList()
                            );
                        Selector.Ed.Regen();
                        picker.ShowDialog();


                        if (picker.ResultIsOk && (picker.Result == PanelAction.Update || picker.Result == PanelAction.Delete))
                        {
                            if (picker.Result == PanelAction.Update)
                                picker.Result = PanelAction.Create;
                            if (source.PanelArray != null)
                            {
                                new FastTransactionWrapper((Document doc, Transaction tr) =>
                                {
                                    left.ErasePanels(tr);
                                    right.ErasePanels(tr);
                                    source.PanelArray.Clean(source, tr);
                                }).Run();
                                source.PanelArray = null;
                            }
                        }

                        if (picker.ResultIsOk && picker.Result == PanelAction.Create)
                        {
                            source.HasDoublePanels = true;
                            FastTransactionWrapper ft =
                                new FastTransactionWrapper((Document doc, Transaction tr) =>
                                {
                                    left.ErasePanels(tr);
                                    right.ErasePanels(tr);
                                    // location.DrawTags(tr);
                                    if (left.RivSize.Nominal.Frente == 18 && Math.Abs(left.RivSize.Nominal.Alto - top.RivSize.Nominal.Alto) == 6 && left.RivSize.Nominal.Alto > top.RivSize.Nominal.Alto)
                                    {
                                        String code = picker.LeftPanel.Code.Substring(0, 6),
                                               lv = picker.LeftPanel.Nivel;
                                        Double height = App.DB.SelectAlto(picker.LeftPanel.Code.Substring(0, 6), "18", lv);
                                        if (height > 0)
                                        {
                                            PanelRaw leftRaw = picker.LeftPanel.Copy(),
                                                     rightRaw = picker.RightPanel.Copy();
                                            leftRaw.Code = String.Format("{0}{1}{2}", code, "18", height);

                                            rightRaw.Code = String.Format("{0}{1}{2}", code, "18", height);

                                            if (left.RivSize.Nominal.Alto == 54)
                                                source.PanelArray = new RivieraDoublePanelArray()
                                                {
                                                    DobleFront = new RivieraPanel54(picker.LeftMampara, picker.DoublePanel, picker.DoublePanelUpperBack, location),
                                                    Left = new RivieraPanel(picker.LeftMampara, leftRaw, location),
                                                    Right = new RivieraPanel(picker.RightMampara, rightRaw, location),
                                                };
                                            else
                                                source.PanelArray = new RivieraDoublePanelArray()
                                                {
                                                    Left = new RivieraPanel(picker.LeftMampara, leftRaw, location),
                                                    Right = new RivieraPanel(picker.RightMampara, rightRaw, location),
                                                    DobleFront = new RivieraPanel(picker.LeftMampara, picker.DoublePanel, location)
                                                };
                                        }
                                        else
                                        {
                                            if (left.RivSize.Nominal.Alto == 54)
                                                source.PanelArray = new RivieraDoublePanelArray()
                                                {
                                                    DobleFront = new RivieraPanel54(picker.LeftMampara, picker.DoublePanel, picker.DoublePanelUpperBack, location),
                                                    Left = new RivieraPanel(picker.LeftMampara, picker.LeftPanel, location),
                                                    Right = new RivieraPanel(picker.RightMampara, picker.RightPanel, location),
                                                };
                                            else
                                                source.PanelArray = new RivieraDoublePanelArray()
                                                {
                                                    Left = new RivieraPanel(picker.LeftMampara, picker.LeftPanel, location),
                                                    Right = new RivieraPanel(picker.RightMampara, picker.RightPanel, location),
                                                    DobleFront = new RivieraPanel(picker.LeftMampara, picker.DoublePanel, location)
                                                };
                                        }
                                    }
                                    else if (left.RivSize.Nominal.Alto > top.RivSize.Nominal.Alto)
                                    {
                                        if (left.RivSize.Nominal.Alto == 54)
                                            source.PanelArray = new RivieraDoublePanelArray()
                                            {
                                                DobleFront = new RivieraPanel54(picker.LeftMampara, picker.DoublePanel, picker.DoublePanelUpperBack, location),
                                                DobleBottom = new RivieraPanel(picker.LeftMampara, picker.DoublePanelUpperBack, location),
                                                Left = new RivieraPanel(picker.LeftMampara, picker.LeftPanel, location),
                                                Right = new RivieraPanel(picker.RightMampara, picker.RightPanel, location),
                                            };
                                        else
                                            source.PanelArray = new RivieraDoublePanelArray()
                                            {
                                                DobleFront = new RivieraPanel(picker.LeftMampara, picker.DoublePanel, location),
                                                DobleBottom = new RivieraPanel(picker.LeftMampara, picker.DoublePanelUpperBack, location),
                                                Left = new RivieraPanel(picker.LeftMampara, picker.LeftPanel, location),
                                                Right = new RivieraPanel(picker.RightMampara, picker.RightPanel, location),
                                            };
                                    }
                                    else
                                    {
                                        if (left.RivSize.Nominal.Alto == 54)
                                            source.PanelArray = new RivieraDoublePanelArray()
                                            {
                                                DobleFront = new RivieraPanel54(picker.LeftMampara, picker.DoublePanel, picker.DoublePanelUpperBack, location),
                                                Left = new RivieraPanel(picker.LeftMampara, picker.LeftPanel, location),
                                                Right = new RivieraPanel(picker.RightMampara, picker.RightPanel, location),
                                            };
                                        else
                                            source.PanelArray = new RivieraDoublePanelArray()
                                            {
                                                Left = new RivieraPanel(picker.LeftMampara, picker.LeftPanel, location),
                                                Right = new RivieraPanel(picker.RightMampara, picker.RightPanel, location),
                                                DobleFront = new RivieraPanel(picker.LeftMampara, picker.DoublePanel, location)
                                            };
                                    }
                                    if (picker.chBiombo.IsChecked.Value)
                                        source.PanelArray.Biombo = new RivieraPanelBiombo(picker.LeftMampara, picker.DoubleBiombo, location);
                                    source.PanelArray.Draw(tr);
                                    //Guardamos la cache
                                    source.PanelArray.Save(source, tr);
                                    //Se agregan al proyecto
                                    source.PanelArray.Add();
                                });
                            ft.Run();
                        }
                    }
                    else
                    {
                        Dialog_MessageBox.Show(ERR_JUST_T, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                        Selector.Ed.WriteMessage("\n{0}", ERR_JUST_T);
                    }
                }
            }
            catch (Exception exc)
            {
                Selector.Ed.WriteMessage("\n{0}", exc.Message);
            }
            if (sideAId.IsValid)
                Drawer.Erase(new ObjectIdCollection(new ObjectId[] { sideAId
}));
            if (sideBId.IsValid)
                Drawer.Erase(new ObjectIdCollection(new ObjectId[] { sideBId }));
            if (sideABId.IsValid)
                Drawer.Erase(new ObjectIdCollection(new ObjectId[] { sideABId }));
        }

        public static Mampara FindMampara(JointObject joint, Mampara left, Mampara right)
        {
            Object res;
            Mampara result = null;
            if (joint.Parent != 0 && joint.Parent != left.Handle.Value && joint.Parent != right.Handle.Value)
            {
                res = App.DB[joint.Parent];
                if (res is Mampara)
                    result = res as Mampara;
            }
            else
            {
                res = joint.Children.Where(x => x.Value != 0 && x.Value != left.Handle.Value && x.Value != right.Handle.Value).FirstOrDefault();
                if (res != null)
                {
                    res = App.DB[((KeyValuePair<String, long>)res).Value];
                    if (res is Mampara)
                        result = res as Mampara;
                }
            }
            return result;
        }

        /// <summary>
        /// Realiza una copia del contenido actual del constructor
        /// a una mampara seleccionada
        /// </summary>
        /// <param name="mam">La mampara seleccionada</param>
        /// <param name="tr">La transacción activa</param>
        public void Copy(Mampara mam, Transaction tr)
        {
            if (HasPanels)
            {
                PanelBuilder mamPB = new PanelBuilder(mam);
                this.PastePanels(mam, mamPB, tr);
                this.PasteBiombos(mam, mamPB, tr);
            }
        }


        /// <summary>
        /// Realiza el proceso de actualización de paneles y de biombo
        /// </summary>
        /// <param name="source">La mampara base</param>
        /// <param name="paneles">La información de paneles a actualizar</param>
        /// <param name="biombo">La información de biombo a actualizar</param>
        public void UpdatePaneles(Mampara source, List<PanelRaw> paneles, PanelRaw biombo)
        {
            //La mampara A siempre sera el lado de 0° a 180° la Izquierda
            //La mampara B siempre sera el lado de 180° a 0° la Derecha
            RivieraPanelStack stackA = App.DB[source.Children[FIELD_LEFT_FRONT]] as RivieraPanelStack,
                              stackB = App.DB[source.Children[FIELD_RIGHT_FRONT]] as RivieraPanelStack;
            FastTransactionWrapper trW =
                new FastTransactionWrapper(delegate (Document doc, Transaction tr)
                {
                    //Actualización de biombo
                    RivieraBiombo bObj;
                    //La mampara tiene biombo y no hay información de biombo
                    //Se elimina el biombo existente
                    if (HasBiombo && biombo == null)
                    {
                        (App.DB[source.BiomboId] as RivieraBiombo).Delete(tr);
                        source.BiomboId = 0;
                        source.Data.Set(FIELD_EXTRA, tr, source.Extra);
                    }
                    //La mampara no tiene biombo y hay información de biombo
                    //Se crea un nuevo biombo y se agrega a la BD
                    else if (!HasBiombo && biombo != null)
                        CreateBiombo(source, biombo, tr);
                    //La mampara tiene biombo y hay información de biombo
                    //Se actualiza la información de biombo
                    else if (HasBiombo && biombo != null)
                    {
                        bObj = App.DB[source.BiomboId] as RivieraBiombo;
                        //Checamos si el elemento cambio de tipo
                        String currentSuffix = bObj.Code.Substring(0, 6),
                       newSuffix = biombo.Code.Substring(0, 6);
                        Boolean sameFamily =
                            (currentSuffix == newSuffix) ||
                            (PichonerasCodes.Contains(currentSuffix) && PichonerasCodes.Contains(newSuffix)) ||
                            (CajonerasCodes.Contains(currentSuffix) && CajonerasCodes.Contains(newSuffix)) ||
                            (BiombosCodes.Union(BiombosCodesWithClamps).Contains(currentSuffix) && BiombosCodes.Union(BiombosCodesWithClamps).Contains(newSuffix));
                        if (sameFamily && bObj.RawData.Side == biombo.Side)
                        {
                            bObj.Code = biombo.Code;
                            bObj.PanelData = biombo.ToRow;
                            bObj.RawData = biombo;
                            bObj.Acabado = biombo.Acabado;
                            bObj.Regen();
                            bObj.Save(tr);
                        }
                        else
                        {
                            bObj.Delete(tr);
                            CreateBiombo(source, biombo, tr);
                            bObj = App.DB[source.BiomboId] as RivieraBiombo;
                            bObj.Regen();
                            bObj.Save(tr);
                        }
                    }
                    //Actualización de paneles
                    stackA.Collection = paneles.Where(x => x.Direction == ArrowDirection.Left_Front).ToList();
                    stackB.Collection = paneles.Where(x => x.Direction == ArrowDirection.Right_Front).ToList();
                    //Dibujamos el stack
                    stackA.Regen(tr);
                    stackB.Regen(tr);
                    //Guardamos la info del stack
                    stackA.Save(tr);
                    stackB.Save(tr);
                });
            trW.Run();
        }

        /// <summary>
        /// Realiza el proceso de creación de paneles y de biombo
        /// en caso de existir
        /// </summary>
        /// <param name="source">La mampara base</param>
        /// <param name="paneles">La información de paneles a insertar</param>
        /// <param name="biombo">La información de biombo a insertar</param>
        public void CreatePaneles(Mampara source, List<PanelRaw> paneles, PanelRaw biombo)
        {
            //La mampara A siempre sera el lado de 0° a 180° la Izquierda
            //La mampara B siempre sera el lado de 180° a 0° la Derecha
            RivieraPanelStack stackA = new RivieraPanelStack(source, paneles.Where(x => x.Direction == ArrowDirection.Left_Front).ToArray()),
                              stackB = new RivieraPanelStack(source, paneles.Where(x => x.Direction == ArrowDirection.Right_Front).ToArray());
            FastTransactionWrapper trW =
                new FastTransactionWrapper(delegate (Document doc, Transaction tr)
            {
                if (biombo != null)
                    this.CreateBiombo(source, biombo, tr);
                SavePanelStacks(source, stackA, stackB, tr);
            });
            trW.Run();
        }
        /// <summary>
        /// Realiza el proceso de creación de biombo
        /// </summary>
        /// <param name="source">La mampara base</param>
        /// <param name="biombo">La información de biombo a insertar</param>
        public void CreateBiombo(Mampara source, PanelRaw biombo, Transaction tr)
        {
            String code = biombo.Code.Substring(0, 6);
            RivieraBiombo bObj;
            if (PichonerasCodes.Contains(code))
                bObj = new RivieraPichonera(source, biombo.Code, biombo);
            else if (CajonerasCodes.Contains(code))
                bObj = new RivieraCajonera(source, biombo.Code, biombo);
            else
                bObj = new RivieraBiombo(source, biombo.Code, biombo);
            bObj.Acabado = biombo.Acabado;
            SaveBiombo(source, bObj, tr);
        }
        /// <summary>
        /// Invoca al editor de paneles
        /// </summary>
        /// <param name="source">La mampara base</param>
        /// <param name="action">La acción encontrada por el editor de paneles</param>
        /// <param name="paneles">La información de paneles seleccionada</param>
        /// <param name="biombo">La información de biombo seleccionada</param>
        public void SummonPanelEditor(Mampara source, out PanelAction action, out List<PanelRaw> paneles, out PanelRaw biombo)
        {
            List<PanelData> data = App.DB.PanelsFor(source);
            Dialog_PanelEditor dia = new Dialog_PanelEditor(source, data) { RulesType = typeof(MamparaPanelRules) };
            dia.IsInverted = this.CheckRotatedStatus(source);
            dia.ShowDialog();
            paneles = dia.Data;
            biombo = dia.BiomboData;
            action = dia.Result;
        }
        /// <summary>
        /// Checa si la mampara actual ha sido rotada.
        /// </summary>
        /// <param name="source">La mampara a validar.</param>
        /// <returns>Verdadero si la mampara esta rotada</returns>
        private Boolean CheckRotatedStatus(Mampara source)
        {
            return new BlankTransactionWrapper<Boolean>(
                (Document doc, Transaction tr) =>
                {
                    Boolean flag = false;
                    if (source.Line.ExtensionDictionary.IsValid)
                    {
                        DBDictionary dbDic = source.Line.ExtensionDictionary.GetObject(OpenMode.ForRead) as DBDictionary;
                        DManager dMan = new DManager(dbDic);
                        Xrecord xRec;
                        if (dMan.TryGetRegistry(ROTATE_GEOMETRY_XRECORD, out xRec, tr))
                            return xRec.GetDataAsString(tr)[0] != "0";
                    }
                    return flag;
                }).Run();
        }

        /// <summary>
        /// Invoca al editor de paneles
        /// </summary>
        /// <param name="source">La mampara base</param>
        /// <param name="paneles">La información de paneles seleccionada</param>
        /// <param name="biombo">La información de biombo seleccionada</param>
        public Boolean SummonPanelEditor(Mampara source, out List<PanelRaw> paneles, out PanelRaw biombo)
        {
            List<PanelData> data = App.DB.PanelsFor(source);
            Dialog_PanelEditor dia = new Dialog_PanelEditor(source, data) { RulesType = typeof(MamparaPanelRules), IgnoreCloseValidation = true };
            dia.ShowDialog();
            paneles = dia.Data;
            biombo = dia.BiomboData;
            return dia.ResultIsOk;
        }

        /// <summary>
        /// Realiza un proceso de selección de las mamparas que son similares
        /// a una mampara base
        /// </summary>
        /// <param name="mam">La mampara base</param>
        /// <param name="mamparas">La colección de mamparas similares</param>
        /// <returns>Verdadero si seleccióna más de una mampara</returns>
        public Boolean SelectMamparasLike(Mampara mam, out IEnumerable<Mampara> mamparas)
        {
            SelectionFilterBuilder sFB = new SelectionFilterBuilder(typeof(BlockReference));
            ObjectIdCollection mamIds;
            if (Selector.ObjectIds(MSG_SEL_MAMPARA_TO_PASTE, out mamIds, sFB.Filter))
            {
                mamparas = mamIds.OfType<ObjectId>().
                    Where(x => App.DB[x] != null && App.DB[x] is Mampara).
                    Select<ObjectId, Mampara>(y => App.DB[y] as Mampara).
                    Where(z => z.Frente == mam.Frente && z.Alto == mam.Alto);
            }
            else
                mamparas = null;
            return mamparas != null && mamparas.Count() > 0;
        }

        /// <summary>
        /// Copia el biombo a una mampara existente
        /// </summary>
        /// <param name="mam">La mampara a pegarle el biombo</param>
        /// <param name="mamPB">El constructor de mampara para pegar los biombos</param>
        /// <param name="tr">La transacción activa</param>
        private void PasteBiombos(Mampara mam, PanelBuilder mamPB, Transaction tr)
        {
            RivieraBiombo biombo;
            //La mampara base no tiene biombo y la mampara a pegarle si tiene biombo
            //Se elimina el biombo
            if (!HasBiombo && mam.BiomboId != 0)
            {
                //Borrar tambien la línea
                biombo = App.DB[mam.BiomboId] as RivieraBiombo;
                biombo.Delete(tr);
                mam.BiomboId = 0;
                mam.Data.Set(FIELD_EXTRA, tr, mam.Extra);
            }
            //La mampara base tiene biombo y la mampara a pegarle no tiene biombo
            //Se crea un nuevo biombo y se agrega a la BD
            else if (HasBiombo && mam.BiomboId == 0)
            {
                String code = this.BiomboRaw.Code.Substring(0, 6);
                if (PichonerasCodes.Contains(code))
                    biombo = new RivieraPichonera(mam, this.BiomboRaw.Code, this.BiomboRaw.Copy());
                else if (CajonerasCodes.Contains(code))
                    biombo = new RivieraCajonera(mam, this.BiomboRaw.Code, this.BiomboRaw.Copy());
                else
                    biombo = new RivieraBiombo(mam, this.BiomboRaw.Code, this.BiomboRaw.Copy());
                biombo.Acabado = this.BiomboRaw.Acabado;
                SaveBiombo(mam, biombo, tr);
            }
            //La mampara base tiene biombo y la mampara a pegarle tambien
            //Se actualiza la información de biombo
            else if (HasBiombo && mam.BiomboId != 0)
                UpdateBiombo(mam, App.DB[mam.BiomboId] as RivieraBiombo, this.BiomboRaw, tr);
        }

        private void UpdateBiombo(Mampara source, RivieraBiombo bObj, PanelRaw biombo, Transaction tr)
        {
            //Checamos si el elemento cambio de tipo
            String currentSuffix = bObj.Code.Substring(0, 6),
                   newSuffix = biombo.Code.Substring(0, 6);
            Boolean sameFamily =
                (currentSuffix == newSuffix) ||
                (PichonerasCodes.Contains(currentSuffix) && PichonerasCodes.Contains(newSuffix)) ||
                (CajonerasCodes.Contains(currentSuffix) && CajonerasCodes.Contains(newSuffix)) ||
                (BiombosCodes.Union(BiombosCodesWithClamps).Contains(currentSuffix) && BiombosCodes.Union(BiombosCodesWithClamps).Contains(newSuffix));
            if (sameFamily)
            {
                bObj.Code = biombo.Code;
                bObj.RawData = biombo.Copy();
                bObj.Acabado = biombo.Acabado;
                bObj.Regen();
                bObj.Save(tr);
            }
            else
            {
                bObj.Delete(tr);
                CreateBiombo(source, biombo, tr);
                bObj = App.DB[source.BiomboId] as RivieraBiombo;
                bObj.Regen();
                bObj.Save(tr);
            }
        }



        /// <summary>
        /// Copia de los paneles a una mampara existente
        /// </summary>
        /// <param name="mam">La mampara a pegarle los paneles</param>
        /// <param name="mamPB">El constructor de la mampara a pegarle los paneles</param>
        /// <param name="tr">La transacción activa</param>
        private void PastePanels(Mampara mam, PanelBuilder mamPB, Transaction tr)
        {
            RivieraPanelStack stackA, stackB;
            //Actualizar información de panel
            if (mamPB.HasPanels)
            {
                stackA = App.DB[mam.Children[FIELD_LEFT_FRONT]] as RivieraPanelStack;
                stackB = App.DB[mam.Children[FIELD_RIGHT_FRONT]] as RivieraPanelStack;
                stackA.Collection = this.StackPanelLeftFrontRaw.Select<PanelRaw, PanelRaw>(x => x.Copy()).ToList();
                stackB.Collection = this.StackPanelRightFrontRaw.Select<PanelRaw, PanelRaw>(x => x.Copy()).ToList();
                //Actualizamos el dibujo
                stackA.Regen(tr);
                stackB.Regen(tr);
                stackA.Save(tr);
                stackB.Save(tr);
            }
            else
            {
                stackA = new RivieraPanelStack(mam, this.StackPanelLeftFrontRaw.Select<PanelRaw, PanelRaw>(x => x.Copy()).ToArray());
                stackB = new RivieraPanelStack(mam, this.StackPanelRightFrontRaw.Select<PanelRaw, PanelRaw>(x => x.Copy()).ToArray());
                this.SavePanelStacks(mam, stackA, stackB, tr);
            }

        }
        /// <summary>
        /// Agrega los paneles a una mampara inserteda
        /// </summary>
        /// <param name="mam">La mampara a insertar los paneles</param>
        public static void AddPaneles(Mampara mam)
        {
            PanelBuilder pB = new PanelBuilder(mam);
            if (Ctrl_Mampara.MamparaTemplate != null && Ctrl_Mampara.MamparaTemplate.IsSimilateTo(mam))
                pB.CreatePaneles(mam, Ctrl_Mampara.StackPanelTemplate, Ctrl_Mampara.BiomboTemplate);
        }

        /// <summary>
        /// Realiza el proceso de guardado y de dibujado de los
        /// paneles asociados a una mampara
        /// </summary>
        /// <param name="mam">La mampara a la que se le asocian los paneles</param>
        /// <param name="stackA">La colección de paneles asociados al lado izquierdo de la mampara</param>
        /// <param name="stackB">La colección de paneles asociados al lado derecho de la mampara</param>
        /// <param name="tr">La transacción activa</param>
        private void SavePanelStacks(Mampara mam, RivieraPanelStack stackA, RivieraPanelStack stackB, Transaction tr)
        {
            //Dibujamos el stack
            stackA.Draw(tr, null);
            stackB.Draw(tr, null);
            //Guardamos la cache
            mam.Children[FIELD_LEFT_FRONT] = stackA.Handle.Value;
            mam.Children[FIELD_RIGHT_FRONT] = stackB.Handle.Value;
            stackA.Parent = mam.Handle.Value;
            stackB.Parent = mam.Handle.Value;
            //Se agregan al proyecto
            App.DB.Objects.Add(stackA);
            App.DB.Objects.Add(stackB);
            //Guardamos en memoria
            mam.Data.Set(FIELD_LEFT_FRONT, tr, stackA.Handle.Value.ToString());
            mam.Data.Set(FIELD_RIGHT_FRONT, tr, stackB.Handle.Value.ToString());
            stackA.Save(tr);
            stackB.Save(tr);
        }
        /// <summary>
        /// Realiza el proceso de guardado y de dibujado del
        /// biombo asociado a una mampara
        /// </summary>
        /// <param name="mam">La mampara a la que se le asocia el biombo</param>
        /// <param name="RivieraBiombo">El biombo asociado a la mampara</param>
        /// <param name="tr">La transacción activa</param>
        private void SaveBiombo(Mampara mam, RivieraBiombo biombo, Transaction tr)
        {
            //Dibujar
            biombo.Draw(tr, null);
            //Guardamos la cache
            //asociamos los apuntadores
            biombo.Parent = mam.Handle.Value;
            mam.BiomboId = biombo.Handle.Value;
            //Se agrega el biombo en el proyecto
            App.DB.Objects.Add(biombo);
            //Guardamos en memoria
            biombo.Save(tr);
            mam.Data.Set(FIELD_EXTRA, tr, mam.Extra);
        }
    }
}
