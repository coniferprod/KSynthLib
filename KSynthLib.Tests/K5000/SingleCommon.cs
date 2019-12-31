using Xunit;
using KSynthLib.K5000;

namespace KSynthLib.Tests.K5000
{
    public class SingleCommon
    {
        private readonly SingleCommonSettings _singleCommon;

        public SingleCommon()
        {
            _singleCommon = new SingleCommonSettings();
        }

        [Fact]
        public void DataLength_IsCorrect()
        {
            int length = _singleCommon.ToData().Length;
            Assert.Equal(33, length);

        }

    }
}