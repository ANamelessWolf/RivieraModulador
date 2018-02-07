using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Windows;
using NamelessOld.Libraries.HoukagoTeaTime.MugiChan;
using NamelessOld.Libraries.HoukagoTeaTime.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Yui;
using NamelessOld.Libraries.Yggdrasil.Lilith;
using NamelessOld.Libraries.Yggdrasil.Yuffie;
using System;
using System.Collections.Generic;
using System.Linq;
using static DaSoft.Riviera.OldModulador.Assets.Strings;
using CadDB = Autodesk.AutoCAD.DatabaseServices.Database;

namespace DaSoft.Riviera.OldModulador.Runtime
{
    public class ZoneManager
    {

        //[CommandMethod("DaNTe_AddZone")]
        /// <summary>
        /// Crea una nueva zona
        /// </summary>
        /// <param name="zoneName">El nombre de la zona</param>
        /// <returns>Verdadero si la zona es creada</returns>
        public Boolean AddZone(string zoneName)
        {
            Boolean flag = false;
            try
            {
                FastTransactionWrapper ft = new FastTransactionWrapper
                    (delegate (Document doc, Transaction tr)
                    {
                        doc.LockDocument();
                        Xrecord zones = OpenZoneRecord(tr);
                        string data = zones.GetDataAsString(tr).ConcatenateByCharacter(LilithConstants.ESCAPECHAR);
                        if (data == String.Empty)
                            zones.SetData(tr, zoneName);
                        else
                        {
                            String[] sData = data.Split(LilithConstants.ESCAPECHAR);
                            String[] nData = new String[sData.Length + 1];
                            for (int i = 0; i < sData.Length; i++)
                                nData[i] = sData[i];
                            nData[sData.Length] = zoneName;
                            zones.SetData(tr, nData);
                        }
                    });
                ft.Run();
                flag = true;
            }
            catch (System.Exception exc)
            {
                Selector.Ed.WriteMessage(exc.Message);
            }
            return flag;
        }
        /// <summary>
        /// Agregá una lista de elementos a una zona
        /// </summary>
        /// <param name="zoneName">El nombre de la zona</param>
        /// <param name="filter">Si el filtro esta activo, se ignoran los elementos con zonas</param>
        public void AddElementsToZone(string zoneName, bool filter)
        {
            ObjectIdCollection ids;
            try { Selector.Ed.SetImpliedSelection(new ObjectId[0]); }
            catch (Exception exc) { Selector.Ed.WriteMessage("\n{0}", exc); }
            if (Selector.ObjectIds("Selecciona los elementos agregar a la zona", out ids))
            {
                try
                {
                    FastTransactionWrapper ft = new FastTransactionWrapper
                        (delegate (Document doc, Transaction tr)
                        {
                            doc.LockDocument();
                            foreach (ObjectId id in ids)
                            {
                                Entity ent = (Entity)id.GetObject(OpenMode.ForWrite);
                                Xrecord zone = OpenZoneRecord(ent, tr);
                                String[] data = zone.GetDataAsString(tr);
                                String currentZone = data.Length == 0 ? String.Empty : data[0];
                                if (currentZone == String.Empty || !(filter && currentZone != String.Empty))
                                    zone.SetData(tr, zoneName);

                            }
                        });
                    ft.Run();
                }
                catch (System.Exception exc)
                {
                    Selector.Ed.WriteMessage(exc.Message);
                }
            }
        }
        /// <summary>
        /// Obtiene la lista de zonas disponibles
        /// </summary>
        /// <returns>la lista de zonas disponibles</returns>
        public List<string> ListZones()
        {
            List<string> zones = new List<string>();
            try
            {
                FastTransactionWrapper ft = new FastTransactionWrapper
                    (delegate (Document doc, Transaction tr)
                    {
                        doc.LockDocument();
                        Xrecord xZones = OpenZoneRecord(tr);
                        String[] data = xZones.GetDataAsString(tr);
                        if (data.Length > 0)
                            zones = xZones.GetDataAsString(tr).ToList();

                    });
                ft.Run();
            }
            catch (System.Exception exc)
            {
                Selector.Ed.WriteMessage(exc.Message);
            }
            zones.Sort();
            return zones;
        }
        /// <summary>
        /// Obtiene la lista de cuantificaciones realizadas
        /// </summary>
        /// <returns>la lista de cuantificaciones realizadas</returns>
        public List<string> ListQuantifications()
        {
            List<string> qs = new List<string>();
            try
            {
                FastTransactionWrapper ft = new FastTransactionWrapper
                    (delegate (Document doc, Transaction tr)
                    {
                        doc.LockDocument();
                        Xrecord xrec = OpenQuantificationRecord(tr);
                        String[] data = xrec.GetDataAsString(tr);
                        if (data.Length > 0)
                            qs = data.ToList();

                    });
                ft.Run();
            }
            catch (System.Exception exc)
            {
                Selector.Ed.WriteMessage(exc.Message);
            }
            qs.Sort();
            return qs;
        }
        /// <summary>
        /// Obtiene la lista de cuantificaciones realizadas
        /// </summary>
        /// <returns>la lista de cuantificaciones realizadas</returns>
        public void SaveListQuantifications(List<String> data)
        {
            List<string> qs = new List<string>();
            try
            {
                FastTransactionWrapper ft = new FastTransactionWrapper
                    (delegate (Document doc, Transaction tr)
                    {
                        doc.LockDocument();
                        Xrecord xrec = OpenQuantificationRecord(tr);
                        xrec.UpgradeOpen();
                        xrec.SetData(tr, data.ToArray());
                    });
                ft.Run();
            }
            catch (System.Exception exc)
            {
                Selector.Ed.WriteMessage(exc.Message);
            }
        }
        /// <summary>
        /// Remueva un elemento de la zona
        /// </summary>
        public void RemoveZoneFromElements()
        {
            ObjectIdCollection ids;
            try { Selector.Ed.SetImpliedSelection(new ObjectId[0]); }
            catch (Exception exc) { Selector.Ed.WriteMessage("\n{0}", exc); }
            if (Selector.ObjectIds("Selecciona los elementos a remover su zona", out ids))
            {
                try
                {
                    FastTransactionWrapper ft = new FastTransactionWrapper
                        (delegate (Document doc, Transaction tr)
                        {
                            doc.LockDocument();
                            foreach (ObjectId id in ids)
                            {
                                Entity ent = (Entity)id.GetObject(OpenMode.ForWrite);
                                if (ent.ExtensionDictionary.IsValid)
                                {
                                    Xrecord zone = OpenZoneRecord(ent, tr);
                                    zone.SetData(tr, String.Empty);
                                }
                            }
                        });
                    ft.Run();
                }
                catch (System.Exception exc)
                {
                    Selector.Ed.WriteMessage(exc.Message);
                }
            }
        }
        /// <summary>
        /// Inspección de zona
        /// </summary>
        public void SnoopElementZone()
        {
            ObjectId id;
            try { Selector.Ed.SetImpliedSelection(new ObjectId[0]); }
            catch (Exception exc) { Selector.Ed.WriteMessage("\n{0}", exc); }
            if (Selector.ObjectId("Selecciona un elemento para revisar su zona", out id))
            {
                try
                {
                    FastTransactionWrapper ft = new FastTransactionWrapper
                        (delegate (Document doc, Transaction tr)
                        {
                            doc.LockDocument();
                            TaskDialog td;
                            Entity ent = (Entity)id.GetObject(OpenMode.ForWrite);
                            if (ent.ExtensionDictionary.IsValid)
                            {
                                Xrecord zone = OpenZoneRecord(ent, tr);
                                String[] data = zone.GetDataAsString(tr);
                                if (data.Length == 0 || data[0].Length == 0)
                                {
                                    td = new TaskDialog()
                                    {
                                        WindowTitle = "Zona",
                                        ContentText = "Sin Zona"
                                    };
                                }
                                else
                                    td = new TaskDialog()
                                    {
                                        WindowTitle = "Zona",
                                        ContentText = data[0]
                                    };
                            }
                            else
                            {
                                td = new TaskDialog()
                                {
                                    WindowTitle = "Zona",
                                    ContentText = "Sin Zona"
                                };

                            }
                            td.Show();
                        });
                    ft.Run();
                }
                catch (System.Exception exc)
                {
                    Selector.Ed.WriteMessage(exc.Message);
                }
            }
        }

        /// <summary>
        /// Resalta los elementos que pertenecen a una zona especifica
        /// </summary>
        /// <param name="zoneName">El nombre de la zona</param>
        public void SnoopElementByZone(String zoneName = "")
        {
            ObjectIdCollection ids = new ObjectIdCollection();
            try { Selector.Ed.SetImpliedSelection(new ObjectId[0]); }
            catch (Exception exc) { Selector.Ed.WriteMessage("\n{0}", exc); }
            try
            {
                FastTransactionWrapper ft = new FastTransactionWrapper
                    (delegate (Document doc, Transaction tr)
                    {
                        doc.LockDocument();
                        BlockTableRecord currenSpace = (BlockTableRecord)doc.Database.CurrentSpaceId.GetObject(OpenMode.ForRead);
                        foreach (ObjectId id in currenSpace)
                        {
                            DBObject obj = id.GetObject(OpenMode.ForRead);
                            if (zoneName == "")
                            {
                                if (obj is Entity && !(obj as Entity).ExtensionDictionary.IsValid)
                                    ids.Add(id);
                                else if (obj is Entity && (obj as Entity).ExtensionDictionary.IsValid)
                                {
                                    Xrecord zone = OpenZoneRecord((obj as Entity), tr);
                                    String[] data = zone.GetDataAsString(tr);
                                    if (data.Length == 0 || data[0].Length == 0)
                                        ids.Add(id);
                                }
                            }
                            else if (obj is Entity && (obj as Entity).ExtensionDictionary.IsValid)
                            {
                                Xrecord zone = OpenZoneRecord((obj as Entity), tr);
                                String[] data = zone.GetDataAsString(tr);
                                if (!(data.Length == 0 || data[0].Length == 0) && data[0] == zoneName)
                                    ids.Add(id);
                            }
                        }
                    });
                ft.Run();
                if (ids.Count > 0)
                    Selector.Ed.SetImpliedSelection(ids.OfType<ObjectId>().ToArray());
                Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();
            }
            catch (System.Exception exc)
            {
                Selector.Ed.WriteMessage(exc.Message);
            }
        }

        /// <summary>
        /// Cambia el nombre de una zona a otro nombre
        /// </summary>
        /// <param name="zoneName">El nombre de la zona</param>
        /// <param name="newZoneName">El nombre de la nueva zona</param>
        /// <returns>Verdadero si el nombre es cambiado</returns>
        internal bool RenameZone(string zoneName, string newZoneName)
        {
            Boolean flag = false;
            try
            {
                FastTransactionWrapper ft = new FastTransactionWrapper
                    (delegate (Document doc, Transaction tr)
                    {
                        doc.LockDocument();
                        Xrecord zones = OpenZoneRecord(tr);
                        string data = zones.GetDataAsString(tr).ConcatenateByCharacter(LilithConstants.ESCAPECHAR);
                        data = data.Replace(zoneName, newZoneName).Replace(LilithConstants.DOUBLEESCAPECHAR, LilithConstants.ESCAPECHAR.ToString());
                        if (data[data.Length - 1] == LilithConstants.ESCAPECHAR)
                            data = data.Substring(0, data.Length - 1);
                        if (data == String.Empty)
                            zones.SetData(tr, newZoneName);
                        else
                            zones.SetData(tr, data.Split(LilithConstants.ESCAPECHAR));
                    });
                ft.Run();
                flag = true;
            }
            catch (System.Exception exc)
            {
                Selector.Ed.WriteMessage(exc.Message);
            }
            return flag;
        }

        /// <summary>
        /// Elimina una zona existenta
        /// </summary>
        /// <param name="zoneName">El nombre de la zona</param>
        /// <returns>Verdadero si la zona es removida</returns>
        public Boolean RemoveZone(string zoneName)
        {
            Boolean flag = false;
            try
            {
                FastTransactionWrapper ft = new FastTransactionWrapper
                    (delegate (Document doc, Transaction tr)
                    {
                        doc.LockDocument();
                        Xrecord zones = OpenZoneRecord(tr);
                        string data = zones.GetDataAsString(tr).ConcatenateByCharacter(LilithConstants.ESCAPECHAR);
                        data = data.Replace(zoneName, "").Replace(LilithConstants.DOUBLEESCAPECHAR, LilithConstants.ESCAPECHAR.ToString());
                        if (data == String.Empty)
                            zones.SetData(tr, "");
                        else if (data[data.Length - 1] == LilithConstants.ESCAPECHAR)
                            data = data.Substring(0, data.Length - 1);
                        else
                            zones.SetData(tr, data.Split(LilithConstants.ESCAPECHAR));
                    });
                ft.Run();
                flag = true;
            }
            catch (System.Exception exc)
            {
                Selector.Ed.WriteMessage(exc.Message);
            }
            return flag;
        }
        /// <summary>
        /// Abre el registro de zonas definidas por
        /// la aplicación DaNTe
        /// </summary>
        /// <param name="ent">La entidad seleccionada</param>
        /// <param name="tr">La transacción activa</param>
        /// <returns>El registro de la zona seleccionada</returns>
        private Xrecord OpenZoneRecord(Transaction tr)
        {
            CadDB db = Application.DocumentManager.MdiActiveDocument.Database;
            DBDictionary NOD = (DBDictionary)db.NamedObjectsDictionaryId.GetObject(OpenMode.ForWrite);
            DManager dMan = new DManager(NOD);
            DBDictionary daNTeDic;
            Xrecord zones;
            if (!dMan.TryGetDictionary(CAPTION_DAN, out daNTeDic, tr))
                daNTeDic = dMan.AddDictionary(CAPTION_DAN, tr);
            dMan = new DManager(daNTeDic);
            if (!dMan.TryGetRegistry(CAPTION_ZONE, out zones, tr))
                zones = dMan.AddRegistry(CAPTION_ZONE, tr);
            return zones;
        }
        /// <summary>
        /// Abre el registro de cuantificaciones realizadas
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        /// <returns>El registro de las cuantificaciones</returns>
        private Xrecord OpenQuantificationRecord(Transaction tr)
        {
            CadDB db = Application.DocumentManager.MdiActiveDocument.Database;
            DBDictionary NOD = (DBDictionary)db.NamedObjectsDictionaryId.GetObject(OpenMode.ForWrite);
            DManager dMan = new DManager(NOD);
            DBDictionary daNTeDic;
            Xrecord qs;
            if (!dMan.TryGetDictionary(CAPTION_DAN, out daNTeDic, tr))
                daNTeDic = dMan.AddDictionary(CAPTION_DAN, tr);
            dMan = new DManager(daNTeDic);
            if (!dMan.TryGetRegistry(CAPTION_QUANTIFICATIONS, out qs, tr))
                qs = dMan.AddRegistry(CAPTION_QUANTIFICATIONS, tr);
            return qs;
        }
        /// <summary>
        /// Abre el registro de zona de una entidad seleccionada
        /// </summary>
        /// <param name="ent">La entidad seleccionada</param>
        /// <param name="tr">La transacción activa</param>
        /// <returns>El registro de la zona seleccionada</returns>
        private Xrecord OpenZoneRecord(Entity ent, Transaction tr)
        {
            ExtensionDictionaryManager dMan = new ExtensionDictionaryManager(ent.Id, tr);
            Xrecord zone;
            if (!dMan.TryGetRegistry(CAPTION_ZONE, out zone, tr))
                zone = dMan.AddRegistry(CAPTION_ZONE, tr);
            return zone;
        }
    }
}
