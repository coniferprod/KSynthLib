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

        private int currentValue;
        public int Value
        {
            get 
            {
                return currentValue;
            }

            set
            {
                if (range.Contains(value))
                {
                    currentValue = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Value",
                        string.Format("Value must be in range {0}", this.range.ToString()));
                }
            }
        }

        public DepthType()
        {
            this.range = new Range<int>(-50, 50);
        }

        public DepthType(int v)
        {
            this.range = new Range<int>(-50, 50);  // TODO: redundancy?
            this.currentValue = v;
        }
    }

    public class LevelType
    {
        private Range<int> range;

        private int currentValue;
        public int Value
        {
            get 
            {
                return currentValue;
            }

            set
            {
                if (range.Contains(value))
                {
                    currentValue = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Value",
                        string.Format("Value must be in range {0}", this.range.ToString()));
                }
            }
        }

        public LevelType()
        {
            this.range = new Range<int>(0, 100);
        }

        public LevelType(int v)
        {
            this.range = new Range<int>(0, 100);  // TODO: redundancy?
            this.currentValue = v;
        }
    }

}
