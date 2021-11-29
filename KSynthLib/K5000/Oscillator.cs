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
        public Coarse Coarse;
        public SignedLevel Fine;
        public FixedKey FixedKey; // 0=OFF, 21 ~ 108=ON(A-1 ~ C7)
        public KeyScalingToPitch KSPitch;  // enumeration
        public PitchEnvelope Envelope;

        public DCOSettings()
        {
            Wave = new Wave();
            Envelope = new PitchEnvelope();
            Coarse = new Coarse();
            Fine = new SignedLevel();
            FixedKey = new FixedKey();
        }

        public DCOSettings(byte[] data, int offset)
        {
            byte b = 0;

            (b, offset) = Util.GetNextByte(data, offset);
            var waveMSB = b;

            (b, offset) = Util.GetNextByte(data, offset);
            var waveLSB = b;

            ushort waveNumber = Wave.NumberFrom(waveMSB, waveLSB);
            //Console.Error.WriteLine($"wave MSB = {waveMSB:X2}h, wave LSB = {waveLSB:X2}h, wave number = {waveNumber}");
            Wave = new Wave(waveMSB, waveLSB);

            (b, offset) = Util.GetNextByte(data, offset);
            Coarse = new Coarse(b);

            (b, offset) = Util.GetNextByte(data, offset);
            Fine = new SignedLevel(b);

            (b, offset) = Util.GetNextByte(data, offset);
            FixedKey = new FixedKey(b);

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

            string fixedKeySetting = "OFF";
            if (FixedKey.IsOn)
            {
                fixedKeySetting = string.Format("{0}", FixedKey.Key.Value);
            }

            var waveNumber = "   ";
            if (waveName.Equals("PCM"))
            {
                //builder.Append(string.Format("PCM Wave No. {0} ({1})\n", Wave.Instance[WaveNumber], WaveNumber + 1));
                waveNumber = $"{Wave.Number}";
            }

            builder.Append($"PCM Wave No.   {waveNumber}   Fixed Key = {fixedKeySetting}\n");
            builder.Append($"Coarse         {Coarse.Value}\nFine          {Fine.Value}\n\n");
            builder.Append($"{Envelope}\n");

            return builder.ToString();
        }

        public byte[] ToData()
        {
            var data = new List<byte>();

            data.AddRange(Wave.ToData());

            data.Add(Coarse.ToByte());
            data.Add(Fine.ToByte());
            data.Add(FixedKey.Key.ToByte());
            data.Add((byte)KSPitch);

            data.AddRange(Envelope.ToData());

            return data.ToArray();
        }
    }
}
