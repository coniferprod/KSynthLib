using System;
using System.Collections.Generic;
using System.Text;

using SyxPack;

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

        public EffectKind Kind;
        public SmallEffectParameter Param1;
        public SmallEffectParameter Param2;
        public LargeEffectParameter Param3;
        public EffectSubmix[] Submixes;

        public override byte Checksum
        {
            get
            {
                byte[] bs = CollectData().ToArray();
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

        public byte[] OriginalData;

        public EffectPatch()
        {
            Kind = EffectKind.Reverb1;
            Param1 = new SmallEffectParameter();
            Param2 = new SmallEffectParameter();
            Param3 = new LargeEffectParameter();

            Submixes = new EffectSubmix[SubmixCount];
            for (var i = 0; i < SubmixCount; i++)
            {
                Submixes[i] = new EffectSubmix();
            }

            OriginalData = null;
        }

        public EffectPatch(byte[] data) : this()
        {
            Kind = (EffectKind)data[0];
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

            OriginalData = new byte[DataSize];
            Array.Copy(data, OriginalData, DataSize);
        }

        protected override List<byte> CollectData()
        {
            var data = new List<byte>();

            data.Add((byte)Kind);
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

            return data;
        }

        //
        // ISystemExclusiveData implementation
        //

        public List<byte> Data
        {
            get
            {
                var data = new List<byte>();

                data.AddRange(CollectData());

                // Add checksum (gets computed by the property)
                data.Add(Checksum);

                return data;
            }
        }

        public int DataLength => DataSize;

        public override string ToString()
        {
            var builder = new StringBuilder();
            string name = EffectNames[(int)Kind];
            builder.AppendLine($"{name} P1 = {Param1} P2 = {Param2} P3 = {Param3}");

            for (var i = 0; i < SubmixCount; i++)
            {
                var submix = Submixes[i];
                builder.AppendLine($"{i}: pan = {submix.Pan} send1 = {submix.Send1} send2 = {submix.Send2}");
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
