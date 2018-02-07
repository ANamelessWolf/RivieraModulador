using NamelessOld.Libraries.Yggdrasil.Aerith;
using System.IO;

namespace DaSoft.Riviera.OldModulador.Model
{
    public class MDBFilter : AerithFilter
    {
        public override bool IsDirectoryValid(DirectoryInfo directory)
        {
            return true;
        }

        public override bool IsFileValid(FileInfo file)
        {
            return file.Extension.ToUpper().Replace(".", "") == "MDB";
        }
    }
}
