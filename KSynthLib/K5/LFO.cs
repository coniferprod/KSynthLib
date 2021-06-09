using System;
using System.Text;
using System.Collections.Generic;

namespace KSynthLib.K5
{
    public enum LFOShape  // 1 ~ 6
    {
        Triangle,
        InverseTriangle,
        Square,
        InverseSquare,
        Sawtooth,
        InverseSawtooth
    }

    public class LFO
    {
        public LFOShape Shape;
        public byte Speed;  // 0~99

        private PositiveDepthType _delay;  // 0~31
        public byte Delay
        {
            get => _delay.Value;
            set => _delay.Value = value;
        }

        private PositiveDepthType _trend; // 0~31
        public byte Trend
        {
            get => _trend.Value;
            set => _trend.Value = value;
        }

        public LFO()
        {
            _delay = new PositiveDepthType();
            _trend = new PositiveDepthType();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("*LFO*\n\n");
            builder.Append($" SHAPE= {Shape}\n SPEED= {Speed,2}\n DELAY= {Delay,2}\n TREND= {Trend,2}\n\n\n");

            return builder.ToString();
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();

            data.Add(Convert.ToByte(Shape));
            data.Add(Speed);
            data.Add(Delay);
            data.Add(Trend);

            return data.ToArray();
        }
    }
}
