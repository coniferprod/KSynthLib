using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using KSynthLib.Common;

namespace KSynthLib.K4
{
    public class Filter: ISystemExclusiveData
    {
        public const int DataSize = 14;

        [Range(0, 100, ErrorMessage = "{0} must be between {1} and {2}")]
        public int Cutoff;

        [Range(0, 7, ErrorMessage = "{0} must be between {1} and {2}")]
        public int Resonance;

        public LevelModulation CutoffMod;
        public bool IsLFO;  // 0/off, 1/on
        public FilterEnvelope Env;

        [Range(-50, 50, ErrorMessage = "{0} must be between {1} and {2}")]
        public int EnvelopeDepth;

        [Range(-50, 50, ErrorMessage = "{0} must be between {1} and {2}")]
        public int EnvelopeVelocityDepth;

        public TimeModulation TimeMod;

        public Filter()
        {
            Cutoff = 88;
            Resonance = 0;
            CutoffMod = new LevelModulation();
            IsLFO = false;
            Env = new FilterEnvelope();
            EnvelopeDepth = 0;
            EnvelopeVelocityDepth = 0;
            TimeMod = new TimeModulation();
        }

        public Filter(byte[] data) : this()
        {
            int offset = 0;
            byte b = 0;  // will be reused when getting the next byte

            (b, offset) = Util.GetNextByte(data, offset);
            Cutoff = b;

            (b, offset) = Util.GetNextByte(data, offset);
            Resonance = b & 0b0000_0111;
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
            EnvelopeDepth = b;

            (b, offset) = Util.GetNextByte(data, offset);
            EnvelopeVelocityDepth = b;

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

        public List<byte> GetSystemExclusiveData()
        {
            var data = new List<byte>();

            data.Add((byte)Cutoff);

            var b104 = new StringBuilder("0000");
            b104.Append(IsLFO ? "1" : "0");
            string resonanceString = Convert.ToString(Resonance, 2);
            //Console.Error.WriteLine(string.Format("Filter resonance = {0}, as bit string = '{1}'", resonance, resonanceString));
            b104.Append(resonanceString.PadLeft(3, '0'));
            data.Add(Convert.ToByte(b104.ToString(), 2));

            data.AddRange(CutoffMod.GetSystemExclusiveData());

            data.Add((byte)EnvelopeDepth);
            data.Add((byte)EnvelopeVelocityDepth);

            data.AddRange(Env.GetSystemExclusiveData());
            data.AddRange(TimeMod.GetSystemExclusiveData());

            return data;
        }
    }
}
