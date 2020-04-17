using System;
using KSynthLib.Common;

namespace KSynthLib.K4
{
    //
    // Here are a couple of ranged types.
    // These could probably be made generic and/or use an interface.
    //

    // Used for velocity depth, pressure depth, key scaling depth
    // that have the range -50 ... +50.
    public class DepthType
    {
        private Range<sbyte> range;

        private sbyte _value;
        public sbyte Value
        {
            get => _value;

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
            this.range = new Range<sbyte>(-50, 50);
        }

        public DepthType(sbyte v) : this()
        {
            this.Value = v;
        }
    }

    // Level from 0...100, for example patch volume.
    public class LevelType
    {
        private Range<byte> range;

        private byte _value;
        public byte Value
        {
            get => _value;

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
            this.range = new Range<byte>(0, 100);
            this._value = 0;
        }

        public LevelType(byte v) : this()
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

    public class OutputSettingType
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

        public OutputSettingType()
        {
            this.range = new Range<int>(0, 7);
            this._value = 1;
        }

        public OutputSettingType(int v) : this()
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
        private Range<sbyte> range;

        private sbyte _value;
        public sbyte Value
        {
            get => _value;

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
            this.range = new Range<sbyte>(-24, 24);
            this._value = 0;
        }

        public CoarseType(sbyte v) : this()
        {
            this.Value = v;  // setter throws exception for invalid value
        }
    }

    // Effect number 1...32
    public class EffectNumberType
    {
        private Range<byte> range;

        private byte _value;
        public byte Value
        {
            get => _value;

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
            this.range = new Range<byte>(1, 32);
            this._value = 1; // default value zero would be out of range
        }

        public EffectNumberType(byte v) : this()
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

    public class FixedKeyType
    {
        private Range<byte> range;

        private byte _value;
        public byte Value
        {
            get => _value;

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
            this.range = new Range<byte>(0, 115); // 0 ~ 115 / C-1 ~ G8
        }

        public FixedKeyType(byte v) : this()
        {
            this.Value = v;
        }

        public string NoteName
        {
            get
            {
                string[] notes = new string[] {"A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#"};
                int octave = _value / 12 + 1;
                string name = notes[_value % 12];
                return name + octave;
            }
        }
    }

    // Kawai K4 velocity curve value: 1~8 (in SysEx as 0~7)
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
            this.range = new Range<byte>(1, 8);
            this._value = 1;  // default zero would be out of range
        }

        public VelocityCurveType(byte v) : this()
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
            this.range = new Range<byte>(1, 8);
            this._value = 1;  // default zero would be out of range
        }

        public ResonanceType(byte v) : this()
        {
            this.Value = v;
        }
    }

    public class WaveNumberType
    {
        private Range<ushort> range;

        private ushort _value;
        public ushort Value
        {
            get => _value;

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

        public WaveNumberType()
        {
            this.range = new Range<ushort>(1, 256);
            this._value = 1;  // default zero would be out of range
        }

        public WaveNumberType(ushort v) : this()
        {
            this.Value = v;  // setter throws exception of out-of-range values
        }
    }
}
