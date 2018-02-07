using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.Yggdrasil.Morrigan
{
    public static class DataStructureUtility
    {
        /// <summary>
        /// Expands an array
        /// </summary>
        /// <typeparam name="T">The array type data</typeparam>
        /// <param name="array">The current array</param>
        /// <param name="expand">The size to expand the array</param>
        /// <returns>Expanded array</returns>
        public static T[] ExpandArray<T>(T[] array, int expand = 1) where T : NamelessObject
        {
            T[] nArr = new T[array.Length + expand];
            for (int i = 0; i < array.Length; i++)
                nArr[i] = array[i];
            return nArr;
        }
    }
}
