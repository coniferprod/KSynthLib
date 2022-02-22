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
        public Level Speed;
        public Level Delay;
        public Depth Depth;
        public Depth PressureDepth;

        public LFOSettings()
        {
            Shape = LFOShape.Triangle;
            Speed = new Level();
            Delay = new Level();
            Depth = new Depth();
            PressureDepth = new Depth();
        }

        public LFOSettings(List<byte> data)
        {
            Shape = (LFOShape)(data[0] & 0x03);
            Speed = new Level(data[1]);
            Delay = new Level(data[2]);
            Depth = new Depth(data[3]);
            PressureDepth = new Depth(data[4]);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(string.Format("SHAPE      ={0}\nSPEED     ={1,3}\nDELAY     ={2,3}\nDEPTH     ={3,3}, PRESS DEPTH={4,3}",
                Enum.GetNames(typeof(LFOShape))[(int)Shape],
                Speed, Delay, Depth, PressureDepth));
            return builder.ToString();
        }

        public byte[] ToData()
        {
            var data = new List<byte>();
            data.Add((byte)Shape);
            data.Add(Speed.ToByte());
            data.Add(Delay.ToByte());
            data.Add(Depth.ToByte());
            data.Add(PressureDepth.ToByte());
            return data.ToArray();
        }
    }
}
