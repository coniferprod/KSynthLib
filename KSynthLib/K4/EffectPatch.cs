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

        public const int NumEffectSubmixes = 8;

        private EffectType type;
        private int param1;
        private int param2;
        private int param3;

        public EffectSubmix[] submixes;

        public EffectPatch()
        {
            type = EffectType.Reverb1;
            param1 = 0;
            param2 = 0;
            param3 = 0;

            submixes = new EffectSubmix[NumEffectSubmixes];
            for (int i = 0; i < NumEffectSubmixes; i++)
            {
                submixes[i] = new EffectSubmix();
            }
        }

        public EffectPatch(byte[] data)
        {
            this.type = (EffectType)data[0];
            this.param1 = data[1];
            this.param2 = data[2];
            this.param3 = data[3];

            int offset = 4;
            submixes = new EffectSubmix[NumEffectSubmixes];
            for (int i = 0; i < NumEffectSubmixes; i++)
            {
                submixes[i] = new EffectSubmix(data[offset], data[offset + 1], data[offset + 2]);
                offset += 3;
            }
        }

        protected override byte[] CollectData()
        {
            List<byte> data = new List<byte>();

            data.Add((byte)this.type);
            data.Add((byte)this.param1);
            data.Add((byte)this.param2);
            data.Add((byte)this.param3);

            // Add six dummy bytes
            data.Add(0);
            data.Add(0);
            data.Add(0);
            data.Add(0);
            data.Add(0);
            data.Add(0);

            for (int i = 0; i < NumEffectSubmixes; i++)
            {
                data.AddRange(this.submixes[i].ToData());
            }

            return data.ToArray();
        }
    }
}
