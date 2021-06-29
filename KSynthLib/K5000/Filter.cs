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

    // Same as amp envelope, but decay levels 1...127 are interpreted as -63 ... 63
    public class FilterEnvelope
    {
        public const int DataSize = 6;

        private PositiveLevelType _attackTime; // 0~127
        public byte AttackTime
        {
            get => _attackTime.Value;
            set => _attackTime.Value = value;
        }

        private PositiveLevelType _decay1Time;
        public byte Decay1Time
        {
            get => _decay1Time.Value;
            set => _decay1Time.Value = value;
        }

        private SignedLevelType _decay1Level;
        public sbyte Decay1Level
        {
            get => _decay1Level.Value;
            set => _decay1Level.Value = value;
        }

        private PositiveLevelType _decay2Time;
        public byte Decay2Time
        {
            get => _decay2Time.Value;
            set => _decay2Time.Value = value;
        }

        private SignedLevelType _decay2Level;
        public sbyte Decay2Level
        {
            get => _decay2Level.Value;
            set => _decay2Level.Value = value;
        }

        private PositiveLevelType _releaseTime;
        public byte ReleaseTime
        {
            get => _releaseTime.Value;
            set => _releaseTime.Value = value;
        }

        public FilterEnvelope()
        {
            _attackTime = new PositiveLevelType();
            _decay1Time = new PositiveLevelType(63);
            _decay1Level = new SignedLevelType(32);
            _decay2Time = new PositiveLevelType(63);
            _decay2Level = new SignedLevelType(32);
            _releaseTime = new PositiveLevelType(63);
        }

        public FilterEnvelope(byte[] data, int offset)
        {
            byte b = 0;  // will be reused when getting the next byte

            (b, offset) = Util.GetNextByte(data, offset);
            AttackTime = b;

            (b, offset) = Util.GetNextByte(data, offset);
            Decay1Time = b;

            (b, offset) = Util.GetNextByte(data, offset);
            Decay1Level = (sbyte)(b - 64);

            (b, offset) = Util.GetNextByte(data, offset);
            Decay2Time = b;

            (b, offset) = Util.GetNextByte(data, offset);
            Decay2Level = (sbyte)(b - 64);

            (b, offset) = Util.GetNextByte(data, offset);
            ReleaseTime = b;
        }

        public override string ToString()
        {
            return $"A={AttackTime}, D1=T{Decay1Time} L{Decay1Level}, D2=T{Decay2Time} L{Decay2Level}, R={ReleaseTime}";
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();

            data.Add(AttackTime);
            data.Add(Decay1Time);
            data.Add(_decay1Level.AsByte());
            data.Add((byte)Decay2Time);
            data.Add(_decay2Level.AsByte());
            data.Add(ReleaseTime);

            return data.ToArray();
        }
    }

    public class DCFSettings
    {
        public bool IsActive;
        public FilterMode Mode;

        private VelocityCurveType _velocityCurve;
        public byte VelocityCurve   // 0 ~ 11 (1 ~ 12)
        {
            get => _velocityCurve.Value;
            set => _velocityCurve.Value = value;
        }

        private ResonanceType _resonance; // 0 ~ 7
        public byte Resonance
        {
            get => _resonance.Value;
            set => _resonance.Value = value;
        }

        private ResonanceType _level;  // TODO: maybe make a dedicated type for this
        public byte Level
        {
            get => _level.Value;
            set => _level.Value = value;
        }

        private PositiveLevelType _cutoff;
        public byte Cutoff
        {
            get => _cutoff.Value;
            set => _cutoff.Value = value;
        }

        private SignedLevelType _cutoffKeyScalingDepth;
        public sbyte CutoffKeyScalingDepth // (-63)1 ~ (+63)127
        {
            get => _cutoffKeyScalingDepth.Value;
            set => _cutoffKeyScalingDepth.Value = value;
        }

        private SignedLevelType _cutoffVelocityDepth; // (-63)1 ~ (+63)127
        public sbyte CutoffVelocityDepth
        {
            get => _cutoffVelocityDepth.Value;
            set => _cutoffVelocityDepth.Value = value;
        }

        private SignedLevelType _envelopeDepth; // (-63)1 ~ (+63)127
        public sbyte EnvelopeDepth
        {
            get => _envelopeDepth.Value;
            set => _envelopeDepth.Value = value;
        }

        public FilterEnvelope Envelope;

        private SignedLevelType _keyScalingToEnvelopeAttackTime;
        public sbyte KSToEnvAttackTime
        {
            get => _keyScalingToEnvelopeAttackTime.Value;
            set => _keyScalingToEnvelopeAttackTime.Value = value;
        }

        private SignedLevelType _keyScalingToEnvelopeDecay1Time;
        public sbyte KSToEnvDecay1Time
        {
            get => _keyScalingToEnvelopeDecay1Time.Value;
            set => _keyScalingToEnvelopeDecay1Time.Value = value;
        }

        private SignedLevelType _velocityToEnvDepth;
        public sbyte VelocityToEnvDepth
        {
            get => _velocityToEnvDepth.Value;
            set => _velocityToEnvDepth.Value = value;
        }

        private SignedLevelType _velocityToEnvAttackTime;
        public sbyte VelocityToEnvAttackTime
        {
            get => _velocityToEnvAttackTime.Value;
            set => _velocityToEnvAttackTime.Value = value;
        }

        private SignedLevelType _velocityToEnvDecay1Time;
        public sbyte VelocityToEnvDecay1Time
        {
            get => _velocityToEnvDecay1Time.Value;
            set => _velocityToEnvDecay1Time.Value = value;
        }

        public DCFSettings()
        {
            Envelope = new FilterEnvelope();

            _velocityCurve = new VelocityCurveType();
            _resonance = new ResonanceType();
            _level = new ResonanceType();
            _cutoff = new PositiveLevelType();
            _cutoffKeyScalingDepth = new SignedLevelType();
            _cutoffVelocityDepth = new SignedLevelType();
            _envelopeDepth = new SignedLevelType();
            _keyScalingToEnvelopeAttackTime = new SignedLevelType();
            _keyScalingToEnvelopeDecay1Time = new SignedLevelType();
            _velocityToEnvDepth = new SignedLevelType();
            _velocityToEnvAttackTime = new SignedLevelType();
            _velocityToEnvDecay1Time = new SignedLevelType();
        }

        public DCFSettings(byte[] data, int offset)
        {
            byte b = 0;  // will be reused when getting the next byte
            (b, offset) = Util.GetNextByte(data, offset);
            IsActive = (b == 0);  // 0=Active, 1=Bypass

            (b, offset) = Util.GetNextByte(data, offset);
            Mode = (FilterMode)b;

            (b, offset) = Util.GetNextByte(data, offset);
            _velocityCurve = new VelocityCurveType((byte)(b + 1)); // 0~11 to 1~12

            (b, offset) = Util.GetNextByte(data, offset);
            _resonance = new ResonanceType(b); // 0~7

            (b, offset) = Util.GetNextByte(data, offset);
            _level = new ResonanceType(b); // 0~7 (7~0)

            (b, offset) = Util.GetNextByte(data, offset);
            _cutoff = new PositiveLevelType(b);

            (b, offset) = Util.GetNextByte(data, offset);
            _cutoffKeyScalingDepth = new SignedLevelType(b);

            (b, offset) = Util.GetNextByte(data, offset);
            _cutoffVelocityDepth = new SignedLevelType(b);

            (b, offset) = Util.GetNextByte(data, offset);
            _envelopeDepth = new SignedLevelType(b);

            Envelope = new FilterEnvelope(data, offset);
            offset += FilterEnvelope.DataSize;

            (b, offset) = Util.GetNextByte(data, offset);
            _keyScalingToEnvelopeAttackTime = new SignedLevelType(b);

            (b, offset) = Util.GetNextByte(data, offset);
            _keyScalingToEnvelopeDecay1Time = new SignedLevelType(b);

            (b, offset) = Util.GetNextByte(data, offset);
            _velocityToEnvDepth = new SignedLevelType(b);

            (b, offset) = Util.GetNextByte(data, offset);
            _velocityToEnvAttackTime = new SignedLevelType(b);

            (b, offset) = Util.GetNextByte(data, offset);
            _velocityToEnvDecay1Time = new SignedLevelType(b);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("                     DCF\n");

            string activeSetting = IsActive ? "Active" : "--";
            builder.Append($"DCF      {activeSetting}     Resonance    {Resonance}\n");
            builder.Append($"Cutoff      {Cutoff}        DCF Level    {Level}\n");
            builder.Append($"Mode      {Mode}      KS to Cut   {CutoffKeyScalingDepth}\n");
            builder.Append($"Velo Curve  {VelocityCurve}      Velo to Cut    {CutoffVelocityDepth}\n");

            builder.Append("             DCF Envelope\n");

            builder.Append($"Depth     {EnvelopeDepth}       Dcy2 T    {Envelope.Decay2Time}\n");
            builder.Append($"Atak T    {Envelope.AttackTime}       Dcy2 L    {Envelope.Decay2Level}\n");
            builder.Append($"Dcy1 T    {Envelope.Decay1Time}       Release   {Envelope.ReleaseTime}\n");
            builder.Append($"Dcy1 L    {Envelope.Decay1Level}\n");
            builder.Append($"KS to Attack  {KSToEnvAttackTime,3}   Velo to Env   {VelocityToEnvDepth,3}\n");
            builder.Append($"KS to DCY1    {KSToEnvDecay1Time,3}   Velo to Atk   {VelocityToEnvAttackTime,3}\n");
            builder.Append($"                        Vel to DCY1 = {VelocityToEnvDecay1Time}\n");

            return builder.ToString();
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();

            data.Add((byte)(IsActive ? 1 : 0));
            data.Add((byte)Mode);
            data.Add((byte)(VelocityCurve - 1));  // adjust 1 ~ 12 to 0 ~ 11
            data.Add(Resonance);
            data.Add(Level);
            data.Add(Cutoff);
            data.Add(_cutoffKeyScalingDepth.AsByte());
            data.Add(_cutoffVelocityDepth.AsByte());
            data.Add(_envelopeDepth.AsByte());

            data.AddRange(Envelope.ToData());

            data.Add(_keyScalingToEnvelopeAttackTime.AsByte());
            data.Add(_keyScalingToEnvelopeDecay1Time.AsByte());
            data.Add(_velocityToEnvDepth.AsByte());
            data.Add(_velocityToEnvAttackTime.AsByte());
            data.Add(_velocityToEnvDecay1Time.AsByte());

            return data.ToArray();
        }
    }
}
