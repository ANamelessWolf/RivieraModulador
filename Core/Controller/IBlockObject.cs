using Nameless.Libraries.HoukagoTeaTime.Mio.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Core.Controller
{
    /// <summary>
    /// Implements a Riviera Object represented with a geometry
    /// Block Reference
    /// </summary>
    public interface IBlockObject
    {
        /// <summary>
        /// Riviera object 2D block file
        /// </summary>
        /// <value>
        /// The 2D block file.
        /// </value>
        FileInfo BlockFile2d { get; }
        /// <summary>
        /// Riviera object 3D block file
        /// </summary>
        /// <value>
        /// The 3D block file.
        /// </value>
        FileInfo BlockFile3d { get; }
        /// <summary>
        /// Gets the spacename.
        /// </summary>
        /// <value>
        /// The spacename.
        /// </value>
        String Spacename { get; }
        /// <summary>
        /// Gets or sets the block manager.
        /// </summary>
        /// <value>
        /// The block manager.
        /// </value>
        AutoCADBlock Block { get; set; }
    }
}
