using System;
using System.IO;
using System.Diagnostics;

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

            byte[] data = File.ReadAllBytes(pathName);
            Bank bank = new Bank(data);
            foreach (KSynthLib.K4.SinglePatch sp in bank.Singles)
            {
                Console.WriteLine(sp.Name);
            }

            Console.WriteLine($"Drum has {bank.Drum.Notes.Count} notes");
            Console.WriteLine(bank.Drum);

            pathName = Path.Combine(path, $"tmp/WizooIni.syx");
            data = File.ReadAllBytes(pathName);
            KSynthLib.K5000.SinglePatch singlePatch = new KSynthLib.K5000.SinglePatch(data);
            Console.WriteLine(singlePatch.SingleCommon.Name);


        }
    }
}
