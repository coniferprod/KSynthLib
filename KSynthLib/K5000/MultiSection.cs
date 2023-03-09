using System;
using System.Text;
using System.Collections.Generic;
using System.IO;

using KSynthLib.Common;
using SyxPack;

namespace KSynthLib.K5000
{
    /// <summary>
    /// Represents common settings for a K5000S multi patch (combi on K5000W/R).
    /// </summary>
    public class MultiSection : ISystemExclusiveData
    {
        public static readonly int DataSize = 12;  // See "3.1.6.2 Section Data"

        // 0~127: General MIDI
        // 128~255: B
        // 256~383: A (K5000W)
        // 384~511: D (K5000S/R)
        // 512~639: E (when ME-1 installed)
        // 640~767: F (when ME-1 installed)
        public ushort InstrumentNumber;

        public PositiveLevel Volume;  // 0~127
        public PositiveLevel Pan;  // 0~127

        public EffectPath EffectPath;
        public Transpose Transpose;
        public SignedLevel Tune;
        public Zone Zone;
        public MultiVelocitySwitchSettings VelocitySwitch;
        public Channel ReceiveChannel;  // only K5000S/R, K5000W=0

        public MultiSection(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data, false))
	        {
                byte msb = (byte) ms.ReadByte();
                byte lsb = (byte) ms.ReadByte();
                var (msbString, lsbString) = (Convert.ToString(msb, 2).PadLeft(2, '0'), Convert.ToString(lsb, 2).PadLeft(7, '0'));
                var instrumentNumberString = msbString + lsbString;
                this.InstrumentNumber = (ushort)(Convert.ToUInt16(instrumentNumberString, 2));

                Volume = new PositiveLevel(ms.ReadByte()); // using the int returned by ReadByte as such, no cast to byte needed
                Pan = new PositiveLevel(ms.ReadByte());
                EffectPath = (EffectPath) ms.ReadByte();  // straight from int to enum
                Transpose = new Transpose((byte) ms.ReadByte());  // use byte so that the value can be adjusted from SysEx
                Tune = new SignedLevel((byte) ms.ReadByte());     // ditto ^
                Zone = new Zone((byte) ms.ReadByte(), (byte) ms.ReadByte());
                ReceiveChannel = new Channel((byte) ms.ReadByte());
            }
        }

        //
        // ISystemExclusiveData implementation
        //

        public List<byte> Data
        {
            get
            {
                return new List<byte>();
            }
        }

        public int DataLength => DataSize; // See "3.1.6.2 Section data"
    }

    public class MultiVelocitySwitchSettings: ISystemExclusiveData
    {
        public VelocitySwitchKind SwitchKind;  // enumeration
        public byte Value;  // 1~127

        public MultiVelocitySwitchSettings()
        {
            SwitchKind = VelocitySwitchKind.Off;
            Value = 1;
        }

        public MultiVelocitySwitchSettings(byte kindByte, byte valueByte)
        {
            SwitchKind = (VelocitySwitchKind) kindByte;
            Value = valueByte;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            switch (SwitchKind)
            {
                case VelocitySwitchKind.Off:
                    builder.Append("OFF");
                    break;

                case VelocitySwitchKind.Loud:
                    builder.Append("LOUD");
                    break;

                case VelocitySwitchKind.Soft:
                    builder.Append("SOFT");
                    break;
            }

            builder.Append($" {Value}");

            return builder.ToString();
        }

        //
        // ISystemExclusiveData implementation
        //

        public List<byte> Data
        {
            get
            {
                var data = new List<byte>()
                {
                    (byte) this.SwitchKind,
                    this.Value
                };

                return data;
            }
        }

        public int DataLength => 2; // See "3.1.6.2 Section data"
    }
}
