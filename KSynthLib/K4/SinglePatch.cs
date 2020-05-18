using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;

using KSynthLib.Common;

namespace KSynthLib.K4
{
    public enum SourceMode
    {
        Normal,
        Twin,
        Double
    };

    public enum PolyphonyMode
    {
        Poly1,
        Poly2,
        Solo1,
        Solo2
    };


    /// <summary>
    /// Represents a K4 single patch.
    /// </summary>
    public class SinglePatch : Patch
    {
        /// <value>System Exclusive data length.</value>
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

        //public CommonSettings Common;

        public SourceMode SourceMode;

        public PolyphonyMode PolyphonyMode;

        public bool AMS1ToS2;
        public bool AMS3ToS4;

        public bool[] SourceMutes;

        private PitchBendType _pitchBend;
        public int PitchBend  // 0~12
        {
            get => _pitchBend.Value;
            set => _pitchBend.Value = value;
        }

        public WheelAssignType WheelAssign; // 0/VIB, 1/LFO, 2/DCF

        private DepthType _wheelDepth;
        public sbyte WheelDepth // 0~100 (±50)
        {
            get => _wheelDepth.Value;
            set => _wheelDepth.Value = value;
        }

        public AutoBendSettings AutoBend;  // same as portamento?

        public LFOSettings LFO;

        public VibratoSettings Vibrato;

        private DepthType _pressureFreq;
        public sbyte PressureFreq // 0~100 (±50)
        {
            get => _pressureFreq.Value;
            set => _pressureFreq.Value = value;
        }

        /// <value>The number of sources in a single patch.</value>
        public const int SourceCount = 4;

        public Source[] Sources;
        public Amplifier[] Amplifiers;

        public Filter Filter1;
        public Filter Filter2;

        /// <summary>
        /// Constructs a single patch from default values.
        /// </summary>
        public SinglePatch()
        {
            this._volume = new LevelType(99);
            this._effect = new EffectNumberType(1);
            this._output = new OutputSettingType(0);

            this._name = "NewSound";

            //Common = new CommonSettings();
            SourceMode = SourceMode.Normal;
            PolyphonyMode = PolyphonyMode.Poly1;
            AMS1ToS2 = false;
            AMS3ToS4 = false;
            SourceMutes = new bool[] { false, false, false, false };
            _pitchBend = new PitchBendType(2);
            WheelAssign = WheelAssignType.Vibrato;
            _wheelDepth = new DepthType(0);
            Vibrato = new VibratoSettings();
            LFO = new LFOSettings();
            AutoBend = new AutoBendSettings();
            _pressureFreq = new DepthType();

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

        /// <summary>
        /// Constructs a single patch from binary System Exclusive data.
        /// </summary>
        /// <param name="data">System Exclusive data</param>
        /// <remarks>
        /// The no-argument constructor is used to initialize the data members.
        /// </remarks>
        public SinglePatch(byte[] data) : this()
        {
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

/*
            byte[] commonData = new byte[CommonSettings.DataSize];
            Array.Copy(data, offset, commonData, 0, CommonSettings.DataSize);
            Common = new CommonSettings(commonData);
            offset += CommonSettings.DataSize;
*/

            // source mode = s13 bits 0...1
            (b, offset) = Util.GetNextByte(data, offset);
            SourceMode = (SourceMode)(b & 0x03);
            PolyphonyMode = (PolyphonyMode)((b >> 2) & 0x03);
            AMS1ToS2 = ((b >> 4) & 0x01) == 1;
            AMS3ToS4 = ((b >> 5) & 0x01) == 1;

            (b, offset) = Util.GetNextByte(data, offset);
            SourceMutes[0] = (b & 0x01) == 0; // 0/mute, 1/not mute
            SourceMutes[1] = ((b >> 1) & 0x01) == 0;
            SourceMutes[2] = ((b >> 2) & 0x01) == 0;
            SourceMutes[3] = ((b >> 3) & 0x01) == 0;

            Vibrato = new VibratoSettings();
            Vibrato.Shape = (LFOShape)((b >> 4) & 0x03);

            (b, offset) = Util.GetNextByte(data, offset);
            // Pitch bend = s15 bits 0...3
            _pitchBend = new PitchBendType(b & 0x0f);
            // Wheel assign = s15 bits 4...5
            WheelAssign = (WheelAssignType)((b >> 4) & 0x03);

            (b, offset) = Util.GetNextByte(data, offset);
            // Vibrato speed = s16 bits 0...6
            Vibrato.Speed = (byte)(b & 0x7f);

            // Wheel depth = s17 bits 0...6
            (b, offset) = Util.GetNextByte(data, offset);
            _wheelDepth = new DepthType((sbyte)((b & 0x7f) - 50));  // 0~100 to ±50

            AutoBend = new AutoBendSettings();
            (b, offset) = Util.GetNextByte(data, offset);
            AutoBend.Time = (byte)(b & 0x7f);

            (b, offset) = Util.GetNextByte(data, offset);
            AutoBend.Depth = (sbyte)((b & 0x7f) - 50); // 0~100 to ±50

            (b, offset) = Util.GetNextByte(data, offset);
            AutoBend.KeyScalingTime = (sbyte)((b & 0x7f) - 50); // 0~100 to ±50

            (b, offset) = Util.GetNextByte(data, offset);
            AutoBend.VelocityDepth = (sbyte)((b & 0x7f) - 50); // 0~100 to ±50

            (b, offset) = Util.GetNextByte(data, offset);
            Vibrato.Pressure = (sbyte)((b & 0x7f) - 50); // 0~100 to ±50

            (b, offset) = Util.GetNextByte(data, offset);
            Vibrato.Depth = (sbyte)((b & 0x7f) - 50); // 0~100 to ±50

            LFO = new LFOSettings();

            (b, offset) = Util.GetNextByte(data, offset);
            LFO.Shape = (LFOShape)(b & 0x03);

            (b, offset) = Util.GetNextByte(data, offset);
            LFO.Speed = (byte)(b & 0x7f);

            (b, offset) = Util.GetNextByte(data, offset);
            LFO.Delay = (byte)(b & 0x7f);

            (b, offset) = Util.GetNextByte(data, offset);
            LFO.Depth = (sbyte)((b & 0x7f) - 50); // 0~100 to ±50

            (b, offset) = Util.GetNextByte(data, offset);
            LFO.PressureDepth = (sbyte)((b & 0x7f) - 50); // 0~100 to ±50

            (b, offset) = Util.GetNextByte(data, offset);
            _pressureFreq = new DepthType((sbyte)((b & 0x7f) - 50)); // 0~100 to ±50


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

        /// <summary>
        /// Generates a printable representation of this patch.
        /// </summary>
        /// <returns>
        /// String with patch parameter values.
        /// </returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append($"{Name}\n");
            builder.Append($"VOLUME     ={Volume:3}\nEFFECT PACH= {Effect:2}\nSUBMIX CH  =  {Output}\n");

            builder.Append(string.Format("SOURCE MODE={0}\n", Enum.GetNames(typeof(SourceMode))[(int)SourceMode]));
            builder.Append(string.Format("AM 1>2     ={0}\nAM 3>4     ={1}\n", AMS1ToS2 ? "ON" : "OFF", AMS3ToS4 ? "ON" : "OFF"));
            builder.Append(string.Format("POLY MODE  ={0}\n", Enum.GetNames(typeof(PolyphonyMode))[(int)PolyphonyMode]));
            builder.Append(string.Format("BNDR RANGE = {0,2}\n", PitchBend));
            builder.Append(string.Format("PRESS>FREQ = {0,2}\n", PressureFreq));
            builder.Append(string.Format("WHEEL\nASSIGN     ={0}\nDEPTH      ={1,2}\n", Enum.GetNames(typeof(WheelAssignType))[(int)WheelAssign], WheelDepth));
            builder.Append($"AUTO BEND\n{AutoBend}\n");
            builder.Append($"Sources: {SourceMuteString}\n");
            builder.Append($"VIBRATO\n{Vibrato}\n");
            builder.Append($"LFO\n{LFO}\n");

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

        /// <summary>
        /// Collects the patch data for use in a System Exclusive message.
        /// </summary>
        /// <returns>
        /// Byte array with patch data.
        /// </returns>
        /// <remarks>
        /// The data does not include the checksum.
        /// </remarks>
        protected override byte[] CollectData()
        {
            List<byte> data = new List<byte>();

            byte[] nameBytes = Encoding.ASCII.GetBytes(this.Name.PadRight(10));
            data.AddRange(nameBytes);

            data.Add((byte)Volume);
            data.Add((byte)(Effect - 1));  // convert from 1~32 to 0~31 for SysEx data
            data.Add((byte)(OutputNames.IndexOf(Output)));  // convert 'A', 'B' ... 'H' to 0~7

            // s13 combines source mode, poly mode, and source AM into one byte.
            // Construct a bit string, then convert it to byte.
            StringBuilder b13 = new StringBuilder("00");
            b13.Append(AMS3ToS4 ? "1" : "0");
            b13.Append(AMS1ToS2 ? "1" : "0");
            b13.Append(Convert.ToString((byte)PolyphonyMode, 2).PadLeft(2, '0'));
            b13.Append(Convert.ToString((byte)SourceMode, 2).PadLeft(2, '0'));
            data.Add(Convert.ToByte(b13.ToString(), 2));

            // s14 combines vibrato shape and source mutes into one byte.
            StringBuilder b14 = new StringBuilder("00");
            b14.Append(Convert.ToString((byte)Vibrato.Shape, 2).PadLeft(2, '0'));
            b14.Append(SourceMutes[3] ? "1" : "0");
            b14.Append(SourceMutes[2] ? "1" : "0");
            b14.Append(SourceMutes[1] ? "1" : "0");
            b14.Append(SourceMutes[0] ? "1" : "0");
            data.Add(Convert.ToByte(b14.ToString(), 2));

            // s15 combines pitch bend and wheel assign into one byte.
            StringBuilder b15 = new StringBuilder("");
            b15.Append(Convert.ToString((byte)WheelAssign, 2).PadLeft(4, '0'));
            b15.Append(Convert.ToString((byte)PitchBend, 2).PadLeft(4, '0'));
            data.Add(Convert.ToByte(b15.ToString(), 2));

            data.Add((byte)Vibrato.Speed);
            data.Add((byte)(WheelDepth + 50));  // ±50 to 0...100
            data.Add((byte)AutoBend.Time);
            data.Add((byte)(AutoBend.Depth + 50)); // ±50 to 0...100
            data.Add((byte)(AutoBend.KeyScalingTime + 50)); // ±50 to 0...100
            data.Add((byte)(AutoBend.VelocityDepth + 50)); // ±50 to 0...100
            data.Add((byte)(Vibrato.Pressure + 50)); // ±50 to 0...100
            data.Add((byte)(Vibrato.Depth + 50)); // ±50 to 0...100
            data.Add((byte)LFO.Shape);
            data.Add((byte)LFO.Speed);
            data.Add((byte)LFO.Delay);
            data.Add((byte)(LFO.Depth + 50)); // ±50 to 0...100
            data.Add((byte)(LFO.PressureDepth + 50)); // ±50 to 0...100
            data.Add((byte)(PressureFreq + 50)); // ±50 to 0...100

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

        private string SourceMuteString
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(SourceMutes[0] ? "1" : "-");
                builder.Append(SourceMutes[1] ? "2" : "-");
                builder.Append(SourceMutes[2] ? "3" : "-");
                builder.Append(SourceMutes[3] ? "4" : "-");
                return builder.ToString();
            }
        }
    }
}