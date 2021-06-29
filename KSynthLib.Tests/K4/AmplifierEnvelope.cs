using Xunit;
using KSynthLib.K4;

namespace KSynthLib.Tests.K4
{
    public class AmplifierEnvelopeTests
    {
        public AmplifierEnvelopeTests()
        {
        }

        [Fact]
        public void InitSuccessful()
        {
            AmplifierEnvelope env = new AmplifierEnvelope();
            Assert.NotNull(env);
        }
    }
}
