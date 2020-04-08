using System;
using System.Text;

using KSynthLib.Common;

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

        private LevelType speed;
        public int Speed  // 0~100
        {
            get
            {
                return speed.Value;
            }

            set
            {
                speed.Value = value;
            }
        }

        private LevelType delay;
        public int Delay  // 0~100
        {
            get
            {
                return delay.Value;
            }

            set
            {
                delay.Value = value;
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
        
        private DepthType pressureDepth;
        public int PressureDepth // 0~100 (±50)
        {
            get
            {
                return pressureDepth.Value;
            }

            set
            {
                pressureDepth.Value = value;
            }
        }

        public LFOSettings()
        {
            speed = new LevelType();
            delay = new LevelType();
            depth = new DepthType();
            pressureDepth = new DepthType();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(String.Format("SHAPE      ={0}\nSPEED     ={1,3}\nDELAY     ={2,3}\nDEPTH     ={3,3}, PRESS DEPTH={4,3}", 
                Enum.GetNames(typeof(LFOShape))[(int)Shape],
                Speed, Delay, Depth, PressureDepth));
            return builder.ToString();
        }    
    }
}