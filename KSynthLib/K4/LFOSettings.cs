using System;
using System.Text;
using System.Collections.Generic;

namespace KSynthLib.K4
{
    public enum LFOShape // 0/TRI, 1/SAW, 2/SQR, 3/RND
    {
        Triangle,
        Sawtooth,
        Square,
        Random
    };

    public class LFOSettings
    {
        public LFOShape Shape;

        private LevelType _speed;
        public byte Speed  // 0~100
        {
            get => _speed.Value;
            set => _speed.Value = value;
        }

        private LevelType _delay;
        public byte Delay  // 0~100
        {
            get => _delay.Value;
            set => _delay.Value = value;
        }

        private DepthType _depth;
        public sbyte Depth // 0~100 (±50)
        {
            get => _depth.Value;
            set => _depth.Value = value;
        }

        private DepthType _pressureDepth;
        public sbyte PressureDepth // 0~100 (±50)
        {
            get => _pressureDepth.Value;
            set => _pressureDepth.Value = value;
        }

        public LFOSettings()
        {
            Shape = LFOShape.Triangle;
            _speed = new LevelType();
            _delay = new LevelType();
            _depth = new DepthType();
            _pressureDepth = new DepthType();
        }

        public LFOSettings(List<byte> data)
        {
            Shape = (LFOShape)(data[0] & 0x03);
            _speed = new LevelType(data[1]);
            _delay = new LevelType(data[2]);
            _depth = new DepthType(data[3]);
            _pressureDepth = new DepthType(data[4]);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(string.Format("SHAPE      ={0}\nSPEED     ={1,3}\nDELAY     ={2,3}\nDEPTH     ={3,3}, PRESS DEPTH={4,3}",
                Enum.GetNames(typeof(LFOShape))[(int)Shape],
                Speed, Delay, Depth, PressureDepth));
            return builder.ToString();
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();
            data.Add((byte)Shape);
            data.Add(Speed);
            data.Add(Delay);
            data.Add(_depth.AsByte());
            data.Add(_pressureDepth.AsByte());
            return data.ToArray();
        }
    }
}