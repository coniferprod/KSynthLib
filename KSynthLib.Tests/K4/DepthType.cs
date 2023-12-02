using System;

using KSynthLib.K4;

namespace KSynthLib.Tests.Common;
public class DepthTests
{
    Depth depth;

    public DepthTests()
    {
        depth = new Depth();
    }

    [Test]
    public void Value_IsDefault()
    {
        Assert.That(depth.DefaultValue, Is.EqualTo(depth.Value));
    }

    [Test]
    public void Value_IsSetCorrectly()
    {
        depth.Value = 42;
        Assert.That(42, Is.EqualTo(depth.Value));
    }

    [Test]
    public void Value_ThrowsIfOutOfRange()
    {
        // Test that the Value setter of the DepthType
        // correctly throws an exception by trying to set
        // a value that is out of range.
        Assert.Throws<ArgumentOutOfRangeException>(() => depth.Value = 10000);
    }
}
