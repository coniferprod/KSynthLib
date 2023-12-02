using System;

using KSynthLib.K5;


namespace KSynthLib.Tests.K5;

public class PitchSettingsTests
{
    private PitchSettings settings;

    public PitchSettingsTests()
    {
        settings = new PitchSettings();
        Console.Error.WriteLine(settings);
    }

    [Test]
    public void Coarse_IsCorrectlySet()
    {
        settings.Coarse = new Coarse(-24);
        Assert.That(-24, Is.EqualTo(settings.Coarse.Value));
    }

    [Test]
    public void HasEnvelope()
    {
        Assert.NotNull(settings.Envelope);
    }
}

public class PitchEnvelopeTests
{
    private PitchEnvelope envelope;

    public PitchEnvelopeTests()
    {
        envelope = new PitchEnvelope();
    }

    [Test]
    public void HasCorrectSegmentCount()
    {
        Assert.That(PitchEnvelope.SegmentCount, Is.EqualTo(envelope.Segments.Length));
    }
}

public class PitchEnvelopeSegmentTests
{
    private PitchEnvelopeSegment segment;

    public PitchEnvelopeSegmentTests()
    {
        segment = new PitchEnvelopeSegment();
    }

    [Test]
    public void Rate_IsSetCorrectly()
    {
        segment.Rate = new PositiveDepth(20);
        Assert.That(20, Is.EqualTo(segment.Rate.Value));
    }
}
