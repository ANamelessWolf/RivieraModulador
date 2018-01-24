using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Core.Assets
{
    /// <summary>
    /// Defines the application commands
    /// </summary>
    public static class CMDS
    {
        /*******************************/
        /**** Commandos Applicación ****/
        /*******************************/
        public const String START = "IniciarRivieraApp";
        public const String INIT_APP_UI = "IniciarRivieraAppUI";
        public const String CONFIG_UI = "IniciarConfiguraciones";
        public const String SWAP_3D_MODE = "Swap3DMode";
        /***************************/
        /***** Commandos Bordeo ****/
        /***************************/
        public const String BORDEO_SOW_START = "BordeoSowStart";
        public const String BORDEO_SOW_CONTINUE = "BordeoSowContinue";
        public const String BORDEO_EDIT_PANELS = "BordeoPanelEditor";
        public const String BORDEO_PANEL_COPY_PASTE = "BordeoCopyPastePanel";
    }
}
