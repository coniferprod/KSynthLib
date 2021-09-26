using System;

namespace KSynthLib.Common
{
    public class PatchUtil
    {
        public static string GetPatchName(int p, int patchCount = 16)
        {
        	int bankIndex = p / patchCount;
	        char bankLetter = "ABCD"[bankIndex];
	        int patchIndex = (p % patchCount) + 1;

	        return string.Format("{0}-{1,2}", bankLetter, patchIndex);
        }

        public static int GetPatchNumber(string s)
        {
            string us = s.ToUpper();
            char[] bankNames = new char[] { 'A', 'B', 'C', 'D' };
            int bankIndex = Array.IndexOf(bankNames, us[0]);
            if (bankIndex < 0)
            {
                return 0;
            }

            int number = 0;
            string ns = us.Substring(1);  // take the rest after the bank letter
            try
            {
                number = Int32.Parse(ns) - 1;  // bring to range 0...15
            }
            catch (FormatException)
            {
                Console.Error.WriteLine($"bad patch number: '{s}'");
            }

            return bankIndex * 16 + number;
        }

        // 0 ~ 127 / C-2 ~ G8
        public static string GetNoteName(int noteNumber) {
            string[] notes = new string[] { "C", "C#", "D", "Eb", "E", "F", "F#", "G", "G#", "A", "Bb", "B" };
            int octave = noteNumber / 12 + 1;
            string name = notes[noteNumber % 12];
            return name + octave;
        }
    }
}