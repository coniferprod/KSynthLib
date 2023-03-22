using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

using KSynthLib.Common;
using KSynthLib.K5000;


namespace KSynthLib.Tests.K5000
{
    public class SystemExclusiveTests
    {
        public SystemExclusiveTests()
        {
        }

        [Fact]
        public void FunctionName_IsCorrect()
        {
            var function = SystemExclusiveFunction.AllBlockDump;
            var name = function.Name();
            Assert.Equal("All Block Dump", name);
        }

        [Fact]
        public void DumpHeader_IsCorrectlyParsed()
        {
            var headerData = new byte[] { 0x00, 0x21, 0x00, 0x0a, 0x00, 0x00};
            var header = new DumpHeader(headerData);
            Assert.Equal(
                new DumpHeader(
                    1,
                    Cardinality.Block,
                    BankIdentifier.A,
                    PatchKind.Single,
                    new byte[] { 0x00, 0x00 }
                ),
                header
            );
        }
    }
}
