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

    public class DCOSettings
    {
        public Wave Wave;

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
            Wave = new Wave();
            Envelope = new PitchEnvelope();
            _coarse = new CoarseType();
            _fine = new SignedLevelType();
            _fixedKey = new FixedKeyType();
        }

        public DCOSettings(byte[] data, int offset)
        {
            byte b = 0;

            (b, offset) = Util.GetNextByte(data, offset);
            var waveMSB = b;

            (b, offset) = Util.GetNextByte(data, offset);
            var waveLSB = b;

            Wave = new Wave(waveMSB, waveLSB);

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
            var builder = new StringBuilder();
            builder.Append("                        DCO\n");

            var waveName = "PCM";
            if (Wave.IsAdditive())
            {
                waveName = "ADD";
            }
            builder.Append($"Wave Type = {waveName}    KS Pitch = {KSPitch}\n");

            var fixedKeySetting = FixedKey == 0 ? "OFF" : Convert.ToString(FixedKey);
            var waveNumber = "   ";
            if (waveName.Equals("PCM"))
            {
                //builder.Append(string.Format("PCM Wave No. {0} ({1})\n", Wave.Instance[WaveNumber], WaveNumber + 1));
                waveNumber = $"{Wave.Number}";
            }
            builder.Append($"PCM Wave No.   {waveNumber}   Fixed Key = {fixedKeySetting}\n");

            builder.Append($"Coarse         {Coarse}\nFine          {Fine}\n\n");

            builder.Append($"{Envelope}\n");

            return builder.ToString();
        }

        public byte[] ToData()
        {
            var data = new List<byte>();

            data.AddRange(Wave.ToData());

            data.Add(_coarse.Byte);
            data.Add(_fine.Byte);
            data.Add((byte)FixedKey);
            data.Add((byte)KSPitch);

            data.AddRange(Envelope.ToData());

            return data.ToArray();
        }
    }
}
