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

    public class Zone
    {
        public int Low;
        public int High;

    }

    public class Section
    {
        public const int DataSize = 8;

        public PatchNumberType SinglePatch;
        public Zone KeyboardZone;
        public ChannelType ReceiveChannel;
        public VelocitySwitchType VelocitySwitch;
        public bool IsMuted;
        public SubmixType Output;
        public PlayModeType PlayMode;
        public LevelType Level;
        public CoarseType Transpose;
        public DepthType Tune;

        public Section()
        {
            SinglePatch = new PatchNumberType(1);
            KeyboardZone = new Zone { Low = 0, High = 127 };
            ReceiveChannel = new ChannelType(1);
            VelocitySwitch = VelocitySwitchType.All;
            IsMuted = false;
            Output = SubmixType.A;
            PlayMode = PlayModeType.Keyboard;
            Level = new LevelType(80);
            Transpose = new CoarseType(0);
            Tune = new DepthType(0);
        }

        public Section(byte[] data) : this()
        {
            int offset = 0;
            byte b = 0;  // will be reused when getting the next byte

            (b, offset) = Util.GetNextByte(data, offset);
            SinglePatch = new PatchNumberType(b);

            (b, offset) = Util.GetNextByte(data, offset);
            int zoneLow = b;
            (b, offset) = Util.GetNextByte(data, offset);
            int zoneHigh = b;
            KeyboardZone = new Zone { Low = zoneLow, High = zoneHigh };

            (b, offset) = Util.GetNextByte(data, offset);
            // rcv ch = M15 bits 0...3
            ReceiveChannel = new ChannelType((byte)(b & 0x0f));
            // velo sw = M15 bits 4..5
            VelocitySwitch = (VelocitySwitchType)((b >> 4) & 0x03);
            // section mute = M15 bit 6
            IsMuted = b.IsBitSet(6);

            (b, offset) = Util.GetNextByte(data, offset);
            // out select = M16 bits 0...2
            Output = (SubmixType)(b & 0x07); // 0b00000111

            // play mode = M16 bits 3...4
            PlayMode = (PlayModeType)((b >> 3) & 0x03);

            (b, offset) = Util.GetNextByte(data, offset);
            Level = new LevelType(b);

            (b, offset) = Util.GetNextByte(data, offset);
            Transpose = new CoarseType(b);

            (b, offset) = Util.GetNextByte(data, offset);
            Tune = new DepthType(b);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(string.Format("single = {0}, recv ch = {1}, play mode = {2}\n", PatchUtil.GetPatchName(SinglePatch.Value), ReceiveChannel.Value, PlayMode));
            builder.Append(string.Format("zone = {0} to {1}, vel sw = {2}\n", PatchUtil.GetNoteName(KeyboardZone.Low), PatchUtil.GetNoteName(KeyboardZone.High), VelocitySwitch));
            builder.Append($"level = {Level}, transpose = {Transpose}, tune = {Tune}\n");
            builder.Append($"submix ch = {Output}\n");
            return builder.ToString();
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();

            data.Add(SinglePatch.ToByte());
            data.Add((byte)KeyboardZone.Low);
            data.Add((byte)KeyboardZone.High);

            // Combine rcv ch, velo sw and section mute into one byte for M15/M23 etc.
            byte vb = (byte)VelocitySwitch;
            byte rb = ReceiveChannel.ToByte();
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

            data.Add(Level.ToByte());
            data.Add(Transpose.ToByte());
            data.Add(Tune.ToByte());

            return data.ToArray();
        }
    }
}
