using System;
using System.Collections.Generic;
using System.Text;

using KSynthLib.Common;

namespace KSynthLib.K1
{
    public class Envelope
    {
        public int Attack;
        public int Decay;
        public int Sustain;
        public int Release;

        public Envelope()
        {
        }

        public Envelope(int a, int d, int s, int r)
        {
            Attack = a;
            Decay = d;
            Sustain = s;
            Release = r;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(string.Format("A:{0} D:{1} S:{2} R:{3}", Attack, Decay, Sustain, Release));
            return builder.ToString();
        }

       public byte[] ToData()
        {
            List<byte> buf = new List<byte>();
            buf.Add((byte)Attack);
            buf.Add((byte)Decay);
            buf.Add((byte)Sustain);
            buf.Add((byte)Release);
            return buf.ToArray();
        }
    }

    public class Source
    {
        public int Fine;
        public int Coarse;  // meaningful only if key tracking is on: 60 ~ 108 / ±24
        public int FixedKey; // if key tracking is off, represents the fixed key:0 ~ 127 / C-4 ~ G6
        public int WaveNumber;  // 0 ~ 255 / 1 ~ 256
        public bool IsKeyTrack;
        public bool IsVibratoAutoBend;
        public bool IsPressureToFrequency;
        public byte VelocityCurve;
        public byte EnvelopeLevel;
        public byte EnvelopeDelay;
        public Envelope Env;
        public int VelocityEnvelopeLevel;
        public int PressureEnvelopeLevel;
        public int KeyScalingEnvelopeLevel;
        public int VelocityEnvelopeTime;
        public int KeyScalingEnvelopeTime;
        public int KeyScalingToFrequency;

        public Source(byte[] data)
        {
            int offset = 0;
            byte b = 0;  // will be reused when getting the next byte
            (b, offset) = Util.GetNextByte(data, offset);
            Fine = b;

            (b, offset) = Util.GetNextByte(data, offset);
            // Assume that key tracking is on until we know better
            Coarse = b;
            FixedKey = b;

            (b, offset) = Util.GetNextByte(data, offset);  // wave select l
            int waveLow = b & 0x7f;
            (b, offset) = Util.GetNextByte(data, offset);  // wave select h
            int waveHigh = b & 0x01;
            string waveLowBitString = Convert.ToString(waveLow, 2);
            string waveHighBitString = Convert.ToString(waveHigh, 2);
            string waveKitBitString = waveLowBitString + waveHighBitString;
            WaveNumber = Convert.ToInt32(waveKitBitString, 2);

            // Wave select hi contains also some Boolean flags and the velocity curve:
            IsKeyTrack = b.IsBitSet(1);
            IsVibratoAutoBend = b.IsBitSet(2);
            IsPressureToFrequency = b.IsBitSet(3);
            int curve = (b >> 4) & 0x07;
            VelocityCurve = (byte)curve;

            (b, offset) = Util.GetNextByte(data, offset);
            EnvelopeLevel = b;

            (b, offset) = Util.GetNextByte(data, offset);
            EnvelopeDelay = b;

            (b, offset) = Util.GetNextByte(data, offset);
            int attack = b;

            (b, offset) = Util.GetNextByte(data, offset);
            int decay = b;

            (b, offset) = Util.GetNextByte(data, offset);
            int sustain = b;

            (b, offset) = Util.GetNextByte(data, offset);
            int release = b;

            Env = new Envelope(attack, decay, sustain, release);

            (b, offset) = Util.GetNextByte(data, offset);
            VelocityEnvelopeLevel = b;

            (b, offset) = Util.GetNextByte(data, offset);
            PressureEnvelopeLevel = b;

            (b, offset) = Util.GetNextByte(data, offset);
            KeyScalingEnvelopeLevel = b;

            (b, offset) = Util.GetNextByte(data, offset);
            VelocityEnvelopeTime = b;

            (b, offset) = Util.GetNextByte(data, offset);
            KeyScalingEnvelopeTime = b;

            (b, offset) = Util.GetNextByte(data, offset);
            KeyScalingToFrequency = b;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            string coarseString = "N/A";
            if (IsKeyTrack)
            {
                coarseString = string.Format("{0}", Coarse - 84);  // 60 ~ 108 = ±24  // TODO: is this right?
            }
            builder.Append(string.Format("fine = {0}, coarse = {1}\n", Fine - 50, coarseString));
            builder.Append(string.Format("wave = {0} ({1})\n", Wave.Instance[WaveNumber], WaveNumber + 1));
            builder.Append(string.Format("key tracking = {0}, vib>a.bend = {1}, pres>freq = {2}\n", IsKeyTrack ? "on" : "off", IsVibratoAutoBend ? "on" : "off", IsPressureToFrequency ? "on" : "off"));
            builder.Append(string.Format("velocity curve = {0}\n", VelocityCurve + 1));
            builder.Append(string.Format("ENV = {0}, level = {1}, delay = {2}\n", Env.ToString(), EnvelopeLevel, EnvelopeDelay));
            builder.Append(string.Format("vel-env: level = {0}, time = {1}\n", VelocityEnvelopeLevel - 50, VelocityEnvelopeTime - 50));
            builder.Append(string.Format("prs-env: level = {0}\n", PressureEnvelopeLevel - 50));
            builder.Append(string.Format("KS-env: level = {0}, time = {1}\n", KeyScalingEnvelopeLevel - 50, KeyScalingEnvelopeTime - 50));
            return builder.ToString();
        }

        public byte[] ToData()
        {
            List<byte> buf = new List<byte>();

            buf.Add((byte)Fine);
            if (IsKeyTrack)
            {
                buf.Add((byte)Coarse);
            }
            else
            {
                buf.Add((byte)FixedKey);
            }

            string waveNumberString = Convert.ToString(WaveNumber, 2).PadLeft(7, '0');  // wave number as binary
            string s31 = waveNumberString.Substring(1);  // the last 7 bits only
            buf.Add(Convert.ToByte(s31, 2));
            string s35 =
                ((byte)VelocityCurve).ToBinaryString(3) +
                (IsPressureToFrequency ? "1" : "0") +
                (IsVibratoAutoBend ? "1" : "0") +
                (IsKeyTrack ? "1" : "0") +
                waveNumberString.Substring(0, 1);
            buf.Add(Convert.ToByte(s35, 2));

            buf.Add(EnvelopeLevel);
            buf.Add(EnvelopeDelay);

            foreach (byte b in Env.ToData())
            {
                buf.Add(b);
            }

            buf.Add((byte)VelocityEnvelopeLevel);
            buf.Add((byte)PressureEnvelopeLevel);
            buf.Add((byte)KeyScalingEnvelopeLevel);
            buf.Add((byte)VelocityEnvelopeTime);
            buf.Add((byte)KeyScalingEnvelopeTime);
            buf.Add((byte)KeyScalingToFrequency);

            return buf.ToArray();
        }
    }
}
