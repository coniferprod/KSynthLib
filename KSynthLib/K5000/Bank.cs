using System;
using System.Collections.Generic;
using System.IO;

namespace KSynthLib.K5000;  // file scoped namespaces since C# 10

// Kawai K5000 banks contain either single patches (up to 128 of them)
// or exactly 64 multi patches (called combi on K5000W).

public class SingleBank
{
    // Patches in a bank of singles keyed by tone number.
    public Dictionary<int, SinglePatch> Patches;
}

public class MultiBank
{
    public const int PatchCount = 64;

    public MultiPatch[] Patches;

    public MultiBank()
    {
        Console.WriteLine("In MultiBank() constructor");
        this.Patches = new MultiPatch[PatchCount];
        for (int i = 0; i < PatchCount; i++)
        {
            this.Patches[i] = new MultiPatch();
        }
    }

    /// <summary>
    /// Constructs a multi bank patch from System Exclusive data.
    /// </summary>
    /// <param name="data">The SysEx data bytes.</param>
    public MultiBank(byte[] data) : this()
    {
        Console.WriteLine("In MultiBank(byte[]) constructor");
        Console.WriteLine($"Got {data.Length} bytes as parameter");

        using (MemoryStream memory = new MemoryStream(data, false))
        {
            using (BinaryReader reader = new BinaryReader(memory))
            {
                for (int i = 0; i < PatchCount; i++)
                {
                    var patchData = reader.ReadBytes(MultiPatch.DataSize);
                    this.Patches[i] = new MultiPatch(patchData);
                }
            }
        }
    }

#region ISystemExclusiveData implementation for MultiCommon

    public List<byte> Data
    {
        get
        {
            var data = new List<byte>();

            // There is no bank-level checksum.
            // The patches have their own checksums.

            for (int i = 0; i < PatchCount; i++)
            {
                data.AddRange(this.Patches[i].Data);
            }

            return data;
        }
    }
}

#endregion
