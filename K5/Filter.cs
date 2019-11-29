using System;
using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K5
{
    public struct FilterEnvelopeSegment
    {
        public byte Rate;
        public bool IsMaxSegment;
        public byte Level;
    }

    public struct Filter
    {
        public byte Cutoff;
        public byte CutoffModulation;
        public byte Slope;
        public byte SlopeModulation;
        public byte FlatLevel;
        public sbyte VelocityDepth; // 0~±31
        public sbyte PressureDepth; // 0~±31
        public sbyte KeyScalingDepth;  // 0~±31
        public sbyte EnvelopeDepth; // 0~±31
        public sbyte VelocityEnvelopeDepth;  // 0~±31
        public bool IsActive;
        public bool IsModulationActive;
        public byte LFODepth; // 0~31
        public FilterEnvelopeSegment[] EnvelopeSegments;

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("*DDF*=");
            builder.Append(IsActive ? "ON" : "--");
            builder.Append("   MOD=");
            builder.Append(IsModulationActive ? "ON" : "--");
            builder.Append("\n");
            builder.Append("                   <DEPTH>\n");
            builder.Append(String.Format(" CUTOFF={0,2}-MOD={1,2}  ENV={2,3}-VEL={3,3}\n", Cutoff, CutoffModulation, EnvelopeDepth, VelocityEnvelopeDepth));
            builder.Append(String.Format(" SLOPE ={0,2}-MOD={1,2}  VEL={2,3}\n", Slope, SlopeModulation, VelocityDepth));
            builder.Append(String.Format("FLAT.LV={0,2}         PRS={1,3}\n", FlatLevel, PressureDepth));
            builder.Append(String.Format("                    KS={0,3}\n", KeyScalingDepth));
            builder.Append(String.Format("                   LFO={0,3}\n", LFODepth));
            builder.Append("\n\n");

            builder.Append("*DDF ENV*\n\n    SEG  |");
            for (int i = 0; i < Source.FilterEnvelopeSegmentCount; i++)
            {
                builder.Append(String.Format("{0,3}|", i + 1));
            }
            builder.Append("\n    ------------------------------\n");
            builder.Append("    RATE |");
            for (int i = 0; i < Source.FilterEnvelopeSegmentCount; i++)
            {
                builder.Append(String.Format("{0,3}|", EnvelopeSegments[i].Rate));
            }
            builder.Append("\n    LEVEL|");
            for (int i = 0; i < Source.FilterEnvelopeSegmentCount; i++)
            {
                string levelString = EnvelopeSegments[i].IsMaxSegment ? "  *" : String.Format("{0,3}", EnvelopeSegments[i].Level);
                builder.Append(String.Format("{0}|", levelString));
            }
            builder.Append("\n\n    MAX SEG = ?\n\n");

            return builder.ToString();
        }

        const int DataLength = 23;

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();

            data.Add(Cutoff);
            data.Add(CutoffModulation);
            data.Add(Slope);
            data.Add(SlopeModulation);
            data.Add(FlatLevel);
            data.Add(VelocityDepth.ToByte());
            data.Add(PressureDepth.ToByte());
            data.Add(KeyScalingDepth.ToByte());
            data.Add(EnvelopeDepth.ToByte());
            data.Add(VelocityEnvelopeDepth.ToByte());

            byte b = LFODepth;
            if (IsModulationActive)
            {
                b = b.SetBit(6);
            }
            else
            {
                b = b.UnsetBit(6);
            }
            if (IsActive)
            {
                b = b.SetBit(7);
            }
            else
            {
                b = b.UnsetBit(7);
            }
            data.Add(b);

            for (int i = 0; i < Source.FilterEnvelopeSegmentCount; i++)
            {
                data.Add(EnvelopeSegments[i].Rate);
            }

            for (int i = 0; i < Source.FilterEnvelopeSegmentCount; i++)
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

            if (data.Count != DataLength)
            {
                System.Console.WriteLine(String.Format("WARNING: DDF length, expected = {0}, actual = {1}", DataLength, data.Count));
            }

            System.Console.WriteLine(String.Format("DDF data:\n{0}", Util.HexDump(data.ToArray())));

            return data.ToArray();
        }
    }
}