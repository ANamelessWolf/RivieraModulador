using Autodesk.AutoCAD.DatabaseServices;
using Nameless.Libraries.HoukagoTeaTime.Tsumugi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Core.Controller
{
    public static class AutoCADUtils
    {
        public const String KEY_ID = "ID";
        /// <summary>
        /// Sets the specified field.
        /// </summary>
        /// <param name="dMan">The extension dictionary manager.</param>
        /// <param name="field">The field name.</param>
        /// <param name="value">The string value.</param>
        /// <param name="tr">The active transaction</param>
        public static void Set(this ExtensionDictionaryManager dMan, Transaction tr, String field, params string[] values)
        {
            dMan.AddXRecord(field, tr).SetData(tr, values);
        }
    }
}
