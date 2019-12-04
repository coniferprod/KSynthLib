using System;
using System.Collections.Generic;

namespace KSynthLib.K5000
{
    public abstract class Patch
    {
        protected string name;
        
        public string Name => name;

        private byte checksum;

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
                checksum = value;
            }

        }

        protected abstract byte[] CollectData();

        protected virtual byte ComputeChecksum(byte[] data)
        {
            byte sum = 0;
            foreach (byte b in data)
            {
                sum += b;
            }
            sum += 0xA5;
            return sum;
        }

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