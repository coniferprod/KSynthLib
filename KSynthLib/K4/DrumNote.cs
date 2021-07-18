using System;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K4
{
    public class DrumSource
    {
        public const int DataSize = 5;

        public Wave Wave;

        private LevelType _decay;
        public byte Decay
        {
            get => _decay.Value;
            set => _decay.Value = value;
        }

        private DepthType _tune; // 0~100 / ±50
        public sbyte Tune
        {
            get => _tune.Value;
            set => _tune.Value = value;
        }

        private LevelType _level;  // manual says 0...100, SysEx spec says 0...99
        public byte Level
        {
            get => _level.Value;
            set => _level.Value = value;
        }

        public DrumSource()
        {
            Wave = new Wave(97);  // "KICK"
            _decay = new LevelType(100);
            _tune = new DepthType();
            _level = new LevelType(100);
        }

        public DrumSource(byte[] data) : this()
        {
            byte waveHigh = (byte)(data[0] & 0x01);
            byte waveLow = (byte)(data[1] & 0x7f);
            Wave = new Wave(waveHigh, waveLow);

            Decay = data[2];
            _tune = new DepthType(data[3]);
            Level = data[4];
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();

            byte high = 0x00;
            byte low = 0x00;
            (high, low) = Wave.WaveSelect;
            data.Add(high);
            data.Add(low);

            data.Add(Decay);
            data.Add(_tune.AsByte());
            data.Add(Level);

            return data.ToArray();
        }
    }

    public class DrumNote
    {
        public const int DataSize = 11;  // ten bytes plus checksum

        private OutputSettingType _outputSelect;
        public char OutputSelect
        {
            get => OutputSettingType.OutputNames[_outputSelect.Value];
            set => _outputSelect.Value = OutputSettingType.OutputNames.IndexOf(value);
        }

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
            _outputSelect = new OutputSettingType(0);
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

            _outputSelect = new OutputSettingType((byte)((b >> 4) & 0x07));

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
            byte outputSelect = (byte)(OutputSettingType.OutputNames.IndexOf(OutputSelect));
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
    }
}
