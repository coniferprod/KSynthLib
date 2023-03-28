using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using SyxPack;

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

        public PatchName(byte[] data)
        {
            byte[] bytes =
            {
                data[0],
                data[1],
                data[2],
                data[3],
                data[4],
                data[5],
                data[6],
                data[7],
            };

            this.Value = Encoding.ASCII.GetString(bytes);
        }

        public override string ToString()
        {
            return this.Value;
        }

        //
        // ISystemExclusiveData implementation
        //

        public List<byte> Data
        {
            get
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

        public int DataLength => Length;
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
        public EffectDestination Destination { get; }

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

        //
        // ISystemExclusiveData implementation
        //

        public List<byte> Data
        {
            get
            {
                var data = new List<byte>();

                data.Add((byte)Source);
                data.Add((byte)Destination);
                data.Add(Depth.ToByte());

                return data;
            }
        }

        public int DataLength => 3;
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

    public class MacroControllerParameter: ISystemExclusiveData
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

        //
        // ISystemExclusiveData implementation
        //

        public List<byte> Data
        {
            get
            {
                return new List<byte>() { (byte)Kind, Depth.ToByte() };
            }
        }

        public int DataLength => 2;

        public (byte Kind, byte Depth) Bytes => ((byte)Kind, Depth.ToByte());
    }

    public class MacroController: ISystemExclusiveData
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

        //
        // ISystemExclusiveData implementation
        //

        public List<byte> Data
        {
            get
            {
                var data = new List<byte>();
                data.AddRange(this.Param1.Data);
                data.AddRange(this.Param2.Data);
                return data;
            }
        }

        public int DataLength => Param1.DataLength * 2;
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

    public class Zone: ISystemExclusiveData
    {
        public Key Low;
        public Key High;

        public Zone(byte low, byte high)
        {
            Low = new Key(low);
            High = new Key(high);
        }

        //
        // ISystemExclusiveData implementation
        //

        public List<byte> Data
        {
            get
            {
                return new List<byte> { Low.ToByte(), High.ToByte() };
            }
        }

        public int DataLength => 2;
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
