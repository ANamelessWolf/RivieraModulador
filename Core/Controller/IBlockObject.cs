using Autodesk.AutoCAD.DatabaseServices;
using DaSoft.Riviera.Modulador.Core.Model;
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
        /// Gets the name of the block.
        /// </summary>
        String BlockName { get; }
        /// <summary>
        /// Gets or sets the block manager.
        /// </summary>
        /// <value>
        /// The block manager.
        /// </value>
        RivieraBlock Block { get; }
        /// <summary>
        /// Updates the block position.
        /// </summary>
        /// <param name="tr">The tr.</param>
        /// <param name="blockRef">The block reference.</param>
        void UpdateBlockPosition(Transaction tr, BlockReference blockRef);

    }
}
