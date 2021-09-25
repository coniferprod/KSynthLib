using System;
using System.Collections.Generic;
using System.Text;

using KSynthLib.Common;

namespace KSynthLib.K4
{
    public class DrumSource
    {
        public const int DataSize = 5;

        public Wave Wave;
        public LevelType Decay;
        public DepthType Tune; // 0~100 / ±50
        private LevelType Level;  // manual says 0...100, SysEx spec says 0...99

        public DrumSource()
        {
            Wave = new Wave(97);  // "KICK"
            Decay = new LevelType(99);
            Tune = new DepthType();
            Level = new LevelType(99);
        }

        public DrumSource(byte[] data) : this()
        {
            byte waveHigh = (byte)(data[0] & 0x01);
            byte waveLow = (byte)(data[1] & 0x7f);
            Wave = new Wave(waveHigh, waveLow);

            Decay = new LevelType(data[2]);
            Tune = new DepthType(data[3]);
            Level = new LevelType(data[4]);
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();

            byte high = 0x00;
            byte low = 0x00;
            (high, low) = Wave.WaveSelect;
            data.Add(high);
            data.Add(low);

            data.Add(Decay.ToByte());
            data.Add(Tune.ToByte());
            data.Add(Level.ToByte());

            return data.ToArray();
        }

        /// <summary>
        /// Generates a printable representation of this drum source.
        /// </summary>
        /// <returns>
        /// String with note parameter values.
        /// </returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append($"WAVE = {this.Wave.ToString()}" + "\n");
            builder.Append($"Decay = {this.Decay}, Tune = {this.Tune}, Level = {this.Level}\n");

            return builder.ToString();
        }
    }

    public class DrumNote
    {
        public const int DataSize = 11;  // ten bytes plus checksum

        public SubmixType OutputSelect;
        public DrumSource Source1;
        public DrumSource Source2;

        private byte _checksum;
        public byte Checksum
        {
            get
            {
                byte[] bs = CollectData();
                byte sum = 0;
                foreach (byte b in bs)
                {
                    sum += b;
                }
                sum += 0xA5;
                return sum;
            }

            set => _checksum = value;
        }

        public DrumNote()
        {
            OutputSelect = SubmixType.A;
            Source1 = new DrumSource();
            Source2 = new DrumSource();
        }

        public DrumNote(byte[] data) : this()
        {
            int offset = 0;
            byte b = 0;  // will be reused when getting the next byte

            List<byte> source1Bytes = new List<byte>();
            List<byte> source2Bytes = new List<byte>();

            (b, offset) = Util.GetNextByte(data, offset);
            source1Bytes.Add((byte)(b & 0x01));

            OutputSelect = (SubmixType)((b >> 4) & 0x07);

            (b, offset) = Util.GetNextByte(data, offset);
            source2Bytes.Add(b);

            (b, offset) = Util.GetNextByte(data, offset);
            source1Bytes.Add((byte)(b & 0x7f));

            (b, offset) = Util.GetNextByte(data, offset);
            source2Bytes.Add((byte)(b & 0x7f));

            (b, offset) = Util.GetNextByte(data, offset);
            source1Bytes.Add(b);

            (b, offset) = Util.GetNextByte(data, offset);
            source2Bytes.Add(b);

            (b, offset) = Util.GetNextByte(data, offset);
            source1Bytes.Add(b);

            (b, offset) = Util.GetNextByte(data, offset);
            source2Bytes.Add(b);

            (b, offset) = Util.GetNextByte(data, offset);
            source1Bytes.Add(b);

            (b, offset) = Util.GetNextByte(data, offset);
            source2Bytes.Add(b);

            Source1 = new DrumSource(source1Bytes.ToArray());
            Source2 = new DrumSource(source2Bytes.ToArray());

            (b, offset) = Util.GetNextByte(data, offset);
            Checksum = b;  // store checksum as we get it from SysEx
        }

        private byte[] CollectData()
        {
            List<byte> data = new List<byte>();

            byte[] source1Bytes = this.Source1.ToData();
            byte outputSelect = (byte)OutputSelect;
            byte d11 = (byte)((outputSelect << 4) | source1Bytes[0]);
            source1Bytes[0] = d11;

            byte[] source2Bytes = this.Source2.ToData();

            for (int i = 0; i < source1Bytes.Length; i++)
            {
                data.Add(source1Bytes[i]);
                data.Add(source2Bytes[i]);
            }

            return data.ToArray();
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(CollectData());
            data.Add(Checksum);  // computed by property accessor
            return data.ToArray();
        }

        /// <summary>
        /// Generates a printable representation of this drum note.
        /// </summary>
        /// <returns>
        /// String with note parameter values.
        /// </returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(OutputSelect.ToString() + "\n");
            builder.Append($"SOURCE 1 = {this.Source1}\n");
            builder.Append($"SOURCE 2 = {this.Source2}\n");

            return builder.ToString();
        }
    }
}
