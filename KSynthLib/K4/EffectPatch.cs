using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

using KSynthLib.Common;


namespace KSynthLib.K4
{
    public enum EffectKind
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
        [Range(-7, 7, ErrorMessage = "{0} must be between {1} and {2}")]
        public int Pan;

        [Range(0, 100, ErrorMessage = "{0} must be between {1} and {2}")]
        public int Send1;

        [Range(0, 100, ErrorMessage = "{0} must be between {1} and {2}")]
        public int Send2;

        public EffectSubmix()
        {
            Pan = 0;
            Send1 = 0;
            Send2 = 0;
        }

        public EffectSubmix(int d0, int d1, int d2)
        {
            Pan = d0;
            Send1 = d1;
            Send2 = d2;
        }

        public byte[] ToData()
        {
            var data = new List<byte>();

            data.Add(ByteConverter.ByteFromPan(Pan));
            data.Add((byte)Send1);
            data.Add((byte)Send2);

            return data.ToArray();
        }
    }

    public class EffectPatch: Patch, ISystemExclusiveData
    {
        public const int DataSize = 35;
        public const int SubmixCount = 8;

        public EffectKind Kind;

        [Range(0, 7, ErrorMessage = "{0} must be between {1} and {2}")]
        public int Param1;

        [Range(0, 7, ErrorMessage = "{0} must be between {1} and {2}")]
        public int Param2;

        [Range(0, 31, ErrorMessage = "{0} must be between {1} and {2}")]
        public int Param3;

        public EffectSubmix[] Submixes;

        private byte _checksum;
        public override byte Checksum
        {
            get
            {
                byte[] bs = CollectData();
                byte sum = 0;
                foreach (var b in bs)
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
            Kind = EffectKind.Reverb1;
            Param1 = 0;
            Param2 = 0;
            Param3 = 0;

            Submixes = new EffectSubmix[SubmixCount];
            for (var i = 0; i < SubmixCount; i++)
            {
                Submixes[i] = new EffectSubmix();
            }
        }

        public EffectPatch(byte[] data) : this()
        {
            Kind = (EffectKind)data[0];
            Param1 = data[1];
            Param2 = data[2];
            Param3 = data[3];

            var offset = 4;
            Submixes = new EffectSubmix[SubmixCount];
            for (var i = 0; i < SubmixCount; i++)
            {
                Submixes[i] = new EffectSubmix(
                    (sbyte)(data[offset] - 7),  // 0~15/0~±7 (K4); 0~15/0~±7, 16~21/I1~I6 (K4r)
                    data[offset + 1],
                    data[offset + 2]
                );
                offset += 3;
            }
        }

        protected override byte[] CollectData()
        {
            var data = new List<byte>();

            data.Add((byte)Kind);
            data.Add((byte)Param1);
            data.Add((byte)Param2);
            data.Add((byte)Param3);

            // Add six dummy bytes
            data.Add(0);
            data.Add(0);
            data.Add(0);
            data.Add(0);
            data.Add(0);
            data.Add(0);

            for (var i = 0; i < SubmixCount; i++)
            {
                data.AddRange(this.Submixes[i].ToData());
            }

            return data.ToArray();
        }

        public override byte[] ToData()
        {
            var data = new List<byte>();
            data.AddRange(CollectData());

            // Add checksum (gets computed by the property)
            data.Add(Checksum);

            return data.ToArray();
        }

        public List<byte> GetSystemExclusiveData()
        {
            var data = new List<byte>();

            data.AddRange(CollectData());

            // Add checksum (gets computed by the property)
            data.Add(Checksum);

            return data;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            string name = EffectNames[(int)Kind];
            builder.Append($"{name} P1 = {Param1} P2 = {Param2} P3 = {Param3}\n");

            for (var i = 0; i < SubmixCount; i++)
            {
                var submix = Submixes[i];
                builder.Append($"{i}: pan = {submix.Pan} send1 = {submix.Send1} send2 = {submix.Send2}\n");
            }

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
