using System;
using KSynthLib.Common;
using Range.Net;

namespace KSynthLib.K5
{
    public class Depth: RangedValue
    {
        public Depth() : this(0) { }
        public Depth(int value) : base("Depth", new Range<int>(-31, 31), 0, value) { }
        public Depth(byte value) : this(value - 31) { }
        public byte ToByte() => (byte)(this.Value + 31);
    }

    public class PositiveDepth: RangedValue
    {
        public PositiveDepth() : this(0) { }
        public PositiveDepth(int value) : base("PositiveDepth", new Range<int>(0, 31), 0, value) { }
        public PositiveDepth(byte value) : this((int)value) { }
        public byte ToByte() => (byte)(this.Value);
    }

    public class Level: RangedValue
    {
        public Level() : this(0) { }
        public Level(int value) : base("Level", new Range<int>(0, 99), 99, value) { }
        public Level(byte value) : this((int)value) { }
        public byte ToByte() => (byte)(this.Value);
    }

    public class Rate: RangedValue
    {
        public Rate() : this(0) { }
        public Rate(int value) : base("Rate", new Range<int>(-15, 15), 0, value) { }
        public Rate(byte value) : this((int)value) { }
        public byte ToByte() => (byte)(this.Value);
    }

    public class HarmonicNumber: RangedValue
    {
        public HarmonicNumber() : this(1) { }
        public HarmonicNumber(int value) : base("HarmonicNumber", new Range<int>(1, 63), 1, value) { }
        public HarmonicNumber(byte value) : this((int)value) { }
        public byte ToByte() => (byte)(this.Value);
    }

    public class Coarse: RangedValue
    {
        public Coarse() : this(0) { }
        public Coarse(int value) : base("Coarse", new Range<int>(-48, 48), 0, value) { }
        public Coarse(byte value) : this((int)value) { }
        public byte ToByte() => (byte)(this.Value);
    }

    public class BenderDepth: RangedValue
    {
        public BenderDepth() : this(0) { }
        public BenderDepth(int value) : base("BenderDepth", new Range<int>(0, 24), 0, value) { }
        public BenderDepth(byte value) : this((int)value) { }
        public byte ToByte() => (byte)(this.Value);
    }

    public class EnvelopeDepth: RangedValue
    {
        public EnvelopeDepth() : this(0) { }
        public EnvelopeDepth(int value) : base("EnvelopeDepth", new Range<int>(-24, 24), 0, value) { }
        public EnvelopeDepth(byte value) : this((int)value) { }
        public byte ToByte() => (byte)(this.Value);
    }

    public class Volume: RangedValue
    {
        public Volume() : this(0) { }
        public Volume(int value) : base("Volume", new Range<int>(0, 63), 0, value) { }
        public Volume(byte value) : this((int)value) { }
        public byte ToByte() => (byte)(this.Value);
    }

    public class KeyNumber: RangedValue
    {
        public KeyNumber() : this(60) { }
        public KeyNumber(int value) : base("KeyNumber", new Range<int>(0, 127), 60, value) { }
        public KeyNumber(byte value) : this((int)value) { }
        public byte ToByte() => (byte)(this.Value);

        public string NoteName => PatchUtil.GetNoteName(this.Value);
    }

    public class EnvelopeNumber: RangedValue
    {
        public EnvelopeNumber() : this(1) { }
        public EnvelopeNumber(int value) : base("EnvelopeNumber", new Range<int>(1, 4), 1, value) { }
        public EnvelopeNumber(byte value) : this((int)(value + 1)) { }
        public byte ToByte() => (byte)(this.Value - 1);
    }
}
