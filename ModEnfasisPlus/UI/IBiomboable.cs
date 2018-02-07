using DaSoft.Riviera.OldModulador.Model;
using System;

namespace DaSoft.Riviera.OldModulador.UI
{
    public interface IBiomboable
    {
        PanelStatus Status { get; set; }
        Double Width { get; set; }
        Double Height { get; set; }
        String Text { get; set; }
        String Acabado { get; set; }
        PanelSide Side { get; set; }
        string PanelData { get; }
        Object Data { get; set; }
        void SetAcabado();
    }
}
