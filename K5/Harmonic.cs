using System;
using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K5
{
    public struct Harmonic
    {
        public byte Level;
	    public bool IsModulationActive;  // true if modulation is on for the containing source
	    public byte EnvelopeNumber; // user harmonic envelope number 0/1, 1/2, 2/3 or 3/4
    }

    public struct HarmonicEnvelopeSegment
    {
        public bool IsMaxSegment;
        public byte Level; // 0~31
        public byte Rate;  // 0~31
    }

    public struct HarmonicEnvelope
    {
        public HarmonicEnvelopeSegment[] Segments;

        public bool IsActive;
	
	    public byte Effect; // 0~31 (SysEx manual says "s<x> env<y> off", maybe should be "eff"?)
    }

    public enum HarmonicSelection
    {
        Live,
        Die,
        All
    }

    public struct HarmonicModulation
    {
        public bool IsOn;  // will the selected harmonic be modulated (provided that master mod is on)
	    public byte EnvelopeNumber;  // assigns the selected harmonic to one of the four DHG envelopes
    }

    public enum HarmonicAngle
    {
        Negative = 0,
        Neutral,
        Positive
    }

    public struct HarmonicSettings
    {
	    public sbyte VelocityDepth;  // 0~±31
	    public sbyte PressureDepth;  // 0~±31
    	public sbyte KeyScalingDepth;  // 0~±31
	    public byte LFODepth; // 0~31
	    public HarmonicEnvelope[] Envelopes;
	    public bool IsModulationActive; // master modulation control - if false, all DHG modulation is defeated
	    public HarmonicSelection Selection;
	    public byte RangeFrom; // 1~63
	    public byte RangeTo; // 1~63

	    public HarmonicModulation Odd;
	    public HarmonicModulation Even;
	    public HarmonicModulation Octave;
	    public HarmonicModulation Fifth;
	    public HarmonicModulation All;

    	public HarmonicAngle Angle; // 0/-, 1/0, 1/+ (maybe should be 2/+ ?)
	    public byte HarmonicNumber; // 1~63

	    public bool IsShadowOn;  // this is in S285 bit 7

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(String.Format("*DHG 1*  MOD={0}\n\n", IsModulationActive ? "ON" : "OFF"));
            builder.Append(String.Format("*DHG 2*\n<DEPTH>\n VEL={0,3}  KS={1,3}\n PRS={2,3} LFO={3,3}\n\n",
                VelocityDepth, KeyScalingDepth, PressureDepth, LFODepth));
            builder.Append("ENV|");
            for (int i = 0; i < Source.HarmonicEnvelopeCount; i++)
            {
                builder.Append(String.Format("{0,2}|", i + 1));
            }
            builder.Append("\nACT|");
            for (int i = 0; i < Source.HarmonicEnvelopeCount; i++)
            {
                builder.Append(String.Format("{0}|", Envelopes[i].IsActive ? "ON" : "--"));
            }
            builder.Append("\nEFF|");
            for (int i = 0; i < Source.HarmonicEnvelopeCount; i++)
            {
                builder.Append(String.Format("{0,2}|", Envelopes[i].Effect));
            }
            builder.Append("\n\n");

            builder.Append("*DHG ENV*\n\nSEG |");
            for (int i = 0; i < Source.HarmonicEnvelopeSegmentCount; i++)
            {
                builder.Append(String.Format("{0,5}|", i + 1));
            }
            builder.Append("\n----|RT|LV|RT|LV|RT|LV|RT|LV|RT|LV|RT|LV|\n");

            for (int ei = 0; ei < Source.HarmonicEnvelopeCount; ei++) 
            {
                builder.Append(String.Format("ENV{0}|", ei + 1));
                for (int si = 0; si < Source.HarmonicEnvelopeSegmentCount; si++)
                {
                    HarmonicEnvelopeSegment segment = Envelopes[ei].Segments[si];
                    string levelString = segment.IsMaxSegment ? " *" : String.Format("{0,2}", segment.Level);
                    builder.Append(String.Format("{0,2}|{1}|", segment.Rate, levelString));
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
            for (int i = 0; i < Source.HarmonicEnvelopeCount; i++)
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

            for (int ei = 0; ei < Source.HarmonicEnvelopeCount; ei++)
            {
                for (int si = 0; si < Source.HarmonicEnvelopeSegmentCount; si++)
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
                for (int si = 0; si < Source.HarmonicEnvelopeSegmentCount; si++)
                {
                    b = Envelopes[ei].Segments[si].Rate;
                    data.Add(b);
                }
            }

            if (data.Count != DataLength)
            {
                System.Console.WriteLine(String.Format("WARNING: DHG length, expected = {0}, actual = {1}", DataLength, data.Count));
            }

            return data.ToArray();
        }
    }
}