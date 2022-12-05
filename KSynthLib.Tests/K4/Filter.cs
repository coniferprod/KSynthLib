using Xunit;
using KSynthLib.K4;

namespace KSynthLib.Tests.K4
{
    public class FilterTests
    {
        private readonly Filter _filter;

        public FilterTests()
        {
            _filter = new Filter();

        }

        [Fact]
        public void DataLength_IsCorrect()
        {
            int length = _filter.GetSystemExclusiveData().Count;
            Assert.Equal(Filter.DataSize, length);
        }
    }
}