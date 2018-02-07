using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.OldModulador.Controller;
using DaSoft.Riviera.OldModulador.Model.Delta;
using DaSoft.Riviera.OldModulador.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Mio;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Entities;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Model;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu;
using NamelessOld.Libraries.HoukagoTeaTime.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Tsumugi;
using NamelessOld.Libraries.Yggdrasil.Lain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CODE;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CONST;
using static DaSoft.Riviera.OldModulador.Assets.Strings;

namespace DaSoft.Riviera.OldModulador.Model
{
    public class RivieraPanel : RivieraObject
    {
        /// <summary>
        /// La información cruda del panel
        /// </summary>
        public PanelRaw Raw;
        /// <summary>
        /// Regresa la cadena actual del contenido guardado en el stack
        /// </summary>
        public virtual string PanelData
        {
            get
            {
                return this.Raw.ToRow;
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
        /// El punto final del stack
        /// </summary>
        public override Point2d End
        {
            get
            {
                Double offset = 0.0100d;
                if (App.Riviera.Units == DaNTeUnits.Imperial)
                    offset = (offset).ConvertUnits(Unit_Type.m, Unit_Type.inches);
                return this.Start.ToPoint2dByPolar(this.Frente + offset, this.Angle);
            }
        }
        /// <summary>
        /// Devuelve el punto final de la línea
        /// </summary>
        /// <value>
        /// El punto final de la línea
        /// </value>
        public override Point3d LineEndPoint
        {
            get
            {
                return this.End.ToPoint3d();
            }
        }
        /// <summary>
        /// El frente del rectangulo en unidades reales
        /// </summary>
        public override double Frente
        {
            get
            {
                return GetFrente(this.Raw);
            }
        }
        /// <summary>
        /// Inicializa una clase del tipo <see cref="RivieraPanel"/> class.
        /// </summary>
        /// <param name="mampara">La mampara a la que se inserta el panel.</param>
        /// <param name="panel">La información del panel</param>
        public RivieraPanel(Mampara mampara, PanelRaw panel, RivieraPanelDoubleLocation location) :
            base(location.GetPointForRivieraPanel(panel.Direction),
                 location.GetPointForRivieraPanel(panel.Direction, true),
                 location.GetSize(mampara.Size, panel), PANEL_ITEM, false)
        {
            this.Raw = panel;
            this.CreateContent();
        }
        /// <summary>
        /// Inicializa una clase del tipo <see cref="RivieraPanel"/> class.
        /// </summary>
        /// <param name="mampara">La mampara a la que se inserta el panel.</param>
        /// <param name="panel">La información del panel</param>
        public RivieraPanel(Mampara mampara, PanelRaw panel, RivieraSize size, RivieraPanelDoubleLocation location) :
            base(location.GetPointForRivieraPanel(panel.Direction),
                 location.GetPointForRivieraPanel(panel.Direction, true),
                 size, PANEL_ITEM, false)
        {
            this.Raw = panel;
            this.CreateContent();
        }
        /// <summary>
        /// Devuelve el frente del panel seleccionado
        /// </summary>
        /// <param name="panel">El panel seleccionado</param>
        /// <returns>El valor del frente en valor real</returns>
        private static double GetFrente(PanelRaw panel)
        {
            var frente = App.DB.Panel_Size.Where(x => x.Frente == panel.Frente).FirstOrDefault();
            double f;
            if (frente != null)
                f = frente.Real.Frente.ConvertUnits(Unit_Type.mm, Unit_Type.m);
            else
                f = 0d;
            if (App.Riviera.Units == DaNTeUnits.Imperial)
                f = f.ConvertUnits(Unit_Type.m, Unit_Type.inches);
            return f;
        }

        /// <summary>
        /// Guarda la información del panel
        /// </summary>
        /// <param name="tr">La transacción actual</param>
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
                    this.PanelData
                });
        }
        /// <summary>
        /// Crea el contenido de la articulación del panel
        /// </summary>
        public override void CreateContent()
        {
            if (Is3DEnabled && this.Raw != null)
            {
                Dictionary<String, AutoCADBlock> blocks = new Dictionary<String, AutoCADBlock>();
                String block;
                FileInfo file;
                FastTransactionWrapper trW = new FastTransactionWrapper(
                    delegate (Document doc, Transaction tr)
                    {
                        try
                        {
                            block = Formatname(this.Raw);
                            file = App.Riviera.Delta3D.Where(x => x.Name.Contains(block) && x.Extension.ToUpper() == ".DWG").FirstOrDefault();
                            if (file != null && !blocks.ContainsKey(block))
                                blocks.Add(block, new AutoCADBlock( block, file, tr));

                        }
                        catch (Exception exc)
                        { App.Riviera.Log.AppendEntry(String.Format(ERR_CREATING_3D_PANEL, this.Code, exc.Message), Protocol.Error, "CreateContent"); }
                    });
                trW.Run();
                Content = blocks;
            }
            else
                this.CreateContent2D();
        }
        /// <summary>
        /// Crea el contenido 2D
        /// </summary>
        public void CreateContent2D()
        {
            double ancho = this.Ancho;
            if (App.Riviera.Units == DaNTeUnits.Imperial)
                ancho = ancho.ConvertUnits(Unit_Type.m, Unit_Type.inches);
            Point2d[] pts = new Point2d[]
            {
                this.Start.ToPoint2dByPolar(ancho / 2, this.Angle - Math.PI / 2),
                this.End.ToPoint2dByPolar(ancho / 2, this.Angle - Math.PI / 2),
                this.End.ToPoint2dByPolar(ancho / 2, this.Angle + Math.PI / 2),
                this.Start.ToPoint2dByPolar(ancho / 2, this.Angle + Math.PI / 2)
            };
            Polyline pl = new Polyline();
            pl.Color = Autodesk.AutoCAD.Colors.Color.FromRgb(0, 255, 0);
            for (int i = 0; i < pts.Length; i++)
                pl.AddVertexAt(i, pts[i], 0, 0, 0);
            pl.Closed = true;
            Content = pl;
        }

        /// <summary>
        /// Se realiza un formato especial para las mamparas con doble frente
        /// </summary>
        /// <param name="pan">El panel</param>
        /// <returns>El código o nombre de bloque</returns>
        public string Formatname(PanelRaw pan)
        {
            String code = pan.Code;
            if (pan.Code.Length == 12)
            {
                String code1 = code.Substring(6, 2),
                code2 = code.Substring(8, 2),
                code3 = code.Substring(10);
                int num, f1 = int.TryParse(code1, out num) ? num : 0,
                         f2 = int.TryParse(code2, out num) ? num : 0,
                         sumf = f1 + f2;
                if (sumf == 66)
                    code = String.Format("{3}{0:00}{1:00}{2:00}", f1 > f2 ? f2 : f1, f1 > f2 ? f1 : f2, code3, code.Substring(0, 6));
                else if (sumf == 54)
                    code = String.Format("{3}{0:00}{1:00}{2:00}", 24, 30, code3, code.Substring(0, 6));
                else
                    code = String.Format("{3}{0:00}{1:00}{2:00}", int.Parse(code1), int.Parse(code2), code3, code.Substring(0, 6));
            }
            return String.Format("{0}{1}", pan.Block, code.Substring(6));
        }

        /// <summary>
        /// Dibuja el contenido de la articulación de la mampara
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        public override void DrawContent(Transaction tr)
        {
            AutoCADLayer lay = new AutoCADLayer( LAYER_RIVIERA_GEOMETRY, tr);
            lay.SetStatus( LayerStatus.EnableStatus,tr);
            if (Is3DEnabled && Content is Dictionary<String, AutoCADBlock>)
            {
                Dictionary<String, AutoCADBlock> blocks = Content as Dictionary<String, AutoCADBlock>;
                String block;
                BlockReference blkRef;
                AttManager attMan;
                Point2d insPoint;
                ObjectId blkId;
                Double height;
                //Cambiar el acabado a una función
                //if (this.Raw.Side == PanelSide.Lado_AB)
                //{
                //    this.Raw.Block = "DD2015";
                //    int h = int.Parse(this.Raw.Code.Substring(8, 2)) - 1;
                //    this.Raw.Code = this.Raw.Code.Substring(0, this.Raw.Code.Length - 2) + String.Format("{0:00}", h);
                //}
                block = Formatname(this.Raw);

                if (blocks.ContainsKey(block))
                {
                    if (Raw.APiso)
                        height = this.Raw.Height;
                    else
                        height = this.Raw.Height + 0.004d;
                    if (App.Riviera.Units == DaNTeUnits.Imperial)
                        height = height.ConvertUnits(Unit_Type.m, Unit_Type.inches);
                    Double ancho = this.Ancho / 2;
                    Double front = PAN_OFFSET_M;
                    if (App.Riviera.Units == DaNTeUnits.Imperial)
                    {
                        ancho = ancho.ConvertUnits(Unit_Type.m, Unit_Type.inches);
                        front = front.ConvertUnits(Unit_Type.m, Unit_Type.inches);
                    }

                    insPoint = this.Start.ToPoint2dByPolar(ancho, this.Angle + Math.PI / 2);
                    insPoint = insPoint.ToPoint2dByPolar(front, this.Angle);
                    blkRef = blocks[block].CreateReference(insPoint.ToPoint3d(height), this.Direction.Angle, App.Riviera.Units == DaNTeUnits.Imperial ? 1d.ConvertUnits(Unit_Type.m, Unit_Type.inches) : 1);
                    //Se agrega el bloque
                    blkId = Drawer.Entity(blkRef, tr);
                    attMan = new AttManager(blkId);
                    SetAttributes(tr, attMan);
                    //Se rota el bloque
                    Vector3d v = insPoint.ToPoint3d(height).GetVectorTo(insPoint.ToPoint2dByPolar(1, this.Direction.Angle).ToPoint3d(height));
                    var parent = App.DB[this.Parent];
                    if (parent != null && parent is JointObject)
                    {
                        JointObject jointParent = parent as JointObject;
                        if (this.Raw.Direction == ArrowDirection.Back || this.Raw.Direction == ArrowDirection.Left_Back || this.Raw.Direction == ArrowDirection.Right_Back)
                        {
                            blkRef.TransformBy(Matrix3d.Rotation(Math.PI / 2, v, blkRef.Position));
                            blkRef.TransformBy(Matrix3d.Rotation(Math.PI, Vector3d.ZAxis, this.Start.MiddlePointTo(this.End).ToPoint3d()));
                        }
                        else
                            blkRef.TransformBy(Matrix3d.Rotation(Math.PI / 2, v, blkRef.Position));
                        this.Ids.Add(blkRef.Id);
                    }
                }
            }
            else
                this.Ids.Add(Drawer.Entity(this.Content as Polyline));
            //Se agregán a una capa especial
            if (this.Ids.OfType<ObjectId>().Count(x => this.Ids.IndexOf(x) > 0) > 0)
                lay.AddToLayer(new ObjectIdCollection(this.Ids.OfType<ObjectId>().Where(x => this.Ids.IndexOf(x) > 0).ToArray()), tr);
        }
        /// <summary>
        /// Se encarga de establecer el atributo de bloque.
        /// </summary>
        /// <param name="tr">The tr.</param>
        /// <param name="attMan">The att man.</param>
        public virtual void SetAttributes(Transaction tr, AttManager attMan)
        {
            string code = this.Raw.Code.Substring(6, 4);
            if (this.Raw.Acabado != null && this.Raw.Acabado != String.Empty)
                attMan.SetAttribute("A", this.Raw.Acabado, tr);
            if (code == "5206")
                attMan.SetAttribute("B", this.Raw.Acabado, tr);
        }

        public override void Regen(Transaction tr)
        {
            this.CreateContent();
            Drawer.Erase(tr, this.Ids.OfType<ObjectId>().Where(x => this.Ids.IndexOf(x) > 0).ToArray());
            while (this.Ids.Count > 1)
                this.Ids.RemoveAt(1);
            this.DrawContent(tr);
            this.Data.Set(FIELD_GEOMETRY, tr, SaveGeometry(tr));
        }

        /// <summary>
        /// Obtiene el punto de la línea de información del panel
        /// </summary>
        /// <param name="size">El tamaño de paneles</param>
        /// <param name="panelRaw">La colección de paneles crudos</param>
        /// <returns>El tamaño seleccionado</returns>
        private static Point2d GetPoint(Point2d point, double angle, PanelRaw panelRaw)
        {
            Double rot = angle + Math.PI / 2,
                r = STACK_ANCHO_MM.ConvertUnits(Unit_Type.mm, Unit_Type.m);
            if (App.Riviera.Units == DaNTeUnits.Imperial)
                r = r.ConvertUnits(Unit_Type.m, Unit_Type.inches);
            if (panelRaw != null)
                rot = panelRaw.Direction == ArrowDirection.Front ? angle - Math.PI / 2 : rot;
            return point.ToPoint2dByPolar(r, rot);
        }

        /// <summary>
        /// Obtiene el punto de la línea de información del panel
        /// </summary>
        /// <param name="size">El tamaño de paneles</param>
        /// <param name="panelRaw">La colección de paneles crudos</param>
        /// <returns>El tamaño seleccionado</returns>
        private static Point2d GetEndPoint(Mampara mam, PanelRaw raw)
        {
            Double offset = 0.0100d;
            var start = mam.Start;
            if (App.Riviera.Units == DaNTeUnits.Imperial)
                offset = (offset).ConvertUnits(Unit_Type.m, Unit_Type.inches);
            return start.ToPoint2dByPolar(GetFrente(raw) + offset, mam.Angle);
        }



    }
}