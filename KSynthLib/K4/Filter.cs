using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;

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

            CutoffMod = new LevelModulation();
            (b, offset) = Util.GetNextByte(data, offset);
            CutoffMod.VelocityDepth = (sbyte)((b & 0x7f) - 50);

            (b, offset) = Util.GetNextByte(data, offset);
            CutoffMod.PressureDepth = (sbyte)((b & 0x7f) - 50);

            (b, offset) = Util.GetNextByte(data, offset);
            CutoffMod.KeyScalingDepth = (sbyte)((b & 0x7f) - 50);

            (b, offset) = Util.GetNextByte(data, offset);
            _envelopeDepth = new DepthType((sbyte)((b & 0x7f) - 50));

            (b, offset) = Util.GetNextByte(data, offset);
            _envelopeVelocityDepth = new DepthType((sbyte)((b & 0x7f) - 50));

            Env = new FilterEnvelope();
            (b, offset) = Util.GetNextByte(data, offset);
            Env.Attack = (byte)(b & 0x7f);

            (b, offset) = Util.GetNextByte(data, offset);
            Env.Decay = (byte)(b & 0x7f);

            (b, offset) = Util.GetNextByte(data, offset);
            Env.Sustain = (sbyte)((b & 0x7f) - 50);

            (b, offset) = Util.GetNextByte(data, offset);
            Env.Release = (byte)(b & 0x7f);

            TimeMod = new TimeModulation();
            (b, offset) = Util.GetNextByte(data, offset);
            TimeMod.AttackVelocity = (sbyte)((b & 0x7f) - 50);

            (b, offset) = Util.GetNextByte(data, offset);
            TimeMod.ReleaseVelocity = (sbyte)((b & 0x7f) - 50);

            (b, offset) = Util.GetNextByte(data, offset);
            TimeMod.KeyScaling = (sbyte)((b & 0x7f) - 50);
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

            data.Add((byte)Cutoff);

            StringBuilder b104 = new StringBuilder("0000");
            b104.Append(IsLFO ? "1" : "0");
            int resonance = Resonance - 1;  // from 1...8 to 0...7
            string resonanceString = Convert.ToString(resonance, 2);
            //Debug.WriteLine(String.Format("Filter resonance = {0}, as bit string = '{1}'", resonance, resonanceString));
            b104.Append(resonanceString.PadLeft(3, '0'));
            data.Add(Convert.ToByte(b104.ToString(), 2));

            data.Add((byte)(CutoffMod.VelocityDepth + 50));
            data.Add((byte)(CutoffMod.PressureDepth + 50));
            data.Add((byte)(CutoffMod.KeyScalingDepth + 50));
            data.Add((byte)(EnvelopeDepth + 50));
            data.Add((byte)(EnvelopeVelocityDepth + 50));

            data.AddRange(Env.ToData());

            data.Add((byte)(TimeMod.AttackVelocity + 50));
            data.Add((byte)(TimeMod.ReleaseVelocity + 50));
            data.Add((byte)(TimeMod.KeyScaling + 50));

            return data.ToArray();
        }
    }
}