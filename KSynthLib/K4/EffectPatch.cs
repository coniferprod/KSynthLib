using System;
using System.Collections.Generic;

namespace KSynthLib.K4
{
    public class EffectPatch : Patch
    {
        public const int DataSize = 35;

        public EffectPatch()
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