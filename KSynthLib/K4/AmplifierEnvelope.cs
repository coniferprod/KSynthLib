using System.Collections.Generic;

namespace KSynthLib.K4
{
    public class AmplifierEnvelope
    {
        public LevelType Attack;
        public LevelType Decay;
        public LevelType Sustain;
        public LevelType Release;

        public AmplifierEnvelope()
        {
            Attack = new LevelType();
            Decay = new LevelType();
            Sustain = new LevelType();
            Release = new LevelType();
        }

        public AmplifierEnvelope(byte a, byte d, byte s, byte r)
        {
            Attack = new LevelType(a);
            Decay = new LevelType(d);
            Sustain = new LevelType(s);
            Release = new LevelType(r);
        }

        public AmplifierEnvelope(List<byte> data)
        {
            Attack = new LevelType(data[0]);
            Decay = new LevelType(data[1]);
            Sustain = new LevelType(data[2]);
            Release = new LevelType(data[3]);
        }

        public override string ToString()
        {
            return $"A:{Attack} D:{Decay} S:{Sustain} R:{Release}";
        }

        public byte[] ToData()
        {
            var data = new List<byte>();

            data.Add(Attack.ToByte());
            data.Add(Decay.ToByte());
            data.Add(Sustain.ToByte());
            data.Add(Release.ToByte());

            return data.ToArray();
        }
    }
}
