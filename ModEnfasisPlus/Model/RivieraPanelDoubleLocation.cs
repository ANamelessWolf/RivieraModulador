using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.OldModulador.Controller;
using DaSoft.Riviera.OldModulador.Model.Delta;
using DaSoft.Riviera.OldModulador.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Mio;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Model;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu;
using System;
using System.Collections.Generic;
using System.Linq;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CODE;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CONST;

namespace DaSoft.Riviera.OldModulador.Model
{
    public class RivieraPanelDoubleLocation
    {
        /// <summary>
        /// La mampara superior
        /// </summary>
        public Mampara Top;
        /// <summary>
        /// La mampara izquierda
        /// </summary>
        public Mampara Left;
        /// <summary>
        /// La mampara derecha
        /// </summary>
        public Mampara Right;
        JointObject Joint;
        /// <summary>
        /// El punto inicial de la mampara izquierda.
        /// Left
        /// </summary>
        public Point2d LeftLocation;
        /// <summary>
        /// El punto final de la mampara derecha.
        /// Right
        /// </summary>
        public Point2d RightLocation;
        /// <summary>
        /// El centro del arreglo de las mamparas
        /// Center
        /// </summary>
        public Point2d CenterLocation;

        /// <summary>
        /// El punto superior izquierdo del arreglo de mamparas.
        /// LeftTop
        /// </summary>
        public Point2d LeftTopLocationStart;
        /// <summary>
        /// El punto inferior izquierdo del arreglo de mamparas.
        /// LeftTop
        /// </summary>
        public Point2d LeftBottomLocationStart;
        /// <summary>
        /// El punto superior derecho del arreglo de mamparas.
        /// LeftTop
        /// </summary>
        public Point2d RightTopLocationStart;
        /// <summary>
        /// El punto inferior derecho del arreglo de mamparas.
        /// LeftTop
        /// </summary>
        public Point2d RightBottomLocationStart;

        /// <summary>
        /// El punto superior izquierdo del arreglo de mamparas.
        /// LeftTop
        /// </summary>
        public Point2d LeftTopLocationEnd;
        /// <summary>
        /// El punto inferior izquierdo del arreglo de mamparas.
        /// LeftTop
        /// </summary>
        public Point2d LeftBottomLocationEnd;
        /// <summary>
        /// El punto superior derecho del arreglo de mamparas.
        /// LeftTop
        /// </summary>
        public Point2d RightTopLocationEnd;
        /// <summary>
        /// El punto inferior derecho del arreglo de mamparas.
        /// LeftTop
        /// </summary>
        public Point2d RightBottomLocationEnd;
        /// <summary>
        /// El punto intermedio izquierdo
        /// </summary>
        public Point2d LeftLocationMiddle;
        /// <summary>
        /// El punto intermedio derecho
        /// </summary>
        public Point2d RightLocationMiddle;

        /// <summary>
        /// El punto de inserción de la etiqueta A.
        /// LeftTop
        /// </summary>
        public Point2d LocationTagA;
        /// <summary>
        /// El punto de inserción de la etiqueta B.
        /// LeftTop
        /// </summary>
        public Point2d LocationTagB;
        /// <summary>
        /// El punto de inserción de la etiqueta AB.
        /// LeftTop
        /// </summary>
        public Point2d LocationTagAB;
        /// <summary>
        /// El punto inferior derecho del arreglo de mamparas.
        /// LeftTop
        /// </summary>
        public Point2d TopStart;
        /// <summary>
        /// El punto inferior derecho del arreglo de mamparas.
        /// LeftTop
        /// </summary>
        public Point2d TopEnd;
        /// <summary>
        /// El frente de la mampara horizontal
        /// </summary>
        public Double Frente;
        /// <summary>
        /// La dirección del arreglo de mampara horizontal
        /// </summary>
        public Double Direction;
        /// <summary>
        /// Devuelve el ancho de la mampara
        /// </summary>
        /// <value>
        /// El ancho de la mampara
        /// </value>
        public double MamparaWidth
        {
            get
            {
                Double r = STACK_ANCHO_MM.ConvertUnits(Unit_Type.mm, Unit_Type.m);
                if (App.Riviera.Units == DaNTeUnits.Imperial)
                    r = r.ConvertUnits(Unit_Type.m, Unit_Type.inches);
                return r;
            }
        }

  
        /// <summary>
        /// Inicializa una instancia de la clase <see cref="RivieraPanelDoubleLocation"/>.
        /// </summary>
        /// <param name="frente">El frente de la mampara en metros</param>
        /// <param name="top">La mampara perpendicular.</param>
        /// <param name="left">La mampara izquierda.</param>
        /// <param name="right">La mampara derecha.</param>
        /// <param name="joint">La unión de mamparas.</param>
        public RivieraPanelDoubleLocation(Double frente, Mampara top, Mampara left, Mampara right, JointObject joint)
        {
            this.Frente = frente;
            this.Top = top;
            this.Left = left;
            this.Right = right;
            this.Joint = joint;
            this.FindPoints();
        }
        /// <summary>
        /// Realizá el dibujo de las etiquetas de los puntos que definen el arreglo de
        /// paneles
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        public ObjectIdCollection DrawTags(Transaction tr)
        {
            ObjectIdCollection ids = new ObjectIdCollection(new ObjectId[]
            {
                Drawer.Text("LT1", 0, this.LeftTopLocationStart.ToPoint3d(), this.MamparaWidth / 2, new Margin(), tr, BlockTableRecord.ModelSpace),
                Drawer.Text("LT2", 0, this.LeftTopLocationEnd.ToPoint3d(), this.MamparaWidth / 2, new Margin(), tr, BlockTableRecord.ModelSpace),
                Drawer.Text("LB1", 0, this.LeftBottomLocationStart.ToPoint3d(), this.MamparaWidth / 2, new Margin(), tr, BlockTableRecord.ModelSpace),
                Drawer.Text("LB2", 0, this.LeftBottomLocationEnd.ToPoint3d(), this.MamparaWidth / 2, new Margin(), tr, BlockTableRecord.ModelSpace),
                Drawer.Text("RT1", 0, this.RightTopLocationStart.ToPoint3d(), this.MamparaWidth / 2, new Margin(), tr, BlockTableRecord.ModelSpace),
                Drawer.Text("RT2", 0, this.RightTopLocationEnd.ToPoint3d(), this.MamparaWidth / 2, new Margin(), tr, BlockTableRecord.ModelSpace),
                Drawer.Text("RB1", 0, this.RightBottomLocationStart.ToPoint3d(), this.MamparaWidth / 2, new Margin(), tr, BlockTableRecord.ModelSpace),
                Drawer.Text("RB2", 0, this.RightBottomLocationEnd.ToPoint3d(), this.MamparaWidth / 2, new Margin(), tr, BlockTableRecord.ModelSpace),
                Drawer.Text("T1", 0, this.TopStart.ToPoint3d(), this.MamparaWidth / 2, new Margin(), tr, BlockTableRecord.ModelSpace),
                Drawer.Text("T2", 0, this.TopEnd.ToPoint3d(), this.MamparaWidth / 2, new Margin(), tr, BlockTableRecord.ModelSpace)
            });
            return ids;
        }

        /// <summary>
        /// Realiza la carga de los puntos de la mampara
        /// </summary>
        private void FindPoints()
        {
            Point2d jPt = this.Joint.Start, dirStart, dirEnd, tmp1, tmp2;
            Double ang = Math.PI / 2;
            this.LeftLocation = jPt.GetDistanceTo(Left.Start) > jPt.GetDistanceTo(Left.End) ? Left.Start : Left.End;
            this.RightLocation = jPt.GetDistanceTo(Right.Start) < jPt.GetDistanceTo(Right.End) ? Right.Start : Right.End;
            dirStart = this.LeftLocation;
            dirEnd = jPt.GetDistanceTo(Right.Start) > jPt.GetDistanceTo(Right.End) ? Right.Start : Right.End;
            this.Direction = dirStart.GetVectorTo(dirEnd).Angle;
            this.CenterLocation = this.LeftLocation.MiddlePointTo(this.RightLocation);
            //Los puntos superiores
            if (jPt.GetDistanceTo(Top.Start) < jPt.GetDistanceTo(Top.End))
            {
                this.TopStart = Top.Start;
                this.TopEnd = Top.End;
            }
            else
            {
                this.TopEnd = Top.Start;
                this.TopStart = Top.End;
            }
            //Los puntos iniciales de los paneles
            this.LeftTopLocationStart = this.GetPoint(this.LeftLocation, this.Direction + ang);
            this.LeftBottomLocationStart = this.GetPoint(this.LeftLocation, this.Direction + -ang);
            this.RightTopLocationStart = this.GetPoint(this.RightLocation, this.Direction + ang);
            this.RightBottomLocationStart = this.GetPoint(this.RightLocation, this.Direction + -ang);
            //El punto final para la inserción de los paneles
            this.LeftTopLocationEnd = this.GetEndPoint(this.LeftTopLocationStart);
            this.LeftBottomLocationEnd = this.GetEndPoint(this.LeftBottomLocationStart);
            this.RightTopLocationEnd = this.GetEndPoint(this.RightTopLocationStart);
            this.RightBottomLocationEnd = this.GetEndPoint(this.RightBottomLocationStart);
            //Puntos intermedios para la inserción de los paneles.
            this.LeftLocationMiddle = this.LeftTopLocationStart.MiddlePointTo(this.LeftBottomLocationStart);
            this.RightLocationMiddle = this.RightTopLocationStart.MiddlePointTo(this.RightBottomLocationStart);
            //Puntos de inserción de las etiquetas
            tmp1 = this.GetPoint(this.LeftTopLocationStart, this.Direction + ang);
            tmp2 = this.GetPoint(this.LeftTopLocationEnd, this.Direction + ang);
            this.LocationTagA = tmp1.MiddlePointTo(tmp2);
            tmp1 = this.GetPoint(this.RightTopLocationStart, this.Direction + ang);
            tmp2 = this.GetPoint(this.RightTopLocationEnd, this.Direction + ang);
            this.LocationTagB = tmp1.MiddlePointTo(tmp2);
            tmp1 = this.GetPoint(this.LeftBottomLocationStart, this.Direction - ang);
            tmp2 = this.GetPoint(this.RightBottomLocationEnd, this.Direction - ang);
            this.LocationTagAB = tmp1.MiddlePointTo(tmp2);
        }
        /// <summary>
        /// Selecciona el valor del frente de una mampara en metros
        /// </summary>
        /// <param name="mampara">La mampara a extraer el frente.</param>
        /// <returns>El frente de la ventana</returns>
        public static double PickFrente(Mampara mampara)
        {
            String frente = mampara.Code.Substring(6, 2);
            var fSize = App.DB.Panel_Size.Where(x => x.Frente == frente).FirstOrDefault();
            double f = 0;
            if (fSize != null)
                f = fSize.Real.Frente.ConvertUnits(Unit_Type.mm, Unit_Type.m);
            if (App.Riviera.Units == DaNTeUnits.Imperial)
                f = f.ConvertUnits(Unit_Type.m, Unit_Type.inches);
            return f;
        }

        /// <summary>
        /// Obtiene un punto apartir de una rotación
        /// </summary>
        /// <param name="point">EL punto base.</param>
        /// <param name="rot">El ángulo de rotación</param>
        /// <returns>El punto rotado</returns>
        private Point2d GetPoint(Point2d point, double rot)
        {
            return point.ToPoint2dByPolar(this.MamparaWidth, rot);
        }
        /// <summary>
        /// Obtiene el punto de la línea de información del panel
        /// </summary>
        /// <param name="point">EL punto base.</param>
        /// <returns>El punto rotado</returns>
        private Point2d GetEndPoint(Point2d point)
        {
            Double offset = 0.0100d, frente = this.Frente;
            if (App.Riviera.Units == DaNTeUnits.Imperial)
                offset = offset.ConvertUnits(Unit_Type.m, Unit_Type.inches);
            return point.ToPoint2dByPolar(frente + offset, this.Direction);
        }

        /// <summary>
        /// Calcula el producto cruz de dos mamparas
        /// </summary>
        /// <param name="m1">La primer mampara.</param>
        /// <param name="m2">La segunda mampara.</param>
        /// <returns>El valor de la componente k</returns>
        public static Double CrossProduct(Mampara m1, Mampara m2)
        {
            Vector2d v1 = m1.Start.GetVectorTo(m2.End),
                     v2 = m2.Start.GetVectorTo(m2.End);
            return v1.X * v2.Y - v2.X * v1.Y;
        }
        /// <summary>
        /// Encuentra las mamparas horizontales de una unión en T
        /// </summary>
        /// <param name="joint">La unión de la mampara.</param>
        /// <param name="left">Como salida la mampara úbicada en la posición izquierda.</param>
        /// <param name="right">Como salida la mampara úbicada en la posición derecha.</param>
        /// <param name="top">Como salida la mampara úbicada en la posición perpendicular a las otras dos mamparas.</param>
        public static void FindMampara(JointObject joint, out Mampara left, out Mampara right, out Mampara top)
        {
            //Se realiza la selección de las mamparas
            Mampara[] mamparas = new Mampara[3];
            Mampara tmpA = null, tmpB = null;
            Double r = Math.PI / 180,
                   r0L = r * 358,
                   r0R = r * 2,
                   r180L = r * 178,
                   r180R = r * 182;
            Vector2d dirA = new Vector2d(),
                     dirB = new Vector2d();
            int index = 0;
            top = null;
            if (joint.Parent != 0)
                mamparas[index++] = App.DB[joint.Parent] as Mampara;
            foreach (var objKey in joint.Children.Values.Where(x => x != 0))
                mamparas[index++] = App.DB[objKey] as Mampara;
            //Se calcula la dirección de las mamparas,
            //usando como base el punto inicial de la uníón
            Vector2d[] dir = new Vector2d[3];
            Point2d center = joint.Start;
            //Se calcula la dirección de las mamparas respecto al centro de la unión
            for (int i = 0; i < dir.Length; i++)
            {
                if (center.GetDistanceTo(mamparas[i].Start) < center.GetDistanceTo(mamparas[i].End))
                    dir[i] = mamparas[i].Start.GetVectorTo(mamparas[i].End);
                else
                    dir[i] = mamparas[i].End.GetVectorTo(mamparas[i].Start);
            }
            //El angulo entre vectores
            Double[] ang = new Double[]
            {
                Math.Abs(dir[0].GetAngleTo(dir[1])),
                Math.Abs(dir[0].GetAngleTo(dir[2])),
                Math.Abs(dir[1].GetAngleTo(dir[2])),
            };
            //El indice para la mampara top asociado al
            //caso de cuando los angulos sean de 180°
            int[] topIndex = new int[] { 2, 1, 0 };
            Tuple<int, int>[] tmpIndex = new Tuple<int, int>[]
            {
                new Tuple<int, int> (0,1),
                new Tuple<int, int> (0,2),
                new Tuple<int, int> (1,2),
            };
            for (int i = 0; i < ang.Length && top == null; i++)
                if (ang[i] > (0.75d * Math.PI) || ang[i] == 0)
                {
                    top = mamparas[topIndex[i]];
                    tmpA = mamparas[tmpIndex[i].Item1];
                    tmpB = mamparas[tmpIndex[i].Item2];
                    dirA = dir[tmpIndex[i].Item1];
                    dirB = dir[tmpIndex[i].Item2];
                }
            //delta = top.Angle - (Math.PI / 2);
            //angA = dirA.Angle - delta;
            //angB = dirB.Angle - delta;

            //while (angA < 0)
            //    angA += Math.PI * 2;
            //while (angB < 0)
            //    angB += Math.PI * 2;

            //if (angA > r0L || angA < r0R)
            //    angA = 0;
            //if (angB > r0L || angB < r0R)
            //    angB = 0;
            //if (angA > r180L && angA < r180R)
            //    angA = Math.PI;
            //if (angB > r180L && angB < r180R)
            //    angB = Math.PI;

            Vector2d topV = center.GetDistanceTo(top.Start) < center.GetDistanceTo(top.End) ? new Vector2d(top.End.X - top.Start.X, top.End.Y - top.Start.Y) : new Vector2d(top.Start.X - top.End.X, top.Start.Y - top.End.Y),
                    tmpAV = center.GetDistanceTo(tmpA.Start) < center.GetDistanceTo(tmpA.End) ? new Vector2d(tmpA.End.X - tmpA.Start.X, tmpA.End.Y - tmpA.Start.Y) : new Vector2d(tmpA.Start.X - tmpA.End.X, tmpA.Start.Y - tmpA.End.Y),
                     tmpBV = center.GetDistanceTo(tmpB.Start) < center.GetDistanceTo(tmpB.End) ? new Vector2d(tmpB.End.X - tmpB.Start.X, tmpB.End.Y - tmpB.Start.Y) : new Vector2d(tmpB.Start.X - tmpB.End.X, tmpB.Start.Y - tmpB.End.Y);
            Double tmpADir = topV.X * tmpAV.Y - tmpAV.X * topV.Y,
                tmpBDir = topV.X * tmpBV.Y - tmpBV.X * topV.Y;

            if (tmpADir < tmpBDir)
            {
                left = tmpB;
                right = tmpA;
            }
            else
            {
                left = tmpA;
                right = tmpB;
            }
        }
        /// <summary>
        /// Encuentra las mamparas horizontales de una unión en T
        /// </summary>
        /// <param name="joint">La unión de la mampara.</param>
        /// <param name="left">Como salida la mampara úbicada en la posición izquierda.</param>
        /// <param name="right">Como salida la mampara úbicada en la posición derecha.</param>
        /// <param name="top">Como salida la mampara úbicada en la posición perpendicular a las otras dos mamparas.</param>
        public static void FindMampara(List<RivieraObject> db, JointObject joint, out Mampara left, out Mampara right, out Mampara top)
        {
            //Se realiza la selección de las mamparas
            Mampara[] mamparas = new Mampara[3];
            Mampara tmpA = null, tmpB = null;
            Double angA, angB, delta,
                   r = Math.PI / 180,
                   r0L = r * 358,
                   r0R = r * 2,
                   r180L = r * 178,
                   r180R = r * 182;
            Vector2d dirA = new Vector2d(),
                     dirB = new Vector2d();
            int index = 0;
            top = null;
            if (joint.Parent != 0)
                mamparas[index++] = Runtime.Database.GetRivieraObject(db, joint.Parent) as Mampara;
            foreach (var objKey in joint.Children.Values.Where(x => x != 0))
                mamparas[index++] = Runtime.Database.GetRivieraObject(db, objKey) as Mampara;
            //Se calcula la dirección de las mamparas,
            //usando como base el punto inicial de la uníón
            Vector2d[] dir = new Vector2d[3];
            Point2d center = joint.Start;
            //Se calcula la dirección de las mamparas respecto al centro de la unión
            for (int i = 0; i < dir.Length; i++)
            {
                if (center.GetDistanceTo(mamparas[i].Start) < center.GetDistanceTo(mamparas[i].End))
                    dir[i] = mamparas[i].Start.GetVectorTo(mamparas[i].End);
                else
                    dir[i] = mamparas[i].End.GetVectorTo(mamparas[i].Start);
            }
            //El angulo entre vectores
            Double[] ang = new Double[]
            {
                Math.Abs(dir[0].GetAngleTo(dir[1])),
                Math.Abs(dir[0].GetAngleTo(dir[2])),
                Math.Abs(dir[1].GetAngleTo(dir[2])),
            };
            //El indice para la mampara top asociado al
            //caso de cuando los angulos sean de 180°
            int[] topIndex = new int[] { 2, 1, 0 };
            Tuple<int, int>[] tmpIndex = new Tuple<int, int>[]
            {
                new Tuple<int, int> (0,1),
                new Tuple<int, int> (0,2),
                new Tuple<int, int> (1,2),
            };
            for (int i = 0; i < ang.Length && top == null; i++)
                if ((ang[i] > (0.75d * Math.PI)) || ang[i] == 0)
                {
                    top = mamparas[topIndex[i]];
                    tmpA = mamparas[tmpIndex[i].Item1];
                    tmpB = mamparas[tmpIndex[i].Item2];
                    dirA = dir[tmpIndex[i].Item1];
                    dirB = dir[tmpIndex[i].Item2];
                }
            delta = top.Angle - (Math.PI / 2);
            angA = dirA.Angle - delta;
            angB = dirB.Angle - delta;

            while (angA < 0)
                angA += Math.PI * 2;
            while (angB < 0)
                angB += Math.PI * 2;

            if (angA > r0L || angA < r0R)
                angA = 0;
            if (angB > r0L || angB < r0R)
                angB = 0;
            if (angA > r180L && angA < r180R)
                angA = Math.PI;
            if (angB > r180L && angB < r180R)
                angB = Math.PI;

            if (angA == 0)
            {
                left = tmpB;
                right = tmpA;
            }
            else
            {
                left = tmpA;
                right = tmpB;
            }
        }
        /// <summary>
        /// Devuelve un punto para la ubicación del panel.
        /// </summary>
        /// <param name="panelDirection">La dirección del panel.</param>
        /// <param name="getEndPoint">Si es <c>true</c> se devuelve el punto final en otro caso el punto incial.</param>
        /// <returns>El punto del panel.</returns>
        public Point2d GetPointForRivieraPanel(ArrowDirection panelDirection, Boolean getEndPoint = false)
        {
            if (panelDirection == ArrowDirection.Front && !getEndPoint)
                return this.LeftBottomLocationStart;
            else if (panelDirection == ArrowDirection.Front && getEndPoint)
                return this.RightBottomLocationEnd;
            else if (panelDirection == ArrowDirection.Back && !getEndPoint)
                return this.LeftTopLocationStart;
            else if (panelDirection == ArrowDirection.Back && getEndPoint)
                return this.RightTopLocationEnd;
            else if (panelDirection == ArrowDirection.Left_Back && !getEndPoint)
                return this.LeftTopLocationStart;
            else if (panelDirection == ArrowDirection.Left_Back && getEndPoint)
                return this.LeftTopLocationEnd;
            else if (panelDirection == ArrowDirection.Right_Back && !getEndPoint)
                return this.RightTopLocationStart;
            else if (panelDirection == ArrowDirection.Right_Back && getEndPoint)
                return this.RightTopLocationEnd;
            else if (panelDirection == ArrowDirection.Same && !getEndPoint)
                return this.LeftLocationMiddle;
            else if (panelDirection == ArrowDirection.Same && getEndPoint)
                return this.RightLocationMiddle;
            else
                return this.CenterLocation;
        }
        /// <summary>
        /// Obtiene el tamaño seleccionado para el stack de paneles
        /// </summary>
        /// <param name="size">El tamaño de paneles</param>
        /// <param name="panelRaw">La colección de paneles crudos</param>
        /// <returns>El tamaño seleccionado</returns>
        public RivieraSize GetSize(string size, PanelRaw panelRaw)
        {
            String[] dim = size.Split('X');
            return new RivieraSize()
            {
                Frente = double.Parse(dim[0]),
                Alto = double.Parse(dim[1]),
                Ancho = PAN_ANCHO_MM.ConvertUnits(Unit_Type.mm, Unit_Type.m),
                Code = PANEL_ITEM
            };
        }
        /// <summary>
        /// Obtiene el tamaño seleccionado para el stack de paneles
        /// </summary>
        /// <param name="size">El tamaño de paneles</param>
        /// <param name="panelRaw">La colección de paneles crudos</param>
        /// <returns>El tamaño seleccionado</returns>
        public RivieraSize GetDoubleSize(Mampara mampara, PanelRaw panelRaw)
        {
            String[] dim = mampara.Size.Split('X');
            string frente = dim[0];
            if (frente == "24")
                frente = "52";
            else
                frente = "40";
            return new RivieraSize()
            {
                Frente = double.Parse(frente),
                Alto = double.Parse(dim[1]),
                Ancho = PAN_ANCHO_MM.ConvertUnits(Unit_Type.mm, Unit_Type.m),
                Code = PANEL_ITEM
            };
        }
    }
}
