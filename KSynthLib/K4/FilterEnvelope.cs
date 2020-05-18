using System;
using System.Collections.Generic;

namespace KSynthLib.K4
{
    public class FilterEnvelope
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

        private DepthType _sustain;
        public sbyte Sustain // -50~+50, in SysEx 0~100
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

        public FilterEnvelope()
        {
            _attack = new LevelType();
            _decay = new LevelType();
            _sustain = new DepthType();
            _release = new LevelType();
        }

        public FilterEnvelope(byte a, byte d, sbyte s, byte r)
        {
            _attack = new LevelType(a);
            _decay = new LevelType(d);
            _sustain = new DepthType(s);
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
            data.Add((byte)(Sustain + 50));
            data.Add((byte)Release);
            return data.ToArray();
        }
    }
}