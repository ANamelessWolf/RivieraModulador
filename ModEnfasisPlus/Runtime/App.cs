using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using DaSoft.Riviera.OldModulador.Model;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Entities;
using NamelessOld.Libraries.HoukagoTeaTime.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Yui;
using System;
using System.Collections.Generic;
using System.Linq;
using static DaSoft.Riviera.OldModulador.Assets.Strings;
namespace DaSoft.Riviera.OldModulador.Runtime
{
    public class App
    {
        public static RivieraApplication Riviera;
        public static Database DB;
        public static Boolean DEBUG_MODE = false;


        public delegate void Command();

        /// <summary>
        /// Checa si la información de la aplicación esta lista
        /// </summary>
        public static bool IsReady
        {
            get { return Riviera != null && DB != null; }
        }
        /// <summary>
        /// Ejecuta un comando de la aplicación
        /// </summary>
        /// <param name="cmd">El comando a ejecutar</param>
        public static void RunCommand(Command cmd)
        {
            if (App.IsReady)
            {
                try
                {
                    cmd();
                }
                catch (Exception exc)
                {

                    Selector.Ed.WriteMessage(exc.Message);
                }
            }
            else
                Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(String.Format(WARNING_START_CMD, CMD.START));
        }
        /// <summary>
        /// El proceso de creación de capas de la aplicación
        /// </summary>
        internal static void InitLayers()
        {
            String[] layers = new String[] { LAYER_RIVIERA_OBJECT };
            new FastTransactionWrapper(
                delegate (Document doc, Transaction tr)
                {
                    for (int i = 0; i < layers.Length; i++)
                        new AutoCADLayer(layers[i], tr);
                }).Run();
        }


        /// <summary>
        /// Realiza una limpieza de memoria antes de correr un comando
        /// </summary>
        public static void CleanCache()
        {
            FastTransactionWrapper ft = new FastTransactionWrapper(
                delegate (Document doc, Transaction tr)
                {
                    BlockTable tab = (BlockTable)doc.Database.BlockTableId.GetObject(OpenMode.ForRead);
                    BlockTableRecord rec = (BlockTableRecord)tab[BlockTableRecord.ModelSpace].GetObject(OpenMode.ForRead);
                    IEnumerable<DBObject> objs = rec.OfType<ObjectId>().Select<ObjectId, DBObject>(x => x.GetObject(OpenMode.ForRead)).Where(y => !y.IsErased);
                    objs = objs.Where(z => z is Entity && (z as Entity).Layer == LAYER_RIVIERA_OBJECT);
                    long[] handles = objs.Select<DBObject, long>(x => x.Handle.Value).OrderBy(x => x).ToArray();
                    RivieraObject[] rivObjects = App.DB.Objects.OrderBy(x => x.Handle.Value).ToArray();
                    List<int> invIndex = new List<int>();
                    int hIndex = 0, rIndex = 0;
                    long rivHandle, currentHandle;
                    while (rIndex < rivObjects.Length)
                    {
                        if (hIndex < handles.Length)
                        {
                            currentHandle = handles[hIndex];
                            rivHandle = rivObjects[rIndex].Handle.Value;
                            if (rivHandle == currentHandle)
                            {
                                hIndex++;
                                rIndex++;
                            }
                            else if (rivHandle > currentHandle)
                                hIndex++;
                            else
                            {
                                invIndex.Add(rIndex);
                                rIndex++;
                            }
                        }
                        else
                        {
                            invIndex.Add(rIndex);
                            rIndex++;
                        }
                    }

                    invIndex.ForEach(x => App.DB.Objects.Remove(rivObjects[x]));

                    //for (int i = App.DB.Objects.Count - 1; i >= 0; i--)
                    //    if (!handles.Contains(App.DB.Objects[i].Handle.Value))
                    //        invIndex.Add(i);
                    //foreach (int index in invIndex)
                    //    App.DB.Objects.RemoveAt(index);
                });
            if (App.DB.Objects.Count > 0)
                ft.Run();
        }

    }
}
