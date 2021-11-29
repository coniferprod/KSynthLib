using System.Collections.Generic;
using System.Text;

namespace KSynthLib.K5000
{
    public class ReverbName
    {
        public string Name;
        public string[] ParameterNames;
    }

    public class ReverbSettings
    {
        public const int DataSize = 6;

        public byte ReverbType;  // 0 ~ 10
        public EffectDepth DryWet1;
        public EffectDepth DryWet2;
        public PositiveLevel Param2;
        public PositiveLevel Param3;
        public PositiveLevel Param4;

        public static ReverbName[] ReverbNames =
        {
            /*  0 */ new ReverbName { Name = "Hall 1", ParameterNames = new string[] { "Dry/Wet 2", "Reverb Time", "Predelay Time", "High Frequency Damping" } },
            /*  1 */ new ReverbName { Name = "Hall 2", ParameterNames = new string[] { "Dry/Wet 2", "Reverb Time", "Predelay Time", "High Frequency Damping" } },
            /*  2 */ new ReverbName { Name = "Hall 3", ParameterNames = new string[] { "Dry/Wet 2", "Reverb Time", "Predelay Time", "High Frequency Damping" } },
            /*  3 */ new ReverbName { Name = "Room 1", ParameterNames = new string[] { "Dry/Wet 2", "Reverb Time", "Predelay Time", "High Frequency Damping" } },
            /*  4 */ new ReverbName { Name = "Room 2", ParameterNames = new string[] { "Dry/Wet 2", "Reverb Time", "Predelay Time", "High Frequency Damping" } },
            /*  5 */ new ReverbName { Name = "Room 3", ParameterNames = new string[] { "Dry/Wet 2", "Reverb Time", "Predelay Time", "High Frequency Damping" } },
            /*  6 */ new ReverbName { Name = "Plate 1", ParameterNames = new string[] { "Dry/Wet 2", "Reverb Time", "Predelay Time", "High Frequency Damping" } },
            /*  7 */ new ReverbName { Name = "Plate 2", ParameterNames = new string[] { "Dry/Wet 2", "Reverb Time", "Predelay Time", "High Frequency Damping" } },
            /*  8 */ new ReverbName { Name = "Plate 3", ParameterNames = new string[] { "Dry/Wet 2", "Reverb Time", "Predelay Time", "High Frequency Damping" } },
            /*  9 */ new ReverbName { Name = "Reverse", ParameterNames = new string[] { "Dry/Wet 2", "Feedback", "Predelay Time", "High Frequency Damping" } },
            /* 10 */ new ReverbName { Name = "Long Delay", ParameterNames = new string[]{ "Dry/Wet 2", "Feedback", "Delay Time", "High Frequency Damping" } }
        };

        public ReverbSettings()
        {
            ReverbType = 0;

            DryWet1 = new EffectDepth(50);
            DryWet2 = new EffectDepth(50);
            Param2 = new PositiveLevel(64);
            Param3 = new PositiveLevel(64);
            Param4 = new PositiveLevel(64);
        }

        public ReverbSettings(byte[] data, int offset) : this()
        {
            ReverbType = data[offset];
            DryWet1 = new EffectDepth(data[offset + 1]);
            DryWet2 = new EffectDepth(data[offset + 2]);
            Param2 = new PositiveLevel(data[offset + 3]);
            Param3 = new PositiveLevel(data[offset + 4]);
            Param4 = new PositiveLevel(data[offset + 5]);
        }

        public override string ToString()
        {
            ReverbName name = ReverbNames[ReverbType];
            var builder = new StringBuilder();
            builder.Append($"{name.Name}, Dry/Wet 1 = {DryWet1.Value}\n");
            builder.Append($"P1 {name.ParameterNames[0]} = {DryWet2.Value}\n");
            builder.Append($"P2 {name.ParameterNames[1]} = {Param2.Value}\n");
            builder.Append($"P3 {name.ParameterNames[2]} = {Param3.Value}\n");
            builder.Append($"P4 {name.ParameterNames[3]} = {Param4.Value}\n");
            return builder.ToString();
        }

        public byte[] ToData() => new List<byte>() { ReverbType, DryWet1.ToByte(), DryWet2.ToByte(), Param2.ToByte(), Param3.ToByte(), Param4.ToByte() }.ToArray();
    }
}
