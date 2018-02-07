using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Windows.Data;
using DaSoft.Riviera.OldModulador.Model;
using DaSoft.Riviera.OldModulador.Query;
using DaSoft.Riviera.OldModulador.Runtime;
using DaSoft.Riviera.OldModulador.Runtime.DaNTe;
using DaSoft.Riviera.OldModulador.UI;
using NamelessOld.Libraries.DB.Mikasa.Model;
using NamelessOld.Libraries.DB.Tessa;
using NamelessOld.Libraries.DB.Tessa.Model;
using NamelessOld.Libraries.HoukagoTeaTime.Mio;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Entities;
using NamelessOld.Libraries.HoukagoTeaTime.MugiChan;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu.Shapes2D;
using NamelessOld.Libraries.HoukagoTeaTime.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Tsumugi;
using NamelessOld.Libraries.HoukagoTeaTime.Yui;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static DaSoft.Riviera.OldModulador.Assets.Strings;

namespace DaSoft.Riviera.OldModulador.Controller
{
    public class ModuleManager
    {
        /// <summary>
        /// La lista de ids que definen al modulo
        /// </summary>
        public ObjectIdCollection Ids;
        /// <summary>
        /// La colección de elementos que se cuantifican con
        /// la aplicación
        /// </summary>
        public List<RivieraObject> Database;
        /// <summary>
        /// La colección de ids de elementos que se cuantifican con
        /// DaNTe
        /// </summary>
        public List<Tuple<String, long>> DaNTeCodes;
        /// <summary>
        /// La cuantificación actual
        /// </summary>
        public Quantifier Quantification;

        /// <summary>
        /// Realiza la selección de elementos en el plano
        /// </summary>
        public Boolean Pick()
        {
            return Selector.ObjectIds(MSG_SEL_MOD, out Ids);
        }
        /// <summary>
        /// La caja de colisión del modulo dibujado
        /// </summary>
        public BoundingBox2D Box;
        /// <summary>
        /// El punto de inserción del bloque
        /// </summary>
        public Point3d InsPoint;

        /// <summary>
        /// Carga los códigos de cuantificación de los elementos seleccionados
        /// </summary>
        /// <returns>La colección de clave cantidad</returns>
        public List<Tuple<string, int>> Quantify()
        {
            List<Tuple<string, int>> res = new List<Tuple<string, int>>();
            this.DaNTeCodes = new List<Tuple<String, long>>();
            XDataManager xMan;
            String[] varRes;

            new FastTransactionWrapper(delegate (Document doc, Transaction tr)
            {
                DBObject obj;
                this.Database = new List<RivieraObject>();
                RivieraObject rivObj;

                foreach (ObjectId id in this.Ids)
                {
                    obj = id.GetObject(OpenMode.ForRead);
                    xMan = new XDataManager(obj as Entity);
                    varRes = xMan.GetAsString(tr);
                    if (varRes.Length > 0)
                    {
                        foreach (int index in varRes.Where(x => x == "CON").Select(y => varRes.ToList().IndexOf(y)))
                            if (index + 1 < varRes.Length)
                                this.DaNTeCodes.Add(new Tuple<string, long>(varRes[index + 1], obj.Handle.Value));
                    }
                    if (obj is Line)
                    {
                        rivObj = App.DB.Objects.Where(x => x.Handle == obj.Handle).FirstOrDefault();
                        if (rivObj != null)
                            Database.Add(rivObj);
                    }
                }
                this.Quantification = new Quantifier(this.Database, tr);
                //Cuantificación de elementos de DaNTe
                foreach (Tuple<string, long> danteCode in this.DaNTeCodes)
                    this.Quantification.AddItem(danteCode.Item1, 1, "", danteCode.Item2);

                String[] codes = this.Quantification.ItemQuantification.Select<QuantifiableObject, String>(y => y.Code).ToArray();
                foreach (var item in this.Quantification.ItemQuantification)
                    res.Add(new Tuple<string, int>(item.Code, item.Count));
                List<Tuple<string, int>> claveCantidad;
                foreach (var item in this.Quantification.UnionQuantification)
                {
                    claveCantidad = new List<Tuple<string, int>>();
                    foreach (var subItem in item.Members.Where(x => !codes.Contains(x)))
                        if (claveCantidad.Count(x => x.Item1 == subItem) > 0)
                        {
                            Tuple<string, int> current = claveCantidad.Where(x => x.Item1 == subItem).First();
                            int t = current.Item2 + 1;
                            claveCantidad.Remove(current);
                            claveCantidad.Add(new Tuple<string, int>(subItem, t));
                        }
                        else
                            claveCantidad.Add(new Tuple<string, int>(subItem, item.Count));
                    foreach (var uItem in claveCantidad)
                        res.Add(new Tuple<string, int>(uItem.Item1, uItem.Item2));
                }
            }).Run();
            return res;
        }
        /// <summary>
        /// Realiza el proceso de inserción del bloque
        /// </summary>
        /// <param name="moduleName">El nombre del módulo a insertar</param>
        /// <param name="insPt">El punto de inserción del bloque</param>
        internal void InsertBlock(string moduleName, Point3d insPt)
        {
            if (App.Riviera.Modules != null)
            {
                String fullBlockName = String.Format("MOD_{0}", moduleName);
                new FastTransactionWrapper(delegate (Document doc, Transaction tr)
                {
                    //Se realiza la carga de los bloques
                    String dwg2D = Path.Combine(App.Riviera.Modules2D.FullName, moduleName + ".dwg");
                    String dwg3D = Path.Combine(App.Riviera.Modules3D.FullName, moduleName + ".dwg");
                    if (File.Exists(dwg2D) && File.Exists(dwg3D))
                    {
                        AutoCADBlock blk2D = new AutoCADBlock(moduleName + "2D", new FileInfo(dwg2D), tr);
                        AutoCADBlock blk3D = new AutoCADBlock(moduleName + "3D", new FileInfo(dwg3D), tr);
                        //Se prepara el espacio de dibujo del bloque
                        BlockTable blkTab = doc.Database.BlockTableId.GetObject(OpenMode.ForRead) as BlockTable;
                        BlockTableRecord modRec;
                        if (!blkTab.Has(fullBlockName))
                        {
                            blkTab.UpgradeOpen();
                            modRec = new BlockTableRecord();
                            modRec.Name = fullBlockName;
                            blkTab.Add(modRec);
                            tr.AddNewlyCreatedDBObject(modRec, true);
                            BlockReference blkRef;
                            if (!App.Riviera.Is3DEnabled)
                                blkRef = blk2D.CreateReference(new Point3d(), 0, 1);
                            else
                                blkRef = blk3D.CreateReference(new Point3d(), 0, 1);
                            modRec.AppendEntity(blkRef);
                            tr.AddNewlyCreatedDBObject(blkRef, true);
                        }
                        else
                            modRec = blkTab[fullBlockName].GetObject(OpenMode.ForRead) as BlockTableRecord;
                        BlockReference insRef = new BlockReference(insPt, modRec.Id);
                        BlockTableRecord current = doc.Database.CurrentSpaceId.GetObject(OpenMode.ForWrite) as BlockTableRecord;
                        current.AppendEntity(insRef);
                        tr.AddNewlyCreatedDBObject(insRef, true);
                        ExtensionDictionaryManager dMan = new ExtensionDictionaryManager(insRef.Id, tr);
                        Xrecord r = dMan.AddRegistry(CAPTION_ISMODULE, tr);
                        r.SetData(tr, "SI");

                    }
                    else if (!File.Exists(dwg2D) && !File.Exists(dwg3D))
                        Dialog_MessageBox.Show(String.Format("No existe el bloque {0} ni el bloque {1}", dwg2D, dwg3D), System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                    else if (!File.Exists(dwg2D))
                        Dialog_MessageBox.Show(String.Format("No existe el bloque {0} ", dwg2D), System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                    else if (!File.Exists(dwg3D))
                        Dialog_MessageBox.Show(String.Format("No existe el bloque {0} ", dwg3D), System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);

                }).Run();
            }
            else
                Selector.Ed.WriteMessage("Verifique la carpeta de modulos");
        }

        /// <summary>
        /// Inserta un Modulo
        /// </summary>
        /// <param name="moduleName">Inserta un mudulo en la tabla de bases modulos</param>
        internal void SaveModule(string moduleName, List<Tuple<string, int>> Claves)
        {
            //Conexión al MDB de Modulos
            AccessConnectionBuilder aBuilder = new AccessConnectionBuilder()
            {
                Access_DB_File = App.Riviera.ModulosMDB.FullName,
                OleDbProvider = new AccessProvider(OledbProviders.Microsoft_ACE_OleDb_12),
            };
            AccessConnectionBuilder asocBuilder = new AccessConnectionBuilder()
            {
                Access_DB_File = App.Riviera.AsociadoMDB.FullName,
                OleDbProvider = new AccessProvider(OledbProviders.Microsoft_ACE_OleDb_12),
            };
            List<DaNTeRow> rows = new List<DaNTeRow>();
            new VoidAccess_Transaction<Object>(new AccessConnectionContent(aBuilder.Data).GenerateConnectionString(),
                delegate (Access_Connector conn, Object[] trParameters)
            {
                Query_BasesMDB query = new Query_BasesMDB();
                //Se crea la tabla del módulo o se limpia
                if (!conn.TableExist(moduleName))
                    conn.Update(query.CreateDaNTeTable(moduleName));
                else
                    conn.Update(query.ClearDaNTeTable(moduleName));
                //Agregamos un comentario al archivo de modulo
                conn.Update(query.InsertComment(moduleName, "Nuevo módulo creado " + DateTime.Now.ToShortDateString()));
                //Se insertan los registros en la BD de DaNTe
                DaNTeRow dRow;
                foreach (var row in Claves.OrderBy<Tuple<String, int>, String>(x => x.Item1))
                {
                    dRow = new DaNTeRow(row.Item2, row.Item1, false) { Handle = "0" };
                    rows.Add(dRow);
                    conn.Update(query.InsertDaNTeRow(moduleName, dRow));
                }
            }).Run();
            //Cuantificación de asociados
            new VoidAccess_Transaction<object>(new AccessConnectionContent(aBuilder.Data).GenerateConnectionString(),
                delegate (Access_Connector conn, Object[] trParameters)
                {
                    //Se insertan los registros en la BD de DaNTe
                    Query_BasesMDB query = new Query_BasesMDB();
                    foreach (DaNTeRow dRow in rows)
                    {
                        List<AsociadoRow> asocRows = DaNTe_Transaction.GetAsociados(conn, dRow.Clave, dRow.Value);
                        foreach (var asocRow in asocRows)
                            conn.Update(query.InsertAsociadoRow(moduleName, asocRow, String.Format("{0:x}", dRow.Handle)));
                    }
                }).Run();
        }
        /// <summary>
        /// Creates the image source from a TMP block
        /// </summary>
        /// <returns>The name of the block</returns>
        public ImageSource CreateTemporalBlocks()
        {
            ImageSource src = null;
            new FastTransactionWrapper(delegate (Document doc, Transaction tr)
            {
                BlockTable blkTab = doc.Database.BlockTableId.GetObject(OpenMode.ForRead) as BlockTable;
                BlockTableRecord modRec;
                //Se creo o se selecciona el registro temporal de Modulo
                if (!blkTab.Has(CAPTION_MODULE_RECORD))
                {
                    blkTab.UpgradeOpen();
                    modRec = new BlockTableRecord();
                    modRec.Name = CAPTION_MODULE_RECORD;
                    blkTab.Add(modRec);
                    tr.AddNewlyCreatedDBObject(modRec, true);
                }
                else
                {
                    modRec = blkTab[CAPTION_MODULE_RECORD].GetObject(OpenMode.ForWrite) as BlockTableRecord;
                    Drawer.Erase(tr, modRec.OfType<ObjectId>().ToArray());
                }
                //Se clonan
                //Se realiza el cambio en los ids
                foreach (ObjectId id in this.Ids)
                {
                    if (!id.IsErased)
                    {
                        try
                        {
                            Entity obj = id.GetObject(OpenMode.ForRead).Clone() as Entity;

                            if (obj is BlockReference && (obj as BlockReference).Name.Contains("SPACE_"))
                            {
                                DBObjectCollection blockColl = new DBObjectCollection();
                                obj.Explode(blockColl);
                                foreach (DBObject dbObj in blockColl)
                                {
                                    modRec.AppendEntity((Entity)dbObj);
                                    tr.AddNewlyCreatedDBObject(dbObj, true);
                                }
                            }
                            else
                            {
                                modRec.AppendEntity(obj);
                                tr.AddNewlyCreatedDBObject(obj, true);
                            }
                        }
                        catch (Exception exc)
                        {
                            App.Riviera.Log.AppendEntry(exc.Message, NamelessOld.Libraries.Yggdrasil.Lain.Protocol.Error, "QuantifyModules", "Quantification");
                        }
                    }
                }
                //Se traslada el bloque al origen
                IEnumerable<ObjectId> ids = modRec.OfType<ObjectId>();
                Point3d min = GetMinPoint(ids);
                if (this.InsPoint != new Point3d())
                    min = this.InsPoint;
                Matrix3d tt = Matrix3d.Displacement(new Vector3d(-min.X, -min.Y, -min.Z));
                Entity ent;
                foreach (ObjectId id in ids)
                {
                    ent = id.GetObject(OpenMode.ForWrite) as Entity;
                    ent.TransformBy(tt);
                }
                if (modRec.PreviewIcon == null)
                {
                    acedCommand(5005, "BLOCKICON", 5005, modRec.Name, 5000);
                    object ActiveDocument = doc.GetAcadDocument();
                    object[] data = { "_.BLOCKICON " + modRec.Name + "\n" };
                    ActiveDocument.GetType().InvokeMember("SendCommand", System.Reflection.BindingFlags.InvokeMethod, null, ActiveDocument, data);
                }
                //Se calculan los límites del bloque
                Extents3d extents = new Extents3d();
                extents.AddBlockExtents(modRec);
                //Se calcula el bitmap
                src = CMLContentSearchPreviews.GetBlockTRThumbnail(modRec);
            }).Run();
            return src;
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
            name = blockname.Replace(String.Format(PREFIX_BLOCK, ""), "");
            return table[blockname].GetObject(OpenMode.ForWrite) as BlockTableRecord;
        }
        /// <summary>
        /// Crea un archivo de bloque.
        /// </summary>
        /// <param name="blockName">The name of the block</param>
        public Boolean WriteBlock(String blockName)
        {
            Boolean flag = false;
            if (App.Riviera.Modules != null)
            {
                new FastTransactionWrapper(delegate (Document doc, Transaction tr)
                {
                    BlockTable blkTab = (BlockTable)doc.Database.BlockTableId.GetObject(OpenMode.ForRead);
                    BlockTableRecord tmpSpace = (BlockTableRecord)blkTab[CAPTION_MODULE_RECORD].GetObject(OpenMode.ForRead);
                    //Se crea el bloque 2D
                    Write2DBlock(tr, blockName, doc.Database, tmpSpace);
                    //Se cambia el entorno a 3D
                    App.Riviera.Is3DEnabled = false;

                    if (ChangeViewer.Create3DView(tr, this, out this.Ids))
                    {
                        CreateTemporalBlocks();
                        //Se crea el bloque 3D
                        Write3DBlock(tr, blockName, doc.Database, tmpSpace);
                    }
                    ChangeViewer.ResetView(tr, this.Database, false);
                    Selector.Ed.Regen();
                }).Run();
                flag = true;
            }
            else
                Dialog_MessageBox.Show(MSG_MODULE_DIR_MISSING, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            return flag;
        }
        /// <summary>
        /// Crea un archivo de bloque 2D
        /// </summary>
        /// <param name="tr">La transacción actual</param>
        /// <param name="tmpSpace">El archivo de bloque temporal</param>
        /// <param name="blockName">El nombre del bloque</param>
        /// <param name="sourceDb">La base de datos Actual</param>
        private void Write2DBlock(Transaction tr, String blockName, Autodesk.AutoCAD.DatabaseServices.Database sourceDb, BlockTableRecord tmpSpace)
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            using (Autodesk.AutoCAD.DatabaseServices.Database destDb = new Autodesk.AutoCAD.DatabaseServices.Database(true, false))
            {
                //Abrimos la tabla de la nueva base de datos
                BlockTable extBlkTab = (BlockTable)destDb.BlockTableId.GetObject(OpenMode.ForRead);
                ObjectId destDbMsId = SymbolUtilityServices.GetBlockModelSpaceId(destDb);
                //Se realiza la recolección de Ids de la antigua base de datos
                ObjectIdCollection sourceIds = new ObjectIdCollection();
                IdMapping mapping = new IdMapping();
                foreach (ObjectId id in tmpSpace)
                    sourceIds.Add(id);
                //Se realiza el proceso de clonación
                sourceDb.WblockCloneObjects(sourceIds, destDbMsId, mapping, DuplicateRecordCloning.Replace, false);
                //Se crea el archivo dwg con la nueva
                string dwgname = Path.Combine(App.Riviera.Modules2D.FullName, String.Format("{0}.dwg", blockName));
                destDb.SaveAs(dwgname, DwgVersion.Current);
                FileInfo fi = new FileInfo(dwgname);
                ed.WriteMessage("Bloque creado en {0}: {1}", dwgname, new NamelessOld.Libraries.Yggdrasil.Aerith.AerithSize(fi.Length));
            }
        }
        /// <summary>
        /// Crea un archivo de bloque 3D
        /// </summary>
        /// <param name="tr">La transacción actual</param>
        /// <param name="tmpSpace">El archivo de bloque temporal</param>
        /// <param name="blockName">El nombre del bloque</param>
        /// <param name="sourceDb">La base de datos Actual</param>
        private void Write3DBlock(Transaction tr, String blockName, Autodesk.AutoCAD.DatabaseServices.Database sourceDb, BlockTableRecord modelSpace)
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            using (Autodesk.AutoCAD.DatabaseServices.Database destDb = new Autodesk.AutoCAD.DatabaseServices.Database(true, false))
            {
                //Abrimos la tabla de la nueva base de datos
                BlockTable extBlkTab = (BlockTable)destDb.BlockTableId.GetObject(OpenMode.ForRead);
                ObjectId destDbMsId = SymbolUtilityServices.GetBlockModelSpaceId(destDb);
                //Se realiza la recolección de Ids de la antigua base de datos
                ObjectIdCollection sourceIds = new ObjectIdCollection();
                IdMapping mapping = new IdMapping();
                foreach (ObjectId id in modelSpace)
                    sourceIds.Add(id);
                //Se realiza el proceso de clonación
                sourceDb.WblockCloneObjects(sourceIds, destDbMsId, mapping, DuplicateRecordCloning.Replace, false);
                //Se crea el archivo dwg con la nueva
                string dwgname = Path.Combine(App.Riviera.Modules3D.FullName, String.Format("{0}.dwg", blockName));
                destDb.SaveAs(dwgname, DwgVersion.Current);
                FileInfo fi = new FileInfo(dwgname);
                ed.WriteMessage("Bloque creado en {0}: {1}", dwgname, fi.Length);
            }
        }

        [DllImport("accore.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "acedCommand")]
        private static extern int acedCommand(int type1, string command, int type2, string blockName, int end);
        /// <summary>
        /// Realiza la selección del punto mínimo del punto minimo de un bloque
        /// </summary>
        /// <param name="ids">la colección de los ids seleccionados</param>
        public Point3d GetMinPoint(IEnumerable<ObjectId> ids, Boolean drawBox = false)
        {
            Double minX = double.MaxValue, minY = double.MaxValue, minZ = double.MaxValue,
                   maxX = double.MinValue, maxY = double.MinValue, maxZ = double.MinValue;

            Point3d minPt, maxPt;
            Entity obj;
            foreach (ObjectId id in ids)
            {
                obj = id.GetObject(OpenMode.ForRead) as Entity;

                minPt = obj.GeometricExtents.MinPoint;
                maxPt = obj.GeometricExtents.MaxPoint;
                minX = minPt.X < minX ? minPt.X : minX;
                minY = minPt.Y < minY ? minPt.Y : minY;
                minZ = minPt.Z < minZ ? minPt.Z : minZ;
                maxX = maxPt.X < maxX ? maxX : maxPt.X;
                maxY = maxPt.Y < maxY ? maxY : maxPt.Y;
                maxZ = maxPt.Z < maxZ ? maxZ : maxPt.Z;
            }
            Box = new BoundingBox2D(new Point2d(minX, minY), new Point2d(maxX, maxY));
            Box = Box.Scale(1.05);
            //Point2d end = Box.Min.ToPoint2dByPolar((Box.Vector.Length / 10), Box.Vector.Angle);
            //Double x = Box.Min.X - end.X, y = Box.Min.Y - end.Y;
            //Box = Box.Move(new Vector2d(x, y));
            if (drawBox)
                Drawer.Geometry2D(Box, "", true);
            if (ids.Count() > 0)
                return new Point3d(minX, minY, minZ);
            else
                return new Point3d();
        }

        /// <summary>
        /// Convierte un bitmap en un image source
        /// </summary>
        /// <param name="bitmap">El bitmap a convertir</param>
        /// <returns>The bitmap image</returns>
        public BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            if (bitmap != null)
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                    memory.Position = 0;
                    BitmapImage bitmapimage = new BitmapImage();
                    bitmapimage.BeginInit();
                    bitmapimage.StreamSource = memory;
                    bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapimage.EndInit();

                    return bitmapimage;
                }
            }
            return null;
        }
    }
}
