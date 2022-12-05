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

            sb.AppendFormat("Channel: {0}\n", this.Channel);
            sb.AppendFormat("Function: {0}\n", SystemExclusiveFunctionExtensions.Name(this.Function));
            sb.AppendFormat("Group: {0:X2}h\n", this.Group);
            sb.AppendFormat("MachineID: {0:X2}h\n", this.MachineID);
            sb.AppendFormat("Sub status 1: {0:X2}h\n", this.Substatus1);
            sb.AppendFormat("Sub status 2: {0:X2}h\n", this.Substatus2);

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
            sb.AppendFormat("Locality: {0}\n", locality);

            var cardinality = this.Cardinality switch
            {
                Cardinality.One => "One",
                Cardinality.Block => "Block",
                Cardinality.All => "All",
                _ => "???"
            };
            sb.AppendFormat("Cardinality: {0}\n", cardinality);

            var kind = this.Kind switch
            {
                Kind.SinglePatch => "Single",
                Kind.MultiPatch => "Multi",
                Kind.EffectPatch => "Effect",
                Kind.DrumPatch => "Drum",
                Kind.All => "All",
                _ => "unknown"
            };
            sb.AppendFormat("Kind: {0}\n", kind);

            if (this.Kind == Kind.SinglePatch)
            {
                if (this.Cardinality == Cardinality.Block)
                {
                    sb.Append("Number: -\n");
                }
                else
                {
                    sb.AppendFormat("Number: {0}\n", this.Number + 1);
                }
            }
            else if (this.Kind == Kind.MultiPatch)
            {
                if (this.Cardinality == Cardinality.Block)
                {
                    sb.Append("Number: -\n");
                }
                else
                {
                    sb.AppendFormat("Number: {0}\n", this.Number - 64 + 1);
                }
            }
            else if (this.Kind == Kind.DrumPatch)
            {
                sb.AppendFormat("Number: -\n");
            }
            else if (this.Kind == Kind.EffectPatch)
            {
                if (this.Cardinality == Cardinality.Block)
                {
                    sb.Append("Number: -\n");
                }
                else
                {
                    sb.AppendFormat("Number: {0}\n", this.Number + 1);
                }
            }
            else if (this.Kind == Kind.All)
            {
                sb.Append("Number: -\n");
            }
            else
            {
                sb.Append("Number: ???\n");
            }

            return sb.ToString();
        }
    }
}
