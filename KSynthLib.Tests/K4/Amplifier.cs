using KSynthLib.K4;

namespace KSynthLib.Tests.K4;

public class AmplifierTests
{
    [Test]
    public void Init_Successful()
    {
        Amplifier amp = new();
        Assert.NotNull(amp);
    }

    [Test]
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
        Amplifier amp = new(data);
        Assert.NotNull(amp);
    }

    [Test]
    public void InitEnvelopeSuccessful()
    {
        Amplifier amp = new();
        AmplifierEnvelope env = new(0, 0, 0, 0);
        amp.Env = env;
        Assert.NotNull(amp.Env);
    }
}
