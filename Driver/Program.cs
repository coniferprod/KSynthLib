using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using KSynthLib.Common;
using KSynthLib.K4;
using KSynthLib.K5000;

using SyxPack;

namespace Driver
{
    class Program
    {
        static void Main(string[] args)
        {
            //DoK4();

            if (args.Length < 1)
            {
                Console.WriteLine("Missing filename");
                return;
            }

            DoK5000(args[0]);

        }

        static void DoK4()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var pathName = Path.Combine(path, $"tmp/K4/K4/A401.SYX");

            byte[] fileData = File.ReadAllBytes(pathName);

            // Exercise the new SystemExclusive.ManufacturerSpecificMessage type:
            var message = Message.Create(fileData);
            Console.WriteLine("Message created from file data");
            Console.WriteLine(message);

            var header = new SystemExclusiveHeader(message.Payload.ToArray());
            Console.WriteLine("Header: {0}", header);

            var patchDataLength = message.Payload.Count - SystemExclusiveHeader.DataSize;
            var patchData = message.Payload.GetRange(SystemExclusiveHeader.DataSize, patchDataLength);
            Console.WriteLine("Patch data length = {0}", patchDataLength);

            // Extract the patch bytes (discarding the SysEx header and terminator)
            /*
            var dataLength = fileData.Length - SystemExclusiveHeader.DataSize - 1;
            var data = new byte[dataLength];
            Array.Copy(fileData, SystemExclusiveHeader.DataSize, data, 0, dataLength);
            */

            Bank bank = new Bank(patchData.ToArray());

            Console.WriteLine("Single patches:");
            var patchNumber = 0;
            foreach (KSynthLib.K4.SinglePatch sp in bank.Singles)
            {
                string name = PatchUtil.GetPatchName(patchNumber);
                Console.WriteLine($"{name} {sp.Name}");
                patchNumber += 1;
            }

            Console.WriteLine("\nMulti patches:");
            patchNumber = 0;
            foreach (KSynthLib.K4.MultiPatch mp in bank.Multis)
            {
                string name = PatchUtil.GetPatchName(patchNumber);
                Console.WriteLine($"{name} {mp.Name}");
                patchNumber += 1;
            }

/*
            Console.WriteLine($"Drum has {bank.Drum.Notes.Count} notes");
            Console.WriteLine(bank.Drum);
*/
        }

        static void DoK5000(string fileName)
        {
            byte[] fileData = File.ReadAllBytes(fileName);

            Message m = Message.Create(fileData);
            if (m is not ManufacturerSpecificMessage message)
            {
                Console.WriteLine("Unable to construct a message from file data");
                return;
            }

            // Now `message` is in scope, so we can use it to examine.

            Console.WriteLine($"Message payload length is {message.Payload.Count} bytes");
            if (message.Payload.Count < 32)  // what about parameter messages (in the future?)
            {
                Console.WriteLine("Not enough data!");
                return;
            }

            // Get enough data for the dump header:
            var dumpHeaderBytes = message.Payload.GetRange(0, 32).ToArray();
            Console.WriteLine($"\nDump header raw data:");
            var hexDump = new HexDump();
            hexDump.Data = dumpHeaderBytes.ToList();
            Console.WriteLine(hexDump);

            var dumpHeader = new DumpHeader(dumpHeaderBytes);
            Console.WriteLine($"\nDump header (length {dumpHeader.DataLength} bytes):");
            Console.WriteLine(dumpHeader);

            if (dumpHeader.Cardinality == KSynthLib.K5000.Cardinality.One)
            {
                if (dumpHeader.Kind == PatchKind.Single)  // one single
                {
                    // Sub-byte #1 is the patch number
                    var patchNumber = dumpHeader.Instrument;

                    // The message payload is the single patch data
                    KSynthLib.K5000.SinglePatch patch = new KSynthLib.K5000.SinglePatch(message.Payload.ToArray());

                    Console.WriteLine($"Patch: {patchNumber.Value}  Name: {patch.SingleCommon.Name}");
                }
                else  // one combi/multi
                {

                }
            }
            else  // we have a block of singles or multis
            {
                if (dumpHeader.Kind == PatchKind.Single)
                {
                    var patchCount = dumpHeader.ToneMap.Count;
                    Console.WriteLine($"Contains {patchCount} patches");

                    var bytesLeft = message.Payload.Count - dumpHeader.DataLength;
                    Console.WriteLine($"Will use {bytesLeft} bytes of patch data starting at offset {dumpHeader.DataLength}");
                    var allData = message.Payload.GetRange(dumpHeader.DataLength, bytesLeft);

                    var offset = 0;

                    var patches = new List<KSynthLib.K5000.SinglePatch>();
                    for (int i = 0; i < patchCount; i++)
                    {
                        byte checksum = allData[offset];
                        Console.WriteLine($"Checksum = {checksum:X2}");
                        offset += 1;

                        bytesLeft = allData.Count - offset;
                        var patchData = allData.GetRange(offset, bytesLeft);
                        Console.WriteLine($"Patch data is from {offset} to {offset + bytesLeft} ({patchData.Count} bytes)");

                        hexDump.Data = patchData.GetRange(0, 32);
                        Console.WriteLine($"First 32 bytes of patch data:");
                        Console.WriteLine(hexDump);
                        var patch = new KSynthLib.K5000.SinglePatch(patchData.ToArray());
                        patches.Append(patch);

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

                        Console.WriteLine($"{patch.SingleCommon.Name}: {addCount}ADD {pcmCount}PCM, length = {patch.DataLength}");

                        offset += patch.DataLength;
                    }
                }
                else  // block of combi/multi
                {
                    var bytesLeft = message.Payload.Count - dumpHeader.DataLength;
                    Console.WriteLine($"Will use {bytesLeft} bytes of patch data starting at offset {dumpHeader.DataLength}");
                    var allData = message.Payload.GetRange(dumpHeader.DataLength, bytesLeft);

                    // No tone map present, go straight to processing the multis

                    var offset = 0;

                    var bank = new MultiBank(allData.ToArray());
                    for (int i = 0; i < MultiBank.PatchCount; i++)
                    {
                        var patch = bank.Patches[i];
                        var name = patch.Common.Name;
                        Console.WriteLine($"{i + 1}: {name}");
                    }

                }

            }

/*
            Console.WriteLine($"Total patch size = {totalPatchSize} bytes");

            var patches = patchNumbers.Zip(singlePatches, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);
            foreach (var key in patches.Keys)
            {
                Console.WriteLine($"{dumpHeader.Bank}{(key + 1):D3} {patches[key].SingleCommon.Name}");
            }
*/

            // Test the hex dump
            hexDump.Data = fileData.ToList();
            hexDump.Configuration = new HexDumpConfiguration
            {
                BytesPerLine = 16,
                Uppercase = true,
                Included = IncludeOptions.Offset | IncludeOptions.PrintableCharacters | IncludeOptions.MiddleGap
            };
            var dumpOutput = hexDump.ToString();
            //Console.WriteLine(dumpOutput);
            //Console.WriteLine($"hex dump length = {dumpOutput.Length} characters");
        }
    }
}
