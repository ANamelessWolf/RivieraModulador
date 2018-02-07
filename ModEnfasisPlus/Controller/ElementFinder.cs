using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.OldModulador.Model;
using DaSoft.Riviera.OldModulador.Model.Delta;
using DaSoft.Riviera.OldModulador.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Mio;
using NamelessOld.Libraries.HoukagoTeaTime.MugiChan;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu;
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
    public class ElementFinder
    {
        Double[] DirectionOffset
        {
            get
            {
                //ArrowDirection.Left = 0,
                //ArrowDirection.Front = 1,
                //ArrowDirection.Right = 2,
                double ancho = this.CodeType == DT_JOINT ? (ANCHO_M / 2) : 0.0530d;
                if (App.Riviera.Units == DaNTeUnits.Imperial)
                    ancho = ancho.ConvertUnits(Unit_Type.m, Unit_Type.inches);
                return new Double[]
                {
                    ancho,
                    ancho,
                    ancho,
                };
            }
        }
        /// <summary>
        /// La clase para realizar la selección
        /// </summary>
        public AreaSelector Selector;
        /// <summary>
        /// En caso de tener un panel doble se usa para encontrar la ubicación de los paneles dobles.
        /// </summary>
        public RivieraPanelDoubleLocation PanelDoubleLocation;
        /// <summary>
        /// La clase para realizar la selección
        /// </summary>
        public FenceSelector AsFenceSelector
        {
            get { return this.Selector as FenceSelector; }
        }
        /// <summary>
        /// El punto inicial del elemento base
        /// </summary>
        public Point3d Start;
        /// <summary>
        /// El punto final del elemento base
        /// </summary>
        public Point3d End;
        /// <summary>
        /// La geometría del objeto seleccionado
        /// </summary>
        public Line Line;
        /// <summary>
        /// El id de la geometría seleccionada
        /// </summary>
        public ObjectId Id { get { return this.Line.Id; } }
        /// <summary>
        /// Define el tipo de código de elemento a buscar
        /// DD7 cajoneras y pichoneras
        /// DD8 Biombos
        /// DD1 Mampara
        /// DD3 Remate final
        /// PanelStack Arreglo de paneles
        /// </summary>
        public String CodeType;
        /// <summary>
        /// Los codigos para elementos de tipo biombo, cajoneras o pichoneras
        /// solo tienen padre y se buscan con la dirección same
        /// </summary>
        const String BiomboCodeTypes = "DD7DD8";
        /// <summary>
        /// Los codigos para elementos de tipo mampara
        /// </summary>
        const String MamparaCodeTypes = "DD1";
        /// <summary>
        /// Los codigos para elementos de tipo Remate final
        /// </summary>
        const String RemateFinalCodeTypes = "DD3";
        /// <summary>
        /// Los codigos para arreglos de paneles
        /// </summary>
        const String PanelStackCodeTypes = PANEL_STACK;
        /// <summary>
        /// Los códigos para arreglar las uniones
        /// </summary>
        const String JointCodeTypes = JOINT + DT_JOINT;
        Dictionary<String, long> Pointers;
        long Parent;
        /// <summary>
        /// El filtro para seleccionar los elementos de riviera object
        /// </summary>
        public SelectionFilter Filter
        {
            get
            {
                SelectionFilterBuilder sb = new SelectionFilterBuilder(typeof(Line));
                sb.SetAllowedLayers(LAYER_RIVIERA_OBJECT);
                return sb.Filter;
            }
        }

        public ElementFinder(String code, Line line, Dictionary<String, long> pointers, long parent)
        {
            this.Start = line.StartPoint;
            this.End = line.EndPoint;
            this.Line = line;
            this.Pointers = pointers;
            this.Parent = parent;
            if (code == PANEL_STACK || code == DT_JOINT || code == JOINT)
                this.CodeType = code;
            else
                this.CodeType = code.Substring(0, 3);
        }
        /// <summary>
        /// Realiza el dibujo del elemnto a seleccionar
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        public void Draw(Transaction tr)
        {
            Polyline pl = new Polyline();
            for (int i = 0; i < this.Selector.Vertices.Count; i++)
                pl.AddVertexAt(i, this.Selector.Vertices[i].ToPoint2d(), 0, 0, 0);
            Drawer.Entity(tr, pl);
        }
        /// <summary>
        /// Realiza el proceso de busqueda del elemento
        /// </summary>
        /// <param name="direction">La dirección a buscar el elemento</param>
        /// <returns>El id del elemento seleccionado</returns>
        public ObjectId Find(PointerDirection direction)
        {
            Double ancho = ANCHO_M,
                   searchRadius = PAN_ANCHO_MM.ConvertUnits(Unit_Type.mm, Unit_Type.m) / 2,
                   half = ANCHO_HALF_MAMPARA_M,
                   jointAncho = 0.00005;
            if (App.Riviera.Units == DaNTeUnits.Imperial)
            {
                ancho = ancho.ConvertUnits(Unit_Type.m, Unit_Type.inches);
                searchRadius = searchRadius.ConvertUnits(Unit_Type.m, Unit_Type.inches);
                half = half.ConvertUnits(Unit_Type.m, Unit_Type.inches);
            }
            Point3d center;
            Double ang;



            switch (direction)
            {
                case PointerDirection.Parent:
                    #region Parent
                    if (PanelStackCodeTypes.Contains(this.CodeType))
                    {
                        Double searchAngle;
                        ang = this.Start.ToPoint2d().GetVectorTo(this.End.ToPoint2d()).Angle;
                        searchAngle = ang + Math.PI / 2;
                        center = this.Start.MiddlePointTo(this.End).ToPoint2d().ToPoint2dByPolar(half, searchAngle).ToPoint3d();
                        this.Selector = new FenceSelector(center.ToPoint2d(), searchRadius);
                        if (!this.AsFenceSelector.Search(this.Filter))
                        {
                            searchAngle = ang - Math.PI / 2;
                            center = this.Start.MiddlePointTo(this.End).ToPoint2d().ToPoint2dByPolar(half, searchAngle).ToPoint3d();
                            this.Selector = new FenceSelector(center.ToPoint2d(), searchRadius);
                            this.Zoom(center);
                            this.AsFenceSelector.Search(this.Filter);
                        }
                    }
                    else if (BiomboCodeTypes.Contains(this.CodeType))
                    {
                        center = this.Start.MiddlePointTo(this.End);
                        this.Selector = new FenceSelector(center.ToPoint2d(), searchRadius);
                        this.Zoom(center);
                        this.AsFenceSelector.Search(this.Filter);
                    }
                    else if (RemateFinalCodeTypes.Contains(this.CodeType))
                    {
                        center = this.Start;
                        this.Selector = new FenceSelector(center.ToPoint2d(), searchRadius);
                        this.Zoom(center);
                        this.AsFenceSelector.Search(this.Filter);
                    }
                    else if (MamparaCodeTypes.Contains(this.CodeType))
                    {
                        if (this.Parent != 0 && this.Pointers[FIELD_FRONT] != 0)
                            center = this.Start;
                        else if (this.Parent != 0 && this.Pointers[FIELD_BACK] == 0)
                            center = this.End;
                        else
                            center = new Point3d();
                        if (center.X != 0 && center.Y != 0)
                        {
                            Point3d left = PreviousPoint(center, ArrowDirection.Right_Front),
                                    right = PreviousPoint(PreviousPoint(PreviousPoint(center, ArrowDirection.Front), ArrowDirection.Front), ArrowDirection.Left_Front);

                            Double minX = left.X > right.X ? right.X : left.X,
                                   minY = left.Y > right.Y ? right.Y : left.Y,
                                   maxX = left.X < right.X ? right.X : left.X,
                                   maxY = left.Y < right.Y ? right.Y : left.Y;

                            this.Selector = new WindowSelector(new Point3d(minX, minY, 0), new Point3d(maxX, maxY, 0));
                            this.Zoom(center);
                            int tot = this.Selector.Ed.SelectWindow(new Point3d(minX, minY, 0), new Point3d(maxX, maxY, 0), this.Filter).Value.GetObjectIds().Count();
                            (this.Selector as WindowSelector).Search(SelectType.Window, this.Filter);
                            //Drawer.Entity(new Line(new Point3d(minX, minY, 0), new Point3d(maxX, maxY, 0)));
                        }
                    }
                    else if (JointCodeTypes.Contains(this.CodeType))
                    {
                        if (this.Parent != 0)
                            center = this.Start;
                        else
                            center = new Point3d();
                        if (center.X != 0 && center.Y != 0)
                        {
                            this.Selector = new FenceSelector(center.ToPoint2d(), searchRadius);
                            this.Zoom(center);
                            this.AsFenceSelector.Search(this.Filter);
                        }
                    }
                    #endregion
                    break;
                case PointerDirection.Back:
                    if (this.Pointers[FIELD_BACK] != 0)
                    {
                        center = this.Start;
                        ang = this.Line.StartPoint.ToPoint2d().GetVectorTo(this.End.ToPoint2d()).Angle + Math.PI;
                        center = this.Start.ToPoint2d().ToPoint2dByPolar(searchRadius * 2, ang).ToPoint3d();
                        this.Selector = new FenceSelector(center.ToPoint2d(), searchRadius);
                        this.Zoom(center);
                        this.AsFenceSelector.Search(this.Filter);
                    }
                    break;
                case PointerDirection.Front:
                    if (this.Pointers[FIELD_FRONT] != 0)
                    {
                        if (JointCodeTypes.Contains(this.CodeType))
                            center = this.NextPoint(this.NextPoint(this.Start, ArrowDirection.Front), ArrowDirection.Front);
                        else
                        {
                            center = this.End;
                            ang = this.Line.StartPoint.ToPoint2d().GetVectorTo(this.End.ToPoint2d()).Angle;
                            center = this.End.ToPoint2d().ToPoint2dByPolar(searchRadius * 2, ang).ToPoint3d();
                        }
                        this.Selector = new FenceSelector(center.ToPoint2d(), searchRadius);
                        this.Zoom(center);
                        this.AsFenceSelector.Search(this.Filter);
                        if (this.Selector.Ids.Count == 0)
                        {
                            center = this.End;
                            this.Selector = new FenceSelector(center.ToPoint2d(), jointAncho);
                            this.AsFenceSelector.Search(this.Filter);
                        }
                    }
                    break;
                case PointerDirection.Left:
                    if (this.Pointers[FIELD_LEFT_BACK] != 0 || this.Pointers[FIELD_LEFT_FRONT] != 0)
                    {
                        if (JointCodeTypes.Contains(this.CodeType))
                            center = this.NextPoint(this.NextPoint(this.Start, ArrowDirection.Front), ArrowDirection.Left_Front);
                        else
                        {
                            ang = this.Start.ToPoint2d().GetVectorTo(this.End.ToPoint2d()).Angle + Math.PI / 2;
                            center = this.Start.MiddlePointTo(this.End).ToPoint2d().ToPoint2dByPolar(half, ang).ToPoint3d();
                        }
                        this.Selector = new FenceSelector(center.ToPoint2d(), searchRadius);
                        this.Zoom(center);
                        this.AsFenceSelector.Search(this.Filter);
                    }
                    break;
                case PointerDirection.Right:
                    if (this.Pointers[FIELD_RIGHT_BACK] != 0 || this.Pointers[FIELD_RIGHT_FRONT] != 0)
                    {
                        if (JointCodeTypes.Contains(this.CodeType))
                            center = this.NextPoint(this.NextPoint(this.Start, ArrowDirection.Front), ArrowDirection.Right_Front);
                        else
                        {
                            ang = this.Start.ToPoint2d().GetVectorTo(this.End.ToPoint2d()).Angle - Math.PI / 2;
                            center = this.Start.MiddlePointTo(this.End).ToPoint2d().ToPoint2dByPolar(half, ang).ToPoint3d();
                        }
                        this.Selector = new FenceSelector(center.ToPoint2d(), searchRadius);
                        this.Zoom(center);
                        this.AsFenceSelector.Search(this.Filter);
                    }
                    break;
                case PointerDirection.DoubleFront:
                    center = this.PanelDoubleLocation.LeftBottomLocationStart.MiddlePointTo(this.PanelDoubleLocation.RightBottomLocationEnd).ToPoint3d();
                    this.Selector = new FenceSelector(center.ToPoint2d(), searchRadius);
                    this.Zoom(center);
                    this.AsFenceSelector.Search(this.Filter);
                    break;
                case PointerDirection.DoubleBack:
                    center = this.PanelDoubleLocation.LeftTopLocationStart.MiddlePointTo(this.PanelDoubleLocation.RightTopLocationEnd).ToPoint3d();
                    this.Selector = new FenceSelector(center.ToPoint2d(), searchRadius);
                    this.Zoom(center);
                    this.AsFenceSelector.Search(this.Filter);
                    break;
                case PointerDirection.DoubleLeft:
                    center = this.PanelDoubleLocation.LeftTopLocationStart.MiddlePointTo(this.PanelDoubleLocation.LeftTopLocationEnd).ToPoint3d();
                    this.Selector = new FenceSelector(center.ToPoint2d(), searchRadius);
                    this.Zoom(center);
                    this.AsFenceSelector.Search(this.Filter);
                    break;
                case PointerDirection.DoubleRight:
                    center = this.PanelDoubleLocation.RightTopLocationStart.MiddlePointTo(this.PanelDoubleLocation.RightTopLocationEnd).ToPoint3d();
                    this.Selector = new FenceSelector(center.ToPoint2d(), searchRadius);
                    this.Zoom(center);
                    this.AsFenceSelector.Search(this.Filter);
                    break;
                default:
                    center = this.Start.MiddlePointTo(this.End);
                    this.Selector = new FenceSelector(center.ToPoint2d(), searchRadius);
                    this.Zoom(center);
                    this.AsFenceSelector.Search(this.Filter);
                    break;
            }
            if (this.Selector != null && (JointCodeTypes.Contains(this.CodeType) || (BiomboCodeTypes + RemateFinalCodeTypes + PanelStackCodeTypes).Contains(this.CodeType)))
            {
                ObjectId[] ids = this.Selector.Ids.OfType<ObjectId>().Where(x => x.ToString() != this.Id.ToString()).ToArray();
                if (ids.Length == 1)
                    return ids[0];
                else
                {
                    return new BlankTransactionWrapper<ObjectId>(delegate (Document doc, Transaction tr)
                     {
                         ObjectId foundId = new ObjectId();
                         ExtensionDictionaryManager dMan;
                         Xrecord xRec;
                         String code, raw;
                         foreach (ObjectId id in ids)
                         {
                             try
                             {
                                 dMan = new ExtensionDictionaryManager(id, tr);
                                 xRec = dMan.GetRegistry(FIELD_CODE, tr);
                                 code = xRec.GetDataAsString(tr).FirstOrDefault();
                                 if (code.Contains("DD1"))
                                 {
                                     foundId = id;
                                     break;
                                 }
                                 else if (code.Contains("PItem"))
                                 {
                                   //  foundId = id;
                                     xRec = dMan.GetRegistry(FIELD_EXTRA, tr);
                                     raw = xRec.GetDataAsString(tr).FirstOrDefault();
                                     PanelRaw pR = PanelRaw.Parse(raw)[0];
                                     if ((direction == PointerDirection.DoubleLeft && pR.Direction == ArrowDirection.Left_Back) ||
                                         (direction == PointerDirection.DoubleRight && pR.Direction == ArrowDirection.Right_Back))
                                         foundId = id;
                                 }
                             }
                             catch (Exception exc)
                             {
                                 App.Riviera.Log.AppendEntry(exc.Message, NamelessOld.Libraries.Yggdrasil.Lain.Protocol.Error, "Find", "ElementFinder");
                             }
                         }
                         return foundId;
                     }).Run();
                }
            }
            else if (this.Selector != null && (direction == PointerDirection.Front || direction == PointerDirection.Parent) && this.CodeType == "DD1")
            {
                ObjectId[] ids = this.Selector.Ids.OfType<ObjectId>().Where(x => x.ToString() != this.Id.ToString()).ToArray();
                if (ids.Length == 1)
                    return ids[0];
                else
                {
                    return new BlankTransactionWrapper<ObjectId>(delegate (Document doc, Transaction tr)
                    {
                        ObjectId foundId = new ObjectId();
                        ExtensionDictionaryManager dMan;
                        Xrecord xRec;
                        String code;
                        foreach (ObjectId id in ids)
                        {
                            try
                            {
                                dMan = new ExtensionDictionaryManager(id, tr);
                                xRec = dMan.GetRegistry(FIELD_CODE, tr);
                                code = xRec.GetDataAsString(tr).FirstOrDefault();
                                if (code == DT_JOINT || code == JOINT)
                                {
                                    foundId = id;
                                    break;
                                }
                            }
                            catch (Exception exc)
                            {
                                App.Riviera.Log.AppendEntry(exc.Message, NamelessOld.Libraries.Yggdrasil.Lain.Protocol.Error, "Find", "ElementFinder");
                            }
                        }
                        return foundId;
                    }).Run();
                }
            }
            else if (this.Selector != null)
                return this.Selector.Ids.OfType<ObjectId>().Where(x => x.ToString() != this.Id.ToString()).FirstOrDefault();
            else
                return new ObjectId();
        }
        public ArrowDirection GetDirection(ArrowDirection dir)
        {
            if (dir == ArrowDirection.Back)
                dir = ArrowDirection.Front;
            else if (dir == ArrowDirection.Left_Back)
                dir = ArrowDirection.Right_Front;
            else if (dir == ArrowDirection.Right_Back)
                dir = ArrowDirection.Left_Front;
            return dir;
        }
        public Point3d NextPoint(Point3d origin, ArrowDirection dir)
        {
            dir = this.GetDirection(dir);
            Double[] r = this.DirectionOffset,
                     angOffset = new double[]
                     {
                         Math.PI/2,//+90
                         0,
                         -Math.PI/2//-90
                     };
            Double ang = this.Start.ToPoint2d().GetVectorTo(this.End.ToPoint2d()).Angle;
            ang = ang + angOffset[(int)dir];
            return origin.ToPoint2d().ToPoint2dByPolar(r[(int)dir], ang).ToPoint3d();
        }
        public Point3d PreviousPoint(Point3d origin, ArrowDirection dir)
        {
            dir = this.GetDirection(dir);
            Double[] r = this.DirectionOffset,
                     angOffset = new double[]
                     {
                         Math.PI/2,//+90
                         0,
                         -Math.PI/2//-90
                     };
            Double ang = this.End.ToPoint2d().GetVectorTo(this.Start.ToPoint2d()).Angle;
            ang = ang + angOffset[(int)dir];
            return origin.ToPoint2d().ToPoint2dByPolar(r[(int)dir], ang).ToPoint3d();
        }

        private void Zoom(Point3d center)
        {
            double windowSize = 2;
            if (App.Riviera.Units == DaNTeUnits.Imperial)
                windowSize = windowSize.ConvertUnits(Unit_Type.m, Unit_Type.inches);
            ZoomWindow zWin = new ZoomWindow(this.Start.MiddlePointTo(this.End), windowSize, windowSize);
            zWin.SetView(1);
            Selector.Ed.Regen();
        }
    }

}