using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.Linq;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;

namespace NamelessOld.Libraries.HoukagoTeaTime.Yui
{
    public class Selection : NamelessObject
    {
        /// <summary>
        /// The selection result
        /// </summary>
        public PromptSelectionResult Result;
        /// <summary>
        /// Selecciona el primer object id
        /// </summary>
        public ObjectId First
        {
            get { return Ids.OfType<ObjectId>().FirstOrDefault(); }
        }
        /// <summary>
        /// The list of selected Ids
        /// </summary>
        public ObjectIdCollection Ids
        {
            get
            {
                if (this.Result != null && this.Result.Status == PromptStatus.OK)
                    return new ObjectIdCollection(this.Result.Value.GetObjectIds());
                else
                    return new ObjectIdCollection();
            }
        }
        /// <summary>
        /// The prompt selection status result
        /// </summary>
        public PromptStatus Status
        {
            get
            {
                if (this.Result != null)
                    return this.Result.Status;
                else
                    return PromptStatus.None;
            }
        }
        /// <summary>
        /// Access the AutoCAD current editor
        /// </summary>
        public Editor Ed { get { return AcadApp.DocumentManager.MdiActiveDocument.Editor; } }
        /// <summary>
        /// Select all visible objects on screen
        /// </summary>
        /// <param name="filter">The selection filter.</param>
        /// <returns>True if the selection has more than one entity.</returns>
        public Boolean All(SelectionFilter filter)
        {
            if (filter != null)
                this.Result = this.Ed.SelectAll(filter);
            else
                this.Result = this.Ed.SelectAll();

            return this.Status == PromptStatus.OK && this.Result.Value.Count > 0;
        }
        /// <summary>
        /// Select the last selection
        /// </summary>
        /// <returns>True if the selection has more than one entity.</returns>
        public Boolean Last()
        {
            this.Result = this.Ed.SelectLast();
            return this.Status == PromptStatus.OK && this.Result.Value.Count > 0;
        }
        /// <summary>
        /// Select the previous selection
        /// </summary>
        /// <returns>True if the selection has more than one entity.</returns>
        public Boolean Previous()
        {
            this.Result = this.Ed.SelectPrevious();
            return this.Status == PromptStatus.OK && this.Result.Value.Count > 0;
        }
        /// <summary>
        /// Check if an entity is on the selecction
        /// </summary>
        /// <param name="ent">The entity to be checked</param>
        /// <returns>True if the entity is contained</returns>
        public Boolean Contains(Entity ent)
        {
            return this.Ids.OfType<ObjectId>().Count(x => ent.Id == x) > 0;
        }
        /// <summary>
        /// Check if an object id is on the selecction
        /// </summary>
        /// <param name="id">The id to be checked</param>
        /// <returns>True if the id is contained</returns>
        public Boolean Contains(ObjectId id)
        {
            return this.Ids.OfType<ObjectId>().Count(x => id == x) > 0;
        }

    }
}
