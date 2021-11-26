using System;

using Xunit;

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
    }
}
