using DaSoft.Riviera.Modulador.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DaSoft.Riviera.Modulador.Bordeo.Assets.Codes;
using static DaSoft.Riviera.Modulador.Core.Assets.CONST;
using static DaSoft.Riviera.Modulador.Bordeo.Assets.Constants;
using static DaSoft.Riviera.Modulador.Core.Controller.AutoCADUtils;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System.Collections;
using DaSoft.Riviera.Modulador.Bordeo.Controller;
using Nameless.Libraries.HoukagoTeaTime.Ritsu.Utils;
using Nameless.Libraries.HoukagoTeaTime.Mio.Utils;

namespace DaSoft.Riviera.Modulador.Bordeo.Model.Enities
{
    public class BordeoStation : RivieraObject, IEnumerable<RivieraObject>
    {
        /// <summary>
        /// The collection of panel stacks asociated to this station
        /// </summary>
        public IEnumerable<RivieraObject> Members;
        /// <summary>
        /// The list of lengths defined in the given station
        /// </summary>
        public List<Double> Lengths;
        /// <summary>
        /// The station geometry
        /// </summary>
        public Polyline StationGeometry;
        /// <summary>
        /// Gets the geometry that stores the riviera extended data.
        /// </summary>
        /// <value>
        /// The CAD geometry
        /// </value>
        public override Entity CADGeometry => this.StationGeometry;
        /// <summary>
        /// Gets the riviera object available record keys.
        /// </summary>
        /// <value>
        /// The dictionary XRecord Keys.
        /// </value>
        public override string[] Keys => new String[0];

        /// <summary>
        /// Initializes a new instance of the <see cref="BordeoStation"/> class.
        /// </summary>
        /// <param name="code">The riviera code.</param>
        /// <param name="size">The riviera element size.</param>
        /// <param name="start">The riviera start point or insertion point.</param>
        public BordeoStation(Point3d start, IEnumerable<RivieraObject> members) :
            base(BordeoUtils.GetRivieraCode(CODE_STATION), new Ameasurable(), start)
        {
            this.Members = members;
            this.Lengths = new List<double>();
            int i = 0;
            this.Members.ToList().ForEach(x => this.Children.Add(i++.ToString(), x.Handle.Value));
        }
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<RivieraObject> GetEnumerator()
        {
            return this.Members.GetEnumerator();
        }
        /// <summary>
        /// Regens this instance geometry <see cref="P:DaSoft.Riviera.Modulador.Core.Model.RivieraObject.CADGeometry" />.
        /// </summary>
        public override void Regen()
        {
            var first = this.Members.FirstOrDefault();
            Point2d location;
            Double bulge;
            if (this.StationGeometry == null)
            {
                this.StationGeometry = new Polyline();
                this.StationGeometry.Layer = LAYER_RIVIERA_STATION;
                this.GetVertex(first, 0, out location, out bulge);
                this.StationGeometry.AddVertexAt(0, location, bulge, 0, 0);
            }
            else
                while (this.StationGeometry.NumberOfVertices > 1)
                    this.StationGeometry.RemoveVertexAt(this.StationGeometry.NumberOfVertices - 1);
            //Draw the polyline
            foreach (RivieraObject stack in Members)
                if (stack.CADGeometry is Polyline)
                    for (int i = 1; i < (stack.CADGeometry as Polyline).NumberOfVertices; i++)
                        this.AddVertex(stack, i);
                else
                    this.AddVertex(stack, 1);
            //Check if the las vertex is not missing
            var last = this.Members.LastOrDefault();
            if (last.End.GetDistanceTo(this.StationGeometry.EndPoint.ToPoint2d()) > 0)
                this.StationGeometry.AddVertexAt(this.StationGeometry.NumberOfVertices, last.End, 0, 0, 0);
            for (int i = 0; i < this.StationGeometry.NumberOfVertices; i++)
                if (this.StationGeometry.GetSegmentType(i) == SegmentType.Line)
                    this.Lengths.Add(i == 0 || i == this.StationGeometry.NumberOfVertices - 2 ? (int)((this.StationGeometry.GetLineSegment2dAt(i).Length - 0.2228) *1000) : (int)((this.StationGeometry.GetLineSegment2dAt(i).Length - 0.1396)*1000));
        }
        /// <summary>
        /// Updates the direction.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="pastDirection">The past direction.</param>
        /// <param name="currentDirection">The current direction.</param>
        private void UpdateDirection(Point2d location, ref double pastDirection, ref double currentDirection)
        {
            if (this.StationGeometry.NumberOfVertices == 1)
            {
                pastDirection = this.StationGeometry.GetPoint2dAt(0).GetVectorTo(location).Angle;
                currentDirection = pastDirection;
            }
            else if (this.StationGeometry.NumberOfVertices > 2)
            {
                pastDirection = currentDirection;
                currentDirection = this.StationGeometry.GetPoint2dAt(this.StationGeometry.NumberOfVertices - 1).
                    GetVectorTo(location).Angle;
            }
        }
        /// <summary>
        /// Adds the vertex.
        /// </summary>
        /// <param name="stack">The stack to extract its vertex.</param>
        /// <param name="index">The index.</param>
        private void AddVertex(RivieraObject stack, int index)
        {
            Point2d location;
            Double bulge, pastDirection = 0, currentDirection = 0;
            this.GetVertex(stack, index, out location, out bulge);
            //this.UpdateDirection(location, ref pastDirection, ref currentDirection);
            if (this.StationGeometry.GetBulgeAt(this.StationGeometry.NumberOfVertices-1)!= bulge )
                this.StationGeometry.AddVertexAt(this.StationGeometry.NumberOfVertices, location, bulge, 0, 0);
        }
        /// <summary>
        /// Gets the vertex.
        /// </summary>
        /// <param name="stack">The stack.</param>
        /// <param name="index">The index.</param>
        /// <param name="location">The location.</param>
        /// <param name="bulge">The bulge.</param>
        private void GetVertex(RivieraObject stack, int index, out Point2d location, out double bulge)
        {
            if (stack.CADGeometry is Polyline)
            {
                location = (stack.CADGeometry as Polyline).GetPoint2dAt(index);
                bulge = (stack.CADGeometry as Polyline).GetBulgeAt(index);
            }
            else
            {
                location = index == 0 ? (stack.CADGeometry as Line).StartPoint.ToPoint2d() :
                    (stack.CADGeometry as Line).EndPoint.ToPoint2d();
                bulge = 0;
            }

        }
        /// <summary>
        /// Draws this instance
        /// </summary>
        /// <param name="tr">The active transaction.</param>
        /// <returns></returns>
        protected override ObjectIdCollection DrawContent(Transaction tr)
        {
            BlockTableRecord model = tr.GetModelSpace(OpenMode.ForWrite);
            ObjectIdCollection ids = new ObjectIdCollection();
            if (this.CADGeometry == null)
                this.Regen();
            //Se dibuja o actualizá la línea
            if (this.Id.IsValid)
            {
                this.StationGeometry.Id.GetObject(OpenMode.ForWrite);
                this.Regen();
            }
            else
                this.StationGeometry.Draw(model, tr);
            return ids;
        }
        /// <summary>
        /// Gets the riviera object end point.
        /// </summary>
        /// <returns>
        /// The riviera end point
        /// </returns>
        protected override Point2d GetEndPoint()
        {
            return this.StationGeometry.GetPoint2dAt(this.StationGeometry.NumberOfVertices - 1);
        }
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Members.GetEnumerator();
        }
    }
}
