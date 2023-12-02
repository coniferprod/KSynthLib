using KSynthLib.K5;

namespace KSynthLib.Tests.K5;

public class SinglePatchTests
{
    private SinglePatch patch;

    public SinglePatchTests()
    {
        patch = new SinglePatch();
    }

    [Test]
    public void Name_IsTruncatedCorrectly()
    {
        string tooLong = "MyPatch*WithTooLongAName";
        patch.Name = tooLong;
        string actualName = patch.Name;
        Assert.That(SinglePatch.NameLength, Is.EqualTo(actualName.Length));
    }
}
