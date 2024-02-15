using System;

using SyxPack;
using KSynthLib.K5000;
using KSynthLib.Common;


namespace KSynthLib.Tests.K5000;

public class MultiPatchTests
{
    // Multi/combi data: (K5000S v4.04 multi M01)
    // 0F = checksum
    // Common data:
    // 1: Effect = Algorithm 03
    // 2: Reverb: 00 64 0F 1C 1E 18
    // 8: Effect1: 24 28 04 4A 50 4F
    // 14: Effect2: 0B 00 00 05 09 00
    // 20: Effect3: 1A 1C 03 4A 00 31
    // 26: Effect4: 19 56 09 35 00 2D
    // 32: GEQ: 45 44 42 40 3E 3D 41
    // 39: COMMON: Name: 4D 65 67 61 50 6F 77 72
    // 47: Volume: 7F
    // 48: Mute: 03
    // 49: Control: 00 00 40 01 00 40
    private string testData =
        "0F0300640F1C1E182428044A504F0B00000509001A1C034A003119560935002D" +
        "454442403E3D414D656761506F77727F03000040010040";

    private readonly MultiPatch multiPatch;

    public MultiPatchTests()
    {
        byte[] data = Util.HexStringToByteArray(testData);
        Console.Error.WriteLine($"Single Common data from hex string: {data.Length} bytes");
        Console.Error.WriteLine(new HexDump(data));
        multiPatch = new MultiPatch(data);
        Console.Error.WriteLine(multiPatch);
    }
}
