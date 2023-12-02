using KSynthLib.Common;

namespace KSynthLib.Tests.K4;

public class PatchUtilTests
{
    [Test]
    public void GetPatchName_HandlesValid()
    {
        string name = PatchUtil.GetPatchName(0);
        Assert.That("A- 1", Is.EqualTo(name));
    }

    [Test]
    public void GetPatchNumber_HandlesInvalid()
    {
        int number = PatchUtil.GetPatchNumber("a1");
        Assert.That(0, Is.EqualTo(number));
    }
}
