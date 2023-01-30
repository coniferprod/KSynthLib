using System;
using System.Collections.Generic;
using System.Text;

using KSynthLib.Common;


namespace KSynthLib.K4
{
    public enum SystemExclusiveFunction: byte
    {
        OnePatchDumpRequest = 0x00,
        BlockPatchDumpRequest = 0x01,
        AllPatchDumpRequest = 0x02,
        ParameterSend = 0x10,
        OnePatchDataDump = 0x20,
        BlockPatchDataDump = 0x21,
        AllPatchDataDump = 0x22,
        EditBufferDump = 0x23,
        ProgramChange = 0x30,
        WriteComplete = 0x40,
        WriteError = 0x41,
        WriteErrorProtect = 0x42,
        WriteErrorNoCard = 0x43
    }

    public static class SystemExclusiveFunctionExtensions
    {
        public static string Name(this SystemExclusiveFunction function)
        {
            var functionNames = new Dictionary<SystemExclusiveFunction, string>()
            {
                { SystemExclusiveFunction.OnePatchDumpRequest, "One Patch Dump Request" },
                { SystemExclusiveFunction.BlockPatchDumpRequest, "Block Patch Dump Request" },
                { SystemExclusiveFunction.AllPatchDumpRequest, "All Patch Dump Request" },
                { SystemExclusiveFunction.ParameterSend, "Parameter Send" },
                { SystemExclusiveFunction.OnePatchDataDump, "One Patch Data Dump" },
                { SystemExclusiveFunction.BlockPatchDataDump, "Block Patch Data Dump" },
                { SystemExclusiveFunction.AllPatchDataDump, "All Patch Data Dump" },
                { SystemExclusiveFunction.EditBufferDump, "Edit Buffer Dump" },
                { SystemExclusiveFunction.ProgramChange, "Program Change" },
                { SystemExclusiveFunction.WriteComplete, "Write Complete" },
                { SystemExclusiveFunction.WriteError, "Write Error" },
                { SystemExclusiveFunction.WriteErrorProtect, "Write Error (Protect)" },
                { SystemExclusiveFunction.WriteErrorNoCard, "Write Error (No Card)" }
            };

            return functionNames.GetValueOrDefault(function, "(unknown)");
        }
    }

    public class SystemExclusiveHeader: ISystemExclusiveData
    {
        public const int DataSize = 6;

	    public int Channel;
	    public SystemExclusiveFunction Function;
	    public sbyte Group;
	    public sbyte MachineID;
	    public sbyte Substatus1;
	    public sbyte Substatus2;

        public SystemExclusiveHeader(byte channel)
        {
            this.Channel = channel;
        }

        public SystemExclusiveHeader(byte[] data)
        {
            this.Channel = (sbyte)(data[0] + 1);
            this.Function = (SystemExclusiveFunction)data[1];
            this.Group = (sbyte)data[2];
            this.MachineID = (sbyte)data[3];
            this.Substatus1 = (sbyte)data[4];
            this.Substatus2 = (sbyte)data[5];
        }

        public override bool Equals(object obj) => this.Equals(obj as SystemExclusiveHeader);

        public bool Equals(SystemExclusiveHeader other)
        {
            if (other is null)
            {
                return false;
            }

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != other.GetType())
            {
                return false;
            }

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            return (this.Channel == other.Channel)
                && (this.Function == other.Function)
                && (this.Group == other.Group)
                && (this.MachineID == other.MachineID)
                && (this.Substatus1 == other.Substatus1)
                && (this.Substatus2 == other.Substatus2);
        }

        public override int GetHashCode() => (this.Channel, this.Function, this.Group, this.MachineID, this.Substatus1, this.Substatus2).GetHashCode();

        public List<byte> GetSystemExclusiveData()
        {
            var data = new List<byte>();

            data.Add((byte)(this.Channel - 1));
            data.Add((byte)this.Function);
            data.Add((byte)this.Group);
            data.Add((byte)this.MachineID);
            data.Add((byte)this.Substatus1);
            data.Add((byte)this.Substatus2);

            return data;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Channel: {Channel}");
            sb.AppendLine($"Function: {SystemExclusiveFunctionExtensions.Name(this.Function)}");
            sb.AppendLine(string.Format("Group: {0:X2}h", this.Group));
            sb.AppendLine(string.Format("MachineID: {0:X2}h", this.MachineID));
            sb.AppendLine(string.Format("Sub status 1: {0:X2}h", this.Substatus1));
            sb.AppendLine(string.Format("Sub status 2: {0:X2}h", this.Substatus2));

            return sb.ToString();
        }

        public static bool IsValidFunction(byte functionByte)
        {
            foreach (byte b in Enum.GetValues(typeof(SystemExclusiveFunction)))
            {
                if (b == functionByte)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public enum Cardinality
    {
        One,
        Block,
        All,
        Unknown
    }

    public enum Locality
    {
        Internal,
        External,
        Unknown
    }

    public enum Kind
    {
        SinglePatch,
        MultiPatch,
        EffectPatch,
        DrumPatch,
        All,
        Unknown
    }

    public class DumpDescriptor
    {
        public Locality Locality;
        public Cardinality Cardinality;
        public Kind Kind;

        // This comes from header substatus2.
        // For one single/multi, number = 0...63 for single, 64...127 for multi.
        // For one drum/effect, number = 0...31 for effect, 32 for drum.
        // For block single/multi, number = 0 for all singles, 0x40 for all multis.
        // For block effect, number = 0x40 for all effects.
        // For all patch data dump, this value is not used, and is set to -1,
        public int Number;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            var locality = this.Locality switch
            {
                Locality.Internal => "INT",
                Locality.External => "EXT",
                _ => "???"
            };
            sb.AppendLine($"Locality: {locality}");

            var cardinality = this.Cardinality switch
            {
                Cardinality.One => "One",
                Cardinality.Block => "Block",
                Cardinality.All => "All",
                _ => "???"
            };
            sb.AppendLine($"Cardinality: {cardinality}");

            var kind = this.Kind switch
            {
                Kind.SinglePatch => "Single",
                Kind.MultiPatch => "Multi",
                Kind.EffectPatch => "Effect",
                Kind.DrumPatch => "Drum",
                Kind.All => "All",
                _ => "unknown"
            };
            sb.AppendLine($"Kind: {kind}");

            if (this.Kind == Kind.SinglePatch)
            {
                if (this.Cardinality == Cardinality.Block)
                {
                    sb.AppendLine("Number: -");
                }
                else
                {
                    sb.AppendLine($"Number: {this.Number + 1}");
                }
            }
            else if (this.Kind == Kind.MultiPatch)
            {
                if (this.Cardinality == Cardinality.Block)
                {
                    sb.AppendLine("Number: -");
                }
                else
                {
                    sb.AppendLine($"Number: {this.Number - 64 + 1}");
                }
            }
            else if (this.Kind == Kind.DrumPatch)
            {
                sb.AppendLine("Number: -");
            }
            else if (this.Kind == Kind.EffectPatch)
            {
                if (this.Cardinality == Cardinality.Block)
                {
                    sb.AppendLine("Number: -");
                }
                else
                {
                    sb.AppendLine($"Number: {this.Number + 1}");
                }
            }
            else if (this.Kind == Kind.All)
            {
                sb.AppendLine("Number: -");
            }
            else
            {
                sb.AppendLine("Number: ???");
            }

            return sb.ToString();
        }
    }

    // Converts SysEx bytes to and from int values.
    public class SystemExclusiveDataConverter {

        // Depth: -50 ... +50
        public static int DepthFromByte(byte b) => b - 50;
        public static byte ByteFromDepth(int d) => (byte)(d + 50);

        public static int ChannelFromByte(byte b) => b + 1;  // 0...15 to 1...16
        public static byte ByteFromChannel(int c) => (byte)(c - 1);  // 1...16 to 0...15

        public static int PanFromByte(byte b) => b - 7;  // to -7~+7
        public static byte ByteFromPan(int p) => (byte)(p + 7);  // to 0~14

        public static int EffectFromByte(byte b) => b + 1; // to 1...32
        public static byte ByteFromEffect(int e) => (byte)(e - 1);  // to 0...31

        public static int PatchNumberFromByte(byte b) => b + 1;  // to 1...64
        public static byte ByteFromPatchNumber(int p) => (byte)(p - 1); // to 0...63

        public static int TransposeFromByte(byte b) => b - 24;  // to -24...+24
        public static byte ByteFromTranspose(int t) => (byte)(t + 24); // to 0...48

        public static string PatchNameFromBytes(List<byte> data)
        {
            var chars = new List<char>();

            for (var i = 0; i < 10; i++)
            {
                var b = data[i];

                // If there is a character not found in the allowed list,
                // replace it with a space and go to the next character
                if (!Array.Exists(AllowedNameCharacterCodes, element => element.Equals(b)))
                {
                    chars.Add(' ');
                    continue;
                }

                if (b == 0x7e)  // right arrow
                {
                    chars.Add('\u2192');
                }
                else if (b == 0x7f) // left arrow
                {
                    chars.Add('\u2190');
                }
                else if (b == 0x5c) // yen sign
                {
                    chars.Add('\u00a5');
                }
                else  // straight ASCII
                {
                    chars.Add((char)b);
                }
            }

            return new string(chars.ToArray());
        }

        public static List<byte> BytesFromPatchName(string s)
        {
            var bytes = new List<byte>();

            var charArray = s.ToCharArray();
            for (var i = 0; i < charArray.Length; i++)
            {
                char ch = charArray[i];
                byte b = (byte)ch;
                if (ch == '\u2192') // right arrow
                {
                    b = 0x7e;
                }
                else if (ch == '\u2190')  // left arrow
                {
                    b = 0x7f;
                }
                else if (ch == '\u00a5')  // yen sign
                {
                    b = 0x5c;
                }
                bytes.Add(b);
            }

            return bytes;

        }

        public static readonly byte[] AllowedNameCharacterCodes = new byte[]
        {
            0x20, // space
            0x21, // exclamation mark
            0x22, // double quote
            0x23, // hash
            0x24, // dollar sign
            0x25, // percent sign
            0x26, // ampersand
            0x27, // single quote
            (byte)'(',
            (byte)')',
            (byte)'*',
            (byte)'+',
            (byte)'-',
            (byte)',',
            (byte)'/',
            (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7',
            (byte)'8', (byte)'9',
            (byte)':',
            (byte)';',
            (byte)'<',
            (byte)'=',
            (byte)'>',
            (byte)'?',
            (byte)'@', // 0x40
            (byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F', (byte)'G', (byte)'H',
            (byte)'I', (byte)'J', (byte)'K', (byte)'L', (byte)'M', (byte)'N', (byte)'O', (byte)'P',
            (byte)'Q', (byte)'R', (byte)'S', (byte)'T', (byte)'U', (byte)'V', (byte)'W', (byte)'X',
            (byte)'Y', (byte)'Z',
            (byte)'[', // 0x5b
            0x5c,  // yen sign (U+00A5)
            (byte)']', // 0x5d
            (byte)'^', // 0x5e
            (byte)'_', // 0x5f
            (byte)'`', // 0x60
            (byte)'a', (byte)'b', (byte)'c', (byte)'d', (byte)'e', (byte)'f', (byte)'g', (byte)'h',
            (byte)'i', (byte)'j', (byte)'k', (byte)'l', (byte)'m', (byte)'n', (byte)'o', (byte)'p',
            (byte)'q', (byte)'r', (byte)'s', (byte)'t', (byte)'u', (byte)'v', (byte)'w', (byte)'x',
            (byte)'y', (byte)'z',
            (byte)'{', // 0x7B
            (byte)'|', // 0x7C
            (byte)'}', // 0x7D
            0x7e, // right arrow (U+2192)
            0x7f, // left arrow (U+2190)
        };

    }
}
