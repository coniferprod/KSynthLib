using System;
using Range.Net;

namespace KSynthLib.Common
{
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
    }
}
