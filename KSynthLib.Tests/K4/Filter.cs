using KSynthLib.K4;

namespace KSynthLib.Tests.K4;

public class FilterTests
{
    private readonly Filter _filter;

    public FilterTests()
    {
        _filter = new Filter();

    }

    [Test]
    public void DataLength_IsCorrect()
    {
        int length = _filter.Data.Count;
        Assert.That(Filter.DataSize, Is.EqualTo(length));
    }
}
