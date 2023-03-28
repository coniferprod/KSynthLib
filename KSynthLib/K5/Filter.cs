using System;
using System.Text;
using System.Collections.Generic;

using SyxPack;

using KSynthLib.Common;

namespace KSynthLib.K5
{
    public struct FilterEnvelopeSegment
    {
        public byte Rate;
        public bool IsMaxSegment;
        public byte Level;
    }

    public class Filter
    {
        public byte Cutoff;
        public byte CutoffModulation;
        public byte Slope;
        public byte SlopeModulation;
        public byte FlatLevel;

        public Depth VelocityDepth; // 0~±31
        public Depth PressureDepth; // 0~±31
        public Depth KeyScalingDepth; // 0~±31
        public Depth EnvelopeDepth; // 0~±31
        public Depth VelocityEnvelopeDepth; // 0~±31

        public bool IsActive;
        public bool IsModulationActive;

        public PositiveDepth LFODepth;

        public FilterEnvelopeSegment[] EnvelopeSegments;

        public Filter()
        {
            VelocityDepth = new Depth();
            PressureDepth = new Depth();
            KeyScalingDepth = new Depth();
            EnvelopeDepth = new Depth();
            VelocityEnvelopeDepth = new Depth();
            LFODepth = new PositiveDepth();

            EnvelopeSegments = new FilterEnvelopeSegment[Source.FilterEnvelopeSegmentCount];
            for (var i = 0; i < Source.FilterEnvelopeSegmentCount; i++)
            {
                EnvelopeSegments[i] = new FilterEnvelopeSegment();
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append(string.Format("*DDF*={0}   MOD={1}\n", IsActive ? "ON" : "--", IsModulationActive ? "ON" : "--"));
            builder.Append("                   <DEPTH>\n");
            builder.Append(string.Format(" CUTOFF={0,2}-MOD={1,2}  ENV={2,3}-VEL={3,3}\n", Cutoff, CutoffModulation, EnvelopeDepth.Value, VelocityEnvelopeDepth.Value));
            builder.Append(string.Format(" SLOPE ={0,2}-MOD={1,2}  VEL={2,3}\n", Slope, SlopeModulation, VelocityDepth.Value));
            builder.Append(string.Format("FLAT.LV={0,2}         PRS={1,3}\n", FlatLevel, PressureDepth.Value));
            builder.Append($"                    KS={KeyScalingDepth.Value,3}\n");
            builder.Append($"                   LFO={LFODepth.Value,3}\n");
            builder.Append("\n\n");

            builder.Append("*DDF ENV*\n\n    SEG  |");
            for (var i = 0; i < Source.FilterEnvelopeSegmentCount; i++)
            {
                builder.Append(string.Format("{0,3}|", i + 1));
            }
            builder.Append("\n    ------------------------------\n");
            builder.Append("    RATE |");
            for (var i = 0; i < Source.FilterEnvelopeSegmentCount; i++)
            {
                builder.Append(string.Format("{0,3}|", EnvelopeSegments[i].Rate));
            }
            builder.Append("\n    LEVEL|");
            for (var i = 0; i < Source.FilterEnvelopeSegmentCount; i++)
            {
                var levelString = EnvelopeSegments[i].IsMaxSegment ? "  *" : string.Format("{0,3}", EnvelopeSegments[i].Level);
                builder.Append($"{levelString}|");
            }
            builder.Append("\n\n    MAX SEG = ?\n\n");

            return builder.ToString();
        }

        const int DataLength = 23;

        public byte[] ToData()
        {
            var data = new List<byte>();

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

            byte b = LFODepth.ToByte();
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

            for (var i = 0; i < Source.FilterEnvelopeSegmentCount; i++)
            {
                data.Add(EnvelopeSegments[i].Rate);
            }

            for (var i = 0; i < Source.FilterEnvelopeSegmentCount; i++)
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
                Console.Error.WriteLine($"WARNING: DDF length, expected = {DataLength}, actual = {data.Count}", DataLength);
            }

            Console.Error.WriteLine(string.Format("DDF data:\n{0}", new HexDump(data)));

            return data.ToArray();
        }
    }
}