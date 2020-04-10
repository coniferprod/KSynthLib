using System;
using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K4
{
    public class Source
    {
        public const int DataSize = 7;

        private LevelType _delay;
        public int Delay  // 0~100
        {
            get
            {
                return _delay.Value;
            }

            set
            {
                _delay.Value = value;
            }
        }

        public int WaveNumber;  // combined from two bytes

        private EightLevelType _keyScalingCurve;
        public int KeyScalingCurve  // 0~7 / 1~8
        {
            get
            {
                return _keyScalingCurve.Value;
            }

            set
            {
                _keyScalingCurve.Value = value;
            }
        }

        private CoarseType _coarse;
        public int Coarse // 0~48 / ±24
        {
            get 
            {
                return _coarse.Value;
            }

            set
            {
                _coarse.Value = value;
            }
        }

        public bool KeyTracking; 

        public int FixedKey; // 0 ~ 115 / C-1 ~ G8

        private DepthType _fine;        
        public int Fine // 0~100 / ±50
        {
            get
            {
                return _fine.Value;
            }

            set
            {
                _fine.Value = value;
            }
        }

        public bool PressureToFrequencySwitch; 

        public bool VibratoSwitch;

        private EightLevelType _velocityCurve;
        public int VelocityCurve // 0~7 / 1~8
        {
            get
            {
                return _velocityCurve.Value;
            }

            set
            {
                _velocityCurve.Value = value;
            }
        }

        public Source()
        {
            _delay = new LevelType();
            WaveNumber = 10;  // "SAW 1"
            _keyScalingCurve = new EightLevelType();
            _coarse = new CoarseType(0);
            KeyTracking = true;
            FixedKey = 0;
            _fine = new DepthType();
            PressureToFrequencySwitch = true;
            VibratoSwitch = false;
            _velocityCurve = new EightLevelType();
        }

        public Source(byte[] data)
        {
            int offset = 0;
            byte b = 0;  // will be reused when getting the next byte

            (b, offset) = Util.GetNextByte(data, offset);
            _delay = new LevelType(b & 0x7f);

            int waveSelectHigh = 0;
            int waveSelectLow = 0;
            (b, offset) = Util.GetNextByte(data, offset);
            waveSelectHigh = b & 0x01;
            _keyScalingCurve = new EightLevelType(((b >> 4) & 0x07) + 1); // 0...7 to 1...8            

            byte b2 = 0;
            (b2, offset) = Util.GetNextByte(data, offset);
            waveSelectLow = b2 & 0x7f;

            // Combine the wave select bits:
            string waveSelectBitString = String.Format("{0}{1}", Convert.ToString(waveSelectHigh, 2), Convert.ToString(waveSelectLow, 2));
            WaveNumber = Convert.ToInt32(waveSelectBitString, 2) + 1;

            (b, offset) = Util.GetNextByte(data, offset);
            // Here the MIDI implementation's SysEx format is a little unclear.
            // My interpretation is that the low six bits are the coarse value,
            // and b6 is the key tracking bit (b7 is zero).
            KeyTracking = b.IsBitSet(6);
            _coarse = new CoarseType((b & 0x3f) - 24);  // 00 ~ 48 /±24

            (b, offset) = Util.GetNextByte(data, offset);
            FixedKey = b & 0x7f;

            (b, offset) = Util.GetNextByte(data, offset);
            _fine = new DepthType((b & 0x7f) - 50);

            (b, offset) = Util.GetNextByte(data, offset);
            PressureToFrequencySwitch = b.IsBitSet(0);
            VibratoSwitch = b.IsBitSet(1);
            _velocityCurve = new EightLevelType(((b >> 2) & 0x07) + 1);  // 0...7 to 1...8
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("S.COMMON\n");
            builder.Append(String.Format("DELAY      ={0,3}\n", Delay));
            builder.Append(String.Format("VEL CURVE  ={0,3}\n", VelocityCurve));
            builder.Append(String.Format("KS CURVE   ={0,3}\n", KeyScalingCurve + 1));
            builder.Append("DCO\n");
            builder.Append(String.Format("WAVE       ={0,3} ({1})\n", WaveNumber + 1, Wave.Instance[WaveNumber]));
            builder.Append(String.Format("KEY TRACK  ={0}\n", KeyTracking ? "ON" : "OFF"));
            builder.Append(String.Format("COARSE     ={0,3}\nFINE       ={1,3}\n", Coarse - 24, Fine));
            builder.Append(String.Format("FIXED KEY  ={0} ({1})\n", GetNoteName(FixedKey), FixedKey));
            builder.Append(String.Format("PRESS      ={0}\nVIB/A.BEND ={1}\n", PressureToFrequencySwitch ? "ON" : "OFF", VibratoSwitch ? "ON" : "OFF"));
            return builder.ToString();
        }

        public static string GetNoteName(int noteNumber) {
            string[] notes = new string[] {"A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#"};
            int octave = noteNumber / 12 + 1;
            string name = notes[noteNumber % 12];
            return name + octave;
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();
            data.Add((byte)Delay);

            // Make an 8-bit binary string representation
            string waveNumberString = Convert.ToString(WaveNumber, 2).PadLeft(8, '0');
            //System.Console.WriteLine(String.Format("wave number in binary = '{0}'", waveNumberString));
            string ksCurveString = Convert.ToString(KeyScalingCurve - 1, 2).PadLeft(3, '0');  // from 1...8 to 0...7
            StringBuilder b34 = new StringBuilder("0");
            b34.Append(ksCurveString);
            b34.Append("000");
            b34.Append(waveNumberString[0]);
            data.Add(Convert.ToByte(b34.ToString(), 2));
            data.Add(Convert.ToByte(waveNumberString.Substring(1), 2));

            // Pack the coarse and key track values into one byte
            StringBuilder b42 = new StringBuilder("0");
            b42.Append(KeyTracking ? "1" : "0");
            b42.Append(Convert.ToString(Coarse + 24, 2).PadLeft(6, '0'));  // from -24...24 to 0...48
            data.Add(Convert.ToByte(b42.ToString(), 2));

            data.Add((byte)FixedKey);
            data.Add((byte)(Fine + 50));

            // Pack the velocity curve and a few other values into one byte
            StringBuilder b54 = new StringBuilder();
            b54.Append(Convert.ToString(VelocityCurve - 1, 2).PadLeft(6, '0'));
            b54.Append(VibratoSwitch ? "1" : "0");
            b54.Append(PressureToFrequencySwitch ? "1" : "0");
            data.Add(Convert.ToByte(b54.ToString(), 2));

            return data.ToArray();
        }        
    }
}
