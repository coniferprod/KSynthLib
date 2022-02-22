using System;
using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K4
{
    public class Filter
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
            EnvelopeDepth = new Depth();
            EnvelopeVelocityDepth = new Depth();
            TimeMod = new TimeModulation();
        }

        public Filter(byte[] data) : this()
        {
            int offset = 0;
            byte b = 0;  // will be reused when getting the next byte

            (b, offset) = Util.GetNextByte(data, offset);
            Cutoff = new Level(b);

            (b, offset) = Util.GetNextByte(data, offset);
            Resonance = new Resonance(b & 0b00000111);
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
            EnvelopeDepth = new Depth(b);  // constructor with byte parameter adjusts to range

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
            builder.Append($"cutoff = {Cutoff.Value}, resonance = {Resonance.Value}\n");
            builder.Append(string.Format("LFO = {0}\n", IsLFO ? "ON" : "OFF"));
            builder.Append($"envelope: {Env}\n");
            builder.Append(string.Format("cutoff modulation: velocity = {0}, pressure = {1}, key scaling = {2}\n", CutoffMod.VelocityDepth, CutoffMod.PressureDepth, CutoffMod.KeyScalingDepth));
            builder.Append(string.Format("time modulation: attack velocity = {0}, release velocity = {1}, key scaling = {2}\n", TimeMod.AttackVelocity, TimeMod.ReleaseVelocity, TimeMod.KeyScaling));
            return builder.ToString();
        }

        public byte[] ToData()
        {
            var data = new List<byte>();

            data.Add(Cutoff.ToByte());

            var b104 = new StringBuilder("0000");
            b104.Append(IsLFO ? "1" : "0");
            int resonance = Resonance.ToByte();
            string resonanceString = Convert.ToString(resonance, 2);
            //Console.Error.WriteLine(string.Format("Filter resonance = {0}, as bit string = '{1}'", resonance, resonanceString));
            b104.Append(resonanceString.PadLeft(3, '0'));
            data.Add(Convert.ToByte(b104.ToString(), 2));

            data.AddRange(CutoffMod.ToData());

            data.Add(EnvelopeDepth.ToByte());
            data.Add(EnvelopeVelocityDepth.ToByte());

            data.AddRange(Env.ToData());
            data.AddRange(TimeMod.ToData());

            return data.ToArray();
        }
    }
}
