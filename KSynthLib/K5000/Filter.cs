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
            var builder = new StringBuilder();
            builder.Append("                     DCF\n");

            var activeSetting = IsActive ? "Active" : "--";
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
            var data = new List<byte>();

            data.AddRange(new List<byte>() {
                (byte)(IsActive ? 1 : 0),
                (byte)Mode,
                (byte)(VelocityCurve - 1),  // adjust 1 ~ 12 to 0 ~ 11
                Resonance, Level, Cutoff,
                _cutoffKeyScalingDepth.Byte,
                _cutoffVelocityDepth.Byte,
                _envelopeDepth.Byte
            });

            data.AddRange(Envelope.ToData());

            data.AddRange(new List<byte>() {
                _keyScalingToEnvelopeAttackTime.Byte,
                _keyScalingToEnvelopeDecay1Time.Byte,
                _velocityToEnvDepth.Byte,
                _velocityToEnvAttackTime.Byte,
                _velocityToEnvDecay1Time.Byte
            });

            return data.ToArray();
        }
    }
}
