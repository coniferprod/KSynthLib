using System;
using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K5000
{
    public class PatchMap
    {
        public const int Size = 19;  // bytes

        public const int PatchCount = 128;

        private bool[] include;

        public PatchMap()
        {
            include = new bool[PatchCount];
        }

        public PatchMap(byte[] data)
        {
            // TODO: Check that the data length matches

            include = new bool[PatchCount];

            var buf = new StringBuilder();
            for (var i = 0; i < data.Length; i++)
            {
                for (var j = 0; j < 7; j++)
                {
                    buf.Append(data[i].IsBitSet(j) ? "1" : "0");
                }
            }

            // Now we should have a string of ones and zeros.
            // Of the last byte of the patch map, only the bottom two bits are used.
            // The conversion to bit string will have some extra bits, so truncate
            // the result to exactly 128 "bits".
            var bitString = buf.ToString().Substring(0, PatchCount);
            for (var i = 0; i < bitString.Length; i++)
            {
                include[i] = bitString[i] == '1' ? true : false;
            }
        }

        public PatchMap(bool[] incl)
        {
            include = new bool[PatchCount];
            // TODO: Check that lengths match
            for (var i = 0; i < incl.Length; i++)
            {
                include[i] = incl[i];
            }
        }

        public bool this[int i]
        {
            get { return include[i]; }
        }

        public byte[] ToData()
        {
            var buf = new StringBuilder();
            for (var i = 0; i < include.Length; i++)
            {
                buf.Append(include[i] ? "1" : "0");
                // each byte maps seven patches, and every 8th bit must be a zero
                if (i % 8 == 0)
                {
                    buf.Append("0");
                }
            }
            // The patches are enumerated starting from the low bits, so reverse the string.
            var bitString = buf.ToString().Reversed();
            // Now we have a long bit string. Slice it into chunks of eight bits to convert to bytes.
            string[] parts = bitString.Split(8);
            var data = new List<byte>();
            foreach (var s in parts)
            {
                data.Add(Convert.ToByte(s, 2));
            }
            return data.ToArray();
        }
    }
}
