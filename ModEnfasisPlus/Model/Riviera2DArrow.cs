using Autodesk.AutoCAD.Geometry;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu.Shapes2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using NamelessOld.Libraries.HoukagoTeaTime.Mio;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu;
using NamelessOld.Libraries.HoukagoTeaTime.Yui;
using DaSoft.Riviera.OldModulador.Assets;
using DaSoft.Riviera.OldModulador.Controller;
using DaSoft.Riviera.OldModulador.Runtime;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CONST;
namespace DaSoft.Riviera.OldModulador.Model
{
    public class Riviera2DArrow : Geometry2D
    {
        
        /// <summary>
        /// El perimetro de la flecha
        /// </summary>
        public override double Perimeter
        {
            get
            {
                double p = 0d;
                for (int i = 0; i < this.Vertices.Count; i++)
                    if (i == this.Vertices.Count - 1)
                        p += this.Vertices[i].GetDistanceTo(this.Vertices[0]);
                    else
                        p += this.Vertices[i].GetDistanceTo(this.Vertices[i + 1]);
                return p;
            }
        }
        /// <summary>
        /// Devuelve el centro de la flecha
        /// </summary>
        public Point2d Center;
        /// <summary>
        /// El punto de inserción de la etiqueta
        /// </summary>
        public Point2d InsertionPoint;
        /// <summary>
        /// La dirección de la flecha
        /// </summary>
        public ArrowDirection Direction;
        /// <summary>
        /// El tamaño de la flecha
        /// </summary>
        Double Size;
        /// <summary>
        /// El id de la flecha
        /// </summary>
        public new ObjectIdCollection Id;

        /// <summary>
        /// Crea una nueva flecha
        /// </summary>
        /// <param name="insPt">El punto de inserción de la flecha</param>
        /// <param name="size">El tamaño de la flecha</param>
        public Riviera2DArrow(Point2d insPt, double size, ArrowDirection direction)
            : base(insPt + new Vector2d(0, -size / 2),     //P0
                    insPt + new Vector2d(size, -size / 2),  //P1
                    insPt + new Vector2d(size, -size),      //P2
                    insPt + new Vector2d(2 * size, 0),      //P3
                    insPt + new Vector2d(size, size),       //P4
                    insPt + new Vector2d(size, size / 2),   //P5
                    insPt + new Vector2d(0, size / 2))      //P6
        {
            this.InsertionPoint = insPt;
            this.Direction = direction;
            this.Size = size;
            this.Id = new ObjectIdCollection();
        }
        /// <summary>
        /// Borra la flecha
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        public void Erase(Transaction tr)
        {
            Drawer.Erase(this.Id, tr);
        }
        /// <summary>
        /// Checa si la flecha colisiona
        /// </summary>
        /// <returns>El área de colisión de la flecha</returns>
        public bool Collides(Transaction tr)
        {
            double searchArea = ARROW_SEARCH_AREA;
            if (App.Riviera.Units == DaNTeUnits.Imperial)
                searchArea = (searchArea/2).ConvertUnits(Unit_Type.m, Unit_Type.inches);
            Geometry2D pol = new Polygon2D(8, this.Center, searchArea);
            PolygonSelector sel = new PolygonSelector(pol.Vertices.ToPoint3d());
            Boolean flag = false;
            if (sel.Search(SelectType.Crossing, null))
                flag = sel.Filter(this.Id).Count() > 0;
            return flag;
        }
        /// <summary>
        /// Dibuja el rectangulo
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        /// <param name="input">La entrada de la transacción</param>
        public void Draw(Transaction tr, Object input)
        {
            Matrix3d m = Matrix3d.Rotation((double)input, Vector3d.ZAxis, this.InsertionPoint.ToPoint3d());
            this.Id.Add(Drawer.Geometry2D(this, tr, closed: true));
            Polyline pl = this.Id[0].OpenEntity(tr) as Polyline;
            this.RefreshVertices(pl.ExtractVertices());
            this.Id[0].TransformBy(m);
            this.Center = pl.GeometricExtents.MinPoint.MiddlePointTo(pl.GeometricExtents.MaxPoint).ToPoint2d();
            if (App.DEBUG_MODE)
            {
                DBText text = new DBText();
                text.Height = this.Size / 2;
                text.Position = pl.GeometricExtents.MinPoint.MiddlePointTo(pl.GeometricExtents.MaxPoint);
                text.TextString = this.Direction.GetStringDirection();
                this.Id.Add(Drawer.Entity(text, tr));
                Geometry2D pol = new Polygon2D(5, this.Center, ARROW_SEARCH_AREA);
                this.Id.Add(Drawer.Geometry2D(pol));
            }
            if (App.Riviera.Units == DaNTeUnits.Imperial)
                this.Id.Transform(Matrix3d.Scaling(IMPERIAL_FACTOR, this.InsertionPoint.ToPoint3d()), tr);
        }

        public override string ToString()
        {
            return String.Format("Id{0} Dir: {1}", this.Id.Count > 0 ? this.Id[0].ToString() : "(0)", this.Direction.ToHumanReadable());
        }
    }
}
