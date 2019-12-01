namespace KSynthLib.K5
{
    public enum SystemExclusiveFunction
    {
        OneBlockDataRequest = 0x00,
        AllBlockDataRequest = 0x01,
        ParameterSend = 0x10,
        OneBlockDataDump = 0x20,
        AllBlockDataDump = 0x21,
        ProgramSend = 0x30,
        WriteComplete = 0x40,
        WriteError = 0x41,
        WriteErrorProtect = 0x42,
        WriteErrorNoCard = 0x43,
        MachineIDRequest = 0x60,
        MachineIDAcknowledge = 0x61
    }
}
