using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Nameless.Libraries.HoukagoTeaTime.Yui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Geometry;
using Nameless.Libraries.HoukagoTeaTime.Ritsu.Shapes2D;
using Nameless.Libraries.HoukagoTeaTime.Ritsu.Utils;
using Nameless.Libraries.HoukagoTeaTime.Tsumugi;
using static DaSoft.Riviera.Modulador.Core.Assets.CONST;
using static DaSoft.Riviera.Modulador.Core.Controller.AutoCADUtils;
using static DaSoft.Riviera.Modulador.Bordeo.Assets.Codes;
using DaSoft.Riviera.Modulador.Core.Model;
using DaSoft.Riviera.Modulador.Bordeo.Model.Enities;
using System.Windows.Media;
using DaSoft.Riviera.Modulador.Bordeo.Model;
using DaSoft.Riviera.Modulador.Core.Runtime;
namespace DaSoft.Riviera.Modulador.Bordeo.Model.Station
{
    public class StationBuilder
    {
        public List<StationToken> Items;
        public List<StationMoldure> Moldures;
        public StationBuilder()
        {
            this.Items = new List<StationToken>();
            this.Moldures = new List<StationMoldure>();
        }


    }
}