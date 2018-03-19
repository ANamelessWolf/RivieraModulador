using DaSoft.Riviera.Modulador.Bordeo.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static DaSoft.Riviera.Modulador.Core.Assets.CONST;
namespace DaSoft.Riviera.Modulador.Bordeo.Controller
{
  public static  class CanvasBridgeUtility
    {
        /// <summary>
        /// Sets the default data to the canvas bridge controller.
        /// </summary>
        /// <param name="cbridge">The canvas cbridge.</param>
        public static void SetCanvasBridgeDefault(this ICanvasBridge cbridge)
        {
            cbridge.CanvasContainer.Children.OfType<FrameworkElement>().
                Where(x => x is BordeoPuenteItem || x is BordeoPuenteHorItem).
                Select(x => x as IBridgeItem).ToList().
                        ForEach(x =>
                        {
                            x.UpdateSize(KEY_FRONT, 24d);
                            x.UpdateSize(KEY_HEIGHT, 24d);
                            x.UpdateSize(KEY_DEPHT, 18d);
                            x.SetAcabado("01");
                        });
            cbridge.CanvasContainer.Children.OfType<FrameworkElement>().
                Where(x => x is BordeoPazLuzItem).
                Select(x => x as IBridgeItem).ToList().
                ForEach(x =>
                {
                    x.UpdateSize(KEY_FRONT, 24d);
                    x.UpdateSize(KEY_DEPHT, 48d);
                    x.SetAcabado("01");
                });
        }
        /// <summary>
        /// Hides the pazo luz.
        /// </summary>
        /// <param name="cbridge">The cbridge.</param>
        public static void HideCanvasBridgePazoLuz(this ICanvasBridge cbridge)
        {
            cbridge.PazoLuz.Visibility = Visibility.Collapsed;
        }
        /// <summary>
        /// Shows the pazo luz.
        /// </summary>
        /// <param name="cbridge">The cbridge.</param>
        public static void ShowCanvasBridgePazoLuz(this ICanvasBridge cbridge)
        {
            cbridge.PazoLuz.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Hides the Bridge Horizontal.
        /// </summary>
        /// <param name="cbridge">The cbridge.</param>
        public static void HideCanvasBridgeBridgeHor(this ICanvasBridge cbridge)
        {
            cbridge.HorBridge.Visibility = Visibility.Collapsed;
        }
        /// <summary>
        /// Shows the Bridge Horizontal.
        /// </summary>
        /// <param name="cbridge">The cbridge.</param>
        public static void ShowCanvasBridgeBridgeHor(this ICanvasBridge cbridge)
        {
            cbridge.HorBridge.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Loads the canvas bridge.
        /// </summary>
        /// <param name="cbridge">The cbridge.</param>
        public static void LoadCanvasBridge(this ICanvasBridge cbridge)
        {
            cbridge.HideCanvasBridgePazoLuz();
            cbridge.HideCanvasBridgeBridgeHor();
            cbridge.SetCanvasBridgeDefault();
        }
    }
}
