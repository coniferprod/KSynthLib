using System;
using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K5
{
    public class Harmonic
    {
        private LevelType _level;
        public byte Level
        {
            get => _level.Value;
            set => _level.Value = value;
        }

	    public bool IsModulationActive;  // true if modulation is on for the containing source

        private EnvelopeNumberType _envelopeNumber;
	    public byte EnvelopeNumber // user harmonic envelope number 0/1, 1/2, 2/3 or 3/4
        {
            get => _envelopeNumber.Value;
            set => _envelopeNumber.Value = value;
        }

        public Harmonic()
        {
            _level = new LevelType();
            IsModulationActive = false;
            _envelopeNumber = new EnvelopeNumberType(1);
        }
    }

    public class HarmonicEnvelopeSegment
    {
        public bool IsMaxSegment;

        private PositiveDepthType _level; // 0~31
        public byte Level
        {
            get => _level.Value;
            set => _level.Value = value;
        }

        private PositiveDepthType _rate; // 0~31
        public byte Rate
        {
            get => _rate.Value;
            set => _rate.Value = value;
        }

        public HarmonicEnvelopeSegment()
        {
            IsMaxSegment = false;
            _level = new PositiveDepthType();
            _rate = new PositiveDepthType();
        }

        public HarmonicEnvelopeSegment(byte level, byte rate, bool isMaxSeg) : this()
        {
            Level = level;
            Rate = rate;
            IsMaxSegment = isMaxSeg;
        }

        public override string ToString()
        {
            return $"level={Level} rate={Rate} isMaxSeg={IsMaxSegment}";
        }
    }

    public class HarmonicEnvelope
    {
        public const int SegmentCount = 6;

        public HarmonicEnvelopeSegment[] Segments;

        public bool IsActive;

        private PositiveDepthType _effect;
        // 0~31 (SysEx manual says "s<x> env<y> off", maybe should be "eff"?)
	    public byte Effect
        {
            get => _effect.Value;
            set => _effect.Value = value;
        }

        public HarmonicEnvelope()
        {
            IsActive = false;

            Segments = new HarmonicEnvelopeSegment[SegmentCount];
            for (int i = 0; i < SegmentCount; i++)
            {
                Segments[i] = new HarmonicEnvelopeSegment();
            }

            _effect = new PositiveDepthType();
        }
    }

    public enum HarmonicSelection
    {
        Live,
        Die,
        All
    }

    public class HarmonicModulation
    {
        public bool IsOn;  // will the selected harmonic be modulated (provided that master mod is on)

        private EnvelopeNumberType _envelopeNumber;
	    public byte EnvelopeNumber  // assigns the selected harmonic to one of the four DHG envelopes
        {
            get => _envelopeNumber.Value;
            set => _envelopeNumber.Value = value;
        }

        public HarmonicModulation()
        {
            IsOn = false;
            _envelopeNumber = new EnvelopeNumberType(1);
        }
    }

    public enum HarmonicAngle
    {
        Negative = 0,
        Neutral,
        Positive
    }

    public class HarmonicSettings
    {
        public const int HarmonicEnvelopeCount = 4;

        private DepthType _velocityDepth; // 0~±31
        public sbyte VelocityDepth
        {
            get => _velocityDepth.Value;
            set => _velocityDepth.Value = value;
        }

        private DepthType _pressureDepth; // 0~±31
	    public sbyte PressureDepth
        {
            get => _pressureDepth.Value;
            set => _pressureDepth.Value = value;
        }

        private DepthType _keyScalingDepth; // 0~±31
    	public sbyte KeyScalingDepth
        {
            get => _keyScalingDepth.Value;
            set => _keyScalingDepth.Value = value;
        }

        private PositiveDepthType _lfoDepth; // 0~31
	    public byte LFODepth
        {
            get => _lfoDepth.Value;
            set => _lfoDepth.Value = value;
        }

	    public HarmonicEnvelope[] Envelopes;
	    public bool IsModulationActive; // master modulation control - if false, all DHG modulation is defeated
	    public HarmonicSelection Selection;

        private HarmonicNumberType _rangeFrom; // 1~63
	    public byte RangeFrom
        {
            get => _rangeFrom.Value;
            set => _rangeFrom.Value = value;
        }

        private HarmonicNumberType _rangeTo; // 1~63
	    public byte RangeTo
        {
            get => _rangeTo.Value;
            set => _rangeTo.Value = value;
        }

	    public HarmonicModulation Odd;
	    public HarmonicModulation Even;
	    public HarmonicModulation Octave;
	    public HarmonicModulation Fifth;
	    public HarmonicModulation All;

    	public HarmonicAngle Angle; // 0/-, 1/0, 1/+ (maybe should be 2/+ ?)

        private HarmonicNumberType _harmonicNumber; // 1~63
	    public byte HarmonicNumber
        {
            get => _harmonicNumber.Value;
            set => _harmonicNumber.Value = value;
        }

	    public bool IsShadowOn;  // this is in S285 bit 7

        public HarmonicSettings()
        {
            _velocityDepth = new DepthType();
            _pressureDepth = new DepthType();
            _keyScalingDepth = new DepthType();
            _lfoDepth = new PositiveDepthType();
            _rangeFrom = new HarmonicNumberType();
            _rangeTo = new HarmonicNumberType();

            Envelopes = new HarmonicEnvelope[HarmonicEnvelopeCount];
            for (int i = 0; i < HarmonicEnvelopeCount; i++)
            {
                Envelopes[i] = new HarmonicEnvelope();
            }

            _harmonicNumber = new HarmonicNumberType(1);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(string.Format("*DHG 1*  MOD={0}\n\n", IsModulationActive ? "ON" : "OFF"));
            builder.Append(string.Format("*DHG 2*\n<DEPTH>\n VEL={0,3}  KS={1,3}\n PRS={2,3} LFO={3,3}\n\n",
                VelocityDepth, KeyScalingDepth, PressureDepth, LFODepth));
            builder.Append("ENV|");
            for (int i = 0; i < HarmonicSettings.HarmonicEnvelopeCount; i++)
            {
                builder.Append(string.Format("{0,2}|", i + 1));
            }
            builder.Append("\nACT|");
            for (int i = 0; i < HarmonicSettings.HarmonicEnvelopeCount; i++)
            {
                builder.Append(string.Format("{0}|", Envelopes[i].IsActive ? "ON" : "--"));
            }
            builder.Append("\nEFF|");
            for (int i = 0; i < HarmonicSettings.HarmonicEnvelopeCount; i++)
            {
                builder.Append(string.Format("{0,2}|", Envelopes[i].Effect));
            }
            builder.Append("\n\n");

            builder.Append("*DHG ENV*\n\nSEG |");
            for (int i = 0; i < HarmonicEnvelope.SegmentCount; i++)
            {
                builder.Append(string.Format("{0,5}|", i + 1));
            }
            builder.Append("\n----|RT|LV|RT|LV|RT|LV|RT|LV|RT|LV|RT|LV|\n");

            for (int ei = 0; ei < HarmonicSettings.HarmonicEnvelopeCount; ei++)
            {
                builder.Append(string.Format("ENV{0}|", ei + 1));
                for (int si = 0; si < HarmonicEnvelope.SegmentCount; si++)
                {
                    HarmonicEnvelopeSegment segment = Envelopes[ei].Segments[si];
                    string levelString = segment.IsMaxSegment ? " *" : String.Format("{0,2}", segment.Level);
                    builder.Append(string.Format("{0,2}|{1}|", segment.Rate, levelString));
                }
                builder.Append("\n");
            }
            builder.Append("MAX  .... SHADOW=");
            builder.Append(IsShadowOn ? "ON" : "--");
            builder.Append("\n\n");

            return builder.ToString();
        }

        const int DataLength = 16 + 4 * 12; // without the 63 harmonic levels

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();

            data.Add(VelocityDepth.ToByte());
            data.Add(PressureDepth.ToByte());
            data.Add(KeyScalingDepth.ToByte());
            data.Add(LFODepth);

            byte b = 0;
            for (int i = 0; i < HarmonicSettings.HarmonicEnvelopeCount; i++)
            {
                b = Envelopes[i].Effect;
                if (Envelopes[i].IsActive)
                {
                    b = b.SetBit(7);
                }
                data.Add(b);
            }

            b = (byte)Selection;
            if (IsModulationActive)
            {
                b = b.SetBit(7);
            }
            data.Add(b);

            data.Add(RangeFrom);
            data.Add(RangeTo);

            byte lowNybble = (byte)(Even.EnvelopeNumber - 1);
            if (Even.IsOn)
            {
                lowNybble = lowNybble.SetBit(3);
            }
            byte highNybble = (byte)(Odd.EnvelopeNumber - 1);
            if (Odd.IsOn)
            {
                highNybble = highNybble.SetBit(3);
            }
            b = Util.ByteFromNybbles(highNybble, lowNybble);
            data.Add(b);

            lowNybble = (byte)(Fifth.EnvelopeNumber - 1);
            if (Fifth.IsOn)
            {
                lowNybble = lowNybble.SetBit(3);
            }
            highNybble = (byte)(Octave.EnvelopeNumber - 1);
            if (Octave.IsOn)
            {
                highNybble = highNybble.SetBit(3);
            }
            b = Util.ByteFromNybbles(highNybble, lowNybble);
            data.Add(b);

            lowNybble = 0;
            highNybble = (byte)(All.EnvelopeNumber - 1);
            if (All.IsOn)
            {
                highNybble = highNybble.SetBit(3);
            }
            b = Util.ByteFromNybbles(highNybble, lowNybble);
            data.Add(b);

            data.Add((byte)Angle);
            data.Add(HarmonicNumber);

            for (int ei = 0; ei < HarmonicSettings.HarmonicEnvelopeCount; ei++)
            {
                for (int si = 0; si < HarmonicEnvelope.SegmentCount; si++)
                {
                    b = Envelopes[ei].Segments[si].Level;
                    if (Envelopes[ei].Segments[si].IsMaxSegment)
                    {
                        b = b.SetBit(6);
                    }
                    else
                    {
                        b = b.UnsetBit(6);
                    }
                    if (ei == 0)
                    {
                        if (IsShadowOn)
                        {
                            b = b.SetBit(7);
                        }
                        else
                        {
                            b = b.UnsetBit(7);
                        }
                    }
                    data.Add(b);
                }
                for (int si = 0; si < HarmonicEnvelope.SegmentCount; si++)
                {
                    b = Envelopes[ei].Segments[si].Rate;
                    data.Add(b);
                }
            }

            if (data.Count != DataLength)
            {
                Console.WriteLine($"WARNING: DHG length, expected = {DataLength}, actual = {data.Count} bytes");
            }

            return data.ToArray();
        }
    }
}