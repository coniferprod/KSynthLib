using System;
using System.Collections.Generic;
using System.Text;

using KSynthLib.Common;

namespace KSynthLib.K5000
{
    public class PatchName: ISystemExclusiveData
    {
        public static readonly int Length = 8;

        private string _name;

        public string Value
        {
            get => _name.PadRight(Length, ' ');
            set => _name = value.PadRight(Length, ' ');
        }

        public PatchName(string s)
        {
            this.Value = s;
        }

        public PatchName(byte[] data, int offset = 0)
        {
            byte[] bytes =
            {
                data[offset],
                data[offset + 1],
                data[offset + 2],
                data[offset + 3],
                data[offset + 4],
                data[offset + 5],
                data[offset + 6],
                data[offset + 7],
            };

            this.Value = Encoding.ASCII.GetString(bytes);
        }

        public List<byte> GetSystemExclusiveData()
        {
            var bytes = new List<byte>();

            var charArray = this.Value.ToCharArray();
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
    }

    public enum VelocityCurve
    {
        Curve1,
        Curve2,
        Curve3,
        Curve4,
        Curve5,
        Curve6,
        Curve7,
        Curve8,
        Curve9,
        Curve10,
        Curve11,
        Curve12
    }

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

    public class EffectControl: ISystemExclusiveData
    {
        public ControlSource Source { get; }
        public EffectDestination Destination { get; }

        public ControlDepth Depth; // (-31)33 ~ (+31)95

        public EffectControl()
        {
            this.Source = ControlSource.Bender;
            this.Destination = EffectDestination.ReverbDryWet1;
            this.Depth = new ControlDepth();
        }

        public EffectControl(byte[] data)
        {
            this.Source = (ControlSource)data[0];
            this.Destination = (EffectDestination)data[1];
            this.Depth = new ControlDepth(data[2]);
        }

        public override string ToString()
        {
            return $"source = {Source} destination = {Destination} depth = {Depth.Value}";
        }

        public List<byte> GetSystemExclusiveData()
        {
            var data = new List<byte>();

            data.Add((byte)Source);
            data.Add((byte)Destination);
            data.Add(Depth.ToByte());

            return data;
        }
    }

    public enum MacroControllerKind
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
        public MacroControllerKind Kind { get; }
        public ControlDepth Depth;

        public MacroControllerParameter(byte kind, byte depth)
        {
            this.Kind = (MacroControllerKind)kind;
            this.Depth = new ControlDepth(depth);
        }

        public MacroControllerParameter(MacroControllerKind kind, byte depth)
        {
            this.Kind = kind;
            this.Depth = new ControlDepth(depth);
        }

        public MacroControllerParameter()
        {
            this.Kind = MacroControllerKind.Level;
            this.Depth = new ControlDepth();
        }

        public byte[] ToData() => new List<byte>() { (byte)Kind, Depth.ToByte() }.ToArray();

        public (byte Kind, byte Depth) Bytes => ((byte)Kind, Depth.ToByte());
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
            var builder = new StringBuilder();
            builder.Append($"Dest1 = {Param1.Kind}, Depth = {Param1.Depth.Value}. Dest2 = {Param2.Kind}, Depth = {Param2.Depth.Value}");
            return builder.ToString();
        }

        public byte[] ToData()
        {
            var data = new List<byte>();
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

    public class CommonSettings: ISystemExclusiveData
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

        public PositiveLevel Volume;

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
            Volume = new PositiveLevel(80);
        }

        public CommonSettings(byte[] data) : this()
        {
            Reverb = new ReverbSettings();
            Effect1 = new EffectSettings();
            Effect2 = new EffectSettings();
            Effect3 = new EffectSettings();
            Effect4 = new EffectSettings();
            GEQ = new GEQSettings();

            Console.Error.WriteLine(string.Format("Common data:\n{0}", Util.HexDump(data)));

            int offset = 0;
            byte b = 0;  // will be reused when getting the next byte

            (b, offset) = Util.GetNextByte(data, offset);
            EffectAlgorithm = b; // 0 ~ 3
            Console.Error.WriteLine($"Effect Algorithm = {EffectAlgorithm}");

            Reverb = GetReverb(data, offset);
            offset += 6;
            Console.Error.WriteLine($"Reverb = {Reverb}");

            Effect1 = GetEffect(data, offset);
            offset += 6;
            Console.Error.WriteLine($"E1 = {Effect1}");

            Effect2 = GetEffect(data, offset);
            offset += 6;
            Console.Error.WriteLine($"E2 = {Effect2}");

            Effect3 = GetEffect(data, offset);
            offset += 6;
            Console.Error.WriteLine($"E3 = {Effect3}");

            Effect4 = GetEffect(data, offset);
            offset += 6;
            Console.Error.WriteLine($"E4 = {Effect4}");

            GEQ = GetGEQ(data, offset);
            offset += 7;
            Console.Error.WriteLine($"GEQ = {GEQ}");

            (b, offset) = Util.GetNextByte(data, offset);
            DrumMark = (b == 1);

            Name = GetName(data, offset);
            offset += 8;

            (b, offset) = Util.GetNextByte(data, offset);
            Volume = new PositiveLevel(b);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append($"Name = '{Name}' Volume = {Volume.Value}\n");

            builder.Append($"REVERB:\n{Reverb}\n");
            builder.Append($"Effect algorithm = {EffectAlgorithm + 1}\n");
            builder.Append($"EFFECT 1:\n{Effect1}");
            builder.Append($"EFFECT 2:\n{Effect2}");
            builder.Append($"EFFECT 3:\n{Effect3}");
            builder.Append($"EFFECT 4:\n{Effect4}");
            return builder.ToString();
        }

        public List<byte> GetSystemExclusiveData()
        {
            var data = new List<byte>();

            data.Add(EffectAlgorithm);

            data.AddRange(Reverb.GetSystemExclusiveData());
            data.AddRange(Effect1.GetSystemExclusiveData());
            data.AddRange(Effect2.GetSystemExclusiveData());
            data.AddRange(Effect3.GetSystemExclusiveData());
            data.AddRange(Effect4.GetSystemExclusiveData());

            data.AddRange(GEQ.GetSystemExclusiveData());

            data.Add(0); // drum_mark, 0=normal(not drum)

            foreach (var ch in Name)
            {
                data.Add(Convert.ToByte(ch));
            }

            data.Add(Volume.ToByte());

            return data;
        }

        private ReverbSettings GetReverb(byte[] data, int offset)
        {
            return new ReverbSettings()
            {
                ReverbType = data[offset],
                DryWet1 = new EffectDepth(data[offset + 1]),
                DryWet2 = new EffectDepth(data[offset + 2]),
                Param2 = new PositiveLevel(data[offset + 3]),
                Param3 = new PositiveLevel(data[offset + 4]),
                Param4 = new PositiveLevel(data[offset + 5])
            };
        }

        private EffectSettings GetEffect(byte[] data, int offset)
        {
            return new EffectSettings()
            {
                Kind = (EffectKind)(data[offset] - 11),
                Depth = new EffectDepth(data[offset + 1]),
                Param1 = new PositiveLevel(data[offset + 2]),
                Param2 = new PositiveLevel(data[offset + 3]),
                Param3 = new PositiveLevel(data[offset + 4]),
                Param4 = new PositiveLevel(data[offset + 5])
            };
        }

        private GEQSettings GetGEQ(byte[] data, int offset)
        {
            return new GEQSettings(data, offset);
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

    public class Zone: ISystemExclusiveData
    {
        public Key Low;
        public Key High;

        public Zone(byte low, byte high)
        {
            Low = new Key(low);
            High = new Key(high);
        }

        public List<byte> GetSystemExclusiveData()
        {
            return new List<byte> { Low.ToByte(), High.ToByte() };
        }
    }

    public class FixedKey
    {
        public bool IsOn;
        public Key Key;

        public FixedKey()
        {
            Key = new Key();
            IsOn = false;
        }
        public FixedKey(byte key)
        {
            Key = new Key(key);
            IsOn = key != 0x00;
        }
    }
}
