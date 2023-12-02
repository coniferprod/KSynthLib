using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace KSynthLib.Common;

public static class EnumerableExtensions {
    // Split list on predicate.
    // Adapted from https://stackoverflow.com/a/40952271/1016326
    public static IEnumerable<IList<TSource>> Split<TSource>(
        this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        var list = new List<TSource>();

        foreach (var element in source)
        {
            if (predicate(element))
            {
                if (list.Count > 0)
                {
                    yield return list;
                    list = new List<TSource>();
                }
            }
            else
            {
                list.Add(element);
            }
        }

        if (list.Count > 0)
        {
            yield return list;
        }
    }

    /// <summary>
    /// Mix the elements of the two sequences, alternating elements
    /// one by one for as long as possible. If one sequence has more
    /// elements than the other, its leftovers will appear at the
    /// end of the mixed sequence.
    /// Adapted from https://codereview.stackexchange.com/a/207252
    /// </summary>
    public static IEnumerable<T> Interleave<T>
        (this IEnumerable<T> first, IEnumerable<T> second)
    {
        var firstEnumerator = first.GetEnumerator();
        var secondEnumerator = second.GetEnumerator();

        // As long as there are elements in the first sequence...
        while (firstEnumerator.MoveNext())
        {
            // ...take an element from the first sequence.
            yield return firstEnumerator.Current;

            // And, if possible...
            if (secondEnumerator.MoveNext())
            {
                // ...take an element from the second sequence.
                yield return secondEnumerator.Current;
            }
        }

        // If there are any elements left over in the second sequence...
        while (secondEnumerator.MoveNext())
        {
            // ...take each of them.
            yield return secondEnumerator.Current;
        }
    }

    // Adapted from https://stackoverflow.com/a/27533369/1016326
    public static IEnumerable<T> Interleave<T>(this IEnumerable<IEnumerable<T>> sources)
    {
        var queues = sources.Select(x => new Queue<T>(x)).ToList();
        while (queues.Any(x => x.Any()))
        {
            foreach (var queue in queues.Where(x => x.Any()))
            {
                yield return queue.Dequeue();
            }
        }
    }
}

public class Util
{
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
