using System;

using Xunit;

using KSynthLib.K5000;


namespace KSynthLib.Tests.K5000
{
    public class SingleCommon
    {
        private readonly SingleCommonSettings singleCommon;

        public SingleCommon()
        {
            singleCommon = new SingleCommonSettings();
            Console.WriteLine(singleCommon);
        }

        [Fact]
        public void DataLength_IsCorrect()
        {
            int length = singleCommon.ToData().Length;
            Assert.Equal(33, length);
        }

        [Fact]
        public void Name_IsSet()
        {
            Assert.NotNull(singleCommon.Name);
        }

    }
}