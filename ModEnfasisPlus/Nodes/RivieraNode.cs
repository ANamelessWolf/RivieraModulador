using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.OldModulador.Model;
using System.Collections.Generic;

namespace DaSoft.Riviera.OldModulador.Nodes
{
    public abstract class RivieraNode
    {
        public readonly Dictionary<ArrowDirection, RivieraConnection> Connections;

        public Vector2d Direction;
        public RivieraNode()
        {
            this.Connections = new Dictionary<ArrowDirection, RivieraConnection>();
        }
    }
}
