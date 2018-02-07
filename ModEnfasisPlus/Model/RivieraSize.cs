using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.OldModulador.Model
{
    public class RivieraSize
    {
        public Double Frente;
        public Double Alto;
        public Double Ancho;
        public String Code;
        public override string ToString()
        {
            return String.Format("{0}X{1}X{2}", this.Frente, this.Alto, this.Ancho);
        }
    }
}
