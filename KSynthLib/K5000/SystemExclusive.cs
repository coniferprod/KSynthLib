using System;
using System.Collections.Generic;

using KSynthLib.Common;


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

    public enum Cardinality: byte
    {
        One = 0x20,
        Block = 0x21
    }

    public enum BankIdentifier: byte
    {
        A = 0x00,
        B = 0x01,
        // there is no Bank C
        D = 0x02,  // only on K5000S/R
        E = 0x03,
        F = 0x04,
        None = 0xff  // for drum kit/drum inst/combi
    }

    public enum PatchKind: byte
    {
        Single = 0x00,
        Combi = 0x20, // multi on K5000S/R
        DrumKit = 0x10,
        DrumInstrument = 0x11
    }

    public class DumpHeader: IEquatable<DumpHeader>, ISystemExclusiveData
    {
        private Cardinality cardinality;
        public Cardinality Card => this.cardinality;

        private BankIdentifier bankIdentifier;
        public BankIdentifier Bank => this.bankIdentifier;

        private PatchKind patchKind;
        public PatchKind Kind => this.patchKind;

        public DumpHeader(byte[] data)
        {
            this.cardinality = (Cardinality)data[3];
            this.bankIdentifier = (BankIdentifier)data[7];
            this.patchKind = (PatchKind)data[6];
        }

        public DumpHeader(Cardinality cardinality, BankIdentifier bankIdentifier, PatchKind patchKind)
        {
            this.cardinality = cardinality;
            this.bankIdentifier = bankIdentifier;
            this.patchKind = patchKind;
        }

        public override bool Equals(object obj) => this.Equals(obj as DumpHeader);

        public bool Equals(DumpHeader p)
        {
            if (p is null)
            {
                return false;
            }

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, p))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != p.GetType())
            {
                return false;
            }

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            return (Card == p.Card) && (Kind == p.Kind) && (Bank == p.Bank);
        }

        public override int GetHashCode() => (Card, Kind, Bank).GetHashCode();

        public List<byte> GetSystemExclusiveData()
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

            return data;
        }
    }
}
