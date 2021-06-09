using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K5000
{
    public enum EnvelopeLoopType
    {
        Off,
        Loop1,
        Loop2
    }

    /// <summary>Represents the MORF harmonic envelope of an additive source.</summary>
    public class MORFHarmonicEnvelope
    {
        private PositiveLevelType _time1;
        public byte Time1
        {
            get => _time1.Value;
            set => _time1.Value = value;
        }

        private PositiveLevelType _time2;
        public byte Time2
        {
            get => _time2.Value;
            set => _time2.Value = value;
        }

        private PositiveLevelType _time3;
        public byte Time3
        {
            get => _time3.Value;
            set => _time3.Value = value;
        }

        private PositiveLevelType _time4;
        public byte Time4
        {
            get => _time4.Value;
            set => _time4.Value = value;
        }
        public EnvelopeLoopType LoopType;

        public MORFHarmonicEnvelope()
        {
            _time1 = new PositiveLevelType();
            _time2 = new PositiveLevelType();
            _time3 = new PositiveLevelType();
            _time4 = new PositiveLevelType();
            LoopType = EnvelopeLoopType.Off;
        }

        public byte[] ToData()
        {
            var data = new List<byte>();
            data.Add(Time1);
            data.Add(Time2);
            data.Add(Time3);
            data.Add(Time4);
            data.Add((byte)LoopType);
            return data.ToArray();
        }
    }

    public class HarmonicCopyParameters
    {
        private PatchNumberType _patchNumber;
        public byte PatchNumber
        {
            get => _patchNumber.Value;
            set => _patchNumber.Value = value;
        }

        public byte SourceNumber; // 0~11 (0~5:soft,6~11:loud)

        public HarmonicCopyParameters()
        {
            _patchNumber = new PatchNumberType();
            SourceNumber = 0;
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();
            data.Add(PatchNumber);
            data.Add(SourceNumber);
            return data.ToArray();
        }
    }

    public enum MORFHarmonicGroup
    {
        Low,
        High
    }

    public class HarmonicParameters
    {
        public bool Morf;  // true if morf on
        public byte TotalGain;

        // Non-MORF parameters
        public MORFHarmonicGroup Group;  // 0 = LO (1~64), 1 = HI (65~128)

        private SignedLevelType _keyScalingToGain; // (-63)1 ... (+63)127
        public sbyte KeyScalingToGain
        {
            get => _keyScalingToGain.Value;
            set => _keyScalingToGain.Value = value;
        }

        public byte BalanceVelocityCurve;
        public byte BalanceVelocityDepth;

        // MORF parameters
        // Harmonic Copy
        public HarmonicCopyParameters Copy1;
        public HarmonicCopyParameters Copy2;
        public HarmonicCopyParameters Copy3;
        public HarmonicCopyParameters Copy4;

        public MORFHarmonicEnvelope MORFEnvelope;

        public HarmonicParameters()
        {
            _keyScalingToGain = new SignedLevelType();
            Copy1 = new HarmonicCopyParameters();
            Copy2 = new HarmonicCopyParameters();
            Copy3 = new HarmonicCopyParameters();
            Copy4 = new HarmonicCopyParameters();
            MORFEnvelope = new MORFHarmonicEnvelope();
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();
            data.Add((byte)(Morf ? 1 : 0));
            data.Add(TotalGain);
            data.Add((byte)Group);
            data.Add((byte)(KeyScalingToGain + 64));
            data.Add(BalanceVelocityCurve);
            data.Add(BalanceVelocityDepth);

            data.AddRange(Copy1.ToData());
            data.AddRange(Copy2.ToData());
            data.AddRange(Copy3.ToData());
            data.AddRange(Copy4.ToData());

            data.AddRange(MORFEnvelope.ToData());

            return data.ToArray();
        }
    }

    public class EnvelopeSegment
    {
        private PositiveLevelType _rate; // 0 ~ 127
        public byte Rate
        {
            get => _rate.Value;
            set => _rate.Value = value;
        }

        private SignedLevelType _level; // (-63)1 ... (+63)127
        public sbyte Level
        {
            get => _level.Value;
            set => _level.Value = value;
        }

        public EnvelopeSegment()
        {
            _rate = new PositiveLevelType();
            _level = new SignedLevelType();
        }
    }

    public class LoopingEnvelope {
        public EnvelopeSegment Attack;
        public EnvelopeSegment Decay1;
        public EnvelopeSegment Decay2;
        public EnvelopeSegment Release;
        public EnvelopeLoopType LoopType;

        public LoopingEnvelope()
        {
            Attack = new EnvelopeSegment();
            Decay1 = new EnvelopeSegment();
            Decay2 = new EnvelopeSegment();
            Release = new EnvelopeSegment();
            LoopType = EnvelopeLoopType.Off;
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();
            data.Add(Attack.Rate);
            data.Add((byte)(Attack.Level + 64));
            data.Add((byte)Decay1.Rate);
            data.Add((byte)(Decay1.Level + 64));
            data.Add((byte)Decay2.Rate);
            data.Add((byte)(Decay2.Level + 64));
            data.Add((byte)Release.Rate);
            data.Add((byte)(Release.Level - 64));
            return data.ToArray();
        }
    }

    public enum FormantLFOShape
    {
        Triangle,
        Sawtooth,
        Random
    }

    public class FormantLFOSettings
    {
        private PositiveLevelType _speed; // 0~126
        public byte Speed
        {
            get => _speed.Value;
            set => _speed.Value = value;
        }

        public FormantLFOShape Shape;  // enumeration

        private UnsignedLevelType _depth;  // 0~63
        public byte Depth
        {
            get => _depth.Value;
            set => _depth.Value = value;
        }

        public FormantLFOSettings()
        {
            _speed = new PositiveLevelType();
            Shape = FormantLFOShape.Sawtooth;
            _depth = new UnsignedLevelType();
        }
    }

    public class FormantParameters
    {
        private SignedLevelType _bias; // (-63)1 ... (+63)127
        public sbyte Bias
        {
            get => _bias.Value;
            set => _bias.Value = value;
        }

        public int EnvLFOSel;  // 0 = ENV, 1 = LFO

        private SignedLevelType _envelopeDepth; // (-63)1 ... (+63)127
        public sbyte EnvelopeDepth
        {
            get => _envelopeDepth.Value;
            set => _envelopeDepth.Value = value;
        }

        public LoopingEnvelope Envelope;

        private SignedLevelType _velocitySensitivityEnvelopeDepth; // (-63)1 ... (+63)127
        public sbyte VelocitySensitivityEnvelopeDepth
        {
            get => _velocitySensitivityEnvelopeDepth.Value;
            set => _velocitySensitivityEnvelopeDepth.Value = value;
        }

        private SignedLevelType _keyScalingEnvelopeDepth;  // (-63)1 ... (+63)127
        public sbyte KeyScalingEnvelopeDepth
        {
            get => _keyScalingEnvelopeDepth.Value;
            set => _keyScalingEnvelopeDepth.Value = value;
        }

        public FormantLFOSettings LFO;

        public FormantParameters()
        {
            _bias = new SignedLevelType();
            _envelopeDepth = new SignedLevelType();
            Envelope = new LoopingEnvelope();
            _velocitySensitivityEnvelopeDepth = new SignedLevelType();
            _keyScalingEnvelopeDepth = new SignedLevelType();
            LFO = new FormantLFOSettings();
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();
            data.Add((byte)(Bias + 64));
            data.Add((byte)EnvLFOSel);
            data.Add((byte)(EnvelopeDepth + 64));

            data.AddRange(Envelope.ToData());

            data.Add((byte)(VelocitySensitivityEnvelopeDepth + 64));
            data.Add((byte)(KeyScalingEnvelopeDepth + 64));

            data.Add(LFO.Speed);
            data.Add((byte)LFO.Shape);
            data.Add(LFO.Depth);

            return data.ToArray();
        }
    }

    public class HarmonicEnvelope
    {
        public EnvelopeSegment Segment0;
        public EnvelopeSegment Segment1;
        public bool Segment1Loop;
        public EnvelopeSegment Segment2;
        public bool Segment2Loop;
        public EnvelopeSegment Segment3;

        public HarmonicEnvelope()
        {
            Segment0 = new EnvelopeSegment();
            Segment1 = new EnvelopeSegment();
            Segment1Loop = false;
            Segment2 = new EnvelopeSegment();
            Segment2Loop = false;
            Segment3 = new EnvelopeSegment();
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();

            data.Add((byte)Segment0.Rate);
            data.Add((byte)Segment0.Level);

            data.Add((byte)Segment1.Rate);
            byte s1Level = (byte)Segment1.Level;
            if (Segment1Loop)
            {
                s1Level.SetBit(6);
            }
            data.Add(s1Level);

            data.Add((byte)Segment2.Rate);
            byte s2Level = (byte)Segment2.Level;
            if (Segment2Loop)
            {
                s2Level.SetBit(6);
            }
            data.Add(s2Level);

            data.Add((byte)Segment3.Rate);
            data.Add((byte)Segment3.Level);

            return data.ToArray();
        }
    }

    public class AdditiveKit
    {
        public const int WaveNumber = 512;  // if the wave number is 512, the source is ADD
        public const int DataSize = 806;
        public const int HarmonicCount = 64;
        public const int FilterBandCount = 128;

        public byte CheckSum;  // [(HCKIT sum)  + (HCcode1 sum) + (HCcode2 sum) + (FF sum) + (HCenv sum) + (loud sense select) + 0xa5] & 0x7f

        public HarmonicParameters Harmonics;
        public FormantParameters Formant;
        public byte[] SoftHarmonics;
        public byte[] LoudHarmonics;
        public byte[] FormantFilter;
        public HarmonicEnvelope[] HarmonicEnvelopes;

        public AdditiveKit()
        {
            Harmonics = new HarmonicParameters();
            Formant = new FormantParameters();

            SoftHarmonics = new byte[HarmonicCount];
            LoudHarmonics = new byte[HarmonicCount];
            FormantFilter = new byte[FilterBandCount];

            HarmonicEnvelopes = new HarmonicEnvelope[HarmonicCount];
            for (int i = 0; i < HarmonicCount; i++)
            {
                HarmonicEnvelopes[i] = new HarmonicEnvelope();
            }
        }

        public AdditiveKit(byte[] data)
        {
            int offset = 0;
            byte b = 0;  // will be reused when getting the next byte

            (b, offset) = Util.GetNextByte(data, offset);
            CheckSum = b;

            Harmonics = new HarmonicParameters();

            (b, offset) = Util.GetNextByte(data, offset);
            Harmonics.Morf = (b == 1);

            (b, offset) = Util.GetNextByte(data, offset);
            Harmonics.TotalGain = b;

            (b, offset) = Util.GetNextByte(data, offset);
            // value of this byte should be 0 or 1
            Harmonics.Group = (MORFHarmonicGroup)b;

            (b, offset) = Util.GetNextByte(data, offset);
            Harmonics.KeyScalingToGain = (sbyte)(b - 64);

            (b, offset) = Util.GetNextByte(data, offset);
            Harmonics.BalanceVelocityCurve = b;

            (b, offset) = Util.GetNextByte(data, offset);
            Harmonics.BalanceVelocityDepth = b;

            Harmonics.Copy1 = new HarmonicCopyParameters();
            (b, offset) = Util.GetNextByte(data, offset);
            Harmonics.Copy1.PatchNumber = b;
            (b, offset) = Util.GetNextByte(data, offset);
            Harmonics.Copy1.SourceNumber = b;

            Harmonics.Copy2 = new HarmonicCopyParameters();
            (b, offset) = Util.GetNextByte(data, offset);
            Harmonics.Copy2.PatchNumber = b;
            (b, offset) = Util.GetNextByte(data, offset);
            Harmonics.Copy2.SourceNumber = b;

            Harmonics.Copy3 = new HarmonicCopyParameters();
            (b, offset) = Util.GetNextByte(data, offset);
            Harmonics.Copy3.PatchNumber = b;
            (b, offset) = Util.GetNextByte(data, offset);
            Harmonics.Copy3.SourceNumber = b;

            Harmonics.Copy4 = new HarmonicCopyParameters();
            (b, offset) = Util.GetNextByte(data, offset);
            Harmonics.Copy4.PatchNumber = b;
            (b, offset) = Util.GetNextByte(data, offset);
            Harmonics.Copy4.SourceNumber = b;

            Harmonics.MORFEnvelope = new MORFHarmonicEnvelope();
            (b, offset) = Util.GetNextByte(data, offset);
            Harmonics.MORFEnvelope.Time1 = b;
            (b, offset) = Util.GetNextByte(data, offset);
            Harmonics.MORFEnvelope.Time2 = b;
            (b, offset) = Util.GetNextByte(data, offset);
            Harmonics.MORFEnvelope.Time3 = b;
            (b, offset) = Util.GetNextByte(data, offset);
            Harmonics.MORFEnvelope.Time4 = b;
            (b, offset) = Util.GetNextByte(data, offset);
            Harmonics.MORFEnvelope.LoopType = (EnvelopeLoopType)b;

            Formant = new FormantParameters();
            (b, offset) = Util.GetNextByte(data, offset);
            Formant.Bias = (sbyte)(b - 64);
            (b, offset) = Util.GetNextByte(data, offset);
            Formant.EnvLFOSel = b;
            (b, offset) = Util.GetNextByte(data, offset);
            Formant.EnvelopeDepth = (sbyte)(b - 64);

            LoopingEnvelope formantEnvelope = new LoopingEnvelope();
            (b, offset) = Util.GetNextByte(data, offset);
            formantEnvelope.Attack.Rate = b;
            (b, offset) = Util.GetNextByte(data, offset);
            formantEnvelope.Attack.Level = (sbyte)(b - 64);
            (b, offset) = Util.GetNextByte(data, offset);
            formantEnvelope.Decay1.Rate = b;
            (b, offset) = Util.GetNextByte(data, offset);
            formantEnvelope.Decay1.Level = (sbyte)(b - 64);
            (b, offset) = Util.GetNextByte(data, offset);
            formantEnvelope.Decay2.Rate = b;
            (b, offset) = Util.GetNextByte(data, offset);
            formantEnvelope.Decay2.Level = (sbyte)(b - 64);
            (b, offset) = Util.GetNextByte(data, offset);
            formantEnvelope.Release.Rate = b;
            (b, offset) = Util.GetNextByte(data, offset);
            formantEnvelope.Release.Level = (sbyte)(b - 64);
            (b, offset) = Util.GetNextByte(data, offset);
            formantEnvelope.LoopType = (EnvelopeLoopType)b;

            Formant.Envelope = formantEnvelope;

            (b, offset) = Util.GetNextByte(data, offset);
            Formant.VelocitySensitivityEnvelopeDepth = (sbyte)(b - 64);
            (b, offset) = Util.GetNextByte(data, offset);
            Formant.KeyScalingEnvelopeDepth = (sbyte)(b - 64);

            (b, offset) = Util.GetNextByte(data, offset);
            Formant.LFO.Speed = b;
            (b, offset) = Util.GetNextByte(data, offset);
            Formant.LFO.Shape = (FormantLFOShape)b;
            (b, offset) = Util.GetNextByte(data, offset);
            Formant.LFO.Depth = b;

            SoftHarmonics = new byte[HarmonicCount];
            for (int i = 0; i < HarmonicCount; i++)
            {
                (b, offset) = Util.GetNextByte(data, offset);
                SoftHarmonics[i] = b;
            }

            LoudHarmonics = new byte[HarmonicCount];
            for (int i = 0; i < HarmonicCount; i++)
            {
                (b, offset) = Util.GetNextByte(data, offset);
                LoudHarmonics[i] = b;
            }

            // TODO: Maybe the filter bands could be combined with the formant parameters?
            FormantFilter = new byte[FilterBandCount];
            for (int i = 0; i < FilterBandCount; i++)
            {
                (b, offset) = Util.GetNextByte(data, offset);
                FormantFilter[i] = b;
            }

            HarmonicEnvelopes = new HarmonicEnvelope[HarmonicCount];
            for (int i = 0; i < HarmonicCount; i++)
            {
                HarmonicEnvelopes[i] = new HarmonicEnvelope();
                (b, offset) = Util.GetNextByte(data, offset);
                HarmonicEnvelopes[i].Segment0.Rate = b;
                (b, offset) = Util.GetNextByte(data, offset);
                HarmonicEnvelopes[i].Segment0.Level = (sbyte)b;

                (b, offset) = Util.GetNextByte(data, offset);
                HarmonicEnvelopes[i].Segment1.Rate = b;
                (b, offset) = Util.GetNextByte(data, offset);
                HarmonicEnvelopes[i].Segment1Loop = b.IsBitSet(6);
                HarmonicEnvelopes[i].Segment1.Level = (sbyte)(b & 0x3F);  // bottom 6 bits = 0~63

                (b, offset) = Util.GetNextByte(data, offset);
                HarmonicEnvelopes[i].Segment2.Rate = b;
                (b, offset) = Util.GetNextByte(data, offset);
                HarmonicEnvelopes[i].Segment2Loop = b.IsBitSet(6);
                HarmonicEnvelopes[i].Segment2.Level = (sbyte)(b & 0x3F);

                (b, offset) = Util.GetNextByte(data, offset);
                HarmonicEnvelopes[i].Segment3.Rate = b;  // 0~127
                (b, offset) = Util.GetNextByte(data, offset);
                HarmonicEnvelopes[i].Segment3.Level = (sbyte)b;  // 0~63
            }

            (b, offset) = Util.GetNextByte(data, offset);  // 806 dummy
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.Append(string.Format("MORF is {0}\n", Harmonics.Morf ? "ON" : "OFF"));
            b.Append($"Total Gain = {Harmonics.TotalGain}\n");
            b.Append($"Harm group = {Harmonics.Group}\n");
            b.Append("Soft harmonics:\n");
            for (int i = 0; i < HarmonicCount; i++)
            {
                b.Append($"{i}: {SoftHarmonics[i]}\n");
            }
            b.Append("Loud harmonics:\n");
            for (int i = 0; i < HarmonicCount; i++)
            {
                b.Append($"{i}: {LoudHarmonics[i]}\n");
            }

            b.Append("Harmonic envelopes:\n    Atk  Dc1  Dc2  Rls\n");
            for (int i = 0; i < HarmonicCount; i++)
            {
                HarmonicEnvelope env = HarmonicEnvelopes[i];
                b.Append(string.Format("{0}: Level {1}  {2}  {3}  {4}\n", i + 1, env.Segment0.Level, env.Segment1.Level, env.Segment2.Level, env.Segment3.Level));
                b.Append(string.Format("   Rate  {0}  {1}  {2}  {3}\n", env.Segment0.Rate, env.Segment1.Rate, env.Segment2.Rate, env.Segment3.Rate));
            }

            return b.ToString();
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();

            data.AddRange(Harmonics.ToData());
            data.AddRange(Formant.ToData());

            foreach (byte b in SoftHarmonics)
            {
                data.Add(b);
            }
            foreach (byte b in LoudHarmonics)
            {
                data.Add(b);
            }
            foreach (byte b in FormantFilter)
            {
                data.Add(b);
            }

            for (int i = 0; i < HarmonicCount; i++)
            {
                data.AddRange(HarmonicEnvelopes[i].ToData());
            }

            byte[] allData = data.ToArray();
            byte checkSum = ComputeCheckSum(allData);
            data.Insert(0, checkSum);

            data.Add(0);  // 806 dummy

            return data.ToArray();
        }

        private byte ComputeCheckSum(byte[] data)
        {
            // check sum = [(HCKIT sum) + (HCcode1 sum) + (HCcode2 sum) + (FF sum) + (HCenv sum) + (loud sens select) + 0xA5] & 0x7f
            byte total = 0;

            // TODO: Should these be calculated separately and then added to the total?

            // HCKIT sum = MORF flag, harmonics, formant
            total += (byte)(Harmonics.Morf ? 1 : 0);
            byte[] harmonicsData = Harmonics.ToData();
            foreach (byte b in harmonicsData)
            {
                total += b;
            }

            byte[] formantParameterData = Formant.ToData();
            foreach (byte b in formantParameterData)
            {
                total += b;
            }

            // HC code 1 sum
            foreach (byte b in SoftHarmonics)
            {
                total += b;
            }

            // HC code 2 sum
            foreach (byte b in LoudHarmonics)
            {
                total += b;
            }

            // FF sum
            foreach (byte b in FormantFilter)
            {
                total += b;
            }

            for (int i = 0; i < HarmonicCount; i++)
            {
                byte[] harmonicEnvelopeData = HarmonicEnvelopes[i].ToData();
                foreach (byte b in harmonicEnvelopeData)
                {
                    total += b;
                }
            }

            // TODO: What even *is* the "loud sense select" or "LS select"?

            total += 0xA5;

            return (byte)(total & 0x7f);
        }
    }
}
