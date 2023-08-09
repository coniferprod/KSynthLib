using Range.Net;
using KSynthLib.Common;

namespace KSynthLib.K4
{
    /// <summary>
    /// Used for velocity depth, pressure depth, key scaling depth etc.
    /// that have the range -50 ... +50.
    /// </summary>
    public class Depth: RangedValue
    {
        public Depth() : this(0) { }
        public Depth(int value) : base("Depth", new Range<int>(-50, 50), 0, value) { }
        public Depth(byte value) : this(value - 50) { }
        public byte ToByte() => (byte)(this.Value + 50);
    }

    // Level from 0...100, for example patch volume.
    public class Level: RangedValue
    {
        public Level() : this(0) { }
        public Level(int value) : base("Level", new Range<int>(0, 100), 0, value) { }
        public Level(byte value) : this((int)value) { }
        public byte ToByte() => (byte)(this.Value);
    }

    public class PitchBendRange: RangedValue
    {
        public PitchBendRange() : this(0) { }
        public PitchBendRange(int value) : base("PitchBendRange", new Range<int>(0, 12), 0, value) { }
        public PitchBendRange(byte value) : this((int)value) { }
        public byte ToByte() => (byte)(this.Value);
    }

    public class Coarse: RangedValue
    {
        public Coarse() : this(0) { }
        public Coarse(int value) : base("Coarse", new Range<int>(-24, 24), 0, value) { }
        public Coarse(byte value) : this((value & 0x3f) - 24) { }
        public byte ToByte() => (byte)(this.Value + 24);
    }

    public class EffectNumber: RangedValue
    {
        public EffectNumber() : this(1) { }
        public EffectNumber(int value) : base("EffectNumber", new Range<int>(1, 32), 1, value) { }
        public EffectNumber(byte value) : this(value + 1) { }
        public byte ToByte() => (byte)(this.Value - 1);  // adjust to 0...31 for SysEx
    }

    public class SmallEffectParameter: RangedValue
    {
        public SmallEffectParameter() : this(0) { }
        public SmallEffectParameter(int value) : base("SmallEffectParameter", new Range<int>(0, 7), 0, value) { }
        public SmallEffectParameter(byte value) : this(value & 0x07) { }
        public byte ToByte() => (byte)(this.Value);
    }

    public class LargeEffectParameter: RangedValue
    {
        public LargeEffectParameter() : this(0) { }
        public LargeEffectParameter(int value) : base("LargeEffectParameter", new Range<int>(0, 31), 0, value) { }
        public LargeEffectParameter(byte value) : this(value & 0x1f) { }
        public byte ToByte() => (byte)(this.Value);
    }

    public class Key: RangedValue
    {
        public Key() : this(60) { }
        public Key(int value) : base("Key", new Range<int>(0, 127), 60, value) { }
        public Key(byte value) : this((int)value) { }
        public byte ToByte() => (byte)(this.Value);

        public string NoteName => PatchUtil.GetNoteName(this.Value);
    }

    public class Resonance: RangedValue
    {
        public Resonance() : this(0) { }
        public Resonance(int value) : base("Resonance", new Range<int>(0, 7), 0, value) { }
        public Resonance(byte value) : this((int)value) { }
        public byte ToByte() => (byte)(this.Value);
    }

    public class PanValue: RangedValue
    {
        public PanValue() : this(0) { }
        public PanValue(int value) : base("PanValue", new Range<int>(-7, 7), 0, value) { }
        public PanValue(byte value) : this((int)(value - 7)) { }
        public byte ToByte() => (byte)(this.Value + 7); // stored in SysEx as 0...14
    }

    public class PatchNumber: RangedValue
    {
        public PatchNumber() : this(1) { }
        public PatchNumber(int value) : base("PatchNumber", new Range<int>(1, 64), 1, value) { }
        public PatchNumber(byte value) : this((value & 0x3f) + 1) { } // adjust from 0...63 to 1...64
        public byte ToByte() => (byte)(this.Value - 1);  // adjust from 1...64 to 0...63 for SysEx
    }

    public class Transpose: RangedValue
    {
        public Transpose() : this(0) { }
        public Transpose(int value) : base("Transpose", new Range<int>(-24, 24), 0, value) { }
        public Transpose(byte value) : this(value - 24) { } // to -24...+24

        public byte ToByte() => (byte)(this.Value + 24);  // adjust from -24...+24 to 0...48
    }

    public enum VelocityCurve
    {
        Curve1,
        Curve2,
        Curve3,
        Curve4,
        Curve5,
        Curve6,
        Curve7,
        Curve8
    }
}
