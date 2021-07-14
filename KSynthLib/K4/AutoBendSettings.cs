using System.Text;
using System.Collections.Generic;

namespace KSynthLib.K4
{
    public class AutoBendSettings
    {
        private LevelType _time;
        public byte Time  // 0~100
        {
            get => _time.Value;
            set => _time.Value = value;
        }

        private DepthType _depth;
        public sbyte Depth // 0~100 (±50)
        {
            get => _depth.Value;
            set => _depth.Value = value;
        }

        private DepthType _keyScalingTime;
        public sbyte KeyScalingTime // 0~100 (±50)
        {
            get => _keyScalingTime.Value;
            set => _keyScalingTime.Value = value;
        }

        private DepthType _velocityDepth;
        public sbyte VelocityDepth // 0~100 (±50)
        {
            get => _velocityDepth.Value;
            set => _velocityDepth.Value = value;
        }

        public AutoBendSettings()
        {
            _time = new LevelType();
            _depth = new DepthType();
            _keyScalingTime = new DepthType();
            _velocityDepth = new DepthType();
        }

        public AutoBendSettings(byte[] data)
        {
            // When initializing the values, the constructors that take a `byte` argument
            // automatically reset the top bit and scale the value correctly, so we only
            // need to pass in the raw byte from SysEx.
            _time = new LevelType(data[0]);
            _depth = new DepthType(data[1]);
            _keyScalingTime = new DepthType(data[2]);
            _velocityDepth = new DepthType(data[3]);
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

            data.Add(Time);

            // The `AsByte` method returns the value as the raw SysEx byte.
            data.Add(_depth.AsByte());
            data.Add(_keyScalingTime.AsByte());
            data.Add(_velocityDepth.AsByte());

            return data.ToArray();
        }
    }
}