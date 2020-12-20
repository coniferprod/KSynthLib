using System;
using System.Collections.Generic;
using System.Text;

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
        private PanValueType _pan;
        public sbyte Pan
        {
            get => _pan.Value;
            set => _pan.Value = value;
        }

        private SendValueType _send1;
        public byte Send1
        {
            get => _send1.Value;
            set => _send1.Value = value;
        }

        private SendValueType _send2;
        public byte Send2
        {
            get => _send2.Value;
            set => _send2.Value = value;
        }

        public EffectSubmix()
        {
            _pan = new PanValueType();
            _send1 = new SendValueType();
            _send2 = new SendValueType();
        }

        public EffectSubmix(sbyte d0, byte d1, byte d2) : this()
        {
            Pan = d0;
            Send1 = d1;
            Send2 = d2;
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();
            data.Add((byte)Pan);
            data.Add((byte)Send1);
            data.Add((byte)Send2);
            return data.ToArray();
        }
    }

    public class EffectPatch
    {
        public const int DataSize = 35;

        public const int SubmixCount = 8;

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

        public EffectSubmix[] Submixes;

        private byte _checksum;
        public byte Checksum
        {
            get
            {
                byte[] bs = CollectData();
                byte sum = 0;
                foreach (byte b in bs)
                {
                    sum += b;
                }
                sum += 0xA5;
                return sum;
            }

            set => _checksum = value;
        }

        public EffectPatch()
        {
            Type = EffectType.Reverb1;
            _param1 = new EffectParameter1Type();
            _param2 = new EffectParameter1Type();
            _param3 = new EffectParameter3Type();

            Submixes = new EffectSubmix[SubmixCount];
            for (int i = 0; i < SubmixCount; i++)
            {
                Submixes[i] = new EffectSubmix();
            }
        }

        public EffectPatch(byte[] data) : this()
        {
            Type = (EffectType)data[0];
            _param1 = new EffectParameter1Type(data[1]);
            _param2 = new EffectParameter1Type(data[2]);
            _param3 = new EffectParameter3Type(data[3]);

            int offset = 4;
            Submixes = new EffectSubmix[SubmixCount];
            for (int i = 0; i < SubmixCount; i++)
            {
                Submixes[i] = new EffectSubmix(
                    (sbyte)(data[offset] - 7),  // 0~15/0~±7 (K4); 0~15/0~±7, 16~21/I1~I6 (K4r)
                    data[offset + 1],
                    data[offset + 2]
                );
                offset += 3;
            }
        }

        protected byte[] CollectData()
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

            for (int i = 0; i < SubmixCount; i++)
            {
                data.AddRange(this.Submixes[i].ToData());
            }

            return data.ToArray();
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(CollectData());

            // Add checksum (gets computed by the property)
            data.Add(Checksum);

            return data.ToArray();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            string name = EffectNames[(int)Type];
            builder.Append($"{name} P1 = {Param1} P2 = {Param2} P3 = {Param3}\n");

            /*
            for (int i = 0; i < SubmixCount; i++)
            {
                EffectSubmix submix = Submixes[i];
                builder.Append($"{i}: pan = {submix.Pan} send1 = {submix.Send1} send2 = {submix.Send2}\n");
            }
            */
            
            return builder.ToString();
        }

        public static string[] EffectNames =
        {
            "Reverb 1",
            "Reverb 2",
            "Reverb 3",
            "Reverb 4",
            "Gate Reverb",
            "Reverse Gate",
            "Normal Delay",
            "Stereo Panpot Delay",
            "Chorus",
            "Overdrive + Flanger",
            "Overdrive + Normal Delay",
            "Overdrive + Reverb",
            "Normal Delay + Normal Delay",
            "Normal Delay + Stereo Pan Delay",
            "Chorus + Normal Delay",
            "Chorus + Stereo Pan Delay"
        };
    }
}
