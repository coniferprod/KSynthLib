using System;

using SyxPack;
using KSynthLib.K5000;
using KSynthLib.Common;


namespace KSynthLib.Tests.K5000;

public class SingleCommonTests
{
    private readonly SingleCommonSettings singleCommon;

    // Common data from WizooIni.syx:
    private string testData =
        "000002020D410A1000583369221D004A0000002400043A04382A000C0C630042414040" +
        "3F3E410057697A6F6F496E697300000201000201400103400000000000000000000040" +
        "404040404040400000000000";

    public SingleCommonTests()
    {
        byte[] data = Util.HexStringToByteArray(testData);
        Console.Error.WriteLine($"Single Common data from hex string: {data.Length} bytes");
        Console.Error.WriteLine(new HexDump(data));
        singleCommon = new SingleCommonSettings(data);
        Console.Error.WriteLine(singleCommon);
    }

    [Test]
    public void EffectSettings_IsParsedCorrectly()
    {
        Assert.That(EffectAlgorithm.Algorithm1, Is.EqualTo(singleCommon.EffectAlgorithm));
        Assert.That(0, Is.EqualTo(singleCommon.Reverb.ReverbType));
    }

    [Test]
    public void Name_IsParsedCorrectly()
    {
        Assert.That("WizooIni", Is.EqualTo(singleCommon.Name.Value));
    }

    [Test]
    public void DataLength_IsCorrect()
    {
        int length = singleCommon.Data.Count;
        Console.Error.WriteLine($"Length of data generated for SingleCommonSettings = {length}");
        Assert.That(SingleCommonSettings.DataSize, Is.EqualTo(length));
    }

    [Test]
    public void Name_IsSet()
    {
        Assert.NotNull(singleCommon.Name);
    }
}
