using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.DB.Mikasa.Model
{
    public enum ConnectionInterface
    {
        Access = 0,
        MySQL = 1,
        SQL = 2,
        Oracle = 3,
        SQLite = 4,
        NotSupported = -1
    }
}
