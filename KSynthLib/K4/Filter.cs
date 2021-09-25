using System;
using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K4
{
    public class Filter
    {
        public const int DataSize = 14;

        public LevelType Cutoff;

        private ResonanceType _resonance; // 0 ~ 7 / 1 ~ 8
        public byte Resonance
        {
            get => _resonance.Value;
            set => _resonance.Value = value;
        }

        public LevelModulation CutoffMod;
        public bool IsLFO;  // 0/off, 1/on
        public FilterEnvelope Env;
        public DepthType EnvelopeDepth;
        public DepthType EnvelopeVelocityDepth;
        public TimeModulation TimeMod;

        public Filter()
        {
            Cutoff = new LevelType(88);
            _resonance = new ResonanceType();
            CutoffMod = new LevelModulation();
            IsLFO = false;
            Env = new FilterEnvelope();
            EnvelopeDepth = new DepthType();
            EnvelopeVelocityDepth = new DepthType();
            TimeMod = new TimeModulation();
        }

        public Filter(byte[] data) : this()
        {
            int offset = 0;
            byte b = 0;  // will be reused when getting the next byte

            (b, offset) = Util.GetNextByte(data, offset);
            Cutoff = new LevelType(b);

            (b, offset) = Util.GetNextByte(data, offset);
            _resonance = new ResonanceType(b);
            IsLFO = b.IsBitSet(3);

            List<byte> cutoffModBytes = new List<byte>();
            (b, offset) = Util.GetNextByte(data, offset);
            cutoffModBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            cutoffModBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            cutoffModBytes.Add(b);
            CutoffMod = new LevelModulation(cutoffModBytes);

            (b, offset) = Util.GetNextByte(data, offset);
            EnvelopeDepth = new DepthType(b);  // constructor with byte parameter adjusts to range

            (b, offset) = Util.GetNextByte(data, offset);
            EnvelopeVelocityDepth = new DepthType(b);

            List<byte> envBytes = new List<byte>();
            (b, offset) = Util.GetNextByte(data, offset);
            envBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            envBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            envBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            envBytes.Add(b);
            Env = new FilterEnvelope(envBytes);

            List<byte> timeModBytes = new List<byte>();
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
            StringBuilder builder = new StringBuilder();
            builder.Append($"cutoff = {Cutoff}, resonance = {Resonance}\n");
            builder.Append(string.Format("LFO = {0}\n", IsLFO ? "ON" : "OFF"));
            builder.Append($"envelope: {Env}\n");
            builder.Append(string.Format("cutoff modulation: velocity = {0}, pressure = {1}, key scaling = {2}\n", CutoffMod.VelocityDepth, CutoffMod.PressureDepth, CutoffMod.KeyScalingDepth));
            builder.Append(string.Format("time modulation: attack velocity = {0}, release velocity = {1}, key scaling = {2}\n", TimeMod.AttackVelocity, TimeMod.ReleaseVelocity, TimeMod.KeyScaling));
            return builder.ToString();
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();

            data.Add(Cutoff.ToByte());

            StringBuilder b104 = new StringBuilder("0000");
            b104.Append(IsLFO ? "1" : "0");
            int resonance = Resonance - 1;  // from 1...8 to 0...7
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
