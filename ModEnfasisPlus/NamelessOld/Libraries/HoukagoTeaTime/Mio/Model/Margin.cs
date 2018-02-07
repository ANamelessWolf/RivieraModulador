using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.HoukagoTeaTime.Mio.Model
{
    public struct Margin
    {
        /// <summary>
        /// The Top margin
        /// </summary>
        public Double Top;
        /// <summary>
        /// The Bottom margin
        /// </summary>
        public Double Bottom;
        /// <summary>
        /// The Left margin
        /// </summary>
        public Double Left;
        /// <summary>
        /// The Right margin
        /// </summary>
        public Double Right;

        public Margin(Double left, Double bottom)
        {
            this.Left = left;
            this.Bottom = bottom;
            this.Right = 0;
            this.Top = 0;
        }
    }
}
