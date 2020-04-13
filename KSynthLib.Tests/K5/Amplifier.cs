using System;

using Xunit;

using KSynthLib.K5;


namespace KSynthLib.Tests.K5
{
    public class AmplifierTests
    {
        private Amplifier amplifier;

        public AmplifierTests()
        {
            amplifier = new Amplifier();
            Console.WriteLine(amplifier);
        }

        [Fact]
        public void AttackVelocityDepth_IsSetCorrectly()
        {
            amplifier.AttackVelocityDepth = 20;
            Assert.Equal(20, amplifier.AttackVelocityDepth);
        }
    }

    public class AmplifierEnvelopeTests
    {
        private AmplifierEnvelope envelope;

        public AmplifierEnvelopeTests()
        {
            envelope = new AmplifierEnvelope();

        }

        [Fact]
        public void HasCorrectSegmentCount()
        {
            Assert.Equal(AmplifierEnvelope.SegmentCount, envelope.Segments.Length);
        }
    }

}
