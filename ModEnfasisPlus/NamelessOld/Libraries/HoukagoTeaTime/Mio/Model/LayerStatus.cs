using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.Text;

namespace NamelessOld.Libraries.HoukagoTeaTime.Mio.Model
{
    public class LayerStatus : NamelessObject
    {
        /// <summary>
        /// Get the status of an enable layer
        /// </summary>
        public static LayerStatus EnableStatus { get { return new LayerStatus { IsFrozen = false, IsHidden = false, IsLocked = false, IsOff = false }; } }
        /// <summary>
        /// Get the status of disable layer
        /// </summary>
        public static LayerStatus DisableStatus { get { return new LayerStatus { IsFrozen = true, IsHidden = false, IsLocked = false, IsOff = false }; } }
        /// <summary>
        /// Get the status of an Locked layer
        /// </summary>
        public static LayerStatus LockStatus { get { return new LayerStatus { IsFrozen = false, IsHidden = false, IsLocked = true, IsOff = false }; } }
        /// <summary>
        /// This flag show the layer frozen status
        /// </summary>
        public Boolean IsFrozen;
        /// <summary>
        /// This flag show the layer hidden status
        /// </summary>
        public Boolean IsHidden;
        /// <summary>
        /// This flag show the layer locked status
        /// </summary>
        public Boolean IsLocked;
        /// <summary>
        /// This flag show the layer off status
        /// </summary>
        public Boolean IsOff;
        /// <summary>
        /// Layer Status
        /// </summary>
        /// <returns>The layer status string result</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Frozen: ");
            sb.Append(IsFrozen);
            sb.Append(", Hidden: ");
            sb.Append(IsHidden);
            sb.Append(", Locked: ");
            sb.Append(IsLocked);
            sb.Append(", Off: ");
            sb.Append(IsOff);
            return sb.ToString();
        }
    }
}
