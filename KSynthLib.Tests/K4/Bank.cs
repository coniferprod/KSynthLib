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
            Assert.Equal(0, bank.Singles.Count);
            Assert.Equal(0, bank.Multis.Count);
        }

        [Fact]
        public void Bank_IsInitializedFromSystemExclusive()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var pathName = Path.Combine(path, $"tmp/A401.SYX");

            byte[] data = File.ReadAllBytes(pathName);
            Bank bank = new Bank(data);
            Assert.Equal(Bank.SinglePatchCount, bank.Singles.Count);
            foreach (SinglePatch sp in bank.Singles)
            {
                Console.WriteLine(sp.Name);
            }
        }

    }

}

