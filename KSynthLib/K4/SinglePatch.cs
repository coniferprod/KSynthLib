using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;

using KSynthLib.Common;

namespace KSynthLib.K4
{
    public class SinglePatch : Patch
    {
        public const int DataSize = 131;

        private const string OutputNames = "ABCDEFGH";

        private LevelType _volume;
        public byte Volume  // 0~100
        {
            get => _volume.Value;
            set => _volume.Value = value;
        }

        private EffectNumberType _effect;
        public byte Effect  // 1~32 (on K4)
        {
            get => _effect.Value;
            set =>_effect.Value = value;
        }

        private OutputSettingType _output; // 0~7 / A~H (on K4r)
        public char Output
        {
            get => OutputNames[_output.Value];
            set => _output.Value = OutputNames.IndexOf(value);
        }

        public CommonSettings Common;

        public const int SourceCount = 4;

        public Source[] Sources;
        public Amplifier[] Amplifiers;

        public Filter Filter1;
        public Filter Filter2;

        public SinglePatch()
        {
            this._volume = new LevelType(99);
            this._effect = new EffectNumberType(1);
            this._output = new OutputSettingType(0);

            this._name = "NewSound";

            Common = new CommonSettings();

            Sources = new Source[SourceCount];
            Amplifiers = new Amplifier[SourceCount];
            for (int i = 0; i < SourceCount; i++)
            {
                Sources[i] = new Source();
                Amplifiers[i] = new Amplifier();
            }

            Filter1 = new Filter();
            Filter2 = new Filter();
            Checksum = 0;
        }

        public SinglePatch(byte[] data) : this()
        {
            // The no-arg constructor has initialized the other members.

            int offset = 0;
            this._name = GetName(data, offset); // s00...s09
            offset += 10;

            byte b = 0;  // will be reused when getting the next byte

            (b, offset) = Util.GetNextByte(data, offset);
            _volume = new LevelType(b);

            // effect = s11 bits 0...4
            (b, offset) = Util.GetNextByte(data, offset);
            _effect = new EffectNumberType((byte)((b & 0x1f) + 1)); // 0b00011111
            // use range 1~32 when storing the value, 0~31 in SysEx data

            // output select = s12 bits 0...2
            (b, offset) = Util.GetNextByte(data, offset);
            int outputNameIndex = (int)(b & 0x07); // 0b00000111;
            Output = OutputNames[outputNameIndex];

            byte[] commonData = new byte[CommonSettings.DataSize];
            Array.Copy(data, offset, commonData, 0, CommonSettings.DataSize);
            Common = new CommonSettings(commonData);
            offset += CommonSettings.DataSize;

            int totalSourceDataSize = Source.DataSize * SourceCount;
            byte[] sourceData = new byte[totalSourceDataSize];
            Array.Copy(data, offset, sourceData, 0, totalSourceDataSize);
            List<byte> allSourceData = new List<byte>(sourceData);
            List<byte> source1Data = Util.EveryNthElement(allSourceData, 4, 0);
            List<byte> source2Data = Util.EveryNthElement(allSourceData, 4, 1);
            List<byte> source3Data = Util.EveryNthElement(allSourceData, 4, 2);
            List<byte> source4Data = Util.EveryNthElement(allSourceData, 4, 3);

            Sources = new Source[SourceCount];
            Sources[0] = new Source(source1Data.ToArray());
            Sources[1] = new Source(source2Data.ToArray());
            Sources[2] = new Source(source3Data.ToArray());
            Sources[3] = new Source(source4Data.ToArray());

            offset += totalSourceDataSize;

            int totalAmpDataSize = Amplifier.DataSize * SourceCount;
            byte[] ampData = new byte[totalAmpDataSize];
            Array.Copy(data, offset, ampData, 0, totalAmpDataSize);
            List<byte> allAmpData = new List<byte>(ampData);
            List<byte> amp1Data = Util.EveryNthElement(allAmpData, 4, 0);
            List<byte> amp2Data = Util.EveryNthElement(allAmpData, 4, 1);
            List<byte> amp3Data = Util.EveryNthElement(allAmpData, 4, 2);
            List<byte> amp4Data = Util.EveryNthElement(allAmpData, 4, 3);

            Amplifiers = new Amplifier[SourceCount];
            Amplifiers[0] = new Amplifier(amp1Data.ToArray());
            Amplifiers[1] = new Amplifier(amp2Data.ToArray());
            Amplifiers[2] = new Amplifier(amp3Data.ToArray());
            Amplifiers[3] = new Amplifier(amp4Data.ToArray());

            offset += totalAmpDataSize;

            // DCF
            int totalFilterDataSize = Filter.DataSize * 2;
            byte[] filterData = new byte[totalFilterDataSize];
            Array.Copy(data, offset, filterData, 0, totalFilterDataSize);
            List<byte> allFilterData = new List<byte>(filterData);
            List<byte> filter1Data = Util.EveryNthElement(allFilterData, 2, 0);
            List<byte> filter2Data = Util.EveryNthElement(allFilterData, 2, 1);
            Filter1 = new Filter(filter1Data.ToArray());
            Filter2 = new Filter(filter2Data.ToArray());

            offset += totalFilterDataSize;

            (b, offset) = Util.GetNextByte(data, offset);
            // "Check sum value (s130) is the sum of the A5H and s0 ~ s129".
            this.Checksum = b; // store the checksum as we got it from SysEx
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append($"{Name}\n");
            builder.Append($"VOLUME     ={Volume:3}\nEFFECT PACH= {Effect:2}\nSUBMIX CH  =  {Output}\n");
            builder.Append($"{Common}\n");

            StringBuilder sourceString = new StringBuilder();
            StringBuilder ampString = new StringBuilder();
            for (int i = 0; i < SourceCount; i++)
            {
                sourceString.Append($"Source {i+1}:\n{Sources[i]}");
                ampString.Append($"DCA: {Amplifiers[i]}");
            }
            builder.Append(sourceString.ToString());
            builder.Append(ampString.ToString());

            builder.Append($"Filter 1: {Filter1}\n");
            builder.Append($"Filter 2: {Filter2}\n");
            return builder.ToString();
        }

        protected override byte[] CollectData()
        {
            List<byte> data = new List<byte>();

            byte[] nameBytes = Encoding.ASCII.GetBytes(this.Name.PadRight(10));
            data.AddRange(nameBytes);

            data.Add((byte)Volume);
            data.Add((byte)(Effect - 1));  // convert from 1~32 to 0~31 for SysEx data
            data.Add((byte)(OutputNames.IndexOf(Output)));  // convert 'A', 'B' ... 'H' to 0~7

            data.AddRange(Common.ToData());

            // The source data are interleaved, with one byte from each first,
            // then the second, etc. That's why they are emitted in this slightly
            // inelegant way. The same applies for DCA and DCF data.

            byte[] source1Data = Sources[0].ToData();
            byte[] source2Data = Sources[1].ToData();
            byte[] source3Data = Sources[2].ToData();
            byte[] source4Data = Sources[3].ToData();

            for (int i = 0; i < Source.DataSize; i++)
            {
                data.Add(source1Data[i]);
                data.Add(source2Data[i]);
                data.Add(source3Data[i]);
                data.Add(source4Data[i]);
            }

            byte[] amp1Data = Amplifiers[0].ToData();
            byte[] amp2Data = Amplifiers[1].ToData();
            byte[] amp3Data = Amplifiers[2].ToData();
            byte[] amp4Data = Amplifiers[3].ToData();

            for (int i = 0; i < Amplifier.DataSize; i++)
            {
                data.Add(amp1Data[i]);
                data.Add(amp2Data[i]);
                data.Add(amp3Data[i]);
                data.Add(amp4Data[i]);
            }

            byte[] filter1Data = Filter1.ToData();
            byte[] filter2Data = Filter2.ToData();
            for (int i = 0; i < Filter.DataSize; i++)
            {
                data.Add(filter1Data[i]);
                data.Add(filter2Data[i]);
            }

            return data.ToArray();
        }
    }
}