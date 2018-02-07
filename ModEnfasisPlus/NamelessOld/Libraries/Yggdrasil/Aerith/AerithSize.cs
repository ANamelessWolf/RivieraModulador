using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.Text;

namespace NamelessOld.Libraries.Yggdrasil.Aerith
{
    public class AerithSize : NamelessObject
    {
        const long B = 1;
        const long KB = 1024;
        const long MB = 1048576;
        const long GB = 1073741824;
        const long TB = 1099511627776;
        #region Properties
        /// <summary>
        /// The total size in Kylobytes.
        /// </summary>
        public double TotalKiloBytes { get { return GetTotalSize(KB); } }
        /// <summary>
        /// The total size in Megabytes.
        /// </summary>
        public double TotalMegaBytes { get { return GetTotalSize(MB); } }
        /// <summary>
        /// The total size in Gigabytes.
        /// </summary>
        public double TotalGigaBytes { get { return GetTotalSize(GB); } }
        /// <summary>
        /// The total size in Terabytes.
        /// </summary>
        public double TotalTeraBytes { get { return GetTotalSize(TB); } }
        /// <summary>
        /// The length file size 
        /// </summary>
        long[] Size;
        /// <summary>
        /// The bytes in the size
        /// </summary>
        public long Bytes { get { return Size[0]; } }
        /// <summary>
        /// The Kilo bytes in the size
        /// </summary>
        public long KyloBytes { get { return Size[1]; } }
        /// <summary>
        /// The Mega bytes in the size
        /// </summary>
        public long MegaBytes { get { return Size[2]; } }
        /// <summary>
        /// The Giga bytes in the size
        /// </summary>
        public long GigaBytes { get { return Size[3]; } }
        /// <summary>
        /// The Tera bytes in the size
        /// </summary>
        public long TeraBytes { get { return Size[4]; } }
        /// <summary>
        /// The size of the bytes
        /// </summary>
        public long Length { get { return bytes; } set { this.Refresh(value); } }
        long bytes;
        #endregion
        /// <summary>
        /// Creates a new Aerith size
        /// </summary>
        /// <param name="byteSize">The byte size</param>
        public AerithSize(long byteSize)
        {
            this.Length = byteSize;
        }
        /// <summary>
        /// Creates a new human readable size with a Kylobyte value
        /// </summary>
        /// <param name="size">The size in Kylobytes</param>
        /// <returns>The Aerith size</returns>
        public static AerithSize FromKiloBytes(double size)
        {
            return new AerithSize((long)(size * KB));
        }
        /// <summary>
        /// Creates a new human readable size with a Megabytes value
        /// </summary>
        /// <param name="size">The size in Megabytes</param>
        /// <returns>The Aerith size</returns>
        public static AerithSize FromMegaBytes(double size)
        {
            return new AerithSize((long)(size * MB));
        }
        /// <summary>
        /// Creates a new human readable size with a Gigabyte value
        /// </summary>
        /// <param name="size">The size in Gigabytes</param>
        /// <returns>The Aerith size</returns>
        public static AerithSize FromGigaBytes(double size)
        {
            return new AerithSize((long)(size * GB));
        }
        /// <summary>
        /// Creates a new human readable size with a Terabyte value
        /// </summary>
        /// <param name="size">The size in Terabytes</param>
        /// <returns>The Aerith size</returns>
        public static AerithSize FromTeraBytes(double size)
        {
            return new AerithSize((long)(size * TB));
        }
        /// <summary>
        /// Print the Aerith size in human readable
        /// </summary>
        /// <returns>The size value</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            String[] formats = new String[] { "B", "KB", "MB", "GB", "TB" };

            for (int i = formats.Length - 1; i > 0; i--)
                if (this.Size[i] != 0)
                {
                    sb.Append(this.Size[i]);
                    sb.Append(" " + formats[i] + ", ");
                }
            if (sb.ToString().Length - 2 > 0)
                return sb.ToString().Substring(0, sb.ToString().Length - 2);
            else
                return "0 B";
        }
        /// <summary>
        /// Refresh the current size
        /// </summary>
        /// <param name="value">The file size value</param>
        void Refresh(long value)
        {
            this.Size = new long[5];
            long[] formats = new long[] { B, KB, MB, GB, TB };
            for (int i = (formats.Length - 1); i > 0; i--)
                if (value >= formats[i])
                {
                    this.Size[i] = value / formats[i];
                    value = value - this.Size[i] * formats[i];
                }
                else
                    this.Size[i] = 0;
            this.bytes = value;
        }
        /// <summary>
        /// Gets the total size in the specific Formatter.
        /// </summary>
        /// <param name="unitSize">The selected units.</param>
        /// <returns>The total size</returns>
        double GetTotalSize(long unitSize)
        {
            if (this.Length >= unitSize)
                return (double)this.Length / (double)unitSize;
            else
                return 0;
        }
    }
}
