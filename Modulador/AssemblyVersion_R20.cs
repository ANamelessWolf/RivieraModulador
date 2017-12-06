using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador
{
    /// <summary>
    /// Specifies the application version
    /// </summary>
    public class NamelessAssemblyVersion
    {
        public const string ASSEMBLY_TITLE = "Riviera Modulador" + AUTOCAD_VERSION;
        public const string ASSEMBLY_DESCRIPTION = "Modulador de Riviera";
        public const string ASSEMBLY_FRAMEWORK_VERSION = "[4.5]";
        public const string AUTOCAD_VERSION = "R20";
        public const string MINOR_VERSION = "20";
        public const string FRAMEWORK_VERSION = "45";
        public const string ASSEMBLY_COMPANY = "CAD.:Labs";
        public const string ASSEMBLY_PRODUCT = "riv_modulador [" + AUTOCAD_VERSION + "]";
        public const string ASSEMBLY_TRADEMARK = "A Nameless Wolf";
        public const string ASSEMBLY_VERSION = "3." + MINOR_VERSION + ".0." + FRAMEWORK_VERSION;
        public const string ASSEMBLY_FILE_VERSION = "3." + MINOR_VERSION + ".0." + FRAMEWORK_VERSION;
        public const String GUID = "90CF8202-908C-412E-B48E-D27BE9DDF904";
    }
}
