using System.Collections.Generic;

namespace KSynthLib.K4
{
    public class FilterEnvelope
    {
        public LevelType Attack;
        public LevelType Decay;
        public DepthType Sustain;
        public LevelType Release;

        public FilterEnvelope()
        {
            Attack = new LevelType();
            Decay = new LevelType();
            Sustain = new DepthType();
            Release = new LevelType();
        }

        public FilterEnvelope(int a, int d, int s, int r)
        {
            Attack = new LevelType(a);
            Decay = new LevelType(d);
            Sustain = new DepthType(s);
            Release = new LevelType(r);
        }

        public FilterEnvelope(List<byte> data)
        {
            Attack = new LevelType(data[0]);
            Decay = new LevelType(data[1]);
            Sustain = new DepthType(data[2]);  // constructor adjusts to range
            Release = new LevelType(data[3]);
        }

        public override string ToString()
        {
            return $"A:{Attack} D:{Decay} S:{Sustain} R:{Release}";
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();
            data.Add(Attack.ToByte());
            data.Add(Decay.ToByte());
            data.Add(Sustain.ToByte());
            data.Add(Release.ToByte());
            return data.ToArray();
        }
    }
}
