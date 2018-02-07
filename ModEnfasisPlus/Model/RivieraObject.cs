using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.OldModulador.Controller;
using DaSoft.Riviera.OldModulador.Model.Delta;
using DaSoft.Riviera.OldModulador.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Mio;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Entities;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Model;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu.Shapes2D;
using NamelessOld.Libraries.HoukagoTeaTime.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Yui;
using NamelessOld.Libraries.Yggdrasil.Lain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CONST;
using static DaSoft.Riviera.OldModulador.Assets.Strings;
using AutoCADDB = Autodesk.AutoCAD.DatabaseServices.Database;
namespace DaSoft.Riviera.OldModulador.Model
{
    public class RivieraObject
    {
        public event RoutedEventHandler Transformed;
        /// <summary>
        /// La lista de direcciones disponibles 
        /// </summary>
        public Riviera2DArrow[] Arrows;
        /// <summary>
        /// Accede al geometryExtens del objeto
        /// </summary>
        public Point2d[] GeometricExtents
        {
            get
            {
                Point2dCollection pts = new Point2dCollection();
                if (this.Arrows != null)
                    foreach (Riviera2DArrow arr in this.Arrows)
                        foreach (Point2d pt in arr.Vertices)
                            pts.Add(pt);
                pts.Add(this.Start);
                pts.Add(this.End);
                Point2d[] geoExt = new Point2d[2];
                pts.GeometricExtents(out geoExt[0], out geoExt[1]);
                return geoExt;
            }
        }
        /// <summary>
        /// The line represent the box direction
        /// </summary>
        public Line Line;
        /// <summary>
        /// Accede al Handle del objeto
        /// </summary>
        public Handle Handle
        {
            get { return this.Line.Handle; }
        }
        /// <summary>
        /// Se actualiza la línea si cambian los puntos
        /// </summary>
        public Point3d[] Last;
        /// <summary>
        /// Las dimensiones del objeto
        /// </summary>
        Double _Ancho, _Frente, _Alto;
        /// <summary>
        /// El tamaño del objeto formateado
        /// FrenteXAltoXAncho
        /// </summary>
        public String Size
        {
            get { return String.Format("{0}X{1}X{2}", _Frente, _Alto, _Ancho); }
        }
        /// <summary>
        /// Devuelve el tamaño del elemento
        /// </summary>
        /// <value>
        /// The size of the riv.
        /// </value>
        public ElementSize RivSize
        {
            get
            {
                String frente = this.Code.Substring(6, 2);
                String alto = this.Code.Substring(8, 2);
                var fSize = App.DB.Panel_Size.Select(p => p as ElementSize).Union(App.DB.Mampara_Sizes).Where(x => this.Code.Contains(x.Code) && x.Frente == frente && x.Alto == alto).FirstOrDefault();
                return fSize;
            }
        }
        /// <summary>
        /// Define la dirección del rectangulo.
        /// </summary>
        public Vector2d Direction
        {
            get { return new Vector2d(this.Line.EndPoint.X - this.Line.StartPoint.X, this.Line.EndPoint.Y - this.Line.StartPoint.Y); }
        }
        /// <summary>
        /// Genera un rectangulo usando el frente del rectangulo y revisa si la geometría colisiona
        /// con un elemento en la dirección frontal o trasera
        /// </summary>
        /// <param name="size">El elemento seleccionado</param>
        /// <param name="dir">La dirección deseada para validar la colisión</param>
        /// <returns>Verdadero si el elemento colisiona en la dirección deseada</returns>
        public Boolean CollideFrom(RivieraSize size, ArrowDirection dir)
        {
            if (dir == ArrowDirection.Back || dir == ArrowDirection.Front)
            {
                double ancho = size.Ancho / 2,
                       frente = this.Frente,
                       ang = dir == ArrowDirection.Front ? this.Start.GetVectorTo(this.End).Angle : this.End.GetVectorTo(this.Start).Angle,
                       r = this.Direction.Length;
                Buffer2D buffer;
                if (dir == ArrowDirection.Front)
                    buffer = new Buffer2D(this.End, this.End.ToPoint2dByPolar(size.Frente, ang), size.Ancho);
                else
                    buffer = new Buffer2D(this.Start, this.Start.ToPoint2dByPolar(size.Frente, ang), size.Ancho);
                PolygonSelector sel = new PolygonSelector(buffer);
                sel.Search(SelectType.Crossing, null);
                return sel.Filter(this.Ids).Count() > 0;
            }
            else
                return false;
        }

        internal void UpdateChild(Transaction tr, long currentChildId, long newChildId)
        {
            String childKey = this.Children.Where(x => x.Value == currentChildId).FirstOrDefault().Key;
            if (childKey != null)
            {
                this.Children[childKey] = newChildId;
                this.Data.Set(childKey, tr, newChildId.ToString());
            }
        }


        /// <summary>
        /// The id collection
        /// </summary>
        public ObjectIdCollection Ids;
        /// <summary>
        /// El punto de inserción de la etiqueta
        /// </summary>
        public Point2d Start
        {
            get { return this.Line.StartPoint.ToPoint2d(); }
        }
        /// <summary>
        /// El punto de final de la etiqueta
        /// </summary>
        public virtual Point2d End
        {
            get { return this.Line.EndPoint.ToPoint2d(); }
        }
        /// <summary>
        /// El código del elemento
        /// </summary>
        public String Code;
        /// <summary>
        /// El nombre del elemento
        /// </summary>
        public virtual String ItemName { get { return "Mampara"; } }
        /// <summary>
        /// Las llaves del contenido que guarda la mampara
        /// </summary>
        public String[] Keys
        {
            get
            {
                return new String[]
                {
                    FIELD_LEFT_FRONT,
                    FIELD_FRONT,
                    FIELD_RIGHT_FRONT,
                    FIELD_BACK,
                    FIELD_LEFT_BACK,
                    FIELD_RIGHT_BACK,
                };
            }
        }
        /// <summary>
        /// Accede al padre del objeto
        /// </summary>
        public long Parent;
        /// <summary>
        /// Accede a los hijos conectados en la mampara
        /// </summary>
        public Dictionary<String, long> Children;
        /// <summary>
        /// Establece el tamaño del objeto
        /// </summary>
        /// <param name="size">El tamaño a establecer</param>
        public void SetSize(RivieraSize size)
        {
            this._Frente = size.Frente;
            this._Alto = size.Alto;
            this._Ancho = size.Ancho;
        }
        /// <summary>
        /// El contenido enlazado en la interfaz
        /// </summary>
        public Object Content;
        /// <summary>
        /// La información guardada en el objeto
        /// </summary>
        public RivieraData Data;
        /// <summary>
        /// El ancho del rectangulo
        /// </summary>
        public double Ancho
        {
            get
            {
                return _Ancho;
            }
        }

        /// <summary>
        /// El frente del rectangulo
        /// </summary>
        public virtual double Frente
        {
            get
            {
                return _Frente;
            }
        }
        /// <summary>
        /// El alto del rectangulo, solo
        /// es de forma informativa
        /// </summary>
        public double Alto
        {
            get
            {
                return _Alto;
            }
        }
        /// <summary>
        /// Calcula el punto final de la mampara con respecto al punto
        /// inicial de la mampara
        /// </summary>
        /// <param name="mampara">The mampara.</param>
        /// <returns>El punto final de la mampara</returns>
        public Point2d GetEndPoint()
        {
            return this.GetEndPoint(this.Frente);
        }
        /// <summary>
        /// Calcula el punto final de la mampara con respecto al punto
        /// inicial de la mampara
        /// </summary>
        /// <param name="frente">La distancia usada para calcular el punto final</param>
        /// <returns>El punto final de la mampara</returns>
        public Point2d GetEndPoint(double frente, Boolean calc = false)
        {
            if (App.Riviera.Units == DaNTeUnits.Imperial || calc)
                return this.Start.ToPoint2dByPolar(frente, this.Angle);
            else
                return this.End;
        }
        /// <summary>
        /// Obtiene el angulo de la caja de colisión
        /// </summary>
        public double Angle { get { return this.Direction.Angle; } }
        /// <summary>
        /// Verdadero si el objeto tiene hijos,
        /// Necesita que los apuntadores sean distintos de 0
        /// </summary>
        public bool HasChildren
        {
            get
            {
                return this.Children.Values.Where(x => x != 0).Count() > 0;
            }
        }

        public virtual Point3d LineEndPoint { get { return this.Line.StartPoint.ToPoint2d().ToPoint2dByPolar(this.Frente, this.Angle).ToPoint3d(); } }



        /// <summary>
        /// Crea un nuevo rectangulo gráfico
        /// </summary>
        /// <param name="pt0">El punto inicial del rectangulo</param>
        /// <param name="ptf">El punto final del rectangulo</param>
        /// <param name="size">El tamaño del rectangulo</param>
        public RivieraObject(Point2d pt0, Point2d ptf, RivieraSize size, String code, Boolean createContent = true)
        {
            this.Ids = new ObjectIdCollection();
            this.Children = new Dictionary<string, long>();
            foreach (String key in Keys)
                this.Children.Add(key, 0);
            this._Frente = size.Frente;
            this._Alto = size.Alto;
            this._Ancho = size.Ancho;
            this.Code = code;
            this.Line = new Line(pt0.ToPoint3d(), ptf.ToPoint3d());
            if (createContent)
                this.CreateContent();
        }
        /// <summary>
        /// Realiza la carga de la información del elemento
        /// </summary>
        /// <param name="tr">La transacción del elemento</param>
        public void Load(ObjectId id, Transaction tr)
        {
            AutoCADDB db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            this.Children = new Dictionary<string, long>();
            foreach (String key in Keys)
                this.Children[key] = long.Parse(this.Data[key, tr]);
            this.Parent = long.Parse(this.Data[FIELD_PARENT, tr]);
            this.Ids.Add(id);
            string handleStr = this.Data[FIELD_GEOMETRY, tr];
            //Leer la geometría, si la geometría esta corrupta, se crea
            //nueva geometría
            try
            {
                if (handleStr.Contains(','))
                    foreach (string h in handleStr.Split(','))
                        this.Ids.Add(db.GetObjectId(false, new Handle(long.Parse(h)), 0));
                else
                    this.Ids.Add(db.GetObjectId(false, new Handle(long.Parse(this.Data[FIELD_GEOMETRY, tr])), 0));
            }
            catch (Exception exc)
            {
                Selector.Ed.WriteMessage("Se encontro handle desconocido, regenerando la geometría para la entidad {0}:({1})", this.Code, id.ToString());
                App.Riviera.Log.AppendEntry(String.Format("Se encontro handle desconocido, regenerando la geometría para la entidad {0}:({1})\n{2}", this.Code, id.ToString(), exc.Message), Protocol.Warning, "LoadRivieraObject");
                this.Ids.Clear();
                this.Ids.Add(id);
                this.CreateContent();
                this.DrawContent(tr);
                this.Data.Set(FIELD_GEOMETRY, tr, this.SaveGeometry(tr));
            }
            this.Line = this.Ids[0].GetObject(OpenMode.ForRead) as Line;
            //this.Line.Modified += Line_Modified;
            Last = new Point3d[] { this.Line.StartPoint, this.Line.EndPoint };
        }



        /// <summary>
        /// Guarda la información del objeto
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        public virtual void Save(Transaction tr)
        {
            this.Data.Save(tr,
                new String[]
                {
                    this.Code,
                    this.Size,
                    this.Start.ToString(),
                    this.End.ToString(),
                    this.Children[FIELD_LEFT_FRONT].ToString(),
                    this.Children[FIELD_FRONT].ToString(),
                    this.Children[FIELD_RIGHT_FRONT].ToString(),
                    this.Children[FIELD_BACK].ToString(),
                    this.Children[FIELD_LEFT_BACK].ToString(),
                    this.Children[FIELD_RIGHT_BACK].ToString(),
                    this.Parent.ToString(),
                    this.SaveGeometry(tr),
                    String.Empty
                });
        }
        /// <summary>
        /// Guarda la geometría del elemento
        /// </summary>
        /// <returns>La cadena para guardar la geometría del elemento</returns>
        internal string SaveGeometry(Transaction tr)
        {
            String handleVal = String.Empty;
            for (int i = 1; i < this.Ids.Count; i++)
                handleVal += this.Ids[i].OpenEntity(tr).Handle.Value.ToString() + ",";
            return handleVal != String.Empty ? handleVal.Substring(0, handleVal.Length - 1) : String.Empty;
        }
        /// <summary>
        /// Revisa si un objeto esta esta relacionado a este objeto.
        /// </summary>
        /// <param name="obj">El objeto relacionado</param>
        /// <returns>Verdadero si el objeto esta relacionado con esta instancia.</returns>
        public Boolean IsRelated(RivieraObject obj)
        {
            //Checamos si el objeto es padre de esta instancia
            if (obj.Handle.Value == this.Parent)
                return true;
            //Checamos si el objeto es el biombo asociado a esta mampara
            else if ((this is Mampara) && (this as Mampara).BiomboId == obj.Handle.Value)
                return true;
            //Finalmente se revisa si el objeto es un hijo de esta instancia
            else
                return this.Children.Values.Where(x => x != 0).Contains(obj.Handle.Value);
        }
        /// <summary>
        /// Agrega o remplaza un hijo al elemento riviera
        /// </summary>
        /// <param name="dir">La dirección a la que se asocia el hijo</param>
        /// <param name="handle">El handle del hijo</param>
        /// <param name="tr">La transacción activa</param>
        public virtual void AddChildren(ArrowDirection dir, Handle handle, Transaction tr)
        {
            this.Children[dir.GetString()] = handle.Value;
            this.Data.Set(dir.GetString(), tr, handle.Value.ToString());
        }
        /// <summary>
        /// Cretes the content that is linked to the geometry line
        /// </summary>
        /// <returns></returns>
        public virtual void CreateContent()
        {
            Point2d min, max;
            GetPoints(this.Line.StartPoint.ToPoint2d(), this.Line.EndPoint.ToPoint2d(), new RivieraSize() { Alto = this.Alto, Ancho = this.Ancho, Frente = this.Frente }).GeometricExtents(out min, out max);
            Content = new BoundingBox2D(min, max);
        }

        /// <summary>
        /// Muestra las direcciones disponibles
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        public virtual void ShowDirections(Transaction tr, ArrowDirection dir = ArrowDirection.None)
        {
            Double ancho = this.Ancho,
                    d = this.Ancho * 0.6d,
                    h = Math.Sqrt(d * d * 2);
            if (App.Riviera.Units == DaNTeUnits.Imperial)
            {
                ancho = this.Ancho.ConvertUnits(Unit_Type.m, Unit_Type.inches);
                d = d.ConvertUnits(Unit_Type.m, Unit_Type.inches);
                h = h.ConvertUnits(Unit_Type.m, Unit_Type.inches);
            }
            this.Arrows = new Riviera2DArrow[]
            {
                new Riviera2DArrow(this.End.ToPoint2dByPolar(h, this.Angle - Math.PI / 4), ARROW_SIZE, ArrowDirection.Right_Front),
                new Riviera2DArrow(this.End.ToPoint2dByPolar(h, this.Angle + Math.PI / 4), ARROW_SIZE, ArrowDirection.Left_Front),
                new Riviera2DArrow(this.End.ToPoint2dByPolar(ancho, this.Angle), ARROW_SIZE, ArrowDirection.Front),
                new Riviera2DArrow(this.Start.ToPoint2dByPolar(-h, this.Angle - Math.PI / 4), ARROW_SIZE, ArrowDirection.Left_Back),
                new Riviera2DArrow(this.Start.ToPoint2dByPolar(-h, this.Angle + Math.PI / 4), ARROW_SIZE, ArrowDirection.Right_Back),
                new Riviera2DArrow(this.Start.ToPoint2dByPolar(-ancho, this.Angle), ARROW_SIZE, ArrowDirection.Back)
            };

            if (dir == ArrowDirection.Front || dir == ArrowDirection.Left_Front || dir == ArrowDirection.Right_Front || dir == ArrowDirection.None)
            {
                this.Arrows[0].Draw(tr, this.Direction.Angle - Math.PI / 2);
                this.Arrows[1].Draw(tr, this.Direction.Angle + Math.PI / 2);
                this.Arrows[2].Draw(tr, this.Direction.Angle);
            }
            if (dir == ArrowDirection.Back || dir == ArrowDirection.Left_Back || dir == ArrowDirection.Right_Back || dir == ArrowDirection.None)
            {
                this.Arrows[3].Draw(tr, this.Direction.Angle + Math.PI / 2);
                this.Arrows[4].Draw(tr, this.Direction.Angle - Math.PI / 2);
                this.Arrows[5].Draw(tr, this.Direction.Angle + Math.PI);
            }
        }
        /// <summary>
        /// Realiza un regen en el dibujo para visualizar
        /// solo las flechas que no chocan con ningun objeto
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        public void RegenArrows(Transaction tr)
        {
            if (this.Arrows != null)
                for (int i = 0; i < this.Arrows.Length; i++)
                    if (this.Arrows[i] != null)
                        this.Arrows[i] = this.Arrows[i].ArrowCollides(tr) ? null : this.Arrows[i];

        }
        /// <summary>
        /// Dibuja el rectangulo
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        /// <param name="input">La entrada de la transacción</param>
        public void Draw(Transaction tr, object input)
        {
            Drawing drawing = new Drawing(this.Line) { Layername = LAYER_RIVIERA_OBJECT };
            drawing.Draw();
            this.Ids.Add(drawing.FirstId);
            this.Data = new RivieraData(this.Ids[0], tr);
            this.DrawContent(tr);
            this.Line.EndPoint = this.LineEndPoint;
            DaNTeMemory idC = new DaNTeMemory(this.Line.Id, tr);
            idC.SetId(this.Line.Handle.Value, tr);
            idC.SetUnits(App.Riviera.Units, tr);
            if (App.Riviera.Units == DaNTeUnits.Imperial && !(this is RivieraPanel))
                this.Ids.Transform(Matrix3d.Scaling(IMPERIAL_FACTOR, this.Line.StartPoint), tr);
            Last = new Point3d[] { this.Line.StartPoint, this.Line.EndPoint };

        }
        /// <summary>
        /// Busca la entidad que contiene la aplicación
        /// </summary>
        /// <param name="drawBox">La caja dibujada</param>
        /// <returns>El elemento encontrado</returns>
        public ObjectIdCollection FindGeometry(Boolean drawBox)
        {
            Point2d min, max, s, e;
            Double ang = this.Angle;
            const int OFFSET = 5 / 100;
            s = this.Start.ToPoint2dByPolar(OFFSET, ang - Math.PI);
            e = this.End.ToPoint2dByPolar(OFFSET, ang);
            min = s.ToPoint2dByPolar(OFFSET, ang + Math.PI / 2);
            max = e.ToPoint2dByPolar(OFFSET, ang - Math.PI / 2);

            BoundingBox2D box = new BoundingBox2D(min, max);
            if (drawBox)
                Drawer.Geometry2D(box, Color.FromRgb(0, 255, 0));

            PolygonSelector sel = new PolygonSelector(box);
            sel.Search(SelectType.Window, null);



            return sel.Ids;
        }

        public virtual void DrawContent(Transaction tr)
        {
            Geometry2D geo = this.Content as Geometry2D;
            AutoCADLayer lay = new AutoCADLayer(LAYER_RIVIERA_GEOMETRY, tr);
            lay.SetStatus(LayerStatus.EnableStatus, tr);
            this.Ids.Add(Drawer.Geometry2D(geo, tr, closed: true));
            //Se agregán a una capa especial
            lay.AddToLayer(new ObjectIdCollection(this.Ids.OfType<ObjectId>().Where(x => this.Ids.IndexOf(x) > 0).ToArray()), tr);
        }

        private void Line_Modified(object sender, EventArgs e)
        {
            if (this.Line.StartPoint != Last[0] || this.Line.EndPoint != Last[1])
                this.Regen();
        }
        /// <summary>
        /// Elimina el objeto seleccionado
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        public void Delete(Transaction tr)
        {
            //Se borra la geometria y se elimina del cache
            App.DB.Remove(this.Handle.Value);
            Drawer.Erase(this.Ids, tr);
        }
        /// <summary>
        /// Realiza la selección de objetos asociados a esta instancia
        /// Sí la base de datos no esta inicializada no se pueden obtener
        /// los objetos relacionados.
        /// </summary>
        /// <returns>La colección de objetos asociados.</returns>
        public IEnumerable<RivieraObject> GetObjects()
        {
            IEnumerable<long> handles = this.Children.Values.Where(x => x != 0);
            if (this.Parent != 0)
                handles = handles.Union(new long[] { this.Parent });
            if (this is Model.Delta.Mampara && (this as Mampara).BiomboId != 0)
                handles = handles.Union(new long[] { (this as Mampara).BiomboId });
            if (App.DB.Objects != null)
                return handles.Select(y => App.DB[y]).Where(x => x != null);
            else
                return new RivieraObject[0];
        }
        /// <summary>
        /// Realiza una validación entre las relaciones definidas en el objeto contra
        /// los apuntaodres que tiene guardado el elemento
        /// </summary>
        /// <param name="objects">Los objetos ha validar</param>
        /// <returns>Verdadero si el número de punteros coincide con la relación definida</returns>
        public Boolean CheckPointersVsRelations(IEnumerable<RivieraObject> objects)
        {
            int pointers = objects.Count(),
                relations = objects.Count(x => x.IsRelated(this));
            return pointers == relations;

        }
        /// <summary>
        /// Realiza una actualización de la geometría
        /// </summary>
        public void Regen()
        {
            this.CreateContent();
            VoidTransactionWrapper<Object> trW
                = new VoidTransactionWrapper<object>(delegate (Document doc, Transaction tr, Object[] data)
                {
                    if (this.Ids.Count == 2)
                    {
                        Drawer.Erase(tr, this.Ids[1]);
                        this.Ids.RemoveAt(1);
                    }
                    this.DrawContent(tr);

                });
            trW.Run(null);
        }
        /// <summary>
        /// Realiza una actualización de la geometría
        /// </summary>
        public virtual void Regen(Transaction tr)
        {
            this.CreateContent();
            if (this.Ids.Count == 2)
            {
                Drawer.Erase(tr, this.Ids[1]);
                this.Ids.RemoveAt(1);
            }
            this.DrawContent(tr);
            if (App.Riviera.Units == DaNTeUnits.Imperial)
            {
                Line l = (Line)this.Line.Id.GetObject(OpenMode.ForWrite);
                l.EndPoint = this.GetEndPoint().ToPoint3d(l.EndPoint.Z);
            }
            if (App.Riviera.Units == DaNTeUnits.Imperial)
                this.Ids.Transform(Matrix3d.Scaling(IMPERIAL_FACTOR, this.Line.StartPoint), tr);
            this.Data.Set(FIELD_GEOMETRY, tr, SaveGeometry(tr));
        }
        /// <summary>
        /// Realiza una transformación del elemento seleccionado
        /// </summary>
        /// <param name="matrix">La matriz de transformación</param>
        /// <param name="tr">La transacción activa</param>
        /// <param name="recursive">La opción recursiva aplica para todos los hijos de la mampara</param>
        public void Transform(Matrix3d matrix, Transaction tr, Boolean recursive = true)
        {
            foreach (ObjectId id in this.Ids)
                id.TransformBy(tr, matrix);
            RivieraObject obj;
            if (recursive)
            {
                foreach (long childId in this.Children.Values.Where(x => x != 0))
                {
                    obj = App.DB[childId];
                    if (obj != null)
                        obj.Transform(matrix, tr);

                }
                if (this is Delta.MamparaJoint && (this as Delta.MamparaJoint).PanelArray != null)
                {
                    foreach (var panel in (this as Delta.MamparaJoint).PanelArray.Panels)
                        panel.Transform(matrix, tr);
                }
            }
            else
            {
                if (this is Delta.MamparaJoint && (this as Delta.MamparaJoint).PanelArray != null)
                {
                    foreach (var panel in (this as Delta.MamparaJoint).PanelArray.Panels)
                        panel.Transform(matrix, tr);
                }
            }
            if (this is Delta.Mampara && (this as Delta.Mampara).BiomboId != 0)
                App.DB[(this as Delta.Mampara).BiomboId]?.Transform(matrix, tr);
            if (Transformed != null)
                Transformed(this, new TransformArgs(matrix, tr));

        }

        /// <summary>
        /// Calcula la geometría del rectangulo
        /// </summary>
        /// <param name="pt0">El punto inicial de la dirección del rectangulo</param>
        /// <param name="ptf">El punto final de la dirección del rectangulo</param>
        /// <param name="size">Las dimensiones del rectangulo</param>
        /// <returns>La geometría que define al rectangulo</returns>
        private Point2dCollection GetPoints(Point2d pt0, Point2d ptf, RivieraSize size)
        {
            Double ang = new Vector2d(pt0.DeltaX(ptf), pt0.DeltaY(ptf)).Angle,
                   wHalf = size.Ancho / 2;
            ptf = pt0.ToPoint2dByPolar(size.Frente, ang);
            Point2d[] pts = new Point2d[]
            {
                pt0.ToPoint2dByPolar(wHalf, ang - Math.PI/2),
                ptf.ToPoint2dByPolar(wHalf, ang - Math.PI/2),
                ptf.ToPoint2dByPolar(wHalf, ang + Math.PI/2),
                pt0.ToPoint2dByPolar(wHalf, ang + Math.PI/2),
            };
            return new Point2dCollection(pts);

        }

        /// <summary>
        /// Selecciona una dirección de dibujo
        /// </summary>
        /// <returns>La flecha seleccionada</returns>
        public ArrowDirection PickDirection(Transaction tr)
        {
            ArrowDirection dir = ArrowDirection.None;
            ObjectId id;
            Riviera2DArrow arr;
            if (Selector.ObjectId<Polyline>(SEL_ARROW_DIR, out id))
            {
                arr = this.Arrows.Where(a => a != null).Where(x => x.Id[0] == id).FirstOrDefault();
                if (arr != null)
                    dir = arr.Direction;
            }
            foreach (Riviera2DArrow a in this.Arrows.Where(x => x != null))
                a.Erase(tr);
            this.Arrows = null;
            return dir;
        }

        public override string ToString()
        {
            return String.Format("{0}({1}), {2}", this.GetType().Name, this.Code, this.Handle.Value);
        }
    }
}
