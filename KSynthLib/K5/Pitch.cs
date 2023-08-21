using System.Text;
using System.Collections.Generic;

using SyxPack;
using KSynthLib.Common;

namespace KSynthLib.K5
{
    public class PitchEnvelopeSegment  // DFG ENV
    {
        public PositiveDepth Rate; // 0~31

        public Depth Level; // 0~±31

        public PitchEnvelopeSegment()
        {
            Rate = new PositiveDepth();
            Level = new Depth();
        }

        public override string ToString()
        {
            return $"Rate={Rate} Level={Level}";
        }
    }

    public class PitchEnvelope
    {
        public static int SegmentCount = 6;

        public PitchEnvelopeSegment[] Segments;
        public bool IsLooping;

        public PitchEnvelope()
        {
            Segments = new PitchEnvelopeSegment[SegmentCount];
            for (var i = 0; i < SegmentCount; i++)
            {
                Segments[i] = new PitchEnvelopeSegment();
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("*DFG ENV*\n");
            builder.Append("    SEG  | 1 | 2 | 3 | 4 | 5 | 6 |\n");
            builder.Append("    ------------------------------\n");
            builder.Append("    RATE |");
            for (var i = 0; i < Segments.Length; i++)
            {
                builder.Append($"{Segments[i].Rate.Value,3}|");
            }
            builder.Append("\n");
            builder.Append("    LEVEL|");
            for (var i = 0; i < Segments.Length; i++)
            {
                builder.Append($"{Segments[i].Level.Value,3}|");
            }
            builder.Append("\n\n");
            builder.Append("    LOOP<3-4>=");
            builder.Append(IsLooping ? "YES" : "--");
            builder.Append("\n\n");
            return builder.ToString();
        }
    }

    public class PitchSettings : ISystemExclusiveData
    {
        public Coarse Coarse; // 0~±48

        public Depth Fine; // 0~±31

        public KeyTracking KeyTracking;  // enumeration
        public byte Key;  // the keytracking key, zero if not used
        public EnvelopeDepth EnvelopeDepth; // 0~±24
        public Depth PressureDepth; // 0~±31
        public BenderDepth BenderDepth; // 0~24
        public Depth VelocityEnvelopeDepth; // 0~±31
        public PositiveDepth LFODepth; // 0~31
        public Depth PressureLFODepth; // 0~±31

        public PitchEnvelope Envelope;

        public PitchSettings()
        {
            Coarse = new Coarse();
            Fine = new Depth();
            PressureDepth = new Depth();
            VelocityEnvelopeDepth = new Depth();
            LFODepth = new PositiveDepth();
            PressureLFODepth = new Depth();
            EnvelopeDepth = new EnvelopeDepth();
            BenderDepth = new BenderDepth();
            Envelope = new PitchEnvelope();
        }

        public override string ToString()
        {
            return string.Format(
                "*DFG*              \n\n" +
                "COARSE= {0,2}        <DEPTH>\n" +
                "FINE  = {1,2}        ENV= {2,2}-VEL {3}\n" +
                "                  PRS= {4}\n" +
                "                  LFO= {5,2}-PRS= {6,3}\n" +
                "KEY    ={7}     BND= {8}\n" +
                "FIXNO  ={9}\n\n",
                Coarse.Value, Fine.Value, EnvelopeDepth, VelocityEnvelopeDepth,
                PressureDepth.Value, LFODepth.Value, PressureLFODepth.Value,
                KeyTracking, BenderDepth.Value,
                Key) +
                Envelope;
        }

        public List<byte> Data
        {
            get
            {
                var data = new List<byte>();
                byte b = 0;
                data.Add(Coarse.ToByte());
                data.Add(Fine.ToByte());
                b = Key;  // the tracking key if fixed, 0 if track
                if (KeyTracking == KeyTracking.Fixed)
                {
                    b = b.SetBit(7);
                }
                else
                {
                    b = b.UnsetBit(7);
                }
                data.Add(b);
                data.Add(EnvelopeDepth.ToByte());
                data.Add(PressureDepth.ToByte());
                data.Add(BenderDepth.ToByte());
                data.Add(VelocityEnvelopeDepth.ToByte());
                data.Add(LFODepth.ToByte());
                data.Add(PressureLFODepth.ToByte());

                for (var i = 0; i < Source.PitchEnvelopeSegmentCount; i++)
                {
                    b = Envelope.Segments[i].Rate.ToByte();

                    // Set the envelope looping bit for the first rate only:
                    if (i == 0)
                    {
                        if (Envelope.IsLooping)
                        {
                            b = b.SetBit(7);
                        }
                    }
                    data.Add(b);
                }

                for (var i = 0; i < Source.PitchEnvelopeSegmentCount; i++)
                {
                    byte sb = Envelope.Segments[i].Level.ToByte();
                    data.Add(sb);
                }

                return data;
            }
        }

        public int DataLength => 21;
    }
}
