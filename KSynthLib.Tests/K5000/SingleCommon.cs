using System;

using Xunit;

using KSynthLib.K5000;
using KSynthLib.Common;


namespace KSynthLib.Tests.K5000
{
    public class SingleCommon
    {
        private readonly SingleCommonSettings singleCommon;

        // Common data from WizooIni.syx:
        private string testData = "000002020D410A1000583369221D004A0000002400043A04382A000C0C6300424140403F3E410057697A6F6F496E697300000201000201400103400000000000000000000040404040404040400000000000";

        public SingleCommon()
        {
            byte[] data = Util.HexStringToByteArray(testData);
            Console.WriteLine($"Single Common data from hex string: {data.Length} bytes");
            Console.WriteLine(Util.HexDump(data));
            singleCommon = new SingleCommonSettings(data);
            Console.WriteLine(singleCommon);
        }

        [Fact]
        public void EffectSettings_IsParsedCorrectly()
        {
            Assert.Equal(1, singleCommon.EffectAlgorithm);
            Assert.Equal(0, singleCommon.Reverb.ReverbType);

        }

        [Fact]
        public void Name_IsParsedCorrectly()
        {
            Assert.Equal("WizooIni", singleCommon.Name);
        }

        [Fact]
        public void DataLength_IsCorrect()
        {
            int length = singleCommon.ToData().Length;
            Console.WriteLine($"Length of data generated for SingleCommonSettings = {length}");
            Assert.Equal(SingleCommonSettings.DataSize, length);
        }

        [Fact]
        public void Name_IsSet()
        {
            Assert.NotNull(singleCommon.Name);
        }

    }
}