using System;
using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K4
{
    public class Filter
    {
        public const int DataSize = 14;

        private LevelType _cutoff;
        public byte Cutoff  // 0~100
        {
            get => _cutoff.Value;
            set => _cutoff.Value = value;
        }

        private ResonanceType _resonance; // 0 ~ 7 / 1 ~ 8
        public byte Resonance
        {
            get => _resonance.Value;
            set => _resonance.Value = value;
        }

        public LevelModulation CutoffMod;

        public bool IsLFO;  // 0/off, 1/on

        public FilterEnvelope Env;

        private DepthType _envelopeDepth;
        public sbyte EnvelopeDepth // 0 ~ 100 (±50)
        {
            get => _envelopeDepth.Value;
            set => _envelopeDepth.Value = value;
        }

        private DepthType _envelopeVelocityDepth;
        public sbyte EnvelopeVelocityDepth // 0 ~ 100 (±50)
        {
            get => _envelopeVelocityDepth.Value;
            set => _envelopeVelocityDepth.Value = value;
        }

        public TimeModulation TimeMod;

        public Filter()
        {
            _cutoff = new LevelType(88);
            _resonance = new ResonanceType();
            CutoffMod = new LevelModulation();
            IsLFO = false;
            Env = new FilterEnvelope();
            _envelopeDepth = new DepthType();
            _envelopeVelocityDepth = new DepthType();
            TimeMod = new TimeModulation();
        }

        public Filter(byte[] data) : this()
        {
            int offset = 0;
            byte b = 0;  // will be reused when getting the next byte

            (b, offset) = Util.GetNextByte(data, offset);
            _cutoff = new LevelType((byte)(b & 0x7f));

            (b, offset) = Util.GetNextByte(data, offset);
            _resonance = new ResonanceType((byte)((b & 0x07) + 1));  // from 0...7 to 1...8
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
            _envelopeDepth = new DepthType(b);  // constructor with byte parameter adjusts to range

            (b, offset) = Util.GetNextByte(data, offset);
            _envelopeVelocityDepth = new DepthType(b);

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

            data.Add(Cutoff);

            StringBuilder b104 = new StringBuilder("0000");
            b104.Append(IsLFO ? "1" : "0");
            int resonance = Resonance - 1;  // from 1...8 to 0...7
            string resonanceString = Convert.ToString(resonance, 2);
            //Console.Error.WriteLine(string.Format("Filter resonance = {0}, as bit string = '{1}'", resonance, resonanceString));
            b104.Append(resonanceString.PadLeft(3, '0'));
            data.Add(Convert.ToByte(b104.ToString(), 2));

            data.AddRange(CutoffMod.ToData());

            data.Add(_envelopeDepth.AsByte());
            data.Add(_envelopeVelocityDepth.AsByte());

            data.AddRange(Env.ToData());
            data.AddRange(TimeMod.ToData());

            return data.ToArray();
        }
    }
}