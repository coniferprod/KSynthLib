using System;

using KSynthLib.K5000;


namespace KSynthLib.Tests.K5000;

public class OscillatorTests
{
    DCOSettings settings;

    public OscillatorTests()
    {
        settings = new DCOSettings();
        Console.Error.WriteLine(settings);
    }

    [Test]
    public void Coarse_IsCorrectlySet()
    {
        settings.Coarse.Value = -12;
        Assert.That(-12, Is.EqualTo(settings.Coarse.Value));
    }
}
