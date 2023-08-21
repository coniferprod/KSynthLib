using System;
using System.Text;
using System.Collections.Generic;

using SyxPack;

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

    public class LFO : ISystemExclusiveData
    {
        public LFOShape Shape;
        public byte Speed;  // 0~99

        public PositiveDepth Delay;  // 0~31
        public PositiveDepth Trend; // 0~31

        public LFO()
        {
            this.Delay = new PositiveDepth();
            this.Trend = new PositiveDepth();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append("*LFO*\n\n");
            builder.Append($" SHAPE= {Shape}\n SPEED= {Speed,2}\n DELAY= {Delay.Value,2}\n TREND= {Trend.Value,2}\n\n\n");

            return builder.ToString();
        }

        public List<byte> Data
        {
            get
            {
                var data = new List<byte>();

                data.Add(Convert.ToByte(Shape));
                data.Add(Speed);
                data.Add(Delay.ToByte());
                data.Add(Trend.ToByte());

                return data;
            }
        }

        public int DataLength => 4;  // TODO: Check length
    }
}
