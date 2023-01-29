using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using KSynthLib.Common;


namespace KSynthLib.K4
{
    public class AmplifierEnvelope: ISystemExclusiveData
    {
        [Range(0, 100, ErrorMessage = "{0} must be between {1} and {2}")]
        public int Attack;

        [Range(0, 100, ErrorMessage = "{0} must be between {1} and {2}")]
        public int Decay;

        [Range(0, 100, ErrorMessage = "{0} must be between {1} and {2}")]
        public int Sustain;

        [Range(0, 100, ErrorMessage = "{0} must be between {1} and {2}")]
        public int Release;

        public AmplifierEnvelope()
        {
            Attack = 0;
            Decay = 0;
            Sustain = 0;
            Release = 0;
        }

        public AmplifierEnvelope(byte a, byte d, byte s, byte r)
        {
            Attack = a;
            Decay = d;
            Sustain = s;
            Release = r;
        }

        public AmplifierEnvelope(List<byte> data)
        {
            Attack = data[0];
            Decay = data[1];
            Sustain = data[2];
            Release = data[3];
        }

        public override string ToString()
        {
            return $"A:{Attack} D:{Decay} S:{Sustain} R:{Release}";
        }

        public List<byte> GetSystemExclusiveData()
        {
            var data = new List<byte>();

            data.Add((byte)Attack);
            data.Add((byte)Decay);
            data.Add((byte)Sustain);
            data.Add((byte)Release);

            return data;
        }
    }
}
