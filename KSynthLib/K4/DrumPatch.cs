using System;
using System.Collections.Generic;

namespace KSynthLib.K4
{
    public class DrumSource
    {

    }

    public class DrumNote
    {
        public int OutputSelect;
        public int Wave;
        
        public int Decay;
        public int Tune;
        public int Level;
        public byte Checksum;

    }

    public class DrumPatch : Patch
    {
        public const int DataSize = 682;
        
        private int receiveChannel;

        private int volume;
        private int velocityDepth;

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