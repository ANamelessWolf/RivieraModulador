using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using NamelessOld.Libraries.HoukagoTeaTime.Mio;
using System;
using System.Collections.Generic;

namespace NamelessOld.Libraries.HoukagoTeaTime.Yui
{
    public class SelectionFilterBuilder
    {


        /// <summary>
        /// The collection of allowed layers
        /// </summary>
        public String[] Layers;
        /// <summary>
        /// The collection of allowed types
        /// </summary>
        public Type[] Types;
        /// <summary>
        /// Create a filter that only allows certain types of
        /// entities
        /// </summary>
        /// <param name="types">The allowed types</param>
        public SelectionFilterBuilder(params Type[] types)
        {
            this.Layers = new String[0];
            this.Types = types;
        }
        /// <summary>
        /// Set the allowed types to the filter
        /// </summary>
        /// <param name="types">The collection of allowed types</param>
        public void SetAllowedTypes(params Type[] types)
        {
            this.Types = types;
        }
        /// <summary>
        /// Set the allowed types to the filter
        /// </summary>
        /// <param name="layers">The collection of allowed layers</param>
        public void SetAllowedLayers(params String[] layers)
        {
            this.Layers = layers;
        }
        /// <summary>
        /// Gets the selection filter
        /// </summary>
        public SelectionFilter Filter
        {
            get
            {
                List<TypedValue> tpVals = new List<TypedValue>();
                tpVals.Add(new TypedValue((int)DxfCode.Operator, "<or"));
                foreach (Type type in this.Types)
                    tpVals.Add(type.GetDxfName());
                tpVals.Add(new TypedValue((int)DxfCode.Operator, "or>"));
                return new SelectionFilter(tpVals.ToArray());
            }
        }


    }
}
