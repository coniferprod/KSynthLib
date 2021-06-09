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

    public class Filter
    {
        public byte Cutoff;
        public byte CutoffModulation;
        public byte Slope;
        public byte SlopeModulation;
        public byte FlatLevel;

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

        private DepthType _envelopeDepth; // 0~±31
        public sbyte EnvelopeDepth
        {
            get => _envelopeDepth.Value;
            set => _envelopeDepth.Value = value;
        }

        private DepthType _velocityEnvelopeDepth; // 0~±31
        public sbyte VelocityEnvelopeDepth
        {
            get => _velocityEnvelopeDepth.Value;
            set => _velocityEnvelopeDepth.Value = value;
        }

        public bool IsActive;
        public bool IsModulationActive;

        private PositiveDepthType _lfoDepth;
        public byte LFODepth
        {
            get => _lfoDepth.Value;
            set => _lfoDepth.Value = value;
        }

        public FilterEnvelopeSegment[] EnvelopeSegments;

        public Filter()
        {
            _velocityDepth = new DepthType();
            _pressureDepth = new DepthType();
            _keyScalingDepth = new DepthType();
            _envelopeDepth = new DepthType();
            _velocityEnvelopeDepth = new DepthType();
            _lfoDepth = new PositiveDepthType();

            EnvelopeSegments = new FilterEnvelopeSegment[Source.FilterEnvelopeSegmentCount];
            for (int i = 0; i < Source.FilterEnvelopeSegmentCount; i++)
            {
                EnvelopeSegments[i] = new FilterEnvelopeSegment();
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(string.Format("*DDF*={0}   MOD={1}\n", IsActive ? "ON" : "--", IsModulationActive ? "ON" : "--"));
            builder.Append("                   <DEPTH>\n");
            builder.Append(string.Format(" CUTOFF={0,2}-MOD={1,2}  ENV={2,3}-VEL={3,3}\n", Cutoff, CutoffModulation, EnvelopeDepth, VelocityEnvelopeDepth));
            builder.Append(string.Format(" SLOPE ={0,2}-MOD={1,2}  VEL={2,3}\n", Slope, SlopeModulation, VelocityDepth));
            builder.Append(string.Format("FLAT.LV={0,2}         PRS={1,3}\n", FlatLevel, PressureDepth));
            builder.Append($"                    KS={KeyScalingDepth,3}\n");
            builder.Append($"                   LFO={LFODepth,3}\n");
            builder.Append("\n\n");

            builder.Append("*DDF ENV*\n\n    SEG  |");
            for (int i = 0; i < Source.FilterEnvelopeSegmentCount; i++)
            {
                builder.Append(string.Format("{0,3}|", i + 1));
            }
            builder.Append("\n    ------------------------------\n");
            builder.Append("    RATE |");
            for (int i = 0; i < Source.FilterEnvelopeSegmentCount; i++)
            {
                builder.Append(string.Format("{0,3}|", EnvelopeSegments[i].Rate));
            }
            builder.Append("\n    LEVEL|");
            for (int i = 0; i < Source.FilterEnvelopeSegmentCount; i++)
            {
                string levelString = EnvelopeSegments[i].IsMaxSegment ? "  *" : string.Format("{0,3}", EnvelopeSegments[i].Level);
                builder.Append($"{levelString}|");
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
                Console.Error.WriteLine($"WARNING: DDF length, expected = {DataLength}, actual = {data.Count}", DataLength);
            }

            Console.Error.WriteLine(string.Format("DDF data:\n{0}", Util.HexDump(data.ToArray())));

            return data.ToArray();
        }
    }
}