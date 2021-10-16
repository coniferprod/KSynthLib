using System;
using System.Collections.Generic;
using System.Text;

namespace KSynthLib.Common
{
    // Byte extensions from https://derekwill.com/2015/03/05/bit-processing-in-c/
    public static class ByteExtensions
    {
        public static bool IsBitSet(this byte b, int pos)
        {
            if (pos < 0 || pos > 7)
            {
                throw new ArgumentOutOfRangeException("pos", "Index must be in the range of 0-7.");
            }

            return (b & (1 << pos)) != 0;
        }

        public static byte SetBit(this byte b, int pos)
        {
            if (pos < 0 || pos > 7)
            {
                throw new ArgumentOutOfRangeException("pos", "Index must be in the range of 0-7.");
            }

            return (byte)(b | (1 << pos));
        }

        public static byte UnsetBit(this byte b, int pos)
        {
            if (pos < 0 || pos > 7)
            {
                throw new ArgumentOutOfRangeException("pos", "Index must be in the range of 0-7.");
            }

            return (byte)(b & ~(1 << pos));
        }

        public static byte ToggleBit(this byte b, int pos)
        {
            if (pos < 0 || pos > 7)
            {
                throw new ArgumentOutOfRangeException("pos", "Index must be in the range of 0-7.");
            }

            return (byte)(b ^ (1 << pos));
        }

        public static string ToBinaryString(this byte b, int padding = 8)
        {
            return Convert.ToString(b, 2).PadLeft(padding, '0');
        }

        public static sbyte ToSignedByte(this byte b)
        {
            return unchecked((sbyte)b);
        }
    }

    public static class SignedByteExtensions
    {
        public static byte ToByte(this sbyte s)
        {
            return unchecked((byte)s);
        }
    }
}