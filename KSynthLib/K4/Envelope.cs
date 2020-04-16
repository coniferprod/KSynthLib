using System;
using System.Text;
using System.Collections.Generic;

namespace KSynthLib.K4
{
    public class Envelope
    {
        private LevelType _attack;
        public int Attack // 0~100
        {
            get => _attack.Value;
            set => _attack.Value = value;
        }

        private LevelType _decay;
        public int Decay // 0~100
        {
            get => _decay.Value;
            set => _decay.Value = value;
        }

        private LevelType _sustain;
        public int Sustain // 0~100
        {
            get => _sustain.Value;
            set => _sustain.Value = value;
        }

        private LevelType _release;
        public int Release // 0~100
        {
            get => _release.Value;
            set => _release.Value = value;
        }

        public Envelope()
        {
            _attack = new LevelType();
            _decay = new LevelType();
            _sustain = new LevelType();
            _release = new LevelType();
        }

        public Envelope(int a, int d, int s, int r)
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