using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.OldModulador.Controller;
using DaSoft.Riviera.OldModulador.Controller.Delta;
using DaSoft.Riviera.OldModulador.Model.Delta;
using DaSoft.Riviera.OldModulador.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Mio;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Model;
using NamelessOld.Libraries.HoukagoTeaTime.MugiChan;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu;
using NamelessOld.Libraries.HoukagoTeaTime.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CODE;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CONST;
using static DaSoft.Riviera.OldModulador.Assets.Strings;
namespace DaSoft.Riviera.OldModulador.Model
{
    public class JointObject : RivieraObject
    {

        /// <summary>
        /// Devuelve la información extra de la unión de Mampara
        /// El primer valor indica si la mampara tiene asociado un arreglo de paneles dobles.
        /// </summary>
        public String Extra
        {
            get { return HasDoublePanels ? String.Format("{0}|{1}", HasDoublePanels, this.PanelArray.Data) : String.Empty; }

        }
        /// <summary>
        /// El id del panel asignado a la posición izquierda
        /// </summary>
        public long PanelDoubleLeftId;
        /// <summary>
        /// El id del panel asignado a la posición derecha
        /// </summary>
        public long PanelDoubleRighttId;
        /// <summary>
        /// El id del panel doble
        /// </summary>
        public long PanelDoubleId;
        /// <summary>
        /// El id del panel doble
        /// </summary>
        public long PanelDoubleBottomId;
        /// <summary>
        /// El id del biombo
        /// </summary>
        public long BiomboId;

        /// <summary>
        /// Verdadero cuando la mampara tiene asociado un panel doble
        /// </summary>
        public Boolean HasDoublePanels;
        /// <summary>
        /// El arreglo de paneles asociados a esta unión.
        /// </summary>
        public RivieraDoublePanelArray PanelArray;
        /// <summary>
        /// Define los offsets para la inserción del siguiente punto, 
        /// dependiendo la dirección a insertar
        /// Los indices que usa este arreglo se definen con los enumeradores
        /// de ArrowDirection
        /// Todos los offsets estan en m
        /// </summary>
        public virtual Double[] DirectionOffset
        {
            get
            {
                //ArrowDirection.Left_Front = 0,
                //ArrowDirection.Front = 1,
                //ArrowDirection.Right_Front = 2,
                double ancho = 0.0530d;
                if (App.Riviera.Units == DaNTeUnits.Imperial)
                    ancho = ancho.ConvertUnits(Unit_Type.m, Unit_Type.inches);
                return new Double[]
                {
                    ancho,
                    0d,
                    ancho,
                };
            }
        }
        /// <summary>
        /// Aunque la union es un punto, para que pueda tener dirección,
        /// se utiliza la unión como una recta de una decima de milimetro.
        /// </summary>
        /// <param name="pt">El punto de unión</param>
        /// <param name="angDirection">La dirección de la unión</param>
        /// <returns>El punto final de la unión</returns>
        static Point2d GetEndPoint(Point2d pt, double angDirection)
        {
            Double frente = DefaultSize.Frente;

            return pt.ToPoint2dByPolar(frente, angDirection);
        }
        /// <summary>
        /// Convierte el objeto a tipo de union de cuantificaión
        /// </summary>
        /// <returns>El objeto cuantificable</returns>
        public QuantifiableUnion QuantifyableUnion()
        {
            return new QuantifiableUnion(this);
        }

        /// <summary>
        /// El tamaño default de la unión es una decima de milimetro
        /// </summary>
        static RivieraSize DefaultSize
        {
            get
            {
                return new RivieraSize()
                {
                    Alto = 0,
                    Ancho = 0,
                    Code = String.Empty,
                    Frente = 0.0001
                };
            }
        }
        /// <summary>
        /// Devuelve el tipo de unión al que pertenece la articulación
        /// </summary>
        public virtual JointType Type
        {
            get { return JointType.Joint_I; }
        }



        /// <summary>
        /// Crea una unión
        /// </summary>
        /// <param name="pt">El punto de la únion</param>
        /// <param name="angDirection">La dirección que tiene la articulación</param>
        public JointObject(Point2d pt, double angDirection) :
            base(pt, GetEndPoint(pt, angDirection), DefaultSize, JOINT)
        {

        }
        /// <summary>
        /// Realiza la carga del campo extra para la unión seleccionada.
        /// </summary>
        /// <param name="v">El campo extra</param>
        public void LoadExtra(string[] data)
        {
            if (data != null)
            {
                //Primer campo data
                this.HasDoublePanels = Boolean.Parse(data[0]);
                this.PanelDoubleLeftId = long.Parse(data[1]);
                this.PanelDoubleRighttId = long.Parse(data[2]);
                this.PanelDoubleId = long.Parse(data[3]);
                if (data.Length > 4)
                {
                    VoidTransactionWrapper<long> vT = new VoidTransactionWrapper<long>((Document doc, Transaction tr, long[] ids) =>
                    {
                        for (int i = 0; i < ids.Length; i++)
                        {
                            Entity ent = ids[i].GetId().OpenEntity(tr);
                            if (ent.ExtensionDictionary.IsValid)
                            {
                                ExtensionDictionaryManager dman = new ExtensionDictionaryManager(ent.ObjectId, tr);
                                var eData = dman.GetRegistry(FIELD_EXTRA, tr).GetDataAsString(tr);
                                if (eData[0].Split('|')[2] == "DD2011")
                                    this.PanelDoubleBottomId = ent.Id.Handle.Value;
                                else
                                    this.BiomboId = ent.Id.Handle.Value;
                            }
                        }
                    });
                    vT.Run(data.Skip(4).Select(x => long.Parse(x)).ToArray());
                }
            }
        }
        /// <summary>
        /// Convierte una mampara de tipo Joint a una mampara de tipo
        /// mamparaJoint
        /// </summary>
        /// <param name="dir">la dirección de la mampara a seleccionar.</param>
        public MamparaJoint Upgrade(Transaction tr, ArrowDirection dir)
        {
            MamparaJoint mJ;
            Point2d pt;
            Matrix3d moveTransform;
            Double ancho = ANCHO_M;
            if (App.Riviera.Units == DaNTeUnits.Imperial)
                ancho = ancho.ConvertUnits(Unit_Type.m, Unit_Type.inches);
            if (dir == ArrowDirection.Back)
            {
                pt = this.Start.ToPoint2dByPolar(ancho, this.Angle + Math.PI);
                moveTransform = Matrix3d.Displacement(this.Start.ToPoint3d().GetVectorTo(pt.ToPoint3d()));
            }
            else
            {
                pt = this.Start;
                moveTransform = Matrix3d.Displacement(this.Start.ToPoint3d().GetVectorTo(this.Start.ToPoint2dByPolar(ancho, this.Angle).ToPoint3d()));
            }
            mJ = new MamparaJoint(pt, this.Angle);
            mJ.Draw(tr, null);
            mJ.Parent = this.Parent;
            if (this.Parent != 0)
            {
                App.DB[this.Parent].UpdateChild(tr, this.Handle.Value, mJ.Handle.Value);
                if (dir == ArrowDirection.Back)
                    this.Move(this.Parent, this, moveTransform, tr);
            }
            foreach (string childKey in this.Children.Keys)
            {
                mJ.Children[childKey] = this.Children[childKey];
                if (this.Children[childKey] != 0)
                {
                    App.DB[this.Children[childKey]].Parent = mJ.Handle.Value;
                    App.DB[this.Children[childKey]].Data.Set(FIELD_PARENT, tr, mJ.Handle.Value.ToString());
                    if (dir == ArrowDirection.Front)
                        App.DB[this.Children[childKey]].Transform(moveTransform, tr, true);
                }
            }

            this.Delete(tr);
            mJ.Save(tr);
            App.DB.Objects.Add(mJ);
            return mJ;
        }
        /// <summary>
        /// Realiza el movimiento de la mampara en dirección ascendente
        /// </summary>
        /// <param name="parent">El id del padre</param>
        /// <param name="obj">El objeto actual</param>
        /// <param name="transform">La transformada aplicar</param>
        /// <param name="tr">La transacción activa</param>
        private void Move(long parentId, RivieraObject obj, Matrix3d transform, Transaction tr)
        {
            RivieraObject parent = App.DB[parentId];
            if (parent != null)
            {
                parent.Transform(transform, tr, false);
                Move(parent.Parent, parent, transform, tr);

                foreach (long childId in parent.Children.Values.Where(x => x != 0 && x != obj.Handle.Value))
                    App.DB[childId]?.Transform(transform, tr);
            }
        }
        /// <summary>
        /// Agrega o remplaza un hijo al elemento joint
        /// </summary>
        /// <param name="dir">La dirección a la que se asocia el hijo</param>
        /// <param name="handle">El handle del hijo</param>
        /// <param name="tr">La transacción activa</param>
        public override void AddChildren(ArrowDirection dir, Handle handle, Transaction tr)
        {
            base.AddChildren(this.GetDirection(dir), handle, tr);
        }
        /// <summary>
        /// Crea el bloque 3D de una unión
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        public virtual ObjectIdCollection Create3D(Transaction tr)
        {
            ObjectIdCollection ids = new ObjectIdCollection();
            MamparaRematesDrawer d = new MamparaRematesDrawer(this);
            List<RemateGeometry> remates = new List<RemateGeometry>();
            try
            {
                d.SelectRemate(ref remates);

                if (d.Union != JointType.Joint_I && !this.HasDoublePanels)
                    ids.Add(d.CreateColumn(tr));
                Mampara left, right, top;
                if (this.HasDoublePanels && d.AreBackPanelesExtended(this, out left, out right, out top))
                    remates.Add(d.CreateRemate(this, left, top));
                foreach (ObjectId id in d.CreateRemates(tr, remates))
                    ids.Add(id);
                if (this.HasDoublePanels)
                    ids.Add(d.AddColumnTopper(d.GetMamparas(), tr));
                return ids;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        internal void DrawSideTags(RivieraPanelDoubleLocation location, out ObjectId sideAId, out ObjectId sideBId, out ObjectId sideABId)
        {
            BlankTransactionWrapper<ObjectId[]> fT = new BlankTransactionWrapper<ObjectId[]>(
                delegate (Document doc, Transaction tr)
                {
                    double ancho = location.MamparaWidth;
                    return new ObjectId[]
                    {
                        Drawer.Text("Lado A", 0, location.LocationTagA.ToPoint3d(), ancho, new Margin(-ancho*2, 0), tr, String.Empty),
                        Drawer.Text("Lado B", 0, location.LocationTagB.ToPoint3d(), ancho, new Margin(-ancho*2, 0), tr, String.Empty),
                        Drawer.Text("Lado AB", 0, location.LocationTagAB.ToPoint3d(), ancho, new Margin(-ancho*2, 0), tr, String.Empty),
                    };
                });
            ObjectId[] res = fT.Run();
            sideAId = res[0];
            sideBId = res[1];
            sideABId = res[2];
        }

        /// <summary>
        /// Obtiene el siguiente punto a insertar el objeto
        /// </summary>
        /// <param name="dir">La dirección a insertar el otro objeto</param>
        /// <param name="ang">Como parametro de salida se tiene el angulo de dirección</param>
        /// <returns>El nuevo punto para insertar el objeto.</returns>
        public virtual Point3d NextPoint(ArrowDirection dir, out Double ang)
        {
            dir = this.GetDirection(dir);

            Double[] r = this.DirectionOffset,
                     angOffset = new double[]
                     {
                         Math.PI/2,//+90
                         0,
                         -Math.PI/2//-90
                     };
            ang = this.Angle + angOffset[(int)dir];
            return this.Start.ToPoint2dByPolar(r[(int)dir], ang).ToPoint3d();
        }
        /// <summary>
        /// Una articulación solo tiene 3 direcciones, left front y right
        /// </summary>
        /// <param name="dir">La dirección de la articulación</param>
        /// <returns>La dirección de la arcticulación</returns>
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
        /// <summary>
        /// Sobre escribe la dirección de la aplicación
        /// </summary>
        /// <param name="tr">Transacción activa</param>
        public override void ShowDirections(Transaction tr, ArrowDirection dir = ArrowDirection.None)
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
            Point2d[] pts = new Point2d[] { this.End, this.End, this.End };
            this.CreateDirections(h, d, ancho, pts, tr);

        }
        /// <summary>
        /// Se encarga de dibujar las flechas de la articulación en su posición correspondiente
        /// </summary>
        /// <param name="h">diagonal de la flecha</param>
        /// <param name="d">Porcentaje de ancho de inserción</param>
        /// <param name="width">Ancho</param>
        /// <param name="insPt">Los puntos base para insertar las flechas</param>
        /// <param name="tr">Transacción activa</param>
        internal void CreateDirections(double h, double d, double width, Point2d[] insPt, Transaction tr)
        {
            ArrowDirection[] dirs =
                new ArrowDirection[]
                {
                    ArrowDirection.Right_Front,
                    ArrowDirection.Left_Front,
                    ArrowDirection.Front
                };
            Double[] r = new Double[] { h, h, width };
            Double[] ang = new Double[dirs.Length];
            this.Arrows = new Riviera2DArrow[dirs.Length];
            for (int i = 0; i < dirs.Length; i++)
            {
                ang[i] = DirectionAngle(dirs[i]);
                this.Arrows[i] = new Riviera2DArrow(insPt[i].ToPoint2dByPolar(r[i], ang[i]), ARROW_SIZE, dirs[i]);
                this.Arrows[i].Draw(tr, ang[i]);
            }
        }

        /// <summary>
        /// Defpendiendo la dirección seleccionada
        /// se calcula el angulo de dirección de la unión
        /// </summary>
        /// <param name="dir">La dirección seleccionada</param>
        /// <returns>El angulo segun la dirección seleccionada</returns>
        internal double DirectionAngle(ArrowDirection dir)
        {
            Double ang;
            if (dir == ArrowDirection.Right_Front || dir == ArrowDirection.Left_Back)
                ang = this.Direction.Angle - Math.PI / 2;
            else if (dir == ArrowDirection.Left_Front || dir == ArrowDirection.Right_Back)
                ang = this.Direction.Angle + Math.PI / 2;
            else
                ang = this.Direction.Angle;
            return ang;
        }


    }
}
