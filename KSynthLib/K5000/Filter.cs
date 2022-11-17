using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K5000
{
    public enum FilterMode
    {
        LowPass,
        HighPass
    }


    public class DCFSettings: ISystemExclusiveData
    {
        public bool IsActive;
        public FilterMode Mode;
        public VelocityCurve VelocityCurve;
        public ResonanceLevel Resonance; // 0 ~ 7
        public UnsignedLevel Level;  // filter input level (7~0)
        public PositiveLevel Cutoff;
        public SignedLevel CutoffKeyScalingDepth;
        public SignedLevel CutoffVelocityDepth; // (-63)1 ~ (+63)127
        public SignedLevel EnvelopeDepth; // (-63)1 ~ (+63)127
        public FilterEnvelope Envelope;
        public SignedLevel KeyScalingToEnvelopeAttackTime;
        public SignedLevel KeyScalingToEnvelopeDecay1Time;
        public SignedLevel VelocityToEnvelopeDepth;
        public SignedLevel VelocityToEnvelopeAttackTime;
        public SignedLevel VelocityToEnvelopeDecay1Time;

        public DCFSettings()
        {
            Envelope = new FilterEnvelope();
            VelocityCurve = VelocityCurve.Curve1;
            Resonance = new ResonanceLevel();
            Level = new UnsignedLevel();
            Cutoff = new PositiveLevel();
            CutoffKeyScalingDepth = new SignedLevel();
            CutoffVelocityDepth = new SignedLevel();
            EnvelopeDepth = new SignedLevel();
            KeyScalingToEnvelopeAttackTime = new SignedLevel();
            KeyScalingToEnvelopeDecay1Time = new SignedLevel();
            VelocityToEnvelopeDepth = new SignedLevel();
            VelocityToEnvelopeAttackTime = new SignedLevel();
            VelocityToEnvelopeDecay1Time = new SignedLevel();
        }

        public DCFSettings(byte[] data, int offset)
        {
            byte b = 0;  // will be reused when getting the next byte
            (b, offset) = Util.GetNextByte(data, offset);
            IsActive = (b == 0);  // 0=Active, 1=Bypass

            (b, offset) = Util.GetNextByte(data, offset);
            Mode = (FilterMode)b;

            (b, offset) = Util.GetNextByte(data, offset);
            VelocityCurve = (VelocityCurve)b;

            (b, offset) = Util.GetNextByte(data, offset);
            Resonance = new ResonanceLevel(b); // 0~7

            (b, offset) = Util.GetNextByte(data, offset);
            Level = new UnsignedLevel(b);

            (b, offset) = Util.GetNextByte(data, offset);
            Cutoff = new PositiveLevel(b);

            (b, offset) = Util.GetNextByte(data, offset);
            CutoffKeyScalingDepth = new SignedLevel(b);

            (b, offset) = Util.GetNextByte(data, offset);
            CutoffVelocityDepth = new SignedLevel(b);

            (b, offset) = Util.GetNextByte(data, offset);
            EnvelopeDepth = new SignedLevel(b);

            Envelope = new FilterEnvelope(data, offset);
            offset += FilterEnvelope.DataSize;

            (b, offset) = Util.GetNextByte(data, offset);
            KeyScalingToEnvelopeAttackTime = new SignedLevel(b);

            (b, offset) = Util.GetNextByte(data, offset);
            KeyScalingToEnvelopeDecay1Time = new SignedLevel(b);

            (b, offset) = Util.GetNextByte(data, offset);
            VelocityToEnvelopeDepth = new SignedLevel(b);

            (b, offset) = Util.GetNextByte(data, offset);
            VelocityToEnvelopeAttackTime = new SignedLevel(b);

            (b, offset) = Util.GetNextByte(data, offset);
            VelocityToEnvelopeDecay1Time = new SignedLevel(b);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("                     DCF\n");

            var activeSetting = IsActive ? "Active" : "--";
            builder.Append($"DCF      {activeSetting}     Resonance    {Resonance.Value}\n");
            builder.Append($"Cutoff      {Cutoff.Value}        DCF Level    {Level.Value}\n");
            builder.Append($"Mode      {Mode}      KS to Cut   {CutoffKeyScalingDepth.Value}\n");
            builder.Append($"Velo Curve  {VelocityCurve}      Velo to Cut    {CutoffVelocityDepth.Value}\n");

            builder.Append("             DCF Envelope\n");

            builder.Append($"Depth     {EnvelopeDepth.Value}       Dcy2 T    {Envelope.Decay2Time.Value}\n");
            builder.Append($"Atak T    {Envelope.AttackTime.Value}       Dcy2 L    {Envelope.Decay2Level.Value}\n");
            builder.Append($"Dcy1 T    {Envelope.Decay1Time.Value}       Release   {Envelope.ReleaseTime.Value}\n");
            builder.Append($"Dcy1 L    {Envelope.Decay1Level.Value}\n");
            builder.Append($"KS to Attack  {KeyScalingToEnvelopeAttackTime.Value,3}   Velo to Env   {VelocityToEnvelopeDepth.Value,3}\n");
            builder.Append($"KS to DCY1    {KeyScalingToEnvelopeDecay1Time.Value,3}   Velo to Atk   {VelocityToEnvelopeAttackTime.Value,3}\n");
            builder.Append($"                        Vel to DCY1 = {VelocityToEnvelopeDecay1Time.Value}\n");

            return builder.ToString();
        }

        public List<byte> GetSystemExclusiveData()
        {
            var data = new List<byte>();

            data.AddRange(new List<byte>() {
                (byte)(IsActive ? 1 : 0),
                (byte)Mode,
                (byte)VelocityCurve,
                Resonance.ToByte(), Level.ToByte(), Cutoff.ToByte(),
                CutoffKeyScalingDepth.ToByte(),
                CutoffVelocityDepth.ToByte(),
                EnvelopeDepth.ToByte()
            });

            data.AddRange(Envelope.ToData());

            data.AddRange(new List<byte>() {
                KeyScalingToEnvelopeAttackTime.ToByte(),
                KeyScalingToEnvelopeDecay1Time.ToByte(),
                VelocityToEnvelopeDepth.ToByte(),
                VelocityToEnvelopeAttackTime.ToByte(),
                VelocityToEnvelopeDecay1Time.ToByte()
            });

            return data;
        }
    }
}
