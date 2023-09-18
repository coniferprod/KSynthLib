using System;
using System.Collections.Generic;
using System.Text;

using SyxPack;
using KSynthLib.Common;
using System.Diagnostics.Metrics;

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
        private MIDIChannel _channel;
        public MIDIChannel Channel => this._channel;

        private Cardinality _cardinality;
        public Cardinality Cardinality => this._cardinality;

        private BankIdentifier _bankIdentifier;
        public BankIdentifier Bank => this._bankIdentifier;

        private PatchKind _patchKind;
        public PatchKind Kind => this._patchKind;

        // NOTE: Not all dump headers have a valid tone number.
        private PatchNumber _tone;
        public PatchNumber Tone => this._tone;

        // NOTE: Not all dump headers have a valid tone map.
        private ToneMap _toneMap;
        public ToneMap ToneMap => this._toneMap;

        // NOTE: Not all dump headers have a valid instrument number.
        private InstrumentNumber _instrument;
        public InstrumentNumber Instrument => this._instrument;

        // Constructs a dump header from System Exclusive data.
        // Note that the data must be the message payload (starting
        // after the SysEx initiator and the manufacturer ID).
        // The offsets are relative to that (unlike the K5000 MIDI spec).
        public DumpHeader(byte[] data)
        {
            this._tone = new PatchNumber();
            this._toneMap = new ToneMap();
            this._instrument = new InstrumentNumber();

            // channel byte ("3rd" in spec)
            // gets adjusted to 1~16
            this._channel = new MIDIChannel(data[0]);

            // cardinality ("4th" in spec)
            this._cardinality = (Cardinality)data[1];

            bool valid = true;
            valid = (data[2] == 0x00) && (data[3] == 0x0A);

            this._patchKind = (PatchKind)data[4];
            this._bankIdentifier = BankIdentifier.None;

            // For single drum instrument or combi, save the instrument number.
            if (this.Cardinality == Cardinality.One)
            {
                switch (this._patchKind) {
                case PatchKind.DrumInstrument:
                case PatchKind.Combi:
                    this._instrument = new InstrumentNumber(data[5]);
                    this._bankIdentifier = BankIdentifier.None;
                    break;
                case PatchKind.DrumKit:
                    this._bankIdentifier = BankIdentifier.None;
                    break;
                default:
                    this._bankIdentifier = (BankIdentifier) data[5];
                    break;
                }
            }
            else  // must be a block
            {
                // For all others except block drum instrument or combi, save the bank identifier
                switch (this._patchKind)
                {
                case PatchKind.DrumInstrument:
                case PatchKind.Combi:
                    this._bankIdentifier = BankIdentifier.None;
                    break;

                default:
                    this._bankIdentifier = (BankIdentifier) data[5];
                    break;
                }
                // No need to save anything for block drum instrument or block combi, they have only data left
            }

            if (!valid)
            {
                throw new ArgumentException("Dump header data not recognized");
            }

            // Now we should have filled the dump header fields common to all dump types.
            // Collect the rest of the dump header data as necessary.

            if (this.Cardinality == Cardinality.One)
            {
                // All dumps of one single have a tone number...
                if (this.Kind == PatchKind.Single)
                {
                    this._tone = new PatchNumber(data[6]);  // note the index
                }
                // ...while the dumps of one drum instrument or combi have an instrument number...
                else if (this.Kind == PatchKind.Combi || this.Kind == PatchKind.DrumInstrument)
                {
                    this._instrument = new InstrumentNumber(data[5]);  // also note the index
                }
                // ...but one drum kit doesn't have either one of those.
            }
            else if (this.Cardinality == Cardinality.Block)
            {
                if (this.Kind == PatchKind.Single)
                {
                    if (this.Bank != BankIdentifier.B)  // PCM bank has no tone map
                    {
                        // Get the tone map
                        var tempBytes = new List<byte>(data);
                        var toneMapBytes = tempBytes.GetRange(6, ToneMap.DataSize);
                        this._toneMap = new ToneMap(toneMapBytes.ToArray());
                    }
                }
                // No other bytes for block combi/multi or drum instrument
                else
                {
                    this._bankIdentifier = BankIdentifier.None;
                }
            }

            bool hasToneMap = this.Cardinality == Cardinality.Block && this.Kind == PatchKind.Single && this.Bank != BankIdentifier.B;
        }

        public DumpHeader(
            MIDIChannel channel,
            Cardinality cardinality,
            BankIdentifier bankIdentifier,
            PatchKind patchKind,
            PatchNumber tone,
            ToneMap toneMap,
            InstrumentNumber instrument
        )
        {
            this._channel = channel;
            this._cardinality = cardinality;
            this._bankIdentifier = bankIdentifier;
            this._patchKind = patchKind;
            this._tone = tone;
            this._toneMap = toneMap;
            this._instrument = instrument;
        }

        public override bool Equals(object obj) => this.Equals(obj as DumpHeader);

        public bool Equals(DumpHeader p)
        {
            System.Console.WriteLine("Hello from DumpHeader.Equals");

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
                Channel.Equals(p.Channel) &&
                (Cardinality == p.Cardinality) &&
                (Kind == p.Kind) &&
                (Bank == p.Bank) &&
                ToneMap.Equals(p.ToneMap) &&
                Tone.Equals(p.Tone) &&
                Instrument.Equals(p.Instrument);
        }

        public override int GetHashCode() => (Channel, Cardinality, Kind, Bank).GetHashCode();

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"Channel={Channel} Cardinality={Cardinality} ");

            if (Bank != BankIdentifier.None)
            {
                sb.Append($"Bank={Bank}");
            }
            else
            {
                sb.Append("Bank=N/A");
            }

            sb.Append($" Kind={Kind} Tone={Tone} Tone map={ToneMap} Instrument={Instrument}");

            return sb.ToString();
        }

        private List<byte> CollectData()
        {
            var data = new List<byte>();

            data.Add(this.Channel.ToByte()); // adjusts to 0~15 for SysEx
            data.Add((byte)this.Cardinality);
            data.Add(0x00);
            data.Add(0x0a);
            data.Add((byte)this.Kind);

            switch (this.Kind)
            {
            case PatchKind.Single:  // either one or block, singles have a bank ID
                data.Add((byte)this.Bank);
                break;
            case PatchKind.DrumInstrument:
            case PatchKind.Combi:
                // Only single drum instrument and combi have an instrument number
                if (this.Cardinality == Cardinality.One)
                {
                    data.Add(this.Instrument.ToByte());
                }
                break;
            default:  // nothing for drum kit
                break;
            }

            if (this.Cardinality == Cardinality.One)
            {
                // Emit tone number for one single, nothing for others
                if (this.Kind == PatchKind.Single)
                {
                    data.Add(this.Tone.ToByte());
                }
            }

            // All block single banks except PCM Bank B have a tone map
            if (this.Cardinality == Cardinality.Block)
            {
                if (this.Kind == PatchKind.Single)
                {
                    if (this.Bank != BankIdentifier.B)
                    {
                        data.AddRange(this.ToneMap.Data);
                    }
                }
            }

            // Nothing emitted for block drum instrument or combi

            System.Console.Write("Dump header bytes: ");
            foreach (var b in data)
            {
                System.Console.Write($"{b:X2} ");
            }
            System.Console.WriteLine($" ({data.Count} bytes)");

            return data;
        }

        //
        // ISystemExclusiveData implementation
        //

        public List<byte> Data
        {
            get
            {
                return this.CollectData();
            }
        }

        public int DataLength
        {
            get
            {
                return this.CollectData().Count;
            }
        }
    }
}
