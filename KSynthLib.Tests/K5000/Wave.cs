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

        [Fact]
        public void NumberFrom_IsCorrect()
        {
            var wave = new Wave(411);
            var (high, low) = wave.WaveSelect;
            Assert.Equal(0x03, high);
            Assert.Equal(0x1b, low);
        }

        [Fact]
        public void WaveSelect_IsCorrect()
        {
            var number = Wave.NumberFrom(0x03, 0x1b);
            Assert.Equal(411, number);
        }
    }
}
