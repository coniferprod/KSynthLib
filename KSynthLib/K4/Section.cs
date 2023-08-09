using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

    public class Section : ISystemExclusiveData
    {
        public const int DataSize = 8;

        public PatchNumber SinglePatch;
        public Zone KeyboardZone;
        public Channel ReceiveChannel;
        public VelocitySwitchType VelocitySwitch;
        public bool IsMuted;
        public SubmixType Output;
        public PlayModeType PlayMode;
        public Level Level;
        public Transpose Transpose;
        public Depth Tune;

        public Section()
        {
            SinglePatch = new PatchNumber(1);
            KeyboardZone = new Zone { Low = 0, High = 127 };
            ReceiveChannel = new Channel(1);
            VelocitySwitch = VelocitySwitchType.All;
            IsMuted = false;
            Output = SubmixType.A;
            PlayMode = PlayModeType.Keyboard;
            Level = new Level(80);
            Transpose = new Transpose();
            Tune = new Depth(0);
        }

        public Section(byte[] data) : this()
        {
            byte b;  // will be reused when getting the next byte
            int offset = 0;

            (b, offset) = Util.GetNextByte(data, offset);
            SinglePatch = new PatchNumber(b);

            (b, offset) = Util.GetNextByte(data, offset);
            int zoneLow = b;
            (b, offset) = Util.GetNextByte(data, offset);
            int zoneHigh = b;
            KeyboardZone = new Zone { Low = zoneLow, High = zoneHigh };

            (b, offset) = Util.GetNextByte(data, offset);
            // rcv ch = M15 bits 0...3
            ReceiveChannel = new Channel((byte)(b & 0x0f));
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
            Level = new Level(b);

            (b, offset) = Util.GetNextByte(data, offset);
            Transpose = new Transpose(b);

            (b, offset) = Util.GetNextByte(data, offset);
            Tune = new Depth(b);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine(
                string.Format(
                    "single = {0}, recv ch = {1}, play mode = {2}",
                    PatchUtil.GetPatchName(SinglePatch.Value),
                    ReceiveChannel,
                    PlayMode
                )
            );

            builder.AppendLine(
                string.Format(
                    "zone = {0} to {1}, vel sw = {2}",
                    PatchUtil.GetNoteName(KeyboardZone.Low),
                    PatchUtil.GetNoteName(KeyboardZone.High),
                    VelocitySwitch
                )
            );
            builder.AppendLine($"level = {Level}, transpose = {Transpose}, tune = {Tune}");
            builder.AppendLine($"submix ch = {Output}");

            return builder.ToString();
        }

        //
        // ISystemExclusiveData implementation
        //

        public List<byte> Data
        {
            get
            {
                var data = new List<byte>();

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

                return data;
            }
        }

        public int DataLength => 8;
    }
}
