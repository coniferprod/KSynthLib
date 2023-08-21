using System.Text;
using System.Collections.Generic;

using SyxPack;
using KSynthLib.Common;

namespace KSynthLib.K4
{
    /// <summary>
    /// One source of a single patch.
    /// </summary>
    public class Source : ISystemExclusiveData
    {
        public const int DataSize = 7;

        public Level Delay;
        public Wave Wave;
        public VelocityCurve KeyScalingCurve;
        public Coarse Coarse;
        public bool KeyTrack;
        public Key FixedKey;  // 0 ~ 115 / C-1 ~ G8
        public Depth Fine;
        public bool PressureFrequency;
        public bool Vibrato;
        public VelocityCurve VelocityCurve;

        /// <summary>
        /// Constructs a source from default values.
        /// </summary>
        public Source()
        {
            Delay = new Level();
            Wave = new Wave(10);  // "SAW 1"
            KeyScalingCurve = VelocityCurve.Curve1;
            Coarse = new Coarse(0);
            KeyTrack = true;
            FixedKey = new Key(60);
            Fine = new Depth();
            PressureFrequency = true;
            Vibrato = false;
            VelocityCurve = VelocityCurve.Curve1;
        }

        /// <summary>
        /// Constructs a source from binary System Exclusive data.
        /// </summary>
        /// <param name="data">System Exclusive data</param>
        /// <remarks>
        /// The no-argument constructor is used to initialize the data members.
        /// </remarks>
        public Source(byte[] data) : this()
        {
            int offset = 0;
            byte b = 0;  // will be reused when getting the next byte

            (b, offset) = Util.GetNextByte(data, offset);
            Delay = new Level(b);

            byte waveSelectHigh = 0;
            byte waveSelectLow = 0;
            (b, offset) = Util.GetNextByte(data, offset);
            waveSelectHigh = (byte)(b & 0x01);
            KeyScalingCurve = (VelocityCurve)((b >> 4) & 0x07);

            byte b2 = 0;
            (b2, offset) = Util.GetNextByte(data, offset);
            waveSelectLow = (byte)(b2 & 0x7f);

            Wave = new Wave(waveSelectHigh, waveSelectLow);

            (b, offset) = Util.GetNextByte(data, offset);
            // Here the MIDI implementation's SysEx format is a little unclear.
            // My interpretation is that the low six bits are the coarse value,
            // and b6 is the key tracking bit (b7 is zero).
            KeyTrack = b.IsBitSet(6);
            Coarse = new Coarse((sbyte)((b & 0x3f) - 24));  // 00 ~ 48 to ±24

            (b, offset) = Util.GetNextByte(data, offset);
            FixedKey = new Key(b);

            (b, offset) = Util.GetNextByte(data, offset);
            Fine = new Depth(b);

            (b, offset) = Util.GetNextByte(data, offset);
            PressureFrequency = b.IsBitSet(0);
            Vibrato = b.IsBitSet(1);
            VelocityCurve = (VelocityCurve)((b >> 2) & 0x07);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine("S.COMMON");
            builder.AppendLine($"DELAY      ={Delay,3}");
            builder.AppendLine($"VEL CURVE  ={VelocityCurve,3}");
            builder.AppendLine($"KS CURVE   ={KeyScalingCurve,3}");
            builder.AppendLine("DCO");
            builder.AppendLine(string.Format("WAVE       ={0,3} ({1})", Wave.Number, Wave.Name));
            builder.AppendLine(string.Format("KEY TRACK  ={0}", KeyTrack ? "ON" : "OFF"));
            builder.AppendLine($"COARSE     ={Coarse,3}\nFINE       ={Fine,3}");
            builder.AppendLine($"FIXED KEY  ={FixedKey.NoteName} ({FixedKey})");
            builder.AppendLine(
                string.Format(
                    "PRESS      ={0}\nVIB/A.BEND ={1}",
                    PressureFrequency ? "ON" : "OFF",
                    Vibrato ? "ON" : "OFF"
                )
            );

            return builder.ToString();
        }

        //
        // Implementation of ISystemExclusiveData interface
        //

        /// <summary>
        /// Generates a binary System Exclusive representation of the data.
        /// </summary>
        /// <returns>
        /// A byte array with SysEx data.
        /// </returns>
        public List<byte> Data
        {
            get
            {
                var data = new List<byte>();
                data.Add(Delay.ToByte());

                // s34/s35/s36/s37 wave select h and ks

                byte s34 = (byte)(((byte)KeyScalingCurve) << 4);  // shift it to the top four bits
                var (waveSelectHigh, waveSelectLow) = Wave.WaveSelect;
                if (waveSelectHigh == 0x01)
                {
                    s34.SetBit(0);
                }
                data.Add(s34);

                // s38/s39/s40/s41 wave select l
                data.Add(waveSelectLow);

                // s42/s43/s44/s45 key track and coarse
                byte s42 = Coarse.ToByte();
                if (KeyTrack) {
                    s42.SetBit(6);
                }
                data.Add(s42);

                data.Add(FixedKey.ToByte());
                data.Add(Fine.ToByte());

                // s54/s55/s56/s57 vel curve, vib/a.bend, prs/freq
                byte s54 = (byte)(((byte)VelocityCurve) << 2);
                if (Vibrato)
                {
                    s54.SetBit(1);
                }
                if (PressureFrequency)
                {
                    s54.SetBit(0);
                }
                data.Add(s54);

                return data;
            }
        }

        public int DataLength => DataSize;
    }
}
