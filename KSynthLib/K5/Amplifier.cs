using System;
using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K5
{
    public struct AmplifierEnvelope
    {
        public AmplifierEnvelopeSegment[] Segments;
    }

    public struct AmplifierEnvelopeSegment
    {
        public bool IsRateModulationOn;
        public byte Rate; // 0~31
        public bool IsMaxSegment;  // only one segment can be max
        public byte Level; // 0~31
    }

    public struct Amplifier 
    {
        public static readonly Dictionary<string, AmplifierEnvelope> Envelopes = new Dictionary<string, AmplifierEnvelope>
        {
            {
                "regular", new AmplifierEnvelope
                { 
                    Segments = new AmplifierEnvelopeSegment[]
                    {
                        new AmplifierEnvelopeSegment { IsRateModulationOn = true, Rate = 0, IsMaxSegment = true, Level = 25 },
                        new AmplifierEnvelopeSegment { IsRateModulationOn = true, Rate = 21, IsMaxSegment = false, Level = 27 },
                        new AmplifierEnvelopeSegment { IsRateModulationOn = true, Rate = 28, IsMaxSegment = false, Level = 0 },
                        new AmplifierEnvelopeSegment { IsRateModulationOn = true, Rate = 0, IsMaxSegment = false, Level = 0 },
                        new AmplifierEnvelopeSegment { IsRateModulationOn = true, Rate = 30, IsMaxSegment = false, Level = 0 },
                        new AmplifierEnvelopeSegment { IsRateModulationOn = true, Rate = 0, IsMaxSegment = false, Level = 0 },
                        new AmplifierEnvelopeSegment { IsRateModulationOn = true, Rate = 0, IsMaxSegment = false, Level = 0 }
                    }
                }
            },
            {
                "silent", new AmplifierEnvelope
                {
                    Segments = new AmplifierEnvelopeSegment[]
                    {
                        new AmplifierEnvelopeSegment { IsRateModulationOn = true, Rate = 0, IsMaxSegment = false, Level = 0 },
                        new AmplifierEnvelopeSegment { IsRateModulationOn = true, Rate = 0, IsMaxSegment = false, Level = 0 },
                        new AmplifierEnvelopeSegment { IsRateModulationOn = true, Rate = 0, IsMaxSegment = false, Level = 0 },
                        new AmplifierEnvelopeSegment { IsRateModulationOn = true, Rate = 0, IsMaxSegment = false, Level = 0 },
                        new AmplifierEnvelopeSegment { IsRateModulationOn = true, Rate = 0, IsMaxSegment = false, Level = 0 },
                        new AmplifierEnvelopeSegment { IsRateModulationOn = true, Rate = 0, IsMaxSegment = false, Level = 0 },
                        new AmplifierEnvelopeSegment { IsRateModulationOn = true, Rate = 0, IsMaxSegment = false, Level = 0 }
                    }
                }
            }
        };

        public bool IsActive;
	    public sbyte AttackVelocityDepth;  // 0~±31
	    public sbyte PressureDepth;  // 0~±31
	    public sbyte KeyScalingDepth;  // 0~±31
	    public byte LFODepth; // 0~31
	    public sbyte AttackVelocityRate;  // 0~±15
	    public sbyte ReleaseVelocityRate;  // 0~±15
	    public sbyte KeyScalingRate;  // 0~±15
        public AmplifierEnvelopeSegment[] EnvelopeSegments;

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("*DDA*=");
            builder.Append(IsActive ? "ON" : "--");
            builder.Append("\n\n");
            builder.Append("            <DEPTH>       <RATE>\n");
            builder.Append(String.Format("         AT VEL={0,3}     AT VEL={1,3}\n", AttackVelocityDepth, AttackVelocityRate));
            builder.Append(String.Format("            PRS={0,3}     RL VEL={1,3}\n", PressureDepth, ReleaseVelocityRate));
            builder.Append(String.Format("             KS={0,3}         KS={1,3}\n", KeyScalingDepth, KeyScalingRate));
            builder.Append(String.Format("            LFO={0,3}\n", LFODepth));
            builder.Append("\n\n");

            builder.Append("*DDA ENV*\n\n    SEG  |");
            for (int i = 0; i < Source.AmplifierEnvelopeSegmentCount; i++)
            {
                builder.Append(String.Format("{0,3}|", i + 1));
            }
            builder.Append("\n    ----------------------------------\n");
            builder.Append("    RATE |");
            for (int i = 0; i < Source.AmplifierEnvelopeSegmentCount; i++)
            {
                builder.Append(String.Format("{0,3}|", EnvelopeSegments[i].Rate));
            }
            builder.Append("\n    LEVEL|");
            for (int i = 0; i < Source.AmplifierEnvelopeSegmentCount; i++)
            {
                string levelString = String.Format("{0,3}{1}", EnvelopeSegments[i].Level, EnvelopeSegments[i].IsMaxSegment ? "*" : " ");
                builder.Append(String.Format("{0}|", levelString));
            }
            builder.Append("\n    RTMOD|");
            for (int i = 0; i < Source.AmplifierEnvelopeSegmentCount; i++)
            {
                string rateModulationString = EnvelopeSegments[i].IsRateModulationOn ? " ON" : " --";
                builder.Append(String.Format("{0} |", rateModulationString));
            }

            builder.Append($"\n    MAX SEG = ?   (seg1level={EnvelopeSegments[0].Level})\n\n");

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
            
            for (int i = 0; i < Source.AmplifierEnvelopeSegmentCount; i++)
            {
                b = EnvelopeSegments[i].Rate;
                //System.Console.WriteLine(String.Format("seg={0}, rate={1}, mod={2}", i, EnvelopeSegments[i].Rate, EnvelopeSegments[i].IsRateModulationOn));
                if (EnvelopeSegments[i].IsRateModulationOn)
                {
                    b = b.SetBit(6);
                }
                else
                {
                    b = b.UnsetBit(6);
                }
                data.Add(b);
                //System.Console.WriteLine(String.Format("{0:X2}H", b));
            }

            for (int i = 0; i < Source.AmplifierEnvelopeSegmentCount - 1; i++)
            {
                b = EnvelopeSegments[i].Level;
                if (EnvelopeSegments[i].IsMaxSegment)
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
                System.Console.WriteLine(String.Format("WARNING: DDA length, expected = {0}, actual = {1}", DataLength, data.Count));
            }

            return data.ToArray();
        }
    }    
}