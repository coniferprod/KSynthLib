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
            _startLevel = new SignedLevelType(b);

            (b, offset) = Util.GetNextByte(data, offset);
            _attackTime = new PositiveLevelType(b);

            (b, offset) = Util.GetNextByte(data, offset);
            _attackLevel = new SignedLevelType(b);

            (b, offset) = Util.GetNextByte(data, offset);
            _decayTime = new PositiveLevelType(b);

            (b, offset) = Util.GetNextByte(data, offset);
            _timeVelocitySensitivity = new SignedLevelType(b);

            (b, offset) = Util.GetNextByte(data, offset);
            _levelVelocitySensitivity = new SignedLevelType(b);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("                  Pitch Envelope\n");
            builder.Append($"Strt L  {StartLevel,3}       Vel to\n");
            builder.Append($"Atak T  {AttackTime,3}     Level {LevelVelocitySensitivity,3}\n");
            builder.Append($"Atak L  {AttackLevel,3}     Time  {TimeVelocitySensitivity,3}\n");  // TODO: Sign
            builder.Append($"Decy T  {DecayTime,3}\n");
            return builder.ToString();
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();

            data.Add(_startLevel.AsByte());
            data.Add(AttackTime);
            data.Add(_attackLevel.AsByte());
            data.Add(DecayTime);
            data.Add(_timeVelocitySensitivity.AsByte());
            data.Add(_levelVelocitySensitivity.AsByte());

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

        private FixedKeyType _fixedKey; // 0=OFF, 21 ~ 108=ON(A-1 ~ C7)
        public byte FixedKey
        {
            get => _fixedKey.Value;
            set => _fixedKey.Value = value;
        }

        public KeyScalingToPitch KSPitch;  // enumeration
        public PitchEnvelope Envelope;

        public DCOSettings()
        {
            Envelope = new PitchEnvelope();
            _coarse = new CoarseType();
            _fine = new SignedLevelType();
            _fixedKey = new FixedKeyType();
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
            Console.Error.WriteLine(string.Format("wave kit MSB = {0:X2} | {1}, LSB = {2:X2} | {3}, combined = {4}, result = {5}",
                waveMSB, waveMSBBitString, waveLSB, waveLSBBitString, waveBitString, waveNumber));

            WaveNumber = waveNumber;

            (b, offset) = Util.GetNextByte(data, offset);
            _coarse = new CoarseType(b);

            (b, offset) = Util.GetNextByte(data, offset);
            _fine = new SignedLevelType(b);

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
            builder.Append("                        DCO\n");

            string waveName = "PCM";
            if (WaveNumber == AdditiveKit.WaveNumber)
            {
                waveName = "ADD";
            }
            builder.Append($"Wave Type = {waveName}    KS Pitch = {KSPitch}\n");

            string fixedKeySetting = FixedKey == 0 ? "OFF" : Convert.ToString(FixedKey);
            string waveNumber = "   ";
            if (waveName.Equals("PCM"))
            {
                //builder.Append(string.Format("PCM Wave No. {0} ({1})\n", Wave.Instance[WaveNumber], WaveNumber + 1));
                waveNumber = $"{Wave.Instance[WaveNumber]}";
            }
            builder.Append($"PCM Wave No.   {waveNumber}   Fixed Key = {fixedKeySetting}\n");

            builder.Append($"Coarse         {Coarse}\nFine          {Fine}\n\n");

            builder.Append($"{Envelope}\n");

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

            data.Add(_coarse.AsByte());
            data.Add(_fine.AsByte());
            data.Add((byte)FixedKey);
            data.Add((byte)KSPitch);

            data.AddRange(Envelope.ToData());

            return data.ToArray();
        }
    }
}
