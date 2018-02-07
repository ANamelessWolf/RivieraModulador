using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.OldModulador.Controller;
using DaSoft.Riviera.OldModulador.Model.Delta;
using DaSoft.Riviera.OldModulador.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Mio;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Model;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu;
using System;
using System.Linq;

namespace DaSoft.Riviera.OldModulador.Model
{
    public class DoubleMamparaReportItem
    {
        /// <summary>
        /// Obtiene acceso a la mampara de la izquierda
        /// </summary>
        readonly Mampara Left;
        /// <summary>
        /// Obtiene acceso a la mampara de la derecha
        /// </summary>
        readonly Mampara Right;
        /// <summary>
        /// Obtiene acceso a la unión de las mamparas
        /// </summary>
        readonly JointObject Joint;


        public MamparaReportItem LadoA, LadoB_left, LadoB_Right;

        public DoubleMamparaReportItem(JointObject joint)
        {
            Mampara top;
            RivieraPanelDoubleLocation.FindMampara(joint, out Left, out Right, out top);
            var pArray = joint.PanelArray;
            this.Joint = joint;
            //LadoA
            LadoA = new MamparaReportItem()
            {
                Mampara = String.Format("{0} / {1}", this.Left.Code.AddAcabado(Left), this.Right.Code.AddAcabado(Right)),
                Biombo = null,
                Panels = new System.Collections.Generic.Dictionary<double, PanelRaw>()
            };
            if (pArray.DobleFront is RivieraPanel54)
            {
                LadoA.Panels.Add(0, (pArray.DobleFront as RivieraPanel54).LowerRaw);
                LadoA.Panels.Add((pArray.DobleFront as RivieraPanel54).UpperRaw.Height, (pArray.DobleFront as RivieraPanel54).UpperRaw);
            }
            else
            {
                LadoA.Panels.Add(0, pArray.DobleFront.Raw);
            }
            //LadoB
            LadoB_left = new MamparaReportItem()
            {
                Mampara = this.Left.Code,
                Biombo = null,
                Panels = new System.Collections.Generic.Dictionary<double, PanelRaw>()
            };
            LadoB_Right = new MamparaReportItem()
            {
                Mampara = this.Right.Code,
                Biombo = null,
                Panels = new System.Collections.Generic.Dictionary<double, PanelRaw>()
            };
            //Biombo
            if (pArray.Biombo != null)
                LadoA.Panels.Add(pArray.Biombo.Raw.Height, pArray.Biombo.Raw);
            if (pArray.DobleBottom != null)
            {
                LadoB_left.Panels.Add(0, pArray.Left.Raw);
                LadoB_Right.Panels.Add(0, pArray.Right.Raw);
                var s = App.DB.Panel_Size.Where(x => x.Code == pArray.Left.Raw.Code.Substring(0, 6) && x.Alto == pArray.Left.Raw.Code.Substring(8, 2));
                var result = s.FirstOrDefault();
                LadoB_left.Panels.Add(result.Real.Alto.ConvertUnits(Unit_Type.mm, Unit_Type.m), pArray.DobleBottom.Raw);
            }
            else
            {
                LadoB_left.Panels.Add(0, pArray.Left.Raw);
                LadoB_Right.Panels.Add(0, pArray.Right.Raw);
            }
        }
        /// <summary>
        /// Crea las etiquetas de reportes de las mamparas seleccionadas
        /// </summary>
        /// <param name="sideA">El texto de la etiqueta para el lado A</param>
        /// <param name="sideB">El texto de la etiqueta para el lado B</param>
        /// <param name="sideATag">La salida es la etiqueta del ladao A</param>
        /// <param name="sideBTag">La salida es la etiqueta del ladao B</param>
        public void CreateReportTags(String sideA, String sideB, out DBText sideATag, out DBText sideBTag, out DBText sideCTag)
        {
            String sideC = sideB.Replace("B", "C");
            DBText[] res;
            double ancho = (this.Left.Ancho + 0.05);
            if (App.Riviera.Units == DaNTeUnits.Imperial)
                ancho = (this.Left.Ancho).ConvertUnits(Unit_Type.m, Unit_Type.inches);
            Point2d start = this.Left.Start.GetDistanceTo(this.Joint.Start) < this.Left.End.GetDistanceTo(this.Joint.Start) ? this.Left.End : this.Left.Start,
                    end = this.Right.Start.GetDistanceTo(this.Joint.Start) < this.Right.End.GetDistanceTo(this.Joint.Start) ? this.Right.End : this.Right.Start;
            Double ang = start.GetVectorTo(end).Angle;
            res = new DBText[]
            {
                Drawer.CreateText(sideA, ang, start.MiddlePointTo(end).ToPoint2dByPolar(ancho, ang - Math.PI / 2).ToPoint3d(), ancho / 2, new Margin()),
                Drawer.CreateText(sideB, ang, this.Left.Start.MiddlePointTo(this.Left.End).ToPoint2dByPolar(ancho/2, ang + Math.PI / 2).ToPoint3d(), ancho / 2, new Margin()),
                Drawer.CreateText(sideC, ang, this.Right.Start.MiddlePointTo(this.Right.End).ToPoint2dByPolar(ancho/2, ang + Math.PI / 2).ToPoint3d(), ancho / 2, new Margin())
            };
            for (int i = 0; i < res.Length; i++)
                res[i].Position -= new Vector3d((res[i].GeometricExtents.MaxPoint.X - res[i].GeometricExtents.MinPoint.X) / 2d, 0, 0);
            sideATag = res[0];
            sideBTag = res[1];
            sideCTag = res[2];
        }

        internal double GetFrente()
        {
            Point2d start = this.Left.Start.GetDistanceTo(this.Joint.Start) > this.Left.End.GetDistanceTo(this.Joint.Start) ? this.Left.End : this.Left.Start,
        end = this.Right.Start.GetDistanceTo(this.Joint.Start) < this.Right.End.GetDistanceTo(this.Joint.Start) ? this.Left.End : this.Left.Start;
            return start.GetDistanceTo(end);
        }
    }
}
