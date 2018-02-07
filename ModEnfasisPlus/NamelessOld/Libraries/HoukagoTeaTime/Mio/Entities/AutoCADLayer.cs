using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using NamelessOld.Libraries.HoukagoTeaTime.Assets;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Model;
using NamelessOld.Libraries.HoukagoTeaTime.Runtime;
using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.Collections.Generic;
using System.Linq;
namespace NamelessOld.Libraries.HoukagoTeaTime.Mio.Entities
{
    public class AutoCADLayer : NamelessObject
    {
        /// <summary>
        /// Gets the layer name
        /// </summary>
        public readonly String Layername;
        /// <summary>
        /// The layer Object Id
        /// </summary>
        public new ObjectId Id;
        #region Constructor
        /// <summary>
        /// Create a new autocad layer
        /// </summary>
        /// <param name="layerName">The name of the layer</param>
        public AutoCADLayer(string layerName)
        {
            try
            {
                if (ValidateLayername(layerName))
                {
                    this.Layername = layerName;
                    TransactionWrapper<String, ObjectId> tr =
                        new TransactionWrapper<String, ObjectId>(CreateLayerTransaction);
                    this.Id = tr.Run(layerName);
                }
                else
                    throw new RomioException(Errors.InvalidLayerName);
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", Errors.CanNotCreateLayer, exc.Message), exc);
            }
        }



        /// <summary>
        /// Create a new autocad layer with an active transaction
        /// </summary>
        /// <param name="layerName">The name of the layer</param>
        /// <param name="tr">An active transaction</param>
        public AutoCADLayer(string layerName, Transaction tr)
        {
            try
            {
                if (ValidateLayername(layerName))
                {
                    this.Layername = layerName;
                    this.Id = CreateLayerTransaction(Application.DocumentManager.MdiActiveDocument, tr, layerName);
                }
                else
                    throw new RomioException(Errors.InvalidLayerName);
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", Errors.CanNotCreateLayer, exc.Message), exc);
            }
        }
        /// <summary>
        /// Create a new autocad layer
        /// </summary>
        /// <param name="layerName">The name of the layer</param>
        /// <param name="color">The color to be asigned to the layer</param>
        public AutoCADLayer(string layerName, Color color)
        {
            try
            {
                if (ValidateLayername(layerName))
                {
                    this.Layername = layerName;
                    TransactionWrapper<Object, ObjectId> tr =
                        new TransactionWrapper<Object, ObjectId>(CreateLayerWithColorTransaction);
                    this.Id = tr.Run(layerName, color);

                }
                else
                    throw new RomioException(Errors.InvalidLayerName);
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", Errors.CanNotCreateLayer, exc.Message), exc);
            }
        }
        /// <summary>
        /// Create a new autocad layer
        /// </summary>
        /// <param name="layerName">The name of the layer</param>
        /// <param name="color">The color to be asigned to the layer</param>
        public AutoCADLayer(string layerName, Color color, Transaction tr)
        {
            try
            {
                if (ValidateLayername(layerName))
                {
                    this.Id = this.CreateLayerWithColorTransaction(Application.DocumentManager.MdiActiveDocument, tr, layerName, color);
                    this.Layername = layerName;
                }
                else
                    throw new RomioException(Errors.InvalidLayerName);
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", Errors.CanNotCreateLayer, exc.Message), exc);
            }
        }
        #endregion
        #region Actions
        /// <summary>
        /// Gets the layer current color
        /// </summary>
        /// <returns>The layer color</returns>
        public Color GetColor()
        {
            try
            {
                BlankTransactionWrapper<Color> tr =
                      new BlankTransactionWrapper<Color>(CheckLayerColorTransaction);
                return tr.Run();
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", Errors.FailedToGetLayerColor, exc.Message), exc);
            }
        }
        /// <summary>
        /// Gets the layer current color
        /// </summary>
        /// <param name="tr">The active transaction</param>
        /// <returns>The layer color</returns>
        public Color GetColor(Transaction tr)
        {
            try
            {
                return CheckLayerColorTransaction(Application.DocumentManager.MdiActiveDocument, tr);
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", Errors.FailedToGetLayerColor, exc.Message), exc);
            }
        }
        /// <summary>
        /// Get the layer status
        /// </summary>
        public LayerStatus CheckStatus()
        {
            try
            {
                BlankTransactionWrapper<LayerStatus> tr =
                      new BlankTransactionWrapper<LayerStatus>(CheckLayerStatusTransaction);
                return tr.Run();
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", Errors.FailedToGetLayerStatus, exc.Message), exc);
            }
        }
        /// <summary>
        /// Get the layer status
        /// </summary>
        /// <param name="tr">An active transaction</param>
        public LayerStatus CheckStatus(Transaction tr)
        {
            try
            {
                return this.CheckLayerStatusTransaction(Application.DocumentManager.MdiActiveDocument, tr);
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", Errors.FailedToGetLayerStatus, exc.Message), exc);
            }
        }

        /// <summary>
        /// Set the layer status
        /// </summary>
        /// <param name="status">The status to set</param>
        public void SetStatus(LayerStatus status)
        {
            try
            {
                VoidTransactionWrapper<LayerStatus> tr =
                      new VoidTransactionWrapper<LayerStatus>(UpdateLayerStatusTransaction);
                tr.Run(status);
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", Errors.FailedToUpdateStatus, exc.Message), exc);
            }
        }
        /// <summary>
        /// Set the layer status
        /// </summary>
        /// <param name="status">The status to set</param>
        /// <param name="tr">An active transaction</param>
        public void SetStatus(LayerStatus status, Transaction tr)
        {
            try
            {
                this.UpdateLayerStatusTransaction(Application.DocumentManager.MdiActiveDocument, tr, status);
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", Errors.FailedToUpdateStatus, exc.Message), exc);
            }
        }
        /// <summary>
        /// Updates the color of the layer
        /// </summary>
        /// <param name="color">The color of the layer</param>
        public void UpdateColor(Color color)
        {
            try
            {
                VoidTransactionWrapper<Color> tr =
                      new VoidTransactionWrapper<Color>(SetLayerColorTransaction);
                tr.Run(color);
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", Errors.FailedToUpdateStatus, exc.Message), exc);
            }
        }
        /// <summary>
        /// Updates the color of the layer
        /// </summary>
        /// <param name="color">The color of the layer</param>
        /// <param name="tr">An active transaction</param>
        public void UpdateColor(Color color, Transaction tr)
        {
            try
            {
                this.SetLayerColorTransaction(Application.DocumentManager.MdiActiveDocument, tr, color);
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", Errors.FailedToUpdateStatus, exc.Message), exc);
            }
        }
        /// <summary>
        /// Set this layer as the active or current layer, for the current drawing
        /// </summary>
        public void SetAsCurrentLayer()
        {
            try
            {
                VoidTransactionWrapper<Object> tr =
                      new VoidTransactionWrapper<Object>(SetAsCurrentLayerTransaction);
                tr.Run();
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", Errors.FailedToUpdateStatus, exc.Message), exc);
            }
        }
        /// <summary>
        /// Set this layer as the active or current layer, for the current drawing
        /// </summary>
        /// <param name="tr">An active transaction</param>
        public void SetAsCurrentLayer(Transaction tr)
        {
            try
            {
                this.SetAsCurrentLayerTransaction(Application.DocumentManager.MdiActiveDocument, tr);
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", Errors.FailedToUpdateStatus, exc.Message), exc);
            }
        }
        /// <summary>
        /// Add a group of entities(by its Ids) to the current layer
        /// </summary>
        /// <param name="objIds">The entities object ids collection</param>
        public void AddToLayer(ObjectIdCollection objIds)
        {
            try
            {
                VoidTransactionWrapper<ObjectIdCollection> tr =
                      new VoidTransactionWrapper<ObjectIdCollection>(AddToLayerTransaction);
                tr.Run(objIds);
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", Errors.FailedToUpdateStatus, exc.Message), exc);
            }
        }
        /// <summary>
        /// Add a group of entities(by its Ids) to the current layer
        /// </summary>
        /// <param name="objIds">The entities object ids collection</param>
        /// <param name="tr">An active transaction</param>
        public void AddToLayer(ObjectIdCollection objIds, Transaction tr)
        {
            try
            {
                this.AddToLayerTransaction(Application.DocumentManager.MdiActiveDocument, tr, objIds);
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", Errors.FailedToUpdateStatus, exc.Message), exc);
            }
        }
        /// <summary>
        /// Delete any item contained on this layer
        /// </summary>
        /// <param name="tr">An active transaction</param>
        public void Clear(Transaction tr)
        {
            try
            {
                this.DeleteMembersLayerTransaction(Application.DocumentManager.MdiActiveDocument, tr);
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", Errors.CanNotBeErased, exc.Message), exc);
            }
        }
        /// <summary>
        /// Delete any item contained on this layer
        /// </summary>
        public void Clear()
        {
            try
            {
                FastTransactionWrapper tr =
                      new FastTransactionWrapper(DeleteMembersLayerTransaction);
                tr.Run();
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", Errors.CanNotBeErased, exc.Message), exc);
            }
        }
        /// <summary>
        /// Get a list of layers in the current drawing.
        /// Retrieve the list of the layers names
        /// </summary>
        /// <returns>The list of layers</returns>
        public static List<String> ListLayers()
        {
            try
            {
                BlankTransactionWrapper<List<String>> trMom =
                    new BlankTransactionWrapper<List<String>>(ListLayersTransaction);
                return trMom.Run();
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", Errors.FailedToListLayers, exc.Message), exc);
            }
        }
        /// <summary>
        /// Get a list of layers in the current drawing.
        /// Retrieve the list of the layers names
        /// </summary>
        /// <param name="tr">The Active transaction</param>
        /// <returns>The list of layers</returns>
        public static List<String> ListLayers(Transaction tr)
        {
            try
            {
                return ListLayersTransaction(Application.DocumentManager.MdiActiveDocument, tr);
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", Errors.FailedToListLayers, exc.Message), exc);
            }
        }
        /// <summary>
        /// Get the active layer for the current drawing
        /// </summary>
        /// <returns>The active layer</returns>
        public static AutoCADLayer GetActiveLayer()
        {
            try
            {
                BlankTransactionWrapper<AutoCADLayer> trMom =
                    new BlankTransactionWrapper<AutoCADLayer>(SelectActiveLayerTransaction);
                return trMom.Run();
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", Errors.FailedToListLayers, exc.Message), exc);
            }
        }
        /// <summary>
        /// Get the active layer for the current drawing
        /// </summary>
        /// <param name="tr">The Active transaction</param>
        /// <returns>The active layer</returns>
        public static AutoCADLayer GetActiveLayer(Transaction tr)
        {
            try
            {
                return SelectActiveLayerTransaction(Application.DocumentManager.MdiActiveDocument, tr);
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", Errors.FailedToListLayers, exc.Message), exc);
            }
        }
        /// <summary>
        /// Validates the name of a layer, true if the layer name is valid
        /// </summary>
        /// <param name="layerName">The name of the layer</param>
        /// <returns>True if the layer is valid</returns>
        public static bool ValidateLayername(string layerName)
        {
            Boolean flag = true;
            //1: El nombre de la capa no puede estar vacio
            if (layerName == String.Empty)
                flag = false;
            else
            {
                //2: Se revisa que no contenga caracteres invalidos.
                foreach (Char ch in layerName)
                    if (Strings.InvalidLayerCharacters.Contains(ch))
                    {
                        flag = false;
                        break;
                    }
            }

            return flag;
        }
        #endregion
        #region Transacciones
        /// <summary>
        /// Check the layer color transaction
        /// </summary>
        /// <param name="doc">The AutoCAD Active Document</param>
        /// <param name="tr">The active transaction</param>
        /// <returns>The layer color</returns>
        private Color CheckLayerColorTransaction(Document doc, Transaction tr)
        {
            LayerTableRecord layRec = this.Id.GetObject(OpenMode.ForRead) as LayerTableRecord;
            return layRec.Color;
        }
        /// <summary>
        /// Creates a layer or open an existant layer by its, name
        /// </summary>
        /// <param name="doc">The AutoCAD Active Document</param>
        /// <param name="tr">The active transaction</param>
        /// <param name="data">The parameters that receive the transaction</param>
        /// <returns>The ObjectId of the transaction</returns>
        private ObjectId CreateLayerTransaction(Document doc, Transaction tr, params string[] data)
        {
            string layerName = data[0];
            ObjectId layId;
            LayerTable layTab = (LayerTable)doc.Database.LayerTableId.GetObject(OpenMode.ForRead);
            LayerTableRecord layRec;
            if (layTab.Has(layerName))
            {
                layId = layTab[layerName];
                layRec = layId.GetObject(OpenMode.ForRead) as LayerTableRecord;
            }
            else
            {
                layTab.UpgradeOpen();
                layRec = new LayerTableRecord();
                layRec.Name = layerName;
                layId = layTab.Add(layRec);
                tr.AddNewlyCreatedDBObject(layRec, true);
            }
            return layId;
        }
        /// <summary>
        /// Check the layer status
        /// </summary>
        /// <param name="doc">The AutoCAD Active Document</param>
        /// <param name="tr">The active transaction</param>
        /// <returns>The current layer status</returns>
        private LayerStatus CheckLayerStatusTransaction(Document doc, Transaction tr)
        {
            LayerTableRecord layRec = this.Id.GetObject(OpenMode.ForRead) as LayerTableRecord;
            LayerStatus status = new LayerStatus()
            {
                IsFrozen = layRec.IsFrozen,
                IsLocked = layRec.IsLocked,
                IsHidden = layRec.IsHidden,
                IsOff = layRec.IsOff
            };
            return status;
        }
        /// <summary>
        /// Updates the layer status
        /// </summary>
        /// <param name="doc">The AutoCAD Active Document</param>
        /// <param name="tr">The active transaction</param>
        /// <param name="data">The parameters that receive the transaction</param>
        private void UpdateLayerStatusTransaction(Document doc, Transaction tr, params LayerStatus[] data)
        {
            LayerTableRecord layRec = this.Id.GetObject(OpenMode.ForWrite) as LayerTableRecord;
            LayerStatus status = data[0];
            layRec.IsLocked = status.IsLocked;
            layRec.IsFrozen = status.IsFrozen;
            layRec.IsHidden = status.IsHidden;
            layRec.IsOff = status.IsOff;
        }
        /// <summary>
        /// Set the Laye color
        /// </summary>
        /// <param name="doc">The AutoCAD Active Document</param>
        /// <param name="tr">The active transaction</param>
        /// <param name="data">The layer color</param>
        private void SetLayerColorTransaction(Document doc, Transaction tr, params Color[] data)
        {
            LayerTableRecord layRec = this.Id.GetObject(OpenMode.ForWrite) as LayerTableRecord;
            layRec.Color = data[0];
        }
        /// <summary>
        /// Creates a layer or open an existant layer by its name.
        /// And sets the layer color
        /// </summary>
        /// <param name="doc">The AutoCAD Active Document</param>
        /// <param name="tr">The active transaction</param>
        /// <param name="data">The parameters that receive the transaction</param>
        /// <returns>The ObjectId of the transaction</returns>
        private ObjectId CreateLayerWithColorTransaction(Document doc, Transaction tr, params object[] data)
        {
            ObjectId id = CreateLayerTransaction(doc, tr, data[0] as String);
            this.Id = id;
            this.SetLayerColorTransaction(doc, tr, (Color)data[1]);
            return id;
        }
        /// <summary>
        /// Get the name of all layers, and export it on a list
        /// </summary>
        /// <param name="doc">The AutoCAD Active Document</param>
        /// <param name="tr">The active transaction</param>
        /// <param name="data">The parameters that receive the transaction</param>
        /// <returns>The list of the layers names</returns>
        private static List<String> ListLayersTransaction(Document doc, Transaction tr)
        {
            LayerTable layTab = (LayerTable)doc.Database.LayerTableId.GetObject(OpenMode.ForRead);
            return layTab.OfType<ObjectId>().Select<ObjectId, String>(x => ((LayerTableRecord)x.GetObject(OpenMode.ForRead)).Name).ToList();
        }
        /// <summary>
        /// Gets the active layer
        /// </summary>
        /// <param name="doc">The AutoCAD Active Document</param>
        /// <param name="tr">The active transaction</param>
        /// <param name="data">The parameters that receive the transaction</param>
        /// <returns>The active layer as AutoCADLayer</returns>
        private static AutoCADLayer SelectActiveLayerTransaction(Document doc, Transaction tr)
        {
            LayerTableRecord layRec = (LayerTableRecord)doc.Database.Clayer.GetObject(OpenMode.ForRead);
            return new AutoCADLayer(layRec.Name);
        }
        /// <summary>
        /// Set this layer as the current layer
        /// </summary>
        /// <param name="doc">The AutoCAD Active Document</param>
        /// <param name="tr">The active transaction</param>
        /// <param name="data">The parameters that receive the transaction</param>
        private void SetAsCurrentLayerTransaction(Document doc, Transaction tr, params object[] trParameters)
        {
            LayerTableRecord layRec = this.Id.GetObject(OpenMode.ForWrite) as LayerTableRecord;
            LayerStatus status = LayerStatus.EnableStatus;
            layRec.IsLocked = status.IsLocked;
            layRec.IsHidden = status.IsHidden;
            layRec.IsFrozen = status.IsFrozen;
            layRec.IsOff = status.IsOff;
            doc.Database.Clayer = layRec.Id;
        }
        /// <summary>
        /// Move a collection of object ids to the current layer.
        /// </summary>
        /// <param name="doc">The AutoCAD Active Document</param>
        /// <param name="tr">The active transaction</param>
        /// <param name="data">The parameters that receive the transaction</param>
        private void AddToLayerTransaction(Document doc, Transaction tr, params ObjectIdCollection[] data)
        {
            foreach (ObjectId objId in data[0])
                try
                {
                    objId.MoveToLayer(this.Layername, tr);
                }
                catch (Exception)
                {
                    continue;
                }
        }

        /// <summary>
        /// Deletes the elements contained on this layer, for the
        /// current block
        /// </summary>
        /// <param name="doc">The AutoCAD Active Document</param>
        /// <param name="tr">La transacción activa</param>
        private void DeleteMembersLayerTransaction(Document doc, Transaction tr)
        {
            BlockTableRecord currentBlock = doc.Database.CurrentSpaceId.GetObject(OpenMode.ForRead) as BlockTableRecord;
            DBObject obj;
            foreach (ObjectId objId in currentBlock)
                try
                {
                    obj = objId.GetObject(OpenMode.ForRead);
                    if (obj is Entity && (obj as Entity).Layer == this.Layername)
                        Drawer.EraseObject(doc, tr, objId);
                }
                catch (Exception)
                {
                    continue;
                }
        }
        #endregion
    }
}
