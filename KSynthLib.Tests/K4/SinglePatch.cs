using System.Collections.Generic;

using KSynthLib.K4;
using KSynthLib.Common;

namespace KSynthLib.Tests.K4;

public class SinglePatchTests
{
    byte[] data = new byte[]
    {
        // Name s00...s09 = "Melo Vox 1"
        0x4D, 0x65, 0x6C, 0x6F, 0x20, 0x56, 0x6F, 0x78, 0x20, 0x31,

        0x64, // s10 Volume = 100
        0x20, // s11 = effect = 20H = 0b00100000
        0x06, // s12 = out select
        0x04, // s13 = source mode, poly mode, AM 1>2 and 3>4
        0x0C, // s14 = source mutes + vib shape
        0x02, // s15 = pitch bend + wheel assign
        0x1C, // s16 = vib speed
        0x3F, // s17 = wheel depth
        0x39, // s18 = auto bend time
        0x31, // s19 = auto bend depth
        0x32, // s20 = auto bend KS time
        0x32, // s21 = auto bend vel>dep
        0x32, // s22 = vib prs>vib
        0x3D, // s23 = vibrato depth
        0x00, // s24 = LFO shape
        0x30, // s25 = LFO speed
        0x00, // s26 = LFO delay
        0x32, // s27 = LFO depth
        0x32, // s28 = LFO prs>dep
        0x32, // s29 = pres>freq

        // Sources
        // Common
        0x00, 0x00, 0x02, 0x03,  // s30...s33 = delay for S1...S4
        0x00, 0x00, 0x50, 0x40,  // s34...s37 = wave select h + ks curve for S1...S4
        0x12, 0x12, 0x7E, 0x7F,  // s38...s41 = wave select l
        0x4C, 0x4C, 0x5A, 0x5B,  // s42...s45 = coarse + key track
        0x00, 0x34, 0x02, 0x03,  // s46...s49 = fix
        0x2C, 0x37, 0x34, 0x35,  // s50...s53 = fine
        0x02, 0x02, 0x15, 0x11,  // s54...s57 = prs>frq sw + vib/a.bend sw + vel curve

        // DCA
        0x4B, 0x4B, 0x34, 0x35,  // s58...s61 = envelope level
        0x36, 0x36, 0x34, 0x35,  // s62...s65 = envelope attack
        0x48, 0x48, 0x34, 0x35,  // s66...s69 = envelope decay
        0x5A, 0x5A, 0x34, 0x35,  // s70...s73 = envelope sustain
        0x40, 0x40, 0x02, 0x01,  // s74...s77 = envelope release
        0x41, 0x41, 0x35, 0x36,  // s78...s81 = level mod vel
        0x32, 0x32, 0x35, 0x36,  // s82...s85 = level mod prs
        0x2C, 0x2C, 0x35, 0x36,  // s86...s89 = level mod ks
        0x32, 0x32, 0x35, 0x36,  // s90...s93 = time mod on vel
        0x32, 0x32, 0x35, 0x36,  // s94...s97 = time mod off vel
        0x32, 0x32, 0x33, 0x34,  // s98...s101 = time mod ks

        // DCF
        0x31, 0x51,  // s102, s103 = cutoff for F1 and F2
        0x02, 0x07,  // s104, s105 = resonance + LFO sw
        0x32, 0x34,  // s106, s107 = F1, F2 cutoff mod vel
        0x5B, 0x34,  // s108, s109 = F1, F2 cutoff mod prs
        0x32, 0x34,  // s110, s111 = F1, F2 cutoff mod ks
        0x36, 0x34,  // s112, s113 = F1, F2 dcf env dep
        0x32, 0x33,  // s114, s115 = F1, F2 dcf env vel dep
        0x56, 0x01,  // s116, s117 = F1, F2 dcf env attack
        0x64, 0x02,  // s118, s119 = F1, F2 dcf env decay
        0x32, 0x63,  // s120, s121 = F1, F2 dcf env sustain
        0x56, 0x01,  // s122, s123 = F1, F2 dcf env release
        0x32, 0x33,  // s124, s125 = F1, F2 dcf time mod on vel
        0x32, 0x33,  // s126, s127 = F1, F2 dcf time mod off vel
        0x32, 0x33,  // s128, s129 = F1, F2 dcf time mod ks
        0x6E,        // checksum
    };

    byte[] filterData = new byte[]
    {
        // DCF
        0x31, 0x51,  // s102, s103 = cutoff for F1 and F2
        0x02, 0x07,  // s104, s105 = resonance + LFO sw
        0x32, 0x34,  // s106, s107 = F1, F2 cutoff mod vel
        0x5B, 0x34,  // s108, s109 = F1, F2 cutoff mod prs
        0x32, 0x34,  // s110, s111 = F1, F2 cutoff mod ks
        0x36, 0x34,  // s112, s113 = F1, F2 dcf env dep
        0x32, 0x33,  // s114, s115 = F1, F2 dcf env vel dep
        0x56, 0x01,  // s116, s117 = F1, F2 dcf env attack
        0x64, 0x02,  // s118, s119 = F1, F2 dcf env decay
        0x32, 0x63,  // s120, s121 = F1, F2 dcf env sustain
        0x56, 0x01,  // s122, s123 = F1, F2 dcf env release
        0x32, 0x33,  // s124, s125 = F1, F2 dcf time mod on vel
        0x32, 0x33,  // s126, s127 = F1, F2 dcf time mod off vel
        0x32, 0x33,  // s128, s129 = F1, F2 dcf time mod ks
    };

    byte[] filter1Bytes = new byte[]
    {
        0x31, 0x02, 0x32, 0x5B, 0x32, 0x36, 0x32, 0x56, 0x64, 0x32, 0x56, 0x32, 0x32, 0x32
    };

    byte[] filter2Bytes = new byte[]
    {
        0x51, 0x07, 0x34, 0x34, 0x34, 0x34, 0x33, 0x01, 0x02, 0x63, 0x01, 0x33, 0x33, 0x33
    };

    byte[] amplifierData = new byte[]
    {
        // DCA
        0x4B, 0x4B, 0x34, 0x35,  // s58...s61 = envelope level
        0x36, 0x36, 0x34, 0x35,  // s62...s65 = envelope attack
        0x48, 0x48, 0x34, 0x35,  // s66...s69 = envelope decay
        0x5A, 0x5A, 0x34, 0x35,  // s70...s73 = envelope sustain
        0x40, 0x40, 0x02, 0x01,  // s74...s77 = envelope release
        0x41, 0x41, 0x35, 0x36,  // s78...s81 = level mod vel
        0x32, 0x32, 0x35, 0x36,  // s82...s85 = level mod prs
        0x2C, 0x2C, 0x35, 0x36,  // s86...s89 = level mod ks
        0x32, 0x32, 0x35, 0x36,  // s90...s93 = time mod on vel
        0x32, 0x32, 0x35, 0x36,  // s94...s97 = time mod off vel
        0x32, 0x32, 0x33, 0x34,  // s98...s101 = time mod ks
    };

    byte[] source1AmplifierData = new byte[]
    {
        0x4B, 0x36, 0x48, 0x5A, 0x40, 0x41, 0x32, 0x2C, 0x32, 0x32, 0x32
    };

    byte[] source2AmplifierData = new byte[]
    {
        0x4B, 0x36, 0x48, 0x5A, 0x40, 0x41, 0x32, 0x2C, 0x32, 0x32, 0x32
    };

    byte[] source3AmplifierData = new byte[]
    {
        0x34, 0x34, 0x34, 0x34, 0x02, 0x35, 0x35, 0x35, 0x35, 0x35, 0x33
    };

    byte[] source4AmplifierData = new byte[]
    {
        0x35, 0x35, 0x35, 0x35, 0x01, 0x36, 0x36, 0x36, 0x36, 0x36, 0x34
    };

    public SinglePatchTests()
    {
    }

/*
patch with the name right-arrow left-arrow exclamation-mark double-quote hash dollar-sign
percent-sign ampersand single-quote open-parenthesis
40 00 20 00 04 00 3E
name part = 7E 7F 21 22 23 24 25 26 27 28
46 39 06 04 0C 27 17 56 06 22 32 32 32 34 03 64 00 32 32 32 00 00 02 03 00 70 50 40 0E 0E 7E 7F 4C 4C 5A 5B 35 01 02 03 2D 37 34 35 03 05 15 11 4B 4B 34 35 16 17 34 35 64 64 34 35 64 64 34 35 1F 1F 02 01 32 32 35 36 32 32 35 36 32 32 35 36 32 32 35 36 32 32 35 36 32 33 33 34 2F 51 09 07 44 34 64 34 3B 34 32 34 32 33 21 01 3F 02 32 63 30 01 32 33 32 33 32 33 40
// this is actually "Taurs2Pole" from A401 in its entirety
*/

/*
patch with name at-sign left-square-bracket yen-sign right-square-bracket caret underscore
backtick left-brace bar right-brace
40 00 20 00 04 00 3F
name part = 40 5B 5C 5D 5E 5F 60 7B 7C 7D
50 00 00 00 0E 22 30 4E 03 2A 32 32 3F 32 00 00 00 32 32 43 00 00 00 00 00 00 00 00 09 14 00 00 58 4C 18 18 00 00 00 00 32 35 32 32 03 03 00 00 52 50 64 64 10 10 00 00 48 45 32 32 5A 4B 00 00 40 0E 32 32 40 40 32 32 32 32 32 32 2F 2F 32 32 43 32 32 32 32 32 32 32 32 32 32 32 0D 64 0D 00 4A 32 32 32 3E 32 47 32 42 32 21 00 3D 32 32 32 58 32 32 32 32 32 32 32 01
*/


    [Test]
    public void InitFromData_IsSuccessful()
    {
        SinglePatch singlePatch = new SinglePatch(data);
        //Console.Error.WriteLine($"Single patch name = '{singlePatch.Name}'");

        Assert.That("Melo Vox 1", Is.EqualTo(singlePatch.Name.Value));
    }

    [Test]
    public void FilterBytesAreSeparatedCorrectly()
    {
        List<byte[]> fb = Util.SeparateBytes(filterData, 2);
        Assert.That(filter1Bytes, Is.EqualTo(fb[0]));
        Assert.That(filter2Bytes, Is.EqualTo(fb[1]));
    }

    [Test]
    public void AmplifierBytesAreSeparatedCorrecly()
    {
        List<byte[]> ab = Util.SeparateBytes(amplifierData, 4);
        Assert.That(source1AmplifierData, Is.EqualTo(ab[0]));
        Assert.That(source2AmplifierData, Is.EqualTo(ab[1]));
        Assert.That(source3AmplifierData, Is.EqualTo(ab[2]));
        Assert.That(source4AmplifierData, Is.EqualTo(ab[3]));
    }

    [Test]
    public void SystemExclusiveData_IsCorrectLength()
    {
        SinglePatch single = new ();

        List<byte> data = single.Data;
        Assert.That(SinglePatch.DataSize, Is.EqualTo(data.Count));
    }

    [Test]
    public void InitSuccessful()
    {
        SinglePatch single = new ();
        Assert.NotNull(single);
    }

    [Test]
    public void NameIsTruncatedWhenSet()
    {
        SinglePatch sp = new ();
        string longName = "MyPatch*WithTooLongAName";
        sp.Name = new PatchName(longName);
        Assert.That(PatchName.Length, Is.EqualTo(sp.Name.Value.Length));
    }
}
