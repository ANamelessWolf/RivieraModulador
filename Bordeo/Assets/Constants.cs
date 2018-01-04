using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Bordeo.Assets
{
    public static class Constants
    {
        //public const String KEY_FRONT = "Front";
        //public const String KEY_BACK = "Back";
        //public const String KEY_LEFT_90 = "Left90";
        //public const String KEY_LEFT_135 = "Left135";
        //public const String KEY_RIGHT_90 = "Right90";
        //public const String KEY_RIGHT_135 = "Right135";
        //public const String KEY_EXTRA = "ExtraContent";

        public const String FOLDER_NAME_BLOCKS_BORDEO = "Bordeo";
        /// <summary>
        /// Define la dirección del bloque a la izquierda
        /// </summary>
        public const String BLOCK_DIR_LFT = "LFT";
        /// <summary>
        /// Define la dirección del bloque a la derecha
        /// </summary>
        public const String BLOCK_DIR_RGT = "RGT";
        /// <summary>
        /// Define el prefijo de instancia de un bloque en L
        /// con dirección
        /// </summary>
        public const String PREFIX_BLOCK_INST = "SPACE_{0}{1}";
        /// <summary>
        /// Define el prefijo de contenido de un bloque 2D o 3D en L
        /// </summary>
        public const String PREFIX_BLOCK_CONT = "CONTENT_{0}{1}{2}";
        /// <summary>
        /// Define el prefijo de instancia de un bloque variante en L
        /// </summary>
        public const String PREFIX_BLOCK_VAR__INST = "SPACE_VAR_{0}{1}";
        /// <summary>
        /// Define el prefijo de contenido de un bloque variante 2D o 3D en L
        /// </summary>
        public const String PREFIX_BLOCK_VAR_CONT = "CONTENT_VAR_{0}{1}{2}";
    }
}
