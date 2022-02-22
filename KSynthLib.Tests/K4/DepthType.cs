using System;
using Xunit;

using KSynthLib.K4;

namespace KSynthLib.Tests.Common
{
    public class DepthTests
    {
        Depth depth;

        public DepthTests()
        {
            depth = new Depth();
        }

        [Fact]
        public void Value_IsDefault()
        {
            Assert.Equal(depth.DefaultValue, depth.Value);
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
