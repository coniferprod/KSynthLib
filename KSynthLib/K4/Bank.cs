using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;

using KSynthLib.Common;

namespace KSynthLib.K4
{
    public class Bank
    {
        public const int SinglePatchCount = 64;

        public List<SinglePatch> Singles;
        public List<MultiPatch> Multis;

        public Bank()
        {
            Singles = new List<SinglePatch>();
            Multis = new List<MultiPatch>();
        }

        public Bank(byte[] data) : this()
        {
            SystemExclusiveHeader header = new SystemExclusiveHeader(data);
            Console.WriteLine(Util.HexDump(header.ToData()));

            int patchDataLength = data.Length - SystemExclusiveHeader.DataSize - 1;
            byte[] patchData = new byte[patchDataLength];
            int dataStartIndex = SystemExclusiveHeader.DataSize;
            Console.WriteLine($"Copying {patchDataLength} bytes from offset {dataStartIndex} to patchData");
            Buffer.BlockCopy(data, dataStartIndex, patchData, 0, patchDataLength);

            int offset = 0;
            for (int i = 0; i < SinglePatchCount; i++)
            {
                byte[] singleData = new byte[SinglePatch.DataSize];
                Console.WriteLine($"Copying {SinglePatch.DataSize} bytes from offset {offset} to singleData");
                Buffer.BlockCopy(patchData, offset, singleData, 0, SinglePatch.DataSize);
                Console.WriteLine(Util.HexDump(singleData));
                this.Singles.Add(new SinglePatch(singleData));
                offset += SinglePatch.DataSize;
            }
        }
    }
}

