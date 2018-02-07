using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.OldModulador.Controller;
using DaSoft.Riviera.OldModulador.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Mio;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Entities;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static DaSoft.Riviera.OldModulador.Assets.Strings;

namespace DaSoft.Riviera.OldModulador.Model.Delta
{
    public class MamparaRemateFinal : RivieraObject, IBlockObject
    {

        /// <summary>
        /// The 2D Block file
        /// </summary>
        public FileInfo BlockFile2d
        {
            get
            {
                return this.Code.BlockFile2D();
            }
        }
        /// <summary>
        /// The 3D Block file
        /// </summary>
        public FileInfo BlockFile3d
        {
            get
            {
                return this.Code.BlockFile3D();
            }
        }
        /// <summary>
        /// El nombre del prefijo de bloque en donde se insertan los 
        /// bloques de mampara
        /// </summary>
        public String Spacename
        {
            get
            {
                return String.Format(PREFIX_BLOCK, this.Code);
            }
        }
        /// <summary>
        /// Verdadero si el modo 3D se encuentra activo
        /// </summary>
        public Boolean Is3DEnabled
        {
            get { return App.Riviera.Is3DEnabled; }
        }
        /// <summary>
        /// El contenido del bloque a insertar
        /// </summary>
        public AutoCADBlock Block { get { return _Block; } set { _Block = value; } }
        AutoCADBlock _Block;

        /// <summary>
        /// Crea un remate final para una mampara
        /// </summary>
        /// <param name="pt0">El punto inicial del rectangulo</param>
        /// <param name="ptf">El punto final del rectangulo</param>
        /// <param name="size">El tamaño del rectangulo</param>
        public MamparaRemateFinal(Mampara mampara, Point2d pt0, Point2d ptf, RivieraSize size, String code) :
            base(pt0, ptf, size, code)
        {
            if (mampara.Remates == null)
                mampara.Remates = new List<MamparaRemateFinal>();
            mampara.Remates.Add(this);
        }
        /// <summary>
        /// Crea el contenido de la mampara en este caso la inserción del bloque.
        /// </summary>
        public override void CreateContent()
        {
            if (this.BlockContent(this.Code))
                this.Content = new AutoCADBlock(this.Spacename);
            else
                base.CreateContent();
        }

        /// <summary>
        /// Realiza el dibujado del bloque a insertar
        /// </summary>
        /// <param name="tr">La transacción del bloque a insertar</param>
        public override void DrawContent(Transaction tr)
        {
            AutoCADBlock space = this.Content as AutoCADBlock;
            if (this.DrawBlockContent(space, this.Code, tr))
            {
                AutoCADLayer lay = new AutoCADLayer(LAYER_RIVIERA_GEOMETRY, tr);
                lay.SetStatus(LayerStatus.EnableStatus, tr);
                this.Ids.Add(Drawer.Entity(space.CreateReference(this.Line.StartPoint, this.Angle)));
                //Se agregán a una capa especial
                lay.AddToLayer(new ObjectIdCollection(this.Ids.OfType<ObjectId>().Where(x => this.Ids.IndexOf(x) > 0).ToArray()), tr);
            }
            else
                base.DrawContent(tr);

        }


    }
}
