using System;
using System.Collections.Generic;
using System.Text;

using KSynthLib.Common;

namespace KSynthLib.K1
{
    // "Determines whether the tone patch uses all four SOURCEs or only two."
    public enum SourceMode
    {
        TwoSource,
        FourSource
    }

    public enum PolyphonyMode
    {
        Poly1,
        Poly2,
        Solo
    }

    public enum AmplitudeModulationMode
    {
        Off,
        Modulated,
        Reversed
    }

    public enum LFOWaveform
    {
        Triangle,
        Sawtooth,
        Square,
        Random
    }

    public struct LFOSettings
    {
        public int Speed;
    }

    public enum WheelAssign
    {
        Depth,
        Speed
    }

    public class SinglePatch
    {
        public const int DataSize = 88;
        public const int NumSources = 4;

        public string Name;
        public byte Volume;
        public PolyphonyMode PMode;
        public SourceMode SMode;
        public AmplitudeModulationMode AM12;
        public AmplitudeModulationMode AM34;
        public int PressureFrequencyDepth;  // 0 ~ 100 (-50 ~ +50)
        public int VibratoDepth; // 0 ~ 100 (-50 ~ +50)
        public int VibratoPressureDepth; // 0 ~ 100 (-50 ~ +50)
        public int PitchBend; // 0 ~ 12
        public int LFOSpeed;
        public LFOWaveform LFOShape;
        public int KeyScalingCurve;
        public WheelAssign VibratoWheelAssign;
        public int AutoBendDepth; // 0 ~ 100 (-50 ~ +50)
        public int AutoBendTime; // 0 ~ 100
        public int AutoBendVelocityDepth; // 0 ~ 100 (-50 ~ +50)
        public int AutoBendKeyScalingTime; // 0 ~ 100 (-50 ~ +50)

        public bool[] SourceMuted;
        public Source[] Sources;

        public byte Checksum;

        public SinglePatch(byte[] data)
        {
            int offset = 0;
            byte b = 0;  // will be reused when getting the next byte

            Name = GetName(data, offset);
            offset += Name.Length;

            (b, offset) = Util.GetNextByte(data, offset);
            Volume = b;

            (b, offset) = Util.GetNextByte(data, offset);
            PMode = (PolyphonyMode)(b & 0x03);

            SMode = SourceMode.TwoSource;
            if (b.IsBitSet(2))
            {
                SMode = SourceMode.FourSource;
            }
            AM12 = (AmplitudeModulationMode)((b >> 3) & 0x03);
            AM34 = (AmplitudeModulationMode)((b >> 5) & 0x03);

            string s11 = Convert.ToString(b, 2);
            Console.Error.WriteLine(string.Format("s11 = {0}", s11));

            (b, offset) = Util.GetNextByte(data, offset);
            PressureFrequencyDepth = b;

            (b, offset) = Util.GetNextByte(data, offset);
            VibratoDepth = b;

            (b, offset) = Util.GetNextByte(data, offset);
            VibratoPressureDepth = b;

            (b, offset) = Util.GetNextByte(data, offset);
            PitchBend = b;

            (b, offset) = Util.GetNextByte(data, offset);
            LFOSpeed = b;

            (b, offset) = Util.GetNextByte(data, offset);
            LFOShape = (LFOWaveform)(b & 0x03);
            KeyScalingCurve = ((b >> 2) & 0x07);
            VibratoWheelAssign = (WheelAssign)((b >> 5) & 0x07);

            (b, offset) = Util.GetNextByte(data, offset);
            AutoBendDepth = b;

            (b, offset) = Util.GetNextByte(data, offset);
            AutoBendTime = b;

            (b, offset) = Util.GetNextByte(data, offset);
            AutoBendVelocityDepth = b;

            (b, offset) = Util.GetNextByte(data, offset);
            AutoBendKeyScalingTime = b;

            SourceMuted = new bool[NumSources];
            (b, offset) = Util.GetNextByte(data, offset);
            string s22 = b.ToBinaryString(4);
            Console.Error.WriteLine(string.Format("Source mutes = {0}", s22));
            SourceMuted[0] = !b.IsBitSet(0);
            SourceMuted[1] = !b.IsBitSet(1);
            SourceMuted[2] = !b.IsBitSet(2);
            SourceMuted[3] = !b.IsBitSet(3);

            var source1Data = new byte[16];
            var source2Data = new byte[16];
            var source3Data = new byte[16];
            var source4Data = new byte[16];

            for (var i = 0; i < 16; i++)
            {
                (b, offset) = Util.GetNextByte(data, offset);
                source1Data[i] = b;

                (b, offset) = Util.GetNextByte(data, offset);
                source2Data[i] = b;

                (b, offset) = Util.GetNextByte(data, offset);
                source3Data[i] = b;

                (b, offset) = Util.GetNextByte(data, offset);
                source4Data[i] = b;
            }

            Sources = new Source[NumSources];
            Sources[0] = new Source(source1Data);
            Sources[1] = new Source(source2Data);
            Sources[2] = new Source(source3Data);
            Sources[3] = new Source(source4Data);

            (b, offset) = Util.GetNextByte(data, offset);
            Checksum = b;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(Name);
            builder.Append("\n");
            var modeString = "???";
            if (SMode == SourceMode.FourSource)
            {
                modeString = "FOUR";
            }
            else if (SMode == SourceMode.TwoSource)
            {
                modeString = "TWO";
            }
            builder.Append(string.Format("volume = {0}, sources = {1}\n", Volume + 1, modeString));
            builder.Append("SOURCES:\n");
            for (var i = 0; i < NumSources; i++)
            {
                builder.Append(string.Format("S{0}: {1}\n", i + 1, Sources[i].ToString()));
            }
            builder.Append(string.Format("checksum = {0,2:X2}H", Checksum));

            return builder.ToString();
        }

        private string GetName(byte[] data, int offset)
        {
            byte[] bytes =
            {
                data[offset],
                data[offset + 1],
                data[offset + 2],
                data[offset + 3],
                data[offset + 4],
                data[offset + 5],
                data[offset + 6],
                data[offset + 7],
                data[offset + 8],
                data[offset + 9]
            };
            string name = Encoding.ASCII.GetString(bytes);
            return name;
        }

        public byte[] ToData()
        {
            var buf = new List<byte>();

            byte[] nameBytes = Encoding.ASCII.GetBytes(Name);
            foreach (var b in nameBytes)
            {
                buf.Add(b);
            }

            buf.Add((byte)Volume);

            string s11 = ((byte)AM34).ToBinaryString(2) + ((byte)AM12).ToBinaryString(2) + ((byte)SMode).ToBinaryString(1) + ((byte)PMode).ToBinaryString(2);
            Console.Error.WriteLine(string.Format("s11 = {0}", s11));
            buf.Add(Convert.ToByte(s11, 2));

            buf.Add((byte)PressureFrequencyDepth);
            buf.Add((byte)VibratoDepth);
            buf.Add((byte)VibratoPressureDepth);
            buf.Add((byte)PitchBend);
            buf.Add((byte)LFOSpeed);

            string s17 = ((byte)VibratoWheelAssign).ToBinaryString(3) + ((byte)KeyScalingCurve).ToBinaryString(3) + ((byte)LFOShape).ToBinaryString(2);
            buf.Add(Convert.ToByte(s17, 2));

            buf.Add((byte)AutoBendDepth);
            buf.Add((byte)AutoBendTime);
            buf.Add((byte)AutoBendVelocityDepth);
            buf.Add((byte)AutoBendKeyScalingTime);

            byte mutesValue = 0x00;
            for (var i = 0; i < NumSources; i++)
            {
                if (SourceMuted[i])
                {
                    mutesValue.SetBit(i);
                }
            }
            // Hope the SysEx spec has this the right way around...
            buf.Add(mutesValue);

            // Collect the source data lists into one list, then interleave
            var allSourceData = new List<List<byte>>();
            foreach (var source in Sources)
            {
                allSourceData.Add(new List<byte>(source.ToData()));
            }
            buf.AddRange(allSourceData.Interleave());

            byte checksum = ComputeChecksum(buf.ToArray());
            buf.Add(checksum);
            return buf.ToArray();
        }

        // K1 checksum = "sum of the A5H and s0 ~ s86, and bit 7 must be clear"
        private byte ComputeChecksum(byte[] data)
        {
            Console.Error.WriteLine(string.Format("Computing checksum from a buffer of {0} bytes...", data.Length));
            int sum = 0xA5;
            foreach (var b in data)
            {
                sum = (sum + b) & 255;
            }
            return (byte)(sum & 127);
        }
    }
}