using System;
using System.Collections.Generic;
using Xunit;
using KSynthLib.Common;

namespace KSynthLib.Tests.Common
{
    public class UtilTests
    {
        public UtilTests()
        {

        }

        [Fact]
        public void HexString_IsConvertedToByteArray()
        {
            string hexString = "112233";
            byte[] actual = Util.HexStringToByteArray(hexString);
            byte[] expected = new byte[] { 0x11, 0x22, 0x33 };
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BytesAreInterleaved()
        {
            var a1 = new List<byte>();
            a1.Add(1);
            a1.Add(2);
            a1.Add(3);

            var a2 = new List<byte>();
            a2.Add(4);
            a2.Add(5);
            a2.Add(6);

            var expected = new List<byte>();
            expected.Add(1);
            expected.Add(4);
            expected.Add(2);
            expected.Add(5);
            expected.Add(3);
            expected.Add(6);

            var actual = Util.InterleaveBytes(a1, a2);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BytesAreDivided()
        {
            var a = new List<byte>();
            a.Add(1);
            a.Add(4);
            a.Add(2);
            a.Add(5);
            a.Add(3);
            a.Add(6);

            var a1 = new List<byte>();
            a1.Add(1);
            a1.Add(2);
            a1.Add(3);

            var a2 = new List<byte>();
            a2.Add(4);
            a2.Add(5);
            a2.Add(6);

            var (actual1, actual2) = Util.DivideBytes(a);
            Assert.Equal(a1, actual1);
            Assert.Equal(a2, actual2);
        }

        [Fact]
        public void BytesAreChunked()
        {
            var ba = new List<byte>();
            for (byte b = 0; b < 100; b++)
            {
                ba.Add(b);
            }

            List<List<byte>> chunks = Util.Chunked(ba, 16);

            // Original array length = 100
            // Divided into chunks of 16: 6 * 16 + 4
            // Total number of chunks: 7
            // Length of last chunk: 4
            Assert.Equal(7, chunks.Count);
            Assert.Equal(4, chunks[chunks.Count - 1].Count);
        }

        [Fact]
        public void EmptyArrayIsNotChunked()
        {
            Assert.Throws<ArgumentException>(() => Util.Chunked(new List<byte>(), 16));
        }

        [Fact]
        public void BadChunkSizeIsRejected()
        {
            var ba = new List<byte>();
            for (byte b = 0; b < 100; b++)
            {
                ba.Add(b);
            }

            Assert.Throws<ArgumentException>(() => Util.Chunked(ba, 0));
        }

        [Fact]
        public void ChunkSizeOfOneIsCorrect()
        {
            var ba = new List<byte>();
            for (byte b = 0; b < 100; b++)
            {
                ba.Add(b);
            }

            // Using the default chunk size of 1,
            // the number of chunks should equal the number of bytes:
            List<List<byte>> chunks = Util.Chunked(ba);
            Assert.Equal(ba.Count, chunks.Count);
        }

        [Fact]
        public void IsPackedCorrectly()
        {
            var packedBytes = new byte[] { 42, 101, 74, 103, 76, 105, 78, 107 };
            var packed = new List<byte>(packedBytes);

            var unpackedBytes = new byte[] { 101, 202, 103, 204, 105, 206, 107 };
            var unpacked = new List<byte>(unpackedBytes);

            var actualPacked = Util.Packed(unpacked);

            Assert.Equal(packed, actualPacked);
        }

        [Fact]
        public void IsUnpackedCorrectly()
        {
            var packedBytes = new byte[] { 42, 101, 74, 103, 76, 105, 78, 107 };
            var packed = new List<byte>(packedBytes);

            var unpackedBytes = new byte[] { 101, 202, 103, 204, 105, 206, 107 };
            var unpacked = new List<byte>(unpackedBytes);

            var actualUnpacked = Util.Unpacked(packed);

            Assert.Equal(unpacked, actualUnpacked);
        }
    }
}
