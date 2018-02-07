using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.OldModulador.Model
{
    public struct TreatmentUnion
    {
        /// <summary>
        /// Define el nombre de la unión
        /// </summary>
        public String Union;
        /// <summary>
        /// El tratamiento aplicar en la unión
        /// </summary>
        public TratamientoUnion Treatment;
        /// <summary>
        /// La lista de entidades
        /// </summary>
        public List<long> Entities;
    }
}
