using System.Collections.Generic;

using KSynthLib.K4;

namespace KSynthLib.Tests.K4;

public class DrumPatchTests
{
    DrumPatch patch;

    public DrumPatchTests()
    {
        patch = new DrumPatch();
    }

    [Test]
    public void HasCorrectNumberOfNotes()
    {
        Assert.That(DrumPatch.NoteCount, Is.EqualTo(patch.Notes.Count));
    }

    [Test]
    public void Data_IsCorrectSize()
    {
        List<byte> data = patch.Data;
        Assert.That(DrumPatch.DataSize, Is.EqualTo(data.Count));
    }
}
