using System;
using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K5
{
    public class Harmonic
    {
        public Level Level;
	    public bool IsModulationActive;  // true if modulation is on for the containing source

        public EnvelopeNumber EnvelopeNumber; // user harmonic envelope number 0/1, 1/2, 2/3 or 3/4

        public Harmonic()
        {
            Level = new Level();
            IsModulationActive = false;
            EnvelopeNumber = new EnvelopeNumber(1);
        }
    }

    public class HarmonicEnvelopeSegment
    {
        public bool IsMaxSegment;

        public PositiveDepth Level; // 0~31
        public PositiveDepth Rate; // 0~31

        public HarmonicEnvelopeSegment()
        {
            IsMaxSegment = false;
            Level = new PositiveDepth();
            Rate = new PositiveDepth();
        }

        public HarmonicEnvelopeSegment(byte level, byte rate, bool isMaxSegment) : this()
        {
            this.Level = new PositiveDepth(level);
            this.Rate = new PositiveDepth(rate);
            this.IsMaxSegment = isMaxSegment;
        }

        public override string ToString()
        {
            return $"level={this.Level.Value} rate={this.Rate.Value} isMaxSegment={IsMaxSegment}";
        }
    }

    public class HarmonicEnvelope
    {
        public const int SegmentCount = 6;

        public HarmonicEnvelopeSegment[] Segments;

        public bool IsActive;

        public PositiveDepth Effect;
        // 0~31 (SysEx manual says "s<x> env<y> off", maybe should be "eff"?)

        public HarmonicEnvelope()
        {
            IsActive = false;

            Segments = new HarmonicEnvelopeSegment[SegmentCount];
            for (var i = 0; i < SegmentCount; i++)
            {
                Segments[i] = new HarmonicEnvelopeSegment();
            }

            Effect = new PositiveDepth();
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

        public EnvelopeNumber EnvelopeNumber; // assigns the selected harmonic to one of the four DHG envelopes

        public HarmonicModulation()
        {
            IsOn = false;
            EnvelopeNumber = new EnvelopeNumber(1);
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

        public Depth VelocityDepth; // 0~±31
        public Depth PressureDepth; // 0~±31
        public Depth KeyScalingDepth; // 0~±31
        public PositiveDepth LFODepth; // 0~31

	    public HarmonicEnvelope[] Envelopes;
	    public bool IsModulationActive; // master modulation control - if false, all DHG modulation is defeated
	    public HarmonicSelection Selection;

        public HarmonicNumber RangeFrom; // 1~63
        public HarmonicNumber RangeTo; // 1~63

	    public HarmonicModulation Odd;
	    public HarmonicModulation Even;
	    public HarmonicModulation Octave;
	    public HarmonicModulation Fifth;
	    public HarmonicModulation All;

    	public HarmonicAngle Angle; // 0/-, 1/0, 1/+ (maybe should be 2/+ ?)

        public HarmonicNumber HarmonicNumber; // 1~63

	    public bool IsShadowOn;  // this is in S285 bit 7

        public HarmonicSettings()
        {
            VelocityDepth = new Depth();
            PressureDepth = new Depth();
            KeyScalingDepth = new Depth();
            LFODepth = new PositiveDepth();
            RangeFrom = new HarmonicNumber();
            RangeTo = new HarmonicNumber();

            Envelopes = new HarmonicEnvelope[HarmonicEnvelopeCount];
            for (var i = 0; i < HarmonicEnvelopeCount; i++)
            {
                Envelopes[i] = new HarmonicEnvelope();
            }

            HarmonicNumber = new HarmonicNumber(1);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(string.Format("*DHG 1*  MOD={0}\n\n", IsModulationActive ? "ON" : "OFF"));
            builder.Append(string.Format("*DHG 2*\n<DEPTH>\n VEL={0,3}  KS={1,3}\n PRS={2,3} LFO={3,3}\n\n",
                VelocityDepth.Value, KeyScalingDepth.Value, PressureDepth.Value, LFODepth.Value));
            builder.Append("ENV|");
            for (var i = 0; i < HarmonicSettings.HarmonicEnvelopeCount; i++)
            {
                builder.Append(string.Format("{0,2}|", i + 1));
            }
            builder.Append("\nACT|");
            for (var i = 0; i < HarmonicSettings.HarmonicEnvelopeCount; i++)
            {
                builder.Append(string.Format("{0}|", Envelopes[i].IsActive ? "ON" : "--"));
            }
            builder.Append("\nEFF|");
            for (var i = 0; i < HarmonicSettings.HarmonicEnvelopeCount; i++)
            {
                builder.Append(string.Format("{0,2}|", Envelopes[i].Effect));
            }
            builder.Append("\n\n");

            builder.Append("*DHG ENV*\n\nSEG |");
            for (var i = 0; i < HarmonicEnvelope.SegmentCount; i++)
            {
                builder.Append(string.Format("{0,5}|", i + 1));
            }
            builder.Append("\n----|RT|LV|RT|LV|RT|LV|RT|LV|RT|LV|RT|LV|\n");

            for (var ei = 0; ei < HarmonicSettings.HarmonicEnvelopeCount; ei++)
            {
                builder.Append(string.Format("ENV{0}|", ei + 1));
                for (var si = 0; si < HarmonicEnvelope.SegmentCount; si++)
                {
                    HarmonicEnvelopeSegment segment = Envelopes[ei].Segments[si];
                    var levelString = segment.IsMaxSegment ? " *" : string.Format("{0,2}", segment.Level);
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
            var data = new List<byte>();

            data.Add(VelocityDepth.ToByte());
            data.Add(PressureDepth.ToByte());
            data.Add(KeyScalingDepth.ToByte());
            data.Add(LFODepth.ToByte());

            byte b = 0;
            for (var i = 0; i < HarmonicSettings.HarmonicEnvelopeCount; i++)
            {
                b = Envelopes[i].Effect.ToByte();
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

            data.Add(RangeFrom.ToByte());
            data.Add(RangeTo.ToByte());

            byte lowNybble = Even.EnvelopeNumber.ToByte();
            if (Even.IsOn)
            {
                lowNybble = lowNybble.SetBit(3);
            }
            byte highNybble = Odd.EnvelopeNumber.ToByte();
            if (Odd.IsOn)
            {
                highNybble = highNybble.SetBit(3);
            }
            b = Util.ByteFromNybbles(highNybble, lowNybble);
            data.Add(b);

            lowNybble = Fifth.EnvelopeNumber.ToByte();
            if (Fifth.IsOn)
            {
                lowNybble = lowNybble.SetBit(3);
            }
            highNybble = Octave.EnvelopeNumber.ToByte();
            if (Octave.IsOn)
            {
                highNybble = highNybble.SetBit(3);
            }
            b = Util.ByteFromNybbles(highNybble, lowNybble);
            data.Add(b);

            lowNybble = 0;
            highNybble = All.EnvelopeNumber.ToByte();
            if (All.IsOn)
            {
                highNybble = highNybble.SetBit(3);
            }
            b = Util.ByteFromNybbles(highNybble, lowNybble);
            data.Add(b);

            data.Add((byte)Angle);
            data.Add(HarmonicNumber.ToByte());

            for (var ei = 0; ei < HarmonicSettings.HarmonicEnvelopeCount; ei++)
            {
                for (var si = 0; si < HarmonicEnvelope.SegmentCount; si++)
                {
                    b = Envelopes[ei].Segments[si].Level.ToByte();
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
                for (var si = 0; si < HarmonicEnvelope.SegmentCount; si++)
                {
                    b = Envelopes[ei].Segments[si].Rate.ToByte();
                    data.Add(b);
                }
            }

            if (data.Count != DataLength)
            {
                Console.Error.WriteLine($"WARNING: DHG length, expected = {DataLength}, actual = {data.Count} bytes");
            }

            return data.ToArray();
        }
    }
}
