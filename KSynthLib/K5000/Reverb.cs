using System;
using System.Collections.Generic;
using System.Text;

using KSynthLib.Common;

namespace KSynthLib.K5000
{
    public class ReverbName
    {
        public string Name;
        public string[] ParameterNames;
    }

    public class ReverbSettings
    {
        public byte ReverbType;  // 0 ~ 10

        private EffectDepthType _dryWet;
        public byte DryWet  // 0 ~ 100
        {
            get => _dryWet.Value;
            set => _dryWet.Value = value;
        }
        
        private PositiveLevelType _param1;
        public byte Param1  // 0 ~ 127
        {
            get => _param1.Value;
            set => _param1.Value = value;
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

            _dryWet = new EffectDepthType(50);
            _param1 = new PositiveLevelType(64);
            _param2 = new PositiveLevelType(64);
            _param3 = new PositiveLevelType(64);
            _param4 = new PositiveLevelType(64);
        }

        public override string ToString()
        {
            ReverbName name = ReverbNames[ReverbType];
            StringBuilder builder = new StringBuilder();
            builder.Append(string.Format("{0}, dry/wet = {1}\n", name.Name, DryWet));
            builder.Append(string.Format("P1 {0} = {1}\n", name.ParameterNames[0], Param1));
            builder.Append(string.Format("P2 {0} = {1}\n", name.ParameterNames[1], Param2));
            builder.Append(string.Format("P3 {0} = {1}\n", name.ParameterNames[2], Param3));
            builder.Append(string.Format("P4 {0} = {1}\n", name.ParameterNames[3], Param4));
            return builder.ToString();
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();
            data.Add(ReverbType);
            data.Add(DryWet);
            data.Add(Param1);
            data.Add(Param2);
            data.Add(Param3);
            data.Add(Param4);
            return data.ToArray();
        }
    }
}
