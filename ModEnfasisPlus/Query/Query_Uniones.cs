using NamelessOld.Libraries.DB.Misa.Model;
using System;

namespace DaSoft.Riviera.OldModulador.Query
{
    public class Query_Uniones : OracleQuery
    {
        /// <summary>
        /// El número máximo de codigos de unión soportados
        /// </summary>
        public static int MaxItems
        {
            get { return MAX_ELEMENTS; }
        }

        #region Constants
        const String TABLENAME = "DT_UNIONES";
        const String FIELD_HEIGHT = "AM";
        const String FIELD_UNION = "TIPO_UNION";
        const String FIELD_QUANTIFY = "CUANT";
        const String FIELD_COUNT = "CANT";
        const String FIELD_VISIBILITY = "VIS";

        const int MAX_ELEMENTS = 7;


        readonly String[] FIELD_HEIGHTS;
        readonly String[] FIELD_CODES;
        readonly String[] FIELD_COUNTS;
        readonly String[] FIELD_VISIBILITIES;


        #endregion

        /// <summary>
        /// Inicializa el query de uniones
        /// </summary>
        public Query_Uniones()
        {
            FIELD_HEIGHTS
                = new String[]
                {
                    FIELD_HEIGHT + "1_0",
                    FIELD_HEIGHT + "2_90",
                    FIELD_HEIGHT + "3_180",
                    FIELD_HEIGHT + "4_270",
                };
            this.FIELD_CODES = new String[MAX_ELEMENTS];
            this.FIELD_COUNTS = new String[MAX_ELEMENTS];
            this.FIELD_VISIBILITIES = new String[MAX_ELEMENTS];

            for (int i = 1; i <= MAX_ELEMENTS; i++)
            {
                this.FIELD_CODES[i - 1] = FIELD_QUANTIFY + i.ToString();
                this.FIELD_COUNTS[i - 1] = FIELD_COUNT + i.ToString();
                this.FIELD_VISIBILITIES[i - 1] = FIELD_VISIBILITY + "_" + i.ToString();
            }
        }

        /// <summary>
        /// Selecciona todas las descripciones disponibles
        /// </summary>
        /// <returns>El query de selección de descripción</returns>
        public String SelecUniones()
        {
            return String.Format("SELECT * FROM {0}", TABLENAME);
        }
    }
}
