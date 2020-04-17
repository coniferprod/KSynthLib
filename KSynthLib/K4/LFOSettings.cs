using System;
using System.Text;

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
            _speed = new LevelType();
            _delay = new LevelType();
            _depth = new DepthType();
            _pressureDepth = new DepthType();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(string.Format("SHAPE      ={0}\nSPEED     ={1,3}\nDELAY     ={2,3}\nDEPTH     ={3,3}, PRESS DEPTH={4,3}",
                Enum.GetNames(typeof(LFOShape))[(int)Shape],
                Speed, Delay, Depth, PressureDepth));
            return builder.ToString();
        }
    }
}