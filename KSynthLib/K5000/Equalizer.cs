using System.Collections.Generic;
using System.Text;

namespace KSynthLib.K5000
{
    /// <summary>The graphical EQ settings of a patch.</summary>
    public class GEQSettings
    {
        public const int DataSize = 7;

        public const int FrequencyCount = 7;

        public List<Frequency> Frequencies;

        public GEQSettings()
        {
            Frequencies = new List<Frequency>();
            for (var i = 0; i < FrequencyCount; i++)
            {
                Frequencies.Add(new Frequency());
            }
        }

        public GEQSettings(byte[] data, int offset)
        {
            // 58(-6) ~ 70(+6), so 64 is zero
            Frequencies = new List<Frequency>();
            for (var i = 0; i < FrequencyCount; i++)
            {
                Frequencies.Add(new Frequency(data[offset] + i));
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (var freq in Frequencies)
            {
                builder.Append($"{freq.Value}");
            }
            builder.Append("\n");
            // TODO: Add the sign, like "+6" or "-6"
            return builder.ToString();
        }

        public byte[] ToData()
        {
            var data = new List<byte>();
            foreach (var freq in Frequencies)
            {
                data.Add(freq.ToByte());
            }
            return data.ToArray();
        }
    }
}
