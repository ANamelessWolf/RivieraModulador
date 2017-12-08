using Nameless.Libraries.DB.Mikasa.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Core.Model.DB
{
    public class RivieraMeasureRow : DBMappingViewObject
    {
        public override string TableName => throw new NotImplementedException();

        public override string PrimaryKey => throw new NotImplementedException();

        protected override void ParseObject(SelectionResult[] result)
        {
            throw new NotImplementedException();
        }
    }
}
