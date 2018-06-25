using DaSoft.Riviera.Modulador.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Bordeo.Model.Station
{
   public class StationToken
    {
        /// <summary>
        /// El tipo de unión al que pertenece el stack
        /// </summary>
        public StationUnion Union;
        /// <summary>
        /// La dirección que tiene el stack
        /// </summary>
        public StationDirection Direction;
        /// <summary>
        /// The stack
        /// </summary>
        public RivieraObject Stack;
        
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var token = this.Union.ToString();
            if (this.Direction == StationDirection.End)
                token += "f";
            else if (this.Direction == StationDirection.Initial)
                token += "0";
            else if (this.Direction == StationDirection.Invert)
                token += "-";
            else if (this.Direction == StationDirection.Same)
                token += "+";
            return token;
        }
        /// <summary>
        /// Station token I0
        /// </summary>
        /// <value>
        /// The token station
        /// </value>
        public static StationToken Istart => new StationToken() { Direction= StationDirection.Initial, Union= StationUnion.I };
        /// <summary>
        /// Station token If
        /// </summary>
        /// <value>
        /// The token station
        /// </value>
        public static StationToken Ifinal => new StationToken() { Direction = StationDirection.End, Union = StationUnion.I };
        /// <summary>
        /// Station token I+
        /// </summary>
        /// <value>
        /// The token station
        /// </value>
        public static StationToken Iplus => new StationToken() { Direction = StationDirection.Same, Union = StationUnion.I };
        /// <summary>
        /// Station token I-
        /// </summary>
        /// <value>
        /// The token station
        /// </value>
        public static StationToken Iless => new StationToken() { Direction = StationDirection.Invert, Union = StationUnion.I };
        /// <summary>
        /// Station token L900
        /// </summary>
        /// <value>
        /// The token station
        /// </value>
        public static StationToken L90start => new StationToken() { Direction = StationDirection.Initial, Union = StationUnion.L90 };
        /// <summary>
        /// Station token L90f
        /// </summary>
        /// <value>
        /// The token station
        /// </value>
        public static StationToken L90final => new StationToken() { Direction = StationDirection.End, Union = StationUnion.L90 };
        /// <summary>
        /// Station token L90+
        /// </summary>
        /// <value>
        /// The token station
        /// </value>
        public static StationToken L90plus => new StationToken() { Direction = StationDirection.Same, Union = StationUnion.L90 };
        /// <summary>
        /// Station token L90-
        /// </summary>
        /// <value>
        /// The token station
        /// </value>
        public static StationToken L90less => new StationToken() { Direction = StationDirection.Invert, Union = StationUnion.L90 };
        /// <summary>
        /// Station token L1350
        /// </summary>
        /// <value>
        /// The token station
        /// </value>
        public static StationToken L135start => new StationToken() { Direction = StationDirection.Initial, Union = StationUnion.L135 };
        /// <summary>
        /// Station token L135f
        /// </summary>
        /// <value>
        /// The token station
        /// </value>
        public static StationToken L135final => new StationToken() { Direction = StationDirection.End, Union = StationUnion.L135 };
        /// <summary>
        /// Station token L135+
        /// </summary>
        /// <value>
        /// The token station
        /// </value>
        public static StationToken L135plus => new StationToken() { Direction = StationDirection.Same, Union = StationUnion.L135 };
        /// <summary>
        /// Station token L135-
        /// </summary>
        /// <value>
        /// The token station
        /// </value>
        public static StationToken L135less => new StationToken() { Direction = StationDirection.Invert, Union = StationUnion.L135 };
    }
}
