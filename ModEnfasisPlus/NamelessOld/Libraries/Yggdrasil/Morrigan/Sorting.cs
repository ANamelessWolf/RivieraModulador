using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.Yggdrasil.Morrigan
{
    public abstract class Sorting<T> : NamelessObject
    {
        /// <summary>
        /// The data to sort
        /// </summary>
        public T[] Data;
        /// <summary>
        /// Creates a class to sort the data
        /// </summary>
        /// <param name="data">The data to be sorted</param>
        public Sorting(T[] data)
        {
            this.Data = data;
        }
        /// <summary>
        /// Order the elements using the insertion sort
        /// algorithm
        /// </summary>
        /// <param name="sort">The sorting order of the array</param>
        public void InsertionSort(SortingOrder sort = SortingOrder.Ascending)
        {
            if (sort == SortingOrder.Ascending)
                InsertionSortAscending();
            else if (sort == SortingOrder.Descending)
                InsertionSortDescending();
        }
        /// <summary>
        /// Sort the data in descending order
        /// </summary>
        private void InsertionSortDescending()
        {
            T tmp;
            int index;
            //Se comienza en el segundo elemento
            for (int pivot = this.Data.Length - 2; pivot >= 0; pivot--)
            {
                //Se guarda el elemento seleccionado como pivote
                tmp = this.Data[pivot];
                //Se inicia la comparación hacia la izquierda del pivote
                index = pivot;
                //Se realiza la comparación mientras exista un elemento a la izquierda
                //y el elemento de la izquierda sea mayor al pivote.
                while (index < this.Data.Length-1 && IsGreater(this.Data[index + 1], tmp))
                {
                    //Se copia el elemento de la izquierda a la derecha
                    this.Data[index] = this.Data[index + 1];
                    //Se mueve indice de comparación a la izquierda
                    index++;
                }
                //Se actualiza el último con el que se compara y se inserta
                //el pivote
                this.Data[index] = tmp;
            }
        }
        /// <summary>
        /// Sort the data in ascending order
        /// </summary>
        private void InsertionSortAscending()
        {
            T tmp;
            int index;
            //Se comienza en el segundo elemento
            for (int pivot = 1; pivot < this.Data.Length; pivot++)
            {
                //Se guarda el elemento seleccionado como pivote
                tmp = this.Data[pivot];
                //Se inicia la comparación hacia la izquierda del pivote
                index = pivot;
                //Se realiza la comparación mientras exista un elemento a la izquierda
                //y el elemento de la izquierda sea mayor al pivote.
                while (index > 0 && IsGreater(this.Data[index - 1], tmp))
                {
                    //Se copia el elemento de la izquierda a la derecha
                    this.Data[index] = this.Data[index - 1];
                    //Se mueve indice de comparación a la izquierda
                    index--;
                }
                //Se actualiza el último con el que se compara y se inserta
                //el pivote
                this.Data[index] = tmp;
            }
        }

        /// <summary>
        /// Check if value 1 is greater than value 2
        /// </summary>
        /// <param name="val1">The first value to be compared</param>
        /// <param name="val2">The second value to be compared</param>
        /// <returns>True if the value 1 is greater than value 2</returns>
        public abstract Boolean IsGreater(T val1, T val2);
        /// <summary>
        /// Check if value 1 is less than value 2
        /// </summary>
        /// <param name="val1">The first value to be compared</param>
        /// <param name="val2">The second value to be compared</param>
        /// <returns>True if the value 1 is less than value 2</returns>
        public abstract Boolean IsLess(T val1, T val2);
        /// <summary>
        /// Check if value 1 is equal to value 2
        /// </summary>
        /// <param name="val1">The first value to be compared</param>
        /// <param name="val2">The second value to be compared</param>
        /// <returns>True if the value 1 is equal to value 2</returns>
        public abstract Boolean AreEqual(T val1, T val2);
        /// <summary>
        /// Imprime la información de manera ordenada
        /// </summary>
        /// <returns>La información ordenada</returns>
        public override string ToString()
        {
            String data = String.Empty;
            foreach (T d in this.Data)
                data += String.Format("{0}, ", d);
            return data.Substring(0, data.Length - 1);
        }
    }
}
