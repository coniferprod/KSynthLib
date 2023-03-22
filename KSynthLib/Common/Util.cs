using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KSynthLib.Common
{
    public class Util
    {
        // Adapted from https://stackoverflow.com/a/39439915
        // Probably should be augmented to include the delimiter.
        public static List<byte[]> SplitBytesByDelimiter(byte[] data, byte delimiter)
        {
            var retList = new List<byte[]>();
            if (data == null || data.Length < 1)
            {
                return retList; // rather return empty than null, or than throw an exception
            }

            int start = 0;
            int pos = 0;
            byte[] remainder = null; // in case data found at end without terminating delimiter

            while (true)
            {
                // Console.WriteLine("pos " + pos + " start " + start);
                if (pos >= data.Length)
                {
                    break;
                }

                if (data[pos] == delimiter)
                {
                    // Console.WriteLine("delimiter found at pos " + pos + " start " + start);

                    // separator found
                    if (pos == start)
                    {
                        // Console.WriteLine("first char is delimiter, skipping");
                        // skip if first character is delimiter
                        pos++;
                        start++;
                        if (pos >= data.Length)
                        {
                            // last character is a delimiter, yay!
                            remainder = null;
                            break;
                        }
                        else
                        {
                            // remainder exists
                            remainder = new byte[data.Length - start];
                            Buffer.BlockCopy(data, start, remainder, 0, (data.Length - start));
                            continue;
                        }
                    }
                    else
                    {
                        // Console.WriteLine("creating new byte[] at pos " + pos + " start " + start);
                        var ba = new byte[(pos - start)];
                        Buffer.BlockCopy(data, start, ba, 0, (pos - start));
                        retList.Add(ba);

                        start = pos + 1;
                        pos = start;

                        if (pos >= data.Length)
                        {
                            // last character is a delimiter, yay!
                            remainder = null;
                            break;
                        }
                        else
                        {
                            // remainder exists
                            remainder = new byte[data.Length - start];
                            Buffer.BlockCopy(data, start, remainder, 0, (data.Length - start));
                        }
                    }
                }
                else
                {
                    // payload character, continue;
                    pos++;
                }
            }

            if (remainder != null)
            {
                // Console.WriteLine("adding remainder");
                retList.Add(remainder);
            }

            return retList;
        }

        // n1 = high nybble, n2 = low nybble
        public static byte ByteFromNybbles(byte n1, byte n2)
        {
            return (byte)((n1 << 4) | n2);
        }

        public static (byte, byte) NybblesFromByte(byte b)
        {
            byte low = (byte)(b & 0x0F);
            byte high = (byte)((b & 0xF0) >> 4);
            return (high, low);
        }

        public static byte LowNybble(byte b)
        {
            return (byte)(b & 0x0F);
        }

        public static byte HighNybble(byte b)
        {
            return (byte)((b & 0xF0) >> 4);
        }

        public static byte[] ConvertFromTwoNybbleFormat(byte[] data)
        {
            int count = data.Length / 2;  // NOTE: length must be even!
            byte[] result = new byte[count];
            int index = 0;
            int offset = 0;
            while (index < count)
            {
                result[index] = ByteFromNybbles(data[offset], data[offset + 1]);
                index++;
                offset += 2;
            }
        	return result;
        }

        public static byte[] ConvertToTwoNybbleFormat(byte[] data)
        {
            var result = new byte[data.Length * 2];
            int index = 0;
            for (var i = 0; i < data.Length; i++)
            {
                byte highNybble = 0;
                byte lowNybble = 0;
                (highNybble, lowNybble) = NybblesFromByte(data[i]);
                result[index] = highNybble;
                index++;
                result[index] = lowNybble;
                index++;
            }

            return result;
        }

        public static (byte, int) GetNextByte(byte[] data, int offset)
        {
            var b = data[offset];
/*
#if DEBUG
            Console.WriteLine(string.Format("{0:D4}: 0x{1:X2}", offset, b));
#endif
*/
            return (b, offset + 1);
        }

        public static (byte[], int) GetNextBytes(byte[] data, int offset = 0, int count = 1)
        {
            var slice = new byte[count];
            Array.Copy(data, offset, slice, 0, count);
            return (slice, offset + count);
        }

        public static (bool, int) ByteArrayCompare(byte[] a1, byte[] a2)
        {
            if (a1.Length != a2.Length)
            {
                return (false, -1);
            }

            for (var i = 0; i < a1.Length; i++)
            {
                if (a1[i] != a2[i])
                {
                    return (false, i);
                }
            }

            return (true, a1.Length);
        }

        public static List<byte> EveryNthElement(List<byte> list, int n, int start)
        {
            var result = new List<byte>();
            for (var i = 0; i < list.Count; i++)
            {
                if ((i % n) == 0)
                {
                    result.Add(list[i + start]);  // TODO: Check this!
                }
            }
            return result;
        }

        public static List<byte[]> SeparateBytes(byte[] data, int count)
        {
            var dataBytes = new List<byte>(data);
            var byteArrayLists = new List<byte[]>();
            for (var i = 0; i < count; i++)
            {
                byteArrayLists.Add(Util.EveryNthElement(dataBytes, count, i).ToArray());
            }
            return byteArrayLists;
        }

        public static byte[] HexStringToByteArray(String hex)
        {
            int charCount = hex.Length;
            var bytes = new byte[charCount / 2];
            for (var i = 0; i < charCount; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

        public static List<byte> InterleaveBytes(List<List<byte>> buffers)
        {
            // Collect the element counts of each buffer
            var counts = new List<int>();
            foreach (var buf in buffers)
            {
                counts.Add(buf.Count);
            }

            // Find the smallest element count
            var minimumLength = counts.Min(x => x);

            var result = new List<byte>();
            for (var i = 0; i < minimumLength; i++)
            {
                // Get the current byte from each buffer
                foreach (var buf in buffers)
                {
                    result.Add(buf[i]);
                }
            }

            return result;
        }

        public static List<byte> InterleaveBytes(List<byte> a1, List<byte> a2)
        {
            var result = new List<byte>();

            var minimumLength = Math.Min(a1.Count, a2.Count);
            for (var i = 0; i < minimumLength; i++)
            {
                result.Add(a1[i]);
                result.Add(a2[i]);
            }

            return result;
        }

        public static (List<byte>, List<byte>) DivideBytes(List<byte> a)
        {
            var a1 = new List<byte>();
            var a2 = new List<byte>();

            var length = a.Count % 2 == 0 ? a.Count : a.Count - 1;
            var index = 0;
            while (index < length)
            {
                a1.Add(a[index]);
                index++;
                a2.Add(a[index]);
                index++;
            }

            return (a1, a2);
        }
    }
}
