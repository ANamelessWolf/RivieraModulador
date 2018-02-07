using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.OldModulador.Controller;
using DaSoft.Riviera.OldModulador.Model.Delta;
using DaSoft.Riviera.OldModulador.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Mio;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Entities;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Model;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Styler;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu;
using NamelessOld.Libraries.HoukagoTeaTime.Runtime;
using NamelessOld.Libraries.Yggdrasil.Lain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CONST;
using static DaSoft.Riviera.OldModulador.Assets.Strings;

namespace DaSoft.Riviera.OldModulador.Model
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="DaSoft.Riviera.OldModulador.Model.RivieraBiombo" />
    public class RivieraPichonera : RivieraBiombo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RivieraPichonera"/> class.
        /// </summary>
        /// <param name="mampara">La mampara asociada al pichonera</param>
        /// <param name="code">El código de la pichonera</param>
        /// <param name="pichoneraData">La información del pichonera.</param>
        public RivieraPichonera(Mampara mampara, String code, PanelRaw pichoneraData) :
            base(mampara, code, pichoneraData)
        {

        }

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
                Polyline[] geometry;
                GetPichoneraGeometry(out geometry);
                this.Content = geometry;

            }
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
                this.Draw3DContent(tr, false);
                if (this.Code.Substring(0, 6) == "DD7000" && this.RawData.Side == PanelSide.Lado_A)
                {
                    Double h = 0.112d + this.RawData.Height;
                    Matrix3d mT = Matrix3d.Mirroring(new Line3d(this.Start.ToPoint3d(h), this.End.ToPoint3d(h)));
                    (this.Ids.OfType<ObjectId>().Last().GetObject(OpenMode.ForWrite) as BlockReference).TransformBy(mT);
                }
                else
                    this.Draw3DContent(tr, false);

            }
            else
            {
                LineStyler lS = new LineStyler(tr);

                foreach (ObjectId id in Drawer.Entity(tr, this.Content as Polyline[]))
                {
                    Polyline pl = (id.GetObject(OpenMode.ForWrite) as Polyline);
                    if (!lS.LineTypes.ContainsKey(STYLE_DASHED_LINE))
                    {
                        if (lS.LoadLineStyle(tr, STYLE_DASHED_LINE))
                            pl.LinetypeId = lS.LineTypes[STYLE_DASHED_LINE];
                    }
                    else
                        pl.LinetypeId = lS.LineTypes[STYLE_DASHED_LINE];
                    pl.LinetypeScale = 0.005d;
                    if (App.Riviera.Units == DaNTeUnits.Imperial)
                        pl.LinetypeScale = pl.LinetypeScale.ConvertUnits(Unit_Type.m, Unit_Type.inches);
                    if (this.Code.Substring(0, 6) == "DD7000" && this.RawData.Side == PanelSide.Lado_A)
                    {
                        Matrix3d mT = Matrix3d.Mirroring(new Line3d(this.Start.ToPoint3d(), this.End.ToPoint3d()));
                        pl.TransformBy(mT);
                    }
                    this.Ids.Add(id);
                }
            }
            //Se agregán a una capa especial
            lay.AddToLayer(new ObjectIdCollection(this.Ids.OfType<ObjectId>().Where(x => this.Ids.IndexOf(x) > 0).ToArray()), tr);
        }



        private void GetPichoneraGeometry(out Polyline[] pls)
        {
            pls = new Polyline[0];

            if (this.Code.Substring(0, 6) == "DD7000")
                pls = this.CreateDD7000_2D();
            else if (this.Code.Substring(0, 6) == "DD7001")
            {
                Boolean drawMiddles = true;
                string frente = this.Code.Substring(6, 2);
                int num = int.TryParse(frente, out num) ? num : 0;
                drawMiddles = num >= 42;
                pls = this.CreateDD7001_2D(drawMiddles);
            }
            else if (this.Code.Substring(0, 6) == "DD7002" || this.Code.Substring(0, 6) == "DD7003")
                pls = this.CreateDD7002_2D();

        }
        /// <summary>
        /// Define la geometría 2D de para la pichonera DD7000
        /// </summary>
        /// <returns>Las polilíneas que forman la geometria</returns>
        private Polyline[] CreateDD7000_2D()
        {
            Point2d[] pts;
            Double ancho1 = PICH_ANCHO_SMALL_MM.ConvertUnits(Unit_Type.mm, Unit_Type.m),
                ancho2 = PICH_ANCHO_LARGE_MM.ConvertUnits(Unit_Type.mm, Unit_Type.m),
                frente = this.Frente,
                frenteSmall = PICH_FRENTE_SMALL_MM.ConvertUnits(Unit_Type.mm, Unit_Type.m);
            Polyline[] pls = new Polyline[2];
            pts = new Point2d[11];
            //Polilínea inicial
            pts[0] = this.Start;
            pts[1] = pts[0].ToPoint2dByPolar(ancho1, this.Angle + Math.PI / 2);
            pts[2] = pts[1].ToPoint2dByPolar(Frente, this.Angle);
            pts[3] = pts[2].ToPoint2dByPolar(ancho2, this.Angle - Math.PI / 2);
            pts[4] = pts[3].ToPoint2dByPolar(Frente, this.Angle + Math.PI);
            pls[0] = RivieraExtender.CreatePolyline(true, pts.SubArray(1, 4));
            //Segunda polilínea
            pts[5] = pts[0].ToPoint2dByPolar(frenteSmall, this.Angle);
            pts[6] = pts[5].ToPoint2dByPolar(ancho1 - frenteSmall, this.Angle + Math.PI / 2);
            pts[7] = pts[6].ToPoint2dByPolar(ancho2 - frenteSmall, this.Angle - Math.PI / 2);
            pts[8] = pts[6];
            pts[9] = pts[8].ToPoint2dByPolar(Frente - 2 * frenteSmall, this.Angle);
            pts[10] = pts[9].ToPoint2dByPolar(ancho2 - frenteSmall, this.Angle - Math.PI / 2);
            pls[1] = RivieraExtender.CreatePolyline(false, pts.SubArray(7, 10));
            return pls;
        }
        /// <summary>
        /// Define la geometría 2D de para la pichonera DD7001
        /// </summary>
        /// <returns>Las polilíneas que forman la geometria</returns>
        private Polyline[] CreateDD7001_2D(Boolean drawMiddles)
        {
            Point2d[] pts;
            Double ancho = PICH_ANCHO_HALF_MM.ConvertUnits(Unit_Type.mm, Unit_Type.m),
                   frente = this.Frente,
                   frenteSmall = PICH_FRENTE_SMALL_MM.ConvertUnits(Unit_Type.mm, Unit_Type.m);
            Polyline[] pls;
            if (!drawMiddles)
            {
                pls = new Polyline[5];
                pts = new Point2d[15];
            }
            else
            {
                pls = new Polyline[7];
                pts = new Point2d[21];
            }
            //Primer polilínea
            pts[0] = this.Start;
            pts[1] = pts[0].ToPoint2dByPolar(ancho, this.Angle + Math.PI / 2);
            pts[2] = pts[1].ToPoint2dByPolar(frente, this.Angle);
            pts[3] = pts[2].ToPoint2dByPolar(ancho * 2, this.Angle - Math.PI / 2);
            pts[4] = pts[3].ToPoint2dByPolar(frente, this.Angle + Math.PI);
            pls[0] = RivieraExtender.CreatePolyline(true, pts.SubArray(1, 4));
            //Segunda polilínea
            pts[5] = pts[0].ToPoint2dByPolar(frenteSmall, this.Angle);
            pts[6] = pts[5].ToPoint2dByPolar(ancho, this.Angle + Math.PI / 2);
            pts[7] = pts[5].ToPoint2dByPolar(ancho, this.Angle - Math.PI / 2);
            pls[1] = RivieraExtender.CreatePolyline(false, pts.SubArray(6, 7));
            //Tercera polilínea
            pts[8] = pts[5].ToPoint2dByPolar(frente - frenteSmall, this.Angle);
            pts[9] = pts[8].ToPoint2dByPolar(ancho, this.Angle + Math.PI / 2);
            pts[10] = pts[8].ToPoint2dByPolar(ancho, this.Angle - Math.PI / 2);
            pls[2] = RivieraExtender.CreatePolyline(false, pts.SubArray(9, 10));
            //Cuarta polilínea
            pts[11] = pts[5].ToPoint2dByPolar(frenteSmall / 2d, this.Angle + Math.PI / 2);
            pts[12] = pts[8].ToPoint2dByPolar(frenteSmall / 2d, this.Angle + Math.PI / 2);
            pls[3] = RivieraExtender.CreatePolyline(false, pts.SubArray(11, 12));
            //Quinta polilínea
            pts[13] = pts[5].ToPoint2dByPolar(frenteSmall / 2d, this.Angle - Math.PI / 2);
            pts[14] = pts[8].ToPoint2dByPolar(frenteSmall / 2d, this.Angle - Math.PI / 2);
            pls[4] = RivieraExtender.CreatePolyline(false, pts.SubArray(13, 14));
            if (drawMiddles)
            {
                //Sexta polilínea
                pts[15] = pts[0].ToPoint2dByPolar(frente / 2d - frenteSmall / 2, this.Angle);
                pts[16] = pts[15].ToPoint2dByPolar(ancho, this.Angle + Math.PI / 2);
                pts[17] = pts[15].ToPoint2dByPolar(ancho, this.Angle - Math.PI / 2);
                pls[5] = RivieraExtender.CreatePolyline(false, pts.SubArray(16, 17));
                //Septima polilínea
                pts[18] = pts[0].ToPoint2dByPolar(frente / 2d + frenteSmall / 2, this.Angle);
                pts[19] = pts[18].ToPoint2dByPolar(ancho, this.Angle + Math.PI / 2);
                pts[20] = pts[18].ToPoint2dByPolar(ancho, this.Angle - Math.PI / 2);
                pls[6] = RivieraExtender.CreatePolyline(false, pts.SubArray(19, 20));
            }
            return pls;
        }
        /// <summary>
        /// Define la geometría 2D de para la pichonera DD7002 o DD7003
        /// </summary>
        /// <returns>Las polilíneas que forman la geometria</returns>
        private Polyline[] CreateDD7002_2D()
        {
            Point2d[] pts;
            Double ancho = PICH_ANCHO_HALF_MM.ConvertUnits(Unit_Type.mm, Unit_Type.m),
                   frente = this.Frente,
                   frenteSmall = PICH_FRENTE_SMALL_MM.ConvertUnits(Unit_Type.mm, Unit_Type.m);
            Polyline[] pls = new Polyline[3];
            pts = new Point2d[13];
            //Primer polilínea
            pts[0] = this.Start;
            pts[1] = pts[0].ToPoint2dByPolar(ancho, this.Angle + Math.PI / 2);
            pts[2] = pts[1].ToPoint2dByPolar(frente, this.Angle);
            pts[3] = pts[2].ToPoint2dByPolar(ancho * 2, this.Angle - Math.PI / 2);
            pts[4] = pts[3].ToPoint2dByPolar(frente, this.Angle + Math.PI);
            pls[0] = RivieraExtender.CreatePolyline(true, pts.SubArray(1, 4));
            //Segunda polilínea
            pts[5] = pts[1].ToPoint2dByPolar(frenteSmall, this.Angle);
            pts[6] = pts[5].ToPoint2dByPolar(ancho * 2 - frenteSmall, this.Angle - Math.PI / 2);
            pts[7] = pts[6].ToPoint2dByPolar(frente / 2 - 1.5 * frenteSmall, this.Angle);
            pts[8] = pts[7].ToPoint2dByPolar(ancho * 2 - frenteSmall, this.Angle + Math.PI / 2);
            pls[1] = RivieraExtender.CreatePolyline(false, pts.SubArray(5, 8));
            //Tercera polilínea
            pts[9] = pts[8].ToPoint2dByPolar(frenteSmall, this.Angle).ToPoint2dByPolar(ancho * 2, this.Angle - Math.PI / 2);
            pts[10] = pts[9].ToPoint2dByPolar(ancho * 2 - frenteSmall, this.Angle + Math.PI / 2);
            pts[11] = pts[10].ToPoint2dByPolar(frente / 2 - 1.5 * frenteSmall, this.Angle);
            pts[12] = pts[11].ToPoint2dByPolar(ancho * 2 - frenteSmall, this.Angle - Math.PI / 2);
            pls[2] = RivieraExtender.CreatePolyline(false, pts.SubArray(9, 12));
            return pls;
        }

    }
}
