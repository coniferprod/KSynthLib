using System.Collections.Generic;
using Xunit;
using KSynthLib.K4;

namespace KSynthLib.Tests.K4
{
    public class DrumPatchTests
    {
        DrumPatch patch;

        public DrumPatchTests()
        {
            patch = new DrumPatch();
        }

        [Fact]
        public void HasCorrectNumberOfNotes()
        {
            Assert.Equal(DrumPatch.NoteCount, patch.Notes.Count);
        }

        [Fact]
        public void Data_IsCorrectSize()
        {
            List<byte> data = patch.GetSystemExclusiveData();
            Assert.Equal(DrumPatch.DataSize, data.Count);
        }
    }
}
