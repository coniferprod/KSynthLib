using System.Collections.Generic;
using System.Text;

namespace KSynthLib.K5000
{
    /// <summary>The graphical EQ settings of a patch.</summary>
    public class GEQSettings
    {
        public const int DataSize = 7;

        private FreqType _freq1; // 58(-6) ~ 70(+6), so 64 = 0
        public sbyte Freq1
        {
            get => _freq1.Value;
            set => _freq1.Value = value;
        }

        private FreqType _freq2;
        public sbyte Freq2
        {
            get => _freq2.Value;
            set => _freq2.Value = value;
        }

        private FreqType _freq3;
        public sbyte Freq3
        {
            get => _freq3.Value;
            set => _freq3.Value = value;
        }

        private FreqType _freq4;
        public sbyte Freq4
        {
            get => _freq4.Value;
            set => _freq4.Value = value;
        }

        private FreqType _freq5;
        public sbyte Freq5
        {
            get => _freq5.Value;
            set => _freq5.Value = value;
        }

        private FreqType _freq6;
        public sbyte Freq6
        {
            get => _freq6.Value;
            set => _freq6.Value = value;
        }

        private FreqType _freq7;
        public sbyte Freq7
        {
            get => _freq7.Value;
            set => _freq7.Value = value;
        }

        public GEQSettings()
        {
            _freq1 = new FreqType();
            _freq2 = new FreqType();
            _freq3 = new FreqType();
            _freq4 = new FreqType();
            _freq5 = new FreqType();
            _freq6 = new FreqType();
            _freq7 = new FreqType();
        }

        public GEQSettings(byte[] data, int offset)
        {
            // 58(-6) ~ 70(+6), so 64 is zero
            _freq1 = new FreqType(data[offset]);
            _freq2 = new FreqType(data[offset + 1]);
            _freq3 = new FreqType(data[offset + 2]);
            _freq4 = new FreqType(data[offset + 3]);
            _freq5 = new FreqType(data[offset + 4]);
            _freq6 = new FreqType(data[offset + 5]);
            _freq7 = new FreqType(data[offset + 6]);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"{Freq1} {Freq2} {Freq3} {Freq4} {Freq5} {Freq6} {Freq7}\n");
            // TODO: Add the sign, like "+6" or "-6"
            return builder.ToString();
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();
            data.Add(_freq1.AsByte());
            data.Add(_freq2.AsByte());
            data.Add(_freq3.AsByte());
            data.Add(_freq4.AsByte());
            data.Add(_freq5.AsByte());
            data.Add(_freq6.AsByte());
            data.Add(_freq7.AsByte());
            return data.ToArray();
        }
    }
}
