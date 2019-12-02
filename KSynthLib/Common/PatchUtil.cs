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

	        return String.Format("{0}-{1,2}", bankLetter, patchIndex);
        }
    }
}