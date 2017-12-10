using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DaSoft.Riviera.Modulador.Bordeo.Assets.Constants;
namespace DaSoft.Riviera.Modulador.Bordeo.Controller
{
    public static class BordeoUtils
    {
        public static String[] BordeoDirectionKeys()
        {
            return new String[] 
            {
                KEY_BACK, KEY_FRONT, KEY_LEFT_135, KEY_LEFT_90, KEY_RIGHT_135, KEY_RIGHT_90, KEY_EXTRA
            };
        }
    }
}
