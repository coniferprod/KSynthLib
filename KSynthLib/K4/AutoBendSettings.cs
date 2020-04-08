using System;
using System.Text;

using KSynthLib.Common;

namespace KSynthLib.K4
{
    public class AutoBendSettings
    {
        private LevelType time;
        public int Time  // 0~100
        {
            get
            {
                return time.Value;
            }

            set
            {
                time.Value = value;
            }
        }

        private DepthType depth;
        public int Depth // 0~100 (±50)
        {
            get
            {
                return depth.Value;
            }

            set
            {
                depth.Value = value;
            }
        }

        private DepthType keyScalingTime;
        public int KeyScalingTime // 0~100 (±50)
        {
            get
            {
                return keyScalingTime.Value;
            }

            set
            {
                keyScalingTime.Value = value;
            }
        }

        private DepthType velocityDepth;
        public int VelocityDepth // 0~100 (±50)
        {
            get
            {
                return velocityDepth.Value;
            }

            set
            {
                velocityDepth.Value = value;
            }
        }

        public AutoBendSettings()
        {
            time = new LevelType();
            depth = new DepthType();
            keyScalingTime = new DepthType();
            velocityDepth = new DepthType();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(String.Format("TIME       ={0,3}\nDEPTH      ={1,2}\nKS>TIME    ={2,2}\nVEL>DEPTH  ={3,2}", 
                Time, Depth, KeyScalingTime, VelocityDepth));
            return builder.ToString();
        }
    }
}