using System;
using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K4
{
    /// <summary>
    /// A source of a single patch.
    /// </summary>
    public class Source
    {
        public const int DataSize = 7;

        private LevelType _delay;
        public byte Delay  // 0~100
        {
            get => _delay.Value;
            set => _delay.Value = value;
        }

        /// <summary>
        /// Store for the WaveNumber property.
        /// </summary>
        private Wave _wave;

        /// <summary>
        /// The wave number of this source.
        /// </summary>
        /// <value>
        /// Wave number value stored as 1~256.
        /// </value>
        /// <remarks>
        /// In SysEx data the wave number is stored as 0~255 distributed over two bytes.
        /// </remarks>
        public ushort WaveNumber => _wave.Number;

        /// <summary>
        /// Store for the KeyScalingCurve property.
        /// </summary>
        private VelocityCurveType _keyScalingCurve;

        /// <summary>
        /// The key scaling curve.
        /// </summary>
        /// <value>
        /// Key scaling curve stored as 1~8.
        /// </value>
        /// <remarks>
        /// In SysEx data this is stored as 0~7.
        /// </remarks>
        public byte KeyScalingCurve
        {
            get => _keyScalingCurve.Value;
            set => _keyScalingCurve.Value = value;
        }

        private CoarseType _coarse;
        public sbyte Coarse // 0~48 / ±24
        {
            get => _coarse.Value;
            set => _coarse.Value = value;
        }

        public bool KeyTrack;

        private FixedKeyType _fixedKey; // 0 ~ 115 / C-1 ~ G8
        public byte FixedKey
        {
            get => _fixedKey.Value;
            set => _fixedKey.Value = value;
        }

        private DepthType _fine;
        public sbyte Fine // 0~100 / ±50
        {
            get => _fine.Value;
            set => _fine.Value = value;
        }

        public bool PressureFrequency;

        public bool Vibrato;

        private VelocityCurveType _velocityCurve;
        public byte VelocityCurve // 0~7 / 1~8
        {
            get => _velocityCurve.Value;
            set => _velocityCurve.Value = value;
        }

        /// <summary>
        /// Constructs a source from default values.
        /// </summary>
        public Source()
        {
            _delay = new LevelType();
            _wave = new Wave(10);  // "SAW 1"
            _keyScalingCurve = new VelocityCurveType();
            _coarse = new CoarseType(0);
            KeyTrack = true;
            _fixedKey = new FixedKeyType();
            _fine = new DepthType();
            PressureFrequency = true;
            Vibrato = false;
            _velocityCurve = new VelocityCurveType();
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
            _delay = new LevelType((byte)(b & 0x7f));

            byte waveSelectHigh = 0;
            byte waveSelectLow = 0;
            (b, offset) = Util.GetNextByte(data, offset);
            waveSelectHigh = (byte)(b & 0x01);
            _keyScalingCurve = new VelocityCurveType((byte)((b >> 4) & 0x07));

            byte b2 = 0;
            (b2, offset) = Util.GetNextByte(data, offset);
            waveSelectLow = (byte)(b2 & 0x7f);

            ushort waveNumber = Wave.numberFrom(waveSelectHigh, waveSelectLow);
            _wave = new Wave(waveNumber);

            (b, offset) = Util.GetNextByte(data, offset);
            // Here the MIDI implementation's SysEx format is a little unclear.
            // My interpretation is that the low six bits are the coarse value,
            // and b6 is the key tracking bit (b7 is zero).
            KeyTrack = b.IsBitSet(6);
            _coarse = new CoarseType((sbyte)((b & 0x3f) - 24));  // 00 ~ 48 to ±24

            (b, offset) = Util.GetNextByte(data, offset);
            _fixedKey = new FixedKeyType(b);

            (b, offset) = Util.GetNextByte(data, offset);
            _fine = new DepthType(b);

            (b, offset) = Util.GetNextByte(data, offset);
            PressureFrequency = b.IsBitSet(0);
            Vibrato = b.IsBitSet(1);
            _velocityCurve = new VelocityCurveType((byte)((b >> 2) & 0x07));  // 0...7 to 1...8
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("S.COMMON\n");
            builder.Append($"DELAY      ={Delay,3}\n");
            builder.Append($"VEL CURVE  ={VelocityCurve,3}\n");
            builder.Append($"KS CURVE   ={KeyScalingCurve,3}\n");
            builder.Append("DCO\n");
            builder.Append(string.Format("WAVE       ={0,3} ({1})\n", WaveNumber, _wave.Name));
            builder.Append(string.Format("KEY TRACK  ={0}\n", KeyTrack ? "ON" : "OFF"));
            builder.Append($"COARSE     ={Coarse,3}\nFINE       ={Fine,3}\n");
            builder.Append($"FIXED KEY  ={_fixedKey.NoteName} ({FixedKey})\n");
            builder.Append(string.Format("PRESS      ={0}\nVIB/A.BEND ={1}\n", PressureFrequency ? "ON" : "OFF", Vibrato ? "ON" : "OFF"));
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
            data.Add(Delay);

            // s34/s35/s36/s37 wave select h and ks
            byte s34 = (byte)(KeyScalingCurve << 4);  // shift it to the top four bits
            var (waveSelectHigh, waveSelectLow) = _wave.WaveSelect;
            if (waveSelectHigh == 0x01)
            {
                s34.SetBit(0);
            }
            data.Add(s34);

            // s38/s39/s40/s41 wave select l
            data.Add(waveSelectLow);

            // s42/s43/s44/s45 key track and coarse
            byte s42 = _coarse.AsByte();
            if (KeyTrack) {
                s42.SetBit(6);
            }
            data.Add(s42);

            data.Add((byte)FixedKey);
            data.Add(_fine.AsByte());

            // s54/s55/s56/s57 vel curve, vib/a.bend, prs/freq
            byte s54 = (byte)(VelocityCurve << 2);
            if (Vibrato)
            {
                s54.SetBit(1);
            }
            if (PressureFrequency)
            {
                s54.SetBit(0);
            }
            data.Add(s54);

            return data.ToArray();
        }
    }
}
