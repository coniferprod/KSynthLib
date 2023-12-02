using System;

using KSynthLib.K5;


namespace KSynthLib.Tests.K5;

public class FilterTests
{
    private Filter filter;

    public FilterTests()
    {
        filter = new Filter();
        Console.Error.WriteLine(filter);
    }

    [Test]
    public void VelocityDepth_IsSetCorrectly()
    {
        filter.VelocityDepth = new Depth(20);
        Assert.That(20, Is.EqualTo(filter.VelocityDepth.Value));
    }
}
