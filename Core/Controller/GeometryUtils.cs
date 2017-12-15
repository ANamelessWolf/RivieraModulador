using Autodesk.AutoCAD.DatabaseServices;
using DaSoft.Riviera.Modulador.Core.Model;
using Nameless.Libraries.HoukagoTeaTime.Ritsu.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Core.Controller
{
    public static class GeometryUtils
    {
        /// <summary>
        /// Regens the CAD object as line object
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="line">The line.</param>
        public static void RegenAsLine(this RivieraObject obj, ref Line line)
        {
            if (obj.CADGeometry == null)
                line = new Line(obj.Start.ToPoint3d(), obj.End.ToPoint3d());
            else
            {
                line.StartPoint = obj.Start.ToPoint3d();
                line.EndPoint = obj.End.ToPoint3d();
            }
        }
    }
}
