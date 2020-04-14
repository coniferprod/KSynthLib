using System;
using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K5000
{
    /// <summary>
    /// Represents a DCA envelope.
    /// </summary>
    public class AmplifierEnvelope
    {
        private PositiveLevelType _attackTime; // 0~127
        public byte AttackTime
        {
            get => _attackTime.Value;
            set => _attackTime.Value = value;
        }

        private PositiveLevelType _decay1Time; // 0~127
        public byte Decay1Time
        {
            get => _decay1Time.Value;
            set => _decay1Time.Value = value;
        }

        private PositiveLevelType _decay1Level; // 0~127
        public byte Decay1Level
        {
            get => _decay1Level.Value;
            set => _decay1Level.Value = value;
        }

        private PositiveLevelType _decay2Time; // 0~127
        public byte Decay2Time
        {
            get => _decay2Time.Value;
            set => _decay2Time.Value = value;
        }

        private PositiveLevelType _decay2Level; // 0~127
        public byte Decay2Level
        {
            get => _decay2Level.Value;
            set => _decay2Level.Value = value;
        }

        private PositiveLevelType _releaseTime; // 0~127
        public byte ReleaseTime
        {
            get => _releaseTime.Value;
            set => _releaseTime.Value = value;
        }

        public AmplifierEnvelope()
        {
            _attackTime = new PositiveLevelType();
            _decay1Time = new PositiveLevelType();
            _decay1Level = new PositiveLevelType();
            _decay2Time = new PositiveLevelType();
            _decay2Level = new PositiveLevelType();
            _releaseTime = new PositiveLevelType();
        }

        public AmplifierEnvelope(byte a, byte d1t, byte d1l, byte d2t, byte d2l, byte r)
        {
            _attackTime = new PositiveLevelType(a);
            _decay1Time = new PositiveLevelType(d1t);
            _decay1Level = new PositiveLevelType(d1l);
            _decay2Time = new PositiveLevelType(d2t);
            _decay2Level = new PositiveLevelType(d2l);
            _releaseTime = new PositiveLevelType(r);
        }

        public override string ToString()
        {
            return $"A={AttackTime}, D1={Decay1Time}/{Decay1Level}, D2={Decay2Time}/{Decay2Level}, R={ReleaseTime}";
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();

            data.Add((byte)AttackTime);
            data.Add((byte)Decay1Time);
            data.Add((byte)Decay1Level);
            data.Add((byte)Decay2Time);
            data.Add((byte)Decay2Level);
            data.Add((byte)ReleaseTime);

            return data.ToArray();
        }
    }

    /// <summary>
    /// Represents a key scaling control envelope.
    /// </summary>
    public class KeyScalingControlEnvelope
    {
        private SignedLevelType _level;
        public sbyte Level // (-63)1 ~ (+63)127
        {
            get => _level.Value;
            set => _level.Value = value;
        }

        private SignedLevelType _attackTime;
        public sbyte AttackTime // (-63)1 ~ (+63)127
        {
            get => _attackTime.Value;
            set => _attackTime.Value = value;
        }

        private SignedLevelType _decay1Time;
        public sbyte Decay1Time
        {
            get => _decay1Time.Value;
            set => _decay1Time.Value = value;
        }

        private SignedLevelType _releaseTime;
        public sbyte ReleaseTime
        {
            get => _releaseTime.Value;
            set => _releaseTime.Value = value;
        }

        public KeyScalingControlEnvelope()
        {
            _level = new SignedLevelType();
            _attackTime = new SignedLevelType();
            _decay1Time = new SignedLevelType();
            _releaseTime = new SignedLevelType();
        }

        public override string ToString()
        {
            return $"Level={Level} Attack={AttackTime} Decay1={Decay1Time} Release={ReleaseTime}";
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();

            data.Add((byte)(Level + 64));
            data.Add((byte)(AttackTime + 64));
            data.Add((byte)(Decay1Time + 64));
            data.Add((byte)(ReleaseTime + 64));

            return data.ToArray();
        }
    }

    public class VelocityControlEnvelope
    {
        private UnsignedLevelType _level;   // 0 ~ 63
        public byte Level
        {
            get => _level.Value;
            set => _level.Value = value;
        }

        private SignedLevelType _attackTime; // (-63)1 ~ (+63)127
        public sbyte AttackTime
        {
            get => _attackTime.Value;
            set => _attackTime.Value = value;
        }

        private SignedLevelType _decay1Time; // (-63)1 ~ (+63)127
        public sbyte Decay1Time
        {
            get => _decay1Time.Value;
            set => _decay1Time.Value = value;
        }

        private SignedLevelType _releaseTime; // (-63)1 ~ (+63)127
        public sbyte ReleaseTime
        {
            get => _releaseTime.Value;
            set => _releaseTime.Value = value;
        }

        public VelocityControlEnvelope()
        {
            _level = new UnsignedLevelType();
            _attackTime = new SignedLevelType();
            _decay1Time = new SignedLevelType();
            _releaseTime = new SignedLevelType();
        }

        public override string ToString()
        {
            return $"Level={Level} Attack={AttackTime} Decay1={Decay1Time} Release={ReleaseTime}";
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();

            data.Add(Level);
            data.Add((byte)(AttackTime + 64));
            data.Add((byte)(Decay1Time + 64));
            data.Add((byte)(ReleaseTime + 64));

            return data.ToArray();
        }
    }

    public class DCASettings
    {
        private VelocityCurveType _velocityCurve; // values are 0 ~ 11, shown as 1 ~ 12
        public byte VelocityCurve
        {
            get => _velocityCurve.Value;
            set => _velocityCurve.Value = value;
        }

        public AmplifierEnvelope Envelope;
        public KeyScalingControlEnvelope KeyScaling;
        public VelocityControlEnvelope VelocitySensitivity;

        public DCASettings()
        {
            KeyScaling = new KeyScalingControlEnvelope();
            VelocitySensitivity = new VelocityControlEnvelope();

            _velocityCurve = new VelocityCurveType(5);

            Envelope = new AmplifierEnvelope
            {
                AttackTime = 20,
                Decay1Time = 95,
                Decay1Level = 127,
                Decay2Time = 110,
                Decay2Level = 127,
                ReleaseTime = 11
            };
            // Note that an object initializer invokes the default constructor,
            // not the one with six arguments
        }

        public DCASettings(byte[] data, int offset)
        {
            byte b = 0;  // will be reused when getting the next byte

            (b, offset) = Util.GetNextByte(data, offset);
            VelocityCurve = (byte)(b + 1);  // adjust from 0~11 to 1~12

            Envelope = new AmplifierEnvelope();

            (b, offset) = Util.GetNextByte(data, offset);
            Envelope.AttackTime = b;

            (b, offset) = Util.GetNextByte(data, offset);
            Envelope.Decay1Time = b;

            (b, offset) = Util.GetNextByte(data, offset);
            Envelope.Decay1Level = b;

            (b, offset) = Util.GetNextByte(data, offset);
            Envelope.Decay2Time = b;

            (b, offset) = Util.GetNextByte(data, offset);
            Envelope.Decay2Level = b;

            (b, offset) = Util.GetNextByte(data, offset);
            Envelope.ReleaseTime = b;

            KeyScaling = new KeyScalingControlEnvelope();
            (b, offset) = Util.GetNextByte(data, offset);
            KeyScaling.Level = (sbyte)(b - 64);
            (b, offset) = Util.GetNextByte(data, offset);
            KeyScaling.AttackTime = (sbyte)(b - 64);
            (b, offset) = Util.GetNextByte(data, offset);
            KeyScaling.Decay1Time = (sbyte)(b - 64);
            (b, offset) = Util.GetNextByte(data, offset);
            KeyScaling.ReleaseTime = (sbyte)(b - 64);

            VelocitySensitivity = new VelocityControlEnvelope();
            (b, offset) = Util.GetNextByte(data, offset);
            VelocitySensitivity.Level = b;
            (b, offset) = Util.GetNextByte(data, offset);
            VelocitySensitivity.AttackTime = (sbyte)(b - 64);
            (b, offset) = Util.GetNextByte(data, offset);
            VelocitySensitivity.Decay1Time = (sbyte)(b - 64);
            (b, offset) = Util.GetNextByte(data, offset);
            VelocitySensitivity.ReleaseTime = (sbyte)(b - 64);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("                   DCA Envelope\n");
            builder.Append($"VelCrv      {VelocityCurve,3}   Dcy2 T    {Envelope.Decay2Time,3}\n");
            builder.Append($"Atak T      {Envelope.AttackTime,3}   Dcy2 L   {Envelope.Decay2Level,3}\n");
            builder.Append($"Dcy1 T      {Envelope.Decay1Time,3}   Rels T   {Envelope.ReleaseTime,3}\n");
            builder.Append($"Dcy1 L      {Envelope.Decay1Level,3}\n");

            builder.Append("                   DCA Modulation\n");
            builder.Append("  KS TO DCA ENV       VELO TO DCA ENV\n");
            builder.Append($"Level         {KeyScaling.Level,3}     Level   {VelocitySensitivity.Level,3}\n");
            builder.Append($"Attack Time   {KeyScaling.AttackTime,3}    Attack Time    {VelocitySensitivity.AttackTime,3}\n");
            builder.Append($"Decay1 Time   {KeyScaling.Decay1Time,3}    Decay1 Time    {VelocitySensitivity.Decay1Time,3}\n");
            builder.Append($"Release       {KeyScaling.ReleaseTime,3}   Release        {VelocitySensitivity.ReleaseTime,3}\n");

            return builder.ToString();
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();

            data.Add((byte)(VelocityCurve - 1));  // adjust from 1~12 to 0~11

            data.AddRange(Envelope.ToData());
            data.AddRange(KeyScaling.ToData());
            data.AddRange(VelocitySensitivity.ToData());

            return data.ToArray();
        }
    }
}
