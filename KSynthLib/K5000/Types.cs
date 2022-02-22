using System;
using KSynthLib.Common;
using Range.Net;

namespace KSynthLib.K5000
{
    public class Coarse: RangedValue
    {
        public Coarse() : this(0) { }
        public Coarse(int value) : base("Coarse", new Range<int>(-24, 24), 0, value) { }
        public Coarse(byte value) : this(value - 64) { }
        public byte ToByte() => (byte)(this.Value + 64);
    }

    public class SignedLevel: RangedValue
    {
        public SignedLevel() : this(0) { }
        public SignedLevel(int value) : base("SignedLevel", new Range<int>(-63, 63), 0, value) { }
        public SignedLevel(byte value) : this(value - 64) { }
        public byte ToByte() => (byte)(this.Value + 64);
    }

    public class UnsignedLevel: RangedValue
    {
        public UnsignedLevel() : this(0) { }
        public UnsignedLevel(int value) : base("UnsignedLevel", new Range<int>(0, 63), 0, value) { }
        public UnsignedLevel(byte value) : this((int)value) { }
        public byte ToByte() => (byte)(this.Value);
    }

    public class PositiveLevel: RangedValue
    {
        public PositiveLevel() : this(0) { }
        public PositiveLevel(int value) : base("PositiveLevel", new Range<int>(0, 127), 0, value) { }
        public PositiveLevel(byte value) : this((int)value) { }
        public byte ToByte() => (byte)(this.Value);
    }

    public class Volume: RangedValue
    {
        public Volume() : this(0) { }
        public Volume(int value) : base("Volume", new Range<int>(0, 127), 0, value) { }
        public Volume(byte value) : this((int)value) { }
        public byte ToByte() => (byte)(this.Value);
    }

    public class ControlDepth: RangedValue
    {
        public ControlDepth() : this(0) { }
        public ControlDepth(int value) : base("ControlDepth", new Range<int>(-31, 31), 0, value) { }
        public ControlDepth(byte value) : this((int)value - 64) { }
        public byte ToByte() => (byte)(this.Value + 64);
    }

    public class EffectDepth: RangedValue
    {
        public EffectDepth() : this(0) { }
        public EffectDepth(int value) : base("EffectDepth", new Range<int>(0, 100), 0, value) { }
        public EffectDepth(byte value) : this((int)value) { }
        public byte ToByte() => (byte)(this.Value);
    }

    public class ResonanceLevel: RangedValue
    {
        public ResonanceLevel() : this(0) { }
        public ResonanceLevel(int value) : base("ResonanceLevel", new Range<int>(0, 7), 0, value) { }
        public ResonanceLevel(byte value) : this((int)value) { }
        public byte ToByte() => (byte)(this.Value);
    }

    public class Frequency: RangedValue
    {
        public Frequency() : this(0) { }
        public Frequency(int value) : base("Frequency", new Range<int>(-6, 6), 0, value) { }
        public Frequency(byte value) : this((int)value - 64) { }
        public byte ToByte() => (byte)(this.Value + 64);
    }

    public class BenderPitch: RangedValue
    {
        public BenderPitch() : this(0) { }
        public BenderPitch(int value) : base("BenderPitch", new Range<int>(0, 24), 0, value) { }
        public BenderPitch(byte value) : this((int)value) { }
        public byte ToByte() => (byte)(this.Value);
    }

    public class BenderCutoff: RangedValue
    {
        public BenderCutoff() : this(0) { }
        public BenderCutoff(int value) : base("BenderCutoff", new Range<int>(0, 31), 0, value) { }
        public BenderCutoff(byte value) : this((int)value) { }
        public byte ToByte() => (byte)(this.Value);
    }

    public class Key: RangedValue
    {
        public Key() : this(0) { }
        public Key(int value) : base("Key", new Range<int>(0, 127), 0, value) { }
        public Key(byte value) : this((int)value) { }
        public byte ToByte() => (byte)(this.Value);
    }

    public class PatchNumber: RangedValue
    {
        public PatchNumber() : this(0) { }
        public PatchNumber(int value) : base("PatchNumber", new Range<int>(0, 127), 0, value) { }
        public PatchNumber(byte value) : this((int)value) { }
        public byte ToByte() => (byte)(this.Value);
    }
}
