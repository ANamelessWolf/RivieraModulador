using NamelessOld.Libraries.DB.Mikasa.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.DB.Tessa.Model
{
    public abstract class Access_Query : DB_Query
    {
     

        public abstract string TableName { get; }

        public override string FormatValue(string value)
        {
            value = value.Replace("\'", "\\\'");
            return value;
        }
    }
}
