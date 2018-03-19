using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DaSoft.Riviera.Modulador.Bordeo.UI
{
    public interface ICanvasBridge
    {
        /// <summary>
        /// Hides the pazo luz.
        /// </summary>
        void HidePazoLuz();
        /// <summary>
        /// Shows the pazo luz.
        /// </summary>
        void ShowPazoLuz();
        /// <summary>
        /// Sets the default.
        /// </summary>
        void SetDefault();
        /// <summary>
        /// Gets the canvas container.
        /// </summary>
        /// <value>
        /// The canvas container.
        /// </value>
        Grid CanvasContainer { get; }
        /// <summary>
        /// Gets the pazo luz.
        /// </summary>
        /// <value>
        /// The pazo luz.
        /// </value>
        BordeoPazLuzItem PazoLuz { get; }
        /// <summary>
        /// Gets the pazo luz.
        /// </summary>
        /// <value>
        /// The pazo luz.
        /// </value>
        BordeoPuenteHorItem HorBridge { get; }
    }
}