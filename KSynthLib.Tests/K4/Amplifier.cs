using Xunit;
using KSynthLib.K4;

namespace KSynthLib.Tests.K4
{
    public class Amplifier_Init
    {
        public Amplifier_Init()
        {
        }

        [Fact]
        public void Init_Successful()
        {
            Amplifier amp = new Amplifier();
            Assert.NotNull(amp);
        }

        [Fact]
        public void InitFromData_Successful()
        {
            byte[] data = new byte[] { 
                1, 2, 3, 4, 
                1, 2, 3, 4, 
                1, 2, 3, 4, 
                1, 2, 3, 4, 
                0, 0, 0, 0, 
                0, 0, 0, 0, 
                0, 0, 0, 0, 
                0, 0, 0, 0, 
                0, 0, 0, 0 
            };
            Amplifier amp = new Amplifier(data);
            Assert.NotNull(amp);
        }

        [Fact]
        public void InitEnvelopeSuccessful()
        {
            Amplifier amp = new Amplifier();
            Envelope env = new Envelope(0, 0, 0, 0);
            amp.Env = env;
            Assert.NotNull(amp.Env);
        }
    }
}
