using System.Collections.Generic;

namespace KSynthLib.K4
{
    public class FilterEnvelope : ISystemExclusiveData
    {
        public Level Attack;
        public Level Decay;
        public Depth Sustain;
        public Level Release;

        public FilterEnvelope()
        {
            Attack = new Level();
            Decay = new Level();
            Sustain = new Depth();
            Release = new Level();
        }

        public FilterEnvelope(int a, int d, int s, int r)
        {
            Attack = new Level(a);
            Decay = new Level(d);
            Sustain = new Depth(s);
            Release = new Level(r);
        }

        public FilterEnvelope(List<byte> data)
        {
            Attack = new Level(data[0]);
            Decay = new Level(data[1]);
            Sustain = new Depth(data[2]);
            Release = new Level(data[3]);
        }

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
