using NamelessOld.Libraries.DB.Misa.Model;
using System;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CODE;
namespace DaSoft.Riviera.OldModulador.Query
{
    public class Query_Mampara : OracleQuery
    {
        #region Constants
        const string TABLENAME = "DT_COD_FREN_ALT_MAM";
        const string TABLENAME_VALORES = "DT_FREN_ALT_MAM";
        const string TABLENAME_NIV = "DT_ALT_NIV_MAM";
        const string FRENTE_TYPE = "FM";
        const string ALTO_TYPE = "AM";

        const string FIELD_CODE = "CODIGO";
        const string FIELD_FRENTE = "FRENTE";
        const string FIELD_ALTO = "ALTO";
        const string FIELD_VREAL = "VREAL";
        const string FIELD_VNOMINAL = "VNOMINAL";
        const string FIELD_TIPO = "TIPO";

        #endregion
        /// <summary>
        /// El query de selección para seleccionar frentes
        /// </summary>
        public String SELECT_FRENTES
        {
            get
            {
                return String.Format(
                    "SELECT {0}, {1}, {2} " +
                    "FROM {3}, {4} " +
                    "WHERE {5} = '{6}' " +
                    "AND " +
                    "{1} = {7} " +
                    "AND " +
                    "{0} = '{8}' " +
                    "GROUP BY {0}, {1}, {2}",
                    FIELD_CODE, FIELD_FRENTE, FIELD_VREAL,
                    TABLENAME, TABLENAME_VALORES,
                    FIELD_TIPO, FRENTE_TYPE,
                    FIELD_VNOMINAL,
                    DT_MAMPARA);
            }
        }
        /// <summary>
        /// El query de selección para seleccionar altos
        /// </summary>
        public String SELECT_ALTOS
        {
            get
            {
                return String.Format(
                    "SELECT {0}, {1}, {2} " +
                    "FROM {3}, {4} " +
                    "WHERE {5} = '{6}' " +
                    "AND " +
                    "{1} = {7} " +
                    "AND " +
                    "{0} = '{8}' " +
                    "GROUP BY {0}, {1}, {2}",
                    FIELD_CODE, FIELD_ALTO, FIELD_VREAL,
                    TABLENAME, TABLENAME_VALORES,
                    FIELD_TIPO, ALTO_TYPE,
                    FIELD_VNOMINAL,
                    DT_MAMPARA);
            }
        }
        /// <summary>
        /// El query de selección de mamparas
        /// </summary>
        public String SelectMamparas()
        {

            String ALIAS_MAMPARA = "M";
            const String ALIAS_FRENTE = "F";
            const String ALIAS_ALTO = "A";
            return
                String.Format(
                    "SELECT {0}.{3}, {0}.{4}, {1}.{6}, {0}.{5}, {2}.{6} " +
                    "FROM {7} {0}, ({8}) {1}, ({9}) {2} " +
                    "WHERE " +
                    "{0}.{4} = {1}.{4} " +
                    "AND " +
                    "{0}.{5} = {2}.{5} " +
                    "ORDER BY {0}.{4}, {0}.{5}",
                ALIAS_MAMPARA, ALIAS_FRENTE, ALIAS_ALTO,
                FIELD_CODE, FIELD_FRENTE, FIELD_ALTO, FIELD_VREAL,
                TABLENAME, SELECT_FRENTES, SELECT_ALTOS);
        }

        /// <summary>
        /// Selecciona todas los niveles configurables
        /// </summary>
        /// <returns>El query de selección de niveles</returns>
        public String SelectNiveles()
        {
            return String.Format("SELECT * FROM {0}", TABLENAME_NIV);
        }
    }
}
