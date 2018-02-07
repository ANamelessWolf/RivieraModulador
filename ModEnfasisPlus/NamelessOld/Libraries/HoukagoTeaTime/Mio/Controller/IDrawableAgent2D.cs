using NamelessOld.Libraries.HoukagoTeaTime.Ritsu.Shapes2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.HoukagoTeaTime.Mio.Controller
{
    public interface IDrawableAgent2D
    {
        /// <summary>
        /// Defines the agent geometry
        /// </summary>
        Geometry2D Geometry { get; }
        /// <summary>
        /// Defines the agent name category
        /// </summary>
        String CategoryName { get; }

    }
}
