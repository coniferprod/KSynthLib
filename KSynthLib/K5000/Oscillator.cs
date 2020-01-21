using System;
using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K5000
{
    public enum KeyScalingToPitch
    {
        ZeroCent,
        TwentyFiveCent,
        ThirtyThreeCent,
        FiftyCent
    }

    public class PitchEnvelope
    {
        public const int DataSize = 6;
        public int startLevel; // (-63)1 ~ (+63)127
        public int StartLevel
        {
            get
            {
                return this.startLevel;
            }

            set
            {
                if (value < -63 || value > 63)
                {
                    throw new ArgumentException("Start level must be -63...63");
                }
                this.startLevel = value;
            }
        }

        public int attackTime;   // 0 ~ 127
        public int AttackTime
        {
            get
            {
                return this.attackTime;
            }

            set
            {
                if (value < 0 || value > 127)
                {
                    throw new ArgumentException("Attack time must be 0...127");
                }
                this.attackTime = value;
            }
        }

        public int attackLevel;  // (-63)1 ~ (+63)127
        public int AttackLevel
        {
            get
            {
                return this.attackLevel;
            }

            set
            {
                if (value < -63 || value > 63)
                {
                    throw new ArgumentException("Attack level must be -63...63");
                }
                this.attackLevel = value;
            }
        }

        public int decayTime;          // 0 ~ 127

        public int DecayTime
        {
            get
            {
                return this.decayTime;
            }

            set
            {
                if (value < 0 || value > 127)
                {
                    throw new ArgumentException("Decay time must be 0...127");
                }
                this.decayTime = value;
            }
        }

        public int timeVelocitySensitivity; // (-63)1 ~ (+63)127

        public int TimeVelocitySensitivity
        {
            get
            {
                return this.timeVelocitySensitivity;
            }

            set
            {
                if (value < -63 || value > 63)
                {
                    throw new ArgumentException("Time velocity sensitivity must be -63...63");
                }
                this.timeVelocitySensitivity = value;
            }
        }

        public int levelVelocitySensitivity; // (-63)1 ~ (+63)127

        public int LevelVelocitySensitivity
        {
            get
            {
                return this.levelVelocitySensitivity;
            }

            set
            {
                if (value < -63 || value > 63)
                {
                    throw new ArgumentException("Level velocity sensitivity must be -63...63");
                }
                this.levelVelocitySensitivity = value;
            }
        }

        public PitchEnvelope()
        {
            StartLevel = 0;
            AttackTime = 64;
            AttackLevel = 63;
            DecayTime = 64;
            TimeVelocitySensitivity = 64;
            LevelVelocitySensitivity = 64;
        }

        public PitchEnvelope(byte[] data, int offset)
        {
            byte b = 0;
            (b, offset) = Util.GetNextByte(data, offset);
            StartLevel = (int)b - 64;
            (b, offset) = Util.GetNextByte(data, offset);
            AttackTime = b;
            (b, offset) = Util.GetNextByte(data, offset);
            AttackLevel = (int)b - 64;
            (b, offset) = Util.GetNextByte(data, offset);
            DecayTime = b;
            (b, offset) = Util.GetNextByte(data, offset);
            TimeVelocitySensitivity = (int)b - 64;
            (b, offset) = Util.GetNextByte(data, offset);
            LevelVelocitySensitivity = (int)b - 64;
        }

        public override string ToString()
        {
            return String.Format("Start Level = {0}, Atak T = {1}, Atak L = {2}, Dcay T = {3}", StartLevel, AttackTime, AttackLevel, DecayTime);
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();

            data.Add((byte)(StartLevel + 64));
            data.Add((byte)AttackTime);
            data.Add((byte)(AttackLevel + 64));
            data.Add((byte)DecayTime);
            data.Add((byte)(TimeVelocitySensitivity + 64));
            data.Add((byte)(LevelVelocitySensitivity + 64));

            return data.ToArray();
        }
    }

    public class DCOSettings
    {
        public int WaveNumber;

        private int coarse;
        public int Coarse
        {
            get
            {
                return this.coarse;
            }

            set
            {
                if (value < -24 || value > 24)
                {
                    throw new ArgumentException("Coarse must be -24...24");
                }
                this.coarse = value;
            }
        }

        private int fine;
        public int Fine
        {
            get
            {
                return this.fine;
            }

            set
            {
                if (value < -63 || value > 63)
                {
                    throw new ArgumentException("Fine must be -63...63");
                }
                this.fine = value;
            }
        }
        public byte FixedKey;  // 0=OFF, 21 ~ 108=ON(A-1 ~ C7)
        public KeyScalingToPitch KSPitch;
        public PitchEnvelope Envelope;

        public DCOSettings()
        {
            Envelope = new PitchEnvelope();
        }

        public DCOSettings(byte[] data, int offset)
        {
            byte b = 0;
            byte waveMSB = 0;
            (b, offset) = Util.GetNextByte(data, offset);
            waveMSB = b;

            byte waveLSB = 0;
            (b, offset) = Util.GetNextByte(data, offset);
            waveLSB = b;

            string waveMSBBitString = Convert.ToString(waveMSB, 2).PadLeft(3, '0');
            string waveLSBBitString = Convert.ToString(waveLSB, 2).PadLeft(7, '0');
            string waveBitString = waveMSBBitString + waveLSBBitString;
            int waveNumber = Convert.ToInt32(waveBitString, 2);
            System.Console.WriteLine(String.Format("wave kit MSB = {0:X2} | {1}, LSB = {2:X2} | {3}, combined = {4}, result = {5}", 
                waveMSB, waveMSBBitString, waveLSB, waveLSBBitString, waveBitString, waveNumber));

            WaveNumber = waveNumber;

            (b, offset) = Util.GetNextByte(data, offset);
            Coarse = (int)b - 24;
            (b, offset) = Util.GetNextByte(data, offset);
            Fine = (int)b - 64;
            (b, offset) = Util.GetNextByte(data, offset);
            FixedKey = b;
            (b, offset) = Util.GetNextByte(data, offset);
            KSPitch = (KeyScalingToPitch)b;

            Envelope = new PitchEnvelope(data, offset);
            offset += PitchEnvelope.DataSize;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            string waveName = "PCM";
            if (WaveNumber == AdditiveKit.WaveNumber)
            {
                waveName = "ADD";
            } 
            builder.Append(String.Format("Wave Type = {0}  ", waveName));
            if (waveName.Equals("PCM"))
            {
                builder.Append(String.Format("{0} ({1})\n", Wave.Instance[WaveNumber], WaveNumber + 1));
            }
            builder.Append(String.Format("Coarse = {0}  Fine = {1}\n", Coarse, Fine));
            builder.Append(String.Format("KS Pitch = {0}  Fixed Key = {1}\n", KSPitch, FixedKey == 0 ? "OFF" : Convert.ToString(FixedKey)));
            builder.Append(String.Format("Pitch Env: {0}\n", Envelope.ToString()));
            builder.Append(String.Format("Vel To: Level = {0}  Time = {1}\n", Envelope.LevelVelocitySensitivity, Envelope.TimeVelocitySensitivity));

            return builder.ToString();
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();

            // Convert wave kit number to binary string with 10 digits
            string waveBitString = Convert.ToString(WaveNumber, 2).PadLeft(10, '0');
            string msbBitString = waveBitString.Substring(0, 3);
            data.Add(Convert.ToByte(msbBitString, 2));
            string lsbBitString = waveBitString.Substring(3);
            data.Add(Convert.ToByte(lsbBitString, 2));

            data.Add((byte)(Coarse + 24));
            data.Add((byte)(Fine + 64));
            data.Add((byte)FixedKey);
            data.Add((byte)KSPitch);

            data.AddRange(Envelope.ToData());

            return data.ToArray();
        }
    }
}
