using System.Collections.Generic;

using SyxPack;

namespace KSynthLib.K4
{
    public class AmplifierEnvelope : ISystemExclusiveData
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

        public AmplifierEnvelope(List<byte> data) : this(data[0], data[1], data[2], data[3]) { }

        public override string ToString()
        {
            return $"A:{Attack} D:{Decay} S:{Sustain} R:{Release}";
        }

        //
        // ISystemExclusiveData implementation
        //

        public List<byte> Data
        {
            get
            {
                var data = new List<byte>();

                data.Add(Attack.ToByte());
                data.Add(Decay.ToByte());
                data.Add(Sustain.ToByte());
                data.Add(Release.ToByte());

                return data;
            }
        }

        public int DataLength => 4;
    }
}
