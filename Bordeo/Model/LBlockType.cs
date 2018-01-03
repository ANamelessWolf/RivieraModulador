using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Bordeo.Model
{
    public enum LBlockType
    {
        NONE = -1,
        RIGHT_START_MIN_SIZE = 0,
        RIGHT_START_MAX_SIZE = 1,
        LEFT_START_MIN_SIZE = 2,
        LEFT_START_MAX_SIZE = 3,
        RIGHT_SAME_SIZE = 4,
        LEFT_SAME_SIZE = 5,
        CONTENT = 6,
        VARIANT_CONTENT = 7
    }
}
