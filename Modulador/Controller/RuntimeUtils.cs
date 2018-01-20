using DaSoft.Riviera.Modulador.Bordeo.Model;
using DaSoft.Riviera.Modulador.Core.Model;
using DaSoft.Riviera.Modulador.Core.Runtime;
using DaSoft.Riviera.Modulador.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Controller
{
    /// <summary>
    /// Defines the application utils class
    /// </summary>
    public static partial class RuntimeUtils
    {
        /// <summary>
        /// Initializes the database.
        /// </summary>
        /// <param name="app">The riviera application.</param>
        /// <param name="database">The database to initialize on the application.</param>
        public static void InitDatabase(this RivieraApplication app, params RivieraDatabase[] database)
        {
            var databaseLoaded = app.Database.DatabaseLoaded;
            app.Database = new RivieraDatabase();
            app.Database.DatabaseLoaded = databaseLoaded;
            DesignLine[] spLines = new DesignLine[] { DesignLine.Bordeo };
            RivieraDesignDatabase[] dbs = new RivieraDesignDatabase[] { new BordeoDesignDatabase() };
            for (int i = 0; i < spLines.Length; i++)
                app.Database.LineDB.Add(spLines[i], dbs[i]);
            WinAppInitializer win = new WinAppInitializer();
            win.Show();
            app.Database.Init(win);
        }
        /// <summary>
        /// Distincts the by.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <returns></returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}
