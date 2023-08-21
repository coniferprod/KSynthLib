using System.Text;
using System.Collections.Generic;

using SyxPack;
using KSynthLib.Common;

namespace KSynthLib.K5000
{
    public class HarmonicCopyParameters: ISystemExclusiveData
    {
        public PatchNumber PatchNumber;
        public byte SourceNumber; // 0~11 (0~5:soft,6~11:loud)

        public HarmonicCopyParameters()
        {
            PatchNumber = new PatchNumber();
            SourceNumber = 0;
        }

        //
        // ISystemExclusiveData implementation
        //

        public List<byte> Data
        {
            get
            {
                return new List<byte>()
                {
                    PatchNumber.ToByte(),
                    SourceNumber
                };
            }
        }

        public int DataLength => 2;
    }

    public enum MORFHarmonicGroup
    {
        Low,
        High
    }

    public class HarmonicParameters: ISystemExclusiveData
    {
        public bool Morf;  // true if morf on
        public byte TotalGain;

        // Non-MORF parameters
        public MORFHarmonicGroup Group;  // 0 = LO (1~64), 1 = HI (65~128)

        public SignedLevel KeyScalingToGain; // (-63)1 ... (+63)127
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
            KeyScalingToGain = new SignedLevel();
            Copy1 = new HarmonicCopyParameters();
            Copy2 = new HarmonicCopyParameters();
            Copy3 = new HarmonicCopyParameters();
            Copy4 = new HarmonicCopyParameters();
            MORFEnvelope = new MORFHarmonicEnvelope();
        }

        //
        // ISystemExclusiveData implementation
        //

        public List<byte> Data
        {
            get
            {
                var data = new List<byte>();

                data.AddRange(new List<byte>() {
                    (byte)(Morf ? 1 : 0),
                    TotalGain,
                    (byte)Group,
                    KeyScalingToGain.ToByte(),
                    BalanceVelocityCurve,
                    BalanceVelocityDepth
                });

                data.AddRange(Copy1.Data);
                data.AddRange(Copy2.Data);
                data.AddRange(Copy3.Data);
                data.AddRange(Copy4.Data);

                data.AddRange(MORFEnvelope.Data);

                return data;
            }
        }

        public int DataLength
        {
            get
            {
                return
                    6 +
                    Copy1.DataLength + Copy2.DataLength +
                    Copy3.DataLength + Copy4.DataLength +
                    MORFEnvelope.DataLength;
            }
        }
    }

    public enum FormantLFOShape
    {
        Triangle,
        Sawtooth,
        Random
    }

    public class FormantLFOSettings: ISystemExclusiveData
    {
        public PositiveLevel Speed; // 0~127
        public FormantLFOShape Shape;  // enumeration
        public UnsignedLevel Depth;  // 0~63

        public FormantLFOSettings()
        {
            Speed = new PositiveLevel();
            Shape = FormantLFOShape.Sawtooth;
            Depth = new UnsignedLevel();
        }

        public FormantLFOSettings(List<byte> data)
        {
            Speed = new PositiveLevel(data[0]);
            Shape = (FormantLFOShape)data[1];
            Depth = new UnsignedLevel(data[2]);
        }

        //
        // ISystemExclusiveData implementation
        //

        public List<byte> Data
        {
            get
            {
                return new List<byte>()
                {
                    Speed.ToByte(),
                    (byte)Shape,
                    Depth.ToByte()
                };
            }
        }

        public int DataLength => 3;
    }

    public class FormantParameters
    {
        public SignedLevel Bias; // (-63)1 ... (+63)127
        public byte EnvLFOSel;  // 0 = ENV, 1 = LFO
        public SignedLevel EnvelopeDepth; // (-63)1 ... (+63)127
        public LoopingEnvelope Envelope;
        public SignedLevel VelocitySensitivityEnvelopeDepth; // (-63)1 ... (+63)127
        public SignedLevel KeyScalingEnvelopeDepth;  // (-63)1 ... (+63)127
        public FormantLFOSettings LFO;

        public FormantParameters()
        {
            Bias = new SignedLevel();
            EnvelopeDepth = new SignedLevel();
            Envelope = new LoopingEnvelope();
            EnvLFOSel = 0;
            VelocitySensitivityEnvelopeDepth = new SignedLevel();
            KeyScalingEnvelopeDepth = new SignedLevel();
            LFO = new FormantLFOSettings();
        }

        public FormantParameters(List<byte> data)
        {
            Bias = new SignedLevel(data[0]);
            EnvelopeDepth = new SignedLevel(data[1]);
            Envelope = new LoopingEnvelope(data.GetRange(2, 9));
            EnvLFOSel = data[11];
            VelocitySensitivityEnvelopeDepth = new SignedLevel(data[12]);
            KeyScalingEnvelopeDepth = new SignedLevel(data[13]);
            LFO = new FormantLFOSettings(data.GetRange(14, 3));
        }

        //
        // ISystemExclusiveData implementation
        //

        public List<byte> Data
        {
            get
            {
                var data = new List<byte>();

                data.Add(Bias.ToByte());
                data.Add(EnvLFOSel);
                data.Add(EnvelopeDepth.ToByte());

                data.AddRange(Envelope.Data);

                data.Add(VelocitySensitivityEnvelopeDepth.ToByte());
                data.Add(KeyScalingEnvelopeDepth.ToByte());

                data.AddRange(LFO.Data);

                return data;
            }
        }

        public int DataLength => 3;
    }

    public class AdditiveKit: ISystemExclusiveData
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
            for (var i = 0; i < HarmonicCount; i++)
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
            Harmonics.KeyScalingToGain = new SignedLevel(b);

            (b, offset) = Util.GetNextByte(data, offset);
            Harmonics.BalanceVelocityCurve = b;

            (b, offset) = Util.GetNextByte(data, offset);
            Harmonics.BalanceVelocityDepth = b;

            Harmonics.Copy1 = new HarmonicCopyParameters();
            (b, offset) = Util.GetNextByte(data, offset);
            Harmonics.Copy1.PatchNumber = new PatchNumber(b);
            (b, offset) = Util.GetNextByte(data, offset);
            Harmonics.Copy1.SourceNumber = b;

            Harmonics.Copy2 = new HarmonicCopyParameters();
            (b, offset) = Util.GetNextByte(data, offset);
            Harmonics.Copy2.PatchNumber = new PatchNumber(b);
            (b, offset) = Util.GetNextByte(data, offset);
            Harmonics.Copy2.SourceNumber = b;

            Harmonics.Copy3 = new HarmonicCopyParameters();
            (b, offset) = Util.GetNextByte(data, offset);
            Harmonics.Copy3.PatchNumber = new PatchNumber(b);
            (b, offset) = Util.GetNextByte(data, offset);
            Harmonics.Copy3.SourceNumber = b;

            Harmonics.Copy4 = new HarmonicCopyParameters();
            (b, offset) = Util.GetNextByte(data, offset);
            Harmonics.Copy4.PatchNumber = new PatchNumber(b);
            (b, offset) = Util.GetNextByte(data, offset);
            Harmonics.Copy4.SourceNumber = b;

            Harmonics.MORFEnvelope = new MORFHarmonicEnvelope();
            (b, offset) = Util.GetNextByte(data, offset);
            Harmonics.MORFEnvelope.Time1 = new PositiveLevel(b);
            (b, offset) = Util.GetNextByte(data, offset);
            Harmonics.MORFEnvelope.Time2 = new PositiveLevel(b);
            (b, offset) = Util.GetNextByte(data, offset);
            Harmonics.MORFEnvelope.Time3 = new PositiveLevel(b);
            (b, offset) = Util.GetNextByte(data, offset);
            Harmonics.MORFEnvelope.Time4 = new PositiveLevel(b);
            (b, offset) = Util.GetNextByte(data, offset);
            Harmonics.MORFEnvelope.LoopKind = (EnvelopeLoopKind)b;

            Formant = new FormantParameters();
            (b, offset) = Util.GetNextByte(data, offset);
            Formant.Bias = new SignedLevel(b);
            (b, offset) = Util.GetNextByte(data, offset);
            Formant.EnvLFOSel = b;
            (b, offset) = Util.GetNextByte(data, offset);
            Formant.EnvelopeDepth = new SignedLevel(b);

            List<byte> formantEnvelopeBytes = new List<byte>();
            (b, offset) = Util.GetNextByte(data, offset);
            formantEnvelopeBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            formantEnvelopeBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            formantEnvelopeBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            formantEnvelopeBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            formantEnvelopeBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            formantEnvelopeBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            formantEnvelopeBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            formantEnvelopeBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            formantEnvelopeBytes.Add(b);
            Formant.Envelope = new LoopingEnvelope(formantEnvelopeBytes);

            (b, offset) = Util.GetNextByte(data, offset);
            Formant.VelocitySensitivityEnvelopeDepth = new SignedLevel(b);
            (b, offset) = Util.GetNextByte(data, offset);
            Formant.KeyScalingEnvelopeDepth = new SignedLevel(b);

            (b, offset) = Util.GetNextByte(data, offset);
            Formant.LFO.Speed = new PositiveLevel(b);
            (b, offset) = Util.GetNextByte(data, offset);
            Formant.LFO.Shape = (FormantLFOShape)b;
            (b, offset) = Util.GetNextByte(data, offset);
            Formant.LFO.Depth = new UnsignedLevel(b);

            SoftHarmonics = new byte[HarmonicCount];
            for (var i = 0; i < HarmonicCount; i++)
            {
                (b, offset) = Util.GetNextByte(data, offset);
                SoftHarmonics[i] = b;
            }

            LoudHarmonics = new byte[HarmonicCount];
            for (var i = 0; i < HarmonicCount; i++)
            {
                (b, offset) = Util.GetNextByte(data, offset);
                LoudHarmonics[i] = b;
            }

            // TODO: Maybe the filter bands could be combined with the formant parameters?
            FormantFilter = new byte[FilterBandCount];
            for (var i = 0; i < FilterBandCount; i++)
            {
                (b, offset) = Util.GetNextByte(data, offset);
                FormantFilter[i] = b;
            }

            HarmonicEnvelopes = new HarmonicEnvelope[HarmonicCount];
            for (var i = 0; i < HarmonicCount; i++)
            {
                HarmonicEnvelopes[i] = new HarmonicEnvelope();
                (b, offset) = Util.GetNextByte(data, offset);
                HarmonicEnvelopes[i].Segment0.Rate = new PositiveLevel(b);
                (b, offset) = Util.GetNextByte(data, offset);
                HarmonicEnvelopes[i].Segment0.Level = new SignedLevel(b);

                (b, offset) = Util.GetNextByte(data, offset);
                HarmonicEnvelopes[i].Segment1.Rate = new PositiveLevel(b);
                (b, offset) = Util.GetNextByte(data, offset);
                HarmonicEnvelopes[i].Segment1Loop = b.IsBitSet(6);
                HarmonicEnvelopes[i].Segment1.Level = new SignedLevel(b & 0x3F);  // bottom 6 bits = 0~63

                (b, offset) = Util.GetNextByte(data, offset);
                HarmonicEnvelopes[i].Segment2.Rate = new PositiveLevel(b);
                (b, offset) = Util.GetNextByte(data, offset);
                HarmonicEnvelopes[i].Segment2Loop = b.IsBitSet(6);
                HarmonicEnvelopes[i].Segment2.Level = new SignedLevel(b & 0x3F);

                (b, offset) = Util.GetNextByte(data, offset);
                HarmonicEnvelopes[i].Segment3.Rate = new PositiveLevel(b);  // 0~127
                (b, offset) = Util.GetNextByte(data, offset);
                HarmonicEnvelopes[i].Segment3.Level = new SignedLevel(b);  // 0~63
            }

            (b, offset) = Util.GetNextByte(data, offset);  // 806 dummy
        }

        public override string ToString()
        {
            var b = new StringBuilder();
            b.Append(string.Format("MORF is {0}\n", Harmonics.Morf ? "ON" : "OFF"));
            b.Append($"Total Gain = {Harmonics.TotalGain}\n");
            b.Append($"Harm group = {Harmonics.Group}\n");
            b.Append("Soft harmonics:\n");
            for (var i = 0; i < HarmonicCount; i++)
            {
                b.Append($"{i}: {SoftHarmonics[i]}\n");
            }
            b.Append("Loud harmonics:\n");
            for (var i = 0; i < HarmonicCount; i++)
            {
                b.Append($"{i}: {LoudHarmonics[i]}\n");
            }

            b.Append("Harmonic envelopes:\n    Atk  Dc1  Dc2  Rls\n");
            for (var i = 0; i < HarmonicCount; i++)
            {
                HarmonicEnvelope env = HarmonicEnvelopes[i];
                b.Append(string.Format("{0}: Level {1}  {2}  {3}  {4}\n", i + 1, env.Segment0.Level.Value, env.Segment1.Level.Value, env.Segment2.Level.Value, env.Segment3.Level.Value));
                b.Append(string.Format("   Rate  {0}  {1}  {2}  {3}\n", env.Segment0.Rate.Value, env.Segment1.Rate.Value, env.Segment2.Rate.Value, env.Segment3.Rate.Value));
            }

            return b.ToString();
        }

        //
        // ISystemExclusiveData implementation
        //

        public List<byte> Data
        {
            get
            {
                var data = new List<byte>();

                data.AddRange(Harmonics.Data);
                data.AddRange(Formant.Data);
                data.AddRange(SoftHarmonics);
                data.AddRange(LoudHarmonics);
                data.AddRange(FormantFilter);

                for (var i = 0; i < HarmonicCount; i++)
                {
                    data.AddRange(HarmonicEnvelopes[i].Data);
                }

                byte checksum = ComputeChecksum(data);
                data.Insert(0, checksum);  // goes to the front of the buffer

                data.Add(0);  // 806 dummy

                return data;
            }
        }

        public int DataLength => DataSize;

        private byte ComputeChecksum(List<byte> data)
        {
            // check sum = [(HCKIT sum) + (HCcode1 sum) + (HCcode2 sum) + (FF sum) + (HCenv sum) + (loud sens select) + 0xA5] & 0x7f
            byte total = 0;

            // TODO: Should these be calculated separately and then added to the total?

            // HCKIT sum = MORF flag, harmonics, formant
            total += (byte)(Harmonics.Morf ? 1 : 0);
            List<byte> harmonicsData = Harmonics.Data;
            foreach (var b in harmonicsData)
            {
                total += b;
            }

            List<byte> formantParameterData = Formant.Data;
            foreach (var b in formantParameterData)
            {
                total += b;
            }

            // HC code 1 sum
            foreach (var b in SoftHarmonics)
            {
                total += b;
            }

            // HC code 2 sum
            foreach (var b in LoudHarmonics)
            {
                total += b;
            }

            // FF sum
            foreach (var b in FormantFilter)
            {
                total += b;
            }

            for (var i = 0; i < HarmonicCount; i++)
            {
                List<byte> harmonicEnvelopeData = HarmonicEnvelopes[i].Data;
                foreach (var b in harmonicEnvelopeData)
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
