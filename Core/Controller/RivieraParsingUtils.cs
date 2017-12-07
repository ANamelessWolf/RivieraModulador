using DaSoft.Riviera.Modulador.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Core.Controller
{
    /// <summary>
    /// Defines the collection of functions used to parse Riviera Codes
    /// or Riviera Keys
    /// </summary>
    public static class RivieraParsingUtils
    {
        /***************************************/
        /******* Llaves de Línea de diseño *****/
        /***************************************/
        const String KEY_ENFASIS = "EP";
        const String KEY_BORDEO = "BD";
        const String KEY_ANY = "TODAS";
        /***************************************/
        /**** Llaves para tipo de elemento *****/
        /***************************************/
        const String KEY_PANEL = "P";
        const String KEY_PANEL_PISO = "PP";
        const String KEY_BIOMBO = "B";
        const String KEY_CAJONERA = "C";
        const String KEY_PICHONERA = "PI";
        /// <summary>
        /// Parses a key into a design line.
        /// </summary>
        /// <param name="key">The key to parse.</param>
        /// <returns>The design line</returns>
        public static DesignLine ParseDesignLine(this String key)
        {
            if (key == KEY_ENFASIS)
                return DesignLine.Enfasis_Plus;
            else if (key == KEY_BORDEO)
                return DesignLine.Bordeo;
            else if (key == KEY_ANY)
                return DesignLine.Any;
            else
                return DesignLine.None;
        }
        /// <summary>
        /// Parses a key into a element type
        /// </summary>
        /// <param name="key">The key to parse.</param>
        /// <returns>The element type</returns>
        public static RivieraElementType ParseElementType(this String key)
        {
            if (key == KEY_PANEL)
                return RivieraElementType.Panel;
            else if (key == KEY_PANEL_PISO)
                return RivieraElementType.Panel_Piso;
            else if (key == KEY_BIOMBO)
                return RivieraElementType.Biombo;
            else if (key == KEY_CAJONERA)
                return RivieraElementType.Cajonera;
            else if (key == KEY_PICHONERA)
                return RivieraElementType.Pichonera;
            else
                return RivieraElementType.None;
        }
    }
}
