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
    public class RivieraCajonera : RivieraBiombo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RivieraCajonera"/> class.
        /// </summary>
        /// <param name="mampara">La mampara asociada al cajonera</param>
        /// <param name="code">El código de la cajonera</param>
        /// <param name="cajoneraData">La información del cajonera.</param>
        public RivieraCajonera(Mampara mampara, String code, PanelRaw cajoneraData) :
            base(mampara, code, cajoneraData)
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
                Content = Create2D();
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
                this.Draw3DContent(tr, false);
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
                    this.Ids.Add(id);
                }
            }
            //Se agregán a una capa especial
            lay.AddToLayer(new ObjectIdCollection(this.Ids.OfType<ObjectId>().Where(x => this.Ids.IndexOf(x) > 0).ToArray()), tr);
        }
        /// <summary>
        /// Define la geometría 2D de para las cajoneras
        /// </summary>
        /// <returns>Las polilíneas que forman la geometria</returns>
        private Polyline[] Create2D()
        {
            Point2d[] pts;
            Autodesk.AutoCAD.Colors.Color cyan = Autodesk.AutoCAD.Colors.Color.FromRgb(0, 255, 255);
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
            pls[0].Color = cyan;
            //Segunda polilínea
            pts[5] = pts[1].ToPoint2dByPolar(frenteSmall, this.Angle);
            pts[6] = pts[5].ToPoint2dByPolar(ancho * 2 - frenteSmall, this.Angle - Math.PI / 2);
            pts[7] = pts[6].ToPoint2dByPolar(frente / 2 - 1.5 * frenteSmall, this.Angle);
            pts[8] = pts[7].ToPoint2dByPolar(ancho * 2 - frenteSmall, this.Angle + Math.PI / 2);
            pls[1] = RivieraExtender.CreatePolyline(false, pts.SubArray(5, 8));
            pls[1].Color = cyan;
            //Tercera polilínea
            pts[9] = pts[8].ToPoint2dByPolar(frenteSmall, this.Angle).ToPoint2dByPolar(ancho * 2, this.Angle - Math.PI / 2);
            pts[10] = pts[9].ToPoint2dByPolar(ancho * 2 - frenteSmall, this.Angle + Math.PI / 2);
            pts[11] = pts[10].ToPoint2dByPolar(frente / 2 - 1.5 * frenteSmall, this.Angle);
            pts[12] = pts[11].ToPoint2dByPolar(ancho * 2 - frenteSmall, this.Angle - Math.PI / 2);
            pls[2] = RivieraExtender.CreatePolyline(false, pts.SubArray(9, 12));
            pls[2].Color = cyan;
            return pls;
        }
    }
}