using Xunit;
using KSynthLib.K4;

namespace KSynthLib.Tests.K4
{
    public class SinglePatch_Init
    {
        public SinglePatch_Init()
        {
        }

        [Fact]
        public void InitSuccessful()
        {
            SinglePatch single = new SinglePatch();
            Assert.NotNull(single);
        }
    }
}
