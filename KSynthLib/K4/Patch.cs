using System;
using System.Collections.Generic;

using KSynthLib.Common;


namespace KSynthLib.K4
{
    public class PatchName : ISystemExclusiveData
    {
        public static readonly int Length = 10;

        private string _name;

        public string Value
        {
            get => _name.PadRight(Length, ' ').Substring(0, Length);
            set => _name = value.PadRight(Length, ' ').Substring(0, Math.Min(value.Length, Length));
        }

        public PatchName(string s)
        {
            this.Value = s;
        }

        public PatchName(byte[] data, int offset = 0)
        {
            var chars = new List<char>();

            for (var i = offset; i < Length; i++)
            {
                var b = data[i];

                // If there is a character not found in the allowed list,
                // replace it with a space and go to the next character
                if (!Array.Exists(AllowedNameCharacterCodes, element => element.Equals(b)))
                {
                    chars.Add(' ');
                    continue;
                }

                if (b == 0x7e)  // right arrow
                {
                    chars.Add('\u2192');
                }
                else if (b == 0x7f) // left arrow
                {
                    chars.Add('\u2190');
                }
                else if (b == 0x5c) // yen sign
                {
                    chars.Add('\u00a5');
                }
                else  // straight ASCII
                {
                    chars.Add((char)b);
                }
            }

            this.Value = new string(chars.ToArray());
        }

        //
        // ISystemExclusiveData implementation
        //

        public List<byte> Data
        {
            get
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

                return bytes;
            }
        }

        public int DataLength => Length;

        public static readonly byte[] AllowedNameCharacterCodes = new byte[]
        {
            0x20, // space
            0x21, // exclamation mark
            0x22, // double quote
            0x23, // hash
            0x24, // dollar sign
            0x25, // percent sign
            0x26, // ampersand
            0x27, // single quote
            (byte)'(',
            (byte)')',
            (byte)'*',
            (byte)'+',
            (byte)'-',
            (byte)',',
            (byte)'/',
            (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7',
            (byte)'8', (byte)'9',
            (byte)':',
            (byte)';',
            (byte)'<',
            (byte)'=',
            (byte)'>',
            (byte)'?',
            (byte)'@', // 0x40
            (byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F', (byte)'G', (byte)'H',
            (byte)'I', (byte)'J', (byte)'K', (byte)'L', (byte)'M', (byte)'N', (byte)'O', (byte)'P',
            (byte)'Q', (byte)'R', (byte)'S', (byte)'T', (byte)'U', (byte)'V', (byte)'W', (byte)'X',
            (byte)'Y', (byte)'Z',
            (byte)'[', // 0x5b
            0x5c,  // yen sign (U+00A5)
            (byte)']', // 0x5d
            (byte)'^', // 0x5e
            (byte)'_', // 0x5f
            (byte)'`', // 0x60
            (byte)'a', (byte)'b', (byte)'c', (byte)'d', (byte)'e', (byte)'f', (byte)'g', (byte)'h',
            (byte)'i', (byte)'j', (byte)'k', (byte)'l', (byte)'m', (byte)'n', (byte)'o', (byte)'p',
            (byte)'q', (byte)'r', (byte)'s', (byte)'t', (byte)'u', (byte)'v', (byte)'w', (byte)'x',
            (byte)'y', (byte)'z',
            (byte)'{', // 0x7B
            (byte)'|', // 0x7C
            (byte)'}', // 0x7D
            0x7e, // right arrow (U+2192)
            0x7f, // left arrow (U+2190)
        };
    }

    /// <summary>
    /// Abstract base class for patches.
    /// </summary>
    public abstract class Patch
    {
        private byte _checksum;

        /// <summary>
        /// The checksum for this patch.
        /// </summary>
        /// <value>
        /// Computed from the collected data usign the Kawai checksum algorithm.
        /// </value>
        public virtual byte Checksum
        {
            get
            {
                byte[] data = CollectData();
                int sum = 0;
                foreach (byte b in data)
                {
                    sum = (sum + b) & 0xff;
                }
                sum += 0xA5;
                return (byte)(sum & 0x7f);
            }

            set
            {
                _checksum = value;
            }
        }

        protected abstract byte[] CollectData();

        public virtual byte[] ToData()
        {
            var allData = new List<byte>();

            allData.AddRange(CollectData());
            allData.Add(this.Checksum);  // calls CollectData again, perf?

            return allData.ToArray();
        }
    }
}
