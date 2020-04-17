using System;
using Xunit;
using KSynthLib.K4;

namespace KSynthLib.Tests.K4
{
    public class EffectPatchTests
    {
        EffectPatch patch;

        public EffectPatchTests()
        {
            patch = new EffectPatch();
        }

        [Fact]
        public void HasCorrectNumberOfSubmixes()
        {
            Assert.Equal(EffectPatch.SubmixCount, patch.Submixes.Length);
        }

        [Fact]
        public void Data_IsCorrectSize()
        {
            byte[] data = patch.ToData();
            Assert.Equal(EffectPatch.DataSize, data.Length);
        }
    }
}
