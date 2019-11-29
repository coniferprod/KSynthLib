using KSynthLib.Common;

namespace KSynthLib.K5
{
    public enum BankName
    {
        BankA,
        BankB,
        BankC,
        BankD
    }

    public class Track
    {
        public BankName Bank;  // 0/A, 1/B, 2/C, 3/D
        public int Number;

    }

    public class Multi: Patch
    {
        public const int NumTracks = 16;

        public Track[] Tracks;

        public Multi()
        {
            Tracks = new Track[NumTracks];
        }
    }    
}
