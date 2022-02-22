using System;
using System.Collections.Generic;

namespace KSynthLib.K5
{
    public enum SystemExclusiveFunction: byte
    {
        OneBlockDataRequest = 0x00,
        AllBlockDataRequest = 0x01,
        ParameterSend = 0x10,
        OneBlockDataDump = 0x20,
        AllBlockDataDump = 0x21,
        ProgramSend = 0x30,
        WriteComplete = 0x40,
        WriteError = 0x41,
        WriteErrorProtect = 0x42,
        WriteErrorNoCard = 0x43,
        MachineIDRequest = 0x60,
        MachineIDAcknowledge = 0x61
    }

    public static class SystemExclusiveFunctionExtensions
    {
        public static string Name(this SystemExclusiveFunction function)
        {
            var functionNames = new Dictionary<SystemExclusiveFunction, string>()
            {
                { SystemExclusiveFunction.OneBlockDataRequest, "One Block Data Request" },
                { SystemExclusiveFunction.AllBlockDataRequest, "All Block Data Request" },
                { SystemExclusiveFunction.ParameterSend, "Parameter Send" },
                { SystemExclusiveFunction.OneBlockDataDump, "One Block Data Dump" },
                { SystemExclusiveFunction.AllBlockDataDump, "All Block Data Dump" },
                { SystemExclusiveFunction.ProgramSend, "Program Send" },
                { SystemExclusiveFunction.WriteComplete, "Write Complete" },
                { SystemExclusiveFunction.WriteError, "Write Error" },
                { SystemExclusiveFunction.WriteErrorProtect, "Write Error (Protect)" },
                { SystemExclusiveFunction.WriteErrorNoCard, "Write Error (No Card)" },
                { SystemExclusiveFunction.MachineIDRequest, "Machine ID Request" },
                { SystemExclusiveFunction.MachineIDAcknowledge, "Machine ID Acknowledge" }
            };

            return functionNames.GetValueOrDefault(function, "(unknown)");
        }
    }
}
