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

        private EffectDepthType _dryWet1;
        public byte DryWet1  // 0 ~ 100
        {
            get => _dryWet1.Value;
            set => _dryWet1.Value = value;
        }

        private EffectDepthType _dryWet2;
        public byte DryWet2  // 0 ~ 100
        {
            get => _dryWet2.Value;
            set => _dryWet2.Value = value;
        }

        private PositiveLevelType _param2;
        public byte Param2  // 0 ~ 127
        {
            get => _param2.Value;
            set => _param2.Value = value;
        }

        private PositiveLevelType _param3;
        public byte Param3  // 0 ~ 127
        {
            get => _param3.Value;
            set => _param3.Value = value;
        }

        private PositiveLevelType _param4;
        public byte Param4  // 0 ~ 127
        {
            get => _param4.Value;
            set => _param4.Value = value;
        }

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

            _dryWet1 = new EffectDepthType(50);
            _dryWet2 = new EffectDepthType(50);
            _param2 = new PositiveLevelType(64);
            _param3 = new PositiveLevelType(64);
            _param4 = new PositiveLevelType(64);
        }

        public ReverbSettings(byte[] data, int offset) : this()
        {
            ReverbType = data[offset];
            _dryWet1 = new EffectDepthType(data[offset + 1]);
            _dryWet2 = new EffectDepthType(data[offset + 2]);
            _param2 = new PositiveLevelType(data[offset + 3]);
            _param3 = new PositiveLevelType(data[offset + 4]);
            _param4 = new PositiveLevelType(data[offset + 5]);
        }

        public override string ToString()
        {
            ReverbName name = ReverbNames[ReverbType];
            StringBuilder builder = new StringBuilder();
            builder.Append($"{name.Name}, Dry/Wet 1 = {DryWet1}\n");
            builder.Append($"P1 {name.ParameterNames[0]} = {DryWet2}\n");
            builder.Append($"P2 {name.ParameterNames[1]} = {Param2}\n");
            builder.Append($"P3 {name.ParameterNames[2]} = {Param3}\n");
            builder.Append($"P4 {name.ParameterNames[3]} = {Param4}\n");
            return builder.ToString();
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();
            data.Add(ReverbType);
            data.Add(DryWet1);
            data.Add(DryWet2);
            data.Add(Param2);
            data.Add(Param3);
            data.Add(Param4);
            return data.ToArray();
        }
    }
}
