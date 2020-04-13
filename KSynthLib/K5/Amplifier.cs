using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using KSynthLib.Common;

namespace KSynthLib.K5
{
    public class AmplifierEnvelopeSegment
    {
        public bool IsRateModulationOn;

        private PositiveDepthType _rate; // 0~31
        public byte Rate
        {
            get => _rate.Value;
            set => _rate.Value = value;
        }

        public bool IsMaxSegment;  // only one segment can be max

        private PositiveDepthType _level; // 0~31
        public byte Level
        {
            get => _level.Value;
            set => _level.Value = value;
        }

        public AmplifierEnvelopeSegment()
        {
            IsRateModulationOn = false;
            _rate = new PositiveDepthType();
            IsMaxSegment = false;
            _level = new PositiveDepthType();
        }

        public AmplifierEnvelopeSegment(bool isRateMod, byte rate, bool isMaxSeg, byte level) : base()
        {
            IsRateModulationOn = isRateMod;
            Rate = rate;
            IsMaxSegment = isMaxSeg;
            Level = level;            
        }

        public override string ToString()
        {
            return $"rate={Rate} isRateMod={IsRateModulationOn} level={Level} isMaxSeg={IsMaxSegment}";
        }
    }

    public class AmplifierEnvelope
    {
        public static readonly int SegmentCount = 7;

        public AmplifierEnvelopeSegment[] Segments;

        public AmplifierEnvelope()
        {
            Segments = new AmplifierEnvelopeSegment[SegmentCount];
            for (int i = 0; i < SegmentCount; i++)
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

        private DepthType _attackVelocityDepth; // 0~±31
	    public sbyte AttackVelocityDepth
        {
            get => _attackVelocityDepth.Value;
            set => _attackVelocityDepth.Value = value;
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

        private RateType _attackVelocityRate; // 0~±15
	    public sbyte AttackVelocityRate
        {
            get => _attackVelocityRate.Value;
            set => _attackVelocityRate.Value = value;
        }

        private RateType _releaseVelocityRate; // 0~±15
	    public sbyte ReleaseVelocityRate
        {
            get => _releaseVelocityRate.Value;
            set => _releaseVelocityRate.Value = value;
        }

        private RateType _keyScalingRate; // 0~±15
	    public sbyte KeyScalingRate
        {
            get => _keyScalingRate.Value;
            set => _keyScalingRate.Value = value;
        }

        public AmplifierEnvelope Envelope;

        public Amplifier()
        {
            _attackVelocityDepth = new DepthType();
            _pressureDepth = new DepthType();
            _keyScalingDepth = new DepthType();
            _lfoDepth = new PositiveDepthType();
            _attackVelocityRate = new RateType();
            _releaseVelocityRate = new RateType();
            _keyScalingRate = new RateType();

            Envelope = new AmplifierEnvelope();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

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
            for (int i = 0; i < AmplifierEnvelope.SegmentCount; i++)
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
                string levelString = string.Format("{0,3}{1}", Envelope.Segments[i].Level, Envelope.Segments[i].IsMaxSegment ? "*" : " ");
                builder.Append($"{levelString}|");
            }
            builder.Append("\n    RTMOD|");
            for (int i = 0; i < AmplifierEnvelope.SegmentCount; i++)
            {
                string rateModulationString = Envelope.Segments[i].IsRateModulationOn ? " ON" : " --";
                builder.Append($"{rateModulationString} |");
            }

            builder.Append($"\n    MAX SEG = ?   (seg1level={Envelope.Segments[0].Level})\n\n");

            return builder.ToString();
        }

        const int DataLength = 21;

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();

            data.Add(AttackVelocityDepth.ToByte());
            data.Add(PressureDepth.ToByte());
            data.Add(KeyScalingDepth.ToByte());
            byte b = LFODepth;
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
            
            for (int i = 0; i < AmplifierEnvelope.SegmentCount; i++)
            {
                b = Envelope.Segments[i].Rate;
                //Console.WriteLine(string.Format("seg={0}, rate={1}, mod={2}", i, Envelope.Segments[i].Rate, Envelope.Segments[i].IsRateModulationOn));
                if (Envelope.Segments[i].IsRateModulationOn)
                {
                    b = b.SetBit(6);
                }
                else
                {
                    b = b.UnsetBit(6);
                }
                data.Add(b);
                //Console.WriteLine(string.Format("{0:X2}H", b));
            }

            for (int i = 0; i < AmplifierEnvelope.SegmentCount - 1; i++)
            {
                b = Envelope.Segments[i].Level;
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
                Console.WriteLine($"WARNING: DDA length, expected = {DataLength}, actual = {data.Count}");
            }

            return data.ToArray();
        }
    }    
}
