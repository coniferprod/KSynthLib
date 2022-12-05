using System.Collections.Generic;

using KSynthLib.Common;


namespace KSynthLib.K4
{
    public class AmplifierEnvelope: ISystemExclusiveData
    {
        public Level Attack;
        public Level Decay;
        public Level Sustain;
        public Level Release;

        public AmplifierEnvelope()
        {
            Attack = new Level();
            Decay = new Level();
            Sustain = new Level();
            Release = new Level();
        }

        public AmplifierEnvelope(byte a, byte d, byte s, byte r)
        {
            Attack = new Level(a);
            Decay = new Level(d);
            Sustain = new Level(s);
            Release = new Level(r);
        }

        public AmplifierEnvelope(List<byte> data)
        {
            Attack = new Level(data[0]);
            Decay = new Level(data[1]);
            Sustain = new Level(data[2]);
            Release = new Level(data[3]);
        }

        public override string ToString()
        {
            return $"A:{Attack} D:{Decay} S:{Sustain} R:{Release}";
        }

        public List<byte> GetSystemExclusiveData()
        {
            var data = new List<byte>();

            data.Add(Attack.ToByte());
            data.Add(Decay.ToByte());
            data.Add(Sustain.ToByte());
            data.Add(Release.ToByte());

            return data;
        }
    }
}
