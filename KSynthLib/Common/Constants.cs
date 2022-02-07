namespace KSynthLib.Common
{
    public class Constants
    {
        public const byte ManufacturerID = 0x40;  // Kawai = 40h
    }

    public enum MachineID
    {
        K5 = 0x02, // 02h = K5/K5m
        K1 = 0x03, // 03h = K1, K1m, K1 II
        K4 = 0x04, // 04h = K4/K4r
        K5000 = 0x0a // 0ah, sub ID: 01h = K5000W, 02h = K5000S, 03h = K5000R
    }
}
