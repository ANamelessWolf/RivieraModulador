using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Nameless.Libraries.HoukagoTeaTime.Runtime;
using static DaSoft.Riviera.Modulador.Core.Assets.Strings;
using Nameless.Libraries.HoukagoTeaTime.Mio.Entities;
using DaSoft.Riviera.Modulador.Core.Runtime;

namespace DaSoft.Riviera.Modulador.Core.Controller.Transactions
{
    /// <summary>
    /// Defines the AutoCAD Runtime transactions
    /// </summary>
    public static partial class AutoCADTransactions
    {
        /// <summary>
        /// Initializes the layers.
        /// </summary>
        /// <param name="application">The application.</param>
        public static void InitLayers(this App application)
        {
            String[] layers = new String[] { LAYER_RIVIERA_OBJECT, LAYER_ERR, LAYER_RIVIERA_GEOMETRY, LAYER_RIVIERA_REPORT };
            QuickTransactionWrapper qTr = new QuickTransactionWrapper(
                (Document doc, Transaction tr) =>
                {
                    for (int i = 0; i < layers.Length; i++)
                        new AutoCADLayer(layers[i], tr);
                });
            qTr.Run();
        }

    }
}
