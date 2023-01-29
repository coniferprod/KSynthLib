using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

using KSynthLib.Common;

namespace KSynthLib.K4
{
    public class DrumSource: ISystemExclusiveData
    {
        public const int DataSize = 5;

        public Wave Wave;

        [Range(0, 100, ErrorMessage = "{0} must be between {1} and {2}")]
        public int Decay;

        [Range(-50, 50, ErrorMessage = "{0} must be between {1} and {2}")]
        public int Tune;

        [Range(0, 100, ErrorMessage = "{0} must be between {1} and {2}")]
        private int Level;  // manual says 0...100, SysEx spec says 0...99

        public DrumSource()
        {
            Wave = new Wave(97);  // "KICK"
            Decay = 99;
            Tune = 0;
            Level = 99;
        }

        public DrumSource(byte[] data) : this()
        {
            byte waveHigh = (byte)(data[0] & 0x01);
            byte waveLow = (byte)(data[1] & 0x7f);
            Wave = new Wave(waveHigh, waveLow);

            Decay = data[2];
            Tune = ByteConverter.DepthFromByte(data[3]);
            Level = data[4];
        }

        public List<byte> GetSystemExclusiveData()
        {
            var data = new List<byte>();

            byte high = 0x00;
            byte low = 0x00;
            (high, low) = Wave.WaveSelect;
            data.Add(high);
            data.Add(low);

            data.Add((byte)Decay);
            data.Add(ByteConverter.ByteFromDepth(Tune));
            data.Add((byte)Level);

            return data;
        }

        /// <summary>
        /// Generates a printable representation of this drum source.
        /// </summary>
        /// <returns>
        /// String with note parameter values.
        /// </returns>
        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append($"WAVE = {this.Wave.ToString()}" + "\n");
            builder.Append($"Decay = {this.Decay}, Tune = {this.Tune}, Level = {this.Level}\n");

            return builder.ToString();
        }
    }

    public class DrumNote : Patch, ISystemExclusiveData
    {
        public const int DataSize = 11;  // ten bytes plus checksum

        public SubmixType OutputSelect;
        public DrumSource Source1;
        public DrumSource Source2;

        private byte _checksum;
        public override byte Checksum
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

            var source1Bytes = new List<byte>();
            var source2Bytes = new List<byte>();

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

        protected override byte[] CollectData()
        {
            var data = new List<byte>();

            List<byte> source1Bytes = this.Source1.GetSystemExclusiveData();
            byte outputSelect = (byte)OutputSelect;
            byte d11 = (byte)((outputSelect << 4) | source1Bytes[0]);
            source1Bytes[0] = d11;

            List<byte> source2Bytes = this.Source2.GetSystemExclusiveData();

            for (var i = 0; i < source1Bytes.Count; i++)
            {
                data.Add(source1Bytes[i]);
                data.Add(source2Bytes[i]);
            }

            return data.ToArray();
        }

        public override byte[] ToData()
        {
            var data = new List<byte>();

            data.AddRange(this.CollectData());
            data.Add(Checksum);  // computed by property accessor

            return data.ToArray();
        }

        public List<byte> GetSystemExclusiveData()
        {
            var data = new List<byte>();

            data.AddRange(this.CollectData());
            data.Add(Checksum);  // computed by property accessor

            return data;
        }

        /// <summary>
        /// Generates a printable representation of this drum note.
        /// </summary>
        /// <returns>
        /// String with note parameter values.
        /// </returns>
        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append(OutputSelect.ToString() + "\n");
            builder.Append($"SOURCE 1 = {this.Source1}\n");
            builder.Append($"SOURCE 2 = {this.Source2}\n");

            return builder.ToString();
        }
    }
}
