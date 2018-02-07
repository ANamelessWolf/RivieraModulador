using DaSoft.Riviera.OldModulador.Controller;
using DaSoft.Riviera.OldModulador.Model.Delta;
using System;

namespace DaSoft.Riviera.OldModulador.Model
{
    public class MamparaAngleAlto
    {
        public Double Alto;
        public Double Angle;
        public Mampara Mampara;
        public Double AngleDegree
        {
            get
            {
                return this.Angle.ToDegree().To360Degree();
            }
        }
        public MamparaAngleAlto(JointObject joint, Mampara mam)
        {
            this.Alto = mam.Alto;
            this.Angle = mam.GetAngle(joint);
            this.Mampara = mam;
        }

        public override string ToString()
        {
            return String.Format("{0}, Alto: {1}m, Angle: {2}", Mampara.Code, Mampara.Alto, AngleDegree);
        }
    }
}
