using System;
using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K5000
{
    // Represents a tone map found in the block dumps.
    public class ToneMap: ISystemExclusiveData
    {
        public const int DataSize = 19;  // bytes

        public const int ToneCount = 128;

        private bool[] _include;

        public ToneMap()
        {
            this._include = new bool[ToneCount];
        }

        public ToneMap(byte[] data)
        {
            if (data.Length < ToneMap.DataSize)
            {
                throw new ArgumentException("Not enough data for tone map");
            }

            this._include = new bool[ToneCount];

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
            var bitString = buf.ToString().Substring(0, ToneCount);
            for (var i = 0; i < bitString.Length; i++)
            {
                this._include[i] = bitString[i] == '1' ? true : false;
            }
        }

        public ToneMap(bool[] incl)
        {
            if (incl.Length < ToneMap.DataSize)
            {
                throw new ArgumentException("Not enough data for tone map");
            }

            this._include = new bool[ToneCount];
            for (var i = 0; i < incl.Length; i++)
            {
                this._include[i] = incl[i];
            }
        }

        // Indexer to return the status of the given patch number (0~127).
        public bool this[int i]
        {
            get { return this._include[i]; }
        }

        // Returns a string representation of this tone map.
        public override string ToString()
        {
            var sb = new StringBuilder();

            for (var i = 0; i < ToneMap.ToneCount; i++)
            {
                if (this[i])
                {
                    sb.Append($"{i + 1} ");
                }
            }

            return sb.ToString();
        }

        // Returns the count of patches included in this tone map.
        public int Count {
            get {
                var count = 0;
                for (var i = 0; i < ToneMap.ToneCount; i++)
                {
                    if (this[i])
                    {
                        count += 1;
                    }
                }
                return count;
            }
        }

        //
        // ISystemExclusiveData implementation
        //

        // Returns the System Exclusive data of this tone map (19 bytes).
        public List<byte> Data
        {
            get
            {
                var buf = new StringBuilder();
                for (var i = 0; i < this._include.Length; i++)
                {
                    buf.Append(this._include[i] ? "1" : "0");
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
                return data;
            }
        }

        public int DataLength => DataSize;
    }
}
