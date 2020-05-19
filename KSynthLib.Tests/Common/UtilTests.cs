using System;
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

    }
}