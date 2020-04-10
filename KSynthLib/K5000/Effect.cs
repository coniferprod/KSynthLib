using System;
using System.Collections.Generic;
using System.Text;
using KSynthLib.Common;

namespace KSynthLib.K5000
{
    public enum EffectType
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

    public class EffectSettings
    {
        private int _type;  // 0~36 (in SysEx 11~47)

        public EffectType Type
        {
            get => (EffectType)_type;
            set => _type = (int)value;
        }
        
        private EffectDepthType _depth;
        public byte Depth  // 0 ~ 100
        {
            get => _depth.Value;
            set => _depth.Value = value;
        }

        private PositiveLevelType _param1;
        public byte Param1 // 0 ~ 127
        {
            get => _param1.Value;
            set => _param1.Value = value;
        }

        private PositiveLevelType _param2;
        public byte Param2 // 0 ~ 127
        {
            get => _param2.Value;
            set => _param2.Value = value;
        }

        private PositiveLevelType _param3;
        public byte Param3 // 0 ~ 127
        {
            get => _param3.Value;
            set => _param3.Value = value;
        }

        private PositiveLevelType _param4;
        public byte Param4 // 0 ~ 127
        {
            get => _param4.Value;
            set => _param4.Value = value;
        }

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
            Type = EffectType.SingleDelay;

            _depth = new EffectDepthType(50);
            _param1 = new PositiveLevelType(50);
            _param2 = new PositiveLevelType(50);
            _param3 = new PositiveLevelType(50);
            _param4 = new PositiveLevelType(50);
        }


        public override string ToString()
        {
            System.Console.WriteLine(string.Format("Effect type = {0} ({0:X2}h)", _type));
            EffectName name = EffectNames[_type];
            StringBuilder builder = new StringBuilder();
            builder.Append(string.Format("{0}, depth = {1}\n", name.Name, Depth));
            builder.Append(string.Format("P1 {0} = {1}\n", name.ParameterNames[0], Param1));
            builder.Append(string.Format("P2 {0} = {1}\n", name.ParameterNames[1], Param2));
            builder.Append(string.Format("P3 {0} = {1}\n", name.ParameterNames[2], Param3));
            builder.Append(string.Format("P4 {0} = {1}\n", name.ParameterNames[3], Param4));
            return builder.ToString();
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();
            data.Add((byte)(_type + 11));
            data.Add(Depth);
            data.Add(Param1);
            data.Add(Param2);
            data.Add(Param3);
            data.Add(Param4);
            return data.ToArray();
        }        
    }

    // Tthe graphical EQ settings of a patch.
    public class GEQSettings
    {
        private FreqType _freq1; // 58(-6) ~ 70(+6), so 64 = 0
        public sbyte Freq1
        {
            get => _freq1.Value;
            set => _freq1.Value = value;
        }

        private FreqType _freq2;
        public sbyte Freq2
        {
            get => _freq2.Value;
            set => _freq2.Value = value;
        }

        private FreqType _freq3;
        public sbyte Freq3
        {
            get => _freq3.Value;
            set => _freq3.Value = value;
        }

        private FreqType _freq4;
        public sbyte Freq4
        {
            get => _freq4.Value;
            set => _freq4.Value = value;
        }

        private FreqType _freq5;
        public sbyte Freq5
        {
            get => _freq5.Value;
            set => _freq5.Value = value;
        }

        private FreqType _freq6;
        public sbyte Freq6
        {
            get => _freq6.Value;
            set => _freq6.Value = value;
        }

        private FreqType _freq7;
        public sbyte Freq7
        {
            get => _freq7.Value;
            set => _freq7.Value = value;
        }

        public GEQSettings()
        {
            _freq1 = new FreqType();
            _freq2 = new FreqType();
            _freq3 = new FreqType();
            _freq4 = new FreqType();
            _freq5 = new FreqType();
            _freq6 = new FreqType();
            _freq7 = new FreqType();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"{Freq1} {Freq2} {Freq3} {Freq4} {Freq5} {Freq6} {Freq7}\n");
            // TODO: Add the sign, like "+6" or "-6"
            return builder.ToString();
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();
            data.Add((byte)(Freq1 + 64));
            data.Add((byte)(Freq2 + 64));
            data.Add((byte)(Freq3 + 64));
            data.Add((byte)(Freq4 + 64));
            data.Add((byte)(Freq5 + 64));
            data.Add((byte)(Freq6 + 64));
            data.Add((byte)(Freq7 + 64));
            return data.ToArray();
        }     
    }
}