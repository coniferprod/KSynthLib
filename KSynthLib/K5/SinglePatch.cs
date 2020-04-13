using System;
using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K5
{
    public enum ModulationAssign
    {
        DFGLFO,
        DHG,
        Cutoff,
        Slope,
        Off
    }

    public enum PicMode
    {
        S1,
        S2,
        Both
    }

    public class KeyScaling
    {
        private DepthType _right; // 0~±31
        public sbyte Right
        {
            get => _right.Value;
            set => _right.Value = value;
        }

        private DepthType _left; // 0~±31
        public sbyte Left
        {
            get => _left.Value;
            set => _left.Value = value;
        }

        private KeyNumberType _breakpoint;
        public byte Breakpoint
        {
            get => _breakpoint.Value;
            set => _breakpoint.Value = value;
        }

        public KeyScaling()
        {
            _right = new DepthType();
            _left = new DepthType();
            _breakpoint = new KeyNumberType();
        }

        public override string ToString()
        {
            return $"*KS CURVE*\nLEFT={Left,3}    B.POINT={Breakpoint,3}    RIGHT={Right,3}";
        }
    }

    public class SourceSettings
    {
        private PositiveDepthType _delay;  // 0~31
        public byte Delay
        {
            get => _delay.Value;
            set => _delay.Value = value;
        }

        private DepthType _pedalDepth; // 0~±31
        public sbyte PedalDepth
        {
            get => _pedalDepth.Value;
            set => _pedalDepth.Value = value;
        }

        private DepthType _wheelDepth; // 0~±31
        public sbyte WheelDepth
        {
            get => _wheelDepth.Value;
            set => _wheelDepth.Value = value;
        }

        public ModulationAssign PedalAssign;  // enumeration
        public ModulationAssign WheelAssign;  // enumeration
        public KeyScaling KeyScaling;

        public SourceSettings()
        {
            _delay = new PositiveDepthType();
            _pedalDepth = new DepthType();
            _wheelDepth = new DepthType();
            KeyScaling = new KeyScaling();
        }

        public override string ToString()
        {
            return $"Delay = {Delay}, pedal depth = {PedalDepth}, wheel depth = {WheelDepth}, pedal assign = {PedalAssign}, wheel assign = {WheelAssign}";
        }
    }
    
    public class SinglePatch: Patch
    {
        const int FormantLevelCount = 11;
        public static readonly int NameLength = 8;

        private string _name;
        public string Name
        {
            get => _name.Substring(0, NameLength);
            set => _name = value.Substring(0, NameLength);
        }

        private VolumeType _volume;   // 0~63
        public byte Volume
        {
            get => _volume.Value;
            set => _volume.Value = value;
        }

        private DepthType _balance; // 0~±31
        public sbyte Balance
        {
            get => _balance.Value;
            set => _balance.Value = value;
        }

        public SourceSettings Source1Settings;
        public SourceSettings Source2Settings;

        public bool Portamento;

        private VolumeType _portamentoSpeed; // 0~63
        public byte PortamentoSpeed
        {
            get => _portamentoSpeed.Value;
            set => _portamentoSpeed.Value = value;
        }

        public SourceMode SMode;  // enumeration
        public PicMode PMode;  // enumeration 
        public Source Source1;
        public Source Source2;
        public LFO LFO;
        public bool IsFormantOn;  // true Digital Formant filter is on
        
        public int[] FormantLevels;  // levels for bands C-1 ... C9 (0 ~ 63)
        // Used int[] and not byte[] to get the correct JSON serialization

        public byte Filler;  // retain the byte before the checksum (should be zero but not guaranteed)

        public SinglePatch()
        {
            Name = "NewSound";
            _volume = new VolumeType();
            _balance = new DepthType();

            Source1Settings = new SourceSettings();
            Source2Settings = new SourceSettings();

            Portamento = false;
            _portamentoSpeed = new VolumeType();

            SMode = SourceMode.Full;
            PMode = PicMode.Both;

            Source1 = new Source();
            Source2 = new Source();

            LFO = new LFO();

            IsFormantOn = false;
            FormantLevels = new int[FormantLevelCount];
        }

        public SinglePatch(byte[] data) : this()
        {
            int offset = 0;
            byte b = 0;  // will be reused when getting the next byte

            Name = CollectName(data);  // name is S1...S8

            offset += 8;
            (Volume, offset) = Util.GetNextByte(data, offset);  // S9

            (b, offset) = Util.GetNextByte(data, offset);
            Balance = b.ToSignedByte(); // S10

            // Source 1 and Source 2 settings.
        	// Note that the pedal assign and the wheel assign for one source are in the same byte.
            SourceSettings s1s = new SourceSettings();
            s1s.Delay = data[offset];          // S11
            s1s.PedalDepth = data[offset + 2].ToSignedByte(); // S13
            s1s.WheelDepth = data[offset + 4].ToSignedByte(); // S15
            s1s.PedalAssign = (ModulationAssign)Util.HighNybble(data[offset + 6]); // S17 high nybble
            s1s.WheelAssign = (ModulationAssign)Util.LowNybble(data[offset + 6]); // S17 low nybble
            s1s.KeyScaling = new KeyScaling {  // just a placeholder
                Left = 0,
                Right = 0,
                Breakpoint = 0
            };

            SourceSettings s2s = new SourceSettings();
            s2s.Delay = data[offset + 1];                           // S12
            s2s.PedalDepth = data[offset + 3].ToSignedByte();               // S14
            s2s.WheelDepth = data[offset + 5].ToSignedByte();               // S16
            s2s.PedalAssign = (ModulationAssign)Util.HighNybble(data[offset + 7]); // S18 high nybble
            s2s.WheelAssign = (ModulationAssign)Util.LowNybble(data[offset + 7]); // S18 low nybble
            s2s.KeyScaling = new KeyScaling {  // just a placeholder
                Left = 0,
                Right = 0,
                Breakpoint = 0
            };

            // Assign the source settings later, when the keyscaling has been parsed.
            offset += 8;  // advance past the source settings

	        // portamento setting and portamento speed - S19
            (b, offset) = Util.GetNextByte(data, offset);
            Portamento = b.IsBitSet(7);  // use the byte extensions defined in Util.cs
	        PortamentoSpeed = (byte)(b & 0x3f); // 0b00111111

            // mode and "pic mode" - S20
	        (b, offset) = Util.GetNextByte(data, offset);
	        SMode = b.IsBitSet(2) ? SourceMode.Full : SourceMode.Twin;

            byte picModeValue = (byte)(b & 0x03);
	        switch (picModeValue) 
            {
	        case 0:
		        PMode = PicMode.S1;
                break;
	        case 1:
		        PMode = PicMode.S2;
                break;
	        case 2:
		        PMode = PicMode.Both;
                break;
	        default:
		        PMode = PicMode.Both;
                break;
            }

            // Could have handled the source settings as part of the source data,
            // but the portamento and mode settings would have to be special cased.
            // So just copy everything past that, and use it as the source data.

            //Console.WriteLine($"Processed common settings of {offset} bytes. Data length = {data.Length} bytes.");

            // S1 and S2 data are interleaved in S21 ... S468.
            
            int dataLength = data.Length - (offset + FormantLevelCount + 1 + 2);
            //Console.WriteLine(String.Format("dataLength = {0}", dataLength));
            byte[] sourceData = new byte[dataLength];
            //Console.WriteLine(String.Format("About to copy {0} bytes from data at {1} to sourceData at {2}", dataLength, offset, 0));
            Array.Copy(data, offset, sourceData, 0, dataLength);

            // Separate S1 and S2 data. Even bytes are S1, odd bytes are S2.
            // Note that this kind of assumes that the original data length is even.
            byte[] s1d = new byte[dataLength / 2];
            byte[] s2d = new byte[dataLength / 2];
            for (int src = 0, dst = 0; src < dataLength; src += 2, dst++)
            {
                s1d[dst] = sourceData[src];
                s2d[dst] = sourceData[src + 1];
            }

            //Console.WriteLine(String.Format("Source 1 data ({0} bytes):", s1d.Length));
            //Console.WriteLine(Util.HexDump(s1d));
            Source1 = new Source(s1d, 1);

            //Console.WriteLine(String.Format("Source 2 data ({0} bytes):", s2d.Length));
            //Console.WriteLine(Util.HexDump(s2d));
            Source2 = new Source(s2d, 2);

            offset = 468;

            // LFO (S469 ... S472)
    	    (b, offset) = Util.GetNextByte(data, offset);
            LFO.Shape = (LFOShape)b;

    	    (b, offset) = Util.GetNextByte(data, offset);
            LFO.Speed = b;

            (b, offset) = Util.GetNextByte(data, offset);
            LFO.Delay = b;

    	    (b, offset) = Util.GetNextByte(data, offset);
            LFO.Trend = b;

            // Keyscaling (S473 ... S478)
    	    (b, offset) = Util.GetNextByte(data, offset);
            s1s.KeyScaling.Right = b.ToSignedByte();

    	    (b, offset) = Util.GetNextByte(data, offset);
            s2s.KeyScaling.Right = b.ToSignedByte();

    	    (b, offset) = Util.GetNextByte(data, offset);
            s1s.KeyScaling.Left = b.ToSignedByte();

    	    (b, offset) = Util.GetNextByte(data, offset);
            s2s.KeyScaling.Left = b.ToSignedByte();

    	    (b, offset) = Util.GetNextByte(data, offset);
            s1s.KeyScaling.Breakpoint = b;

    	    (b, offset) = Util.GetNextByte(data, offset);
            s2s.KeyScaling.Breakpoint = b;

            Source1Settings = s1s;
            Source2Settings = s2s;

            //Console.WriteLine($"S1 keyscaling = {Source1Settings.KeyScaling}");
            //Console.WriteLine($"S2 keyscaling = {Source2Settings.KeyScaling}");

            // DFT (S479 ... S489)
            FormantLevels = new int[FormantLevelCount];
            for (int i = 0; i < FormantLevelCount; i++)
            {
        	    (b, offset) = Util.GetNextByte(data, offset);
                if (i == 0)
                {
                    IsFormantOn = b.IsBitSet(7);
                    b.UnsetBit(7);  // in the first byte, the low seven bits have the level
                }
                FormantLevels[i] = b;
            }

            // S490 is unused (should be zero, but whatever), so eat it
        	(b, offset) = Util.GetNextByte(data, offset);
            Filler = b;

            // Checksum (S491 ... S492)
        	(b, offset) = Util.GetNextByte(data, offset);
            byte checksumLow = b;
        	(b, offset) = Util.GetNextByte(data, offset);
            byte checksumHigh = b;
        }

        private string CollectName(byte[] data)
        {
            // Brute-forcing the name in: S1 ... S8
            byte[] bytes = { data[0], data[1], data[2], data[3], data[4], data[5], data[6], data[7] };
        	string name = Encoding.ASCII.GetString(bytes);
            return name;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append($"*SINGLE BASIC*          {Name}\n\n");
            builder.Append($"-----|  S1  |  S2  |   NAME={Name}\n");
            builder.Append($"BAL  |      {Balance}      |   MODE={SMode}\n");
            builder.Append($"DELAY|  {Source1Settings.Delay,2}  |  {Source2Settings.Delay,2}   |    VOL ={Volume}\n");
            builder.Append($"PEDAL|{Source1Settings.PedalAssign}|{Source2Settings.PedalAssign}|   POR ={Portamento}--SPD={PortamentoSpeed}\n");
            builder.Append($"P DEP| {Source1Settings.PedalDepth,3}  | {Source2Settings.PedalDepth,3}  |\n");
            builder.Append($"WHEEL|{Source1Settings.WheelAssign}|{Source2Settings.WheelAssign}|\n");
            builder.Append($"W DEP| {Source1Settings.WheelDepth,2} | {Source2Settings.WheelDepth,2}  |\n\n");
            builder.Append($"{Source1Settings.KeyScaling}\n");
            builder.Append($"{Source2Settings.KeyScaling}\n");

            builder.Append($"\n{Source1}\n{Source2}\n");

            StringBuilder formantStringBuilder = new StringBuilder();
            for (int i = 0; i < FormantLevelCount; i++)
            {
                formantStringBuilder.Append($"C{i - 1} ");
            }
            formantStringBuilder.Append("\n");
            for (int i = 0; i < FormantLevelCount; i++)
            {
                formantStringBuilder.Append($"{FormantLevels[i],3}");
            }
            formantStringBuilder.Append("\n");
            string formantString = formantStringBuilder.ToString();
            string formantSetting = IsFormantOn ? "ON" : "--";

            builder.Append(LFO);

            builder.Append($"\n*DFT*={formantSetting}\n\n{formantString}\n\n");

            return builder.ToString();
        }

        public byte[] ToData()
        {
            var buf = new List<byte>();
            byte b = 0;
            byte lowNybble = 0;
            byte highNybble = 0;
            
            foreach (char ch in Name)
            {
                buf.Add(Convert.ToByte(ch));
            }

            buf.Add(Volume);
            buf.Add(Balance.ToByte());

            buf.Add(Source1Settings.Delay);
            buf.Add(Source2Settings.Delay);

            buf.Add(Source1Settings.PedalDepth.ToByte());
            buf.Add(Source2Settings.PedalDepth.ToByte());

            buf.Add(Source1Settings.WheelDepth.ToByte());
            buf.Add(Source2Settings.WheelDepth.ToByte());

            highNybble = (byte)Source1Settings.PedalAssign;
            lowNybble = (byte)Source1Settings.WheelAssign;
            b = Util.ByteFromNybbles(highNybble, lowNybble);
            buf.Add(b);

            highNybble = (byte)Source2Settings.PedalAssign;
            lowNybble = (byte)Source2Settings.WheelAssign;
            b = Util.ByteFromNybbles(highNybble, lowNybble);
            buf.Add(b);

            // portamento and p. speed - S19
            b = PortamentoSpeed;
            if (Portamento)
            {
                b.SetBit(7);
            }
            buf.Add(b);

            // mode and "pic mode" - S20
	        b = (byte)PMode;
            if (SMode == SourceMode.Full)
            {
                b.SetBit(2);
            }
            else 
            {
                b.UnsetBit(2);
            }
            buf.Add(b);

            byte[] s1d = Source1.ToData();
            byte[] s2d = Source2.ToData();
            Console.WriteLine($"S1 data = {s1d.Length} bytes, S2 data = {s2d.Length} bytes");

            // Interleave the two byte arrays:
            int dataLength = s1d.Length;
            List<byte> sd = new List<byte>();
            int index = 0;
            while (index < dataLength)
            {
                sd.Add(s1d[index]);
                sd.Add(s2d[index]);
                index++;
            }
            buf.AddRange(sd);

            buf.AddRange(LFO.ToData());

            buf.Add(Source1Settings.KeyScaling.Right.ToByte());
            buf.Add(Source2Settings.KeyScaling.Right.ToByte());
            buf.Add(Source1Settings.KeyScaling.Left.ToByte());
            buf.Add(Source2Settings.KeyScaling.Left.ToByte());
            buf.Add(Source1Settings.KeyScaling.Breakpoint);
            buf.Add(Source2Settings.KeyScaling.Breakpoint);

            for (int i = 0; i < FormantLevelCount; i++)
            {
                buf.Add((byte)FormantLevels[i]);
            }

            buf.Add(Filler);

            int count = buf.Count;
            int checksum = ComputeChecksum(buf.GetRange(0, count).ToArray());
            buf.Add((byte)(checksum & 0xff));
            buf.Add((byte)((((uint)checksum) >> 8) & 0xFF));

            return buf.ToArray();
        }

        int ComputeChecksum(byte[] data)
        {
            int sum = 0;
            for (int i = 0; i < data.Length; i += 2)
            {
                sum += (((data[i + 1] & 0xFF) << 8) | (data[i] & 0xFF));
            }
                    
            sum = sum & 0xffff;
            sum = (0x5a3c - sum) & 0xffff;

            return sum;
        }
    }
}