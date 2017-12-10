using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.Modulador.Core.Controller;
using Nameless.Libraries.HoukagoTeaTime.Tsumugi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DaSoft.Riviera.Modulador.Core.Controller.AutoCADUtils;
namespace DaSoft.Riviera.Modulador.Core.Model
{
    public abstract class RivieraObject
    {
        /// <summary>
        /// The geometry collection ids
        /// </summary>
        public ObjectIdCollection Ids;
        /// <summary>
        /// Gets the geometry that defines the riviera object data.
        /// </summary>
        /// <value>
        /// The geometry.
        /// </value>
        protected abstract Entity Geometry { get; }
        /// <summary>
        /// Defines the Handle
        /// </summary>
        public Handle Handle
        {
            get { return this.Geometry.Handle; }
        }
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public ObjectId Id
        {
            get { return this.Geometry.Id; }
        }
        /// <summary>
        /// The riviera code
        /// </summary>
        public RivieraCode Code;
        /// <summary>
        /// The measure size
        /// </summary>
        protected abstract RivieraMeasure Size { get; }
        /// <summary>
        /// The Riviera object start point
        /// </summary>
        public abstract Point2d Start { get; }
        /// <summary>
        /// The Riviera object end point
        /// </summary>
        public abstract Point2d End { get; }
        /// <summary>
        /// Define la dirección del rectangulo.
        /// </summary>
        public virtual Vector2d Direction
        {
            get
            {
                double x = this.End.X - this.Start.X,
                       y = this.End.Y - this.Start.Y;
                return new Vector2d(x, y);
            }
        }
        /// <summary>
        /// Gets the riviera object available direction keys.
        /// </summary>
        /// <value>
        /// The dictionary keys.
        /// </value>
        public abstract String[] DirectionKeys { get; }
        /// <summary>
        /// Gets the parent Handel Id
        /// </summary>
        public long Parent;
        /// <summary>
        /// Gets the object children
        /// </summary>
        public Dictionary<String, long> Children;
        /// <summary>
        /// Saves the riviera object.
        /// </summary>
        /// <param name="tr">The active transaction.</param>
        /// <param name="dMan">The extension dictionary manager</param>
        public virtual void Save(Transaction tr)
        {
            var dMan = new ExtensionDictionaryManager(this.Geometry.Id, tr);
            dMan.Set(tr, KEY_ID, this.Handle.Value.ToString());
            foreach (var child in Children)
                dMan.Set(tr, child.Key, child.Value.ToString());
        }
        /// <summary>
        /// Saves the riviera object.
        /// </summary>
        /// <param name="tr">The active transaction.</param>
        /// <param name="dMan">The extension dictionary manager</param>
        public virtual void Load(Transaction tr)
        {
            var dMan = new ExtensionDictionaryManager(this.Geometry.Id, tr);
            this.Children = new Dictionary<string, long>();
            Xrecord field;
            long id;
            foreach (var key in this.DirectionKeys)
                if (dMan.TryGetXRecord(key, out field, tr) && long.TryParse(field.GetDataAsString(tr).FirstOrDefault(), out id))
                    this.Children.Add(key, id);
        }
    }
}
