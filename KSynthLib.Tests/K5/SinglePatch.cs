using System;

using Xunit;

using KSynthLib.K5;


namespace KSynthLib.Tests.K5
{
    public class SinglePatchTests
    {
        private SinglePatch patch;

        public SinglePatchTests()
        {
            patch = new SinglePatch();
            Console.WriteLine(patch);
        }

        [Fact]
        public void Name_IsTruncatedCorrectly()
        {
            string tooLong = "MyPatch*WithTooLongAName";
            patch.Name = tooLong;
            string actualName = patch.Name;
            Assert.Equal(SinglePatch.NameLength, actualName.Length);
        }
    }
}
