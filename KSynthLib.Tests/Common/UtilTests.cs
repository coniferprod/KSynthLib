using System.Collections.Generic;
using System.Linq;

using KSynthLib.Common;

namespace KSynthLib.Tests.Common;

public class UtilTests
{
    public UtilTests()
    {

    }

    [Test]
    public void HexString_IsConvertedToByteArray()
    {
        string hexString = "112233";
        byte[] actual = Util.HexStringToByteArray(hexString);
        byte[] expected = new byte[] { 0x11, 0x22, 0x33 };
        Assert.That(expected, Is.EqualTo(actual));
    }

    [Test]
    public void BytesAreInterleaved()
    {
        var a1 = new List<byte>() { 1, 3, 5 };
        var a2 = new List<byte>() { 2, 4, 6 };

        var expected = new List<byte>() { 1, 2, 3, 4, 5, 6 };
        var actual = a1.Interleave(a2);
        Assert.That(expected, Is.EqualTo(actual));
    }

    [Test]
    public void ListsAreInterleaved()
    {
        var a1 = new List<byte>() { 1, 5, 9 };
        var a2 = new List<byte>() { 2, 6, 10 };
        var a3 = new List<byte>() { 3, 7, 11 };
        var a4 = new List<byte>() { 4, 8, 12 };

        var expected = new List<byte>() {
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12
        };

        var allLists = new List<List<byte>>() { a1, a2, a3, a4 };
        var actual = allLists.Interleave().ToList();
        Assert.That(expected, Is.EqualTo(actual));
    }

    [Test]
    public void BytesAreDivided()
    {
        var a = new List<byte>();
        a.Add(1);
        a.Add(4);
        a.Add(2);
        a.Add(5);
        a.Add(3);
        a.Add(6);

        var a1 = new List<byte>();
        a1.Add(1);
        a1.Add(2);
        a1.Add(3);

        var a2 = new List<byte>();
        a2.Add(4);
        a2.Add(5);
        a2.Add(6);

        var (actual1, actual2) = Util.DivideBytes(a);
        Assert.That(a1, Is.EqualTo(actual1));
        Assert.That(a2, Is.EqualTo(actual2));
    }
}
