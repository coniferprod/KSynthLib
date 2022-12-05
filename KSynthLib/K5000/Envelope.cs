using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K5000
{
    public enum EnvelopeLoopKind
    {
        Off,
        Loop1,
        Loop2
    }

    public class EnvelopeSegment: ISystemExclusiveData
    {
        public PositiveLevel Rate; // 0 ~ 127
        public SignedLevel Level; // (-63)1 ... (+63)127

        public EnvelopeSegment()
        {
            Rate = new PositiveLevel();
            Level = new SignedLevel();
        }

        public EnvelopeSegment(byte rate, byte level)
        {
            Rate = new PositiveLevel(rate);
            Level = new SignedLevel(level);
        }

        public List<byte> GetSystemExclusiveData()
        {
            return new List<byte>() { Rate.ToByte(), Level.ToByte() };
        }
    }

    public class LoopingEnvelope: ISystemExclusiveData
    {
        public EnvelopeSegment Attack;
        public EnvelopeSegment Decay1;
        public EnvelopeSegment Decay2;
        public EnvelopeSegment Release;
        public EnvelopeLoopKind LoopKind;

        public LoopingEnvelope()
        {
            Attack = new EnvelopeSegment();
            Decay1 = new EnvelopeSegment();
            Decay2 = new EnvelopeSegment();
            Release = new EnvelopeSegment();
            LoopKind = EnvelopeLoopKind.Off;
        }

        public LoopingEnvelope(List<byte> data)
        {
            Attack = new EnvelopeSegment(data[0], data[1]);
            Decay1 = new EnvelopeSegment(data[2], data[3]);
            Decay2 = new EnvelopeSegment(data[4], data[5]);
            Release = new EnvelopeSegment(data[6], data[7]);
            LoopKind = (EnvelopeLoopKind)data[8];
        }

        public List<byte> GetSystemExclusiveData()
        {
            var data = new List<byte>();
            data.AddRange(Attack.GetSystemExclusiveData());
            data.AddRange(Decay1.GetSystemExclusiveData());
            data.AddRange(Decay2.GetSystemExclusiveData());
            data.AddRange(Release.GetSystemExclusiveData());
            return data;
        }
    }

    /// <summary>
    /// Represents the MORF harmonic envelope of an additive source.
    /// </summary>
    public class MORFHarmonicEnvelope: ISystemExclusiveData
    {
        public PositiveLevel Time1;
        public PositiveLevel Time2;
        public PositiveLevel Time3;
        public PositiveLevel Time4;
        public EnvelopeLoopKind LoopKind;

        public MORFHarmonicEnvelope()
        {
            Time1 = new PositiveLevel();
            Time2 = new PositiveLevel();
            Time3 = new PositiveLevel();
            Time4 = new PositiveLevel();
            LoopKind = EnvelopeLoopKind.Off;
        }

        public List<byte> GetSystemExclusiveData()
        {
            return new List<byte>() {
               Time1.ToByte(), Time2.ToByte(),
               Time3.ToByte(),
               Time4.ToByte(),
               (byte)LoopKind
            };
        }
    }

    public class HarmonicEnvelope: ISystemExclusiveData
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

        public List<byte> GetSystemExclusiveData()
        {
            var data = new List<byte>();

            data.Add(Segment0.Rate.ToByte());
            data.Add(Segment0.Level.ToByte());

            data.Add(Segment1.Rate.ToByte());
            byte s1Level = Segment1.Level.ToByte();
            if (Segment1Loop)
            {
                s1Level.SetBit(6);
            }
            data.Add(s1Level);

            data.Add(Segment2.Rate.ToByte());
            byte s2Level = Segment2.Level.ToByte();
            if (Segment2Loop)
            {
                s2Level.SetBit(6);
            }
            data.Add(s2Level);

            data.Add(Segment3.Rate.ToByte());
            data.Add(Segment3.Level.ToByte());

            return data;
        }
    }

    /// <summary>
    /// Represents a DCA envelope.
    /// </summary>
    public class AmplifierEnvelope
    {
        public PositiveLevel AttackTime; // 0~127
        public PositiveLevel Decay1Time; // 0~127
        public PositiveLevel Decay1Level; // 0~127
        public PositiveLevel Decay2Time; // 0~127
        public PositiveLevel Decay2Level; // 0~127
        public PositiveLevel ReleaseTime; // 0~127

        public AmplifierEnvelope()
        {
            AttackTime = new PositiveLevel();
            Decay1Time = new PositiveLevel();
            Decay1Level = new PositiveLevel();
            Decay2Time = new PositiveLevel();
            Decay2Level = new PositiveLevel();
            ReleaseTime = new PositiveLevel();
        }

        public AmplifierEnvelope(byte a, byte d1t, byte d1l, byte d2t, byte d2l, byte r)
        {
            AttackTime = new PositiveLevel(a);
            Decay1Time = new PositiveLevel(d1t);
            Decay1Level = new PositiveLevel(d1l);
            Decay2Time = new PositiveLevel(d2t);
            Decay2Level = new PositiveLevel(d2l);
            ReleaseTime = new PositiveLevel(r);
        }

        public AmplifierEnvelope(List<byte> data) : this(data[0], data[1], data[2], data[3], data[4], data[5])
        {
        }

        public override string ToString()
        {
            return $"A={AttackTime.Value}, D1={Decay1Time.Value}/{Decay1Level.Value}, D2={Decay2Time.Value}/{Decay2Level.Value}, R={ReleaseTime.Value}";
        }

        public byte[] ToData() => new List<byte>() {
            AttackTime.ToByte(),
            Decay1Time.ToByte(),
            Decay1Level.ToByte(),
            Decay2Time.ToByte(),
            Decay2Level.ToByte(),
            ReleaseTime.ToByte()
        }.ToArray();
    }

    /// <summary>
    /// Represents a key scaling control envelope.
    /// </summary>
    public class KeyScalingControlEnvelope
    {
        public SignedLevel Level;  // (-63)1 ~ (+63)127
        public SignedLevel AttackTime;  // (-63)1 ~ (+63)127
        public SignedLevel Decay1Time;
        public SignedLevel ReleaseTime;

        public KeyScalingControlEnvelope()
        {
            Level = new SignedLevel();
            AttackTime = new SignedLevel();
            Decay1Time = new SignedLevel();
            ReleaseTime = new SignedLevel();
        }

        public KeyScalingControlEnvelope(List<byte> data)
        {
            Level = new SignedLevel(data[0]);
            AttackTime = new SignedLevel(data[1]);
            Decay1Time = new SignedLevel(data[2]);
            ReleaseTime = new SignedLevel(data[3]);
        }

        public override string ToString()
        {
            return $"Level={Level.Value} Attack={AttackTime.Value} Decay1={Decay1Time.Value} Release={ReleaseTime.Value}";
        }

        public byte[] ToData() => new List<byte>() {
            Level.ToByte(),
            AttackTime.ToByte(),
            Decay1Time.ToByte(),
            ReleaseTime.ToByte()
        }.ToArray();
    }

    public class VelocityControlEnvelope
    {
        public UnsignedLevel Level;   // 0 ~ 63
        public SignedLevel AttackTime; // (-63)1 ~ (+63)127
        public SignedLevel Decay1Time; // (-63)1 ~ (+63)127
        public SignedLevel ReleaseTime; // (-63)1 ~ (+63)127

        public VelocityControlEnvelope()
        {
            Level = new UnsignedLevel();
            AttackTime = new SignedLevel();
            Decay1Time = new SignedLevel();
            ReleaseTime = new SignedLevel();
        }

        public VelocityControlEnvelope(List<byte> data)
        {
            Level = new UnsignedLevel(data[0]);
            AttackTime = new SignedLevel(data[1]);
            Decay1Time = new SignedLevel(data[2]);
            ReleaseTime = new SignedLevel(data[3]);
        }

        public override string ToString()
        {
            return $"Level={Level.Value} Attack={AttackTime.Value} Decay1={Decay1Time.Value} Release={ReleaseTime.Value}";
        }

        public byte[] ToData() => new List<byte>() {
            Level.ToByte(),
            AttackTime.ToByte(),
            Decay1Time.ToByte(),
            ReleaseTime.ToByte()
        }.ToArray();
    }

    // Same as amp envelope, but decay levels 1...127 are interpreted as -63 ... 63
    public class FilterEnvelope
    {
        public const int DataSize = 6;

        public PositiveLevel AttackTime; // 0~127
        public PositiveLevel Decay1Time;
        public SignedLevel Decay1Level;
        public PositiveLevel Decay2Time;
        public SignedLevel Decay2Level;
        public PositiveLevel ReleaseTime;

        public FilterEnvelope()
        {
            AttackTime = new PositiveLevel();
            Decay1Time = new PositiveLevel(63);
            Decay1Level = new SignedLevel(32);
            Decay2Time = new PositiveLevel(63);
            Decay2Level = new SignedLevel(32);
            ReleaseTime = new PositiveLevel(63);
        }

        public FilterEnvelope(byte[] data, int offset)
        {
            byte b = 0;  // will be reused when getting the next byte

            (b, offset) = Util.GetNextByte(data, offset);
            AttackTime = new PositiveLevel(b);

            (b, offset) = Util.GetNextByte(data, offset);
            Decay1Time = new PositiveLevel(b);

            (b, offset) = Util.GetNextByte(data, offset);
            Decay1Level = new SignedLevel(b);

            (b, offset) = Util.GetNextByte(data, offset);
            Decay2Time = new PositiveLevel(b);

            (b, offset) = Util.GetNextByte(data, offset);
            Decay2Level = new SignedLevel(b);

            (b, offset) = Util.GetNextByte(data, offset);
            ReleaseTime = new PositiveLevel(b);
        }

        public override string ToString()
        {
            return $"A={AttackTime.Value}, D1=T{Decay1Time.Value} L{Decay1Level.Value}, D2=T{Decay2Time.Value} L{Decay2Level.Value}, R={ReleaseTime.Value}";
        }

        public byte[] ToData() => new List<byte>() {
            AttackTime.ToByte(),
            Decay1Time.ToByte(),
            Decay1Level.ToByte(),
            Decay2Time.ToByte(),
            Decay2Level.ToByte(),
            ReleaseTime.ToByte()
        }.ToArray();
    }

    public class PitchEnvelope
    {
        public const int DataSize = 6;

        public SignedLevel StartLevel; // (-63)1 ~ (+63)127
        public PositiveLevel AttackTime; // 0 ~ 127
        public SignedLevel AttackLevel; // (-63)1 ~ (+63)127
        public PositiveLevel DecayTime; // 0 ~ 127
        public SignedLevel TimeVelocitySensitivity; // (-63)1 ~ (+63)127
        public SignedLevel LevelVelocitySensitivity; // (-63)1 ~ (+63)127

        public PitchEnvelope()
        {
            StartLevel = new SignedLevel();
            AttackTime = new PositiveLevel();
            AttackLevel = new SignedLevel(63);
            DecayTime = new PositiveLevel(64);
            TimeVelocitySensitivity = new SignedLevel();
            LevelVelocitySensitivity = new SignedLevel();
        }

        public PitchEnvelope(byte[] data, int offset)
        {
            byte b = 0;

            (b, offset) = Util.GetNextByte(data, offset);
            StartLevel = new SignedLevel(b);

            (b, offset) = Util.GetNextByte(data, offset);
            AttackTime = new PositiveLevel(b);

            (b, offset) = Util.GetNextByte(data, offset);
            AttackLevel = new SignedLevel(b);

            (b, offset) = Util.GetNextByte(data, offset);
            DecayTime = new PositiveLevel(b);

            (b, offset) = Util.GetNextByte(data, offset);
            TimeVelocitySensitivity = new SignedLevel(b);

            (b, offset) = Util.GetNextByte(data, offset);
            LevelVelocitySensitivity = new SignedLevel(b);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("                  Pitch Envelope\n");
            builder.Append($"Strt L  {StartLevel.Value,3}       Vel to\n");
            builder.Append($"Atak T  {AttackTime.Value,3}     Level {LevelVelocitySensitivity.Value,3}\n");
            builder.Append($"Atak L  {AttackLevel.Value,3}     Time  {TimeVelocitySensitivity.Value,3}\n");  // TODO: Sign
            builder.Append($"Decy T  {DecayTime.Value,3}\n");
            return builder.ToString();
        }

        public byte[] ToData() => new List<byte>() {
            StartLevel.ToByte(), AttackTime.ToByte(), AttackLevel.ToByte(), DecayTime.ToByte(),
            TimeVelocitySensitivity.ToByte(), LevelVelocitySensitivity.ToByte()
        }.ToArray();
    }
}
