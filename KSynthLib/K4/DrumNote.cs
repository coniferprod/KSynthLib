using System;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K4
{
    public class DrumSource
    {
        private WaveNumberType _wave; // 1~256 (stored as 0~255 distributed over two bytes)
        public ushort Wave
        {
            get => _wave.Value;
            set => _wave.Value = value;
        }

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
            _wave = new WaveNumberType(97);  // "KICK"
            _decay = new LevelType(100);
            _tune = new DepthType();
            _level = new LevelType(100);
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

            int source1WaveHigh = 0;
            int source2WaveHigh = 0;
            int source1WaveLow = 0;
            int source2WaveLow = 0;

            (b, offset) = Util.GetNextByte(data, offset);
            source1WaveHigh = b & 0x01;
            _outputSelect = new OutputSettingType((byte)(((b >> 4) & 0x07) + 1)); // 0...7 to 1...8

            (b, offset) = Util.GetNextByte(data, offset);
            source2WaveHigh = b;

            (b, offset) = Util.GetNextByte(data, offset);
            source1WaveLow = b & 0x7f;

            (b, offset) = Util.GetNextByte(data, offset);
            source2WaveLow = b & 0x7f;

            // Combine the wave select bits for source 1:
            string source1WaveBitString = string.Format("{0}{1}", Convert.ToString(source1WaveHigh, 2), Convert.ToString(source1WaveLow, 2));
            Source1.Wave = (ushort)(Convert.ToUInt16(source1WaveBitString, 2) + 1);

            // Then combine the wave select bits for source 2:
            string source2WaveBitString = string.Format("{0}{1}", Convert.ToString(source2WaveHigh, 2), Convert.ToString(source2WaveLow, 2));
            Source2.Wave = (ushort)(Convert.ToUInt16(source2WaveBitString, 2) + 1);

            (b, offset) = Util.GetNextByte(data, offset);
            Source1.Decay = b;

            (b, offset) = Util.GetNextByte(data, offset);
            Source2.Decay = b;

            (b, offset) = Util.GetNextByte(data, offset);
            Source1.Tune = (sbyte)(b - 50);

            (b, offset) = Util.GetNextByte(data, offset);
            Source2.Tune = (sbyte)(b - 50);

            (b, offset) = Util.GetNextByte(data, offset);
            Source1.Level = b;

            (b, offset) = Util.GetNextByte(data, offset);
            Source2.Level = b;

            (b, offset) = Util.GetNextByte(data, offset);
            Checksum = b;  // store checksum as we get it from SysEx
        }

        private byte[] CollectData()
        {
            List<byte> data = new List<byte>();

            byte source1High = 0;
            byte source1Low = 0;
            byte source2High = 0;
            byte source2Low = 0;

            (source1High, source1Low) = this.ConvertWaveSelectToHighAndLow(Source1.Wave);
            (source2High, source2Low) = this.ConvertWaveSelectToHighAndLow(Source2.Wave);

            byte outputSelect = (byte)(OutputSettingType.OutputNames.IndexOf(OutputSelect));

            byte d11 = (byte)((outputSelect << 4) | source1High);
            data.Add(d11);

            data.Add(source2High);
            data.Add(source1Low);
            data.Add(source2Low);

            data.Add(Source1.Decay);
            data.Add(Source2.Decay);
            data.Add((byte)(Source1.Tune + 50));  // store -50~50 as 0..100
            data.Add((byte)(Source2.Tune + 50));
            data.Add(Source1.Level);
            data.Add(Source2.Level);

            return data.ToArray();
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(CollectData());
            data.Add(Checksum);  // computed by property accessor
            return data.ToArray();
        }

        public (byte, byte) ConvertWaveSelectToHighAndLow(ushort waveNumber)
        {
            // Convert wave number to an 8-bit binary string representation:
            string waveBitString = Convert.ToString(waveNumber, 2).PadLeft(8, '0');

            // Get top bit, convert it to byte and use it as the MSB:
            byte high = Convert.ToByte(waveBitString.Substring(0, 1), 2);

            // Get all but the top bit, convert it to byte and use it as the LSB:
            byte low = Convert.ToByte(waveBitString.Substring(1), 2);

            return (high, low);
        }
    }

}
