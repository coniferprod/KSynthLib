using System;

using KSynthLib.K5;


namespace KSynthLib.Tests.K5;

public class HarmonicEnvelopeSegmentTests
{
    private HarmonicEnvelopeSegment segment;

    public HarmonicEnvelopeSegmentTests()
    {
        segment = new HarmonicEnvelopeSegment();
    }

    [Test]
    public void Level_IsSetCorrectly()
    {
        segment.Level = new PositiveDepth(20);
        Assert.That(20, Is.EqualTo(segment.Level.Value));
    }
}

public class HarmonicEnvelopeTests
{
    private HarmonicEnvelope envelope;

    public HarmonicEnvelopeTests()
    {
        envelope = new HarmonicEnvelope();
    }

    [Test]
    public void HasCorrectNumberOfSegments()
    {
        Assert.That(HarmonicEnvelope.SegmentCount, Is.EqualTo(envelope.Segments.Length));
    }

    [Test]
    public void Effect_IsSetCorrectly()
    {
        envelope.Effect = new PositiveDepth(20);
        Assert.That(20, Is.EqualTo(envelope.Effect.Value));
    }
}

public class HarmonicSettingsTests
{
    private HarmonicSettings settings;

    public HarmonicSettingsTests()
    {
        settings = new HarmonicSettings();
        Console.Error.WriteLine(settings);
    }

    [Test]
    public void HasCorrectNumberOfEnvelopes()
    {
        Assert.That(HarmonicSettings.HarmonicEnvelopeCount, Is.EqualTo(settings.Envelopes.Length));
    }
}
