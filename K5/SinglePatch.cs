using System;
using System.Text;
using System.Linq;
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

    public enum SourceMode
    {
        Twin,
        Full
    }

    public enum PicMode
    {
        S1,
        S2,
        Both
    }

    public struct KeyScaling
    {
        public sbyte Right;
        public sbyte Left;
        public byte Breakpoint;

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("*KS CURVE*\n\n");
            builder.Append(String.Format("LEFT={0,3}    B.POINT={1,3}    RIGHT={2,3}\n", Left, Breakpoint, Right));
            builder.Append("\n\n");

            return builder.ToString();
        }
    }

    public struct SourceSettings
    {
        public byte Delay; // 0~31
        public sbyte PedalDepth; // 0~±31
        public sbyte WheelDepth; // 0~±31
        public ModulationAssign PedalAssign;
        public ModulationAssign WheelAssign;
        public KeyScaling KeyScaling;

        public override string ToString()
        {
            return $"Delay = {Delay}, pedal depth = {PedalDepth}, wheel depth = {WheelDepth}, pedal assign = {PedalAssign}, wheel assign = {WheelAssign}";
        }
    }

    public enum LFOShape  // 1 ~ 6
    {
        Triangle,
        InverseTriangle,
        Square,
        InverseSquare,
        Sawtooth,
        InverseSawtooth
    }

    public class LFO
    {
        public LFOShape Shape;
        public byte Speed;  // 0~99
        public byte Delay;  // 0~31
        public byte Trend;  // 0~31

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("*LFO*\n\n");
            builder.Append(String.Format(" SHAPE= {0}\n SPEED= {1,2}\n DELAY= {2,2}\n TREND= {3,2}\n\n\n", Shape, Speed, Delay, Trend));

            return builder.ToString();
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();

            data.Add(Convert.ToByte(Shape));
            data.Add(Speed);
            data.Add(Delay);
            data.Add(Trend);

            return data.ToArray();
        }
    }
    
    public class SinglePatch: Patch
    {
        const int FormantLevelCount = 11;

        public string Name;
        public byte Volume;  // 0~63
        public sbyte Balance; // 0~±31
        public SourceSettings Source1Settings;
        public SourceSettings Source2Settings;
        public bool Portamento;
        public byte PortamentoSpeed; // 0~63
        public SourceMode SMode;
        public PicMode PMode;
        public Source Source1;
        public Source Source2;
        public LFO LFO;
        public bool IsFormantOn;  // true Digital Formant filter is on
        
        public int[] FormantLevels;  // levels for bands C-1 ... C9 (0 ~ 63)
        // Used int[] and not byte[] to get the correct JSON serialization

        public byte Filler;  // retain the byte before the checksum (should be zero but not guaranteed)

        public SinglePatch()
        {
            Source1Settings = new SourceSettings();
            Source2Settings = new SourceSettings();
            Source1 = new Source();
            Source2 = new Source();
            LFO = new LFO();
            FormantLevels = new int[FormantLevelCount];
        }

        public SinglePatch(byte[] data)
        {
            int offset = 0;
            byte b = 0;  // will be reused when getting the next byte

            Name = GetName(data, offset);

            offset += 8;
            (Volume, offset) = Util.GetNextByte(data, offset);  // S9

            (b, offset) = Util.GetNextByte(data, offset);
            Balance = b.ToSignedByte(); // S10

            // Source 1 and Source 2 settings.
        	// Note that the pedal assign and the wheel assign for one source are in the same byte.
            SourceSettings s1s;
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

            SourceSettings s2s;
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

	        // portamento and p. speed - S19
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

            //System.Console.WriteLine($"Processed common settings of {offset} bytes. Data length = {data.Length} bytes.");

            int dataLength = data.Length - (offset + FormantLevelCount + 1 + 2);
            //System.Console.WriteLine(String.Format("dataLength = {0}", dataLength));
            byte[] sourceData = new byte[dataLength];
            //System.Console.WriteLine(String.Format("About to copy {0} bytes from data at {1} to sourceData at {2}", dataLength, offset, 0));
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

            //System.Console.WriteLine(String.Format("Source 1 data ({0} bytes):", s1d.Length));
            //System.Console.WriteLine(Util.HexDump(s1d));
            Source1 = new Source(s1d, 1);

            //System.Console.WriteLine(String.Format("Source 2 data ({0} bytes):", s2d.Length));
            //System.Console.WriteLine(Util.HexDump(s2d));
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

            System.Console.WriteLine(String.Format("S1 keyscaling = {0}", Source1Settings.KeyScaling));
            System.Console.WriteLine(String.Format("S2 keyscaling = {0}", Source2Settings.KeyScaling));

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

        private string GetName(byte[] data, int offset)
        {
            // Brute-forcing the name in: S1 ... S8
            byte[] bytes = { data[0], data[1], data[2], data[3], data[4], data[5], data[6], data[7] };
        	string name = Encoding.ASCII.GetString(bytes);
            return name;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(String.Format("*SINGLE BASIC*          {0}\n\n", Name));
            builder.Append(String.Format("-----|  S1  |  S2  |   NAME={0}\n", Name));
            builder.Append(String.Format("BAL  |      {0}      |   MODE={1}\n", Balance, SMode));
            builder.Append(String.Format("DELAY|  {0,2}  |  {1,2}   |    VOL ={2}\n", Source1Settings.Delay, Source2Settings.Delay, Volume));
            builder.Append(String.Format("PEDAL|{0}|{1}|   POR ={2}--SPD={3}\n", Source1Settings.PedalAssign, Source2Settings.PedalAssign, Portamento, PortamentoSpeed));
            builder.Append(String.Format("P DEP| {0,3}  | {1,3}  |\n", Source1Settings.PedalDepth, Source2Settings.PedalDepth));;
            builder.Append(String.Format("WHEEL|{0}|{1}|\n", Source1Settings.WheelAssign, Source2Settings.WheelAssign));
            builder.Append(String.Format("W DEP| {0,2} | {1,2}  |\n", Source1Settings.WheelDepth, Source2Settings.WheelDepth));
            builder.Append(Source1Settings.KeyScaling.ToString());
            builder.Append(Source2Settings.KeyScaling.ToString());

            builder.Append("\n");
            builder.Append(Source1.ToString());
            builder.Append("\n");
            builder.Append(Source2.ToString());
            builder.Append("\n");

            StringBuilder formantStringBuilder = new StringBuilder();
            for (int i = 0; i < FormantLevelCount; i++)
            {
                formantStringBuilder.Append(String.Format("C{0} ", i - 1));
            }
            formantStringBuilder.Append("\n");
            for (int i = 0; i < FormantLevelCount; i++)
            {
                formantStringBuilder.Append(String.Format("{0,3}", FormantLevels[i]));
            }
            formantStringBuilder.Append("\n");

            builder.Append(LFO.ToString());

            builder.Append(String.Format("\n*DFT*={0}\n\n{1}\n\n", 
                    IsFormantOn ? "ON" : "--", formantStringBuilder.ToString()));

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
            System.Console.WriteLine(String.Format("S1 data = {0} bytes, S2 data = {1} bytes", s1d.Length, s2d.Length));

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