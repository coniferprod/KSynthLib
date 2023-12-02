using KSynthLib.K5000;

namespace KSynthLib.Tests.K5000;

public class WaveTests
{
    [Test]
    public void Number_IsCorrectlySet()
    {
        var wave = new Wave(411);
        Assert.That(411, Is.EqualTo(wave.Number));
    }

    [Test]
    public void Name_IsCorrectlySet()
    {
        var wave = new Wave(411);
        Assert.That("Syn Saw1 Cyc", Is.EqualTo(wave.Name));
    }

    [Test]
    public void NumberFrom_IsCorrect()
    {
        var wave = new Wave(411);
        var (high, low) = wave.WaveSelect;
        Assert.That(0x03, Is.EqualTo(high));
        Assert.That(0x1b, Is.EqualTo(low));
    }

    [Test]
    public void WaveSelect_IsCorrect()
    {
        var number = Wave.NumberFrom(0x03, 0x1b);
        Assert.That(411, Is.EqualTo(number));
    }
}
