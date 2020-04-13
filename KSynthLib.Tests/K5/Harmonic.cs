using System;

using Xunit;

using KSynthLib.K5;


namespace KSynthLib.Tests.K5
{
    public class HarmonicEnvelopeSegmentTests
    {
        private HarmonicEnvelopeSegment segment;

        public HarmonicEnvelopeSegmentTests()
        {
            segment = new HarmonicEnvelopeSegment();
        }

        [Fact]
        public void Level_IsSetCorrectly()
        {
            segment.Level = 20;
            Assert.Equal(20, segment.Level);
        }
    }

    public class HarmonicEnvelopeTests
    {
        private HarmonicEnvelope envelope;

        public HarmonicEnvelopeTests()
        {
            envelope = new HarmonicEnvelope();
        }

        [Fact]
        public void HasCorrectNumberOfSegments()
        {
            Assert.Equal(HarmonicEnvelope.SegmentCount, envelope.Segments.Length);
        }

        [Fact]
        public void Effect_IsSetCorrectly()
        {
            envelope.Effect = 20;
            Assert.Equal(20, envelope.Effect);
        }

    }

    public class HarmonicSettingsTests
    {
        private HarmonicSettings settings;

        public HarmonicSettingsTests()
        {
            settings = new HarmonicSettings();
            Console.WriteLine(settings);
        }

        [Fact]
        public void HasCorrectNumberOfEnvelopes()
        {
            Assert.Equal(HarmonicSettings.HarmonicEnvelopeCount, settings.Envelopes.Length);
        }
    }
}
