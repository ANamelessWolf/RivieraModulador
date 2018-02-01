using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.Modulador.Core.Controller;
using DaSoft.Riviera.Modulador.Core.Model.DB;
using DaSoft.Riviera.Modulador.Core.Runtime;
using Nameless.Libraries.HoukagoTeaTime.Mio.Utils;
using Nameless.Libraries.HoukagoTeaTime.Ritsu.Utils;
using Nameless.Libraries.HoukagoTeaTime.Tsumugi;
using Nameless.Libraries.HoukagoTeaTime.Yui;
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
        /// The Ids of the entities that defines the Riviera Object geometry
        /// </summary>
        public ObjectIdCollection Ids;
        /// <summary>
        /// Gets the identifier for the CAD Geometry.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public ObjectId Id
        {
            get { return this.CADGeometry.Id; }
        }
        /// <summary>
        /// Gets the handle for the CAD Geometry;
        /// </summary>
        /// <value>
        /// The handle.
        /// </value>
        public Handle Handle
        {
            get { return this.CADGeometry.Handle; }
        }
        /// <summary>
        /// Gets a value indicating whether this instance is root.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is root; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsRoot
        {
            get { return this.Parent == 0; }
        }
        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <returns></returns>
        public RivieraObject GetChildren(List<RivieraObject> objects, ArrowDirection direction)
        {
            string key = direction.GetArrowDirectionName();
            if (this.Children.ContainsKey(key) && this.Children[key] > 0)
                return objects.FirstOrDefault(x => x.Handle.Value == this.Children[key]);
            else
                return null;
        }
        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <returns></returns>
        public RivieraObject GetParent(List<RivieraObject> objects)
        {
            if (this.Parent > 0)
                return objects.FirstOrDefault(x => x.Handle.Value == this.Parent);
            else
                return null;
        }
        /// <summary>
        /// Gets the geometry that stores the riviera extended data.
        /// </summary>
        /// <value>
        /// The CAD geometry
        /// </value>
        public abstract Entity CADGeometry { get; }
        /// <summary>
        /// The riviera code
        /// </summary>
        public RivieraCode Code;
        /// <summary>
        /// The Riviera object size
        /// </summary>
        public RivieraMeasure Size;
        /// <summary>
        /// The riviera object start point
        /// </summary>
        public Point2d Start;
        /// <summary>
        /// The riviera object end point
        /// </summary>
        public Point2d End => GetEndPoint();
        /// <summary>
        /// Defines the riviera object direction
        /// </summary>
        public Vector2d Direction;
        /// <summary>
        /// Gets the riviera object end point.
        /// </summary>
        /// <returns>The riviera end point</returns>
        protected abstract Point2d GetEndPoint();
        /// <summary>
        /// Gets the angle for the inserted riviera object.
        /// </summary>
        /// <value>
        /// The riviera object angle direction.
        /// </value>
        public double Angle { get { return this.Direction.Angle; } }
        /// <summary>
        /// Gets a value indicating whether this instance has children.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has children; otherwise, <c>false</c>.
        /// </value>
        public bool HasChildren
        {
            get
            {
                return this.Children.Values.Where(x => x != 0).Count() > 0;
            }
        }
        /// <summary>
        /// Gets the riviera object available record keys.
        /// </summary>
        /// <value>
        /// The dictionary XRecord Keys.
        /// </value>
        public abstract String[] Keys { get; }
        /// <summary>
        /// The riviera object parent handle value
        /// </summary>
        public long Parent;
        /// <summary>
        /// The riviera object children handle value
        /// Handles are asociated to a Key direction.
        /// </summary>
        public Dictionary<String, long> Children;
        /// <summary>
        /// The riviera description
        /// </summary>
        public RivieraDescription Description;
        /// <summary>
        /// Draws this instance
        /// </summary>
        /// <param name="tr">The active transaction.</param>
        protected abstract ObjectIdCollection DrawContent(Transaction tr);
        /// <summary>
        /// Draws this instance
        /// </summary>
        /// <param name="tr">The active transaction.</param>
        public void Draw(Transaction tr)
        {
            try
            {
                ObjectIdCollection ids = DrawContent(tr);
                foreach (ObjectId id in ids)
                    if (!this.Ids.Contains(id))
                        this.Ids.Add(id);
                //Se eliminan los bloques borrados.
                var erased = this.Ids.OfType<ObjectId>().Where(x => x.IsErased);
                foreach (ObjectId id in erased)
                    this.Ids.Remove(id);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        /// <summary>
        /// Regens this instance geometry <see cref="CADGeometry"/>.
        /// </summary>
        public abstract void Regen();
        /// <summary>
        /// Initializes a new instance of the <see cref="RivieraObject"/> class.
        /// </summary>
        /// <param name="code">The riviera code.</param>
        /// <param name="size">The riviera element size.</param>
        /// <param name="start">The riviera start point or insertion point.</param>
        public RivieraObject(RivieraCode code, RivieraMeasure size, Point3d start)
        {
            this.Ids = new ObjectIdCollection();
            this.Children = new Dictionary<string, long>();
            this.Code = code;
            this.Size = size;
            this.Start = start.ToPoint2d();
            this.Description = new RivieraDescription();
            this.Description.ClassName = this.GetType().FullName;
        }
        /// <summary>
        /// Sets the default acabado.
        /// </summary>
        /// <param name="index">The riviera selected index.</param>
        public void SetAcabado(int index)
        {
            this.Code.SelectedAcabadoIndex = index;
        }
        /// <summary>
        /// Sets the default acabado.
        /// </summary>
        /// <param name="rivieraAcabado">The riviera acabado.</param>
        public void SetAcabado(RivieraAcabado rivieraAcabado)
        {
            this.Code.SelectedAcabadoIndex = this.Code.IndexOf(rivieraAcabado);
        }
        /// <summary>
        /// Sets the default acabado.
        /// </summary>
        /// <param name="rivieraAcabado">The riviera acabado.</param>
        public void SetAcabado(string acabado)
        {
            RivieraAcabado rivieraAcabado = this.Code.FirstOrDefault(x => x.Acabado == acabado);
            this.Code.SelectedAcabadoIndex = this.Code.IndexOf(rivieraAcabado);
        }
        /// <summary>
        /// Saves the riviera object.
        /// </summary>
        /// <param name="tr">The active transaction.</param>
        /// <param name="dMan">The extension dictionary manager</param>
        public virtual void Save(Transaction tr)
        {
            var dMan = new ExtensionDictionaryManager(this.CADGeometry.Id, tr);
            dMan.Set(tr, KEY_ID, this.Handle.Value.ToString());
            List<String> handles = new List<string>();
            foreach (ObjectId id in this.Ids)
                handles.Add(id.Handle.Value.ToString());
            dMan.Set(tr, KEY_GEOMETRY, handles.ToArray());
            foreach (var child in Children.Where(x => x.Value > 0))
                dMan.Set(tr, child.Key, child.Value.ToString());
            dMan.Set(tr, KEY_PARENT, this.Parent.ToString());
            foreach (var conn in this.Description.Connections)
                dMan.Set(tr, conn.Key, conn.Direction, conn.BlockName);
        }
        /// <summary>
        /// Saves the riviera object.
        /// </summary>
        /// <param name="tr">The active transaction.</param>
        /// <param name="dMan">The extension dictionary manager</param>
        public virtual void Load(Transaction tr)
        {
            var dMan = new ExtensionDictionaryManager(this.CADGeometry.Id, tr);
            this.Children = new Dictionary<string, long>();
            Xrecord field;
            long id;
            foreach (var key in this.Keys)
                if (dMan.TryGetXRecord(key, out field, tr) && long.TryParse(field.GetDataAsString(tr).FirstOrDefault(), out id))
                    this.Children.Add(key, id);
        }
        /// <summary>
        /// Connects the specified object to this instance
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <param name="stack">The new bject to connect.</param>
        public virtual void Connect(ArrowDirection direction, RivieraObject newObject)
        {
            newObject.Parent = this.Handle.Value;
            String key = direction.GetArrowDirectionName();
            if (this.Children.ContainsKey(key))
                this.Children[key] = newObject.Handle.Value;
            else
                this.Children.Add(key, newObject.Handle.Value);
            var db = App.Riviera.Database.Objects;
            if (db.FirstOrDefault(x => x.Handle.Value == newObject.Handle.Value) == null)
                db.Add(newObject);
            this.Description.Connections.Add(new RivieraConnection() { BlockName = newObject.Code.Code, Direction = key, Key = key });
            newObject.Description.Connections.Add(new RivieraConnection() { BlockName = this.Code.Code, Direction = key, Key = KEY_PARENT });
        }

        /// <summary>
        /// Erase this instance drew geometry
        /// </summary>
        public void Erase(Transaction tr)
        {
            this.Ids.Erase(tr);
            this.Ids.Clear();
            if (this.CADGeometry.Id.IsValid)
            {
                var obj = this.CADGeometry.Id.GetObject(OpenMode.ForWrite);
                obj.Erase(tr);
            }
        }
        /// <summary>
        /// Erase this instance drew geometry
        /// </summary>
        public void Delete(Transaction tr)
        {
            App.Riviera.Database.Objects.Remove(this);
            this.Erase(tr);
        }
        /// <summary>
        /// Zooms at the specified zoom.
        /// </summary>
        /// <param name="scale">The zoom scale.</param>
        public void ZoomAt(double scale)
        {
            Point3d min, max;
            this.CADGeometry.GetGeometricExtents(out min, out max);
            ZoomWindow zoom = new ZoomWindow(min, max);
            zoom.SetView(scale);
        }
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.Code.Code);
            sb.Append(" ");
            foreach (var child in this.Children.Where(x => x.Value > 0))
                sb.Append(String.Format("{0}: {1}, ", child.Key, child.Value));
            if (this.Children.Count(x => x.Value > 0) > 0)
                return sb.ToString().Substring(0, sb.ToString().Length - 2);
            else
            {
                sb.Append("Sin conexión.");
                return sb.ToString();
            }
        }
    }
}
