using System.Collections.Generic;
using System.Text;

using KSynthLib.Common;


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
        public PanValue Pan;
        public Level Send1;
        public Level Send2;

        public EffectSubmix()
        {
            Pan = new PanValue();
            Send1 = new Level();
            Send2 = new Level();
        }

        public EffectSubmix(int d0, int d1, int d2)
        {
            Pan = new PanValue(d0);
            Send1 = new Level(d1);
            Send2 = new Level(d2);
        }

        public byte[] ToData()
        {
            var data = new List<byte>();
            data.Add(Pan.ToByte());
            data.Add(Send1.ToByte());
            data.Add(Send2.ToByte());
            return data.ToArray();
        }
    }

    public class EffectPatch: Patch, ISystemExclusiveData
    {
        public const int DataSize = 35;
        public const int SubmixCount = 8;

        public EffectType Type;
        public SmallEffectParameter Param1;
        public SmallEffectParameter Param2;
        public LargeEffectParameter Param3;
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
            Type = EffectType.Reverb1;
            Param1 = new SmallEffectParameter();
            Param2 = new SmallEffectParameter();
            Param3 = new LargeEffectParameter();

            Submixes = new EffectSubmix[SubmixCount];
            for (var i = 0; i < SubmixCount; i++)
            {
                Submixes[i] = new EffectSubmix();
            }
        }

        public EffectPatch(byte[] data) : this()
        {
            Type = (EffectType)data[0];
            Param1 = new SmallEffectParameter(data[1]);
            Param2 = new SmallEffectParameter(data[2]);
            Param3 = new LargeEffectParameter(data[3]);

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

            data.Add((byte)this.Type);
            data.Add(Param1.ToByte());
            data.Add(Param2.ToByte());
            data.Add(Param3.ToByte());

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
            string name = EffectNames[(int)Type];
            builder.Append($"{name} P1 = {Param1.Value} P2 = {Param2.Value} P3 = {Param3.Value}\n");

            for (var i = 0; i < SubmixCount; i++)
            {
                var submix = Submixes[i];
                builder.Append($"{i}: pan = {submix.Pan.Value} send1 = {submix.Send1.Value} send2 = {submix.Send2.Value}\n");
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
