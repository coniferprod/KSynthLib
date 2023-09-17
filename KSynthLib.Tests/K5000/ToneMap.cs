using Xunit;

using KSynthLib.K5000;


namespace KSynthLib.Tests.K5000
{
    public class ToneMapTests
    {
        public ToneMapTests()
        {
        }

        [Fact]
        public void Empty()
        {
            bool[] include = new bool[ToneMap.ToneCount];
            for (int i = 0; i < ToneMap.ToneCount; i++)
            {
                include[i] = false;
            }

            var map = new ToneMap(include);
            Assert.Equal(0, map.Count);
        }

        [Fact]
        public void Full()
        {
            bool[] include = new bool[ToneMap.ToneCount];
            for (int i = 0; i < ToneMap.ToneCount; i++)
            {
                include[i] = true;
            }

            var map = new ToneMap(include);
            Assert.Equal(ToneMap.ToneCount, map.Count);
        }

        [Fact]
        public void Some()
        {
            // In a tone map, only the bottom seven bits of each byte are used.
            // So to indicate that tones A001...A007 are included, the first byte
            // must be 0b0111_1111.
            // The whole tone map that includes A001...A101 (or 0...100) would be:
            // sub1 = 0b0111_1111  -- tones A001...A007
            // sub2 = 0b0111_1111  -- tones A008...A014
            // sub3 = 0b0111_1111  -- tones A015...A021
            // sub4 = 0b0111_1111  -- tones A022...A028
            // sub5 = 0b0111_1111  -- tones A029...A035
            // sub6 = 0b0111_1111  -- tones A036...A042
            // sub7 = 0b0111_1111  -- tones A043...A049
            // sub8 = 0b0111_1111  -- tones A050...A056
            // sub9 = 0b0111_1111  -- tones A057...A063
            // sub10 = 0b0111_1111 -- tones A064...A070
            // sub11 = 0b0111_1111 -- tones A071...A077
            // sub12 = 0b0111_1111 -- tones A078...A084
            // sub13 = 0b0111_1111 -- tones A085...A091
            // sub14 = 0b0111_1111 -- tones A092...A098
            // sub15 = 0b0000_0111 -- tones A099...A101
            // sub16...sub19 = 0

            bool[] include = new bool[ToneMap.ToneCount];
            for (int i = 0; i < 101; i++)
            {
                include[i] = true;
            }

            var map = new ToneMap(include);
            Assert.Equal(101, map.Count);
        }
    }
}
