using System;

using Xunit;

using KSynthLib.K5;


namespace KSynthLib.Tests.K5
{
    public class FilterTests
    {
        private Filter filter;

        public FilterTests()
        {
            filter = new Filter();
            Console.Error.WriteLine(filter);
        }

        [Fact]
        public void VelocityDepth_IsSetCorrectly()
        {
            filter.VelocityDepth = new Depth(20);
            Assert.Equal(20, filter.VelocityDepth.Value);
        }
    }
}
