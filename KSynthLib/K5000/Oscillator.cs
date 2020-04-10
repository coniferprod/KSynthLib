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

        private SignedLevelType _startLevel; // (-63)1 ~ (+63)127
        public sbyte StartLevel
        {
            get => _startLevel.Value;
            set => _startLevel.Value = value;
        }

        private PositiveLevelType _attackTime; // 0 ~ 127
        public byte AttackTime
        {
            get => _attackTime.Value;
            set => _attackTime.Value = value;
        }

        private SignedLevelType _attackLevel; // (-63)1 ~ (+63)127
        public sbyte AttackLevel
        {
            get => _attackLevel.Value;
            set => _attackLevel.Value = value;
        }

        private PositiveLevelType _decayTime; // 0 ~ 127
        public byte DecayTime
        {
            get => _decayTime.Value;
            set => _decayTime.Value = value;
        }

        private SignedLevelType _timeVelocitySensitivity; // (-63)1 ~ (+63)127
        public sbyte TimeVelocitySensitivity
        {
            get => _timeVelocitySensitivity.Value;
            set => _timeVelocitySensitivity.Value = value;
        }

        private SignedLevelType _levelVelocitySensitivity; // (-63)1 ~ (+63)127
        public sbyte LevelVelocitySensitivity
        {
            get => _levelVelocitySensitivity.Value;
            set => _levelVelocitySensitivity.Value = value;
        }

        public PitchEnvelope()
        {
            _startLevel = new SignedLevelType();
            _attackTime = new PositiveLevelType();
            _attackLevel = new SignedLevelType(63);
            _decayTime = new PositiveLevelType(64);
            _timeVelocitySensitivity = new SignedLevelType();
            _levelVelocitySensitivity = new SignedLevelType();
        }

        public PitchEnvelope(byte[] data, int offset)
        {
            byte b = 0;

            (b, offset) = Util.GetNextByte(data, offset);
            _startLevel = new SignedLevelType((sbyte)(b - 64));

            (b, offset) = Util.GetNextByte(data, offset);
            _attackTime = new PositiveLevelType(b);

            (b, offset) = Util.GetNextByte(data, offset);
            _attackLevel = new SignedLevelType((sbyte)(b - 64));

            (b, offset) = Util.GetNextByte(data, offset);
            _decayTime = new PositiveLevelType(b);

            (b, offset) = Util.GetNextByte(data, offset);
            _timeVelocitySensitivity = new SignedLevelType((sbyte)(b - 64));

            (b, offset) = Util.GetNextByte(data, offset);
            _levelVelocitySensitivity = new SignedLevelType((sbyte)(b - 64));
        }

        public override string ToString()
        {
            return $"Start Level = {StartLevel}, Atak T = {AttackTime}, Atak L = {AttackLevel}, Dcay T = {DecayTime}";
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

        private CoarseType _coarse;
        public int Coarse
        {
            get => _coarse.Value;
            set => _coarse.Value = value; 
        }

        private SignedLevelType _fine;
        public sbyte Fine
        {
            get => _fine.Value;
            set => _fine.Value = value;
        }

        public byte FixedKey;  // 0=OFF, 21 ~ 108=ON(A-1 ~ C7)
        public KeyScalingToPitch KSPitch;
        public PitchEnvelope Envelope;

        public DCOSettings()
        {
            Envelope = new PitchEnvelope();
            _coarse = new CoarseType();
            _fine = new SignedLevelType();
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
            System.Console.WriteLine(string.Format("wave kit MSB = {0:X2} | {1}, LSB = {2:X2} | {3}, combined = {4}, result = {5}", 
                waveMSB, waveMSBBitString, waveLSB, waveLSBBitString, waveBitString, waveNumber));

            WaveNumber = waveNumber;

            (b, offset) = Util.GetNextByte(data, offset);
            _coarse = new CoarseType(b - 24);

            (b, offset) = Util.GetNextByte(data, offset);
            _fine = new SignedLevelType((sbyte)(b - 64));

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
            builder.Append($"Wave Type = {waveName}  ");
            if (waveName.Equals("PCM"))
            {
                builder.Append(string.Format("{0} ({1})\n", Wave.Instance[WaveNumber], WaveNumber + 1));
            }
            builder.Append($"Coarse = {Coarse}  Fine = {Fine}\n");
            string fixedKeySetting = FixedKey == 0 ? "OFF" : Convert.ToString(FixedKey);
            builder.Append($"KS Pitch = {KSPitch}  Fixed Key = {fixedKeySetting}\n");
            builder.Append($"Pitch Env: {Envelope}\n");
            builder.Append($"Vel To: Level = {Envelope.LevelVelocitySensitivity}  Time = {Envelope.TimeVelocitySensitivity}\n");

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
