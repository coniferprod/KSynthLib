using System;
using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;


namespace KSynthLib.K5
{
    public class PitchEnvelopeSegment  // DFG ENV
    {
        private PositiveDepthType _rate; // 0~31
        public byte Rate
        {
            get => _rate.Value;
            set => _rate.Value = value;
        }

        private DepthType _level; // 0~±31
        public sbyte Level
        {
            get => _level.Value;
            set => _level.Value = value;
        }

        public PitchEnvelopeSegment()
        {
            _rate = new PositiveDepthType();
            _level = new DepthType();
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
            for (int i = 0; i < SegmentCount; i++)
            {
                Segments[i] = new PitchEnvelopeSegment();
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("*DFG ENV*\n");
            builder.Append("    SEG  | 1 | 2 | 3 | 4 | 5 | 6 |\n");
            builder.Append("    ------------------------------\n");
            builder.Append("    RATE |");
            for (int i = 0; i < Segments.Length; i++)
            {
                builder.Append($"{Segments[i].Rate,3}|");
            }
            builder.Append("\n");
            builder.Append("    LEVEL|");
            for (int i = 0; i < Segments.Length; i++)
            {
                builder.Append($"{Segments[i].Level,3}|");
            }
            builder.Append("\n\n");
            builder.Append("    LOOP<3-4>=");
            builder.Append(IsLooping ? "YES" : "--");
            builder.Append("\n\n");
            return builder.ToString();
        }
    }

    public class PitchSettings
    {
        private CoarseType _coarse; // 0~±48
        public sbyte Coarse
        {
            get => _coarse.Value;
            set => _coarse.Value = value;
        }

        private DepthType _fine; // 0~±31
        public sbyte Fine
        {
            get => _fine.Value;
            set => _fine.Value = value;
        }

        public KeyTracking KeyTracking;  // enumeration
        public byte Key;  // the keytracking key, zero if not used

        private EnvelopeDepthType _envelopeDepth; // 0~±24
        public sbyte EnvelopeDepth
        {
            get => _envelopeDepth.Value;
            set => _envelopeDepth.Value = value;
        }

        private DepthType _pressureDepth; // 0~±31
        public sbyte PressureDepth
        {
            get => _pressureDepth.Value;
            set => _pressureDepth.Value = value;
        }

        private BenderDepthType _benderDepth; // 0~24
        public byte BenderDepth
        {
            get => _benderDepth.Value;
            set => _benderDepth.Value = value;
        }

        private DepthType _velocityEnvelopeDepth; // 0~±31
        public sbyte VelocityEnvelopeDepth
        {
            get => _velocityEnvelopeDepth.Value;
            set => _velocityEnvelopeDepth.Value = value;
        }

        private PositiveDepthType _lfoDepth; // 0~31
        public byte LFODepth
        {
            get => _lfoDepth.Value;
            set => _lfoDepth.Value = value;
        }

        private DepthType _pressureLFODepth; // 0~±31
        public sbyte PressureLFODepth
        {
            get => _pressureLFODepth.Value;
            set => _pressureLFODepth.Value = value;
        }

        public PitchEnvelope Envelope;

        const int DataLength = 21;

        public PitchSettings()
        {
            _coarse = new CoarseType();
            _fine = new DepthType();
            _pressureDepth = new DepthType();
            _velocityEnvelopeDepth = new DepthType();
            _lfoDepth = new PositiveDepthType();
            _pressureLFODepth = new DepthType();
            _envelopeDepth = new EnvelopeDepthType();
            _benderDepth = new BenderDepthType();

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
                Coarse, Fine, EnvelopeDepth, VelocityEnvelopeDepth,
                PressureDepth, LFODepth, PressureLFODepth,
                KeyTracking, BenderDepth,
                Key) + 
                Envelope;
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();
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
            data.Add(BenderDepth);
            data.Add(VelocityEnvelopeDepth.ToByte());
            data.Add(LFODepth);
            data.Add(PressureLFODepth.ToByte());

            for (int i = 0; i < Source.PitchEnvelopeSegmentCount; i++)
            {
                b = Envelope.Segments[i].Rate;

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

            for (int i = 0; i < Source.PitchEnvelopeSegmentCount; i++)
            {
                sbyte sb = Envelope.Segments[i].Level;
                data.Add(sb.ToByte());
            }

            if (data.Count != DataLength)
            {
                Console.Error.WriteLine(string.Format("WARNING: DFG length, expected = {0}, actual = {1}", DataLength, data.Count));
            }

            return data.ToArray();
        }
    }


}