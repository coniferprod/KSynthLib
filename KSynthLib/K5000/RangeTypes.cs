using System;

using Range.Net;

namespace KSynthLib.K5000
{
        public abstract class RangedValue
    {
        protected string _name;
        public string Name
        {
            get => this._name;
        }

        protected int _defaultValue;
        public int DefaultValue
        {
            get => this._defaultValue;
            protected set => this._defaultValue = value;
        }

        protected Range<int> _range;

        protected int _value;
        public int Value
        {
            get => _value;
            set
            {
                if (_range.Contains(value))
                {
                    _value = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(this._name,
                        string.Format("Value must be in range {0} (was {1})",
                        this._range.ToString(), value));
                }
            }
        }

        protected RangedValue(string name, Range<int> range, int defaultValue, int initialValue)
        {
            this._name = name;
            this._range = range;
            this._defaultValue = defaultValue;
            this._value = initialValue;
        }
    }

    public class SignedLevel: RangedValue
    {
        public SignedLevel() : this(0) { }
        public SignedLevel(int value) : base("SignedLevel", new Range<int>(-63, 63), 0, value) { }
        public SignedLevel(byte value) : this(value - 64) { }
        public byte ToByte() => (byte)(this.Value + 64);
    }

    public class UnsignedLevel: RangedValue
    {
        public UnsignedLevel() : this(0) { }
        public UnsignedLevel(int value) : base("UnsignedLevel", new Range<int>(0, 63), 0, value) { }
        public UnsignedLevel(byte value) : this((int)value) { }
        public byte ToByte() => (byte)(this.Value);
    }

    public class PositiveLevel: RangedValue
    {
        public PositiveLevel() : this(0) { }
        public PositiveLevel(int value) : base("PositiveLevel", new Range<int>(0, 127), 0, value) { }
        public PositiveLevel(byte value) : this((int)value) { }
        public byte ToByte() => (byte)(this.Value);
    }

    public class ControlDepth: RangedValue
    {
        public ControlDepth() : this(0) { }
        public ControlDepth(int value) : base("ControlDepth", new Range<int>(-31, 31), 0, value) { }
        public ControlDepth(byte value) : this((int)value - 64) { }
        public byte ToByte() => (byte)(this.Value + 64);
    }

    public class EffectDepth: RangedValue
    {
        public EffectDepth() : this(0) { }
        public EffectDepth(int value) : base("EffectDepth", new Range<int>(0, 100), 0, value) { }
        public EffectDepth(byte value) : this((int)value) { }
        public byte ToByte() => (byte)(this.Value);
    }

    public class ResonanceLevel: RangedValue
    {
        public ResonanceLevel() : this(0) { }
        public ResonanceLevel(int value) : base("ResonanceLevel", new Range<int>(0, 7), 0, value) { }
        public ResonanceLevel(byte value) : this((int)value) { }
        public byte ToByte() => (byte)(this.Value);
    }

    public class Frequency: RangedValue
    {
        public Frequency() : this(0) { }
        public Frequency(int value) : base("Frequency", new Range<int>(-6, 6), 0, value) { }
        public Frequency(byte value) : this((int)value - 64) { }
        public byte ToByte() => (byte)(this.Value + 64);
    }

    public class BenderPitch: RangedValue
    {
        public BenderPitch() : this(0) { }
        public BenderPitch(int value) : base("BenderPitch", new Range<int>(0, 24), 0, value) { }
        public BenderPitch(byte value) : this((int)value) { }
        public byte ToByte() => (byte)(this.Value);
    }

    public class BenderCutoff: RangedValue
    {
        public BenderCutoff() : this(0) { }
        public BenderCutoff(int value) : base("BenderCutoff", new Range<int>(0, 31), 0, value) { }
        public BenderCutoff(byte value) : this((int)value) { }
        public byte ToByte() => (byte)(this.Value);
    }


    public class BenderCutoffType
    {
        private Range<byte> range;

        private byte _value;
        public byte Value
        {
            get
            {
                return _value;
            }

            set
            {
                if (range.Contains(value))
                {
                    _value = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Value",
                        string.Format("Value must be in range {0} (was {1})",
                            this.range.ToString(), value));
                }
            }
        }

        public BenderCutoffType()
        {
            this.range = new Range<byte>(0, 31);
        }

        public BenderCutoffType(byte v) : this()
        {
            this.Value = v;
        }
    }

    public class VelocityThresholdType
    {
        private Range<byte> range;

        private byte _value;
        public byte Value
        {
            get
            {
                return _value;
            }

            set
            {
                if (range.Contains(value))
                {
                    _value = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Value",
                        string.Format("Value must be in range {0} (was {1})",
                            this.range.ToString(), value));
                }
            }
        }

        public VelocityThresholdType()
        {
            this.range = new Range<byte>(31, 127);
        }

        public VelocityThresholdType(byte v) : this()
        {
            this.Value = v;
        }
    }

    public class FixedKeyType
    {
        private Range<byte> range;

        private byte _value;
        public byte Value
        {
            get
            {
                return _value;
            }

            set
            {
                if (range.Contains(value))
                {
                    _value = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Value",
                        string.Format("Value must be in range {0} (was {1})",
                            this.range.ToString(), value));
                }
            }
        }

        public FixedKeyType()
        {
            this.range = new Range<byte>(0, 108); // 0=OFF, 21 ~ 108=ON(A-1 ~ C7)
        }

        public FixedKeyType(byte v) : this()
        {
            this.Value = v;
        }
    }

    public class PatchNumberType
    {
        private Range<byte> range;

        private byte _value;
        public byte Value
        {
            get
            {
                return _value;
            }

            set
            {
                if (range.Contains(value))
                {
                    _value = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Value",
                        string.Format("Value must be in range {0} (was {1})",
                            this.range.ToString(), value));
                }
            }
        }

        public PatchNumberType()
        {
            this.range = new Range<byte>(0, 127);
        }

        public PatchNumberType(byte v) : this()
        {
            this.Value = v;
        }
    }
}
