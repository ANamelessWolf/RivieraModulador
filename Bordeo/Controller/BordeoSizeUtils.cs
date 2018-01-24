using DaSoft.Riviera.Modulador.Bordeo.Model;
using DaSoft.Riviera.Modulador.Bordeo.Model.Enities;
using DaSoft.Riviera.Modulador.Bordeo.Runtime;
using DaSoft.Riviera.Modulador.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DaSoft.Riviera.Modulador.Bordeo.Assets.Strings;
using static DaSoft.Riviera.Modulador.Bordeo.Assets.Codes;
namespace DaSoft.Riviera.Modulador.Bordeo.Controller
{
    /// <summary>
    /// Defines the bordeo class utils
    /// Size Utility
    /// </summary>
    public static partial class BordeoUtils
    {
        /// <summary>
        /// Gets the size of the linear drawing.
        /// </summary>
        /// <param name="nominal">The nominal value.</param>
        /// <returns>The linear double value</returns>
        public static Double GetLinearDrawingSize(this Double nominal)
        {
            double val;
            if (nominal == 18d)
                val = 0.457d;
            else if (nominal == 24d)
                val = 0.61d;
            else if (nominal == 30d)
                val = 0.762d;
            else if (nominal == 36d)
                val = 0.915d;
            else if (nominal == 42d)
                val = 1.067d;
            else if (nominal == 48d)
                val = 1.22d;
            else
                throw new BordeoException(String.Format(ERR_UNKNOWN_DRAWING_SIZE, nominal));
            return val;
        }
        /// <summary>
        /// Gets the size of the Panel L 90 drawing.
        /// </summary>
        /// <param name="nominal">The nominal value.</param>
        /// <returns>The L double value</returns>
        public static Double GetPanel90DrawingSize(this Double nominal)
        {
            double val;
            if (nominal == 18d)
                val = 0.3748d;
            else if (nominal == 24d)
                val = 0.5278d;
            else if (nominal == 30d)
                val = 0.67880210d;
            else if (nominal == 36d)
                val = 0.8328d;
            else if (nominal == 42d)
                val = 0.98380210d;
            else if (nominal == 48d)
                val = 1.1378d;
            else
                throw new BordeoException(String.Format(ERR_UNKNOWN_DRAWING_SIZE, nominal));
            return val;
        }
        /// <summary>
        /// Gets the size of the Panel L 135 drawing.
        /// </summary>
        /// <param name="nominal">The nominal value.</param>
        /// <returns>The L double value</returns>
        public static Double GetPanel135DrawingSize(this Double nominal)
        {
            double val;
            if (nominal == 18d)
                val = 0.42295164d;
            else if (nominal == 24d)
                val = 0.57595165d;
            else if (nominal == 30d)
                val = 0.72794954d;
            else if (nominal == 36d)
                val = 0.88095165d;
            else if (nominal == 42d)
                val = 1.03294954d;
            else if (nominal == 48d)
                val = 1.18595165d;
            else
                throw new BordeoException(String.Format(ERR_UNKNOWN_DRAWING_SIZE, nominal));
            return val;
        }

        public static BordeoPanelHeight GetHeights(this IEnumerable<BordeoPanel> panels)
        {
            String panelHeightString = String.Empty;
            foreach (BordeoPanel panel in panels)
                panelHeightString += panel.PanelSize.Alto.Nominal == 27 ? "PB" : "Ps";
            return panelHeightString.GetHeight();
        }
        public static BordeoPanelHeight GetHeights(this IEnumerable<BordeoLPanel> panels)
        {
            String panelHeightString = String.Empty;
            foreach (BordeoLPanel panel in panels)
                panelHeightString += panel.PanelSize.Alto.Nominal == 27 ? "PB" : "Ps";
            return panelHeightString.GetHeight();
        }
        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <param name="panelHeightString">The panel height string.</param>
        /// <returns>The panel height</returns>
        public static BordeoPanelHeight GetHeight(this String panelHeightString)
        {
            BordeoPanelHeight value;
            switch (panelHeightString)
            {
                case "PBPs":
                    value = BordeoPanelHeight.NormalMini;
                    break;
                case "PBPsPB":
                    value = BordeoPanelHeight.NormalMiniNormal;
                    break;
                case "PBPsPsPs":
                    value = BordeoPanelHeight.NormalThreeMini;
                    break;
                case "PBPsPs":
                    value = BordeoPanelHeight.NormalTwoMinis;
                    break;
                case "PBPBPB":
                    value = BordeoPanelHeight.ThreeNormals;
                    break;
                case "PBPBPs":
                    value = BordeoPanelHeight.TwoNormalOneMini;
                    break;
                case "PBPB":
                    value = BordeoPanelHeight.TwoNormals;
                    break;
                default:
                    value = BordeoPanelHeight.None;
                    break;
            }
            return value;
        }
        /// <summary>
        /// Gets the panel measure.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="sizes">The sizes.</param>
        /// <param name="selectedHeights">The selected heights.</param>
        /// <param name="selectedFronts">The selected fronts.</param>
        /// <returns></returns>
        public static List<RivieraMeasure> GetPanelMeasure(this String code, Dictionary<String, ElementSizeCollection> sizes, RivieraSize[] selectedHeights, params RivieraSize[] selectedFronts)
        {
            List<RivieraMeasure> measure = new List<RivieraMeasure>();
            var heights = selectedHeights;
            RivieraSize startFront = selectedFronts[0];
            RivieraSize endFront = selectedFronts.Length > 1 ? selectedFronts[1] : default(RivieraSize);
            foreach (RivieraSize height in heights)
                switch (code)
                {
                    case CODE_PANEL_RECTO:
                        measure.Add(sizes[CODE_PANEL_RECTO].Sizes.Select(x => x as PanelMeasure).
                            FirstOrDefault(y => y.Alto == height && y.Frente == startFront));
                        break;
                    case CODE_PANEL_90:
                        measure.Add(sizes[CODE_PANEL_90].Sizes.Select(x => x as LPanelMeasure).
                            FirstOrDefault(y => y.Alto == height && y.FrenteStart == startFront && y.FrenteEnd == endFront));
                        break;
                    case CODE_PANEL_135:
                        measure.Add(sizes[CODE_PANEL_135].Sizes.Select(x => x as LPanelMeasure).
                            FirstOrDefault(y => y.Alto == height && y.FrenteStart == startFront && y.FrenteEnd == endFront));
                        break;
                }
            return measure;
        }
    }
}
