using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.OldModulador.Controller;
using DaSoft.Riviera.OldModulador.Controller.Delta;
using DaSoft.Riviera.OldModulador.Model;
using DaSoft.Riviera.OldModulador.Model.Delta;
using DaSoft.Riviera.OldModulador.Query;
using NamelessOld.Libraries.DB.Misa;
using NamelessOld.Libraries.DB.Misa.Model;
using NamelessOld.Libraries.HoukagoTeaTime.Mio;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Entities;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Model;
using NamelessOld.Libraries.HoukagoTeaTime.MugiChan;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu;
using NamelessOld.Libraries.HoukagoTeaTime.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Yui;
using NamelessOld.Libraries.Yggdrasil.Lain;
using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CODE;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CONST;
using static DaSoft.Riviera.OldModulador.Assets.Strings;
using static DaSoft.Riviera.OldModulador.Runtime.CMD;
namespace DaSoft.Riviera.OldModulador.Runtime
{
    public class Database : NamelessObject
    {
        public List<long> ErrorHandles;
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
        /// Contiene la carga de tamaños de la mampara
        /// </summary>
        public List<MamparaSize> Mampara_Sizes;
        /// <summary>
        /// Contiene la carga de tamaños de paneles
        /// </summary>
        public List<PanelSize> Panel_Size;
        /// <summary>
        /// La descripción de códigos
        /// </summary>
        public List<RivieraCode> Description;
        /// <summary>
        /// La colección de acabados de la aplicación
        /// </summary>
        public List<RivieraAcabado> Acabados;
        /// <summary>
        /// La relación existente entre alturas y niveles
        /// </summary>
        public List<AltoNivel> Alto_Nivel;
        /// <summary>
        /// La lista de objetos cargados en la aplicación.
        /// </summary>
        public List<RivieraObject> Objects;
        /// <summary>
        /// Reglas de validación para uniones, define las reglas y los tipos de tratamientos que se usan para el tipo de reglas seleccionadas.
        /// </summary>
        public Dictionary<String, TratamientoUnion> TratamientosParaUniones;
        /// <summary>
        /// La lista de restricción de panels
        /// </summary>
        public List<RivieraPanelLevelRestriction> Panel_Restriction;

        /// <summary>
        /// Define la ubicación de las mamparas dobles
        /// </summary>
        public Dictionary<long, RivieraPanelDoubleLocation> JointDoubleLocations;

        /// <summary>
        /// La lista de reglas de cuantificación de uniones
        /// Cuantificación de códigos asociados
        /// </summary>
        public List<MamparaUnionRule> MamparaUnionRules;
        /// <summary>
        /// Define la lista de remates finales encontrados en la aplicación
        /// </summary>
        List<RivieraData> RemateFinalData, PanelesData, BiomboData, PItems;
        /// <summary>
        /// Accede a un objeto de la aplicación por su handle
        /// </summary>
        /// <param name="handleValue">El handle del objeto</param>
        /// <returns>El riviera object</returns>
        public RivieraObject this[long handleValue]
        {
            get
            {
                return GetRivieraObject(this.Objects, handleValue);
            }
        }
        /// <summary>
        /// Accede a un objeto de la aplicación por su handle
        /// </summary>
        /// <param name="handleValue">El handle del objeto</param>
        /// <returns>El riviera object</returns>
        public static RivieraObject GetRivieraObject(List<RivieraObject> db, long handleValue)
        {
            return db != null ? db.Where(x => x.Handle.Value == handleValue).FirstOrDefault() : null;
        }

        /// <summary>
        /// Accede a un objeto de la aplicación por ObjectId de
        /// su geometría
        /// </summary>
        /// <param name="id">EL object id de la geometría</param>
        /// <returns>El riviera object</returns>
        public RivieraObject this[ObjectId id]
        {
            get
            {
                return this.Objects != null ? this.Objects.Where(x => x.Ids.Contains(id)).FirstOrDefault() : null;
            }
        }

        /// <summary>
        /// Crea una nueva BD
        /// </summary>
        public Database()
        {
            this.Mampara_Sizes = new List<MamparaSize>();
            this.Panel_Size = new List<PanelSize>();
            this.RemateFinalData = new List<RivieraData>();
            this.PanelesData = new List<RivieraData>();
            this.PItems = new List<RivieraData>();
            this.JointDoubleLocations = new Dictionary<long, RivieraPanelDoubleLocation>();
            this.BiomboData = new List<RivieraData>();
            this.Alto_Nivel = new List<AltoNivel>();
            this.Description = new List<RivieraCode>();
            this.Acabados = new List<RivieraAcabado>();
            this.Panel_Restriction = new List<RivieraPanelLevelRestriction>();
            this.MamparaUnionRules = new List<MamparaUnionRule>();
            this.TratamientosParaUniones = new Dictionary<string, TratamientoUnion>();
        }
        /// <summary>
        /// Realiza el proceso de carga de la BD
        /// </summary>
        public void Init(Boolean loadUI = true)
        {
            RemateFinalData.Clear();
            PanelesData.Clear();
            BiomboData.Clear();
            PItems.Clear();
            JointDoubleLocations.Clear();
            OracleTask task = new OracleTask();
            if (loadUI)
                task.TaskIsFinished += TaskIsFinished;
            else
                task.TaskIsFinished += TaskIsFinishedNOUI;
            Oracle_Transaction<Object, Object> tr = new Oracle_Transaction<Object, Object>(App.Riviera.Connection, InitMemory);
            tr.RunBGWorker(task);
        }
        /// <summary>
        /// Elimina un elemento de la BD y lo borra del dibujo
        /// </summary>
        /// <param name="ids">La lista de ids a remover</param>
        public void DeleteByIds(ObjectIdCollection ids)
        {
            FastTransactionWrapper trW = new FastTransactionWrapper(
                delegate (Document doc, Transaction tr)
                {
                    //IEnumerable<Entity> ents = ids.OfType<ObjectId>().Select<ObjectId, Entity>(x => x.GetObject(OpenMode.ForRead) as Entity);
                    foreach (ObjectId id in ids)
                    {
                        RivieraObject obj = this[id];
                        if (obj != null)
                            obj.Delete(tr);
                    }
                });
            trW.Run();
        }
        /// <summary>
        /// Elimina un elemento de la BD
        /// </summary>
        /// <param name="value">El handle del elemento a eliminar</param>
        public void Remove(long value)
        {
            int index = -1;
            for (int i = 0; i < this.Objects.Count; i++)
                if (this.Objects[i].Handle.Value == value)
                    index = i;
            if (index != -1)
                this.Objects.RemoveAt(index);
        }

        /// <summary>
        /// Realiza la carga del dibujo actual
        /// </summary>
        public void LoadDrawingData()
        {
            this.Objects = new List<RivieraObject>();
            this.ErrorHandles = new List<long>();
            BlankTransactionWrapper<Object> trw =
                new BlankTransactionWrapper<Object>(
                    delegate (Document doc, Transaction tr)
                    {
                        List<RivieraObject> data = new List<RivieraObject>();
                        BlockTable tab = (BlockTable)doc.Database.BlockTableId.GetObject(OpenMode.ForRead);
                        BlockTableRecord model = (BlockTableRecord)tab[BlockTableRecord.ModelSpace].GetObject(OpenMode.ForRead);
                        //La lista entidades
                        IEnumerable<Entity> rivEnts =
                                    model.OfType<ObjectId>().Select<ObjectId, DBObject>(x => x.GetObject(OpenMode.ForRead)).
                                    Where(y => y is Entity && (y as Entity).Layer == LAYER_RIVIERA_OBJECT).Select<DBObject, Entity>(z => z as Entity);
                        RivieraData dataReader;
                        String code;
                        RivieraObject rObj;
                        //Se carga la información de las entidades
                        foreach (Entity ent in rivEnts)
                        {
                            try
                            {
                                //La información de mamparas, marcos se guardan en líneas
                                if (ent is Line && ent.ExtensionDictionary.IsValid && HasCode(ent, tr))
                                {
                                    dataReader = new RivieraData(ent.Id, tr);
                                    code = dataReader[FIELD_CODE, tr];
                                    rObj = GetRivieraObject(code, dataReader, tr);
                                    if (rObj != null)
                                        data.Add(rObj);
                                }
                            }
                            catch (Exception exc)
                            {
                                this.ErrorHandles.Add(ent.Handle.Value);
                                App.Riviera.Log.AppendEntry(String.Format("Error al cargar entidad {0}\n{1}", ent.Handle.Value, exc.Message), Protocol.Error, this);
                            }
                        }
                        LoadMamparasDependencies(data, tr);
                        Object[] res = new Object[]
                        {
                                data
                        };
                        return res;
                    });
            Object[] result = trw.Run() as Object[];
            this.Objects = result[0] as List<RivieraObject>;
        }

        private bool HasCode(Entity ent, Transaction tr)
        {
            ExtensionDictionaryManager DMan = new ExtensionDictionaryManager(ent.Id, tr);
            Xrecord rec;
            return DMan.TryGetRegistry(FIELD_CODE, out rec, tr);
        }

        private void LoadMamparasDependencies(List<RivieraObject> data, Transaction tr)
        {
            String[] rd;
            long handle = 0;
            PanelRaw[] panels;
            RivieraObject rObj, mObj;
            //Se cargan los remates al final por que requieren de que las mamparas hayan sido cargadas
            foreach (RivieraData rDataReader in this.RemateFinalData)
            {
                try
                {
                    rd = rDataReader.Extract(tr);
                    handle = long.Parse(rd[10]);
                    mObj = data.Where(x => x.Handle.Value == handle).FirstOrDefault();
                    if (mObj != null && mObj is Mampara)
                    {
                        rObj = new MamparaRemateFinal(mObj as Mampara, rd[2].ParseAsPoint(), rd[3].ParseAsPoint(), rd[1].ParseAsSize(), rd[0]);
                        rObj.Data = rDataReader;
                        rObj.Load(rDataReader.Id, tr);
                        data.Add(rObj);
                    }
                }
                catch (Exception exc)
                {
                    this.ErrorHandles.Add(handle);
                    App.Riviera.Log.AppendEntry(String.Format("Error al cargar remates finales Handle: {0}\n{1}", handle, exc.Message), Protocol.Error, this);
                    Selector.Ed.WriteMessage("{0}\n", exc.Message);
                }
            }


            //Se cargan los paneles al final por que requieren de que las mamparas esten cargadas
            foreach (RivieraData rDataReader in this.PanelesData)
            {
                try
                {
                    rd = rDataReader.Extract(tr);
                    handle = long.Parse(rd[10]);
                    mObj = data.Where(x => x.Handle.Value == handle).FirstOrDefault();
                    if (mObj != null && mObj is Mampara)
                    {
                        panels = PanelRaw.Parse(rd[12]);
                        rObj = new RivieraPanelStack(mObj as Mampara, panels);
                        rObj.Data = rDataReader;
                        rObj.Load(rDataReader.Id, tr);
                        data.Add(rObj);
                    }
                }
                catch (Exception exc)
                {
                    this.ErrorHandles.Add(handle);
                    App.Riviera.Log.AppendEntry(String.Format("Error al cargar paneles Handle: {0}\n{1}", handle, exc.Message), Protocol.Error, this);
                    Selector.Ed.WriteMessage("{0}\n", exc.Message);
                }
            }
            Mampara left, top, right;
            RivieraPanel rPanel;
            RivieraPanelDoubleLocation loc;
            this.JointDoubleLocations.Clear();
            var pItemsToAdd = new List<RivieraObject>();
            List<RivieraObject> pItemsForJoint;
            foreach (var joint in data.Where(x => x is MamparaJoint && (x as MamparaJoint).HasDoublePanels))
            {
                pItemsForJoint = new List<RivieraObject>();
                RivieraPanelDoubleLocation.FindMampara(data, joint as MamparaJoint, out left, out right, out top);
                var location = new RivieraPanelDoubleLocation(RivieraPanelDoubleLocation.PickFrente(left), top, left, right, joint as MamparaJoint);
                this.JointDoubleLocations.Add(joint.Handle.Value, location);

                long[] ids = new long[]
                {
                    (joint as MamparaJoint).PanelDoubleLeftId,
                    (joint as MamparaJoint).PanelDoubleRighttId,
                    (joint as MamparaJoint).PanelDoubleBottomId,
                    (joint as MamparaJoint).PanelDoubleId
                };
                if ((joint as MamparaJoint).BiomboId > 0)
                    ids = ids.Union(new long[] { (joint as MamparaJoint).BiomboId }).ToArray();
                //Cargamos los paneles
                var pItemsData = this.PItems.Select(x =>
                {
                    try
                    {
                        if (ids.Contains(x.Id.GetObject(OpenMode.ForRead).Handle.Value))
                            return x;
                        else
                            return null;
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
                ).Where(y => y != null);
                foreach (RivieraData rDataReader in pItemsData)
                {
                    try
                    {
                        rd = rDataReader.Extract(tr);
                        handle = long.Parse(rd[10]);
                        mObj = data.Where(x => x.Handle.Value == handle).FirstOrDefault();
                        var ps = PanelRaw.Parse(rd[12]);
                        var p = ps[0];
                        loc = this.JointDoubleLocations[handle];
                        if (ps.Length == 3)
                            rPanel = new RivieraPanel54(loc.Left, ps[0], ps[1], ps[2], loc);
                        else if (p.Direction == ArrowDirection.Same)
                            rPanel = new RivieraPanelBiombo(loc.Left, p, location);
                        else
                            rPanel = new RivieraPanel(loc.Left, p, loc);
                        rPanel.Data = rDataReader;
                        rPanel.Load(rDataReader.Id, tr);
                        if (p.Direction == ArrowDirection.Left_Back)
                            (mObj as MamparaJoint).PanelDoubleLeftId = rPanel.Handle.Value;
                        else if (p.Direction == ArrowDirection.Right_Back)
                            (mObj as MamparaJoint).PanelDoubleRighttId = rPanel.Handle.Value;
                        else if (p.Direction == ArrowDirection.Front)
                            (mObj as MamparaJoint).PanelDoubleId = rPanel.Handle.Value;
                        else if (p.Direction == ArrowDirection.Back)
                            (mObj as MamparaJoint).PanelDoubleBottomId = rPanel.Handle.Value;
                        else if (p.Direction == ArrowDirection.Same)
                            (mObj as MamparaJoint).BiomboId = rPanel.Handle.Value;
                        pItemsToAdd.Add(rPanel);
                        pItemsForJoint.Add(rPanel);
                    }
                    catch (Exception exc)
                    {
                        this.ErrorHandles.Add(handle);
                        App.Riviera.Log.AppendEntry(String.Format("Error al cargar paneles Handle: {0}\n{1}", handle, exc.Message), Protocol.Error, this);
                        Selector.Ed.WriteMessage("{0}\n", exc.Message);
                    }
                }
                //Se crea la unión del panel
                var pRaws = pItemsData.Select(y => PanelRaw.Parse(y.Extract(tr)[12])[0]);
                if (pRaws.Count(x => x.Direction != ArrowDirection.Same) == 4)
                {
                    (joint as MamparaJoint).PanelArray = new RivieraDoublePanelArray()
                    {
                        DobleFront = pItemsForJoint.Where(x => (x as RivieraPanel).Raw.Direction == ArrowDirection.Front).FirstOrDefault() as RivieraPanel,
                        DobleBottom = pItemsForJoint.Where(x => (x as RivieraPanel).Raw.Direction == ArrowDirection.Back).FirstOrDefault() as RivieraPanel,
                        Left = pItemsForJoint.Where(x => (x as RivieraPanel).Raw.Direction == ArrowDirection.Left_Back).FirstOrDefault() as RivieraPanel,
                        Right = pItemsForJoint.Where(x => (x as RivieraPanel).Raw.Direction == ArrowDirection.Right_Back).FirstOrDefault() as RivieraPanel,
                    };
                }
                else
                {
                    (joint as MamparaJoint).PanelArray = new RivieraDoublePanelArray()
                    {
                        DobleFront = pItemsForJoint.Where(x => (x as RivieraPanel).Raw.Direction == ArrowDirection.Front).FirstOrDefault() as RivieraPanel,
                        Left = pItemsForJoint.Where(x => (x as RivieraPanel).Raw.Direction == ArrowDirection.Left_Back).FirstOrDefault() as RivieraPanel,
                        Right = pItemsForJoint.Where(x => (x as RivieraPanel).Raw.Direction == ArrowDirection.Right_Back).FirstOrDefault() as RivieraPanel,
                    };
                }
                //Existen Biombos
                if (pRaws.Count(x => x.Direction == ArrowDirection.Same) == 1)
                    (joint as MamparaJoint).PanelArray.Biombo = pItemsForJoint.Where(x => (x as RivieraPanel).Raw.Direction == ArrowDirection.Same).FirstOrDefault() as RivieraPanelBiombo;

            }

            //Se agregán los panel items a la BD
            foreach (var pI in pItemsToAdd)
                data.Add(pI);

            //Se cargan los biombos al final por que requieren de las mamparas
            foreach (RivieraData rDataReader in this.BiomboData)
            {
                try
                {
                    rd = rDataReader.Extract(tr);
                    handle = long.Parse(rd[10]);
                    mObj = data.Where(x => x.Handle.Value == handle).FirstOrDefault();
                    if (mObj != null && mObj is Mampara)
                    {
                        String code = rd[0];
                        panels = PanelRaw.Parse(rd[12]);
                        PanelRaw bRaw = panels.FirstOrDefault();
                        String codeShort = bRaw.Code.Substring(0, 6);
                        if (PichonerasCodes.Contains(codeShort))
                            rObj = new RivieraPichonera(mObj as Mampara, bRaw.Code, bRaw);
                        else if (CajonerasCodes.Contains(codeShort))
                            rObj = new RivieraCajonera(mObj as Mampara, bRaw.Code, bRaw);
                        else
                            rObj = new RivieraBiombo(mObj as Mampara, bRaw.Code, bRaw);
                        (rObj as RivieraBiombo).Acabado = panels.FirstOrDefault().Acabado;
                        rObj.Data = rDataReader;
                        rObj.Load(rDataReader.Id, tr);
                        data.Add(rObj);
                    }
                }
                catch (Exception exc)
                {
                    this.ErrorHandles.Add(handle);
                    App.Riviera.Log.AppendEntry(String.Format("Error al cargar biombos Handle: {0}\n{1}", handle, exc.Message), Protocol.Error, this);
                    Selector.Ed.WriteMessage("{0}\n", exc.Message);
                }
            }
        }
        /// <summary>
        /// Esta función válida los elementos encontrados en la base que se encuentran desconectados.
        /// Estos elementos si se dejan en la base pueden causar erores en las cuantificaciones
        /// </summary>
        internal void ValidateDisconnectedElements()
        {
            var mamparas = App.DB.Objects.Where(x => x is Mampara).ToArray();
            var notMamparas = App.DB.Objects.Where(x => !(x is Mampara)).ToArray();
            //Se realiza el proceso primero con las maparas.
            this.ValidateDisconnectedElements(mamparas);
            //Se repite el proceso para los demas elementos.
            this.ValidateDisconnectedElements(notMamparas);
        }
        /// <summary>
        /// Realiza la validación de los elementos desconectados
        /// </summary>
        /// <param name="objs">Los obejetos</param>
        internal void ValidateDisconnectedElements(IEnumerable<RivieraObject> objs)
        {
            //Se realiza la selección de mamparas desconectadas
            new FastTransactionWrapper((Document doc, Transaction tr) =>
            {
                foreach (var obj in objs)
                {
                    try
                    {
                        objs = obj.GetObjects();
                        //Las mamparas seleccionadas estan desconectadas cuando el número de punteros no coincide al número de relaciones
                        //existentes.
                        if (!obj.CheckPointersVsRelations(objs))//Se eliminan estas mamparas.
                            obj.Delete(tr);
                    }
                    catch (Exception exc)
                    {
                        App.Riviera.Log.AppendEntry("Error al checar elementos repetidos" + exc.Message, Protocol.Error, "ValidateDisconnectedElements");
                        doc.Editor.WriteMessage("\nError al checar elementos repetidos", exc.Message);
                    }
                }
            }).Run();
        }
        /// <summary>
        /// Selecciona los altos mediante la selección de un código
        /// </summary>
        /// <param name="code">El código seleccionado</param>
        /// <param name="frente">El frente</param>
        /// <param name="nivel">El nivel seleccionado</param>
        /// <returns>El alto</returns>
        internal double SelectAlto(string code, string frente, string nivel)
        {
            var pns = App.DB.Panel_Size.Where(x => x.Code == code && x.Frente == frente).Select(y => y.Nominal.Alto);
            var pnsByNiv = App.DB.Alto_Nivel.Where(x => x.Type == ElementType.Panel && x.Nivel == nivel);
            return pnsByNiv.Where(x => pns.Contains(x.Alto)).FirstOrDefault().Alto;
        }

        /// <summary>
        /// Realiza un proceso de limpiado en la geometría de la Base de datos.
        /// </summary>
        public void CleanGeometry(out List<ObjectId> elNotErased, out List<RivieraObject> elNotFixed)
        {
            BlankTransactionWrapper<Object> trW = new BlankTransactionWrapper<Object>(
               delegate (Document doc, Transaction tr)
               {
                   //La lista de elementos no arreglados.
                   List<ObjectId> notErased = new List<ObjectId>();
                   List<RivieraObject> notFixed = new List<RivieraObject>();
                   ObjectId id;
                   AutoCADLayer lay = new AutoCADLayer(LAYER_RIVIERA_GEOMETRY, tr);
                   lay.SetStatus(LayerStatus.EnableStatus);
                   BlockTableRecord model = doc.Database.CurrentSpaceId.GetObject(OpenMode.ForWrite) as BlockTableRecord;
                   //Se elimina la geometría en el espacio de modelo
                   foreach (ObjectId geoId in model)
                   {
                       try
                       {
                           DBObject obj = geoId.GetObject(OpenMode.ForRead);
                           if (obj is Entity && (obj as Entity).Layer == LAYER_RIVIERA_GEOMETRY)
                               Drawer.Erase(tr, geoId);
                       }
                       catch (Exception exc)
                       {
                           Selector.Ed.WriteMessage("\n{0}", exc.Message);
                           App.Riviera.Log.AppendEntry(exc.Message, Protocol.Error, "CleanGeometry::void");
                           notErased.Add(geoId);
                       }
                   }
                   //La lista de objetos de los objetos a limpiar
                   bool isDoublePanel;
                   foreach (var obj in App.DB.Objects)
                   {
                       try
                       {
                           id = obj.Ids[0];
                           obj.Ids.Clear();
                           obj.Ids.Add(id);
                           obj.CreateContent();
                           obj.DrawContent(tr);
                           isDoublePanel = obj is RivieraPanel && App.DB[obj.Parent] is MamparaJoint;
                           ObjectIdCollection geometry;
                           if (!isDoublePanel && App.Riviera.Units == DaNTeUnits.Imperial)
                           {
                               geometry = new ObjectIdCollection(obj.Ids.OfType<ObjectId>().Where(x => x != obj.Line.Id).ToArray());
                               if (obj is Mampara)
                               {
                                   Double ang = Mampara54Flipper.GetRotation(obj as Mampara, tr);
                                   var matrix = Matrix3d.Scaling(IMPERIAL_FACTOR, (ang == 0 ? obj.Line.StartPoint : obj.Line.EndPoint));
                                   geometry.Transform(matrix, tr);
                               }
                               else
                                   geometry.Transform(Matrix3d.Scaling(IMPERIAL_FACTOR, obj.Line.StartPoint), tr);
                           }
                           obj.Data.Set(FIELD_GEOMETRY, tr, obj.SaveGeometry(tr));
                       }
                       catch (Exception exc)
                       {
                           Selector.Ed.WriteMessage("\n{0}", exc.Message);
                           App.Riviera.Log.AppendEntry(exc.Message, Protocol.Error, "CleanGeometry::void");
                           notFixed.Add(obj);
                       }
                   }
                   return new Object[] { notErased, notFixed };
               });
            Object[] res = (Object[])trW.Run();
            elNotErased = res[0] as List<ObjectId>;
            elNotFixed = res[1] as List<RivieraObject>;
        }
        /// <summary>
        /// La lista de paneles utiles para una mampara seleccionada
        /// </summary>
        /// <param name="mampara">La mampara seleccionada</param>
        /// <returns>La lista de paneles</returns>
        internal List<PanelData> PanelsFor(Mampara mampara)
        {
            List<PanelData> paneles = new List<PanelData>();
            double frente = double.Parse(mampara.Code.Substring(6, 2)),
                   alto = double.Parse(mampara.Code.Substring(8, 2));
            //1: Se realiza la selección del nivel máximo para la mampara seleccionada
            AltoNivel levelMax = this.Alto_Nivel.Where(x => alto == x.Alto && x.Type == ElementType.Mampara).FirstOrDefault();
            //2: Se realiza la selección de tamaño de paneles con un alto menor o igual a los permitidos
            IEnumerable<PanelSize> panel_sizes = this.Panel_Size.Where(x => x.Nominal.Alto <= levelMax.Alto && x.Nominal.Frente == frente);

            //3: Creamos la lista de paneles
            PanelData panel;
            String desc, tp;
            String lev;
            RivieraCode rCode;
            foreach (PanelSize size in panel_sizes)
            {
                panel = paneles.Where(x => x.Code == size.Code).FirstOrDefault();
                if (panel == null)
                {
                    rCode = this.Description.Where(x => x.Code.Trim() == size.Code.Trim()).FirstOrDefault();
                    desc = rCode.Description;
                    tp = rCode.Tipo;
                    panel = new PanelData()
                    {
                        Code = size.Code.Trim(),
                        Description = desc != null ? desc : CAPTION_NO_DESC,
                        Heights = new Dictionary<string, double>(),
                        HostCode = mampara.Code,
                        Host_Front = frente,
                        Host_Height = alto,
                        Niveles = new List<string>(),
                        FrenteNominal = size.FrenteNominal,
                        Tipo = tp,
                        CanBeDouble = rCode.CanBeDouble
                    };
                    paneles.Add(panel);
                }
                lev = this.Alto_Nivel.Where(x => x.Type == ElementType.Panel && x.Alto == size.Nominal.Alto).FirstOrDefault().Nivel;
                if (lev != null && lev != String.Empty)
                {
                    panel.Niveles.Add(lev);
                    if (!panel.Heights.ContainsKey(lev))
                        panel.Heights.Add(lev, (long)size.Nominal.Alto);
                }

            }
            return paneles;
        }

        /// <summary>
        /// Get the size from a real size formatted from a
        /// mampara
        /// </summary>
        /// <param name="size">The mampara size</param>
        /// <returns>The current mampara size</returns>
        public MamparaSize GetSize(String size)
        {
            return this.Mampara_Sizes.Where(x => x.Real.ConvertUnits(Unit_Type.mm, Unit_Type.m).ToString() == size.ToString()).FirstOrDefault();
        }
        /// <summary>
        /// Get the size from a nominal size to a real size
        /// </summary>
        /// <param name="size">The nominal size</param>
        /// <param name="isFrente">True if the size is from frente</param>
        /// <returns>The current real mampara size</returns>
        public Double GetRealSize(String size, bool isFrente)
        {
            MamparaSize mSize;
            if (isFrente)
                mSize = this.Mampara_Sizes.Where(x => x.Frente == size).FirstOrDefault();
            else
                mSize = this.Mampara_Sizes.Where(x => x.Alto == size).FirstOrDefault();
            if (mSize != null)
                return isFrente ? mSize.Real.Frente : mSize.Real.Alto;
            else
                return 0;
        }
        /// <summary>
        /// Devuelve el objeto de riviera seleccionado y llena la información de Remates finales
        /// </summary>
        /// <param name="code">El código del elemento seleccionado</param>
        /// <param name="dataReader">La información de lectura del remate</param>
        /// <param name="tr">La transacción activa</param>
        /// <returns>El objeto de riviera</returns>
        private RivieraObject GetRivieraObject(string code, RivieraData dataReader, Transaction tr)
        {
            RivieraObject obj = null;
            String[] extra;
            long id;
            if (code != null)
            {
                String[] data = dataReader.Extract(tr);
                if (code.Length >= 6 && code.Substring(0, 6) == DT_MAMPARA)//Si es una mampara de DELTA
                {
                    obj = new Mampara(data[2].ParseAsPoint(), data[3].ParseAsPoint(), data[1].ParseAsSize(), data[0]);
                    extra = data[12].Split('|');
                    (obj as Mampara).BiomboId = long.TryParse(extra[0], out id) ? id : 0;
                }
                else if (code == JOINT)
                {
                    obj = new JointObject(data[2].ParseAsPoint(), data[2].ParseAsPoint().GetVectorTo(data[3].ParseAsPoint()).Angle);
                    if (data[12].Length > 0 && data[12].Contains('|'))
                        (obj as JointObject).LoadExtra(data[12].Split('|'));
                }
                else if (code == DT_JOINT)
                {
                    obj = new MamparaJoint(data[2].ParseAsPoint(), data[2].ParseAsPoint().GetVectorTo(data[3].ParseAsPoint()).Angle);
                    if (data[12].Length > 0 && data[12].Contains('|'))
                        (obj as MamparaJoint).LoadExtra(data[12].Split('|'));
                }
                else if (code == PANEL_STACK)
                    this.PanelesData.Add(dataReader);
                else if (code.Length >= 6 && code.Substring(0, 6) == DT_REMATE_FINAL)
                    this.RemateFinalData.Add(dataReader);
                else if (code == PANEL_ITEM)
                    this.PItems.Add(dataReader);
                else if (code.Length >= 3 && code.Substring(0, 3) == "DD8" || code.Substring(0, 3) == "DD7")
                    this.BiomboData.Add(dataReader);
                if (obj != null)
                {
                    obj.Data = dataReader;
                    obj.Load(dataReader.Id, tr);
                }
            }
            return obj;
        }

        /// <summary>
        /// Inicia el proceso de carga en la BD
        /// </summary>
        private object InitMemory(Oracle_Connector conn, ref BackgroundWorker bgWorker, object trParameters)
        {

            Query_Mampara qM = new Query_Mampara();
            Query_Paneles qP = new Query_Paneles();
            Query_Acabados qA = new Query_Acabados();
            Query_Uniones qU = new Query_Uniones();
            Query_Uniones_Extra qUE = new Query_Uniones_Extra();
            List<AltoNivel> an = new List<AltoNivel>();
            List<RivieraAcabado> acabados = new List<RivieraAcabado>();
            Dictionary<String, TratamientoUnion> vRules = new Dictionary<string, TratamientoUnion>();
            //Tamaños de mamparas
            List<String> rows = conn.SelectRows(qM.SelectMamparas(), LilithConstants.ESCAPECHAR);
            IEnumerable<MamparaSize> mamparaSizes = rows.Select(x => x.Split(LilithConstants.ESCAPECHAR).ParseAsMampara());
            //Tamaños de paneles y biombos
            rows = conn.SelectRows(qP.SelectPaneles(), LilithConstants.ESCAPECHAR);
            IEnumerable<PanelSize> panels = rows.Select(x => x.Split(LilithConstants.ESCAPECHAR).ParseAsPanelData());
            //Descripciones
            rows = conn.SelectRows(qP.SelectDescription(), LilithConstants.ESCAPECHAR);
            IEnumerable<RivieraCode> desc = rows.Select(x => x.Split(LilithConstants.ESCAPECHAR).ParseAsRivieraCode()).Where(y => y.Code != String.Empty);
            //Alturas de nivel
            rows = conn.SelectRows(qP.SelectNiveles(), LilithConstants.ESCAPECHAR);
            rows.ForEach(x => an.Add(new AltoNivel(ElementType.Panel, x.Split(LilithConstants.ESCAPECHAR))));
            rows = conn.SelectRows(qM.SelectNiveles(), LilithConstants.ESCAPECHAR);
            rows.ForEach(x => an.Add(new AltoNivel(ElementType.Mampara, x.Split(LilithConstants.ESCAPECHAR))));
            //Acabados
            rows = conn.SelectRows(qA.SelecAcabados(), LilithConstants.ESCAPECHAR);
            RivieraAcabado.ParseAcabados(rows, ref acabados);
            //Restricciones de nivel de paneles
            rows = conn.SelectRows(qP.SelectRestrictionNivel(), LilithConstants.ESCAPECHAR);
            IEnumerable<RivieraPanelLevelRestriction> pRules = rows.Select(x => x.Split(LilithConstants.ESCAPECHAR).ParseAsRivieraPanelLevelRestriction()).Where(y => y.Code != String.Empty);
            //Reglas de uniones de mampara
            rows = conn.SelectRows(qU.SelecUniones(), LilithConstants.ESCAPECHAR);
            IEnumerable<MamparaUnionRule> pUniones = rows.Select(x => x.Split(LilithConstants.ESCAPECHAR).ParseAsMamparaUnionRule());
            //Reglas de tratamiento de mamparas
            rows = conn.SelectRows(qUE.SelecRules(), LilithConstants.ESCAPECHAR);
            rows.Select(x => x.Split(LilithConstants.ESCAPECHAR)).ToList().ForEach(x =>
            {
                if (!vRules.ContainsKey(x[0]))
                    vRules.Add(x[0], (TratamientoUnion)Enum.Parse(typeof(TratamientoUnion), x[1]));
            });
            return new Object[] { mamparaSizes.ToList(), panels.ToList(), desc.ToList(), an, acabados, pRules.ToList(), pUniones.ToList(), vRules };
        }
        /// <summary>
        /// Una vez que termina el proceso de carga
        /// </summary>
        private void TaskIsFinished(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is Exception)
            {
                Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("Sin conexión a la BD\n");
            }
            else if (e.Result is Object[])
            {
                Object[] data = e.Result as Object[];
                this.Mampara_Sizes = data[0] as List<MamparaSize>;
                this.Panel_Size = data[1] as List<PanelSize>;
                this.Description = data[2] as List<RivieraCode>;
                this.Alto_Nivel = data[3] as List<AltoNivel>;
                this.Acabados = data[4] as List<RivieraAcabado>;
                this.Panel_Restriction = data[5] as List<RivieraPanelLevelRestriction>;
                this.MamparaUnionRules = data[6] as List<MamparaUnionRule>;
                this.TratamientosParaUniones = data[7] as Dictionary<String, TratamientoUnion>;
                Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("Memoria cargada\n");
                Selector.InvokeCMD(DELTA_UI);
            }
        }
        /// <summary>
        /// Una vez que termina el proceso de carga sin cargar la UI
        /// </summary>
        private void TaskIsFinishedNOUI(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is Exception)
            {
                Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("Sin conexión a la BD\n");
            }
            else if (e.Result is Object[])
            {
                Object[] data = e.Result as Object[];
                this.Mampara_Sizes = data[0] as List<MamparaSize>;
                this.Panel_Size = data[1] as List<PanelSize>;
                this.Description = data[2] as List<RivieraCode>;
                this.Alto_Nivel = data[3] as List<AltoNivel>;
                this.Acabados = data[4] as List<RivieraAcabado>;
                this.Panel_Restriction = data[5] as List<RivieraPanelLevelRestriction>;
                this.MamparaUnionRules = data[6] as List<MamparaUnionRule>;
                this.TratamientosParaUniones = data[7] as Dictionary<String, TratamientoUnion>;
                if (Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument != null)
                    Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("Memoria cargada\n");
                Selector.InvokeCMD(REGEN_IDS_ENDED);
            }
        }


    }
}
