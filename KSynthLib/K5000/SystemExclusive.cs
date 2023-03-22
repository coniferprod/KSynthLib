using System;
using System.Collections.Generic;
using System.Text;

using KSynthLib.Common;


namespace KSynthLib.K5000
{
    // System exclusive commands corresponding to Table 5.2 in the K5000 MIDI format spec.
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
        private int _channel;
        public int Channel => this._channel;

        private Cardinality _cardinality;
        public Cardinality Cardinality => this._cardinality;

        private BankIdentifier _bankIdentifier;
        public BankIdentifier Bank => this._bankIdentifier;

        private PatchKind _patchKind;
        public PatchKind Kind => this._patchKind;

        public List<byte> SubBytes;

        // Constructs a dump header from System Exclusive data.
        // Note that the data must be the message payload (starting
        // after the SysEx initiator and the manufacturer ID).
        // The offsets are relative to that (unlike the K5000 MIDI spec).
        public DumpHeader(byte[] data)
        {
            this._channel = data[0] + 1;  // adjust channel to 1~16
            this._cardinality = (Cardinality)data[1];
            this._bankIdentifier = (BankIdentifier)data[5];
            this._patchKind = (PatchKind)data[4];
            this.SubBytes = new List<byte>();

            var bytes = new List<byte>(data);
            var index = 0;
            bool valid = true;
            foreach (var b in bytes)
            {
                if (index == 0)  // channel byte, save and continue
                {
                    this._channel = b + 1;  // adjust channel to 1~16
                    index += 1;
                    continue;
                }

                switch (index)
                {
                    case 0:  // channel byte ("3rd" in spec)
                        this._channel = b + 1;  // adjust channel to 1~16
                        break;

                    case 1:  // cardinality ("4th" in spec)
                        this._cardinality = (Cardinality) b;
                        break;

                    case 2:  // "5th" in spec, always 0x00
                        if (b != 0x00)
                        {
                            valid = false;
                        }
                        break;

                    case 3: // "6th" in spec, always 0x0A
                        if (b != 0x0A)
                        {
                            valid = false;
                        }
                        break;

                    case 4:  // patch kind ("7th" in spec)
                        this._patchKind = (PatchKind) b;
                        break;

                    case 5:  // bank ID ("8th" in spec)
                        this._bankIdentifier = (BankIdentifier) b;
                        break;

                    default:
                        break;
                }

                index += 1;
            }

            if (!valid)
            {
                throw new ArgumentException("Dump header data not recognized");
            }

            if (this.Cardinality == Cardinality.One)
            {
                if (this.Kind == PatchKind.Single)
                {
                    this.SubBytes.Add(data[6]);  // sub1 of single for all banks
                }
                else if (this.Kind == PatchKind.Combi)
                {
                    this.SubBytes.Add(data[5]);  // sub1 of combi/multi
                }
                // No sub-bytes for drum kit or drum instrument
            }
            else if (this.Cardinality == Cardinality.Block)
            {
                if (this.Kind == PatchKind.Single)
                {
                    if (this.Bank != BankIdentifier.B)  // not for PCM bank
                    {
                        // Get the tone map
                        var tempBytes = new List<byte>(data);
                        var toneMapBytes = tempBytes.GetRange(6, ToneMap.DataSize);
                        this.SubBytes.AddRange(toneMapBytes);
                    }
                }
                // No sub-bytes for block combi/multi or drum instrument
                else
                {
                    this._bankIdentifier = BankIdentifier.None;
                }
            }
        }

        public DumpHeader(
            int channel,
            Cardinality cardinality,
            BankIdentifier bankIdentifier,
            PatchKind patchKind,
            byte[] subBytes)
        {
            this._channel = channel;
            this._cardinality = cardinality;
            this._bankIdentifier = bankIdentifier;
            this._patchKind = patchKind;
            this.SubBytes = new List<byte>(subBytes);
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
            return
                (Channel == p.Channel) &&
                (Cardinality == p.Cardinality) &&
                (Kind == p.Kind) &&
                (Bank == p.Bank) &&
                SubBytes.Equals(p.SubBytes);
        }

        public override int GetHashCode() => (Channel, Cardinality, Kind, Bank).GetHashCode();

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"Channel={Channel}, {Cardinality}, {Kind}, ");
            if (Bank != BankIdentifier.None)
            {
                sb.Append($"Bank={Bank}");
            }
            else
            {
                sb.Append("Bank=N/A");
            }
            sb.Append($", Sub-bytes: {SubBytes.Count}");
            return sb.ToString();
        }

        //
        // ISystemExclusiveData implementation
        //

        public List<byte> Data
        {
            get
            {
                var data = new List<byte>();

                switch (this._cardinality)
                {
                    case Cardinality.Block:
                        switch (this._bankIdentifier)
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
                        switch (this._bankIdentifier)
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

        public int DataLength
        {
            get
            {
                return 4 + this.SubBytes.Count;
            }
        }
    }
}
