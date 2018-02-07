using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.HoukagoTeaTime.MugiChan
{
    public interface IHoukagoIdentity
    {
        /// <summary>
        /// The Id of an entity
        /// </summary>
        ObjectId Id { get; }
        /// <summary>
        /// The name of the element type
        /// </summary>
        String ItemName { get; }
        /// <summary>
        /// The name of DBDictionary Keys
        /// </summary>
        String[] Keys { get; }
    }
}
