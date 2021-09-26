using System;
using System.IO;
using System.Diagnostics;
using Xunit;
using KSynthLib.Common;
using KSynthLib.K4;

namespace KSynthLib.Tests.K4
{
    public class BankTests
    {
        [Fact]
        public void Bank_IsInitializedEmpty()
        {
            Bank bank = new Bank();
            Assert.Empty(bank.Singles);
            Assert.Empty(bank.Multis);
            Assert.Empty(bank.Effects);
        }

        [Fact]
        public void Bank_IsInitializedFromSystemExclusive()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var pathName = Path.Combine(path, "Kawai K4 Sounds/A401.SYX");

            byte[] fileData = File.ReadAllBytes(pathName);
            Console.WriteLine($"Read {fileData.Length} bytes from file '{pathName}'");

            // Strip the SysEx header from the data before parsing the bank
            int bankDataLength = fileData.Length - SystemExclusiveHeader.DataSize - 1;
            byte[] data = new byte[bankDataLength];
            int srcOffset = SystemExclusiveHeader.DataSize;
            Console.WriteLine($"BlockCopy: srcOffset={srcOffset} dstOffset=0 count={bankDataLength}");
            Buffer.BlockCopy(fileData, srcOffset, data, 0, bankDataLength);
            Console.WriteLine($"About to parse bank from {data.Length} bytes of data");
            //Console.WriteLine(Util.HexDump(data));

            Bank bank = new Bank(data);
            Assert.Equal(Bank.SinglePatchCount, bank.Singles.Count);
            foreach (SinglePatch sp in bank.Singles)
            {
                Console.Error.WriteLine(sp.Name);
            }

            Assert.Equal(Bank.MultiPatchCount, bank.Multis.Count);

            Console.WriteLine($"Drum note count = {DrumPatch.NoteCount}");

            Assert.Equal(DrumPatch.NoteCount, bank.Drum.Notes.Count);
            Assert.Equal(Bank.EffectPatchCount, bank.Effects.Count);
        }
    }
}
