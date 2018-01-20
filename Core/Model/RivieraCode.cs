using DaSoft.Riviera.Modulador.Core.Model.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Core.Model
{
    /// <summary>
    /// Defines a Riviera Code
    /// </summary>
    public class RivieraCode : IEnumerable<RivieraAcabado>
    {
        /// <summary>
        /// The design line
        /// </summary>
        public DesignLine Line;
        /// <summary>
        /// The riviera code
        /// </summary>
        public String Code;
        /// <summary>
        /// The selected acabado index
        /// </summary>
        public int SelectedAcabadoIndex;
        /// <summary>
        /// The selected acabado 
        /// </summary>
        public RivieraAcabado SelectedAcabado
        {
            get => this.SelectedAcabadoIndex > 0 && this.Acabados.Count < this.SelectedAcabadoIndex ? 
                this.Acabados[this.SelectedAcabadoIndex] : new RivieraAcabado() { Acabado = "", Description = "", RivCode = this };
        }
        /// <summary>
        /// The riviera code description
        /// </summary>
        public String Description;
        /// <summary>
        /// The riviera block name
        /// </summary>
        public String Block;
        /// <summary>
        /// The riviera element type
        /// </summary>
        public RivieraElementType ElementType;
        /// <summary>
        /// If set to true the panel is double ("Panel Doble") otherwise is normal
        /// </summary>
        public Boolean DoublePanel;
        /// <summary>
        /// The list of acabados asigned to this code
        /// </summary>
        protected List<RivieraAcabado> Acabados;
        /// <summary>
        /// Initializes a new instance of the <see cref="RivieraCode"/> class.
        /// </summary>
        public RivieraCode()
        {
            this.Acabados = new List<RivieraAcabado>();
            this.SelectedAcabadoIndex = 0;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="RivieraCode"/> class.
        /// </summary>
        /// <param name="acabados">La colección de acabados.</param>
        public RivieraCode(params string[] acabados)
        {
            this.Acabados = acabados.Select(x => new RivieraAcabado() { Acabado = x, Description = "Sin descripción para el código " + x, RivCode = this }).ToList();
        }
        /// <summary>
        /// Adds the acabado.
        /// </summary>
        /// <param name="acabado">The riviera acabado code.</param>
        /// <param name="description">The riviera description code.</param>
        public void AddAcabado(String acabado, string description)
        {
            this.Acabados.Add(new RivieraAcabado()
            {
                Acabado = acabado,
                Description = description,
                RivCode = this
            });
        }
        /// <summary>
        /// Devuelve un enumerador que procesa una iteración en la colección.
        /// </summary>
        /// <returns>
        /// Enumerador que se puede utilizar para recorrer en iteración la colección.
        /// </returns>
        public IEnumerator<RivieraAcabado> GetEnumerator()
        {
            return this.Acabados.GetEnumerator();
        }
        /// <summary>
        /// Devuelve un enumerador que recorre en iteración una colección.
        /// </summary>
        /// <returns>
        /// Objeto <see cref="T:System.Collections.IEnumerator" /> que puede usarse para recorrer en iteración la colección.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Acabados.GetEnumerator();
        }
    }
}
