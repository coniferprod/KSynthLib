using System;
using System.Collections.Generic;
using System.Text;
using KSynthLib.Common;

namespace KSynthLib.K5000
{
    public enum EffectDestination
    {
        Effect1DryWet,
        Effect1Para,
        Effect2DryWet,
        Effect2Para,
        Effect3DryWet,
        Effect3Para,
        Effect4DryWet,
        Effect4Para,
        ReverbDryWet1,
        ReverbDryWet2
    }

    public enum ControlSource
    {
        Bender,
        ChannelPressure,
        Wheel,
        Expression,
        MIDIVolume,
        PanPot,
        GeneralController1,
        GeneralController2,
        GeneralController3,
        GeneralController4,
        GeneralController5,
        GeneralController6,
        GeneralController7,
        GeneralController8
    }

    public enum ControlDestination
    {
        PitchOffset,
        CutoffOffset,
        Level,
        VibratoDepthOffset,
        GrowlDepthOffset,
        TremoloDepthOffset,
        LFOSpeedOffset,
        AttackTimeOffset,
        Decay1TimeOffset,
        ReleaseTimeOffset,
        VelocityOffset,
        ResonanceOffset,
        PanPotOffset,
        FFBiasOffset,
        FFEnvLFODepthOffset,
        FFEnvLFOSpeedOffset,
        HarmonicLoOffset,
        HarmonicHiOffset,
        HarmonicEvenOffset,
        HarmonicOddOffset
    }

    public class EffectControl
    {
        public ControlSource Source { get; }
        public EffectDestination Destination { get; }

        private EffectControlDepthType _depth; // (-31)33 ~ (+31)95
        public sbyte Depth
        {
            get => _depth.Value;
            set => _depth.Value = value;
        }

        public EffectControl()
        {
            this.Source = ControlSource.Bender;
            this.Destination = EffectDestination.ReverbDryWet1;
            this._depth = new EffectControlDepthType();
        }

        public EffectControl(byte[] data)
        {
            this.Source = (ControlSource)data[0];
            this.Destination = (EffectDestination)data[1];
            this._depth = new EffectControlDepthType((sbyte)(data[2] - 64));
        }

        public override string ToString()
        {
            return $"source = {Source} destination = {Destination} depth = {Depth}";
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();
            data.Add((byte)this.Source);
            data.Add((byte)this.Destination);
            data.Add((byte)(this.Depth + 64));  // bring to range 33 ~ 95 again
            return data.ToArray();
        }
    }

    public enum MacroControllerType
    {
        PitchOffset,
        CutoffOffset,
        Level,
        VibratoDepthOffset,
        GrowlDepthOffset,
        TremoloDepthOffset,
        LFOSpeedOffset,
        AttackTimeOffset,
        Decay1TimeOffset,
        ReleaseTimeOffset,
        VelocityOffset,
        ResonanceOffset,
        PanPotOffset,
        FFBiasOffset,
        FFEnvLFODepthOffset,
        FFEnvLFOSpeedOffset,
        HarmonicLoOffset,
        HarmonicHiOffset,
        HarmonicEvenOffset,
        HarmonicOddOffset
    }

    // Use C# 6.0 read-only auto-properties
    // to make some classes immutable.
    // See https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-6

    public class MacroControllerParameter
    {
        public MacroControllerType Type { get; }

        private MacroDepthType _depth;
        public sbyte Depth
        {
            get => _depth.Value;
            set => _depth.Value = value;
        }

        public MacroControllerParameter(byte type, byte depth)
        {
            this.Type = (MacroControllerType)type;

            int d = depth - 64;  // scale 33...95 to -31...31
            this._depth = new MacroDepthType((sbyte)d);
        }

        public MacroControllerParameter(MacroControllerType type, sbyte depth)
        {
            this.Type = type;
            this._depth = new MacroDepthType(depth);
        }

        public MacroControllerParameter()
        {
            this.Type = MacroControllerType.Level;
            this._depth = new MacroDepthType();  // default 0 is halfway between -31 and 31
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();
            data.Add((byte)this.Type);
            data.Add((byte)(this.Depth + 64));
            return data.ToArray();
        }

        public (byte Type, byte Depth) Bytes
        {
            get
            {
                return ((byte)this.Type, (byte)(this.Depth + 64));
            }
        }
    }

    public class MacroController
    {
        public MacroControllerParameter Param1 { get; }
        public MacroControllerParameter Param2 { get; }

        public MacroController(MacroControllerParameter param1, MacroControllerParameter param2)
        {
            this.Param1 = param1;
            this.Param2 = param2;
        }

        public MacroController()
        {
            this.Param1 = new MacroControllerParameter();
            this.Param2 = new MacroControllerParameter();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"DEST1 = {Param1.Type}, Depth = {Param1.Depth}. DEST2 = {Param2.Type}, Depth = {Param2.Depth}");
            return builder.ToString();
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(this.Param1.ToData());
            data.AddRange(this.Param2.ToData());
            return data.ToArray();
        }
    }

    public enum Switch {
        Off,
        HarmMax,
        HarmBright,
        HarmDark,
        HarmSaw,
        SelectLoud,
        AddLoud,
        Add5th,
        AddOdd,
        AddEven,
        HE1,
        HE2,
        HELoop,
        FFMax,
        FFComb,
        FFHiCut,
        FFComb2
    }

    public class CommonSettings
    {
        public const int DataSize = 48;
        public const int MaxSources = 6;

        public byte EffectAlgorithm;  // 0 ~ 3
        public ReverbSettings Reverb;
        public EffectSettings Effect1;
        public EffectSettings Effect2;
        public EffectSettings Effect3;
        public EffectSettings Effect4;
        public GEQSettings GEQ;
        public bool DrumMark;
        public string Name;

        private PositiveLevelType _volume;
        public byte Volume
        {
            get => _volume.Value;
            set => _volume.Value = value;
        }

        public CommonSettings()
        {
            Reverb = new ReverbSettings();
            Effect1 = new EffectSettings();
            Effect2 = new EffectSettings();
            Effect3 = new EffectSettings();
            Effect4 = new EffectSettings();
            DrumMark = false;
            GEQ = new GEQSettings();
            Name = "Init    ";
            _volume = new PositiveLevelType(80);
        }

        public CommonSettings(byte[] data) : this()
        {
            Reverb = new ReverbSettings();
            Effect1 = new EffectSettings();
            Effect2 = new EffectSettings();
            Effect3 = new EffectSettings();
            Effect4 = new EffectSettings();
            GEQ = new GEQSettings();

            Console.WriteLine(string.Format("Common data:\n{0}", Util.HexDump(data)));

            int offset = 0;
            byte b = 0;  // will be reused when getting the next byte

            (b, offset) = Util.GetNextByte(data, offset);
            EffectAlgorithm = b; // 0 ~ 3
            Console.WriteLine($"Effect Algorithm = {EffectAlgorithm}");

            Reverb = GetReverb(data, offset);
            offset += 6;
            Console.WriteLine($"Reverb = {Reverb}");

            Effect1 = GetEffect(data, offset);
            offset += 6;
            Console.WriteLine($"E1 = {Effect1}");

            Effect2 = GetEffect(data, offset);
            offset += 6;
            Console.WriteLine($"E2 = {Effect2}");

            Effect3 = GetEffect(data, offset);
            offset += 6;
            Console.WriteLine($"E3 = {Effect3}");

            Effect4 = GetEffect(data, offset);
            offset += 6;
            Console.WriteLine($"E4 = {Effect4}");

            GEQ = GetGEQ(data, offset);
            offset += 7;
            Console.WriteLine($"GEQ = {GEQ}");

            (b, offset) = Util.GetNextByte(data, offset);
            DrumMark = (b == 1);

            Name = GetName(data, offset);
            offset += 8;

            (b, offset) = Util.GetNextByte(data, offset);
            _volume = new PositiveLevelType(b);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"Name = '{Name}' Volume = {Volume}\n");

            builder.Append($"REVERB:\n{Reverb}\n");
            builder.Append($"Effect algorithm = {EffectAlgorithm + 1}\n");
            builder.Append($"EFFECT 1:\n{Effect1}");
            builder.Append($"EFFECT 2:\n{Effect2}");
            builder.Append($"EFFECT 3:\n{Effect3}");
            builder.Append($"EFFECT 4:\n{Effect4}");
            return builder.ToString();
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();
            data.Add(EffectAlgorithm);

            data.AddRange(Reverb.ToData());
            data.AddRange(Effect1.ToData());
            data.AddRange(Effect2.ToData());
            data.AddRange(Effect3.ToData());
            data.AddRange(Effect4.ToData());

            data.AddRange(GEQ.ToData());

            data.Add(0); // drum_mark, 0=normal(not drum)

            foreach (char ch in Name)
            {
                data.Add(Convert.ToByte(ch));
            }

            data.Add(Volume);

            return data.ToArray();
        }

        private ReverbSettings GetReverb(byte[] data, int offset)
        {
            return new ReverbSettings()
            {
                ReverbType = data[offset],
                DryWet = data[offset + 1],
                Param1 = data[offset + 2],
                Param2 = data[offset + 3],
                Param3 = data[offset + 4],
                Param4 = data[offset + 5]
            };
        }

        private EffectSettings GetEffect(byte[] data, int offset)
        {
            return new EffectSettings()
            {
                Type = (EffectType)(data[offset] - 11),
                Depth = data[offset + 1],
                Param1 = data[offset + 2],
                Param2 = data[offset + 3],
                Param3 = data[offset + 4],
                Param4 = data[offset + 5]
            };
        }

        private GEQSettings GetGEQ(byte[] data, int offset)
        {
            return new GEQSettings()
            {
                // 58(-6) ~ 70(+6), so 64 is zero
                Freq1 = (sbyte)(data[offset] - 64),
                Freq2 = (sbyte)(data[offset + 1] - 64),
                Freq3 = (sbyte)(data[offset + 2] - 64),
                Freq4 = (sbyte)(data[offset + 3] - 64),
                Freq5 = (sbyte)(data[offset + 4] - 64),
                Freq6 = (sbyte)(data[offset + 5] - 64),
                Freq7 = (sbyte)(data[offset + 6] - 64)
            };
        }

        private string GetName(byte[] data, int offset)
        {
            // Brute-forcing the name in from params 40 ... 47
            byte[] bytes =
            {
                data[offset],
                data[offset + 1],
                data[offset + 2],
                data[offset + 3],
                data[offset + 4],
                data[offset + 5],
                data[offset + 6],
                data[offset + 7]
            };
            string name = Encoding.ASCII.GetString(bytes);
            return name;
        }
    }
}