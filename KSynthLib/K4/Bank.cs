using System;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K4
{
    public class Bank
    {
        public const int SinglePatchCount = 64;
        public const int MultiPatchCount = 64;
        public const int EffectPatchCount = 32;

        public List<SinglePatch> Singles;
        public List<MultiPatch> Multis;

        public DrumPatch Drum;

        public List<EffectPatch> Effects;

        public Bank()
        {
            Singles = new List<SinglePatch>();
            Multis = new List<MultiPatch>();
            Drum = new DrumPatch();
            Effects = new List<EffectPatch>();
        }

        public Bank(byte[] data) : this()
        {
            SystemExclusiveHeader header = new SystemExclusiveHeader(data);
            Console.Error.WriteLine(Util.HexDump(header.ToData()));

            int patchDataLength = data.Length - SystemExclusiveHeader.DataSize - 1;
            byte[] patchData = new byte[patchDataLength];
            int dataStartIndex = SystemExclusiveHeader.DataSize;
            //Console.Error.WriteLine($"Copying {patchDataLength} bytes from offset {dataStartIndex} to patchData");
            Buffer.BlockCopy(data, dataStartIndex, patchData, 0, patchDataLength);

            int offset = 0;
            for (int i = 0; i < SinglePatchCount; i++)
            {
                byte[] singleData = new byte[SinglePatch.DataSize];
                //Console.Error.WriteLine($"Copying {SinglePatch.DataSize} bytes from offset {offset} to singleData");
                Buffer.BlockCopy(patchData, offset, singleData, 0, SinglePatch.DataSize);
                //Console.Error.WriteLine(Util.HexDump(singleData));
                this.Singles.Add(new SinglePatch(singleData));
                offset += SinglePatch.DataSize;
            }

            for (int i = 0; i < MultiPatchCount; i++)
            {
                byte[] multiData = new byte[MultiPatch.DataSize];
                Buffer.BlockCopy(patchData, offset, multiData, 0, MultiPatch.DataSize);
                this.Multis.Add(new MultiPatch(multiData));
                offset += MultiPatch.DataSize;
            }

            byte[] drumData = new byte[DrumPatch.DataSize];
            Buffer.BlockCopy(patchData, offset, drumData, 0, DrumPatch.DataSize);
            this.Drum = new DrumPatch(drumData);
            offset += DrumPatch.DataSize;

            for (int i = 0; i < EffectPatchCount; i++)
            {
                byte[] effectData = new byte[EffectPatch.DataSize];
                Buffer.BlockCopy(patchData, offset, effectData, 0, EffectPatch.DataSize);
                this.Effects.Add(new EffectPatch(effectData));
                offset += EffectPatch.DataSize;
            }
        }
    }
}

