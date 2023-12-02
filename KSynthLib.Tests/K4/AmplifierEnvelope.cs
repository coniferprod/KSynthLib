using KSynthLib.K4;

namespace KSynthLib.Tests.K4;

public class AmplifierEnvelopeTests
{
    [Test]
    public void InitSuccessful()
    {
        AmplifierEnvelope env = new();
        Assert.NotNull(env);
    }
}
