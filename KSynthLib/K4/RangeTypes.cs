using System;
using Range.Net;
using KSynthLib.Common;

namespace KSynthLib.K4
{
    // Used for velocity depth, pressure depth, key scaling depth etc.
    // that have the range -50 ... +50.
    public class DepthType : RangeType
    {
        private const int MIN_VALUE = -50;
        private const int MAX_VALUE = 50;
        private const int DEFAULT_VALUE = 0;

        private int currentValue;

        // Construct a depth with the default value.
        public DepthType()
        {
            base.SetRange(MIN_VALUE, MAX_VALUE, DEFAULT_VALUE);
            this.Value = this.DefaultValue;
        }

        // Construct a depth value from a normal value, clamping it if necessary.
        public DepthType(int value)
        {
            base.SetRange(MIN_VALUE, MAX_VALUE, DEFAULT_VALUE);
            this.Value = base.Clamp(value);
        }

        // Construct a depth value from a raw SysEx byte, adjusting as necessary.
        // For example, a depth value of zero is stored in SysEx as 50, so
        // the raw byte will need to be adjusted by -50.
        public DepthType(byte value)
        {
            base.SetRange(MIN_VALUE, MAX_VALUE, DEFAULT_VALUE);
            this.Value = base.Clamp((int)value - this.MaximumValue);
        }

        public override int Value
        {
            get => this.currentValue;

            set
            {
                if (value >= this.minimumValue && value <= this.maximumValue)
                {
                    this.currentValue = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(this.GetType().Name,
                        string.Format("Value {0} is not in range {1}...{2}",
                            value, this.minimumValue, this.maximumValue));
                }
            }
        }

        // Get the depth value as a SysEx byte, adjusted accordingly.
        // For example, a depth value of zero is stored in SysEx as 50, so
        // the depth value will need to be adjusted by +50 to get the byte in 0...100.
        public override byte ToByte()
        {
            return (byte)(this.Value + this.MaximumValue);
        }
    }

    // Level from 0...100, for example patch volume.
    public class LevelType : RangeType
    {
        private const int MIN_VALUE = 0;
        private const int MAX_VALUE = 100;
        private const int DEFAULT_VALUE = 100;

        private int currentValue;

        // Construct a level with the default value.
        public LevelType()
        {
            base.SetRange(MIN_VALUE, MAX_VALUE, DEFAULT_VALUE);
            this.Value = this.DefaultValue;
        }

        // Construct a level value from a normal value, clamping it if necessary.
        public LevelType(int value)
        {
            base.SetRange(MIN_VALUE, MAX_VALUE, DEFAULT_VALUE);
            this.Value = base.Clamp(value);
        }

        // Construct a level value from a raw SysEx byte.
        public LevelType(byte value)
        {
            base.SetRange(MIN_VALUE, MAX_VALUE, DEFAULT_VALUE);
            this.Value = (int)(value & 0x7f);
        }

        public override int Value
        {
            get => this.currentValue;

            set
            {
                if (value >= this.minimumValue && value <= this.maximumValue)
                {
                    this.currentValue = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(this.GetType().Name,
                        string.Format("Value {0} is not in range {1}...{2}",
                            value, this.minimumValue, this.maximumValue));
                }
            }
        }

        // Get the level value as a SysEx byte.
        public override byte ToByte()
        {
            return (byte)this.Value;
        }
    }

    public class PitchBendRangeType : RangeType
    {
        private const int MIN_VALUE = 0;
        private const int MAX_VALUE = 12;
        private const int DEFAULT_VALUE = 0;

        private int currentValue;

        // Construct a pitch bend range with the default value.
        public PitchBendRangeType()
        {
            base.SetRange(MIN_VALUE, MAX_VALUE, DEFAULT_VALUE);
            this.Value = this.DefaultValue;
        }

        // Construct a pitch bend range from a normal value, clamping it if necessary.
        public PitchBendRangeType(int value)
        {
            base.SetRange(MIN_VALUE, MAX_VALUE, DEFAULT_VALUE);
            this.Value = base.Clamp(value);
        }

        public override int Value
        {
            get => this.currentValue;

            set
            {
                if (value >= this.minimumValue && value <= this.maximumValue)
                {
                    this.currentValue = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(this.GetType().Name,
                        string.Format("Value {0} is not in range {1}...{2}",
                            value, this.minimumValue, this.maximumValue));
                }
            }
        }

        // Get the pitch bend range value as a SysEx byte.
        public override byte ToByte()
        {
            return (byte)this.Value;
        }
    }

    public class CoarseType : RangeType
    {
        private const int MIN_VALUE = -24;
        private const int MAX_VALUE = 24;
        private const int DEFAULT_VALUE = 0;

        private int currentValue;

        // Construct a coarse setting with the default value.
        public CoarseType()
        {
            base.SetRange(MIN_VALUE, MAX_VALUE, DEFAULT_VALUE);
            this.Value = this.DefaultValue;
        }

        // Construct a coarse setting from a normal value, clamping it if necessary.
        public CoarseType(int value)
        {
            base.SetRange(MIN_VALUE, MAX_VALUE, DEFAULT_VALUE);
            this.Value = base.Clamp(value);
        }

        // Construct a coarse setting from a raw SysEx byte.
        public CoarseType(byte value)
        {
            base.SetRange(MIN_VALUE, MAX_VALUE, DEFAULT_VALUE);
            this.Value = (int)(value & 0x3f) - this.MaximumValue;
        }

        public override int Value
        {
            get => this.currentValue;

            set
            {
                if (value >= this.minimumValue && value <= this.maximumValue)
                {
                    this.currentValue = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(this.GetType().Name,
                        string.Format("Value {0} is not in range {1}...{2}",
                            value, this.minimumValue, this.maximumValue));
                }
            }
        }

        // Get the coarse setting value as a SysEx byte.
        public override byte ToByte()
        {
            return (byte)(this.Value + this.MaximumValue);
        }
    }

    // Effect number 1...32
    public class EffectNumberType : RangeType
    {
        private const int MIN_VALUE = 1;
        private const int MAX_VALUE = 32;
        private const int DEFAULT_VALUE = 1;

        private int currentValue;

        // Construct an effect number, clamping it if necessary.
        public EffectNumberType(int value)
        {
            base.SetRange(MIN_VALUE, MAX_VALUE, DEFAULT_VALUE);
            this.Value = base.Clamp(value);
        }

        // Constructs a effect number from a raw SysEx byte.
        // Adjusts the raw value into the range 1...32.
        public EffectNumberType(byte value)
        {
            base.SetRange(MIN_VALUE, MAX_VALUE, DEFAULT_VALUE);
            this.Value = (int)(value + 1); // adjust from 0...31 to 1...32
        }

        public override int Value
        {
            get => this.currentValue;

            set
            {
                if (value >= this.minimumValue && value <= this.maximumValue)
                {
                    this.currentValue = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(this.GetType().Name,
                        string.Format("Value {0} is not in range {1}...{2}",
                            value, this.minimumValue, this.maximumValue));
                }
            }
        }

        // Get the effect number as a SysEx byte.
        public override byte ToByte()
        {
            return (byte)(this.Value - 1);  // adjust from 1...32 to 0...31 for SysEx
        }
    }

    public class EffectParameter1Type
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
                    throw new ArgumentOutOfRangeException("EffectParameter1",
                        string.Format("Value must be in range {0} (was {1})",
                        this.range.ToString(), value));
                }
            }
        }

        public EffectParameter1Type()
        {
            this.range = new Range<byte>(0, 7);
            this._value = 0;
        }

        public EffectParameter1Type(byte v) : this()
        {
            this.Value = v;  // setter throws exception for invalid value
        }
    }

    // There is no EffectParameter2Type.
    // Effect param1 and param2 are the same type (EffectParameter1Type).
    public class EffectParameter3Type
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
                    throw new ArgumentOutOfRangeException("EffectParameter3",
                        string.Format("Value must be in range {0} (was {1})",
                        this.range.ToString(), value));
                }
            }
        }

        public EffectParameter3Type()
        {
            this.range = new Range<byte>(0, 31);
            this._value = 0;
        }

        public EffectParameter3Type(byte v) : this()
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
                    throw new ArgumentOutOfRangeException("FixedKey",
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
            this.Value = (byte)(v & 0x7f);
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
        public const byte MIN_VALUE = 1;
        public const byte MAX_VALUE = 8;
        public const byte DEFAULT_VALUE = 1;

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
                    throw new ArgumentOutOfRangeException("VelocityCurve",
                        string.Format("Value must be in range {0} (was {1})",
                            this.range.ToString(), value));
                }
            }
        }

        public VelocityCurveType()
        {
            this.range = new Range<byte>(MIN_VALUE, MAX_VALUE);
            this._value = DEFAULT_VALUE;
        }

        public VelocityCurveType(byte v) : this()
        {
            this.Value = (byte)(v + 1);
        }

        public byte AsByte() => (byte)(this.Value - 1);
    }

    public class ResonanceType
    {
        public const byte MIN_VALUE = 1;
        public const byte MAX_VALUE = 8;
        public const byte DEFAULT_VALUE = 1;

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
                    throw new ArgumentOutOfRangeException("Resonance",
                        string.Format("Value must be in range {0} (was {1})",
                            this.range.ToString(), value));
                }
            }
        }

        public ResonanceType()
        {
            this.range = new Range<byte>(MIN_VALUE, MAX_VALUE);
            this._value = DEFAULT_VALUE;
        }

        public ResonanceType(byte v) : this()
        {
            this.Value = (byte)((v & 0x07) + 1);
        }

        public byte AsByte() => (byte)(this.Value - 1);
    }

    public class PanValueType : RangeType
    {
        private const int MIN_VALUE = -7;
        private const int MAX_VALUE = 7;
        private const int DEFAULT_VALUE = 0;

        private int currentValue;

        // Construct a pan value with the default value.
        public PanValueType()
        {
            base.SetRange(MIN_VALUE, MAX_VALUE, DEFAULT_VALUE);
            this.Value = this.DefaultValue;
        }

        // Construct a depth value from a normal value, clamping it if necessary.
        public PanValueType(int value)
        {
            base.SetRange(MIN_VALUE, MAX_VALUE, DEFAULT_VALUE);
            this.Value = base.Clamp(value);
        }

        // Construct a pan value from a raw SysEx byte, adjusting as necessary.
        // For example, a pan value of 7 is stored in SysEx as 14, so
        // the raw byte will need to be adjusted by -7.
        public PanValueType(byte value)
        {
            base.SetRange(MIN_VALUE, MAX_VALUE, DEFAULT_VALUE);
            this.Value = base.Clamp((int)value - this.MaximumValue);
        }

        public override int Value
        {
            get => this.currentValue;

            set
            {
                if (value >= this.minimumValue && value <= this.maximumValue)
                {
                    this.currentValue = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(this.GetType().Name,
                        string.Format("Value {0} is not in range {1}...{2}",
                            value, this.MinimumValue, this.MaximumValue));
                }
            }
        }

        // Get the pan value as a SysEx byte, adjusted accordingly.
        // For example, a pan value of +7 is stored in SysEx as 14, so
        // the pan value will need to be adjusted by +7 to get the byte into 0...14.
        public override byte ToByte()
        {
            return (byte)(this.Value + this.MaximumValue);
        }
    }

    public class MidiChannelType
    {
        public const byte MIN_VALUE = 1;
        public const byte MAX_VALUE = 16;
        public const byte DEFAULT_VALUE = 1;

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
                    throw new ArgumentOutOfRangeException("MidiChannel",
                        string.Format("Value must be in range {0} (was {1})",
                            this.range.ToString(), value));
                }
            }
        }

        public MidiChannelType()
        {
            this.range = new Range<byte>(MIN_VALUE, MAX_VALUE);
            this._value = DEFAULT_VALUE;  // default zero would be out of range
        }

        public MidiChannelType(byte v) : this()
        {
            this.Value = (byte)(v + 1);
        }

        public byte AsByte() => (byte)(this.Value - 1);
    }

    public class PatchNumberType : RangeType
    {
        private const int MIN_VALUE = 1;
        private const int MAX_VALUE = 64;
        private const int DEFAULT_VALUE = 1;

        private int currentValue;

        // Construct a patch number, clamping it if necessary.
        public PatchNumberType(int value)
        {
            base.SetRange(MIN_VALUE, MAX_VALUE, DEFAULT_VALUE);
            this.Value = base.Clamp(value);
        }

        // Constructs a patch number from a raw SysEx byte.
        // Adjusts the raw value into the range 1...64.
        public PatchNumberType(byte value)
        {
            base.SetRange(MIN_VALUE, MAX_VALUE, DEFAULT_VALUE);
            this.Value = (int)(value + 1); // adjust from 0...63 to 1...64
        }

        public override int Value
        {
            get => this.currentValue;

            set
            {
                if (value >= this.minimumValue && value <= this.maximumValue)
                {
                    this.currentValue = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(this.GetType().Name,
                        string.Format("Value {0} is not in range {1}...{2}",
                            value, this.minimumValue, this.maximumValue));
                }
            }
        }

        // Get the patch number as a SysEx byte.
        public override byte ToByte()
        {
            return (byte)(this.Value - 1);  // adjust from 1...64 to 0...63 for SysEx
        }
    }

    public class ZoneValueType
    {
        public const byte MIN_VALUE = 0;
        public const byte MAX_VALUE = 127;
        public const byte DEFAULT_VALUE = 0;

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
                    throw new ArgumentOutOfRangeException("ZoneValue",
                        string.Format("Value must be in range {0} (was {1})",
                            this.range.ToString(), value));
                }
            }
        }

        public ZoneValueType()
        {
            this.range = new Range<byte>(MIN_VALUE, MAX_VALUE);
            this._value = DEFAULT_VALUE;
        }

        public ZoneValueType(byte v) : this()
        {
            this.Value = v;  // setter throws exception for out-of-range values
        }
    }
}
