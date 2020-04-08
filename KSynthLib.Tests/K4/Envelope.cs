using Xunit;
using KSynthLib.K4;

namespace KSynthLib.Tests.K4
{
    public class Envelope_Init
    {
        public Envelope_Init()
        {
        }

        [Fact]
        public void InitSuccessful()
        {
            Envelope env = new Envelope();
            Assert.NotNull(env);
        }
    }
}
