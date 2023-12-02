using System;

using KSynthLib.K5;


namespace KSynthLib.Tests.K5;

public class AmplifierTests
{
    private Amplifier amplifier;

    public AmplifierTests()
    {
        amplifier = new Amplifier();
        Console.Error.WriteLine(amplifier);
    }

    [Test]
    public void AttackVelocityDepth_IsSetCorrectly()
    {
        amplifier.AttackVelocityDepth = new Depth(20);
        Assert.That(20, Is.EqualTo(amplifier.AttackVelocityDepth.Value));
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
    public void HasCorrectSegmentCount()
    {
        Assert.That(AmplifierEnvelope.SegmentCount, Is.EqualTo(envelope.Segments.Length));
    }
}
