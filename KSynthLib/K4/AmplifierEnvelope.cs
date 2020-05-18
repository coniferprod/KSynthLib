using System;
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

        public override string ToString()
        {
            return $"A:{Attack} D:{Decay} S:{Sustain} R:{Release}";
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
