using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using KSynthLib.Common;


namespace KSynthLib.K4
{
    public enum LFOShape // 0/TRI, 1/SAW, 2/SQR, 3/RND
    {
        Triangle,
        Sawtooth,
        Square,
        Random
    };

    public class LFOSettings : ISystemExclusiveData
    {
        public LFOShape Shape;

        [Range(0, 100, ErrorMessage = "{0} must be between {1} and {2}")]
        public int Speed;

        [Range(0, 100, ErrorMessage = "{0} must be between {1} and {2}")]
        public int Delay;

        [Range(-50, 50, ErrorMessage = "{0} must be between {1} and {2}")]
        public int Depth;

        [Range(-50, 50, ErrorMessage = "{0} must be between {1} and {2}")]
        public int PressureDepth;

        public LFOSettings()
        {
            Shape = LFOShape.Triangle;
            Speed = 0;
            Delay = 0;
            Depth = 0;
            PressureDepth = 0;
        }

        public LFOSettings(List<byte> data)
        {
            Shape = (LFOShape)(data[0] & 0x03);
            Speed = data[1];
            Delay = data[2];
            Depth = SystemExclusiveDataConverter.DepthFromByte(data[3]);
            PressureDepth = SystemExclusiveDataConverter.DepthFromByte(data[4]);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append(
                string.Format(
                    "SHAPE      ={0}\nSPEED     ={1,3}\nDELAY     ={2,3}\nDEPTH     ={3,3}, PRESS DEPTH={4,3}",
                    Enum.GetNames(typeof(LFOShape))[(int)Shape],
                    Speed, Delay, Depth, PressureDepth
                )
            );

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

                data.Add((byte)Shape);
                data.Add((byte)Speed);
                data.Add((byte)Delay);
                data.Add(SystemExclusiveDataConverter.ByteFromDepth(Depth));
                data.Add(SystemExclusiveDataConverter.ByteFromDepth(PressureDepth));

                return data;
            }
        }

        public int DataLength => 5;
    }
}
