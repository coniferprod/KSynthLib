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
        public int Cutoff  // 0~100
        {
            get
            {
                return _cutoff.Value;
            }

            set
            {
                _cutoff.Value = value;
            }
        }

        private EightLevelType _resonance; // 0 ~ 7 / 1 ~ 8
        public int Resonance
        {
            get
            {
                return _resonance.Value;
            }

            set
            {
                _resonance.Value = value;
            }
        }

        public LevelModulation CutoffMod;

        public bool IsLFO;  // 0/off, 1/on

        public Envelope Env;

        private DepthType envelopeDepth;
        public int EnvelopeDepth // 0 ~ 100 (±50)
        {
            get
            {
                return envelopeDepth.Value;
            }

            set 
            {
                envelopeDepth.Value = value;
            }
        }

        private DepthType envelopeVelocityDepth;
        public int EnvelopeVelocityDepth // 0 ~ 100 (±50)
        {
            get
            {
                return envelopeVelocityDepth.Value;
            }

            set
            {
                envelopeVelocityDepth.Value = value;
            }
        }

        public TimeModulation TimeMod;

        public Filter()
        {
            _cutoff = new LevelType(88);
            _resonance = new EightLevelType();
            CutoffMod = new LevelModulation();
            IsLFO = false;
            Env = new Envelope();
            envelopeDepth = new DepthType();
            envelopeVelocityDepth = new DepthType();
            TimeMod = new TimeModulation();
        }

        public Filter(byte[] data)
        {
            int offset = 0;
            byte b = 0;  // will be reused when getting the next byte

            (b, offset) = Util.GetNextByte(data, offset);
            _cutoff = new LevelType(b & 0x7f);

            (b, offset) = Util.GetNextByte(data, offset);
            _resonance = new EightLevelType((b & 0x07) + 1);  // from 0...7 to 1...8
            IsLFO = b.IsBitSet(3);

            CutoffMod = new LevelModulation();
            (b, offset) = Util.GetNextByte(data, offset);
            CutoffMod.VelocityDepth = (b & 0x7f) - 50;

            (b, offset) = Util.GetNextByte(data, offset);
            CutoffMod.PressureDepth = (b & 0x7f) - 50;

            (b, offset) = Util.GetNextByte(data, offset);
            CutoffMod.KeyScalingDepth = (b & 0x7f) - 50;

            (b, offset) = Util.GetNextByte(data, offset);
            envelopeDepth = new DepthType((b & 0x7f) - 50);

            (b, offset) = Util.GetNextByte(data, offset);
            envelopeVelocityDepth = new DepthType((b & 0x7f) - 50);

            Env = new Envelope();
            (b, offset) = Util.GetNextByte(data, offset);
            Env.Attack = b & 0x7f;

            (b, offset) = Util.GetNextByte(data, offset);
            Env.Decay = b & 0x7f;

            (b, offset) = Util.GetNextByte(data, offset);
            Env.Sustain = b & 0x7f;
            
            (b, offset) = Util.GetNextByte(data, offset);
            Env.Release = b & 0x7f;

            TimeMod = new TimeModulation();
            (b, offset) = Util.GetNextByte(data, offset);
            TimeMod.AttackVelocity = (b & 0x7f) - 50;

            (b, offset) = Util.GetNextByte(data, offset);
            TimeMod.ReleaseVelocity = (b & 0x7f) - 50;

            (b, offset) = Util.GetNextByte(data, offset);
            TimeMod.KeyScaling = (b & 0x7f) - 50;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(String.Format("cutoff = {0}, resonance = {1}\n", Cutoff, Resonance));
            builder.Append(String.Format("LFO = {0}\n", IsLFO ? "ON" : "OFF"));
            builder.Append(String.Format("envelope: {0}\n", Env.ToString()));
            builder.Append(String.Format("cutoff modulation: velocity = {0}, pressure = {1}, key scaling = {2}\n", CutoffMod.VelocityDepth, CutoffMod.PressureDepth, CutoffMod.KeyScalingDepth));
            builder.Append(String.Format("time modulation: attack velocity = {0}, release velocity = {1}, key scaling = {2}\n", TimeMod.AttackVelocity, TimeMod.ReleaseVelocity, TimeMod.KeyScaling));
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
            data.Add((byte)Env.Attack);
            data.Add((byte)Env.Decay);
            data.Add((byte)Env.Sustain);
            data.Add((byte)Env.Release);
            data.Add((byte)(TimeMod.AttackVelocity + 50));
            data.Add((byte)(TimeMod.ReleaseVelocity + 50));
            data.Add((byte)(TimeMod.KeyScaling + 50));
                        
            return data.ToArray();
        }
    }
}