using System.Text;
using System.Collections.Generic;

namespace KSynthLib.K4
{
    public class AutoBendSettings
    {
        public LevelType Time;
        public DepthType Depth;
        public DepthType KeyScalingTime;
        public DepthType VelocityDepth;

        public AutoBendSettings()
        {
            Time = new LevelType();
            Depth = new DepthType();
            KeyScalingTime = new DepthType();
            VelocityDepth = new DepthType();
        }

        public AutoBendSettings(byte[] data)
        {
            // When initializing the values, the constructors that take a `byte` argument
            // automatically reset the top bit and scale the value correctly, so we only
            // need to pass in the raw byte from SysEx.
            Time = new LevelType(data[0]);
            Depth = new DepthType(data[1]);
            KeyScalingTime = new DepthType(data[2]);
            VelocityDepth = new DepthType(data[3]);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(string.Format("TIME       ={0,3}\nDEPTH      ={1,2}\nKS>TIME    ={2,2}\nVEL>DEPTH  ={3,2}",
                Time, Depth, KeyScalingTime, VelocityDepth));
            return builder.ToString();
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();

            data.Add(Time.ToByte());

            // The `ToByte` method returns the value as the raw SysEx byte.
            data.Add(Depth.ToByte());
            data.Add(KeyScalingTime.ToByte());
            data.Add(VelocityDepth.ToByte());

            return data.ToArray();
        }
    }
}
