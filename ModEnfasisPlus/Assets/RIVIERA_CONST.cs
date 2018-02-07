using System;

namespace DaSoft.Riviera.OldModulador.Assets
{
    public static class RIVIERA_CONST
    {
        /// <summary>
        /// El factor de escala imperial
        /// </summary>
        public const Double IMPERIAL_FACTOR = 39.3700787401575d;
        /// <summary>
        /// La tamaño de un nivel en pixeles
        /// </summary>
        public const Double NIVEL_SIZE_PX = 150d;
        /// <summary>
        /// La tamaño de un nivel en pixeles
        /// </summary>
        public const Double BIOMBO_SIZE_PX = 60d;
        /// <summary>
        /// La offset de piso a panel en pixeles
        /// </summary>
        public const Double FLOOR_OFFSET_PX = 20d;
        /// <summary>
        /// La offset de piso en M
        /// </summary>
        public const Double FLOOR_OFFSET_M = 0.1480d;
        /// <summary>
        /// El ancho de la mampara en metros
        /// </summary>
        public const Double ANCHO_M = 0.0820d;
        /// <summary>
        /// El ancho de la mampara en valor nominal
        /// </summary>
        public const Double MAM_ANCHO_NOM = 3d;
        /// <summary>
        /// El ancho de la mampara en milimetros
        /// </summary>
        public const Double MAM_ANCHO_MM = 82d;
        /// <summary>
        /// El ancho de inserción del arreglo de mamparas
        /// </summary>
        public const Double STACK_ANCHO_MM = 34.3d;
        /// <summary>
        /// El ancho de la mampara en valor nominal
        /// </summary>
        public const Double PAN_ANCHO_NOM = 0.5d;
        /// <summary>
        /// El ancho de la mampara en milimetros
        /// </summary>
        public const Double PAN_ANCHO_MM = 12d;

        public const Double PICH_ANCHO_SMALL_MM = 41d;
        public const Double PICH_ANCHO_MADERA_MM = 9.5d;
        public const Double PICH_FRENTE_SMALL_MM = 19d;
        public const Double PICH_ANCHO_LARGE_MM = 249d;
        public const Double PICH_ANCHO_HALF_MM = 228.5d;
        public const Double PICH_FRENTE_HALF_MM = 429d;

        /// <summary>
        /// El offset para la inserción de panel en metros
        /// </summary>
        public const Double PAN_OFFSET_M = 0.01d / 2d;
        /// <summary>
        /// El frente del remate en metros en metros
        /// </summary>
        public const Double REMATE_FRENTE_M = 0.012d;
        /// <summary>
        /// El factor de zoom de la ventana
        /// </summary>
        public const Double ZOOM = 2.5d;
        /// <summary>
        /// El tamaño de las flechas de selección
        /// en metros
        /// </summary>
        public const Double ARROW_SIZE = 0.0250d;
        /// <summary>
        /// El tamaño del área de busqueda de la flecha
        /// en metros
        /// </summary>
        public const Double ARROW_SEARCH_AREA = 0.036d;
        public const Double ANCHO_HALF_MAMPARA_M = 0.0343d;
        /// <summary>
        /// Altura normal
        /// </summary>
        public const String H1 = "A";
        /// <summary>
        /// Altura menor H1
        /// </summary>
        public const String H2 = "AM";
        /// <summary>
        /// Altura menor H2
        /// </summary>
        public const String H3 = "AMM";
        /// <summary>
        /// Altura menor H3
        /// </summary>
        public const String H4 = "AMMM";
        /// <summary>
        /// Mampara uno
        /// </summary>
        public const String M1 = "AM1";
        /// <summary>
        /// Mampara dos
        /// </summary>
        public const String M2 = "AM2";
        /// <summary>
        /// Mampara tres
        /// </summary>
        public const String M3 = "AM3";
        /// <summary>
        /// Mampara cuatro
        /// </summary>
        public const String M4 = "AM4";
        /// <summary>
        /// Union tipo I
        /// </summary>
        public const String UNION_I = "I";
        /// <summary>
        /// Union tipo L
        /// </summary>
        public const String UNION_L = "L";
        /// <summary>
        /// Union tipo T
        /// </summary>
        public const String UNION_T = "T";
        /// <summary>
        /// Union tipo X
        /// </summary>
        public const String UNION_X = "X";
        /// <summary>
        /// El nombre de la capa de unión
        /// </summary>
        public const String LAYER_UNION = "Id Unión";
        /// <summary>
        /// Permite realizar una rotación en la geometría seleccionada
        /// </summary>
        public const String ROTATE_GEOMETRY_XRECORD = "RotateGeometry";

    }
}
