using System;

using Xunit;

using KSynthLib.K5;


namespace KSynthLib.Tests.K5
{
    public class PitchSettingsTests
    {
        private PitchSettings settings;

        public PitchSettingsTests()
        {
            settings = new PitchSettings();
            Console.WriteLine(settings);
        }

        [Fact]
        public void Coarse_IsCorrectlySet()
        {
            settings.Coarse = -24;
            Assert.Equal(-24, settings.Coarse);
        }

        [Fact]
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

        [Fact]
        public void HasCorrectSegmentCount()
        {
            Assert.Equal(PitchEnvelope.SegmentCount, envelope.Segments.Length);
        }
    }

    public class PitchEnvelopeSegmentTests
    {
        private PitchEnvelopeSegment segment;

        public PitchEnvelopeSegmentTests()
        {
            segment = new PitchEnvelopeSegment();
        }

        [Fact]
        public void Rate_IsSetCorrectly()
        {
            segment.Rate = 20;
            Assert.Equal(20, segment.Rate);
        }
    }

}

