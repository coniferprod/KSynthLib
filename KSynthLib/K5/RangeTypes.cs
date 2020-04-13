using System;

using Range.Net;


namespace KSynthLib.K5
{
    // Used for velocity depth, pressure depth, key scaling depth
    // that have the range -31 ... +31.
    public class DepthType
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

        public DepthType()
        {
            this.range = new Range<sbyte>(-31, 31);
        }

        public DepthType(sbyte v) : this()
        {
            this.Value = v;
        }
    }

    public class PositiveDepthType
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

        public PositiveDepthType()
        {
            this.range = new Range<byte>(0, 31);
        }

        public PositiveDepthType(byte v) : this()
        {
            this.Value = v;
        }
    }

    public class CoarseType
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

        public CoarseType()
        {
            this.range = new Range<sbyte>(-48, 48);
        }

        public CoarseType(sbyte v) : this()
        {
            this.Value = v;
        }
    }

    // Used for harmonic levels
    public class LevelType
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

        public LevelType()
        {
            this.range = new Range<byte>(0, 99);
        }

        public LevelType(byte v) : this()
        {
            this.Value = v;
        }
    }

    public class RateType
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

        public RateType()
        {
            this.range = new Range<sbyte>(-15, 15);
        }

        public RateType(sbyte v) : this()
        {
            this.Value = v;
        }
    }

    public class VolumeType
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

        public VolumeType()
        {
            this.range = new Range<byte>(0, 63);
        }

        public VolumeType(byte v) : this()
        {
            this.Value = v;
        }
    }

    public class HarmonicNumberType
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

        public HarmonicNumberType()
        {
            this.range = new Range<byte>(1, 63);

            // Set the value explicitly because the default value
            // of zero would be invalid.
            this._value = 1;
        }

        public HarmonicNumberType(byte v) : this()
        {
            this.Value = v;
        }
    }

    public class KeyNumberType
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

        public KeyNumberType()
        {
            this.range = new Range<byte>(0, 127);
        }

        public KeyNumberType(byte v) : this()
        {
            this.Value = v;
        }
    }

        public class EnvelopeDepthType
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

        public EnvelopeDepthType()
        {
            this.range = new Range<sbyte>(-24, 24);
        }

        public EnvelopeDepthType(sbyte v) : this()
        {
            this.Value = v;
        }
    }
    public class BenderDepthType
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

        public BenderDepthType()
        {
            this.range = new Range<byte>(0, 24);
        }

        public BenderDepthType(byte v) : this()
        {
            this.Value = v;
        }
    }

    public class EnvelopeNumberType
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

        public EnvelopeNumberType()
        {
            this.range = new Range<byte>(1, 4);
            _value = 1; // zero default would be out of range
        }

        public EnvelopeNumberType(byte v) : this()
        {
            this.Value = v;
        }
    }

}