using System.Collections.Generic;

namespace KSynthLib.K5000
{
    // System exclusive commands corresponding to Table 5.2 in the MIDI format spec.
    public enum SystemExclusiveFunction: byte
    {
        OneBlockDumpRequest = 0x00,
        AllBlockDumpRequest = 0x01,
        ParameterSend = 0x10,
        TrackControl = 0x11,
        OneBlockDump = 0x20,
        AllBlockDump = 0x21,
        ModeChange = 0x31,
        Remote = 0x32,
        WriteComplete = 0x40,
        WriteError = 0x41,
        WriteErrorByProtect = 0x42,
        WriteErrorByMemoryFull = 0x44,
        WriteErrorByNoExpandMemory = 0x45
    }

    public static class SystemExclusiveFunctionExtensions
    {
        public static string Name(this SystemExclusiveFunction function)
        {
            var functionNames = new Dictionary<SystemExclusiveFunction, string>()
            {
                { SystemExclusiveFunction.OneBlockDumpRequest, "One Block Dump Request" },
                { SystemExclusiveFunction.AllBlockDumpRequest, "All Block Dump Request" },
                { SystemExclusiveFunction.ParameterSend, "Parameter Send" },
                { SystemExclusiveFunction.TrackControl, "Track Control" },
                { SystemExclusiveFunction.OneBlockDump, "One Block Dump" },
                { SystemExclusiveFunction.AllBlockDump, "All Block Dump" },
                { SystemExclusiveFunction.ModeChange, "Mode Change" },
                { SystemExclusiveFunction.Remote, "Remote" },
                { SystemExclusiveFunction.WriteComplete, "Write Complete" },
                { SystemExclusiveFunction.WriteError, "Write Error" },
                { SystemExclusiveFunction.WriteErrorByProtect, "Write Error (Protect)" },
                { SystemExclusiveFunction.WriteErrorByMemoryFull, "Write Error (Memory Full)" },
                { SystemExclusiveFunction.WriteErrorByNoExpandMemory, "Write Error (No Expansion Memory)" }
            };

            string name;
            if (functionNames.TryGetValue(function, out name))
            {
                return name;
            }
            else
            {
                return "(unknown)";
            }
        }
    }

    public enum Cardinality
    {
        One,
        Block
    }

    public enum BankIdentifier
    {
        A,
        B,
        D,
        E,
        F
    }

    public enum PatchKind
    {
        Single,
        Multi
    }

    public class DumpHeader
    {
        private Cardinality cardinality;
        private BankIdentifier bankIdentifier;
        private PatchKind patchKind;

        public DumpHeader(Cardinality cardinality, BankIdentifier bankIdentifier, PatchKind patchKind)
        {
            this.cardinality = cardinality;
            this.bankIdentifier = bankIdentifier;
            this.patchKind = patchKind;
        }

        public byte[] ToData()
        {
            var data = new List<byte>();

            switch (cardinality)
            {
                case Cardinality.Block:
                    switch (bankIdentifier)
                    {
                        case BankIdentifier.A:
                            data.Add((byte)SystemExclusiveFunction.AllBlockDump);
                            data.Add(0x00);
                            data.Add(0x0a);
                            data.Add(0x00);
                            data.Add(0x00);
                            break;

                        default:
                            break;
                    }
                    break;

                case Cardinality.One:
                    switch (bankIdentifier)
                    {
                        case BankIdentifier.A:
                            data.Add((byte)SystemExclusiveFunction.OneBlockDump);
                            data.Add(0x00);
                            data.Add(0x0a);
                            data.Add(0x00);
                            data.Add(0x00);
                            break;

                        default:
                            break;
                    }

                    break;
            }

            return data.ToArray();
        }
    }
}
