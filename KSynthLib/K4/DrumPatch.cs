using System;
using System.Collections.Generic;

namespace KSynthLib.K4
{
    public class DrumPatch : Patch
    {
        public const int DataSize = 682;
        
        public DrumPatch()
        {

        }

        protected override byte[] CollectData()
        {
            List<byte> data = new List<byte>();

            // Fill with dummy data. TODO: Use real data.
            for (int i = 0; i < DataSize - 1; i++)
            {
                data.Add(0);
            }

            return data.ToArray();
        }
    }
}