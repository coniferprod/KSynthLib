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

        [Fact]
        public void DumpHeader_OneA_IsCorrectlyParsed()
        {
            var headerData = new byte[]
            {
                0x00,  // channel
                0x20,  // 20h
                0x00,  // 00h
                0x0a,  // 0Ah
                0x00,  // 00h
                0x00,  // 00h
                0x00,  // sub1 = 00h
            };
            var header = new DumpHeader(headerData);
            Assert.Equal(headerData, header.Data);
        }

        [Fact]
        public void DumpHeader_BlockA_IsCorrectlyParsed()
        {
            var headerData = new byte[]
            {
                0x00,  // channel
                0x21,  // 21h
                0x00,  // 00h
                0x0a,  // 0Ah
                0x00,  // 00h
                0x00,  // 00h
                0x00, 0x00, 0x00, 0x00,  // tone map 19 bytes
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00,
            };
            var header = new DumpHeader(headerData);
            Assert.Equal(headerData, header.Data);
        }
    }
}
