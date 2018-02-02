using Autodesk.AutoCAD.DatabaseServices;
using DaSoft.Riviera.Modulador.Bordeo.Model;
using DaSoft.Riviera.Modulador.Core.Model;
using DaSoft.Riviera.Modulador.Core.Model.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Bordeo.Controller
{
    /// <summary>
    /// Defines a bordeo stack that has panels
    /// that can be styled.
    /// </summary>
    public interface IBordeoPanelStyler
    {
        /// <summary>
        /// Gets or sets the riviera bordeo code.
        /// </summary>
        /// <value>
        /// The riviera bordeo code.
        /// </value>
        String RivieraBordeoCode { get; }
        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        RivieraMeasure BordeoPanelSize { get; }
        /// <summary>
        /// Gets or sets the heights.
        /// </summary>
        /// <value>
        /// The heights.
        /// </value>
        BordeoPanelHeight Height { get; }
        /// <summary>
        /// Gets or sets the acabados lado a.
        /// </summary>
        /// <value>
        /// The acabados lado a.
        /// </value>
        IEnumerable<RivieraAcabado> AcabadosLadoA { get; }
        /// <summary>
        /// Gets or sets the acabados lado b.
        /// </summary>
        /// <value>
        /// The acabados lado b.
        /// </value>
        IEnumerable<RivieraAcabado> AcabadosLadoB { get; }
        /// <summary>
        /// Updates the panel stack.
        /// </summary>
        /// <param name="newHeight">The new height.</param>
        /// <param name="acabadoLadoA">The acabado lado a.</param>
        /// <param name="acabadosLadoB">The acabados lado b.</param>
        void UpdatePanelStack(BordeoPanelHeight newHeight, String[] acabadoLadoA, String[] acabadosLadoB);
        /// <summary>
        /// Gets the default moving front.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <returns>The default moving front(FRENTE) in Nominal value</returns>
        string GetDefaultMovingFront(ArrowDirection direction);
        /// <summary>
        /// Gets the move direction.
        /// </summary>
        /// <param name="tr">The active transaction.</param>
        /// <returns>The picked arrow direction</returns>
        ArrowDirection GetMoveDirection();
        /// <summary>
        /// Moves the specified panel.
        /// </summary>
        /// <param name="tr">The active transaction.</param>
        /// <param name="direction">The picked direction.</param>
        /// <param name="front">The riviera size front.</param>
        void Move(Transaction tr, ArrowDirection direction, RivieraSize front);
    }
}
