using System;

using KSynthLib.K5;


namespace KSynthLib.Tests.K5;

public class SourceTests
{
    private Source source;

    public SourceTests()
    {
        source = new Source();
        Console.Error.WriteLine(source);
    }

    [Test]
    public void HasHarmonics()
    {
        Assert.That(Source.HarmonicCount, Is.EqualTo(source.Harmonics.Length));
    }
}

public class SourceSettingsTests
{
    private SourceSettings settings;

    public SourceSettingsTests()
    {
        settings = new SourceSettings();
    }

    [Test]
    public void Delay_IsSetCorrectly()
    {
        settings.Delay = new PositiveDepth(20);
        Assert.That(20, Is.EqualTo(settings.Delay.Value));
    }
}
