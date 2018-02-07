using NamelessOld.Libraries.Yggdrasil.Aerith;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DaSoft.Riviera.OldModulador.Model
{
    public class FilterDwg : AerithFilter
    {
        /// <summary>
        /// El nombre del archivo a filtrar.
        /// </summary>
        public String Name;
        /// <summary>
        /// El nombre del directorio padre,
        /// para la aplicación es 2D o 3D
        /// </summary>
        public String ParentName;
        /// <summary>
        /// No hay una validación para directorio 2D o 3D
        /// </summary>
        /// <param name="directory">El directorio a validar</param>
        /// <returns>Verdadero si el directorio es válido</returns>
        public override bool IsDirectoryValid(DirectoryInfo directory)
        {
            return true;
        }
        /// <summary>
        /// Verifica si un archivo es válido a filtrar
        /// </summary>
        /// <param name="file">El archivo a validar</param>
        /// <returns>Verdadero si el archivo es valido</returns>
        public override bool IsFileValid(FileInfo file)
        {
            Boolean valid = file.Directory.Name == ParentName && file.Name.ToUpper() == Name.ToUpper();
            return valid;
        }
    }
}
