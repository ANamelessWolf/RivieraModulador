using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using DaSoft.Riviera.OldModulador.Model;
using DaSoft.Riviera.OldModulador.Model.Delta;
using DaSoft.Riviera.OldModulador.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Mio;
using NamelessOld.Libraries.HoukagoTeaTime.MugiChan;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu;
using NamelessOld.Libraries.HoukagoTeaTime.Yui;
using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.Collections.Generic;
using System.Linq;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CONST;
using static DaSoft.Riviera.OldModulador.Assets.Strings;
namespace DaSoft.Riviera.OldModulador.Controller
{
    public class ModuladorIdMapper : NamelessObject
    {
        /// <summary>
        /// La lista de los elementos a mapear
        /// </summary>
        public Dictionary<long, long> Mapping;
        /// <summary>
        /// La lista de objetos que contienen un id erroneo
        /// </summary>
        public List<DBObject> ObjToFix;


        /// <summary>
        /// La lista de objetos que no se pudieron mapear
        /// </summary>
        public Dictionary<long, DBObject> Failed;
        /// <summary>
        /// BlockReference filter
        /// </summary>
        public SelectionFilter BlockFilter
        {
            get
            {
                TypedValue[] tps = new TypedValue[]
                {
                new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(BlockReference)).DxfName),
                };
                return new SelectionFilter(tps);
            }
        }
        /// <summary>
        /// DBText filter
        /// </summary>
        public SelectionFilter TextFilter
        {
            get
            {
                TypedValue[] tps = new TypedValue[]
                {
                new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(DBText)).DxfName),
                };
                return new SelectionFilter(tps);
            }
        }
        /// <summary>
        /// Polyline filter
        /// </summary>
        public SelectionFilter PolylineFilter
        {
            get
            {
                TypedValue[] tps = new TypedValue[]
                {
                new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Polyline)).DxfName),
                };
                return new SelectionFilter(tps);
            }
        }
        /// <summary>
        /// Line filter
        /// </summary>
        public SelectionFilter LineFilter
        {
            get
            {
                TypedValue[] tps = new TypedValue[]
                {
                new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Line)).DxfName),
                };
                return new SelectionFilter(tps);
            }
        }
        /// <summary>
        /// Inicializa la estructura para mapear los ids
        /// </summary>
        public ModuladorIdMapper()
        {
            this.Mapping = new Dictionary<long, long>();
            this.ObjToFix = new List<DBObject>();
            this.Failed = new Dictionary<long, DBObject>();
        }
        /// <summary>
        /// Checa si el elemento fue copiado y tiene la estructura del modulador.
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        /// <param name="obj">El objeto copiado</param>
        /// <returns>Verdadero si el elemento es copiado</returns>
        public bool IsRivieraObject(Transaction tr, DBObject obj)
        {
            if (obj is Line && (obj as Line).Layer == LAYER_RIVIERA_OBJECT)
            {
                long savHan = this.GetSavedHandle(tr, obj);
                return savHan != 0 && obj.Handle.Value != savHan;
            }
            else
                return false;
        }
        /// <summary>
        /// Obtiene el id guardado de un elemento tipo línea
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        /// <param name="obj">El objeto abrir</param>
        /// <returns>El handle del id seleccionado</returns>
        private long GetSavedHandle(Transaction tr, DBObject obj)
        {
            long han, savHan = 0;
            //1: Extracción en caso de no ser un elemento válido,
            //se regresa 0
            if (obj.ExtensionDictionary.IsValid)
            {
                ExtensionDictionaryManager dMan;
                dMan = new ExtensionDictionaryManager(obj.Id, tr);
                Xrecord recId;
                if (dMan.TryGetRegistry(FIELD_REG_ID, out recId, tr))
                {
                    String hanStr;
                    hanStr = recId.GetDataAsString(tr).FirstOrDefault();
                    savHan = long.TryParse(hanStr, out han) ? han : 0;
                }
            }
            //2: Se agrega el elemento a la lista de mapeo en
            //caso de que el id guardado sea distinto al de la entidad
            if (savHan != 0 && savHan != obj.Handle.Value && !this.Mapping.ContainsKey(savHan))
                this.Mapping.Add(savHan, obj.Handle.Value);
            return savHan;
        }
        /// <summary>
        /// Agregá un elemento a lista de objetos arreglar ids
        /// </summary>
        /// <param name="obj">El objeto arreglar sus ids</param>
        public void AddObject(DBObject obj)
        {
            this.ObjToFix.Add(obj);
        }
        /// <summary>
        /// Inicia el proceso de mapeo
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        public void Fix(Transaction tr)
        {
            String[] fieldsToUpdate =
                new String[]
                {
                    FIELD_LEFT_FRONT,
                    FIELD_FRONT,
                    FIELD_RIGHT_FRONT,
                    FIELD_BACK,
                    FIELD_LEFT_BACK,
                    FIELD_RIGHT_BACK,
                    FIELD_PARENT,
                    FIELD_REG_ID,
                    FIELD_EXTRA,
                };
            Xrecord xRec;
            ExtensionDictionaryManager dMan;
            long handle, originalHandle, num;
            ElementFinder finder;
            RivieraObject originalObj;
            PointerDirection direction;
            ObjectId foundId;
            Entity foundObject;
            String code;
            foreach (DBObject obj in ObjToFix)
            {
                handle = (obj as Line).Handle.Value;
                dMan = new ExtensionDictionaryManager(obj.Id, tr);
                obj.UpgradeOpen();
                xRec = dMan.GetRegistry(FIELD_REG_ID, tr);
                code = dMan.GetRegistry(FIELD_CODE, tr).GetDataAsString(tr)[0];
                if (code == "PItem")
                    continue;
                else if (code == "DDMJoint")
                {
                    String extra = dMan.GetRegistry(FIELD_EXTRA, tr).GetDataAsString(tr)[0];
                    if (extra != String.Empty)
                    {
                        Line[] pItems = ObjToFix.Where(x => new ExtensionDictionaryManager(x.Id, tr).GetRegistry(FIELD_CODE, tr).GetDataAsString(tr)[0] == "PItem").Select(y => y as Line).ToArray();
                        var ids = pItems.Select(x => x.Id.ToString()).ToArray();
                        Double h = MAM_ANCHO_MM * 3;
                        h = h.ConvertUnits(Unit_Type.mm, Unit_Type.m);
                        if (App.Riviera.Units == DaNTeUnits.Imperial)
                            h = h.ConvertUnits(Unit_Type.m, Unit_Type.inches);
                        PolygonSelector fenceSel = new PolygonSelector((obj as Line).StartPoint.MiddlePointTo((obj as Line).EndPoint).ToPoint2d(), h);
                        double windowSize = 1.5;
                        if (App.Riviera.Units == DaNTeUnits.Imperial)
                            windowSize = windowSize.ConvertUnits(Unit_Type.m, Unit_Type.inches);
                        ZoomWindow zWin = new ZoomWindow((obj as Line).StartPoint.MiddlePointTo((obj as Line).EndPoint), windowSize, windowSize);
                        zWin.SetView(1);
                        Selector.Ed.Regen();
                        SelectionFilterBuilder sb = new SelectionFilterBuilder(typeof(Line));
                        sb.SetAllowedLayers(LAYER_RIVIERA_OBJECT);
                        if (fenceSel.Search(SelectType.Crossing, sb.Filter))
                        {
                            Line[] panels = pItems.Where(x => fenceSel.Ids.Contains(x.Id)).ToArray();
                            PanelRaw raw;
                            long left = 0, right = 0, front = 0, bottom = 0, self = 0;
                            String val;
                            //  Drawer.CreatePolyline((new NamelessOld.Libraries.HoukagoTeaTime.Ritsu.Shapes2D.Polygon2D(14, (obj as Line).StartPoint.MiddlePointTo((obj as Line).EndPoint).ToPoint2d(), h)));
                            foreach (Line panel in panels)
                            {
                                raw = PanelRaw.Parse(new ExtensionDictionaryManager(panel.Id, tr).GetRegistry(FIELD_EXTRA, tr).GetDataAsString(tr)[0])[0];
                                if (raw.Direction == ArrowDirection.Front)
                                    front = panel.Handle.Value;
                                else if (raw.Direction == ArrowDirection.Left_Back)
                                    left = panel.Handle.Value;
                                else if (raw.Direction == ArrowDirection.Right_Back)
                                    right = panel.Handle.Value;
                                else if (raw.Direction == ArrowDirection.Back)
                                    bottom = panel.Handle.Value;
                                else if (raw.Direction == ArrowDirection.Same)
                                    self = panel.Handle.Value;
                                foreach (String field in fieldsToUpdate)
                                {
                                    xRec = new ExtensionDictionaryManager(panel.Id, tr).GetRegistry(field, tr);
                                    val = xRec.GetDataAsString(tr)[0];
                                    if (field == FIELD_PARENT)
                                        xRec.SetData(tr, (obj.Handle.Value.ToString()));
                                    //Se actualiza el Id y la geometría
                                    else if (field == FIELD_REG_ID)
                                    {
                                        xRec.SetData(tr, panel.Handle.Value.ToString());
                                        //Start
                                        xRec = new ExtensionDictionaryManager(panel.Id, tr).GetRegistry(FIELD_START, tr);
                                        xRec.SetData(tr, (obj as Line).StartPoint.ToString());
                                        //End
                                        xRec = new ExtensionDictionaryManager(panel.Id, tr).GetRegistry(FIELD_END, tr);
                                        xRec.SetData(tr, (obj as Line).EndPoint.ToString());
                                    }
                                    //Los valores del campo extra no cambian
                                    else if (field == FIELD_EXTRA)
                                        continue;
                                    //Los apuntadores del PItem
                                    else if (val != "0")
                                    {

                                    }
                                }


                            }

                            if (panels.Length >= 4 && bottom != 0)
                                extra = String.Format("{0}|{1}", true, RivieraDoublePanelArray.CreateExtra(false, self, left, right, front, bottom));
                            else 
                                extra = String.Format("{0}|{1}", true, RivieraDoublePanelArray.CreateExtra(true, self, left, right, front));
                            xRec = dMan.GetRegistry(FIELD_EXTRA, tr);
                            val = xRec.GetDataAsString(tr)[0];
                            xRec.SetData(tr, extra);
                        }
                        //En caso de cambiar se debe seleccionar el último registro del joint que es el del id.
                        xRec = dMan.GetRegistry(FIELD_REG_ID, tr);
                    }

                }
                originalHandle = long.TryParse(xRec.GetDataAsString(tr).FirstOrDefault(), out num) ? num : 0;
                if (this.Mapping.ContainsKey(originalHandle))
                {
                    try
                    {
                        originalObj = App.DB[originalHandle];
                        finder = new ElementFinder(originalObj.Code, (obj as Line), originalObj.Children, originalObj.Parent);
                        foreach (String field in fieldsToUpdate)
                        {
                            try
                            {
                                //Se actualiza el Id y la geometría
                                if (field == FIELD_REG_ID)
                                {
                                    xRec = dMan.GetRegistry(field, tr);
                                    xRec.SetData(tr, obj.Handle.Value.ToString());
                                    //Start
                                    xRec = dMan.GetRegistry(FIELD_START, tr);
                                    xRec.SetData(tr, (obj as Line).StartPoint.ToString());
                                    //End
                                    xRec = dMan.GetRegistry(FIELD_END, tr);
                                    xRec.SetData(tr, (obj as Line).EndPoint.ToString());
                                }
                                //Se actualiza el campo Extra
                                else if (field == FIELD_EXTRA)
                                {
                                    xRec = dMan.GetRegistry(field, tr);
                                    handle = long.TryParse(xRec.GetDataAsString(tr).FirstOrDefault(), out num) ? num : 0;
                                    if (handle != 0 && finder.CodeType == "DD1")
                                    {
                                        direction = PointerDirection.Same;
                                        foundId = finder.Find(direction);
                                        if (foundId.IsValid)
                                        {
                                            foundObject = foundId.GetObject(OpenMode.ForRead) as Entity;
                                            handle = foundObject.Handle.Value;
                                            xRec.SetData(tr, handle.ToString());
                                        }
                                        else if (!this.Failed.ContainsKey(obj.Handle.Value))
                                            this.Failed.Add(obj.Handle.Value, obj);
                                    }
                                }
                                else if ((field == FIELD_PARENT && originalObj.Parent != 0) ||
                                    (originalObj.Children.ContainsKey(field) && originalObj.Children[field] != 0))
                                {
                                    if (field == FIELD_PARENT)
                                        direction = PointerDirection.Parent;
                                    else if (field == FIELD_FRONT)
                                        direction = PointerDirection.Front;
                                    else if (field == FIELD_BACK)
                                        direction = PointerDirection.Back;
                                    else if (field == FIELD_LEFT_FRONT || field == FIELD_LEFT_BACK)
                                        direction = PointerDirection.Left;
                                    else if (field == FIELD_RIGHT_FRONT || field == FIELD_RIGHT_BACK)
                                        direction = PointerDirection.Right;
                                    else
                                        direction = PointerDirection.None;
                                    if (direction != PointerDirection.None)
                                    {
                                        xRec = dMan.GetRegistry(field, tr);
                                        foundId = finder.Find(direction);
                                        if (foundId.IsValid)
                                        {
                                            foundObject = foundId.GetObject(OpenMode.ForRead) as Entity;
                                            handle = foundObject.Handle.Value;
                                            xRec.SetData(tr, handle.ToString());
                                        }
                                        else if (!this.Failed.ContainsKey(obj.Handle.Value))
                                            this.Failed.Add(obj.Handle.Value, obj);
                                    }
                                    else if (!this.Failed.ContainsKey(obj.Handle.Value))
                                        this.Failed.Add(obj.Handle.Value, obj);
                                }
                            }
                            catch (System.Exception exc)
                            {
                                App.Riviera.Log.AppendEntry(exc.Message, NamelessOld.Libraries.Yggdrasil.Lain.Protocol.Error, this);
                                Selector.Ed.WriteMessage("\n{0}", exc.Message);
                                if (!this.Failed.ContainsKey(obj.Handle.Value))
                                    this.Failed.Add(obj.Handle.Value, obj);
                            }
                        }//End For each
                    }//End Try
                    catch (System.Exception exc)
                    {
                        App.Riviera.Log.AppendEntry(exc.Message, NamelessOld.Libraries.Yggdrasil.Lain.Protocol.Error, this);
                        Selector.Ed.WriteMessage("\n{0}", exc.Message);
                        if (!this.Failed.ContainsKey(obj.Handle.Value))
                            this.Failed.Add(obj.Handle.Value, obj);
                    }//En Fix
                }//End If
            }



            //xRec = dMan.GetRegistry(FIELD_GEOMETRY, tr);
            //string[] data = xRec.GetDataAsString(tr);
            //string[] handles = data[0].Split(',');
            //if (handles.Length == 1)
            //{
            //    handle = long.TryParse(handles[0], out num) ? num : 0;
            //    if (handle != 0)
            //        MapGeometry(tr, (obj as Line), handle, dMan);
            //}
            //else if (handles.Length > 1)
            //    MapGeometry(tr, (obj as Line), handles, dMan);
        }

        private void Zoom(Point3d center)
        {
            double windowSize = 1.5;
            if (App.Riviera.Units == DaNTeUnits.Imperial)
                windowSize = windowSize.ConvertUnits(Unit_Type.m, Unit_Type.inches);
            ZoomWindow zWin = new ZoomWindow(center, windowSize, windowSize);
            zWin.SetView(1);
            Selector.Ed.Regen();
        }

        /// <summary>
        /// Actualiza el mapeo de la geometría del elemento buscando en el area seleccionada
        /// </summary>
        /// <param name="tr">La transacción actual</param>
        /// <param name="line">La líne con la información de la geometría</param>
        /// <param name="handle">El handle de la geometría que apunta el registro el elemento</param>
        /// <param name="dMan">El manejador de diccionario</param>
        private void MapGeometry(Transaction tr, Line line, long handle, ExtensionDictionaryManager dMan)
        {
            Point3d cen = line.StartPoint;
            //Abrimos la geometría existente
            Entity ent = (Entity)handle.GetId().GetObject(OpenMode.ForRead),
                   foundEnt;
            Xrecord xRec;
            this.Zoom(cen);
            FenceSelector sel;
            Selector.Ed.Regen();
            long han = 0;
            //Solo existe como geometría una polilínea o bloque
            sel = new FenceSelector(cen.ToPoint2d(), 0.01d);

            if (ent is BlockReference && sel.Search(this.BlockFilter))
                UpdateBlockReference(sel, line, ent, out foundEnt, out han);
            else if (ent is Polyline && sel.Search(this.PolylineFilter))
            {
                foundEnt = sel.First.GetObject(OpenMode.ForRead) as Entity;
                han = foundEnt.Handle.Value;
            }
            else if (ent is DBText && sel.Search(this.PolylineFilter))
            {
                foundEnt = sel.First.GetObject(OpenMode.ForRead) as Entity;
                PolygonSelector selWin = new PolygonSelector((foundEnt as Polyline).Vertices());
                if (selWin.Search(SelectType.Window, null))
                {
                    foreach (ObjectId entId in selWin.Ids)
                    {
                        foundEnt = entId.GetObject(OpenMode.ForRead) as Entity;
                        if (ent is DBText)
                        {
                            han = foundEnt.Handle.Value;
                            break;
                        }
                        else
                            han = 0;
                    }
                }
            }
            if (han != 0)
            {
                //Geometría
                xRec = dMan.GetRegistry(FIELD_GEOMETRY, tr);
                xRec.SetData(tr, han.ToString());
            }
            //Start
            xRec = dMan.GetRegistry(FIELD_START, tr);
            xRec.SetData(tr, line.StartPoint.ToString());
            //End
            xRec = dMan.GetRegistry(FIELD_END, tr);
            xRec.SetData(tr, line.EndPoint.ToString());
        }
        /// <summary>
        /// Actualiza la geometría cuando es de tipo blockreference
        /// </summary>
        /// <param name="sel">La selección fence activa</param>
        /// <param name="line">La línea que contiene la información</param>
        /// <param name="ent">La entidad de referencia definida en el bloque</param>
        /// <param name="foundEnt">La entidad encontrada</param>
        /// <param name="han">El handle de la entidad encontrada</param>
        private void UpdateBlockReference(FenceSelector sel, Line line, Entity ent, out Entity foundEnt, out long han)
        {
            IEnumerable<BlockReference> refs =
                sel.Ids.OfType<ObjectId>().
                Select<ObjectId, BlockReference>(x => x.GetObject(OpenMode.ForRead) as BlockReference).
                Where(y => y.Name == (ent as BlockReference).Name);
            if (refs.Count() > 1)
            {
                //Si existe más de uno posiblemente se trate de una I, se buscara en el punto inicial y
                //el resultado correcto será la intersección de la selección.
                han = 0;
                Point3d cen = line.EndPoint;
                this.Zoom(cen);
                FenceSelector sel2 = new FenceSelector(cen.ToPoint2d(), 0.01d);

                Selector.Ed.Regen();
                sel2.Search(BlockFilter);
                IEnumerable<BlockReference> refs2 =
                    sel2.Ids.OfType<ObjectId>().
                    Select<ObjectId, BlockReference>(x => x.GetObject(OpenMode.ForRead) as BlockReference).
                    Where(y => y.Name == (ent as BlockReference).Name);
                foundEnt = refs.Intersect(refs2).FirstOrDefault();
            }
            else
                foundEnt = refs.FirstOrDefault();
            han = foundEnt.Handle.Value;
        }

        /// <summary>
        /// Actualiza el mapeo de la geometría del elemento buscando en el area seleccionada
        /// </summary>
        /// <param name="tr">La transacción actual</param>
        /// <param name="line">La líne con la información de la geometría</param>
        /// <param name="handle">El handle de la geometría que apunta el registro el elemento</param>
        /// <param name="dMan">El manejador de diccionario</param>
        private void MapGeometry(Transaction tr, Line line, string[] handles, ExtensionDictionaryManager dMan)
        {
            long handle, num, han;
            String handleVal = String.Empty;
            Xrecord xRec;
            foreach (String handleStr in handles)
            {
                handle = long.TryParse(handleStr, out num) ? num : 0;
                Point3d cen = line.StartPoint;
                //Abrimos la geometría existente
                Entity ent = (Entity)handle.GetId().GetObject(OpenMode.ForRead),
                       foundEnt;
                ZoomWindow zWin = new ZoomWindow(cen, 1, 1);
                FenceSelector sel;
                zWin.SetView(1);
                Selector.Ed.Regen();

                //Solo existe como geometría una polilínea o bloque
                sel = new FenceSelector(cen.ToPoint2d(), 0.01d);

                if (ent is BlockReference && sel.Search(this.BlockFilter))
                {
                    UpdateBlockReference(sel, line, ent, out foundEnt, out han);
                    handleVal += foundEnt.Handle.Value + ",";
                }
                else if (ent is Polyline && sel.Search(this.PolylineFilter))
                {
                    foundEnt = sel.First.GetObject(OpenMode.ForRead) as Entity;
                    handleVal += foundEnt.Handle.Value + ",";
                }
                else if (ent is DBText && sel.Search(this.PolylineFilter))
                {
                    foundEnt = sel.First.GetObject(OpenMode.ForRead) as Entity;
                    PolygonSelector selWin = new PolygonSelector((foundEnt as Polyline).Vertices());
                    if (selWin.Search(SelectType.Window, null))
                    {
                        foreach (ObjectId entId in selWin.Ids)
                        {
                            foundEnt = entId.GetObject(OpenMode.ForRead) as Entity;
                            if (ent is DBText)
                            {
                                handleVal += foundEnt.Handle.Value + ",";
                                break;
                            }
                        }
                    }
                }
            }
            if (handleVal != String.Empty)
            {
                //Geometría
                xRec = dMan.GetRegistry(FIELD_GEOMETRY, tr);
                xRec.SetData(tr, handleVal.Substring(0, handleVal.Length - 1));
            }
            //Start
            xRec = dMan.GetRegistry(FIELD_START, tr);
            xRec.SetData(tr, line.StartPoint.ToString());
            //End
            xRec = dMan.GetRegistry(FIELD_END, tr);
            xRec.SetData(tr, line.EndPoint.ToString());
        }
    }
}
