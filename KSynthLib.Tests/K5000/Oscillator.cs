using System;

using Xunit;

using KSynthLib.K5000;


namespace KSynthLib.Tests.K5000
{
    public class OscillatorTests
    {
        DCOSettings settings;

        public OscillatorTests()
        {
            settings = new DCOSettings();
            Console.Error.WriteLine(settings);
        }

        [Fact]
        public void Coarse_IsCorrectlySet()
        {
            settings.Coarse.Value = -12;
            Assert.Equal(-12, settings.Coarse.Value);
        }
    }
}
