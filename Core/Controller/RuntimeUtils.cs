using DaSoft.Riviera.Modulador.Core.Model;
using DaSoft.Riviera.Modulador.Core.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Core.Controller
{
    /// <summary>
    /// Defines the application utils class
    /// </summary>
    public static partial class ApplicationUtils
    {
        /// <summary>
        /// Gets the parent object.
        /// </summary>
        /// <param name="obj">The parent object.</param>
        /// <returns>The parent object</returns>
        public static RivieraObject GetParent(this RivieraObject obj)
        {
            if (obj.Handle.Value != 0)
                return App.Riviera.Database.Objects.FirstOrDefault(x => x.Handle.Value == obj.Handle.Value);
            else
                return null;
        }
    }
}
