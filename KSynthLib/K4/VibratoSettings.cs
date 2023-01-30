using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        [Range(0, 100, ErrorMessage = "{0} must be between {1} and {2}")]
        public int Speed;

        [Range(-50, 50, ErrorMessage = "{0} must be between {1} and {2}")]
        public int Pressure;

        [Range(-50, 50, ErrorMessage = "{0} must be between {1} and {2}")]
        public int Depth;

        public VibratoSettings()
        {
            Shape = LFOShape.Triangle;
            Speed = 0;
            Pressure = 0;
            Depth = 0;
        }

        public VibratoSettings(List<byte> data)
        {
            Shape = (LFOShape)((data[0] >> 4) & 0x03);
            Speed = data[1] & 0x7f;
            Pressure = data[2] & 0x7f;
            Depth = data[3] & 0x7f;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append(
                string.Format(
                    "SHAPE      ={0}\nSPEED      ={1,3}\nDEPTH      ={2,3}\nPRESS DEPTH={3,3}",
                    Enum.GetNames(typeof(LFOShape))[(int)Shape],
                    Speed, Depth, Pressure
                )
            );

            return builder.ToString();
        }
    }
}
