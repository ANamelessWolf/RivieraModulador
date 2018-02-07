using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.OldModulador.Controller;
using DaSoft.Riviera.OldModulador.Runtime;
using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;

namespace DaSoft.Riviera.OldModulador.Model.Delta
{
    public class RemateGeometry : NamelessObject
    {
        /// <summary>
        /// El padre del remate
        /// </summary>
        public JointObject Parent;
        /// <summary>
        /// El código del remate
        /// </summary>
        public String Code;
        /// <summary>
        /// El id del remate
        /// </summary>
        public new ObjectId Id;
        /// <summary>
        /// El desfazamiento del remate con respecto al suelo
        /// En valor nominal
        /// </summary>
        public int FloorOffset;
        /// <summary>
        /// El punto de inserción del remate
        /// </summary>
        public Point3d InsertionPoint;
        /// <summary>
        /// El angulo de rotación para el remate
        /// </summary>
        public Double Angle;
        /// <summary>
        /// Inicializa una instancia de la clase<see cref="RemateGeometry" /> class.
        /// </summary>
        /// <param name="code">El código del remate</param>
        /// <param name="insPt">El punto de inserción del remate</param>
        /// <param name="angle">El ángulo de rotación para el remate</param>
        /// <param name="parent">El padre del remate</param>
        /// <param name="floorOffset">El desfazamiento del remate con respecto al suelo</param>
        public RemateGeometry(JointObject parent, String code, Point3d insPt, int floorOffset, double angle = 0)
        {
            this.Parent = parent;
            this.Code = code;
            Double r = DaSoft.Riviera.OldModulador.Assets.RIVIERA_CONST.REMATE_FRENTE_M * -1d;
            this.InsertionPoint = new Point3d(insPt.X + r * Math.Cos(angle), insPt.Y + r * Math.Sin(angle), 0);
            this.Angle = angle;
            this.FloorOffset = floorOffset;
        }
        /// <summary>
        /// Regresa una <see cref="System.String" /> que representa la instacia
        /// </summary>
        /// <returns>
        /// Una <see cref="System.String" /> que representa la instacia.
        /// </returns>
        public override string ToString()
        {
            return String.Format("{0} Ang:{1}", this.Code, this.Angle);
        }

        internal Block3D CreateBlock(Transaction tr)
        {
            return new Block3D(tr, this);
        }
    }
}
