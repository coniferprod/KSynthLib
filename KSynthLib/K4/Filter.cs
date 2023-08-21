using System;
using System.Text;
using System.Collections.Generic;

using SyxPack;
using KSynthLib.Common;

namespace KSynthLib.K4
{
    public class Filter : ISystemExclusiveData
    {
        public const int DataSize = 14;

        public Level Cutoff;
        public Resonance Resonance;
        public LevelModulation CutoffMod;
        public bool IsLFO;  // 0/off, 1/on
        public FilterEnvelope Env;
        public Depth EnvelopeDepth;
        public Depth EnvelopeVelocityDepth;
        public TimeModulation TimeMod;

        public Filter()
        {
            Cutoff = new Level(88);
            Resonance = new Resonance(0);
            CutoffMod = new LevelModulation();
            IsLFO = false;
            Env = new FilterEnvelope();
            EnvelopeDepth = new Depth(0);
            EnvelopeVelocityDepth = new Depth(0);
            TimeMod = new TimeModulation();
        }

        public Filter(byte[] data) : this()
        {
            byte b;  // will be reused when getting the next byte
            int offset = 0;

            (b, offset) = Util.GetNextByte(data, offset);
            Cutoff = new Level(b);

            (b, offset) = Util.GetNextByte(data, offset);
            Resonance = new Resonance(b & 0b0000_0111);
            IsLFO = b.IsBitSet(3);

            var cutoffModBytes = new List<byte>();
            (b, offset) = Util.GetNextByte(data, offset);
            cutoffModBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            cutoffModBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            cutoffModBytes.Add(b);
            CutoffMod = new LevelModulation(cutoffModBytes);

            (b, offset) = Util.GetNextByte(data, offset);
            EnvelopeDepth = new Depth(b);

            (b, offset) = Util.GetNextByte(data, offset);
            EnvelopeVelocityDepth = new Depth(b);

            var envBytes = new List<byte>();
            (b, offset) = Util.GetNextByte(data, offset);
            envBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            envBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            envBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            envBytes.Add(b);
            Env = new FilterEnvelope(envBytes);

            var timeModBytes = new List<byte>();
            (b, offset) = Util.GetNextByte(data, offset);
            timeModBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            timeModBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            timeModBytes.Add(b);
            TimeMod = new TimeModulation(timeModBytes);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine($"cutoff = {Cutoff}, resonance = {Resonance}");
            builder.AppendLine(string.Format("LFO = {0}", IsLFO ? "ON" : "OFF"));
            builder.AppendLine($"envelope: {Env}");
            builder.AppendLine($"cutoff modulation: {CutoffMod}");
            builder.AppendLine($"time modulation: {TimeMod}");

            return builder.ToString();
        }

        //
        // ISystemExclusiveData implementation
        //

        public List<byte> Data
        {
            get
            {
                var data = new List<byte>();

                data.Add(Cutoff.ToByte());

                var b104 = new StringBuilder("0000");
                b104.Append(IsLFO ? "1" : "0");
                string resonanceString = Convert.ToString(Resonance.ToByte(), 2);
                //Console.Error.WriteLine(string.Format("Filter resonance = {0}, as bit string = '{1}'", resonance, resonanceString));
                b104.Append(resonanceString.PadLeft(3, '0'));
                data.Add(Convert.ToByte(b104.ToString(), 2));

                data.AddRange(CutoffMod.Data);

                data.Add(EnvelopeDepth.ToByte());
                data.Add(EnvelopeVelocityDepth.ToByte());

                data.AddRange(Env.Data);
                data.AddRange(TimeMod.Data);

                return data;
            }
        }

        public int DataLength => DataSize;
    }
}
