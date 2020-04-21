using System;
using System.Text;
using System.Collections.Generic;

namespace KSynthLib.K4
{
    public abstract class Patch
    {
        public const int NameLength = 10;

        protected string _name;
        public string Name
        {
            get => _name.Substring(0, NameLength);
            set => _name = value.Substring(0, NameLength);
        }

        private byte _checksum;
        public byte Checksum
        {
            get
            {
                byte[] data = CollectData();
                byte sum = 0;
                foreach (byte b in data)
                {
                    sum += b;
                }
                sum += 0xA5;
                return sum;
            }

            set
            {
                _checksum = value;
            }
        }

        protected string GetName(byte[] data, int offset)
        {
            // Brute-force the name in s0 ... s9
            byte[] bytes = { data[0], data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[8], data[9] };
        	return Encoding.ASCII.GetString(bytes);
        }

        protected byte ComputeChecksum(byte[] data)
        {
            byte sum = 0;
            foreach (byte b in data)
            {
                sum += b;
            }
            sum += 0xA5;
            return sum;
        }

        protected abstract byte[] CollectData();

        public byte[] ToData()
        {
            List<byte> allData = new List<byte>();

            byte[] data = CollectData();
            allData.AddRange(data);

            byte sum = ComputeChecksum(data);
            allData.Add(sum);

            return allData.ToArray();
        }
    }
}