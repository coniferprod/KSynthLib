using System.Collections.Generic;

using KSynthLib.K4;
using KSynthLib.Common;

namespace KSynthLib.Tests.K4;

public class SourceTests
{
    byte[] sourceData = new byte[]
    {
        // Common
        0x00, 0x00, 0x02, 0x03,  // s30...s33 = delay for S1...S4
        0x00, 0x00, 0x50, 0x40,  // s34...s37 = wave select h + ks curve for S1...S4
        0x12, 0x12, 0x7E, 0x7F,  // s38...s41 = wave select l
        0x4C, 0x4C, 0x5A, 0x5B,  // s42...s45 = coarse + key track
        0x00, 0x34, 0x02, 0x03,  // s46...s49 = fix
        0x2C, 0x37, 0x34, 0x35,  // s50...s53 = fine
        0x02, 0x02, 0x15, 0x11,  // s54...s57 = prs>frq sw + vib/a.bend sw + vel curve
    };

    [Test]
    public void InitFromData_IsSuccessful()
    {
        List<byte[]> sourceBytes = Util.SeparateBytes(sourceData, 4);
        Source s1 = new Source(sourceBytes[0]);
        Assert.That(0, Is.EqualTo(s1.Delay.Value));
    }
}
