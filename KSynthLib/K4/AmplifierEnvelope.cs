using System.Collections.Generic;

namespace KSynthLib.K4
{
    public class AmplifierEnvelope
    {
        private LevelType _attack;
        public byte Attack // 0~100
        {
            get => _attack.Value;
            set => _attack.Value = value;
        }

        private LevelType _decay;
        public byte Decay // 0~100
        {
            get => _decay.Value;
            set => _decay.Value = value;
        }

        private LevelType _sustain;
        public byte Sustain // 0~100
        {
            get => _sustain.Value;
            set => _sustain.Value = value;
        }

        private LevelType _release;
        public byte Release // 0~100
        {
            get => _release.Value;
            set => _release.Value = value;
        }

        public AmplifierEnvelope()
        {
            _attack = new LevelType();
            _decay = new LevelType();
            _sustain = new LevelType();
            _release = new LevelType();
        }

        public AmplifierEnvelope(byte a, byte d, byte s, byte r)
        {
            _attack = new LevelType(a);
            _decay = new LevelType(d);
            _sustain = new LevelType(s);
            _release = new LevelType(r);
        }

        public AmplifierEnvelope(List<byte> data)
        {
            _attack = new LevelType(data[0]);
            _decay = new LevelType(data[1]);
            _sustain = new LevelType(data[2]);
            _release = new LevelType(data[3]);
        }

        public override string ToString()
        {
            return $"A:{Attack} D:{Decay} S:{Sustain} R:{Release}";
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();
            data.Add(Attack);
            data.Add(Decay);
            data.Add(Sustain);
            data.Add(Release);
            return data.ToArray();
        }
    }
}
