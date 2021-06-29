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
            _time = new LevelType((byte)(data[0] & 0x7f));
            _depth = new DepthType((sbyte)((data[1] & 0x7f) - 50)); // 0~100 to ±50
            _keyScalingTime = new DepthType((sbyte)((data[2] & 0x7f) - 50)); // 0~100 to ±50
            _velocityDepth = new DepthType((sbyte)((data[3] & 0x7f) - 50)); // 0~100 to ±50
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
            data.Add((byte)(Depth + 50)); // ±50 to 0...100
            data.Add((byte)(KeyScalingTime + 50)); // ±50 to 0...100
            data.Add((byte)(VelocityDepth + 50)); // ±50 to 0...100

            return data.ToArray();
        }
    }
}