using KSynthLib.K4;

namespace KSynthLib.Tests.K4;

public class SystemExclusiveTests
{
    public SystemExclusiveTests()
    {
    }

    [Test]
    public void ProgramChange_HasCorrectLength()
    {
        byte[] data = new byte[]
        {
            0x00, // channel 1
            0x30, // program change (INT/EXT)
            0x00, // synth group
            0x04, // K4/K4r ID no.
            0x00, // substatus1 = INT
        };

        var header = new SystemExclusiveHeader(data);
        Assert.That(data.Length, Is.EqualTo(header.DataLength));
    }

    [Test]
    public void WriteError_HasCorrectLength()
    {
        byte[] data = new byte[]
        {
            0x00, // channel 1
            0x41, // write error
            0x00, // synth group
            0x04, // K4/K4r ID no.
        };

        var header = new SystemExclusiveHeader(data);
        Assert.That(data.Length, Is.EqualTo(header.DataLength));
    }

    [Test]
    public void AllPatchDataDump_HasCorrectLength()
    {
        byte[] data = new byte[]
        {
            0x00, // channel 1
            0x22, // all patch data dump
            0x00, // synth group
            0x04, // K4/K4r ID no.
            0x00, // INT
            0x00,
        };

        var header = new SystemExclusiveHeader(data);
        Assert.That(data.Length, Is.EqualTo(header.DataLength));
    }
}
