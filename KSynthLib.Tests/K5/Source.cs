using System;

using Xunit;

using KSynthLib.K5;


namespace KSynthLib.Tests.K5
{
    public class SourceTests
    {
        private Source source;

        public SourceTests()
        {
            source = new Source();
            Console.Error.WriteLine(source);
        }

        [Fact]
        public void HasHarmonics()
        {
            Assert.Equal(Source.HarmonicCount, source.Harmonics.Length);
        }
    }

    public class SourceSettingsTests
    {
        private SourceSettings settings;

        public SourceSettingsTests()
        {
            settings = new SourceSettings();
        }

        [Fact]
        public void Delay_IsSetCorrectly()
        {
            settings.Delay = new PositiveDepth(20);
            Assert.Equal(20, settings.Delay.Value);
        }
    }
}
