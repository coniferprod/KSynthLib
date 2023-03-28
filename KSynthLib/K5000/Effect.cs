using System.Collections.Generic;
using System.Text;

using KSynthLib.Common;

namespace KSynthLib.K5000
{
    public enum EffectAlgorithm
    {
        Algorithm1,
        Algorithm2,
        Algorithm3,
        Algorithm4
    }

    public enum EffectPath
    {
        Path1,
        Path2,
        Path3,
        Path4
    }

    public enum EffectKind
    {
        EarlyReflection1,
        EarlyReflection2,
        TapDelay1,
        TapDelay2,
        SingleDelay,
        DualDelay,
        StereoDelay,
        CrossDelay,
        AutoPan,
        AutoPanAndDelay,
        Chorus1,
        Chorus2,
        Chorus1AndDelay,
        Chorus2AndDelay,
        Flanger1,
        Flanger2,
        Flanger1AndDelay,
        Flanger2AndDelay,
        Ensemble,
        EnsembleAndDelay,
        Celeste,
        CelesteAndDelay,
        Tremolo,
        TremoloAndDelay,
        Phaser1,
        Phaser2,
        Phaser1AndDelay,
        Phaser2AndDelay,
        Rotary,
        AutoWah,
        Bandpass,
        Exciter,
        Enhancer,
        Overdrive,
        Distortion,
        OverdriveAndDelay,
        DistortionAndDelay
    }

    public class EffectName
    {
        public string Name;
        public string[] ParameterNames;
    };

    public class EffectSettings: ISystemExclusiveData
    {
        public const int DataSize = 6;

        public EffectKind Kind;
        public EffectDepth Depth;
        public PositiveLevel Param1;
        public PositiveLevel Param2;
        public PositiveLevel Param3;
        public PositiveLevel Param4;

        public static EffectName[] EffectNames =
        {
            /*  0 */ new EffectName { Name = "Hall 1", ParameterNames = new string[] { "Dry/Wet 2", "Reverb Time", "Predelay Time", "High Frequency Damping" } },
            /*  1 */ new EffectName { Name = "Hall 2", ParameterNames = new string[] { "Dry/Wet 2", "Reverb Time", "Predelay Time", "High Frequency Damping" } },
            /*  2 */ new EffectName { Name = "Hall 3", ParameterNames = new string[] { "Dry/Wet 2", "Reverb Time", "Predelay Time", "High Frequency Damping" } },
            /*  3 */ new EffectName { Name = "Room 1", ParameterNames = new string[] { "Dry/Wet 2", "Reverb Time", "Predelay Time", "High Frequency Damping" } },
            /*  4 */ new EffectName { Name = "Room 2", ParameterNames = new string[] { "Dry/Wet 2", "Reverb Time", "Predelay Time", "High Frequency Damping" } },
            /*  5 */ new EffectName { Name = "Room 3", ParameterNames = new string[] { "Dry/Wet 2", "Reverb Time", "Predelay Time", "High Frequency Damping" } },
            /*  6 */ new EffectName { Name = "Plate 1", ParameterNames = new string[] { "Dry/Wet 2", "Reverb Time", "Predelay Time", "High Frequency Damping" } },
            /*  7 */ new EffectName { Name = "Plate 2", ParameterNames = new string[] { "Dry/Wet 2", "Reverb Time", "Predelay Time", "High Frequency Damping" } },
            /*  8 */ new EffectName { Name = "Plate 3", ParameterNames = new string[] { "Dry/Wet 2", "Reverb Time", "Predelay Time", "High Frequency Damping" } },
            /*  9 */ new EffectName { Name = "Reverse", ParameterNames = new string[] { "Dry/Wet 2", "Feedback", "Predelay Time", "High Frequency Damping" } },
            /* 10 */ new EffectName { Name = "Long Delay", ParameterNames = new string[]{ "Dry/Wet 2", "Feedback", "Delay Time", "High Frequency Damping" } },
            /* 11 */ new EffectName { Name = "Early Reflection 1", ParameterNames = new string[] { "Slope", "Predelay Time", "Feedback", "?" } },
            /* 12 */ new EffectName { Name = "Early Reflection 2", ParameterNames = new string[] { "Slope", "Predelay Time", "Feedback", "?" } },
            /* 13 */ new EffectName { Name = "Tap Delay 1", ParameterNames = new string[] { "Delay Time 1", "Tap Level", "Delay Time 2", "?" } },
            /* 14 */ new EffectName { Name = "Tap Delay 2", ParameterNames = new string[] { "Delay Time 1", "Tap Level", "Delay Time 2", "?" } },
            /* 15 */ new EffectName { Name = "Single Delay", ParameterNames = new string[] { "Delay Time Fine", "Delay Time Coarse", "Feedback", "?" } },
            /* 16 */ new EffectName { Name = "Dual Delay", ParameterNames = new string[] { "Delay Time Left", "Feedback Left", "Delay Time Right", "Feedback Right" } },
            /* 17 */ new EffectName { Name = "Stereo Delay", ParameterNames = new string[]{ "Delay Time", "Feedback", "?", "?" } },
            /* 18 */ new EffectName { Name = "Cross Delay", ParameterNames = new string[]{ "Delay Time", "Feedback", "?", "?" } },
            /* 19 */ new EffectName { Name = "Auto Pan", ParameterNames = new string[] { "Speed", "Depth", "Predelay Time", "Wave" } },
            /* 20 */ new EffectName { Name = "Auto Pan & Delay", ParameterNames = new string[] { "Speed", "Depth", "Delay Time", "Wave" } },
            /* 21 */ new EffectName { Name = "Chorus 1", ParameterNames = new string[] { "Speed", "Depth", "Predelay Time", "Wave" } },
            /* 22 */ new EffectName { Name = "Chorus 2", ParameterNames = new string[] { "Speed", "Depth", "Predelay Time", "Wave" } },
            /* 23 */ new EffectName { Name = "Chorus 1 & Delay", ParameterNames = new string[] { "Speed", "Depth", "Delay Time", "Wave" } },
            /* 24 */ new EffectName { Name = "Chorus 2 & Delay", ParameterNames = new string[] { "Speed", "Depth", "Delay Time", "Wave" } },
            /* 25 */ new EffectName { Name = "Flanger 1", ParameterNames = new string[] { "Speed", "Depth", "Predelay Time", "Feedback" } },
            /* 26 */ new EffectName { Name = "Flanger 2", ParameterNames = new string[] { "Speed", "Depth", "Predelay Time", "Feedback" } },
            /* 27 */ new EffectName { Name = "Flanger 1 & Delay", ParameterNames = new string[] { "Speed", "Depth", "Delay Time", "Feedback" } },
            /* 28 */ new EffectName { Name = "Flanger 2 & Delay", ParameterNames = new string[] { "Speed", "Depth", "Delay Time", "Feedback" } },
            /* 29 */ new EffectName { Name = "Ensemble", ParameterNames = new string[] { "Depth", "Predelay Time", "?", "?" } },
            /* 30 */ new EffectName { Name = "Ensemble & Delay", ParameterNames = new string[] { "Depth", "Delay Time", "?", "?" } },
            /* 31 */ new EffectName { Name = "Celeste", ParameterNames = new string[] { "Speed", "Depth", "Predelay Time", "?" } },
            /* 32 */ new EffectName { Name = "Celeste & Delay", ParameterNames = new string[] { "Speed", "Depth", "Delay Time", "?" } },
            /* 33 */ new EffectName { Name = "Tremolo", ParameterNames = new string[] { "Speed", "Depth", "Predelay Time", "Wave" } },
            /* 34 */ new EffectName { Name = "Tremolo & Delay", ParameterNames = new string[] { "Speed", "Depth", "Delay Time", "Wave" } },
            /* 35 */ new EffectName { Name = "Phaser 1", ParameterNames = new string[] { "Speed", "Depth", "Predelay Time", "Feedback" } },
            /* 36 */ new EffectName { Name = "Phaser 2", ParameterNames = new string[] { "Speed", "Depth", "Predelay Time", "Feedback" } },
            /* 37 */ new EffectName { Name = "Phaser 1 & Delay", ParameterNames = new string[] { "Speed", "Depth", "Delay Time", "Feedback" } },
            /* 38 */ new EffectName { Name = "Phaser 2 & Delay", ParameterNames = new string[] { "Speed", "Depth", "Delay Time", "Feedback" } },
            /* 39 */ new EffectName { Name = "Rotary", ParameterNames = new string[] { "Slow Speed", "Fast Speed", "Acceleration", "Slow/Fast Switch" } },
            /* 40 */ new EffectName { Name = "Auto Wah", ParameterNames = new string[] { "Sense", "Frequency Bottom", "Frequency Top", "Resonance" } },
            /* 41 */ new EffectName { Name = "Bandpass", ParameterNames = new string[] { "Center Frequency", "Bandwidth", "?", "?" } },
            /* 42 */ new EffectName { Name = "Exciter", ParameterNames = new string[] { "EQ Low", "EQ High", "Intensity", "?" } },
            /* 43 */ new EffectName { Name = "Enhancer", ParameterNames = new string[] { "EQ Low", "EQ High", "Intensity", "?" } },
            /* 44 */ new EffectName { Name = "Overdrive", ParameterNames = new string[] { "EQ Low", "EQ High", "Output Level", "Drive" } },
            /* 45 */ new EffectName { Name = "Distortion", ParameterNames = new string[] { "EQ Low", "EQ High", "Output Level", "Drive" } },
            /* 46 */ new EffectName { Name = "Overdrive & Delay", ParameterNames = new string[] { "EQ Low", "EQ High", "Delay Time", "Drive" } },
            /* 47 */ new EffectName { Name = "Distortion & Delay", ParameterNames = new string[] { "EQ Low", "EQ High", "Delay Time", "Drive" } }
        };

        // Create an effect with sensible default settings.
        public EffectSettings()
        {
            Kind = EffectKind.SingleDelay;
            Depth = new EffectDepth(50);
            Param1 = new PositiveLevel(50);
            Param2 = new PositiveLevel(50);
            Param3 = new PositiveLevel(50);
            Param4 = new PositiveLevel(50);
        }

        public EffectSettings(byte[] data, int offset)
        {
            // Effect type is 11~47 in SysEx, so a value of 11 means effect 0, and 47 means effect 36.
            // Adjust the value from SysEx to 0~36.
            //Console.Error.WriteLine($"effect type from SysEx = {data[offset]}");
            Kind = (EffectKind)(data[offset] - 11);

            Depth = new EffectDepth(data[offset + 1]);
            Param1 = new PositiveLevel(data[offset + 2]);
            Param2 = new PositiveLevel(data[offset + 3]);
            Param3 = new PositiveLevel(data[offset + 4]);
            Param4 = new PositiveLevel(data[offset + 5]);
        }

        public EffectSettings(byte[] data)
        {
            Kind = (EffectKind)(data[0] - 11);

            Depth = new EffectDepth(data[1]);
            Param1 = new PositiveLevel(data[2]);
            Param2 = new PositiveLevel(data[3]);
            Param3 = new PositiveLevel(data[4]);
            Param4 = new PositiveLevel(data[5]);
        }

        public override string ToString()
        {
            //Console.Error.WriteLine(string.Format("Effect type = {0}", _type));
            EffectName name = EffectNames[(int)Kind];
            var builder = new StringBuilder();
            builder.Append(string.Format("{0}, depth = {1}\n", name.Name, Depth.Value));
            builder.Append(string.Format("P1 {0} = {1}\n", name.ParameterNames[0], Param1.Value));
            builder.Append(string.Format("P2 {0} = {1}\n", name.ParameterNames[1], Param2.Value));
            builder.Append(string.Format("P3 {0} = {1}\n", name.ParameterNames[2], Param3.Value));
            builder.Append(string.Format("P4 {0} = {1}\n", name.ParameterNames[3], Param4.Value));
            return builder.ToString();
        }

#region ISystemExclusiveData implementation for EffectSettings

        public List<byte> Data
        {
            get
            {
                var data = new List<byte>();

                // Adjust the effect type back to 11~47:
                data.Add((byte)((int)Kind + 11));

                data.Add(Depth.ToByte());
                data.Add(Param1.ToByte());
                data.Add(Param2.ToByte());
                data.Add(Param3.ToByte());
                data.Add(Param4.ToByte());

                return data;
            }
        }

        public int DataLength => 6;
    }

#endregion

}
