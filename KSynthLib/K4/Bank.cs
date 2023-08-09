using System;
using System.Collections.Generic;
using System.Diagnostics;

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

        // Parse the bank from System Exclusive data. Expects the data without the
        // SysEx header.
        public Bank(byte[] data) : this()
        {
            var offset = 0;
            for (var i = 0; i < SinglePatchCount; i++)
            {
                var singleData = new byte[SinglePatch.DataSize];
                //Console.Error.WriteLine($"Copying {SinglePatch.DataSize} bytes from offset {offset} to singleData");
                Buffer.BlockCopy(data, offset, singleData, 0, SinglePatch.DataSize);
                Debug.Assert(singleData.Length == SinglePatch.DataSize);
                //Console.Error.WriteLine($"Single data length = {singleData.Length}");
                //Console.Error.WriteLine(Util.HexDump(singleData));

                var singlePatch = new SinglePatch(singleData);
                this.Singles.Add(singlePatch);
                offset += SinglePatch.DataSize;
            }

            for (var i = 0; i < MultiPatchCount; i++)
            {
                var multiData = new byte[MultiPatch.DataSize];
                Buffer.BlockCopy(data, offset, multiData, 0, MultiPatch.DataSize);
                Debug.Assert(multiData.Length == MultiPatch.DataSize);
                //Console.Error.WriteLine($"Multi data length = {multiData.Length}");
                var multiPatch = new MultiPatch(multiData);
                this.Multis.Add(multiPatch);
                offset += MultiPatch.DataSize;
            }

            var drumData = new byte[DrumPatch.DataSize];
            Buffer.BlockCopy(data, offset, drumData, 0, DrumPatch.DataSize);
            this.Drum = new DrumPatch(drumData);
            offset += DrumPatch.DataSize;

            for (var i = 0; i < EffectPatchCount; i++)
            {
                var effectData = new byte[EffectPatch.DataSize];
                Buffer.BlockCopy(data, offset, effectData, 0, EffectPatch.DataSize);
                this.Effects.Add(new EffectPatch(effectData));
                offset += EffectPatch.DataSize;
            }
        }
    }
}
