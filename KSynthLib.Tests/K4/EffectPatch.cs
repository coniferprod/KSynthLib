using KSynthLib.K4;

namespace KSynthLib.Tests.K4;

public class EffectPatchTests
{
    EffectPatch patch;

    public EffectPatchTests()
    {
        patch = new EffectPatch();
    }

    [Test]
    public void HasCorrectNumberOfSubmixes()
    {
        Assert.That(EffectPatch.SubmixCount, Is.EqualTo(patch.Submixes.Length));
    }

    [Test]
    public void Data_IsCorrectSize()
    {
        byte[] data = patch.ToData().ToArray();
        Assert.That(EffectPatch.DataSize, Is.EqualTo(data.Length));
    }
}
