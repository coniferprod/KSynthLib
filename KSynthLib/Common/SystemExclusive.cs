using System;
using System.Collections.Generic;

namespace KSynthLib.Common
{
    public class SystemExclusiveHeader
    {
        public const byte Initiator = 0xF0;
        public const byte Terminator = 0xF7;

        public const int DataSize = 8;

        public byte ManufacturerID;
	    public byte Channel;
	    public byte Function;
	    public byte Group;
	    public byte MachineID;
	    public byte Substatus1;
	    public byte Substatus2;

        public SystemExclusiveHeader()
        {

        }

        public SystemExclusiveHeader(byte[] data)
        {
            // TODO: Check that data[0] is the SysEx identifier $F0
            ManufacturerID = data[1];
            Channel = data[2];
		    Function = data[3];
		    Group = data[4];
		    MachineID = data[5];
		    Substatus1 = data[6];
		    Substatus2 = data[7];
        }

        public override string ToString()
        {
            return string.Format("ManufacturerID = {0,2:X2}h, Channel = {1,2:X2}h, Function = {2,2:X2}h, Group = {3,2:X2}h, MachineID = {4,2:X2}h, Substatus1 = {5,2:X2}h, Substatus2 = {6,2:X2}h", ManufacturerID, Channel, Function, Group, MachineID, Substatus1, Substatus2);
        }

        public byte[] ToData()
        {
            var data = new List<byte>();
            data.Add(ManufacturerID);
            data.Add(Channel);
            data.Add(Function);
            data.Add(Group);
            data.Add(MachineID);
            data.Add(Substatus1);
            data.Add(Substatus2);
            return data.ToArray();
        }
    }
}
