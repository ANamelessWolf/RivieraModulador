using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.OldModulador.Controller;
using DaSoft.Riviera.OldModulador.Controller.Delta;
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
    public class RivieraBiombo : RivieraObject
    {
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
        /// La información del Biombo
        /// </summary>
        public PanelRaw RawData;
        /// <summary>
        /// La altura del biombo a insertar
        /// </summary>
        public Double Z;
        /// <summary>
        /// El acabado del biombo
        /// </summary>
        public String Acabado;
        /// <summary>
        /// El punto final del stack
        /// </summary>
        public override Point2d End
        {
            get
            {
                if (App.Riviera.Units == DaNTeUnits.Imperial)
                    return this.Start.ToPoint2dByPolar(this.Frente, this.Angle);
                else
                    return base.End;
            }
        }
        /// <summary>
        /// Set the panel data
        /// </summary>
        public string PanelData;
        /// <summary>
        /// Verdadero si el modo 3D se encuentra activo
        /// </summary>
        public Boolean Is3DEnabled
        {
            get { return App.Riviera.Is3DEnabled; }
        }

        /// <summary>
        /// Crea una nueva instancia de la clase <see cref="RivieraBiombo"/>.
        /// </summary>
        /// <param name="mampara">La mampara asociada al biombo</param>
        /// <param name="code">El código del biombo</param>
        /// <param name="biomboData">La información del biombo.</param>
        public RivieraBiombo(Mampara mampara, String code, PanelRaw biomboData) :
            base(mampara.Start, mampara.GetEndPoint(), GetSize(mampara.Size, biomboData), code)
        {
            this.PanelData = biomboData.ToRow;
            this.RawData = biomboData;

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
        /// Obtiene el tamaño seleccionado para el stack de paneles
        /// </summary>
        /// <param name="size">El tamaño de paneles</param>
        /// <param name="panelRaw">La colección de paneles crudos</param>
        /// <returns>El tamaño seleccionado</returns>
        public static RivieraSize GetSize(string size, PanelRaw panelRaw)
        {
            String[] dim = size.Split('X');
            return new RivieraSize()
            {
                Frente = double.Parse(dim[0]),
                Alto = double.Parse(dim[1]),
                Ancho = PAN_ANCHO_MM.ConvertUnits(Unit_Type.mm, Unit_Type.m),
                Code = PANEL_STACK
            };
        }

        /// <summary>
        /// Crea el contenido de la articulación de la mampara
        /// </summary>
        public override void CreateContent()
        {
            if (Is3DEnabled && this.RawData != null)
            {
                Dictionary<String, AutoCADBlock> blocks = new Dictionary<String, AutoCADBlock>();
                String block;
                FileInfo file;
                FastTransactionWrapper trW = new FastTransactionWrapper(
                    delegate (Document doc, Transaction tr)
                    {
                        PanelRaw pan = this.RawData;
                        try
                        {
                            block = FormatName(pan);
                            file = App.Riviera.Delta3D.Where(x => x.Name.Contains(block) && x.Extension.ToUpper() == ".DWG").FirstOrDefault();
                            if (file != null && !blocks.ContainsKey(block))
                                blocks.Add(block, new AutoCADBlock(block, file, tr));

                        }
                        catch (Exception exc)
                        { App.Riviera.Log.AppendEntry(String.Format(ERR_CREATING_3D_PANEL, this.Code, exc.Message), Protocol.Error, "CreateContent"); }
                    });
                trW.Run();
                Content = blocks;
            }
            else
            {
                Point2d[] pts = new Point2d[]
                {
                   this.Start.ToPoint2dByPolar(this.Ancho / 2, this.Angle - Math.PI / 2),
                   this.End.ToPoint2dByPolar(this.Ancho / 2, this.Angle - Math.PI / 2),
                   this.End.ToPoint2dByPolar(this.Ancho / 2, this.Angle + Math.PI / 2),
                   this.Start.ToPoint2dByPolar(this.Ancho / 2, this.Angle + Math.PI / 2)
                };
                Polyline pl = new Polyline();
                for (int i = 0; i < pts.Length; i++)
                    pl.AddVertexAt(i, pts[i], 0, 0, 0);
                pl.Closed = true;
                Content = pl;
            }
        }
        /// <summary>
        /// Se realiza un formato especial para las mamparas con doble frente
        /// </summary>
        /// <param name="pan">El panel</param>
        /// <returns>El código o nombre de bloque</returns>
        public string FormatName(PanelRaw pan)
        {
            String code = pan.Code;
            if (pan.Code.Length == 12)
            {
                String code1 = code.Substring(6, 2),
                code2 = code.Substring(8, 2),
                code3 = code.Substring(10);
                code = String.Format("{2}{0}{1}", int.Parse(code1) + int.Parse(code2), code3, code.Substring(0, 6));
            }
            return String.Format("{0}{1}", pan.Block, code.Substring(6));
        }

        /// <summary>
        /// Dibuja el contenido de la articulación de la mampara
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        public override void DrawContent(Transaction tr)
        {
            AutoCADLayer lay = new AutoCADLayer(LAYER_RIVIERA_GEOMETRY, tr);
            lay.SetStatus(LayerStatus.EnableStatus, tr);
            if (Is3DEnabled && Content is Dictionary<String, AutoCADBlock>)
                this.Draw3DContent(tr);
            else
                this.Ids.Add(Drawer.Entity(this.Content as Polyline));
            //Se agregán a una capa especial
            lay.AddToLayer(new ObjectIdCollection(this.Ids.OfType<ObjectId>().Where(x => this.Ids.IndexOf(x) > 0).ToArray()), tr);
        }
        /// <summary>
        /// Dibuja el contenido 3d para los elementos de tipo bloque
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        public void Draw3DContent(Transaction tr, Boolean rotate = true)
        {
            Dictionary<String, AutoCADBlock> blocks = Content as Dictionary<String, AutoCADBlock>;
            String block;
            BlockReference blkRef;
            AttManager attMan;
            Point2d insPoint;

            ObjectId blkId;
            Double height, ancho;
            //Cambiar el acabado a una función

            PanelRaw pan = this.RawData;
            block = FormatName(pan);
            if (blocks.ContainsKey(block))
            {

                height = pan.Height;
                if (this.Code.Substring(0, 6) == "DD7000")
                {
                    insPoint = this.Start.ToPoint2dByPolar(this.Ancho / 2, this.Angle + Math.PI / 2);
                    ancho = PICH_ANCHO_SMALL_MM.ConvertUnits(Unit_Type.mm, Unit_Type.m);
                    insPoint = insPoint.ToPoint2dByPolar(ancho, this.Angle + Math.PI / 2);
                }
                else
                    insPoint = this.Start.ToPoint2dByPolar(this.Ancho / 2, this.Angle + Math.PI / 2);



                if (this.CajonerasCodes.Union(this.PichonerasCodes).Contains(this.Code.Substring(0, 6)))
                    height = height + PAN_ANCHO_MM.ConvertUnits(Unit_Type.mm, Unit_Type.m);
                blkRef = blocks[block].CreateReference(insPoint.ToPoint3d(height), this.Direction.Angle);
                //Se agrega el bloque
                blkId = Drawer.Entity(blkRef, tr);
                attMan = new AttManager(blkId);
                attMan.SetAttribute("A", pan.Acabado, tr);
                if (rotate)
                {
                    //Se rota el bloque
                    Vector3d v = insPoint.ToPoint3d(height).GetVectorTo(insPoint.ToPoint2dByPolar(1, this.Direction.Angle).ToPoint3d(height));
                    blkRef.TransformBy(Matrix3d.Rotation(Math.PI / 2, v, blkRef.Position));
                }
                //Se camba la altura para bloques especiales.
                this.Ids.Add(blkRef.Id);
                Double ang = Mampara54Flipper.GetRotation(this, tr);
                if (ang != 0)
                {
                    Matrix3d rot = Matrix3d.Rotation(ang, Vector3d.ZAxis, this.Line.StartPoint.MiddlePointTo(this.Line.EndPoint));
                    (this.Ids[1].GetObject(OpenMode.ForWrite) as Entity).TransformBy(rot);
                }

            }
        }

        public override void Regen(Transaction tr)
        {
            this.CreateContent();
            Drawer.Erase(tr, this.Ids.OfType<ObjectId>().Where(x => this.Ids.IndexOf(x) > 0).ToArray());
            while (this.Ids.Count > 1)
                this.Ids.RemoveAt(1);
            this.DrawContent(tr);
            if (App.Riviera.Units == DaNTeUnits.Imperial)
            {
                Line l = (Line)this.Line.Id.GetObject(OpenMode.ForWrite);
                l.EndPoint = this.End.ToPoint3d(l.EndPoint.Z);
            }
            this.Data.Set(FIELD_GEOMETRY, tr, SaveGeometry(tr));
            if (App.Riviera.Units == DaNTeUnits.Imperial)
                this.Ids.Transform(Matrix3d.Scaling(IMPERIAL_FACTOR, this.Line.StartPoint), tr);
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
            if (panelRaw != null)
                rot = panelRaw.Direction == ArrowDirection.Left_Front ? rot : angle - Math.PI / 2;
            return point.ToPoint2dByPolar(r, rot);
        }
    }
}
