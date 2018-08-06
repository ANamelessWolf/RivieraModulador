using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.Modulador.Core.Model;
using Nameless.Libraries.HoukagoTeaTime.Mio.Utils;
using Nameless.Libraries.HoukagoTeaTime.Tsumugi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DaSoft.Riviera.Modulador.Core.Controller.AutoCADUtils;
using static DaSoft.Riviera.Modulador.Core.Assets.CONST;
namespace DaSoft.Riviera.Modulador.Core.Controller
{
    public abstract class RivieraLoader
    {
        /// <summary>
        /// The dictionary manager
        /// </summary>
        public ExtensionDictionaryManager DManager;
        /// <summary>
        /// Gets or sets the bordeo codes.
        /// </summary>
        /// <value>
        /// The bordeo codes.
        /// </value>
        public abstract String[] LineCodes { get; }
        /// <summary>
        /// Initializes a new instance of the <see cref="BordeoLoader"/> class.
        /// </summary>
        /// <param name="dManager">The dictionary manager.</param>
        public RivieraLoader(ExtensionDictionaryManager dManager)
        {
            this.DManager = dManager;
        }
        /// <summary>
        /// Determines whether [is code of the design line] [the specified code].
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>
        ///   <c>true</c> if [is a code from the design line] [the specified code]; otherwise, <c>false</c>.
        /// </returns>
        public Boolean IsDesignLineCode(String code)
        {
            return this.LineCodes.Contains(code);
        }
        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public void GetLocation(Transaction tr, out Point3d start, out Point3d end)
        {
            String[] location = this.DManager.GetXRecord(KEY_LOCATION, tr).GetDataAsString(tr);
            var coords = location[0].Replace(", ","@").Split('@');
            start = new Point3d(double.Parse(coords[0]), double.Parse(coords[1]), 0);
            coords = location[1].Replace(", ", "@").Split('@');
            end = new Point3d(double.Parse(coords[0]), double.Parse(coords[1]), 0);
        }
        /// <summary>
        /// Sets the children.
        /// </summary>
        /// <param name="tr">The tr.</param>
        /// <param name="children">The children.</param>
        public void SetChildren(Transaction tr, ref Dictionary<string, long> children, params string[] dirs)
        {
            Xrecord child;
            string handleStr;
            long handleVal;
            foreach (string key in dirs)
                if (this.DManager.TryGetXRecord(key, out child, tr))
                {
                    handleStr = child.GetDataAsString(tr)[0];
                    handleVal = long.Parse(handleStr);
                    if (!children.ContainsKey(key))
                        children.Add(key, handleVal);
                }
        }
        /// <summary>
        /// Loads the connections.
        /// </summary>
        /// <param name="tr">The tr.</param>
        /// <param name="obj">The object.</param>
        /// <param name="connKeys">The connection keys.</param>
        public void LoadConnections(Transaction tr, RivieraObject obj, params String[] connKeys)
        {
            Xrecord conn;
            string[] data;
            foreach (var key in connKeys)
                if (this.DManager.TryGetXRecord("Des_"+key, out conn, tr))
                {
                    data = conn.GetDataAsString(tr);
                    obj.Description.Connections.Add(new RivieraConnection() { Key = key, Direction = data[0], BlockName = data[1] });
                }
        }
        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <typeparam name="T">The data type</typeparam>
        /// <param name="tr">The transaction.</param>
        /// <returns>The Riviera Object</returns>
        public T GetEntity<T>(Transaction tr) where T : Entity
        {
            string handleStr = this.DManager.GetXRecord(KEY_ID, tr).GetDataAsString(tr).FirstOrDefault();
            long handleVal = long.Parse(handleStr);
            return handleVal.GetId().Open<T>(tr);
        }
        /// <summary>
        /// Gets the parent
        /// </summary>
        /// <param name="tr">The transaction.</param>
        /// <returns>The Riviera Object</returns>
        public long GetParent(Transaction tr)
        {
            string handleStr = this.DManager.GetXRecord(KEY_ID, tr).GetDataAsString(tr).FirstOrDefault();
            return long.Parse(handleStr);
        }
        /// <summary>
        /// Loads the geometry.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="tr">The tr.</param>
        public ObjectIdCollection LoadGeometry(Transaction tr)
        {
            ObjectIdCollection ids = new ObjectIdCollection();
            string[] geometry = this.DManager.GetXRecord(KEY_GEOMETRY, tr).GetDataAsString(tr);
            long handle;
            foreach (string handleStr in geometry)
            {
                try
                {
                    handle = long.Parse(handleStr);
                    ids.Add(handle.GetId());
                }
                catch (Exception) { }
            }
            return ids;
        }
        /// <summary>
        /// Loads the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="tr">The tr.</param>
        public void Load(ref RivieraObject obj, Transaction tr)
        {
            this.SetChildren(tr, ref obj.Children, KEY_DIR_BACK, KEY_DIR_FRONT);
            obj.Parent = this.GetParent(tr);
        }
    }
}
