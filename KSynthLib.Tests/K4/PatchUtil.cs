using System;

using Xunit;
using KSynthLib.K4;
using KSynthLib.Common;

namespace KSynthLib.Tests.K4
{
    public class PatchUtilTests
    {
        [Fact]
        public void GetPatchName_HandlesValid()
        {
            string name = PatchUtil.GetPatchName(0);
            Assert.Equal("A- 1", name);
        }

        [Fact]
        public void GetPatchNumber_HandlesInvalid()
        {
            int number = PatchUtil.GetPatchNumber("a1");
            Assert.Equal(0, number);
        }
    }
}
