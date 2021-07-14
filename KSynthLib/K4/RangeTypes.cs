using System;

using Range.Net;

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
        public const sbyte MIN_VALUE = -50;
        public const sbyte MAX_VALUE = 50;
        public const sbyte DEFAULT_VALUE = 0;

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
            this.range = new Range<sbyte>(DepthType.MIN_VALUE, DepthType.MAX_VALUE);
            this._value = DepthType.DEFAULT_VALUE;
        }

        public DepthType(sbyte v) : this()
        {
            this.Value = v;
        }

        // Initializes value from raw SysEx byte.
        public DepthType(byte b) : this()
        {
            this.Value = (sbyte)((b & 0x7f) - DepthType.MAX_VALUE);
        }

        // Returns the value as a raw SysEx byte.
        public byte AsByte() => (byte)(this.Value + DepthType.MAX_VALUE);
    }

    // Level from 0...100, for example patch volume.
    public class LevelType
    {
        public const byte MIN_VALUE = 0;
        public const byte MAX_VALUE = 100;
        public const byte DEFAULT_VALUE = 100;

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
            this.range = new Range<byte>(LevelType.MAX_VALUE, LevelType.MAX_VALUE);
            this._value = LevelType.DEFAULT_VALUE;
        }

        public LevelType(byte v) : this()
        {
            this.Value = (byte)(v & 0x7f);  // setter throws exception of out-of-range values
        }
    }

    public class OutputSettingType
    {
        public const int MIN_VALUE = 0;
        public const int MAX_VALUE = 7;
        public const int DEFAULT_VALUE = 0;

        private Range<int> range;

        private int _value;
        public int Value
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

        public OutputSettingType()
        {
            this.range = new Range<int>(OutputSettingType.MIN_VALUE, OutputSettingType.MAX_VALUE);
            this._value = OutputSettingType.DEFAULT_VALUE;
        }

        public OutputSettingType(int v) : this()
        {
            this.Value = v;  // setter throws exception for invalid value
        }

        public const string OutputNames = "ABCDEFGH";
    }

    public class PitchBendRangeType
    {
        public const int MIN_VALUE = 0;
        public const int MAX_VALUE = 12;
        public const int DEFAULT_VALUE = 0;

        private Range<int> range;

        private int _value;
        public int Value
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

        public PitchBendRangeType()
        {
            this.range = new Range<int>(MIN_VALUE, MAX_VALUE);
            this._value = DEFAULT_VALUE;
        }

        public PitchBendRangeType(int v) : this()
        {
            this.Value = v;  // setter throws exception for invalid value
        }
    }

    public class CoarseType
    {
        public const sbyte MIN_VALUE = -24;
        public const sbyte MAX_VALUE = 24;
        public const sbyte DEFAULT_VALUE = 0;

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
            this.range = new Range<sbyte>(MIN_VALUE, MAX_VALUE);
            this._value = DEFAULT_VALUE;
        }

        public CoarseType(sbyte v) : this()
        {
            this.Value = v;  // setter throws exception for invalid value
        }

        public CoarseType(byte b) : this()
        {
            this.Value = (sbyte)((b & 0x3f) - MAX_VALUE);
        }

        public byte AsByte() => (byte)(this.Value + MAX_VALUE);
    }

    // Effect number 1...32
    public class EffectNumberType
    {
        public const byte MIN_VALUE = 1;
        public const byte MAX_VALUE = 32;
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
                    throw new ArgumentOutOfRangeException("Value",
                        string.Format("Value must be in range {0} (was {1})",
                        this.range.ToString(), value));
                }
            }
        }

        public EffectNumberType()
        {
            this.range = new Range<byte>(MIN_VALUE, MAX_VALUE);
            this._value = DEFAULT_VALUE;
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

    // There is no EffectParameter2Type.
    // Effect param1 and param2 are the same type (EffectParameter1Type).
    public class EffectParameter3Type
    {
        private Range<int> range;

        private int _value;
        public int Value
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

        public EffectParameter3Type()
        {
            this.range = new Range<int>(0, 31);
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
                    throw new ArgumentOutOfRangeException("Value",
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
            this.Value = v;
        }
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
                    throw new ArgumentOutOfRangeException("Value",
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
            this.Value = v;
        }
    }

    public class WaveNumberType
    {
        public const ushort MIN_VALUE = 1;
        public const ushort MAX_VALUE = 256;
        public const ushort DEFAULT_VALUE = 1;

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
            this.range = new Range<ushort>(MIN_VALUE, MAX_VALUE);
            this._value = DEFAULT_VALUE;
        }

        public WaveNumberType(ushort v) : this()
        {
            this.Value = v;  // setter throws exception for out-of-range values
        }

        public (byte, byte) ConvertToHighAndLow()
        {
            // Convert wave number to an 8-bit binary string representation:
            string waveBitString = Convert.ToString(Value, 2).PadLeft(8, '0');

            // Get top bit, convert it to byte and use it as the MSB:
            byte high = Convert.ToByte(waveBitString.Substring(0, 1), 2);

            // Get all but the top bit, convert it to byte and use it as the LSB:
            byte low = Convert.ToByte(waveBitString.Substring(1), 2);

            return (high, low);
        }
    }

    public class PanValueType
    {
        public const sbyte MIN_VALUE = -7;
        public const sbyte MAX_VALUE = 7;
        public const sbyte DEFAULT_VALUE = 0;

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

        public PanValueType()
        {
            this.range = new Range<sbyte>(MIN_VALUE, MAX_VALUE);
            this._value = DEFAULT_VALUE;
        }

        public PanValueType(sbyte v) : this()
        {
            this.Value = v;
        }
        public PanValueType(byte b) : this()
        {
            this.Value = (sbyte)(b - MAX_VALUE);
        }

        public byte AsByte() => (byte)(this.Value + MAX_VALUE);
    }

    public class SendValueType
    {
        public const byte MIN_VALUE = 0;
        public const byte MAX_VALUE = 100;
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
                    throw new ArgumentOutOfRangeException("Value",
                        string.Format("Value must be in range {0} (was {1})",
                            this.range.ToString(), value));
                }
            }
        }

        public SendValueType()
        {
            this.range = new Range<byte>(MIN_VALUE, MAX_VALUE);  // manual says 0...100, SysEx spec says 0...99
            this._value = DEFAULT_VALUE;
        }

        public SendValueType(byte v) : this()
        {
            this.Value = v;  // setter throws exception for out-of-range values
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
                    throw new ArgumentOutOfRangeException("Value",
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
            this.Value = v;
        }
    }

    public class PatchNumberType
    {
        // TODO: Should this have 1...64? Or a more elaborate "A-1"..."D-16" etc.?
        public const byte MIN_VALUE = 0;
        public const byte MAX_VALUE = 63;
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
                    throw new ArgumentOutOfRangeException("Value",
                        string.Format("Value must be in range {0} (was {1})",
                            this.range.ToString(), value));
                }
            }
        }

        public PatchNumberType()
        {
            this.range = new Range<byte>(MIN_VALUE, MAX_VALUE);
            this._value = MIN_VALUE;
        }

        public PatchNumberType(byte v) : this()
        {
            this.Value = v;  // setter throws exception for out-of-range values
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
                    throw new ArgumentOutOfRangeException("Value",
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
