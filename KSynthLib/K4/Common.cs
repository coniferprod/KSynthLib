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
    /// Single patch common settings.
    /// </summary>
    /// <remarks>
    /// s13 ... s29 in the System Exclusive specification
    /// </remarks>
    public class CommonSettings
    {
        /// <value>The number of bytes in the single common settings.</value>
        public const int DataSize = 17;

        public SourceMode SourceMode;

        public PolyphonyMode PolyphonyMode;

        public bool AMS1ToS2;
        public bool AMS3ToS4;

        public bool S1Mute;
        public bool S2Mute;
        public bool S3Mute;
        public bool S4Mute;

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

        /// <summary>
        /// Constructs the single common settings from default values.
        /// </summary>
        public CommonSettings()
        {
            SourceMode = SourceMode.Normal;
            PolyphonyMode = PolyphonyMode.Poly1;
            AMS1ToS2 = false;
            AMS3ToS4 = false;
            _pitchBend = new PitchBendType(2);
            WheelAssign = WheelAssignType.Vibrato;
            _wheelDepth = new DepthType(0);
            Vibrato = new VibratoSettings();
            LFO = new LFOSettings();
            AutoBend = new AutoBendSettings();
            _pressureFreq = new DepthType();
        }

        /// <summary>
        /// Constructs the single common settings from binary System Exclusive data.
        /// </summary>
        /// <param name="data">System Exclusive data</param>
        /// <remarks>
        /// The no-argument constructor is used to initialize the data members.
        /// </remarks>
        public CommonSettings(byte[] data) : this()
        {
            int offset = 0; // name is s00 ... s09, others s10 ... s12
            byte b = 0;  // will be reused when getting the next byte

            // source mode = s13 bits 0...1
            (b, offset) = Util.GetNextByte(data, offset);
            SourceMode = (SourceMode)(b & 0x03);
            PolyphonyMode = (PolyphonyMode)((b >> 2) & 0x03);
            AMS1ToS2 = ((b >> 4) & 0x01) == 1;
            AMS3ToS4 = ((b >> 5) & 0x01) == 1;

            (b, offset) = Util.GetNextByte(data, offset);
            S1Mute = (b & 0x01) == 0;  // 0/mute, 1/not mute
            S2Mute = ((b >> 1) & 0x01) == 0;  // 0/mute, 1/not mute
            S3Mute = ((b >> 2) & 0x01) == 0;  // 0/mute, 1/not mute
            S4Mute = ((b >> 3) & 0x01) == 0;  // 0/mute, 1/not mute

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
        }

        /// <summary>
        /// Generates a printable representation of the settings.
        /// </summary>
        /// <returns>
        /// String with parameter values.
        /// </returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(string.Format("SOURCE MODE={0}\n", Enum.GetNames(typeof(SourceMode))[(int)SourceMode]));
            builder.Append(string.Format("AM 1>2     ={0}\nAM 3>4     ={1}\n", AMS1ToS2 ? "ON" : "OFF", AMS3ToS4 ? "ON" : "OFF"));
            builder.Append(string.Format("POLY MODE  ={0}\n", Enum.GetNames(typeof(PolyphonyMode))[(int)PolyphonyMode]));
            builder.Append(string.Format("BNDR RANGE = {0,2}\n", PitchBend));
            builder.Append(string.Format("PRESS>FREQ = {0,2}\n", PressureFreq));
            builder.Append(string.Format("WHEEL\nASSIGN     ={0}\nDEPTH      ={1,2}\n", Enum.GetNames(typeof(WheelAssignType))[(int)WheelAssign], WheelDepth));
            builder.Append($"AUTO BEND\n{AutoBend}\n");
            builder.Append(string.Format("Sources: {0}\n", GetSourceMuteString(S1Mute, S2Mute, S3Mute, S4Mute)));
            builder.Append($"VIBRATO\n{Vibrato}\n");
            builder.Append($"LFO\n{LFO}\n");
            return builder.ToString();
        }

        /// <summary>
        /// Generates a binary System Exclusive representation of the data.
        /// </summary>
        /// <returns>
        /// A byte array with SysEx data.
        /// </returns>
        public byte[] ToData()
        {
            List<byte> data = new List<byte>();

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
            b14.Append(S4Mute ? "1" : "0");
            b14.Append(S3Mute ? "1" : "0");
            b14.Append(S2Mute ? "1" : "0");
            b14.Append(S1Mute ? "1" : "0");
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

            var arr = data.ToArray();
            Debug.WriteLine($"Common settings = {arr.Length} bytes");
            return arr;
        }

        private string GetSourceMuteString(bool s1, bool s2, bool s3, bool s4)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(s1 ? "1" : "-");
            builder.Append(s2 ? "2" : "-");
            builder.Append(s3 ? "3" : "-");
            builder.Append(s4 ? "4" : "-");
            return builder.ToString();
        }
    }
}
