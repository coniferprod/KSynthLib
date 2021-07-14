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

        private LevelType _speed;
        public byte Speed // 0~100
        {
            get => _speed.Value;
            set => _speed.Value = value;
        }

        private DepthType _pressure;
        public sbyte Pressure // 0~100 (±50)
        {
            get => _pressure.Value;
            set => _pressure.Value = value;
        }

        private DepthType _depth;
        public sbyte Depth // 0~100 (±50)
        {
            get => _depth.Value;
            set => _depth.Value = value;
        }

        public VibratoSettings()
        {
            Shape = LFOShape.Triangle;
            _speed = new LevelType();
            _pressure = new DepthType();
            _depth = new DepthType();
        }

        public VibratoSettings(List<byte> data)
        {
            Shape = (LFOShape)((data[0] >> 4) & 0x03);
            _speed = new LevelType(data[1]);
            _pressure = new DepthType(data[2]);
            _depth = new DepthType(data[3]);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(string.Format("SHAPE      ={0}\nSPEED      ={1,3}\nDEPTH      ={2,3}\nPRESS DEPTH={3,3}",
                Enum.GetNames(typeof(LFOShape))[(int)Shape],
                Speed, Depth, Pressure));
            return builder.ToString();
        }
    }
}