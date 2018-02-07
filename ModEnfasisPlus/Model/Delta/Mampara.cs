using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.OldModulador.Controller;
using DaSoft.Riviera.OldModulador.Controller.Delta;
using DaSoft.Riviera.OldModulador.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Mio;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Entities;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Model;
using NamelessOld.Libraries.HoukagoTeaTime.MugiChan;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu;
using NamelessOld.Libraries.HoukagoTeaTime.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static DaSoft.Riviera.OldModulador.Assets.Strings;

namespace DaSoft.Riviera.OldModulador.Model.Delta
{
    public class Mampara : RivieraObject, IBlockObject
    {
        /// <summary>
        /// The 2D Block file
        /// </summary>
        public FileInfo BlockFile2d
        {
            get
            {
                return this.Code.BlockFile2D();
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
        /// El nombre del prefijo de bloque en donde se insertan los 
        /// bloques de mampara
        /// </summary>
        public String Spacename
        {
            get
            {
                return String.Format(PREFIX_BLOCK, this.Code);
            }
        }
        /// <summary>
        /// Verdadero si el modo 3D se encuentra activo
        /// </summary>
        public Boolean Is3DEnabled
        {
            get { return App.Riviera.Is3DEnabled; }
        }
        /// <summary>
        /// El contenido del bloque a insertar
        /// </summary>
        public AutoCADBlock Block { get { return _Block; } set { _Block = value; } }
        AutoCADBlock _Block;
        /// <summary>
        /// Accede al remate asignado a la mampara
        /// </summary>
        public List<MamparaRemateFinal> Remates;
        /// <summary>
        /// Checa si la mampara tiene paneles, los
        /// paneles solo se guardan en los nodos Izquierdo frontal y derecho frontal
        /// </summary>
        public Boolean HasPanels
        {
            get { return this.Children[FIELD_LEFT_FRONT] != 0 && this.Children[FIELD_RIGHT_FRONT] != 0; }
        }
        /// <summary>
        /// Obtiene el angulo de la mampara con respecto al 
        /// centro de la unión
        /// </summary>
        /// <param name="obj">La unión seleccionada</param>
        /// <returns>El ángulo seleccionado</returns>
        public Double GetAngle(JointObject obj)
        {
            Double angle = this.Angle;
            try
            {
                Point2d center = obj.GeometricExtents[0].MiddlePointTo(obj.GeometricExtents[1]);
                //La regla dice que las mamparas salen de la unión, por lo que el start point debe ser el punto más
                //cercano al centro de la union
                double dStart = center.GetDistanceTo(this.Start),
                       dEnd = center.GetDistanceTo(this.End);
                //La mampara esta dibujada de cabeza, sumar 180°
                if (dEnd < dStart)
                    angle += Math.PI;
            }
            catch (Exception exc)
            {
                App.Riviera.Log.AppendEntry(exc.Message, NamelessOld.Libraries.Yggdrasil.Lain.Protocol.Error, "GetAngle", "Mampara");
            }
            return angle;
        }

        /// <summary>
        /// El id del Biombo
        /// </summary>
        public long BiomboId;

        /// <summary>
        /// Verdadero cuando la mampara tiene asociado un panel doble y se encuantra ubicada en una posición izquierda o derecha
        /// </summary>
        public Boolean HasDoublePanels
        {
            get
            {
                var joints = App.DB.Objects.Where(x => (x is MamparaJoint) && (x as MamparaJoint).Type == JointType.Joint_T && (x as MamparaJoint).PanelArray != null);
                if (joints.Count() > 0)
                {
                    var joint = joints.Where(x => x.Parent == this.Handle.Value || x.Children.Values.Count(y => y == this.Handle.Value) > 0).FirstOrDefault();
                    Mampara left, right, top;
                    //Se realiza el proceso de selección de la mampara
                    if (joint != null)
                    {
                        RivieraPanelDoubleLocation.FindMampara(joint as MamparaJoint, out left, out right, out top);
                        if (left.Handle.Value == this.Handle.Value || right.Handle.Value == this.Handle.Value)
                            return true;
                        else
                            return false;

                    }
                    else
                        return false;
                }
                else
                    return false;
            }
        }
        /// <summary>
        /// Verdadero cuando la mampara tiene asociado un panel doble y se encuantra ubicada en una posición izquierda o derecha
        /// </summary>
        public MamparaJoint JointDoublePanels
        {
            get
            {
                var joints = App.DB.Objects.Where(x => (x is MamparaJoint) && (x as MamparaJoint).Type == JointType.Joint_T && (x as MamparaJoint).PanelArray != null);
                if (joints.Count() > 0)
                {
                    var joint = joints.Where(x => x.Parent == this.Handle.Value || x.Children.Values.Count(y => y == this.Handle.Value) > 0).FirstOrDefault();
                    //Se realiza el proceso de selección de la mampara
                    if (joint != null)
                        return joint as MamparaJoint;
                    else
                        return null;
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets the extra information
        /// First element is the BiomboId
        /// </summary>
        public String Extra
        {
            get { return String.Format("{0}", BiomboId); }
        }
        /// <summary>
        /// Crea un mampara
        /// </summary>
        /// <param name="pt0">El punto inicial del rectangulo</param>
        /// <param name="ptf">El punto final del rectangulo</param>
        /// <param name="size">El tamaño del rectangulo</param>
        public Mampara(Point2d pt0, Point2d ptf, RivieraSize size, String code) :
            base(pt0, ptf, size, code)
        {
            this.BiomboId = 0;
        }
        /// <summary>
        /// Crea el contenido de la mampara en este caso la inserción del bloque.
        /// </summary>
        public override void CreateContent()
        {
            if (this.BlockContent(this.Code))
                this.Content = new AutoCADBlock(this.Spacename);
            else
                base.CreateContent();
        }
        /// <summary>
        /// Borra los paneles asociados a está mampara
        /// </summary>
        public void ErasePanels(Transaction tr)
        {
            if (this.HasPanels)
            {
                RivieraObject leftPanel = App.DB[this.Children[FIELD_LEFT_FRONT]],
                              rightPanel = App.DB[this.Children[FIELD_RIGHT_FRONT]];
                leftPanel.Children[FIELD_LEFT_FRONT] = 0;
                leftPanel.Children[FIELD_RIGHT_FRONT] = 0;
                leftPanel.Delete(tr);
                rightPanel.Delete(tr);
            }
        }

        /// <summary>
        /// Realiza el intercambio del contenido de la mampara hacia otro tipo de
        /// mampara
        /// </summary>
        /// <param name="size">El nuevo tamaño de la mampara</param>
        /// <param name="code">El nuevo código de la mampara</param>
        /// <param name="tr">La transacción activa</param>
        public void Change(RivieraSize size, string code, Point2d start, Point2d end, Transaction tr)
        {
            Line l = this.Line.Id.GetObject(OpenMode.ForWrite) as Line;
            this.Line.StartPoint = start.ToPoint3d();
            this.Line.EndPoint = end.ToPoint3d();
            base.SetSize(size);
            this.Code = code;
            this.Regen(tr);
            this.Save(tr);
        }
        /// <summary>
        /// Realiza el dibujado del bloque a insertar
        /// </summary>
        /// <param name="tr">La transacción del bloque a insertar</param>
        public override void DrawContent(Transaction tr)
        {
            AutoCADBlock space = this.Content as AutoCADBlock;
            if (this.DrawBlockContent(space, this.Code, tr))
            {
                AutoCADLayer lay = new AutoCADLayer(LAYER_RIVIERA_GEOMETRY, tr);
                lay.SetStatus(LayerStatus.EnableStatus, tr);
                this.Ids.Add(Drawer.Entity(space.CreateReference(this.Line.StartPoint, this.Angle)));
                //En las mamparas el segundo elemento Ids es el bloque
                Double ang = Mampara54Flipper.GetRotation(this, tr);
                if (ang != 0)
                {
                    Matrix3d rot = Matrix3d.Rotation(ang, Vector3d.ZAxis, this.Line.StartPoint.MiddlePointTo(this.Line.EndPoint));
                    (this.Ids[1].GetObject(OpenMode.ForWrite) as Entity).TransformBy(rot);
                }
                //Se agregán a una capa especial
                lay.AddToLayer(new ObjectIdCollection(this.Ids.OfType<ObjectId>().Where(x => this.Ids.IndexOf(x) > 0).ToArray()), tr);
            }
            else
                base.DrawContent(tr);

        }
        /// <summary>
        /// Dibuja las etiquetas de lado de las mamparas
        /// </summary>
        /// <param name="sideAId">La salida es el id de la etiqueta del ladao A</param>
        /// <param name="sideBId">La salida es el id de la etiqueta del ladao B</param>
        public void DrawSideTags(out ObjectId sideAId, out ObjectId sideBId)
        {
            BlankTransactionWrapper<ObjectId[]> fT = new BlankTransactionWrapper<ObjectId[]>(
                delegate (Document doc, Transaction tr)
            {
                Double ang = this.Angle,
                ancho = this.Ancho;
                if (App.Riviera.Units == DaNTeUnits.Imperial)
                    ancho = ancho.ConvertUnits(Unit_Type.m, Unit_Type.inches);
                return new ObjectId[]
                {
                    Drawer.Text("Lado A", this.Angle, this.Start.ToPoint2dByPolar(ancho, ang + Math.PI / 2).ToPoint3d(), ancho / 2, new Margin(), tr, String.Empty),
                    Drawer.Text("Lado B", this.Angle, this.Start.ToPoint2dByPolar(ancho, ang - Math.PI / 2).ToPoint3d(), ancho / 2, new Margin(), tr, String.Empty)
                };
            });
            ObjectId[] res = fT.Run();
            sideAId = res[0];
            sideBId = res[1];
        }
        /// <summary>
        /// Crea las etiquetas de reportes de las mamparas seleccionadas
        /// </summary>
        /// <param name="sideA">El texto de la etiqueta para el lado A</param>
        /// <param name="sideB">El texto de la etiqueta para el lado B</param>
        /// <param name="sideATag">La salida es la etiqueta del ladao A</param>
        /// <param name="sideBTag">La salida es la etiqueta del ladao B</param>
        public void CreateReportTags(String sideA, String sideB, out DBText sideATag, out DBText sideBTag)
        {
            DBText[] res;
            double ancho = (this.Ancho);
            if (App.Riviera.Units == DaNTeUnits.Imperial)
                ancho = (this.Ancho).ConvertUnits(Unit_Type.m, Unit_Type.inches);
            res = new DBText[]
            {
                Drawer.CreateText(sideA, this.Angle, this.Start.MiddlePointTo(this.End).ToPoint2dByPolar(ancho/2, this.Angle + Math.PI / 2).ToPoint3d(), ancho / 2, new Margin()),
                Drawer.CreateText(sideB, this.Angle, this.Start.MiddlePointTo(this.End).ToPoint2dByPolar(ancho, this.Angle - Math.PI / 2).ToPoint3d(), ancho / 2, new Margin())
            };
            sideATag = res[0];
            sideBTag = res[1];
        }
        /// <summary>
        /// Checa si dos mamparas son similares,
        /// dos mamparas son similares si tienen mismo frente y misma altura
        /// </summary>
        /// <param name="mam">La mampara a comparar</param>
        /// <returns>Verdadero si son similares</returns>
        public bool IsSimilateTo(Mampara mam)
        {
            return this.Frente == mam.Frente && this.Alto == mam.Alto;
        }
    }
}
