using System.Text;
using System.Collections.Generic;

namespace KSynthLib.K4
{
    public class AutoBendSettings
    {
        public Level Time;
        public Depth Depth;
        public Depth KeyScalingTime;
        public Depth VelocityDepth;

        public AutoBendSettings()
        {
            Time = new Level();
            Depth = new Depth();
            KeyScalingTime = new Depth();
            VelocityDepth = new Depth();
        }

        public AutoBendSettings(byte[] data)
        {
            // When initializing the values, the constructors that take a `byte` argument
            // automatically reset the top bit and scale the value correctly, so we only
            // need to pass in the raw byte from SysEx.
            Time = new Level(data[0]);
            Depth = new Depth(data[1]);
            KeyScalingTime = new Depth(data[2]);
            VelocityDepth = new Depth(data[3]);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(string.Format("TIME       ={0,3}\nDEPTH      ={1,2}\nKS>TIME    ={2,2}\nVEL>DEPTH  ={3,2}",
                Time, Depth, KeyScalingTime, VelocityDepth));
            return builder.ToString();
        }

        public byte[] ToData()
        {
            var data = new List<byte>();

            data.Add(Time.ToByte());

            // The `ToByte` method returns the value as the raw SysEx byte.
            data.Add(Depth.ToByte());
            data.Add(KeyScalingTime.ToByte());
            data.Add(VelocityDepth.ToByte());

            return data.ToArray();
        }
    }
}
