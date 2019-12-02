using Xunit;
using KSynthLib.Common;

namespace KSynthLib.Tests
{
    public class SystemExclusive_Length
    {
        private readonly SystemExclusiveHeader _systemExclusive;

        public SystemExclusive_Length()
        {
            _systemExclusive = new SystemExclusiveHeader();
        }

        [Fact]
        public void DataLength_IsCorrect()
        {
            int length = _systemExclusive.ToData().Length;
            Assert.Equal(7, length);
        }
    }
}