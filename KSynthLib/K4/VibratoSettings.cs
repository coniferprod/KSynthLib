using System;
using System.Text;
using System.Collections.Generic;

namespace KSynthLib.K4
{
    public enum WheelAssignType
    {
        Vibrato,
        LFO,
        DCF
    }

    public class VibratoSettings
    {
        public LFOShape Shape; // 0/TRI, 1/SAW, 2/SQR, 3/RND
        public LevelType Speed;
        public DepthType Pressure;
        public DepthType Depth;

        public VibratoSettings()
        {
            Shape = LFOShape.Triangle;
            Speed = new LevelType();
            Pressure = new DepthType();
            Depth = new DepthType();
        }

        public VibratoSettings(List<byte> data)
        {
            Shape = (LFOShape)((data[0] >> 4) & 0x03);
            Speed = new LevelType(data[1] & 0x7f);
            Pressure = new DepthType(data[2] & 0x7f);
            Depth = new DepthType(data[3] & 0x7f);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(string.Format("SHAPE      ={0}\nSPEED      ={1,3}\nDEPTH      ={2,3}\nPRESS DEPTH={3,3}",
                Enum.GetNames(typeof(LFOShape))[(int)Shape],
                Speed, Depth, Pressure));
            return builder.ToString();
        }
    }
}
