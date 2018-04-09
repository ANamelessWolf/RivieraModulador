using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DaSoft.Riviera.Modulador.Bordeo.Controller
{
    public class PropertyChangedArgs : SelectionChangedEventArgs
    {
        /// <summary>
        /// The property name
        /// </summary>
        public String PropertyName;
        /// <summary>
        /// The property value
        /// </summary>
        public Object PropertyValue;
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangedArgs"/> class.
        /// </summary>
        /// <param name="propName">Name of the property.</param>
        /// <param name="propValue">The property value.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="removedItems">The removed items.</param>
        /// <param name="addedItems">The added items.</param>
        public PropertyChangedArgs(String propName, Object propValue, RoutedEvent id, IList removedItems, IList addedItems) :
            base(id, removedItems, addedItems)
        {
            this.PropertyName = propName;
            this.PropertyValue = propValue;
        }
    }
}
