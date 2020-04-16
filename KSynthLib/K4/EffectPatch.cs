using System;
using System.Collections.Generic;

namespace KSynthLib.K4
{
    public enum EffectType
    {
        Reverb1,
        Reverb2,
        Reverb3,
        Reverb4,
        GateReverb,
        ReverseGate,
        NormalDelay,
        StereoPanpotDelay,
        Chorus,
        OverdrivePlusFlanger,
        OverdrivePlusNormalDelay,
        OverdrivePlusReverb,
        NormalDelayPlusNormalDelay,
        NormalDelayPlusSteroPanpotDelay,
        ChorusPlusNormalDelay,
        ChorusPlusStereoPanpotDelay
    }

    public class EffectSubmix
    {
        private int pan;
        private int send1;
        private int send2;

        public EffectSubmix()
        {
            this.pan = 8;  // TODO: figure out value vs. property
            this.send1 = 0;
            this.send2 = 0;
        }

        public EffectSubmix(byte d0, byte d1, byte d2)
        {
            this.pan = d0;
            this.send1 = d1;
            this.send2 = d2;
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();
            data.Add((byte)pan);
            data.Add((byte)send1);
            data.Add((byte)send2);
            return data.ToArray();
        }
    }

    public class EffectPatch : Patch
    {
        public const int DataSize = 35;

        public const int EffectSubmixCount = 8;

        public EffectType Type;

        private EffectParameter1Type _param1;
        public int Param1
        {
            get => _param1.Value;
            set => _param1.Value = value;
        }

        private EffectParameter1Type _param2;
        public int Param2
        {
            get => _param2.Value;
            set => _param2.Value = value;
        }

        private EffectParameter3Type _param3;
        public int Param3
        {
            get => _param3.Value;
            set => _param3.Value = value;
        }

        public EffectSubmix[] submixes;

        public EffectPatch()
        {
            Type = EffectType.Reverb1;
            _param1 = new EffectParameter1Type();
            _param2 = new EffectParameter1Type();
            _param3 = new EffectParameter3Type();

            submixes = new EffectSubmix[EffectSubmixCount];
            for (int i = 0; i < EffectSubmixCount; i++)
            {
                submixes[i] = new EffectSubmix();
            }
        }

        public EffectPatch(byte[] data)
        {
            Type = (EffectType)data[0];
            _param1 = new EffectParameter1Type(data[1]);
            _param2 = new EffectParameter1Type(data[2]);
            _param3 = new EffectParameter3Type(data[3]);

            int offset = 4;
            submixes = new EffectSubmix[EffectSubmixCount];
            for (int i = 0; i < EffectSubmixCount; i++)
            {
                submixes[i] = new EffectSubmix(data[offset], data[offset + 1], data[offset + 2]);
                offset += 3;
            }
        }

        protected override byte[] CollectData()
        {
            List<byte> data = new List<byte>();

            data.Add((byte)this.Type);
            data.Add((byte)this.Param1);
            data.Add((byte)this.Param2);
            data.Add((byte)this.Param3);

            // Add six dummy bytes
            data.Add(0);
            data.Add(0);
            data.Add(0);
            data.Add(0);
            data.Add(0);
            data.Add(0);

            for (int i = 0; i < EffectSubmixCount; i++)
            {
                data.AddRange(this.submixes[i].ToData());
            }

            return data.ToArray();
        }
    }
}
