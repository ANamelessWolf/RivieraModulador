using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.OldModulador.Model;
using DaSoft.Riviera.OldModulador.Model.Delta;
using DaSoft.Riviera.OldModulador.Runtime;
using DaSoft.Riviera.OldModulador.UI;
using NamelessOld.Libraries.HoukagoTeaTime.Mio;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Entities;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Model;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu.Shapes2D;
using NamelessOld.Libraries.HoukagoTeaTime.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Yui;
using System;
using System.Collections.Generic;
using System.Linq;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CODE;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CONST;
using static DaSoft.Riviera.OldModulador.Assets.Strings;
namespace DaSoft.Riviera.OldModulador.Controller
{
    public class Quantifier
    {
        public Dictionary<long, TreatmentUnion> InvalidUnions;
        /// <summary>
        /// Cuantificación de elementos gráficos del dibujo.
        /// </summary>
        public List<QuantifiableObject> ItemQuantification;
        /// <summary>
        /// Cuantificación de elementos por uniones
        /// </summary>
        public List<QuantifiableUnion> UnionQuantification;
        /// <summary>
        /// Cuantificación de modulos
        /// </summary>
        public List<QuantifiableModule> ModuleQuantification;

        public List<RivieraObject> Erroneous_Items;

        /// <summary>
        /// Los elementos gráficos que cuantifican
        /// </summary>
        Type[] AllowedGraphicItems = new Type[]
        {
            typeof(Mampara),
            typeof(MamparaRemateFinal),
            typeof(RivieraPanel),
            typeof(RivieraPanel54),
            typeof(RivieraPanelBiombo),
        };


        /// <summary>
        /// Inicia el proceso de cuantificación
        /// </summary>
        public Quantifier()
        {
            this.ItemQuantification = new List<QuantifiableObject>();
            this.UnionQuantification = new List<QuantifiableUnion>();
            this.ModuleQuantification = new List<QuantifiableModule>();
            this.Erroneous_Items = new List<RivieraObject>();
            this.InvalidUnions = new Dictionary<long, TreatmentUnion>();
            AutoCADLayer layer = new AutoCADLayer(LAYER_UNION);
            layer.Clear();
            FastTransactionWrapper ft = new FastTransactionWrapper(
                delegate (Document doc, Transaction tr)
                {

                    //Cuantificación de elementos
                    foreach (RivieraObject obj in App.DB.Objects)
                    {
                        try
                        {
                            if (this.IsQuantifiableData(obj))
                                this.AddItem(obj, obj.GetZone(tr));
                        }
                        catch (Exception exc)
                        {
                            Selector.Ed.WriteMessage(exc.Message);
                            this.Erroneous_Items.Add(obj);
                        }
                    }
                    //Cuantificación por uniones
                    foreach (JointObject obj in App.DB.Objects.Where(x => x is JointObject))
                    {
                        try
                        {
                            this.AddUnionItem(obj, obj.GetZone(tr));
                        }
                        catch (Exception exc)
                        {
                            Selector.Ed.WriteMessage(exc.Message);
                            this.Erroneous_Items.Add(obj);
                        }

                    }
                    //Cuantificación de pilas de paneles
                    foreach (RivieraPanelStack obj in App.DB.Objects.Where(x => x is RivieraPanelStack))
                    {
                        try
                        {
                            if (obj.Collection != null)
                                foreach (PanelRaw raw in obj.Collection)
                                    this.AddItem(raw, obj.GetZone(tr), obj.Handle.Value);
                        }
                        catch (Exception exc)
                        {
                            Selector.Ed.WriteMessage(exc.Message);
                            this.Erroneous_Items.Add(obj);
                        }
                    }
                    //Cuantificación de modulos
                    QuantifyModules(tr, doc);
                    //Cuantificación de elementos no gráficos de enfasis plus
                    QuantifierEnfasisPlus qE = new QuantifierEnfasisPlus(this, App.DB.Objects);
                    qE.Quantify(tr);
                    //Cuantificación para paneles dobles
                    foreach (MamparaJoint obj in App.DB.Objects.Where(x => x is MamparaJoint && (x as MamparaJoint).Type == JointType.Joint_T && (x as MamparaJoint).HasDoublePanels))
                    {
                        try
                        {
                            qE.QuantifyDoublePanels(App.DB.Objects, obj, tr);
                        }
                        catch (Exception exc)
                        {
                            Selector.Ed.WriteMessage(exc.Message);
                            this.Erroneous_Items.Add(obj);
                        }

                    }
                    if (this.InvalidUnions.Count > 0)
                    {
                        Dialog_MessageBox.Show(WARNING_X_BAD_UNION, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                        BoundingBox2D box;
                        Point2d min, max;
                        foreach (var xJoint in this.InvalidUnions)
                        {
                            var ents = xJoint.Value.Entities.Select(x => x.GetId().OpenEntity(tr));
                            new Point2dCollection(ents.Select(x => x.GeometricExtents.MinPoint.ToPoint2d()).Union(ents.Select(y => y.GeometricExtents.MaxPoint.ToPoint2d())).ToArray()).GeometricExtents(out min, out max);
                            box = new BoundingBox2D(min, max);
                            ObjectId id = Drawer.Geometry2D(box, Color.FromRgb(255, 0, 0), tr, closed: true);
                            //(id.GetObject(OpenMode.ForWrite) as Entity).Layer = LAYER_ERR;
                        }
                    }
                });
            ft.Run();
        }


        /// <summary>
        /// Inicia el proceso de cuantificación
        /// </summary>
        public Quantifier(List<RivieraObject> data, Transaction tr)
        {
            this.ItemQuantification = new List<QuantifiableObject>();
            this.UnionQuantification = new List<QuantifiableUnion>();
            this.InvalidUnions = new Dictionary<long, TreatmentUnion>();
            AutoCADLayer layer = new AutoCADLayer(LAYER_UNION);
            layer.Clear();
            //Cuantificación de elementos
            foreach (RivieraObject obj in data)
                if (this.IsQuantifiableData(obj))
                    this.AddItem(obj, obj.GetZone(tr));
            //Cuantificación por uniones
            foreach (JointObject obj in data.Where(x => x is JointObject))
                this.AddUnionItem(obj, obj.GetZone(tr));
            //Cuantificación de pilas de paneles
            foreach (RivieraPanelStack obj in data.Where(x => x is RivieraPanelStack))
                if (obj.Collection != null)
                    foreach (PanelRaw raw in obj.Collection.Where(x => x.Code != DT_PANEL_NULL))
                        this.AddItem(raw, obj.GetZone(tr), obj.Handle.Value);
            //Cuantificación de elementos no gráficos de enfasis plus
            QuantifierEnfasisPlus qE = new QuantifierEnfasisPlus(this, data);
            qE.Quantify(tr);
            //Cuantificación para paneles dobles
            foreach (MamparaJoint obj in data.Where(x => x is MamparaJoint && (x as MamparaJoint).Type == JointType.Joint_T && (x as MamparaJoint).HasDoublePanels))
            {
                try
                {
                    qE.QuantifyDoublePanels(data, obj, tr);
                }
                catch (Exception exc)
                {
                    Selector.Ed.WriteMessage(exc.Message);
                    this.Erroneous_Items.Add(obj);
                }

            }
            if (this.InvalidUnions.Count > 0)
            {
                Dialog_MessageBox.Show(WARNING_X_BAD_UNION, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                BoundingBox2D box;
                foreach (var xJoint in this.InvalidUnions)
                {
                    var ents = xJoint.Value.Entities.Select(x => x.GetId().OpenEntity(tr));
                    box = new BoundingBox2D(ents.Select(x => x.GeometricExtents.MinPoint).Min().ToPoint2d(), ents.Select(x => x.GeometricExtents.MinPoint).Max().ToPoint2d());
                    ObjectId id = Drawer.Geometry2D(box, Color.FromRgb(255, 0, 0), tr, closed: true);
                    (id.GetObject(OpenMode.ForWrite) as Entity).Layer = LAYER_ERR;
                }
            }
        }
        /// <summary>
        /// Agregá un elemento a la lista de elementos cuantificables
        /// de tipo union
        /// </summary>
        /// <param name="zoneName">El nombre de la zona</param>
        /// <param name="obj">El objeto a cuantificar</param>
        private void AddUnionItem(JointObject obj, String zoneName)
        {
            int index = -1;
            TratamientoUnion treatment;
            List<long> ents;
            RuleQuantification rule = new RuleQuantification(obj, zoneName);
            rule.Quantify();
            if (rule.IsNotValid(out treatment, out ents))
                InvalidUnions.Add(obj.Handle.Value, new TreatmentUnion() { Treatment = treatment, Union = rule.Rules[0].UnionName, Entities = ents });
            //Agrega los elemenentos a la cuantificación actual
            rule.QuantificationInfo.Members.Sort();
            for (int i = 0; i < this.UnionQuantification.Count && index == -1; i++)
                if (rule.QuantificationInfo.EqualtTo(this.UnionQuantification[i]))
                    index = i;
            rule.QuantificationInfo.Handle = obj.Handle.Value;
            //if (index == -1)
            this.UnionQuantification.Add(rule.QuantificationInfo);
            //else
            //this.UnionQuantification[index].Count += 1;
        }

        /// <summary>
        /// Realiza el proceso de cuantificación de modulos
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        /// <param name="doc">El documento actual</param>
        private void QuantifyModules(Transaction tr, Document doc)
        {
            BlockTableRecord model = (BlockTableRecord)doc.Database.CurrentSpaceId.GetObject(OpenMode.ForRead);
            foreach (ObjectId id in model)
            {
                try
                {
                    DBObject obj = id.GetObject(OpenMode.ForRead);
                    if (obj is BlockReference)
                    {
                        String name = (obj as BlockReference).Name;
                        if (name.Length > 4 && name.Substring(0, 4) == "MOD_")
                        {
                            this.ModuleQuantification.Add(
                                new QuantifiableModule()
                                {
                                    Block = (obj as BlockReference),
                                    Code = name.Substring(4),
                                    Count = 1,
                                    Handle = (obj as BlockReference).Handle.Value,
                                    Visibility = true,
                                    Zone = (obj as BlockReference).GetZone(tr)
                                });
                        }
                    }
                }
                catch (Exception exc)
                {
                    App.Riviera.Log.AppendEntry(exc.Message, NamelessOld.Libraries.Yggdrasil.Lain.Protocol.Error, "QuantifyModules", "Quantification");
                }
            }
        }

        /// <summary>
        /// Agregá un elemento a la lista de elementos cuantificables
        /// </summary>
        /// <param name="zone">La zona del elemento</param>
        /// <param name="obj">El objeto a cuantificar</param>
        public void AddItem(RivieraObject obj, String zone)
        {
            String code;
            if (obj is Mampara && FantasmaQuantifier.CheckFantasma(obj, out code))
            {
                FantasmaQuantifier fQ = new FantasmaQuantifier(code, obj, zone);
                foreach (QuantifiableObject item in fQ.Quantify())
                    this.ItemQuantification.Add(item);
            }
            else if (obj is RivieraPanel54)
            {
                this.ItemQuantification.Add(new QuantifiableObject() { Code = (obj as RivieraPanel54).LowerRaw.Code.AddAcabado((obj as RivieraPanel54).LowerRaw), Count = 1, Visibility = true, Zone = zone, Handle = obj.Handle.Value });
                this.ItemQuantification.Add(new QuantifiableObject() { Code = (obj as RivieraPanel54).UpperRaw.Code.AddAcabado((obj as RivieraPanel54).UpperRaw), Count = 1, Visibility = true, Zone = zone, Handle = obj.Handle.Value });
            }
            else if (obj is RivieraPanelBiombo)
                this.ItemQuantification.Add(new QuantifiableObject() { Code = (obj as RivieraPanelBiombo).Raw.Code.AddAcabado((obj as RivieraPanelBiombo).Raw), Count = 1, Visibility = true, Zone = zone, Handle = obj.Handle.Value });
            else if (obj is RivieraPanel)
            {
                //No hay paneles de 6" se deben reportar dos paneles con frente de 24 o 18
                if ((obj as RivieraPanel).Raw.Code.Substring(8, 2) == "06")
                {
                    int frente = (obj as RivieraPanel).Raw.Code.Substring(6, 2) == "52" ? 24 : 18;
                    string c = (obj as RivieraPanel).Raw.Code.Substring(0, 6);
                    this.ItemQuantification.Add(new QuantifiableObject() { Code = String.Format("{0}{1}{2}", c, frente, "06").AddAcabado((obj as RivieraPanel).Raw), Count = 2, Visibility = true, Zone = zone, Handle = obj.Handle.Value });
                    this.ItemQuantification.Add(new QuantifiableObject() { Code = String.Format("{0}{1}", DT_RELLENO_INTERMEDIO, "06").AddAcabado((obj as RivieraPanel).Raw), Count = 1, Visibility = true, Zone = zone, Handle = obj.Handle.Value });
                }
                else
                    this.ItemQuantification.Add(new QuantifiableObject() { Code = (obj as RivieraPanel).Raw.Code.AddAcabado((obj as RivieraPanel).Raw), Count = 1, Visibility = true, Zone = zone, Handle = obj.Handle.Value });
            }
            else
                this.ItemQuantification.Add(new QuantifiableObject() { Code = obj.Code.AddAcabado(obj), Count = 1, Visibility = true, Zone = zone, Handle = obj.Handle.Value });
        }
        /// <summary>
        /// Agregá un elemento a la lista de elementos cuantificables
        /// </summary>
        /// <param name="zone">La zona del elemento</param>
        /// <param name="code">El código del elemento a cuantificar</param>
        /// <param name="count">El total de elementos a cuantificar</param>
        public void AddItem(String code, int count, String zone, long handle)
        {
            //Cuantificar en Grupo
            this.ItemQuantification.Add(new QuantifiableObject() { Code = code, Count = count, Visibility = true, Zone = zone, Handle = handle });
        }
        /// <summary>
        /// Agregá un elemento a la lista de elementos cuantificables
        /// </summary>
        /// <param name="raw">El panel a cuantificar</param>
        /// <param name="zone">La zona del elemento</param>
        public void AddItem(PanelRaw raw, String zone, long handle)
        {
            //Se omiten los fantasmas
            if (!raw.Code.Contains("DD0000"))
                this.ItemQuantification.Add(new QuantifiableObject() { Code = raw.Code.AddAcabado(raw), Count = 1, Visibility = true, Zone = zone, Handle = handle });
        }
        /// <summary>
        /// Valida que un elemento sea cuantificable
        /// </summary>
        /// <param name="obj">El objeto a cuantificar</param>
        /// <returns>Verdadero si el elemento es cuantificable</returns>
        private bool IsQuantifiableData(RivieraObject obj)
        {
            return this.AllowedGraphicItems.Contains(obj.GetType());
        }
    }
}
