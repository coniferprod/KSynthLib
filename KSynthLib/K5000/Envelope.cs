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

        public byte[] ToData() => new List<byte>() { Rate, _level.Byte }.ToArray();
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
            data.AddRange(Attack.ToData());
            data.AddRange(Decay1.ToData());
            data.AddRange(Decay2.ToData());
            data.AddRange(Release.ToData());
            return data.ToArray();
        }
    }

    /// <summary>
    /// Represents the MORF harmonic envelope of an additive source.
    /// </summary>
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

        public byte[] ToData() => new List<byte>() {
            Time1, Time2, Time3, Time4, (byte)LoopType
        }.ToArray();
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

        public AmplifierEnvelope(List<byte> data) : this(data[0], data[1], data[2], data[3], data[4], data[5])
        {
        }

        public override string ToString()
        {
            return $"A={AttackTime}, D1={Decay1Time}/{Decay1Level}, D2={Decay2Time}/{Decay2Level}, R={ReleaseTime}";
        }

        public byte[] ToData() => new List<byte>() {
            AttackTime,
            Decay1Time,
            Decay1Level,
            Decay2Time,
            Decay2Level,
            ReleaseTime
        }.ToArray();
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

        public KeyScalingControlEnvelope(List<byte> data)
        {
            _level = new SignedLevelType(data[0]);
            _attackTime = new SignedLevelType(data[1]);
            _decay1Time = new SignedLevelType(data[2]);
            _releaseTime = new SignedLevelType(data[3]);
        }

        public override string ToString()
        {
            return $"Level={Level} Attack={AttackTime} Decay1={Decay1Time} Release={ReleaseTime}";
        }

        public byte[] ToData() => new List<byte>() {
            _level.Byte,
            _attackTime.Byte,
            _decay1Time.Byte,
            _releaseTime.Byte
        }.ToArray();
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

        public VelocityControlEnvelope(List<byte> data)
        {
            _level = new UnsignedLevelType(data[0]);
            _attackTime = new SignedLevelType(data[1]);
            _decay1Time = new SignedLevelType(data[2]);
            _releaseTime = new SignedLevelType(data[3]);
        }

        public override string ToString()
        {
            return $"Level={Level} Attack={AttackTime} Decay1={Decay1Time} Release={ReleaseTime}";
        }

        public byte[] ToData() => new List<byte>() {
            Level,
            _attackTime.Byte,
            _decay1Time.Byte,
            _releaseTime.Byte
        }.ToArray();
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
            _decay1Level = new SignedLevelType(b);

            (b, offset) = Util.GetNextByte(data, offset);
            Decay2Time = b;

            (b, offset) = Util.GetNextByte(data, offset);
            _decay2Level = new SignedLevelType(b);

            (b, offset) = Util.GetNextByte(data, offset);
            ReleaseTime = b;
        }

        public override string ToString()
        {
            return $"A={AttackTime}, D1=T{Decay1Time} L{Decay1Level}, D2=T{Decay2Time} L{Decay2Level}, R={ReleaseTime}";
        }

        public byte[] ToData() => new List<byte>() {
            AttackTime,
            Decay1Time,
            _decay1Level.Byte,
            Decay2Time,
            _decay2Level.Byte,
            ReleaseTime
        }.ToArray();
    }

    public class PitchEnvelope
    {
        public const int DataSize = 6;

        private SignedLevelType _startLevel; // (-63)1 ~ (+63)127
        public sbyte StartLevel
        {
            get => _startLevel.Value;
            set => _startLevel.Value = value;
        }

        private PositiveLevelType _attackTime; // 0 ~ 127
        public byte AttackTime
        {
            get => _attackTime.Value;
            set => _attackTime.Value = value;
        }

        private SignedLevelType _attackLevel; // (-63)1 ~ (+63)127
        public sbyte AttackLevel
        {
            get => _attackLevel.Value;
            set => _attackLevel.Value = value;
        }

        private PositiveLevelType _decayTime; // 0 ~ 127
        public byte DecayTime
        {
            get => _decayTime.Value;
            set => _decayTime.Value = value;
        }

        private SignedLevelType _timeVelocitySensitivity; // (-63)1 ~ (+63)127
        public sbyte TimeVelocitySensitivity
        {
            get => _timeVelocitySensitivity.Value;
            set => _timeVelocitySensitivity.Value = value;
        }

        private SignedLevelType _levelVelocitySensitivity; // (-63)1 ~ (+63)127
        public sbyte LevelVelocitySensitivity
        {
            get => _levelVelocitySensitivity.Value;
            set => _levelVelocitySensitivity.Value = value;
        }

        public PitchEnvelope()
        {
            _startLevel = new SignedLevelType();
            _attackTime = new PositiveLevelType();
            _attackLevel = new SignedLevelType(63);
            _decayTime = new PositiveLevelType(64);
            _timeVelocitySensitivity = new SignedLevelType();
            _levelVelocitySensitivity = new SignedLevelType();
        }

        public PitchEnvelope(byte[] data, int offset)
        {
            byte b = 0;

            (b, offset) = Util.GetNextByte(data, offset);
            _startLevel = new SignedLevelType(b);

            (b, offset) = Util.GetNextByte(data, offset);
            _attackTime = new PositiveLevelType(b);

            (b, offset) = Util.GetNextByte(data, offset);
            _attackLevel = new SignedLevelType(b);

            (b, offset) = Util.GetNextByte(data, offset);
            _decayTime = new PositiveLevelType(b);

            (b, offset) = Util.GetNextByte(data, offset);
            _timeVelocitySensitivity = new SignedLevelType(b);

            (b, offset) = Util.GetNextByte(data, offset);
            _levelVelocitySensitivity = new SignedLevelType(b);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("                  Pitch Envelope\n");
            builder.Append($"Strt L  {StartLevel,3}       Vel to\n");
            builder.Append($"Atak T  {AttackTime,3}     Level {LevelVelocitySensitivity,3}\n");
            builder.Append($"Atak L  {AttackLevel,3}     Time  {TimeVelocitySensitivity,3}\n");  // TODO: Sign
            builder.Append($"Decy T  {DecayTime,3}\n");
            return builder.ToString();
        }

        public byte[] ToData() => new List<byte>() {
            _startLevel.Byte, AttackTime, _attackLevel.Byte, DecayTime,
            _timeVelocitySensitivity.Byte, _levelVelocitySensitivity.Byte
        }.ToArray();
    }
}
