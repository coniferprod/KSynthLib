using System;
using Range.Net;

namespace KSynthLib.Common
{
    public abstract class RangeType
    {
        protected int minimumValue;
        public int MinimumValue
        {
            get => this.minimumValue;
            protected set => this.minimumValue = value;
        }

        protected int maximumValue;
        public int MaximumValue
        {
            get => this.maximumValue;
            protected set => this.maximumValue = value;
        }

        public abstract int Value
        {
            get;
            set;
        }

        protected int defaultValue;
        public int DefaultValue
        {
            get => this.defaultValue;
            protected set => this.defaultValue = value;
        }

        public RangeType()
        {
            this.SetRange(0, 99, 0);
        }

        protected void SetRange(int minimumValue, int maximumValue, int defaultValue)
        {
            this.MinimumValue = minimumValue;
            this.MaximumValue = maximumValue;
            this.DefaultValue = defaultValue;
        }

        protected int Clamp(int value)
        {
            int newValue = value;
            if (value < this.MinimumValue)
            {
                newValue = this.MinimumValue;
            }
            else if (value > this.maximumValue)
            {
                newValue = this.MaximumValue;
            }
            return newValue;
        }

        // Returns the value as a raw SysEx byte.
        public abstract byte ToByte();

        public override string ToString()
        {
            return string.Format("{0}", this.Value);
        }
    }

    public class EightLevelType
    {
        private Range<int> range;

        private int _value;
        public int Value
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

        public EightLevelType()
        {
            this.range = new Range<int>(1, 8);
            this._value = 1;
        }

        public EightLevelType(int v) : this()
        {
            this.Value = v;  // setter throws exception for invalid value
        }
    }

    public class PitchBendType
    {
        private Range<int> range;

        private int _value;
        public int Value
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

        public PitchBendType()
        {
            this.range = new Range<int>(0, 12);
            this._value = 0;
        }

        public PitchBendType(int v) : this()
        {
            this.Value = v;  // setter throws exception for invalid value
        }
    }

    public class CoarseType
    {
        private Range<int> range;

        private int _value;
        public int Value
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

        public CoarseType()
        {
            this.range = new Range<int>(-24, 24);
            this._value = 0;
        }

        public CoarseType(int v) : this()
        {
            this.Value = v;  // setter throws exception for invalid value
        }

        public CoarseType(byte b) : this()
        {
            this.Value = b - 24;
        }

        public byte Byte => (byte)(this.Value + 24);
    }

    public class EffectParameter1Type
    {
        private Range<int> range;

        private int _value;
        public int Value
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

        public EffectParameter1Type()
        {
            this.range = new Range<int>(0, 7);
            this._value = 0;
        }

        public EffectParameter1Type(int v) : this()
        {
            this.Value = v;  // setter throws exception for invalid value
        }
    }

    public class EffectParameter3Type
    {
        private Range<int> range;

        private int _value;
        public int Value
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

        public EffectParameter3Type()
        {
            this.range = new Range<int>(0, 30);  // or 31?
            this._value = 0;
        }

        public EffectParameter3Type(int v) : this()
        {
            this.Value = v;  // setter throws exception for invalid value
        }
    }
}
