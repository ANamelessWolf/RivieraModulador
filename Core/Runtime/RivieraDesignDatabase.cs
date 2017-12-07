using DaSoft.Riviera.Modulador.Core.Model;
using DaSoft.Riviera.Modulador.Core.Model.DB;
using Nameless.Libraries.DB.Mikasa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Core.Runtime
{
    public abstract class RivieraDesignDatabase
    {
        /// <summary>
        /// The Design line working database
        /// </summary>
        public DesignLine Line;
        /// <summary>
        /// The Design line codes asociated to the design line
        /// </summary>
        List<RivieraCode> Codes = new List<RivieraCode>();
        /// <summary>
        /// Initializes a new instance of the <see cref="RivieraDatabase"/> class.
        /// </summary>
        /// <param name="line">The Riviera design line.</param>
        public RivieraDesignDatabase(DesignLine line)
        {
            this.Codes = new List<RivieraCode>();
        }
        /// <summary>
        /// Loads the design model data.
        /// </summary>
        /// <param name="gloabalResult">The gloabal result.</param>
        /// <param name="designResult">The design result.</param>
        public void LoadDesignModelData(RivieraDatabaseResult gloabalResult)
        {
            this.Codes = RivieraCodeRow.GetRivieraCodeForLine(gloabalResult.RivieraCodeRows, this.Line);
            this.LoadDesignModelData(gloabalResult.DesignResult[this.Line]);
        }
        /// <summary>
        /// Loads the design line data.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <returns>The design line data result</returns>
        public abstract Object InitDesignDatabase(DB_Connector conn);
        /// <summary>
        /// Loads the design application model
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <returns>The design line data result</returns>
        public abstract void LoadDesignModelData(Object result);
    }
}
