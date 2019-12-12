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
        public ControlSource Source;
        public EffectDestination Destination;
        public sbyte Depth;  // (~31)33 ~ (+31)95

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(String.Format("source = {0} destination = {1} depth = {2}", Source, Destination, Depth));
            return builder.ToString();
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();
            data.Add((byte)Source);
            data.Add((byte)Destination);
            data.Add((byte)(Depth + 64));  // bring to range 33 ~ 95 again
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

    public class MacroControllerParameter
    {
        public MacroControllerType Type;
        public sbyte Depth;
    }

    public class MacroController
    {
        public MacroControllerParameter Param1;
        public MacroControllerParameter Param2;

        public MacroController()
        {
            Param1 = new MacroControllerParameter();
            Param2 = new MacroControllerParameter();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(String.Format("DEST1 = {0}, Depth = {1}. DEST2 = {2}, Depth = {3}", Param1.Type, Param1.Depth, Param2.Type, Param2.Depth));
            return builder.ToString();
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
        public static int DataSize = 48;
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
        public byte Volume;


        public CommonSettings()
        {
            Name = "Init    ";
            Reverb = new ReverbSettings();
            Effect1 = new EffectSettings();
            Effect2 = new EffectSettings();
            Effect3 = new EffectSettings();
            Effect4 = new EffectSettings();
            GEQ = new GEQSettings();
        }

        public CommonSettings(byte[] data)
        {
            Reverb = new ReverbSettings();
            Effect1 = new EffectSettings();
            Effect2 = new EffectSettings();
            Effect3 = new EffectSettings();
            Effect4 = new EffectSettings();
            GEQ = new GEQSettings();

            Console.WriteLine(String.Format("Common data:\n{0}", Util.HexDump(data)));

            int offset = 0;
            byte b = 0;  // will be reused when getting the next byte

            (b, offset) = Util.GetNextByte(data, offset);  
            EffectAlgorithm = b; // 0 ~ 3
            Console.WriteLine(String.Format("Effect Algorithm = {0}", EffectAlgorithm));

            Reverb = GetReverb(data, offset);
            offset += 6;
            Console.WriteLine(String.Format("Reverb = {0}", Reverb.ToString()));

            Effect1 = GetEffect(data, offset);
            offset += 6;
            Console.WriteLine(String.Format("E1 = {0}", Effect1.ToString()));

            Effect2 = GetEffect(data, offset);
            offset += 6;
            Console.WriteLine(String.Format("E2 = {0}", Effect2.ToString()));

            Effect3 = GetEffect(data, offset);
            offset += 6;
            Console.WriteLine(String.Format("E3 = {0}", Effect3.ToString()));

            Effect4 = GetEffect(data, offset);
            offset += 6;
            Console.WriteLine(String.Format("E4 = {0}", Effect4.ToString()));

            GEQ = GetGEQ(data, offset);
            offset += 7;
            Console.WriteLine(String.Format("GEQ = {0}", GEQ.ToString()));

            (b, offset) = Util.GetNextByte(data, offset);
            DrumMark = (b == 1);

            Name = GetName(data, offset);
            offset += 8;
            
            (b, offset) = Util.GetNextByte(data, offset);
            Volume = b;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(String.Format("Name = '{0}' Volume = {1}\n", Name, Volume));

            builder.Append(String.Format("REVERB:\n{0}\n", Reverb.ToString()));
            builder.Append(String.Format("Effect algorithm = {0}\n", EffectAlgorithm + 1));
            builder.Append(String.Format("EFFECT 1:\n{0}", Effect1.ToString()));
            builder.Append(String.Format("EFFECT 2:\n{0}", Effect2.ToString()));
            builder.Append(String.Format("EFFECT 3:\n{0}", Effect3.ToString()));
            builder.Append(String.Format("EFFECT 4:\n{0}", Effect4.ToString()));
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
                Type = data[offset],
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
                Type = data[offset],
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