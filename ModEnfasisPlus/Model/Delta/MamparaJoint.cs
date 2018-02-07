using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.OldModulador.Controller;
using DaSoft.Riviera.OldModulador.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Mio;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Entities;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Model;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu;
using System;
using System.IO;
using System.Linq;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CODE;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CONST;
using static DaSoft.Riviera.OldModulador.Assets.Strings;
namespace DaSoft.Riviera.OldModulador.Model.Delta
{
    public class MamparaJoint : JointObject
    {
        /// <summary>
        /// La dirección del offset de la mampara
        /// </summary>
        public override Double[] DirectionOffset
        {
            get
            {
                //ArrowDirection.Left_Front = 0,
                //ArrowDirection.Front = 1,
                //ArrowDirection.Right_Front = 2,
                double ancho = (ANCHO_M / 2d);
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
        /// The 3D Block file
        /// </summary>
        public FileInfo BlockFile3d
        {
            get
            {
                return this.Code.BlockFile3D();
            }
        }
        /// <summary>
        /// Devuelve el tipo de unión al que pertenece la articulación
        /// </summary>
        public override JointType Type
        {
            get
            {
                int count = this.Children.Values.Where(x => x == 0).Count() - 3;
                if (count == 0 && this.Parent != 0)
                    return JointType.Joint_X;
                else if (count == 1 && this.Parent != 0)
                    return JointType.Joint_T;
                else if (count == 2 && this.Parent != 0)
                    return JointType.Joint_L;
                else if (count == 0 && this.Parent == 0)
                    return JointType.Joint_T;
                else if (count == 1 && this.Parent == 0)
                    return JointType.Joint_L;
                else
                    return JointType.None;
            }
        }
        /// <summary>
        /// El texto de la union
        /// </summary>
        public DBText Text;
        Point2d Center;
        /// <summary>
        /// Crea una unión
        /// </summary>
        /// <param name="pt">El punto de la únion</param>
        /// <param name="angDirection">La dirección que tiene la articulación</param>
        public MamparaJoint(Point2d pt, double angDirection) :
            base(pt, angDirection)
        {
            this.Code = DT_JOINT;
            Center = pt.ToPoint2dByPolar(ANCHO_M / 2, angDirection);
            this.Transformed += MamparaJoint_Transformed;
            this.CreateContent();
        }
        /// <summary>
        /// Devuelve las uniones para una unión en T
        /// </summary>
        public void GetMamparaInT(out Mampara vertical, out Mampara[] horizontals)
        {
            if (this.Type == JointType.Joint_T)
            {
                Mampara[] mamparas = new Mampara[3];
                int startIndex = 0;
                if (this.Parent != 0)
                {
                    mamparas[0] = App.DB[this.Parent] as Mampara;
                    startIndex = 1;
                }
                var childIds = this.Children.Values.Where(x => x != 0).ToArray();
                for (int i = startIndex; i < mamparas.Length; i++)
                    mamparas[i] = App.DB[childIds[i - 1]] as Mampara;
                double[] ang = new double[]
                {
                    mamparas[0].Direction.GetAngleTo(mamparas[1].Direction),
                    mamparas[0].Direction.GetAngleTo(mamparas[2].Direction),
                };
                if (ang[0] == 0 || ang[0] == Math.PI)
                {
                    vertical = mamparas[2];
                    horizontals = new Mampara[] { mamparas[0], mamparas[1] };
                }
                else if (ang[1] == 0 || ang[1] == Math.PI)
                {
                    vertical = mamparas[1];
                    horizontals = new Mampara[] { mamparas[0], mamparas[2] };
                }
                else
                {
                    vertical = mamparas[0];
                    horizontals = new Mampara[] { mamparas[1], mamparas[2] };
                }
            }
            else
                throw new Exception("La mampara no es una unión en T");
        }

        /// <summary>
        /// Se ejecuta una vez que el nodo de mampara ha sido actualizado
        /// </summary>
        private void MamparaJoint_Transformed(object sender, System.Windows.RoutedEventArgs e)
        {
            TransformArgs args = e as TransformArgs;
            this.Center = this.Center.ToPoint3d().TransformBy(args.Matrix).ToPoint2d();
        }

        /// <summary>
        /// Crea el contenido de la articulación de la mampara
        /// </summary>
        public override void CreateContent()
        {
            Point2d front = this.Start.ToPoint2dByPolar(0.0001d, 0).ToPoint2dByPolar(ANCHO_M, this.Angle);

            Point2d[] pts = new Point2d[]
            {
                this.Start.ToPoint2dByPolar(ANCHO_M / 2, this.Angle - Math.PI / 2),
                front.ToPoint2dByPolar(ANCHO_M / 2, this.Angle - Math.PI / 2),
                front.ToPoint2dByPolar(ANCHO_M / 2, this.Angle + Math.PI / 2),
                this.Start.ToPoint2dByPolar(ANCHO_M / 2, this.Angle + Math.PI / 2)
            };
            Polyline pl = new Polyline();
            if (this.Text != null && this.Text.Id.IsValid)
                Drawer.Erase(new ObjectIdCollection(new ObjectId[] { this.Text.Id }));
            DBText txt = new DBText();
            this.Center = this.Start.ToPoint2dByPolar(ANCHO_M / 2, this.Angle);
            txt.TextString = String.Format("{0}", this.Type.GetLetter());

            txt.Rotation = 0d;
            txt.Height = ANCHO_M / 4;
            txt.Position = (this.Center - new Vector2d(txt.Height / 2, txt.Height / 2)).ToPoint3d();
            this.Text = txt;
            for (int i = 0; i < pts.Length; i++)
                pl.AddVertexAt(i, pts[i], 0, 0, 0);
            pl.Closed = true;
            Content = pl;
        }
        /// <summary>
        /// Dibuja el contenido de la articulación de la mampara
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        public override void DrawContent(Transaction tr)
        {
            Drawing drawing;
            AutoCADLayer lay = new AutoCADLayer(LAYER_RIVIERA_GEOMETRY, tr);
            lay.SetStatus(LayerStatus.EnableStatus, tr);
            if (this.Text != null)
            {
                drawing = new Drawing(this.Content as Polyline, this.Text) { Layername = LAYER_RIVIERA_OBJECT };
                drawing.Draw(tr);
                foreach (ObjectId id in drawing.Ids)
                    this.Ids.Add(id);
            }
            else
            {
                drawing = new Drawing(this.Content as Polyline) { Layername = LAYER_RIVIERA_OBJECT };
                drawing.Draw(tr);
                this.Ids.Add(drawing.FirstId);
            }
            //Se agregán a una capa especial
            lay.AddToLayer(new ObjectIdCollection(this.Ids.OfType<ObjectId>().Where(x => this.Ids.IndexOf(x) > 0).ToArray()), tr);
        }
        /// <summary>
        /// Actualiza el valor de la etiqueta
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        public void UpdateTag(Transaction tr)
        {
            DBObject obj = null;
            for (int i = 0; !(obj is DBText) && i < this.Ids.Count; i++)
            {
                try
                {
                    obj = this.Ids[i].GetObject(OpenMode.ForRead);
                }
                catch (Exception)
                {
                    continue;
                }
            }
            if (obj is DBText)
            {
                this.Text = obj as DBText;
                this.Text.UpgradeOpen();
                this.Text.TextString = this.Type.GetLetter();
            }
        }
        /// <summary>
        /// Obtiene el siguiente punto a insertar el objeto
        /// </summary>
        /// <param name="dir">La dirección a insertar el otro objeto</param>
        /// <param name="ang">Como parametro de salida se tiene el angulo de dirección</param>
        /// <returns>El nuevo punto para insertar el objeto.</returns>
        public override Point3d NextPoint(ArrowDirection dir, out double ang)
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
            Point2d center = this.Center;
            if (dir == ArrowDirection.Front)
                center = this.Start.ToPoint2dByPolar(0.0001d + ANCHO_M / 2, ang);
            if (App.Riviera.Units == DaNTeUnits.Imperial)
            {
                if (dir == ArrowDirection.Front)
                    center = this.Start.ToPoint2dByPolar(0.0001d + (ANCHO_M / 2).ConvertUnits(Unit_Type.m, Unit_Type.inches), this.Angle);
                else
                    center = this.Start.ToPoint2dByPolar((ANCHO_M / 2).ConvertUnits(Unit_Type.m, Unit_Type.inches), this.Angle);
            }
            return center.ToPoint2dByPolar(r[(int)dir], ang).ToPoint3d();
        }

        /// <summary>
        /// Sobre escribe la dirección de la aplicación
        /// </summary>
        /// <param name="tr">Transacción activa</param>
        public override void ShowDirections(Transaction tr, ArrowDirection dir = ArrowDirection.None)
        {
            Double ancho = this.Ancho,
                   d = ANCHO_M / 2,
                   h = Math.Sqrt(d * d * 2) * 1.2d;
            Point2d cen = this.Center;
            if (App.Riviera.Units == DaNTeUnits.Imperial)
            {
                ancho = this.Ancho.ConvertUnits(Unit_Type.m, Unit_Type.inches);
                d = d.ConvertUnits(Unit_Type.m, Unit_Type.inches);
                h = h.ConvertUnits(Unit_Type.m, Unit_Type.inches);
                cen = this.Start.ToPoint2dByPolar(d, this.Angle);
            }
            Point2d[] pts;
            if (App.Riviera.Units == DaNTeUnits.Metric)
                pts = new Point2d[] { cen, cen, cen.ToPoint2dByPolar(d * 1.5d, this.Angle) };
            else
                pts = new Point2d[] { cen, cen, cen.ToPoint2dByPolar(d * 1.6d, this.Angle) };
            this.CreateDirections(h, d, ancho, pts, tr);
        }
        /// <summary>
        /// Convierte el elemento a una articulación simple
        /// A fuerza tiene un nodo front y debe tener un padre
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        public void Downgrade(Transaction tr)
        {
            Mampara[] mam = new Mampara[2];

            if (this.PanelArray != null)
            {
                this.PanelArray.Clean(this, tr);
                this.PanelArray = null;
            }

            if (this.Parent != 0 && this.Children[FIELD_FRONT] != 0)
            {
                mam[0] = App.DB[this.Parent] as Mampara;
                mam[1] = App.DB[this.Children[FIELD_FRONT]] as Mampara;
                JointObject joint = new JointObject(this.Start, this.Angle);
                joint.Draw(tr, null);
                joint.Parent = mam[0].Handle.Value;
                joint.Children[FIELD_FRONT] = mam[1].Handle.Value;
                joint.Save(tr);

                //Actualizamos las mamparas
                mam[0].Children[FIELD_FRONT] = joint.Handle.Value;
                mam[1].Parent = joint.Handle.Value;
                Matrix3d moveTransform = Matrix3d.Displacement(mam[1].Start.ToPoint3d().GetVectorTo(mam[0].End.ToPoint3d()));
                mam[1].Transform(moveTransform, tr);
                App.DB.Objects.Add(joint);
                this.Delete(tr);
            }
        }
        public override void Save(Transaction tr)
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
                    this.Extra
            });
        }
    }
}
