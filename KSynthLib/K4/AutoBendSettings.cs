using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using KSynthLib.Common;


namespace KSynthLib.K4
{
    public class AutoBendSettings: ISystemExclusiveData
    {
        [Range(0, 100, ErrorMessage = "{0} must be between {1} and {2}")]
        public int Time;

        [Range(-50, 50, ErrorMessage = "{0} must be between {1} and {2}")]
        public int Depth;

        [Range(-50, 50, ErrorMessage = "{0} must be between {1} and {2}")]
        public int KeyScalingTime;

        [Range(-50, 50, ErrorMessage = "{0} must be between {1} and {2}")]
        public int VelocityDepth;

        public AutoBendSettings()
        {
            Time = 0;
            Depth = 0;
            KeyScalingTime = 0;
            VelocityDepth = 0;
        }

        public AutoBendSettings(byte[] data)
        {
            // When initializing the values, the constructors that take a `byte` argument
            // automatically reset the top bit and scale the value correctly, so we only
            // need to pass in the raw byte from SysEx.
            Time = data[0];
            Depth = data[1];
            KeyScalingTime = data[2];
            VelocityDepth = data[3];
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(string.Format("TIME       ={0,3}\nDEPTH      ={1,2}\nKS>TIME    ={2,2}\nVEL>DEPTH  ={3,2}",
                Time, Depth, KeyScalingTime, VelocityDepth));
            return builder.ToString();
        }

        public List<byte> GetSystemExclusiveData()
        {
            var data = new List<byte>();

            data.Add((byte)Time);

            // The `ToByte` method returns the value as the raw SysEx byte.
            data.Add(ByteConverter.ByteFromDepth(Depth));
            data.Add(ByteConverter.ByteFromDepth(KeyScalingTime));
            data.Add(ByteConverter.ByteFromDepth(VelocityDepth));

            return data;
        }
    }
}
