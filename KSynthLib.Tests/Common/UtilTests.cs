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
    }
}
