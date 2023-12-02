using System;
using System.IO;

using KSynthLib.K4;

namespace KSynthLib.Tests.K4;

public class BankTests
{
    [Test]
    public void Bank_IsInitializedEmpty()
    {
        Bank bank = new();
        Assert.That(bank.Singles, Is.Empty);
        Assert.That(bank.Multis, Is.Empty);
        Assert.That(bank.Effects, Is.Empty);
    }

    [Test]
    public void Bank_IsInitializedFromSystemExclusive()
    {
        var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var pathName = Path.Combine(path, "Kawai K4 Sounds/A401.SYX");

        byte[] fileData = File.ReadAllBytes(pathName);
        Console.WriteLine($"Read {fileData.Length} bytes from file '{pathName}'");

        // Strip the SysEx header from the data before parsing the bank
        int bankDataLength = fileData.Length - SystemExclusiveHeader.DataSize - 1;
        byte[] data = new byte[bankDataLength];
        int srcOffset = SystemExclusiveHeader.DataSize;
        Console.WriteLine($"BlockCopy: srcOffset={srcOffset} dstOffset=0 count={bankDataLength}");
        Buffer.BlockCopy(fileData, srcOffset, data, 0, bankDataLength);
        Console.WriteLine($"About to parse bank from {data.Length} bytes of data");
        //Console.WriteLine(Util.HexDump(data));

        Bank bank = new(data);
        Assert.That(Bank.SinglePatchCount, Is.EqualTo(bank.Singles.Count));
        foreach (SinglePatch sp in bank.Singles)
        {
            Console.Error.WriteLine(sp.Name);
        }

        Assert.That(Bank.MultiPatchCount, Is.EqualTo(bank.Multis.Count));

        Console.WriteLine($"Drum note count = {DrumPatch.NoteCount}");

        Assert.That(DrumPatch.NoteCount, Is.EqualTo(bank.Drum.Notes.Count));
        Assert.That(Bank.EffectPatchCount, Is.EqualTo(bank.Effects.Count));
    }
}
