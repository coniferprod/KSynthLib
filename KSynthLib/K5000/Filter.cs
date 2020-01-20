using System;
using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K5000
{
    public enum FilterMode
    {
        LowPass,
        HighPass
    }

    // Same as amp envelope, but decay levels 1...127 are interpreted as -63 ... 63
    public class FilterEnvelope
    {
        public const int DataSize = 6;

        private int attackTime; // 0~127
        public int AttackTime
        {
            get
            {
                return this.attackTime;
            }

            set
            {
                if (value < 0 || value > 127)
                {
                    throw new ArgumentException("Attack time must be 0...127");
                }
                this.attackTime = value;
            }
        }

        private int decay1Time;
        public int Decay1Time
        {
            get
            {
                return this.decay1Time;
            }

            set
            {
                if (value < 0 || value > 127)
                {
                    throw new ArgumentException("Decay 1 time must be 0...127");
                }
                this.decay1Time = value;
            }
        }

        private int decay1Level;
        public int Decay1Level
        {
            get
            {
                return this.decay1Level;
            }

            set
            {
                if (value < -63 || value > 63)
                {
                    throw new ArgumentException("Decay 1 level must be -63...63");
                }
                this.decay1Level = value;
            }
        }

        private int decay2Time;
        public int Decay2Time
        {
            get
            {
                return this.decay2Time;
            }

            set
            {
                if (value < 0 || value > 127)
                {
                    throw new ArgumentException("Decay 2 time must be 0...127");
                }
                this.decay2Time = value;
            }
        }

        private int decay2Level;
        public int Decay2Level
        {
            get
            {
                return this.decay2Level;
            }

            set
            {
                if (value < -63 || value > 63)
                {
                    throw new ArgumentException("Decay 2 level must be -63...63");
                }
                this.decay2Level = value;
            }
        }

        private int releaseTime; // 0~127
        public int ReleaseTime
        {
            get
            {
                return this.releaseTime;
            }

            set
            {
                if (value < 0 || value > 127)
                {
                    throw new ArgumentException("Release time must be 0...127");
                }
                this.releaseTime = value;
            }
        }

        public FilterEnvelope(byte[] data, int offset)
        {
            byte b = 0;  // will be reused when getting the next byte

            (b, offset) = Util.GetNextByte(data, offset);
            AttackTime = b;

            (b, offset) = Util.GetNextByte(data, offset);
            Decay1Time = b;

            (b, offset) = Util.GetNextByte(data, offset);
            Decay1Level = (int)b - 64;

            (b, offset) = Util.GetNextByte(data, offset);
            Decay2Time = b;

            (b, offset) = Util.GetNextByte(data, offset);
            Decay2Level = (int)b - 64;

            (b, offset) = Util.GetNextByte(data, offset);
            ReleaseTime = b;
        }

        public override string ToString()
        {
            return String.Format("A={0}, D1=T{1} L{2}, D2=T{3} L{4}, R={5}", AttackTime, Decay1Time, Decay1Level, Decay2Time, Decay2Level, ReleaseTime);
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();

            data.Add((byte)AttackTime);
            data.Add((byte)Decay1Time);
            data.Add((byte)(Decay1Level + 64));
            data.Add((byte)Decay2Time);
            data.Add((byte)(Decay2Level + 64));
            data.Add((byte)ReleaseTime);

            return data.ToArray();
        }
    }

    public class DCFSettings
    {
        public bool IsActive;
        public FilterMode Mode;

        private int velocityCurve;

        public int VelocityCurve   // 0 ~ 11 (1 ~ 12)
        {
            get
            {
                return this.velocityCurve;
            }

            set
            {
                if (value < 1 || value > 12)
                {
                    throw new ArgumentException("Velocity curve must be 1...12");
                }
                this.velocityCurve = value;
            }
        }

        private int resonance; // 0 ~ 7

        public int Resonance
        {
            get
            {
                return this.resonance;
            }

            set
            {
                if (value < 0 || value > 7)
                {
                    throw new ArgumentException("Resonance must be 0...7");
                }
                this.resonance = value;
            }
        }

        public int Level;

        private int cutoff;
        public int Cutoff
        {
            get
            {
                return this.cutoff;
            }

            set
            {
                if (value < 0 || value > 127)
                {
                    throw new ArgumentException("Cutoff must be 0...127");
                }
                this.cutoff = value;
            }
        }

        private int cutoffKeyScalingDepth;

        public int CutoffKeyScalingDepth // (-63)1 ~ (+63)127
        {
            get
            {
                return this.cutoffKeyScalingDepth;
            }

            set
            {
                if (value < -63 || value > 63)
                {
                    throw new ArgumentException("Cutoff KS depth must be -63...+63");
                }
                this.cutoffKeyScalingDepth = value;
            }
        }

        private int cutoffVelocityDepth; // (-63)1 ~ (+63)127
        public int CutoffVelocityDepth
        {
            get
            {
                return this.cutoffVelocityDepth;
            }

            set
            {
                if (value < -63 || value > 63)
                {
                    throw new ArgumentException("Cutoff vel depth must be -63...+63");
                }
                this.cutoffVelocityDepth = value;
            }
        }

        private int envelopeDepth; // (-63)1 ~ (+63)127
        public int EnvelopeDepth
        {
            get
            {
                return this.envelopeDepth;
            }

            set
            {
                if (value < -63 || value > 63)
                {
                    throw new ArgumentException("Envelope depth must be -63...+63");
                }
                this.envelopeDepth = value;
            }
        }


        public FilterEnvelope Envelope;

        private int keyScalingToEnvelopeAttackTime;
        public int KSToEnvAttackTime
        {
            get
            {
                return this.keyScalingToEnvelopeAttackTime;
            }

            set
            {
                if (value < -63 || value > 63)
                {
                    throw new ArgumentException("KS to Env Attack Time must be -63...+63");
                }
                this.keyScalingToEnvelopeAttackTime = value;
            }
        }

        private int keyScalingToEnvelopeDecay1Time;
        public int KSToEnvDecay1Time
        {
            get
            {
                return this.keyScalingToEnvelopeDecay1Time;
            }

            set
            {
                if (value < -63 || value > 63)
                {
                    throw new ArgumentException("KS to Env Decay 1 Time must be -63...+63");
                }
                this.keyScalingToEnvelopeDecay1Time = value;
            }
        }

        private int velocityToEnvelopeDepth;
        public int VelocityToEnvDepth
        {
            get
            {
                return this.velocityToEnvelopeDepth;
            }

            set
            {
                if (value < -63 || value > 63)
                {
                    throw new ArgumentException("Velo to Env Depth must be -63...+63");
                }
                this.velocityToEnvelopeDepth = value;
            }
        }

        private int velocityToEnvelopeAttackTime;
        public int VelocityToEnvAttackTime
        {
            get
            {
                return this.velocityToEnvelopeAttackTime;
            }

            set
            {
                if (value < -63 || value > 63)
                {
                    throw new ArgumentException("Velo to Env Attack Time must be -63...+63");
                }
                this.velocityToEnvelopeDepth = value;
            }
        }

        private int velocityToEnvelopeDecay1Time;
        public int VelocityToEnvDecay1Time
        {
            get
            {
                return this.velocityToEnvelopeDecay1Time;
            }

            set
            {
                if (value < -63 || value > 63)
                {
                    throw new ArgumentException("Velo to Env Attack Time must be -63...+63");
                }
                this.velocityToEnvelopeDecay1Time = value;
            }
        }

        public DCFSettings()
        {
            Envelope = new FilterEnvelope();
        }

        public DCFSettings(byte[] data, int offset)
        {
            byte b = 0;  // will be reused when getting the next byte
            (b, offset) = Util.GetNextByte(data, offset);
            IsActive = (b == 0);  // 0=Active, 1=Bypass

            (b, offset) = Util.GetNextByte(data, offset);
            Mode = (FilterMode)b;

            (b, offset) = Util.GetNextByte(data, offset);
            VelocityCurve = (int)b + 1;  // 0~11 (1~12)

            (b, offset) = Util.GetNextByte(data, offset);
            Resonance = (int)b;  // 0~7

            (b, offset) = Util.GetNextByte(data, offset);
            Level = b;  // 0~7 (7~0)

            (b, offset) = Util.GetNextByte(data, offset);
            Cutoff = (int)b;

            (b, offset) = Util.GetNextByte(data, offset);
            CutoffKeyScalingDepth = (int)b - 64;

            (b, offset) = Util.GetNextByte(data, offset);
            CutoffVelocityDepth = (int)b - 64;

            (b, offset) = Util.GetNextByte(data, offset);
            EnvelopeDepth = (int)b - 64;

            Envelope = new FilterEnvelope(data, offset);
            offset += FilterEnvelope.DataSize;

            (b, offset) = Util.GetNextByte(data, offset);
            KSToEnvAttackTime = (int)b - 64;

            (b, offset) = Util.GetNextByte(data, offset);
            KSToEnvDecay1Time = (int)b - 64;

            (b, offset) = Util.GetNextByte(data, offset);
            VelocityToEnvDepth = (int)b - 64;

            (b, offset) = Util.GetNextByte(data, offset);
            VelocityToEnvAttackTime = (int)b - 64;

            (b, offset) = Util.GetNextByte(data, offset);
            VelocityToEnvDecay1Time = (int)b - 64;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(String.Format("active = {0}, mode = {1}\n", IsActive ? "YES" : "NO", Mode));
            builder.Append(String.Format("velocity curve = {0}\n", VelocityCurve));
            builder.Append(String.Format("resonance = {0}, level = {1}, cutoff = {2}\n", Resonance, Level, Cutoff));
            builder.Append(String.Format("cutoff KS depth = {0}, cutoff vel depth = {1}\n", CutoffKeyScalingDepth, CutoffVelocityDepth));
            builder.Append(String.Format("envelope = {0}\n", Envelope.ToString()));
            builder.Append(String.Format("DCF Mod.: KS to Attack = {0}  KS to Dcy1 = {1}  Vel to Env = {2}  Vel to Atk = {3}  Vel to Dcy1 = {4}\n", 
                KSToEnvAttackTime, KSToEnvDecay1Time, VelocityToEnvDepth, VelocityToEnvAttackTime, VelocityToEnvDecay1Time));
            return builder.ToString();
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();

            data.Add((byte)(IsActive ? 1 : 0));
            data.Add((byte)Mode);
            data.Add((byte)(VelocityCurve - 1));  // adjust 1 ~ 12 to 0 ~ 11
            data.Add((byte)Resonance);
            data.Add((byte)Level);
            data.Add((byte)Cutoff);
            data.Add((byte)(CutoffKeyScalingDepth + 64));
            data.Add((byte)(CutoffVelocityDepth + 64));
            data.Add((byte)(EnvelopeDepth + 64));

            data.AddRange(Envelope.ToData());

            data.Add((byte)(KSToEnvAttackTime + 64));
            data.Add((byte)(KSToEnvDecay1Time + 64));
            data.Add((byte)(VelocityToEnvDepth + 64));
            data.Add((byte)(VelocityToEnvAttackTime + 64));
            data.Add((byte)(VelocityToEnvDecay1Time + 64));

            return data.ToArray();
        }
    }
}
