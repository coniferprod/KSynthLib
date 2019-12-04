using System;
using System.Collections.Generic;

namespace KSynthLib.K5000
{
    public class MultiPatch : Patch
    {
        protected override byte[] CollectData()
        {
            List<byte> data = new List<byte>();

            return data.ToArray();
        }

        protected override byte ComputeChecksum(byte[] data)
        {
            return 0;
        }

    }
}