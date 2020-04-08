using System;
using System.Text;

using KSynthLib.Common;

namespace KSynthLib.K4
{
    public enum WheelAssign
    {
        Vibrato,
        LFO,
        DCF
    }

    public class VibratoSettings
    {
        public LFOShape Shape; // 0/TRI, 1/SAW, 2/SQR, 3/RND

        private LevelType speed;
        public int Speed // 0~100
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

        private DepthType pressure;
        public int Pressure // 0~100 (±50)
        {
            get
            {
                return pressure.Value;
            }

            set
            {
                pressure.Value = value;
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

        public VibratoSettings()
        {
            speed = new LevelType();
            pressure = new DepthType();
            depth = new DepthType();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(String.Format("SHAPE      ={0}\nSPEED      ={1,3}\nDEPTH      ={2,3}\nPRESS DEPTH={3,3}", 
                Enum.GetNames(typeof(LFOShape))[(int)Shape], 
                Speed, Depth, Pressure));
            return builder.ToString();
        }
    }
}