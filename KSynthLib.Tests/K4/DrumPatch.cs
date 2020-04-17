using System;
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
            byte[] data = patch.ToData();
            Assert.Equal(DrumPatch.DataSize, data.Length);
        }
    }
}
