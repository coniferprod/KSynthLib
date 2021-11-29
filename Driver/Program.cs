using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

using KSynthLib.Common;
using KSynthLib.K4;

using KSynthLib.K5000;

namespace Driver
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var pathName = Path.Combine(path, $"tmp/K4/K4/A401.SYX");

            byte[] fileData = File.ReadAllBytes(pathName);

            /*
            Bank bank = new Bank(fileData);
            foreach (KSynthLib.K4.SinglePatch sp in bank.Singles)
            {
                Console.WriteLine(sp.Name);
            }

            Console.WriteLine($"Drum has {bank.Drum.Notes.Count} notes");
            Console.WriteLine(bank.Drum);

            byte[] fileData = File.ReadAllBytes(pathName);
            pathName = Path.Combine(path, $"tmp/WizooIni.syx");
            fileData = File.ReadAllBytes(pathName);
            KSynthLib.K5000.SinglePatch singlePatch = new KSynthLib.K5000.SinglePatch(fileData);
            Console.WriteLine(singlePatch.SingleCommon.Name);
            */

            pathName = Path.Combine(path, $"tmp/K5000/ASL-SYX/Disk1/Collctn1.syx");
            fileData = File.ReadAllBytes(pathName);
            var dumpHeader = new DumpHeader(fileData);
            Console.WriteLine($"Card = {dumpHeader.Card}, Bank = {dumpHeader.Bank}, Kind = {dumpHeader.Kind}");

            var offset = 8;  // skip the SysEx header

            // For a block data dump, need to parse the tone map
            var patchMapData = new byte[PatchMap.Size];
            Array.Copy(fileData, offset, patchMapData, 0, PatchMap.Size);
            var patchMap = new PatchMap(patchMapData);

            Console.WriteLine("Patches included:");
            var patchCount = 0;
            for (var i = 0; i < PatchMap.PatchCount; i++)
            {
                if (patchMap[i])
                {
                    patchCount += 1;
                    Console.Write(i + 1);
                    Console.Write(" ");
                }
            }
            Console.WriteLine($"\nTotal = {patchCount} patches");

            offset += PatchMap.Size;  // skip past the tone map

            // Console.Error.WriteLine($"offset = {offset}");
            var patchDataLength = fileData.Length - offset - 1;  // leave out the header, tonemap and SysEx terminator
            var patchData = new byte[patchDataLength];
            Array.Copy(fileData, offset, patchData, 0, patchDataLength);

            var totalPatchSize = 0;

            // Whatever the first patch is, it must be at least this many bytes
            var minimumPatchSize = SingleCommonSettings.DataSize + 2 * KSynthLib.K5000.Source.DataSize;
            //Console.Error.WriteLine($"minimum patch size = {minimumPatchSize}");

            var singlePatches = new List<KSynthLib.K5000.SinglePatch>();
            for (var i = 0; i < patchCount; i++)
            {
                // We don't know yet how many bytes the patch is, but it is at least the minimum size:
                var singleData = new byte[minimumPatchSize];

                Array.Copy(fileData, offset, singleData, 0, minimumPatchSize);
                //Console.Error.WriteLine($"Copied {minimumPatchSize} bytes from offset {offset} to singleData");
                Console.Error.WriteLine(Util.HexDump(singleData));
                Console.Error.WriteLine($"checksum = {singleData[0]:X2}H");

/*
                var patchOffset = 1;  // skip the checksum
                var singleCommonData = new byte[SingleCommonSettings.DataSize];
                Buffer.BlockCopy(singleData, patchOffset, singleCommonData, 0, SingleCommonSettings.DataSize);
                var singleCommon = new SingleCommonSettings(singleCommonData);
                Console.Error.WriteLine(Util.HexDump(singleCommonData));
                Console.Error.WriteLine(singleCommon);

                var sourceData = new byte[KSynthLib.K5000.Source.DataSize];
                Array.Copy(fileData, offset, sourceData, 0, KSynthLib.K5000.Source.DataSize);
                Console.Error.WriteLine($"zone low = {sourceData[0]:X2}H, zone high = {sourceData[1]:X2}H");
                Console.Error.WriteLine($"vel sw = {sourceData[2].ToBinaryString()}b ({sourceData[2]:X2}h)");
                var velocitySwitch = new VelocitySwitchSettings();
                velocitySwitch.SwitchKind = (VelocitySwitchKind)(sourceData[2] >> 5);  // isolate bits 5 & 6
                velocitySwitch.Threshold = (byte)(sourceData[2] & 0b00011111);  // isolate bottom 5 bits
                Console.Error.WriteLine($"Vel.SW = {velocitySwitch}");

                var testSource = new KSynthLib.K5000.Source();
                Console.Error.WriteLine(testSource);
*/


                var patch = new KSynthLib.K5000.SinglePatch(singleData);

                // Find out how many PCM and ADD sources
                var pcmCount = 0;
                var addCount = 0;
                foreach (var source in patch.Sources)
                {
                    if (source.IsAdditive)
                    {
                        addCount += 1;
                    }
                    else
                    {
                        pcmCount += 1;
                    }
                }

                // Figure out the total size of the single patch based on the counts
                var patchSize = SingleCommonSettings.DataSize + pcmCount * KSynthLib.K5000.Source.DataSize + addCount * KSynthLib.K5000.Source.DataSize + addCount * AdditiveKit.DataSize;
                Console.WriteLine($"{pcmCount}PCM {addCount}ADD size={patchSize} bytes");
                Array.Copy(patchData, offset, singleData, 0, patchSize);

                offset += patchSize;
                totalPatchSize = patchSize;

                singlePatches.Add(patch);
            }

            Console.WriteLine($"Total patch size = {totalPatchSize} bytes");

            foreach (var singlePatch in singlePatches)
            {
                Console.WriteLine($"{singlePatch.SingleCommon.Name}");
            }

        }
    }
}
