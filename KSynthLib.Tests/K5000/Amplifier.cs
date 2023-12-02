using System;

using KSynthLib.K5000;


namespace KSynthLib.Tests.K5000;

public class AmplifierTests
{
    private DCASettings settings;

    public AmplifierTests()
    {
        settings = new DCASettings();
        Console.Error.WriteLine(settings);
    }

    [Test]
    public void VelocityCurve_IsSetCorrectly()
    {
        settings.VelocityCurve = VelocityCurve.Curve6;
        Assert.That(VelocityCurve.Curve6, Is.EqualTo(settings.VelocityCurve));
    }
}

public class AmplifierEnvelopeTests
{
    private AmplifierEnvelope envelope;

    public AmplifierEnvelopeTests()
    {
        envelope = new AmplifierEnvelope();
    }

    [Test]
    public void AttackTime_IsCorrectlySet()
    {
        envelope.AttackTime.Value = 100;
        var attackTime = envelope.AttackTime;
        Assert.That(100, Is.EqualTo(attackTime.Value));
    }

    [Test]
    public void AttackTime_RejectsBadValue()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => envelope.AttackTime.Value = 200);
    }
}

public class KeyScalingControlEnvelopeTests
{
    KeyScalingControlEnvelope envelope;

    public KeyScalingControlEnvelopeTests()
    {
        envelope = new KeyScalingControlEnvelope();
    }

    [Test]
    public void Level_IsSetCorrectly()
    {
        envelope.Level.Value = 32;
        var level = envelope.Level;
        Assert.That(32, Is.EqualTo(level.Value));
    }

    [Test]
    public void Level_BadValueIsRejected()
    {
        // Try to set a value that is out of range -63...+63:
        Assert.Throws<ArgumentOutOfRangeException>(() => envelope.Level.Value = 100);
    }
}
