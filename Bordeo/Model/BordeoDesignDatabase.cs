using DaSoft.Riviera.Modulador.Core.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaSoft.Riviera.Modulador.Core.Model;
using Nameless.Libraries.DB.Mikasa;

namespace DaSoft.Riviera.Modulador.Bordeo.Model
{
    /// <summary>
    /// Defines the Bordeo Design Database
    /// </summary>
    /// <seealso cref="DaSoft.Riviera.Modulador.Core.Runtime.RivieraDesignDatabase" />
    public class BordeoDesignDatabase : RivieraDesignDatabase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BordeoDesignDatabase"/> class.
        /// </summary>
        /// <param name="line">The Riviera design line.</param>
        public BordeoDesignDatabase(DesignLine line) :
            base(line)
        {
        }
        /// <summary>
        /// Loads the design line data.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <returns>
        /// The design line data result
        /// </returns>
        public override object InitDesignDatabase(DB_Connector conn)
        {
            BordeoDatabaseResult res = new BordeoDatabaseResult();
            res = this.InitSizes(conn);
        }
        /// <summary>
        /// Initializes the sizes.
        /// </summary>
        /// <param name="conn">The connection.</param>
        private BordeoDatabaseResult InitSizes(DB_Connector conn)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Loads the design application model
        /// </summary>
        /// <param name="result"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override void LoadDesignModelData(object result)
        {
            throw new NotImplementedException();
        }
    }
}
