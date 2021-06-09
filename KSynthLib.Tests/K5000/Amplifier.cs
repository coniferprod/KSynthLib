using System;

using Xunit;

using KSynthLib.K5000;


namespace KSynthLib.Tests.K5000
{
    public class AmplifierTests
    {
        private DCASettings settings;

        public AmplifierTests()
        {
            settings = new DCASettings();
            Console.Error.WriteLine(settings);
        }

        [Fact]
        public void VelocityCurve_IsSetCorrectly()
        {
            settings.VelocityCurve = 1;
            Assert.Equal(1, settings.VelocityCurve);
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
        public void AttackTime_IsCorrectlySet()
        {
            envelope.AttackTime = 100;
            var attackTime = envelope.AttackTime;
            Assert.Equal(100, attackTime);
        }

        [Fact]
        public void AttackTime_RejectsBadValue()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => envelope.AttackTime = 200);
        }
    }

    public class KeyScalingControlEnvelopeTests
    {
        KeyScalingControlEnvelope envelope;

        public KeyScalingControlEnvelopeTests()
        {
            envelope = new KeyScalingControlEnvelope();
        }

        [Fact]
        public void Level_IsSetCorrectly()
        {
            envelope.Level = 32;
            var level = envelope.Level;
            Assert.Equal(32, level);
        }

        [Fact]
        public void Level_BadValueIsRejected()
        {
            // Try to set a value that is out of range -63...+63:
            Assert.Throws<ArgumentOutOfRangeException>(() => envelope.Level = 100);
        }
    }
}
