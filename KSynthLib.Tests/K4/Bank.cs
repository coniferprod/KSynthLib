using System;
using System.IO;
using System.Diagnostics;
using Xunit;
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
        }

        [Fact]
        public void Bank_IsInitializedFromSystemExclusive()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var pathName = Path.Combine(path, "Kawai K4 Sounds/A401.SYX");

            byte[] data = File.ReadAllBytes(pathName);
            Bank bank = new Bank(data);
            Assert.Equal(Bank.SinglePatchCount, bank.Singles.Count);
            foreach (SinglePatch sp in bank.Singles)
            {
                Console.Error.WriteLine(sp.Name);
            }
        }

    }

}

