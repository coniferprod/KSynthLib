using System;
using System.Collections.Generic;
using Xunit;
using KSynthLib.K4;
using KSynthLib.Common;

namespace KSynthLib.Tests.K4
{
    public class SourceTests
    {
        string rawString =
        "00 00 02 03 " +
        "00 00 50 40 " +
        "12 12 7E 7F " +
        "4C 4C 5A 5B " +
        "00 34 02 03 " +
        "2C 37 34 35 " +
        "02 02 15 11";

        [Fact]
        public void InitFromData_IsSuccessful()
        {
            string hexString = rawString.Replace(" ", "");
            byte[] data = Util.HexStringToByteArray(hexString);

            int sourceDataLength = Source.DataSize * 4;
            byte[] sourceData = new byte[sourceDataLength];
            List<byte> source1Data = Util.EveryNthElement(new List<byte>(sourceData), 4, 0);
            Source s1 = new Source(source1Data.ToArray());
            Assert.Equal(0, s1.Delay.Value);
        }

    }
}
