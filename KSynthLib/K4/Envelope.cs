using System;
using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K4
{
    public class Envelope
    {
        private LevelType attack;
        public int Attack // 0~100
        {
            get
            {
                return attack.Value;
            }

            set
            {
                attack.Value = value;
            }
        }

        private LevelType decay;
        public int Decay // 0~100
        {
            get
            {
                return decay.Value;
            }

            set
            {
                decay.Value = value;
            }
        }

        private LevelType sustain;
        public int Sustain // 0~100
        {
            get
            {
                return sustain.Value;
            }

            set
            {
                sustain.Value = value;
            }
        }

        private LevelType release;
        public int Release // 0~100
        {
            get
            {
                return release.Value;
            }

            set
            {
                release.Value = value;
            }
        }

        public Envelope()
        {
            attack = new LevelType();
            decay = new LevelType();
            sustain = new LevelType();
            release = new LevelType();
        }

        public Envelope(int a, int d, int s, int r)
        {            
            attack = new LevelType(a);
            decay = new LevelType(d);
            sustain = new LevelType(s);
            release = new LevelType(r);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(String.Format("A:{0} D:{1} S:{2} R:{3}", Attack, Decay, Sustain, Release));
            return builder.ToString();
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();
            data.Add((byte)Attack);
            data.Add((byte)Decay);
            data.Add((byte)Sustain);
            data.Add((byte)Release);
            return data.ToArray();
        }
    }
}