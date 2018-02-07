using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.OldModulador.Controller;
using DaSoft.Riviera.OldModulador.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Mio;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Entities;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Model;
using NamelessOld.Libraries.HoukagoTeaTime.Yui;
using NamelessOld.Libraries.Yggdrasil.Aerith;
using NamelessOld.Libraries.Yggdrasil.Lain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static DaSoft.Riviera.OldModulador.Assets.Strings;

namespace DaSoft.Riviera.OldModulador.Model
{
    public class ChangeViewer
    {
        /// <summary>
        /// El documento activo
        /// </summary>
        Document Document;
        /// <summary>
        /// La transacción actual
        /// </summary>
        Transaction Tr;
        /// <summary>
        /// Verdadero si el modo 3D se encuentra activo
        /// </summary>
        Boolean Is3DEnabled;

        /// <summary>
        /// Crea un nuevo visor que se encarga de cambiar la vista.
        /// </summary>
        /// <param name="doc">El documento activo</param>
        /// <param name="tr">La transacción activa</param>
        /// <param name="is3dVwEnabled">Verdadero si la vista 3d es la actual</param>
        public ChangeViewer(Document doc, Transaction tr, Boolean is3dVwEnabled)
        {
            this.Document = doc;
            this.Tr = tr;
            this.Is3DEnabled = is3dVwEnabled;
        }
        /// <summary>
        /// Realiza un intercambio de vista entre un modo 2D a uno 3d,
        /// o viceversa
        /// </summary>
        public void Swap()
        {
            String name;
            BlockTableRecord conBlk;
            //1: Se abre la tabla de bloques
            BlockTable blkTab = (BlockTable)this.Document.Database.BlockTableId.GetObject(OpenMode.ForRead);
            //2: Se realiza la selección de todos los bloques cargados
            IEnumerable<String> insBlks = this.ExtractBlockNames(blkTab);
            //3: Se realiza un filtrado de los bloques contenedores
            IEnumerable<String> conBlks = this.FilterContainerBlocks(insBlks);
            //4: Los bloques tipo módulos
            IEnumerable<String> modBlks = insBlks.Where(x => x.Contains("MOD_"));
            //4: Se realiza la interación con la colección de bloques contenedores y se
            //realizá un intercambio en su contenido
            foreach (String blockname in conBlks.Union(modBlks))
            {
                try
                {
                    //5: Se abre el contenedor de bloque y se obtiene el nombre
                    //simple del bloque
                    conBlk = this.OpenContainer(blkTab, blockname, out name);
                    //6: Se realiza el cambio de vista dependiendo del estado actual
                    if (this.Is3DEnabled)//3D->2D
                    {
                        this.Swap(this.Tr, blkTab, insBlks, conBlk, name, "2D");
                        this.Isolate(this.Tr, true);
                    }
                    else//2D->3D
                    {
                        this.Swap(this.Tr, blkTab, insBlks, conBlk, name, "3D");
                        this.Isolate(this.Tr, false);
                    }
                }
                catch (Exception exc)
                {
                    String eMsg = "Error al cambiar el bloque" + blockname + "\n" + exc.Message;
                    App.Riviera.Log.AppendEntry(eMsg, Protocol.Error, "Swap", "ChageViewer");
                    Selector.Ed.WriteMessage(eMsg);
                }
            }
            //Los paneles son casos especiales ya que un elemento se cambia a muchos
            App.Riviera.Is3DEnabled = !App.Riviera.Is3DEnabled;
            App.DB.Objects.Where(x => x is RivieraPanelStack).ToList().ForEach(x => x.Regen(this.Tr));
            App.DB.Objects.Where(x => x is RivieraPanel).ToList().ForEach(x => x.Regen(this.Tr));
            App.DB.Objects.Where(x => x is RivieraBiombo).ToList().ForEach(x => x.Regen(this.Tr));

            //CreateExtra 3D or Delete Disposable 3D
            if (this.Is3DEnabled)//3D->2D
            {
                DBObject obj;
                foreach (ObjectId entId in this.Document.Database.CurrentSpaceId.GetObject(OpenMode.ForRead) as BlockTableRecord)
                {
                    obj = entId.GetObject(OpenMode.ForRead);
                    if (obj is BlockReference && (obj as BlockReference).ExtensionDictionary.IsValid)
                    {
                        DBDictionary d = (obj as BlockReference).ExtensionDictionary.GetObject(OpenMode.ForRead) as DBDictionary;
                        if (d.Contains(FIELD_DISPOSABLE_3D))
                            Drawer.Erase(this.Tr, obj.Id);
                    }
                }
            }
            else//2D->3D
            {
                foreach (JointObject joint in App.DB.Objects.Where(x => x is JointObject))
                    try
                    {
                        joint.Create3D(this.Tr);
                    }
                    catch (Exception exc)
                    {
                        App.Riviera.Log.AppendEntry(exc.Message, Protocol.Error, "Swap:void");
                        Selector.Ed.WriteMessage("\n{0}", exc.Message);
                    }
            }
            //Actualiza los estados de las capas.
            AutoCADLayer lay = new AutoCADLayer(LAYER_RIVIERA_OBJECT, this.Tr);
            if (this.Is3DEnabled)//3D->2D
                lay.SetStatus(LayerStatus.EnableStatus);
            else//2D->3D
                lay.SetStatus(LayerStatus.DisableStatus);
        }
        /// <summary>
        /// Vuelve invisible los textos y polilíneas contenidos en la capa
        /// de geometria
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        /// <param name="visibleStatus">El estado de visibilidad</param>
        private void Isolate(Transaction tr, bool visibleStatus)
        {
            BlockTableRecord model = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database.CurrentSpaceId.GetObject(OpenMode.ForRead) as BlockTableRecord;
            DBObject obj;
            foreach (ObjectId id in model)
            {
                obj = id.GetObject(OpenMode.ForRead);
                if (obj is Entity && (obj as Entity).Layer == LAYER_RIVIERA_GEOMETRY && (obj is Polyline || obj is DBText))
                {
                    obj.UpgradeOpen();
                    (obj as Entity).Visible = visibleStatus;
                }
            }
        }


        /// <summary>
        /// Obtiene los nombres de los bloques insertados en la tabla
        /// de bloques del documento actual
        /// </summary>
        /// <param name="table">La tabla de bloques del documento actual</param>
        /// <returns>La colección de los nombres de bloques</returns>
        private IEnumerable<String> ExtractBlockNames(BlockTable table)
        {
            return table.OfType<ObjectId>().
                             Select<ObjectId, DBObject>(x => x.GetObject(OpenMode.ForRead)).
                             Where(x => x is BlockTableRecord).Select<DBObject, String>(y => (y as BlockTableRecord).Name);
        }
        /// <summary>
        /// Realiza el filtrado de bloques contenedores, un bloque contenedor utiliza el prefijo
        /// SPACE_
        /// </summary>
        /// <param name="insBlks">La colección de nombres de bloques insertados en la tabla de bloques</param>
        /// <returns>La colección de bloques que contienen el prefijo</returns>
        private IEnumerable<String> FilterContainerBlocks(IEnumerable<string> insBlks)
        {
            return insBlks.Where(x => x.Contains(String.Format(PREFIX_BLOCK, "")));
        }
        /// <summary>
        /// Abre el bloque contenedor por su nombre en modo escritura
        /// </summary>
        /// <param name="blockname">El nombre del bloque contenedor</param>
        /// <param name="name">El nombre del bloque sin el prefijo "SPACE_"</param>
        /// <param name="table">La tabla de bloques del documento actual</param>
        /// <returns>El registro del contenedor de bloques</returns>
        private BlockTableRecord OpenContainer(BlockTable table, string blockname, out string name)
        {
            if (blockname.Contains("SPACE_"))
                name = blockname.Replace(String.Format("SPACE_", blockname), "");
            else if (blockname.Contains("MOD_"))
                name = blockname.Replace(String.Format("MOD_", ""), "");
            else
                name = blockname;
            return table[blockname].GetObject(OpenMode.ForWrite) as BlockTableRecord;
        }
        /// <summary>
        /// Cambia el contenido del bloque contenedor
        /// </summary>
        /// <param name="table">La tabla de bloques del documento actual</param>
        /// <param name="insBlks">La colección de nombres de bloques insertados en la tabla de bloques</param>
        /// <param name="conBlk">El registro de bloque con el contenedor</param>
        /// <param name="name">El nombre del bloque, para buscar el archivo</param>
        /// <param name="mode">El modo al que se cambiara usar "2D" o "3D"</param>
        private void Swap(Transaction tr, BlockTable blkTab, IEnumerable<String> insBlks, BlockTableRecord conBlk, string name, string mode)
        {
            String nameFormatted = name + mode;
            BlockTableRecord block;
            BlockReference blkRef;
            //Antes de realizar el intercambio, se verifica que el bloque este cargado.
            //O se intenta cargar en caso de que no lo estuviese
            if (this.LoadBlock(tr, insBlks.Contains(nameFormatted), nameFormatted))
            {
                //Se obtiene el Id del nuevo contenido
                block = blkTab[nameFormatted].GetObject(OpenMode.ForWrite) as BlockTableRecord;
                //Se borra el contenido del contenedor
                foreach (ObjectId id in conBlk)
                    id.GetObject(OpenMode.ForWrite).Erase();
                //Se crea el nuevo contenido
                blkRef = new BlockReference(new Point3d(), block.Id);
                //Se realiza la inserción del nuevo contenido
                conBlk.AppendEntity(blkRef);
                tr.AddNewlyCreatedDBObject(blkRef, true);
            }
        }
        /// <summary>
        /// Se realiza la carga del bloque si no ha sido cargado
        /// </summary>
        /// <param name="isBlockLoaded">Esta bandera nos dice si el bloque ya fue cargado</param>
        /// <param name="blockName">El nombre del bloque a cargar</param>
        /// <param name="tr">La transacción activa</param>
        /// <returns>Verdadero si el bloque ya fue cargado o si fue cargado con exito</returns>
        private Boolean LoadBlock(Transaction tr, Boolean isBlockLoaded, string blockName)
        {
            //Si el bloque no esta cargado se debe cargar.
            if (!isBlockLoaded)
            {
                Boolean result = false;                                                                                 //Define si el bloque es cargado
                String name = blockName.Substring(0, blockName.Length - 2);
                String parent = blockName.Substring(blockName.Length - 2);
                FileScanner scn = new FileScanner(App.Riviera.AppDirectory, true);
                AerithFilter filter = new FilterDwg() { Name = String.Format("{0}.dwg", name), ParentName = parent };
                scn.Find(filter);
                FileInfo file = scn.Files.FirstOrDefault();
                //La carga solo se realiza si el bloque existe
                if (file != null)
                {
                    new AutoCADBlock(blockName, file, tr);
                    result = true;
                }
                return result;
            }
            else
                return isBlockLoaded;
        }
        /// <summary>
        /// Se crea la vista 3d de los elementos seleccionados
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        /// <param name="moduleManager">El manejador de modulos</param>
        /// <param name="ids">La colección de ids generada</param>
        /// <returns>Verdadero si se llega a crear la vista 3D</returns>
        internal static bool Create3DView(Transaction tr, ModuleManager moduleManager, out ObjectIdCollection ids)
        {
            try
            {
                App.Riviera.Is3DEnabled = true;
                ids = new ObjectIdCollection();
                foreach (var obj in moduleManager.Database)
                {
                    obj.Regen(tr);
                    foreach (ObjectId id in obj.Ids)
                        ids.Add(id);
                    //Rellenos y remates finales de uniones
                    if (obj is JointObject)
                        foreach (ObjectId id in (obj as JointObject).Create3D(tr))
                            if (id.IsValid)
                                ids.Add(id);
                }
                foreach (ObjectId id in moduleManager.Ids)
                    if (!ids.Contains(id))
                        ids.Add(id);
            }
            catch (Exception exc)
            {
                ids = new ObjectIdCollection();
                Selector.Ed.WriteMessage("\n{0}", exc.Message);
            }
            return ids.Count > 0;
        }
        /// <summary>
        /// Resetea la vista actual
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        /// <param name="db">La base de datos seleccionada</param>
        /// <param name="is3DView">Verdadero si la vista 3d esta activa</param>
        internal static void ResetView(Transaction tr, List<RivieraObject> db, Boolean is3DView)
        {
            App.Riviera.Is3DEnabled = false;
            foreach (var obj in db)
                obj.Regen(tr);
        }
    }
}
