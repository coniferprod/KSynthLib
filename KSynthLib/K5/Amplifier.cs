using System;
using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K5
{
    public class AmplifierEnvelopeSegment
    {
        public bool IsRateModulationOn;

        public PositiveDepth Rate; // 0~31

        public bool IsMaxSegment;  // only one segment can be max

        public PositiveDepth Level; // 0~31

        public AmplifierEnvelopeSegment()
        {
            IsRateModulationOn = false;
            Rate = new PositiveDepth();
            IsMaxSegment = false;
            Level = new PositiveDepth();
        }

        public AmplifierEnvelopeSegment(bool isRateMod, byte rate, bool isMaxSeg, byte level) : base()
        {
            IsRateModulationOn = isRateMod;
            Rate = new PositiveDepth(rate);
            IsMaxSegment = isMaxSeg;
            Level = new PositiveDepth(level);
        }

        public override string ToString()
        {
            return $"rate={Rate} isRateMod={IsRateModulationOn} level={Level} isMaxSeg={IsMaxSegment}";
        }
    }

    public class AmplifierEnvelope
    {
        public const int SegmentCount = 7;

        public AmplifierEnvelopeSegment[] Segments;

        public AmplifierEnvelope()
        {
            Segments = new AmplifierEnvelopeSegment[SegmentCount];
            for (var i = 0; i < SegmentCount; i++)
            {
                Segments[i] = new AmplifierEnvelopeSegment();
            }
        }
    }

    public class Amplifier
    {
        public static readonly Dictionary<string, AmplifierEnvelope> Envelopes = new Dictionary<string, AmplifierEnvelope>
        {
            {
                "regular", new AmplifierEnvelope
                {
                    Segments = new AmplifierEnvelopeSegment[]
                    {
                        new AmplifierEnvelopeSegment(true, 0, true, 25),
                        new AmplifierEnvelopeSegment(true, 21, false, 27),
                        new AmplifierEnvelopeSegment(true, 28, false, 0),
                        new AmplifierEnvelopeSegment(true, 0, false, 0),
                        new AmplifierEnvelopeSegment(true, 30, false, 0),
                        new AmplifierEnvelopeSegment(true, 0, false, 0),
                        new AmplifierEnvelopeSegment(true, 0, false, 0)
                    }
                }
            },
            {
                "silent", new AmplifierEnvelope
                {
                    Segments = new AmplifierEnvelopeSegment[]
                    {
                        new AmplifierEnvelopeSegment(true, 0, false, 0),
                        new AmplifierEnvelopeSegment(true, 0, false, 0),
                        new AmplifierEnvelopeSegment(true, 0, false, 0),
                        new AmplifierEnvelopeSegment(true, 0, false, 0),
                        new AmplifierEnvelopeSegment(true, 0, false, 0),
                        new AmplifierEnvelopeSegment(true, 0, false, 0),
                        new AmplifierEnvelopeSegment(true, 0, false, 0)
                    }
                }
            }
        };

        public bool IsActive;

        public Depth AttackVelocityDepth; // 0~±31
        public Depth PressureDepth; // 0~±31
        public Depth KeyScalingDepth; // 0~±31
        public PositiveDepth LFODepth; // 0~31
        public Rate AttackVelocityRate; // 0~±15
        public Rate ReleaseVelocityRate; // 0~±15
        public Rate KeyScalingRate; // 0~±15
        public AmplifierEnvelope Envelope;

        public Amplifier()
        {
            AttackVelocityDepth = new Depth();
            PressureDepth = new Depth();
            KeyScalingDepth = new Depth();
            LFODepth = new PositiveDepth();
            AttackVelocityRate = new Rate();
            ReleaseVelocityRate = new Rate();
            KeyScalingRate = new Rate();
            Envelope = new AmplifierEnvelope();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append("*DDA*=");
            builder.Append(IsActive ? "ON" : "--");
            builder.Append("\n\n");
            builder.Append("            <DEPTH>       <RATE>\n");
            builder.Append(string.Format("         AT VEL={0,3}     AT VEL={1,3}\n", AttackVelocityDepth, AttackVelocityRate));
            builder.Append(string.Format("            PRS={0,3}     RL VEL={1,3}\n", PressureDepth, ReleaseVelocityRate));
            builder.Append(string.Format("             KS={0,3}         KS={1,3}\n", KeyScalingDepth, KeyScalingRate));
            builder.Append(string.Format("            LFO={0,3}\n", LFODepth));
            builder.Append("\n\n");

            // The following code is kind of ugly, with many similar for loops,
            // but all the alternatives I could come up with were a little
            // too much trouble for what they are worth (but obviously would
            // be good for learning). Maybe I'll revisit this some day.

            builder.Append("*DDA ENV*\n\n    SEG  |");
            for (var i = 0; i < AmplifierEnvelope.SegmentCount; i++)
            {
                builder.Append($"{i + 1,3} |");
            }

            builder.Append("\n    -----------------------------------------\n");
            builder.Append("    RATE |");
            for (int i = 0; i < AmplifierEnvelope.SegmentCount; i++)
            {
                builder.Append($"{Envelope.Segments[i].Rate,3} |");
            }
            builder.Append("\n    LEVEL|");
            for (int i = 0; i < AmplifierEnvelope.SegmentCount; i++)
            {
                var levelString = string.Format("{0,3}{1}", Envelope.Segments[i].Level, Envelope.Segments[i].IsMaxSegment ? "*" : " ");
                builder.Append($"{levelString}|");
            }
            builder.Append("\n    RTMOD|");
            for (int i = 0; i < AmplifierEnvelope.SegmentCount; i++)
            {
                var rateModulationString = Envelope.Segments[i].IsRateModulationOn ? " ON" : " --";
                builder.Append($"{rateModulationString} |");
            }

            builder.Append($"\n    MAX SEG = ?   (seg1level={Envelope.Segments[0].Level})\n\n");

            return builder.ToString();
        }

        const int DataLength = 21;

        public byte[] ToData()
        {
            var data = new List<byte>();

            data.Add(AttackVelocityDepth.ToByte());
            data.Add(PressureDepth.ToByte());
            data.Add(KeyScalingDepth.ToByte());
            byte b = LFODepth.ToByte();
            if (IsActive)
            {
                b = b.SetBit(7);
            }
            else
            {
                b = b.UnsetBit(7);
            }
            data.Add(b);
            data.Add(AttackVelocityRate.ToByte());
            data.Add(ReleaseVelocityRate.ToByte());
            data.Add(KeyScalingRate.ToByte());

            for (var i = 0; i < AmplifierEnvelope.SegmentCount; i++)
            {
                b = Envelope.Segments[i].Rate.ToByte();
                //Console.Error.WriteLine(string.Format("seg={0}, rate={1}, mod={2}", i, Envelope.Segments[i].Rate, Envelope.Segments[i].IsRateModulationOn));
                if (Envelope.Segments[i].IsRateModulationOn)
                {
                    b = b.SetBit(6);
                }
                else
                {
                    b = b.UnsetBit(6);
                }
                data.Add(b);
                //Console.Error.WriteLine(string.Format("{0:X2}H", b));
            }

            for (var i = 0; i < AmplifierEnvelope.SegmentCount - 1; i++)
            {
                b = Envelope.Segments[i].Level.ToByte();
                if (Envelope.Segments[i].IsMaxSegment)
                {
                    b = b.SetBit(6);
                }
                else
                {
                    b = b.UnsetBit(6);
                }
                data.Add(b);
            }

            data.Add(0);

            if (data.Count != DataLength)
            {
                Console.Error.WriteLine($"WARNING: DDA length, expected = {DataLength}, actual = {data.Count}");
            }

            return data.ToArray();
        }
    }
}
