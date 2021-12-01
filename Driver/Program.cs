using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

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

            pathName = Path.Combine(path, $"tmp/K5000/ASL-SYX/Disk1/DBankINT.syx");
            fileData = File.ReadAllBytes(pathName);
            var dumpHeader = new DumpHeader(fileData);
            Console.WriteLine($"Card = {dumpHeader.Card}, Bank = {dumpHeader.Bank}, Kind = {dumpHeader.Kind}");

            var offset = 8;  // skip the SysEx header

            // For a block data dump, need to parse the tone map
            byte[] buffer;
            (buffer, offset) = Util.GetNextBytes(fileData, offset, PatchMap.Size);
            // now the offset has been updated to past the tone map
            var patchMap = new PatchMap(buffer);

            List<int> patchNumbers = new List<int>();

            Console.WriteLine("Patches included:");
            var patchCount = 0;
            for (var i = 0; i < PatchMap.PatchCount; i++)
            {
                if (patchMap[i])
                {
                    patchCount += 1;
                    Console.Write(i + 1);
                    Console.Write(" ");

                    patchNumbers.Add(i);
                }
            }
            Console.WriteLine($"\nTotal = {patchCount} patches");

            // Console.Error.WriteLine($"offset = {offset}");

            // Whatever the first patch is, it must be at least this many bytes (always has at least two sources)
            var minimumPatchSize = SingleCommonSettings.DataSize + 2 * KSynthLib.K5000.Source.DataSize;
            //Console.Error.WriteLine($"minimum patch size = {minimumPatchSize}");

            var totalPatchSize = 0;  // the total size of all the single patches

            var singlePatches = new List<KSynthLib.K5000.SinglePatch>();
            for (var i = 0; i < patchCount; i++)
            {
                var startOffset = offset;  // save the current offset because we need to copy more bytes later

                var sizeToRead = Math.Max(minimumPatchSize, fileData.Length - offset);
                Console.WriteLine($"About to read {sizeToRead} bytes starting from offset {offset:X4}h");

                // We don't know yet how many bytes the patch is, but it is at least the minimum size
                (buffer, offset) = Util.GetNextBytes(fileData, offset, sizeToRead);
                // the offset has now been updated past the read size, so need to adjust it back later

                //Console.Error.WriteLine($"Copied {minimumPatchSize} bytes from offset {offset} to singleData");
                //Console.Error.WriteLine(Util.HexDump(buffer));
                Console.Error.WriteLine($"checksum = {buffer[0]:X2}H");

                var patch = new KSynthLib.K5000.SinglePatch(buffer);

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
                var patchSize = 1 + SingleCommonSettings.DataSize  // includes the checksum
                    + patch.Sources.Length * KSynthLib.K5000.Source.DataSize  // all sources have this part
                    + addCount * AdditiveKit.DataSize;
                Console.WriteLine($"{pcmCount}PCM {addCount}ADD size={patchSize} bytes");

                offset = startOffset;  // back up to the start of the patch data
                // Read the whole patch now that we know its size
                Console.WriteLine($"About to read {patchSize} bytes starting from offset {offset:X4}h");
                (buffer, offset) = Util.GetNextBytes(fileData, offset, patchSize);

                totalPatchSize += patchSize;

                singlePatches.Add(patch);
                //Console.WriteLine(patch);
                //Console.WriteLine($"{patch.SingleCommon.Name}");
                //Console.WriteLine("------------");

                string jsonString = JsonConvert.SerializeObject(
                    	patch,
                    	Newtonsoft.Json.Formatting.Indented,
                    	new Newtonsoft.Json.Converters.StringEnumConverter()
                	);
	                Console.WriteLine(jsonString);
            }

/*
            Console.WriteLine($"Total patch size = {totalPatchSize} bytes");

            var patches = patchNumbers.Zip(singlePatches, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);
            foreach (var key in patches.Keys)
            {
                Console.WriteLine($"{dumpHeader.Bank}{(key + 1):D3} {patches[key].SingleCommon.Name}");
            }
*/

        }
    }
}
