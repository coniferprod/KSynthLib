using System;
using Range.Net;

namespace KSynthLib.Common
{
    //
    // Here are a couple of ranged types.
    // These could probably be made generic and/or use an interface.
    //

    // Used for velocity depth, pressure depth, key scaling depth
    // that have the range -50 ... +50.
    public class DepthType
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

        public DepthType()
        {
            this.range = new Range<int>(-50, 50);
        }

        public DepthType(int v) : this()
        {
            this.Value = v;
        }
    }

    public class LevelType
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

        public LevelType()
        {
            this.range = new Range<int>(0, 100);
            this._value = 0;
        }

        public LevelType(int v) : this()
        {
            this.Value = v;  // setter throws exception of out-of-range values
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

        public byte AsByte()
        {
            return (byte)(this.Value + 24);
        }
    }

    public class EffectNumberType
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

        public EffectNumberType()
        {
            this.range = new Range<int>(1, 32);
            this._value = 0;
        }

        public EffectNumberType(int v) : this()
        {
            this.Value = v;  // setter throws exception for invalid value
        }
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
