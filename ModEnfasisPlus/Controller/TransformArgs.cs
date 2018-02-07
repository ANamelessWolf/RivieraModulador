using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System.Windows;

namespace DaSoft.Riviera.OldModulador.Controller
{
    public class TransformArgs : RoutedEventArgs
    {
        /// <summary>
        /// La matriz de la transformada aplicada
        /// </summary>
        public Matrix3d Matrix;
        /// <summary>
        /// La transacción activa
        /// </summary>
        public Transaction Tr;
        /// <summary>
        /// Crea un argumento de transformación
        /// </summary>
        /// <param name="matrix">La matriz aplicada</param>
        /// <param name="tr">la transacción activa</param>
        public TransformArgs(Matrix3d matrix, Transaction tr) : base()
        {
            this.Matrix = matrix;
            this.Tr = tr;
        }
    }
}
