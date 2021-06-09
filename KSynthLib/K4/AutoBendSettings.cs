using System.Text;

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

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(string.Format("TIME       ={0,3}\nDEPTH      ={1,2}\nKS>TIME    ={2,2}\nVEL>DEPTH  ={3,2}",
                Time, Depth, KeyScalingTime, VelocityDepth));
            return builder.ToString();
        }
    }
}