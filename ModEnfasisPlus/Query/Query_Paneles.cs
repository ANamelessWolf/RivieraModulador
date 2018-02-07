using NamelessOld.Libraries.DB.Misa.Model;
using System;
namespace DaSoft.Riviera.OldModulador.Query
{
    public class Query_Paneles : OracleQuery
    {
        #region Constants
        const string TABLENAME = "DT_COD_FREN_ALT_PAN";
        const string TABLENAME_VALORES = "DT_FREN_ALT_MAM";
        const string TABLENAME_DESC = "DT_COD_DESC";
        const string TABLENAME_NIV = "DT_ALT_NIV_PAN";
        const string TABLENAME_RULES = "DT_COD_RULES_PAN";
        const string FRENTE_TYPE = "FG";
        const string ALTO_TYPE = "AG";
        const string FIELD_CODE = "CODIGO";
        const string FIELD_FRENTE = "FRENTE";
        const string FIELD_ALTO = "ALTO";
        const string FIELD_VREAL = "VREAL";
        const string FIELD_VNOMINAL = "VNOMINAL";
        const string FIELD_TIPO = "TIPO";
        const string FIELD_N1 = "NIVEL1";
        const string FIELD_N2 = "NIVEL2";
        const string FIELD_N3 = "NIVEL3";
        const string FIELD_N4 = "NIVEL4";

        #endregion

        /// <summary>
        /// El query de selección para seleccionar frentes
        /// </summary>
        public String SELECT_FRENTES
        {
            get
            {
                return String.Format(
                    "SELECT " +
                        "P.{0}, P.{1}, S.{2} " +
                    "FROM " +
                        "{3} P, " +
                        "{4} S " +
                    "WHERE " +
                        "S.{5} = '{6}' " +
                        "AND " +
                        "P.{1} = S.{7} ",
                    FIELD_CODE, FIELD_FRENTE, FIELD_VREAL,
                    TABLENAME, TABLENAME_VALORES,
                    FIELD_TIPO, FRENTE_TYPE,
                    FIELD_VNOMINAL);
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
                    "SELECT " +
                        "P.{0}, P.{1}, S.{2} " +
                    "FROM " +
                        "{3} P, " +
                        "{4} S " +
                    "WHERE " +
                        "S.{5} = '{6}' " +
                        "AND " +
                        "P.{1} = S.{7} ",
                    FIELD_CODE, FIELD_ALTO, FIELD_VREAL,
                    TABLENAME, TABLENAME_VALORES,
                    FIELD_TIPO, ALTO_TYPE,
                    FIELD_VNOMINAL);
            }
        }
        /// <summary>
        /// Checa si un código existe en la tabla de paneles,
        /// Este Query se usa en un WHERE y requiere de un alias de la tabla de frentes y altos
        /// </summary>
        /// <param name="fAlias">El frente del alias</param>
        /// <param name="hAlias">La altura del alias</param>
        /// <returns>El query de validación</returns>
        public String CodeIsValid(String fAlias, String hAlias)
        {
            return String.Format(
                "(SELECT " +
                    "COUNT({3}) " +
                "FROM " +
                    "{2} " +
                "WHERE " +
                    "{4} = {0}.{4} " +
                    "AND " +
                    "{5} = {1}.{5} " +
                    "AND " +
                    "{3} = {1}.{3}" +
                ")>0",
                fAlias, hAlias,
                TABLENAME,
                FIELD_CODE, FIELD_FRENTE, FIELD_ALTO);
        }
        /// <summary>
        /// Realiza la selección de paneles
        /// </summary>
        /// <returns>El query de selección de paneles</returns>
        public String SelectPaneles()
        {
            return String.Format(
                "SELECT H.{0}, F.{1}, F.{2} F_REAL, H.{3}, H.{2} A_REAL " +
                "FROM " +
                    "({4}) H " +
                "INNER JOIN " +
                    "({5}) F " +
                "ON " +
                "H.{0} = F.{0} " +
                "AND " +
                "{6} " +
                "GROUP BY H.{0}, F.{1}, F.{2}, H.{3}, H.{2} " +
                "ORDER BY H.{0}, F.{1}, H.{3}",
                FIELD_CODE, FIELD_FRENTE, FIELD_VREAL, FIELD_ALTO,
                SELECT_ALTOS,
                SELECT_FRENTES,
                CodeIsValid("F", "H"));
        }
        /// <summary>
        /// Selecciona todas las descripciones disponibles
        /// </summary>
        /// <returns>El query de selección de descripción</returns>
        public String SelectDescription()
        {
            return String.Format("SELECT * FROM {0}", TABLENAME_DESC);
        }
        /// <summary>
        /// Selecciona todas los niveles configurables
        /// </summary>
        /// <returns>El query de selección de niveles</returns>
        public String SelectNiveles()
        {
            return String.Format("SELECT * FROM {0}", TABLENAME_NIV);
        }

        /// <summary>
        /// Selecciona todas las restricciones de nivel asociadas a un código
        /// </summary>
        /// <returns>El query de selección de niveles</returns>
        public String SelectRestrictionNivel()
        {
            return String.Format("SELECT * " +
                                 "FROM {0} " +
                                 "WHERE " +
                                 "{1} = 'N' OR " +
                                 "{2} = 'N' OR " +
                                 "{3} = 'N' OR " +
                                 "{4} = 'N'",
                                 TABLENAME_RULES,
                                 FIELD_N1, FIELD_N2, FIELD_N3, FIELD_N4);
        }



    }
}

