using KSynthLib.K5000;


namespace KSynthLib.Tests.K5000;

public class SystemExclusiveTests
{
    public SystemExclusiveTests()
    {
    }

    [Test]
    public void FunctionName_IsCorrect()
    {
        var function = SystemExclusiveFunction.AllBlockDump;
        var name = function.Name();
        Assert.That("All Block Dump", Is.EqualTo(name));
    }

    [Test]
    public void DumpHeader_OneA_IsCorrectlyParsed()
    {
        var headerData = new byte[]
        {
            0x00,  // channel
            0x20,  // 20h
            0x00,  // 00h
            0x0a,  // 0Ah
            0x00,  // 00h
            0x00,  // 00h
            0x00,  // sub1 = 00h
        };
        var header = new DumpHeader(headerData);
        Assert.That(headerData, Is.EqualTo(header.Data));
    }

    [Test]
    public void DumpHeader_BlockA_IsCorrectlyParsed()
    {
        var headerData = new byte[]
        {
            0x00,  // channel
            (byte)Cardinality.Block,
            0x00,
            0x0a,
            0x00,
            (byte)BankIdentifier.A,

            // tone map 19 bytes
            0b0111_1111, 0b0111_1111, 0b0111_1111, 0b0111_1111,
            0b0111_1111, 0b0111_1111, 0b0111_1111, 0b0111_1111,
            0b0111_1111, 0b0111_1111, 0b0111_1111, 0b0111_1111,
            0b0111_1111, 0b0111_1111, 0b0000_0111, 0x00,
            0x00, 0x00, 0x00
        };
        var header = new DumpHeader(headerData);

        // Construct a tone map with tones 001..101 (0..100) included
        bool[] include = new bool[ToneMap.ToneCount];
        for (int i = 0; i < 101; i++)
        {
            include[i] = true;
        }

        var actual = new DumpHeader(
            new MIDIChannel(1),
            Cardinality.Block,
            BankIdentifier.A,
            PatchKind.Single,
            new PatchNumber(),  // don't care
            new ToneMap(include),
            new InstrumentNumber()  // don't care
        );

        Assert.That(header, Is.EqualTo(actual));
    }
}
