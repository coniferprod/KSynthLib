using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using KSynthLib.Common;

namespace KSynthLib.K4
{
    public class SystemExclusiveHeader
    {
        public const int DataSize = 6;

	    public byte Channel;
	    public byte Function;
	    public byte Group;
	    public byte MachineID;
	    public byte Substatus1;
	    public byte Substatus2;

        public SystemExclusiveHeader(byte channel)
        {
            this.Channel = channel;
        }

        public SystemExclusiveHeader(byte[] data)
        {
            Channel = data[0];
		    Function = data[1];
		    Group = data[2];
		    MachineID = data[3];
		    Substatus1 = data[4];
		    Substatus2 = data[5];
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append(string.Format("Channel = {0}", this.Channel + 1));
            builder.Append(string.Format(", Function = {0,2:X2}H ({1})", this.Function, GetFunctionName((SystemExclusiveFunction)this.Function)));
            builder.Append(string.Format(", Group = {2,2:X2}h, MachineID = {3,2:X2}h, Substatus1 = {4,2:X2}h, Substatus2 = {5,2:X2}h", Channel, Function, Group, MachineID, Substatus1, Substatus2));

            return builder.ToString();
        }

        public static readonly Dictionary<SystemExclusiveFunction, string> FunctionNames = new Dictionary<SystemExclusiveFunction, string>()
        {
            { SystemExclusiveFunction.AllPatchDataDump, "All Patch Data Dump" },
            { SystemExclusiveFunction.AllPatchDumpRequest, "All Patch Data Dump Request" },
            { SystemExclusiveFunction.BlockPatchDataDump, "Block Patch Data Dump" },
            { SystemExclusiveFunction.BlockPatchDumpRequest, "Block Patch Data Dump Request" },
            { SystemExclusiveFunction.EditBufferDump, "Edit Buffer Dump" },
            { SystemExclusiveFunction.OnePatchDataDump, "One Patch Data Dump" },
            { SystemExclusiveFunction.OnePatchDumpRequest, "One Patch Data Dump Request" },
            { SystemExclusiveFunction.ParameterSend, "Parameter Send" },
            { SystemExclusiveFunction.ProgramChange, "Program Change" },
            { SystemExclusiveFunction.WriteComplete, "Write Complete" },
            { SystemExclusiveFunction.WriteError, "Write Error" },
            { SystemExclusiveFunction.WriteErrorProtect, "Write Error (Protect)" },
            { SystemExclusiveFunction.WriteErrorNoCard, "Write Error (No Card)" }
        };

        public static bool IsValidFunction(byte functionByte)
        {
            Dictionary<SystemExclusiveFunction, string>.KeyCollection keys = FunctionNames.Keys;
            var keyBytes = keys.Select(k => (byte)k).ToArray();
            return keyBytes.Contains(functionByte);
        }

        public static string GetFunctionName(SystemExclusiveFunction function)
        {
            var functionName = "";
            if (FunctionNames.TryGetValue(function, out functionName))
            {
                return functionName;
            }
            else
            {
                return "unknown";
            }
        }

        public byte[] ToData()
        {
            var data = new List<byte>();
            data.Add(Channel);
            data.Add(Function);
            data.Add(Group);
            data.Add(MachineID);
            data.Add(Substatus1);
            data.Add(Substatus2);
            return data.ToArray();
        }
    }

    public enum SystemExclusiveFunction
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
}
