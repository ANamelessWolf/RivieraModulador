using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.OldModulador.Controller;
using DaSoft.Riviera.OldModulador.Model.Delta;
using DaSoft.Riviera.OldModulador.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Mio;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Entities;
using NamelessOld.Libraries.HoukagoTeaTime.MugiChan;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu;
using System;
using System.IO;
using System.Linq;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CONST;
using static DaSoft.Riviera.OldModulador.Assets.Strings;
namespace DaSoft.Riviera.OldModulador.Model
{
    public class Block3D
    {
        /// <summary>
        /// El elemento padre del bloque 3D
        /// </summary>
        public RivieraObject Parent;
        /// <summary>
        /// El Id del bloque insertado
        /// </summary>
        public ObjectId Id;
        /// <summary>
        /// Crea un bloque 3D, el bloque 3D no tiene información de cuantificación
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        /// <param name="blockname">El nombre del bloque</param>
        /// <param name="file">El archivo al bloque</param>
        /// <param name="obj">El objeto de riviera que genera al bloque</param>
        public Block3D(Transaction tr, String blockname, FileInfo file, RivieraObject obj)
        {
            AutoCADBlock block = new AutoCADBlock(blockname, file, tr);
            this.Parent = obj;
            double scale = App.Riviera.Units == DaNTeUnits.Metric ? 1d : IMPERIAL_FACTOR;
            BlockReference blkRef = block.CreateReference(obj.Start.ToPoint3d(), obj.Direction.Angle, scale);
            blkRef.Layer = LAYER_RIVIERA_GEOMETRY;
            this.Id = Drawer.Entity(blkRef);
            ExtensionDictionaryManager dMan = new ExtensionDictionaryManager(this.Id, tr);
            dMan.AddRegistry(FIELD_DISPOSABLE_3D, tr);
        }
        /// <summary>
        /// Crea un bloque 3D, el bloque 3D no tiene información de cuantificación
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        /// <param name="remate">La geometría del remate</param>
        public Block3D(Transaction tr, RemateGeometry remate)
        {
            FileInfo file = App.Riviera.Delta3D.Where(x => x.Name == (remate.Code + ".dwg")).FirstOrDefault();
            AutoCADBlock block = new AutoCADBlock(remate.Code, file, tr);
            this.Parent = remate.Parent;
            double scale = App.Riviera.Units == DaNTeUnits.Metric ? 1d : IMPERIAL_FACTOR;
            Double offSet = App.DB.Mampara_Sizes.Where(x => x.Alto == String.Format("{0:00}", remate.FloorOffset)).FirstOrDefault().Real.Alto;
            //if (App.Riviera.Units == DaNTeUnits.Imperial)
            offSet = offSet / 1000d;
            offSet *= scale;
            Point3d insPt = remate.InsertionPoint + new Vector3d(0, 0, offSet + REMATE_FRENTE_M);
            BlockReference blkRef = block.CreateReference(insPt, remate.Angle, scale);
            //Solo para probar la ubicación
            //blkRef.Color = Autodesk.AutoCAD.Colors.Color.FromRgb(255, 0, 0);
            blkRef.Layer = LAYER_RIVIERA_GEOMETRY;
            this.Id = Drawer.Entity(blkRef);
            ExtensionDictionaryManager dMan = new ExtensionDictionaryManager(this.Id, tr);
            dMan.AddRegistry(FIELD_DISPOSABLE_3D, tr);
        }

        public void UpdateHeight(Transaction tr, Double mamparaHeight)
        {
            BlockReference blkRef = this.Id.GetObject(OpenMode.ForWrite) as BlockReference;
            if (App.Riviera.Units == DaNTeUnits.Metric)
            {
                mamparaHeight = mamparaHeight.ConvertUnits(Unit_Type.inches, Unit_Type.m);
                mamparaHeight -= 0.0240d;
            }
            else
                mamparaHeight -= 0.0240d.ConvertUnits(Unit_Type.m, Unit_Type.inches);
            blkRef.Position = new Point3d(blkRef.Position.X, blkRef.Position.Y, blkRef.Position.Z + mamparaHeight);
            //blkRef.TransformBy(Matrix3d.Displacement(new Vector3d(0, 0, mamparaHeight)));
            //   blkRef.Rotation = Math.PI / 2;
        }
    }
}
