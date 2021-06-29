using System;
using Xunit;
using KSynthLib.Common;

namespace KSynthLib.Tests.Common
{
    public class DepthTypeTests
    {
        DepthType depth;

        public DepthTypeTests()
        {
            depth = new DepthType();
        }

        [Fact]
        public void Value_IsDefault()
        {
            Assert.Equal(0, depth.Value);
        }

        [Fact]
        public void Value_IsSetCorrectly()
        {
            depth.Value = 42;
            Assert.Equal(42, depth.Value);
        }

        [Fact]
        public void Value_ThrowsIfOutOfRange()
        {
            // Test that the Value setter of the DepthType
            // correctly throws an exception by trying to set
            // a value that is out of range.
            Assert.Throws<ArgumentOutOfRangeException>(() => depth.Value = 10000);
        }
    }
}
