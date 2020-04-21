using System;
using System.IO;
using System.Diagnostics;

using KSynthLib.Common;
using KSynthLib.K4;

namespace Driver
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var pathName = Path.Combine(path, $"tmp/k4tmp/A401.SYX");

            byte[] data = File.ReadAllBytes(pathName);
            Bank bank = new Bank(data);
            foreach (SinglePatch sp in bank.Singles)
            {
                Console.WriteLine(sp.Name);
            }
        }
    }
}
