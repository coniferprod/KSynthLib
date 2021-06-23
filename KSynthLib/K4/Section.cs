using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K4
{
    public enum VelocitySwitchType
    {
        All,
        Soft,
        Loud
    }

    public enum PlayModeType
    {
        Keyboard,
        Midi,
        Mix
    }

    public class Section
    {
        public const int DataSize = 8;

        private PatchNumberType _singlePatch;
        private byte SinglePatch
        {
            get => _singlePatch.Value;
            set => _singlePatch.Value = value;
        }

        private ZoneValueType _zoneLow;
        public byte ZoneLow
        {
            get => _zoneLow.Value;
            set => _zoneLow.Value = value;
        }

        private ZoneValueType _zoneHigh;
        private byte ZoneHigh
        {
            get => _zoneHigh.Value;
            set => _zoneHigh.Value = value;
        }

        private MidiChannelType _receiveChannel;
        private byte ReceiveChannel
        {
            get => _receiveChannel.Value;
            set => _receiveChannel.Value = value;
        }

        public VelocitySwitchType VelocitySwitch;

        private bool IsMuted;

        private OutputSettingType _output;
        public char Output
        {
            get => OutputSettingType.OutputNames[_output.Value];
            set => _output.Value = OutputSettingType.OutputNames.IndexOf(value);
        }

        public PlayModeType PlayMode;

        private LevelType _level;
        public byte Level
        {
            get => _level.Value;
            set => _level.Value = value;
        }

        private CoarseType _transpose;
        private sbyte Transpose
        {
            get => _transpose.Value;
            set => _transpose.Value = value;
        }

        private DepthType _tune;
        private sbyte Tune
        {
            get => _tune.Value;
            set => _tune.Value = value;
        }

        public Section()
        {
            _singlePatch = new PatchNumberType();
            _zoneLow = new ZoneValueType();
            _zoneHigh = new ZoneValueType(127);
            _receiveChannel = new MidiChannelType(1);
            VelocitySwitch = VelocitySwitchType.All;
            IsMuted = false;
            _output = new OutputSettingType();
            PlayMode = PlayModeType.Keyboard;
            _level = new LevelType(80);
            _transpose = new CoarseType(0);
            _tune = new DepthType(0);
        }

        public Section(byte[] data) : this()
        {
            int offset = 0;
            byte b = 0;  // will be reused when getting the next byte

            (b, offset) = Util.GetNextByte(data, offset);
            SinglePatch = b;

            (b, offset) = Util.GetNextByte(data, offset);
            ZoneLow = b;

            (b, offset) = Util.GetNextByte(data, offset);
            ZoneHigh = b;

            (b, offset) = Util.GetNextByte(data, offset);
            // rcv ch = M15 bits 0...3
            ReceiveChannel = (byte)((b & 0x0f) + 1);  // 0~15 to 1~16
            // velo sw = M15 bits 4..5
            VelocitySwitch = (VelocitySwitchType)((b >> 4) & 0x03);
            // section mute = M15 bit 6
            IsMuted = b.IsBitSet(6);

            (b, offset) = Util.GetNextByte(data, offset);
            // out select = M16 bits 0...2
            int outputNameIndex = (int)(b & 0x07); // 0b00000111;
            Output = OutputSettingType.OutputNames[outputNameIndex];

            // play mode = M16 bits 3...4
            PlayMode = (PlayModeType)((b >> 3) & 0x03);

            (b, offset) = Util.GetNextByte(data, offset);
            Level = b;

            (b, offset) = Util.GetNextByte(data, offset);
            Transpose = (sbyte)(b - 24);

            (b, offset) = Util.GetNextByte(data, offset);
            Tune = (sbyte)(b - 50);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(string.Format("single = {0}, recv ch = {1}, play mode = {2}\n", PatchUtil.GetPatchName(SinglePatch), ReceiveChannel, PlayMode));
            builder.Append(string.Format("zone = {0} to {1}, vel sw = {2}\n", GetNoteName(ZoneLow), GetNoteName(ZoneHigh), VelocitySwitch));
            builder.Append($"level = {Level}, transpose = {Transpose}, tune = {Tune}\n");
            builder.Append($"submix ch = {Output}\n");
            return builder.ToString();
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();

            data.Add((byte)SinglePatch);
            data.Add((byte)ZoneLow);
            data.Add((byte)ZoneHigh);

            // Combine rcv ch, velo sw and section mute into one byte for M15/M23 etc.
            byte vb = (byte)VelocitySwitch;
            byte rb = (byte)(ReceiveChannel - 1);
            byte vbp = (byte)(vb << 4);
            byte m15 = (byte)(rb | vbp);
            if (IsMuted)
            {
                m15.SetBit(6);
            }
            data.Add(m15);

            // Combine "out select" and "mode" into one byte for M16/M24 etc.
            byte os = (byte)Output;
            byte m = (byte)PlayMode;
            byte m16 = (byte)(os | m);
            data.Add(m16);

            data.Add((byte)Level);
            data.Add((byte)(Transpose + 24));
            data.Add((byte)(Tune + 50));

            return data.ToArray();
        }

        // 0 ~ 127 / C-2 ~ G8
        private string GetNoteName(int noteNumber) {
            string[] notes = new string[] { "C", "C#", "D", "Eb", "E", "F", "F#", "G", "G#", "A", "Bb", "B" };
            int octave = noteNumber / 12 + 1;
            string name = notes[noteNumber % 12];
            return name + octave;
        }
    }
}
