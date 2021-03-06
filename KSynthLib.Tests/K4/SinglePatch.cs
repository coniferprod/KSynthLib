using System;
using System.Text.RegularExpressions;
using Xunit;
using KSynthLib.K4;
using KSynthLib.Common;

namespace KSynthLib.Tests.K4
{
    public class SinglePatchTests
    {
        // Hex bytes of patch A-1 from A401.SYX, copied with Hex Fiend in macOS:
        string rawString = "4D 65 6C 6F 20 56 6F 78 20 31 64 20 06 04 0C 02 1C 3F 39 31 32 32 32 3D 00 30 00 32 32 32 00 00 02 03 00 00 50 40 12 12 7E 7F 4C 4C 5A 5B 00 34 02 03 2C 37 34 35 02 02 15 11 4B 4B 34 35 36 36 34 35 48 48 34 35 5A 5A 34 35 40 40 02 01 41 41 35 36 32 32 35 36 2C 2C 35 36 32 32 35 36 32 32 35 36 32 32 33 34 31 51 02 07 32 34 5B 34 32 34 36 34 32 33 56 01 64 02 32 63 56 01 32 33 32 33 32 33 6E";

        // name = first 10 bytes
        // 4D 65 6C 6F 20 56 6F 78 20 31 = "Melo Vox 1"

        // volume = 64h = 100
        // effect = 20h = 0b00100000

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


        [Fact]
        public void InitFromData_IsSuccessful()
        {
            // Interpret the bytes as hex and put them in a byte array:
            string hexString = rawString.Replace(" ", "");
            byte[] data = Util.HexStringToByteArray(hexString);

            SinglePatch singlePatch = new SinglePatch(data);
            //Console.Error.WriteLine($"Single patch name = '{singlePatch.Name}'");

            Assert.Equal("Melo Vox 1", singlePatch.Name);
        }

        [Fact]
        public void SystemExclusiveData_IsCorrectLength()
        {
            SinglePatch single = new SinglePatch();

            byte[] data = single.ToData();
            Assert.Equal(SinglePatch.DataSize, data.Length);
        }

        [Fact]
        public void InitSuccessful()
        {
            SinglePatch single = new SinglePatch();
            Assert.NotNull(single);
        }

        [Fact]
        public void NameIsTruncatedWhenSet()
        {
            SinglePatch sp = new SinglePatch();
            string longName = "MyPatch*WithTooLongAName";
            sp.Name = longName;
            Assert.Equal(SinglePatch.NameLength, sp.Name.Length);
        }
    }
}
