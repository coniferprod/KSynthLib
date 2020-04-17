using System;
using Xunit;
using KSynthLib.K4;

namespace KSynthLib.Tests.K4
{
    public class DrumNoteTests
    {
        DrumNote note;

        public DrumNoteTests()
        {
            note = new DrumNote();
        }

        [Fact]
        public void ConvertWaveSelect_IsCorrect()
        {
            ushort waveNumber = 200;
            // 200 is 0xC8 hex, and "1100 1000" binary
            // so MSB should be 1 and LSBs should be "100 1000" or 0x48 hex or 72 decimal
            byte waveNumberHigh = 0;
            byte waveNumberLow = 0;
            (waveNumberHigh, waveNumberLow) = note.ConvertWaveSelectToHighAndLow(waveNumber);
            Assert.Equal(1, waveNumberHigh);
            Assert.Equal(0x48, waveNumberLow);
        }

    }
}
