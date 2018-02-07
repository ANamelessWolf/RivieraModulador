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
using System.Text;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CODE;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CONST;
using static DaSoft.Riviera.OldModulador.Assets.Strings;

namespace DaSoft.Riviera.OldModulador.Model
{
    public class RivieraPanelStack : RivieraObject
    {
        /// <summary>
        /// La colección de paneles
        /// </summary>
        public List<PanelRaw> Collection;
        /// <summary>
        /// Regresa la cadena actual del contenido guardado en el stack
        /// </summary>
        private string PanelData
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                Collection.ForEach(x => sb.Append(x.ToRow));
                return sb.ToString();
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
                if (App.Riviera.Units == DaNTeUnits.Imperial)
                    return this.Start.ToPoint2dByPolar(this.Frente, this.Angle);
                else
                    return base.End;
            }
        }
        /// <summary>
        /// Crea un nuevo rectangulo gráfico
        /// </summary>
        /// <param name="pt0">El punto inicial del rectangulo</param>
        /// <param name="ptf">El punto final del rectangulo</param>
        /// <param name="size">El tamaño del rectangulo</param>
        public RivieraPanelStack(Mampara mampara, params PanelRaw[] panels) :
            base(GetPoint(mampara.Start, mampara.Angle, panels.FirstOrDefault()),
                 GetPoint(mampara.GetEndPoint(), mampara.Angle, panels.FirstOrDefault()),
                 GetSize(mampara.Size, panels.FirstOrDefault()), PANEL_STACK)
        {
            this.Collection = panels.ToList();

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
        private static RivieraSize GetSize(string size, PanelRaw panelRaw)
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
            if (Is3DEnabled && this.Collection != null)
            {
                Dictionary<String, AutoCADBlock> blocks = new Dictionary<String, AutoCADBlock>();
                String block;
                FileInfo file;
                FastTransactionWrapper trW = new FastTransactionWrapper(
                    delegate (Document doc, Transaction tr)
                    {
                        foreach (PanelRaw pan in this.Collection)
                        {
                            try
                            {
                                block = Formatname(pan);
                                file = App.Riviera.Delta3D.Where(x => x.Name.Contains(block) && x.Extension.ToUpper() == ".DWG").FirstOrDefault();
                                if (file != null && !blocks.ContainsKey(block))
                                    blocks.Add(block, new AutoCADBlock(block, file, tr));

                            }
                            catch (Exception exc)
                            { App.Riviera.Log.AppendEntry(String.Format(ERR_CREATING_3D_PANEL, this.Code, exc.Message), Protocol.Error, "CreateContent"); }
                        }
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
        private string Formatname(PanelRaw pan)
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
            AutoCADLayer lay = new AutoCADLayer(LAYER_RIVIERA_GEOMETRY, tr);
            lay.SetStatus(LayerStatus.EnableStatus, tr);
            if (Is3DEnabled && Content is Dictionary<String, AutoCADBlock>)
            {
                Dictionary<String, AutoCADBlock> blocks = Content as Dictionary<String, AutoCADBlock>;
                String block;
                BlockReference blkRef;
                AttManager attMan;
                Point2d insPoint;
                ObjectId blkId;
                Double height;
                Boolean firstIsAPiso = this.Collection.FirstOrDefault().APiso;
                Boolean first = true;
                //Cambiar el acabado a una función
                foreach (PanelRaw pan in this.Collection)
                {
                    block = Formatname(pan);
                    if (blocks.ContainsKey(block))
                    {

                        height = (pan.APiso ? 0 : 0.128d) + pan.Height;
                        if (first)
                            first = false;
                        else
                            height += 0.004d;
                        insPoint = this.Start.ToPoint2dByPolar(this.Ancho / 2, this.Angle + Math.PI / 2);
                        insPoint = insPoint.ToPoint2dByPolar(PAN_OFFSET_M, this.Angle);
                        blkRef = blocks[block].CreateReference(insPoint.ToPoint3d(height), this.Direction.Angle);
                        //Se agrega el bloque
                        blkId = Drawer.Entity(blkRef, tr);
                        attMan = new AttManager(blkId);
                        if (pan.Acabado != null && pan.Acabado != String.Empty)
                            attMan.SetAttribute("A", pan.Acabado, tr);
                        //Se rota el bloque
                        Vector3d v = insPoint.ToPoint3d(height).GetVectorTo(insPoint.ToPoint2dByPolar(1, this.Direction.Angle).ToPoint3d(height));

                        if (App.DB[this.Parent]?.Children[DaSoft.Riviera.OldModulador.Assets.Strings.FIELD_LEFT_FRONT] == this.Handle.Value)
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
            //if (this.Ids.Count == 2)
            //    this.Data.Set(FIELD_GEOMETRY, tr, this.Ids[1].GetObject(OpenMode.ForRead).Handle.Value.ToString());
            //else
            //{
            //    IEnumerable<String> handles = Ids.OfType<ObjectId>().Where(x => Ids.IndexOf(x) != 0).Select(y => y.Handle.Value.ToString());
            //    StringBuilder sb = new StringBuilder();

            //    foreach(String h in handles)
            //        sb.Append(h + ",");
            //    String val = sb.ToString().Substring(0, sb.ToString().Length - 1);
            //    this.Data.Set(FIELD_GEOMETRY, tr, val);
            //}
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
            if (App.Riviera.Units == DaNTeUnits.Imperial)
                r = r.ConvertUnits(Unit_Type.m, Unit_Type.inches);
            if (panelRaw != null)
                rot = panelRaw.Direction == ArrowDirection.Left_Front ? rot : angle - Math.PI / 2;
            return point.ToPoint2dByPolar(r, rot);
        }
    }
}
