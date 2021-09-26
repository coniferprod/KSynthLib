using System;
using Range.Net;
using KSynthLib.Common;

namespace KSynthLib.K4
{
    /// <summary>
    /// Used for velocity depth, pressure depth, key scaling depth etc.
    /// that have the range -50 ... +50.
    /// </summary>
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

    public class SmallEffectParameterType : RangeType
    {
        private const int MIN_VALUE = 0;
        private const int MAX_VALUE = 7;
        private const int DEFAULT_VALUE = 0;

        private int currentValue;

        // Construct an effect parameter value with the default value.
        public SmallEffectParameterType()
        {
            base.SetRange(MIN_VALUE, MAX_VALUE, DEFAULT_VALUE);
            this.Value = this.DefaultValue;
        }

        // Construct an effect parameter value, clamping it if necessary.
        public SmallEffectParameterType(int value)
        {
            base.SetRange(MIN_VALUE, MAX_VALUE, DEFAULT_VALUE);
            this.Value = base.Clamp(value);
        }

        // Constructs an effect parameter number from a raw SysEx byte.
        public SmallEffectParameterType(byte value)
        {
            base.SetRange(MIN_VALUE, MAX_VALUE, DEFAULT_VALUE);
            this.Value = (int)(value);
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

        // Get the effect parameter as a SysEx byte.
        public override byte ToByte()
        {
            return (byte)(this.Value);
        }
    }

    public class LargeEffectParameterType : RangeType
    {
        private const int MIN_VALUE = 0;
        private const int MAX_VALUE = 31;
        private const int DEFAULT_VALUE = 0;

        private int currentValue;

        // Construct an effect parameter value with the default value.
        public LargeEffectParameterType()
        {
            base.SetRange(MIN_VALUE, MAX_VALUE, DEFAULT_VALUE);
            this.Value = this.DefaultValue;
        }
        // Construct an effect parameter value, clamping it if necessary.
        public LargeEffectParameterType(int value)
        {
            base.SetRange(MIN_VALUE, MAX_VALUE, DEFAULT_VALUE);
            this.Value = base.Clamp(value);
        }

        // Constructs an effect parameter number from a raw SysEx byte.
        public LargeEffectParameterType(byte value)
        {
            base.SetRange(MIN_VALUE, MAX_VALUE, DEFAULT_VALUE);
            this.Value = (int)(value);
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

        // Get the effect parameter as a SysEx byte.
        public override byte ToByte()
        {
            return (byte)(this.Value);
        }
    }

    public class KeyType : RangeType
    {
        private const int MIN_VALUE = 0;
        private const int MAX_VALUE = 127;
        private const int DEFAULT_VALUE = 60;

        private int currentValue;

        // Construct a key number, clamping if necessary.
        public KeyType(int value)
        {
            base.SetRange(MIN_VALUE, MAX_VALUE, DEFAULT_VALUE);
            this.Value = base.Clamp(value);
        }

        // Constructs a key number from a raw SysEx byte.
        public KeyType(byte value)
        {
            base.SetRange(MIN_VALUE, MAX_VALUE, DEFAULT_VALUE);
            this.Value = (int)(value);
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

        // Get the key number as a SysEx byte.
        public override byte ToByte()
        {
            return (byte)this.Value;
        }

        public string NoteName
        {
            get => PatchUtil.GetNoteName(Value);
        }
    }

    public enum VelocityCurveType
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

    public class ResonanceType : RangeType
    {
        private const int MIN_VALUE = 0;
        private const int MAX_VALUE = 7;
        private const int DEFAULT_VALUE = 0;
        private int currentValue;

        // Construct a resonance, clamping if necessary.
        public ResonanceType(int value)
        {
            base.SetRange(MIN_VALUE, MAX_VALUE, DEFAULT_VALUE);
            this.Value = base.Clamp(value);
        }

        // Constructs a resonance from a raw SysEx byte.
        public ResonanceType(byte value)
        {
            base.SetRange(MIN_VALUE, MAX_VALUE, DEFAULT_VALUE);
            this.Value = (int)(value);
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

        // Get the resonance as a SysEx byte.
        public override byte ToByte()
        {
            return (byte)this.Value;
        }
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

    public class ChannelType : RangeType
    {
        private const int MIN_VALUE = 1;
        private const int MAX_VALUE = 16;
        private const int DEFAULT_VALUE = 1;

        private int currentValue;

        // Construct a MIDI channel value with the default value.
        public ChannelType()
        {
            base.SetRange(MIN_VALUE, MAX_VALUE, DEFAULT_VALUE);
            this.Value = this.DefaultValue;
        }

        // Construct a MIDI channel value from a normal value, clamping it if necessary.
        public ChannelType(int value)
        {
            base.SetRange(MIN_VALUE, MAX_VALUE, DEFAULT_VALUE);
            this.Value = base.Clamp(value);
        }

        // Construct a MIDI channel value from a raw SysEx byte, adjusting to 1...16.
        public ChannelType(byte value)
        {
            base.SetRange(MIN_VALUE, MAX_VALUE, DEFAULT_VALUE);
            this.Value = base.Clamp((int)value + 1);
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

        // Get the MIDI channel value as a SysEx byte, adjusted to 0...15.
        public override byte ToByte()
        {
            return (byte)(this.Value - 1);
        }
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
}
