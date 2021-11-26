using Xunit;
using KSynthLib.Common;

namespace KSynthLib.Tests.Common
{
    public class SystemExclusiveTests
    {
        private readonly SystemExclusiveHeader _systemExclusive;

        public SystemExclusiveTests()
        {
            _systemExclusive = new SystemExclusiveHeader(0);
        }

        [Fact]
        public void DataLength_IsCorrect()
        {
            int length = _systemExclusive.ToData().Length;
            Assert.Equal(7, length);
        }
    }
}