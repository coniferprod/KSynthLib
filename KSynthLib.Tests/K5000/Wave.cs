using System;
using Xunit;
using KSynthLib.K5000;

namespace KSynthLib.Tests.K5000
{
    public class WaveTests
    {
        [Fact]
        public void Number_IsCorrectlySet()
        {
            var wave = new Wave(411);
            Assert.Equal(411, wave.Number);
        }

        [Fact]
        public void Name_IsCorrectlySet()
        {
            var wave = new Wave(411);
            Assert.Equal("Syn Saw1 Cyc", wave.Name);
        }
    }
}
