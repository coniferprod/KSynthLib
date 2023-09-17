using System;
using Range.Net;

namespace KSynthLib.Common
{
    public interface IPatch
    {
        byte Checksum { get; }
        string Name { get; set; }
    }

    public abstract class RangedValue
    {
        protected string _name;
        public string Name
        {
            get => this._name;
            protected set => this._name = value;
        }

        protected int _defaultValue;
        public int DefaultValue
        {
            get => this._defaultValue;
            protected set => this._defaultValue = value;
        }

        protected Range<int> _range;

        public int MinValue => _range.Minimum;
        public int MaxValue => _range.Maximum;

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

        public override string ToString()
        {
            return $"{this.Value}";
        }
    }

    /// <summary>/// MIDI channel 1 ... 16</summary>
    public class Channel: RangedValue
    {
        public Channel() : this(1) { }
        public Channel(int value) : base("Channel", new Range<int>(1, 16), 1, value) { }
        public Channel(byte value) : this(value + 1) { }  // adjust from 0...15 to 1...16
        public byte ToByte() => (byte)(this.Value - 1);  // 1~16 to 0~15
    }
}
