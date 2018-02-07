using DaSoft.Riviera.OldModulador.Model;
using NamelessOld.Libraries.DB.Misa.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.OldModulador.Query
{
    public class Query_Ejecutivo : OracleQuery
    {
        #region Constants
        const string TABLENAME = "EJECUTIVO";
        const string FIELD_ID = "ID";
        const string FIELD_PASS = "PASSWORD";
        const string FIELD_COMPANY = "CLAVE_COMPANIA";
        #endregion


        public String SelectUserID(UserCredential cred)
        {
            return "SELECT " + FIELD_ID + " " +
                           "FROM " + TABLENAME + " " +
                           "WHERE" + " " +
                                    FIELD_ID + " = " + cred.Username + " " +
                                    "AND " +
                                    FIELD_PASS + " = '" + cred.Password + "' " +
                                    "AND " +
                                    FIELD_COMPANY + " = " + ((int)cred.Company).ToString();
        }
    }
}
