using System;
using System.Text;
using System.Collections.Generic;

namespace KSynthLib.K5000
{
    public class PatchName
    {
        public static readonly int Length = 8;

        private string _name;

        public string Value
        {
            get => _name.PadRight(Length, ' ');
            set => _name = value.PadRight(Length, ' ');
        }

        public PatchName(string s)
        {
            this.Value = s;
        }

        public PatchName(byte[] data, int offset = 0)
        {
            byte[] bytes =
            {
                data[offset],
                data[offset + 1],
                data[offset + 2],
                data[offset + 3],
                data[offset + 4],
                data[offset + 5],
                data[offset + 6],
                data[offset + 7],
            };

            this.Value = Encoding.ASCII.GetString(bytes);
        }

        public byte[] ToBytes()
        {
            var bytes = new List<byte>();

            var charArray = this.Value.ToCharArray();
            for (var i = 0; i < charArray.Length; i++)
            {
                char ch = charArray[i];
                byte b = (byte)ch;
                if (ch == '\u2192') // right arrow
                {
                    b = 0x7e;
                }
                else if (ch == '\u2190')  // left arrow
                {
                    b = 0x7f;
                }
                else if (ch == '\u00a5')  // yen sign
                {
                    b = 0x5c;
                }
                bytes.Add(b);
            }

            return bytes.ToArray();
        }
    }

    /// <summary>
    /// Abstract base class providing subclasses customization points
    /// for computing checksums and collecting data for building System Exclusive
    /// messages.
    /// </summary>
    public abstract class Patch
    {
        protected byte _checksum;

        /// <value>
        /// Virtual property to compute a checksum for patch data.
        /// </value>
        /// <remarks>
        /// This property should be overridden by any subclass.
        /// </remarks>
        public virtual byte Checksum
        {
            get
            {
                byte[] data = CollectData();
                byte sum = 0;
                foreach (byte b in data)
                {
                    sum += b;
                }
                sum += 0xA5;
                return sum;
            }

            set => _checksum = value;
        }

        public Patch()
        {
            _checksum = 0x00;
        }

        protected abstract byte[] CollectData();

        /// <summary>
        /// Collects the data that makes up this patch in System Exclusive format.
        /// </summary>
        /// <returns>The SysEx data bytes.</returns>
        public byte[] ToData()
        {
            var allData = new List<byte>();

            byte[] data = CollectData();
            allData.AddRange(data);
            allData.Add(this.Checksum);

            return allData.ToArray();
        }
    }
}