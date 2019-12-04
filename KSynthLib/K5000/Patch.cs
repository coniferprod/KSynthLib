using System;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K5000
{
    public abstract class Patch
    {
        public CommonSettings Common;

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

        public Patch()
        {
            Common = new CommonSettings();
        }

        public Patch(byte[] data)
        {
            int offset = 0;
            byte b = 0;
            (b, offset) = Util.GetNextByte(data, offset);
            Checksum = b;
            Console.WriteLine(String.Format("{0:X8} check sum = {1:X2}", offset, Checksum));

            byte[] commonData = new byte[CommonSettings.DataSize];
            Buffer.BlockCopy(data, offset, commonData, 0, CommonSettings.DataSize);
            Common = new CommonSettings(commonData);
            offset += CommonSettings.DataSize;
            Console.WriteLine(String.Format("{0:X8} parsed {1} ({1:X4}h) bytes of common data", offset, CommonSettings.DataSize));
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