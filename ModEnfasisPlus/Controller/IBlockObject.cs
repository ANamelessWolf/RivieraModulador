using NamelessOld.Libraries.HoukagoTeaTime.Mio.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.OldModulador.Controller
{
    public interface IBlockObject
    {
        /// <summary>
        /// El Bloque 2D
        /// </summary>
        FileInfo BlockFile2d { get; }
        /// <summary>
        /// El Bloque 3D
        /// </summary>
        FileInfo BlockFile3d { get; }
        /// <summary>
        /// El nombre del prefijo de bloque en donde se insertan los 
        /// bloques de mampara
        /// </summary>
        String Spacename { get; }
        /// <summary>
        /// Verdadero si el modo 3D se encuentra activo
        /// </summary>
        Boolean Is3DEnabled { get; }
        /// <summary>
        /// El registro con la información del contenido de bloque
        /// </summary>
        AutoCADBlock Block { get; set; }
    }
}
