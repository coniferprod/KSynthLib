using System;

using Range.Net;


namespace KSynthLib.K5000
{
    public class SignedLevelType
    {
        private Range<sbyte> range;

        private sbyte _value;
        public sbyte Value
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

        public SignedLevelType()
        {
            this.range = new Range<sbyte>(-63, 63);
        }

        public SignedLevelType(sbyte v) : this()
        {
            this.Value = v;
        }
    }

    public class UnsignedLevelType
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

        public UnsignedLevelType()
        {
            this.range = new Range<byte>(0, 63);
        }

        public UnsignedLevelType(byte v) : this()
        {
            this.Value = v;
        }
    }

    public class PositiveLevelType
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

        public PositiveLevelType()
        {
            this.range = new Range<byte>(0, 127);
        }

        public PositiveLevelType(byte v) : this()
        {
            this.Value = v;
        }
    }

    public class VelocityCurveType
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

        public VelocityCurveType()
        {
            this.range = new Range<byte>(1, 12);
        }

        public VelocityCurveType(byte v) : this()
        {
            this.Value = v;
        }
    }

    public class EffectControlDepthType
    {
        private Range<sbyte> range;

        private sbyte _value;
        public sbyte Value
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

        public EffectControlDepthType()
        {
            this.range = new Range<sbyte>(-32, 32);
        }

        public EffectControlDepthType(sbyte v) : this()
        {
            this.Value = v;
        }
    }

    public class EffectDepthType
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

        public EffectDepthType()
        {
            this.range = new Range<byte>(0, 100);
        }

        public EffectDepthType(byte v) : this()
        {
            this.Value = v;
        }
    }

    public class MacroDepthType
    {
        private Range<sbyte> range;

        private sbyte _value;
        public sbyte Value
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

        public MacroDepthType()
        {
            this.range = new Range<sbyte>(-31, 31);
        }

        public MacroDepthType(sbyte v) : this()
        {
            this.Value = v;
        }
    }

    public class FreqType
    {
        private Range<sbyte> range;

        private sbyte _value;
        public sbyte Value
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

        public FreqType()
        {
            this.range = new Range<sbyte>(-6, 6);
        }

        public FreqType(sbyte v) : this()
        {
            this.Value = v;
        }
    }

    public class ResonanceType
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

        public ResonanceType()
        {
            this.range = new Range<byte>(0, 7);
        }

        public ResonanceType(byte v) : this()
        {
            this.Value = v;
        }
    }

    public class BenderPitchType
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

        public BenderPitchType()
        {
            this.range = new Range<byte>(0, 24);
        }

        public BenderPitchType(byte v) : this()
        {
            this.Value = v;
        }
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

    public class EffectPathType
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

        public EffectPathType()
        {
            this.range = new Range<byte>(0, 3);
        }

        public EffectPathType(byte v) : this()
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

    public class EffectAlgorithmType
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

        public EffectAlgorithmType()
        {
            this.range = new Range<byte>(1, 4);
            _value = 1;  // default zero would be out of range
        }

        public EffectAlgorithmType(byte v) : this()
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
