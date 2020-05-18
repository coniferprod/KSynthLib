using Xunit;
using KSynthLib.K4;

namespace KSynthLib.Tests.K4
{
    public class AmplifierEnvelope_Init
    {
        public AmplifierEnvelope_Init()
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
