using Xunit;
using KSynthLib.K4;

namespace KSynthLib.K4.Tests
{
    public class Filter_Data
    {
        private readonly Filter _filter;

        public Filter_Data()
        {
            _filter = new Filter();

        }

        [Fact]
        public void DataLength_IsCorrect()
        {
            int length = _filter.ToData().Length;
            Assert.Equal(Filter.DataSize, length);
        }
    }

}