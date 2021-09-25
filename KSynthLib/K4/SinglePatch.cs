using System;
using System.Text;
using System.Collections.Generic;
using System.IO;

using KSynthLib.Common;

namespace KSynthLib.K4
{
    public enum SourceMode
    {
        Normal,
        Twin,
        Double
    };

    public enum PolyphonyMode
    {
        Poly1,
        Poly2,
        Solo1,
        Solo2
    };

    public enum SubmixType
    {
        A,
        B,
        C,
        D,
        E,
        F,
        G,
        H
    }

    /// <summary>
    /// Represents a K4 single patch.
    /// </summary>
    public class SinglePatch : Patch
    {
        /// <value>System Exclusive data length.</value>
        public const int DataSize = 131;

        public LevelType Volume;
        public EffectNumberType Effect; // 1~32 (on K4)
        public SubmixType Submix;
        public SourceMode SourceMode;
        public PolyphonyMode PolyphonyMode;
        public bool AM12;
        public bool AM34;

        /// <summary>
        /// Source mute values: <c>true</c> if the source indicated by the index
        /// is muted, <c>false</c> otherwise.
        /// </summary>
        public bool[] SourceMutes;

        public PitchBendRangeType PitchBendRange;

        public WheelAssignType WheelAssign; // 0/VIB, 1/LFO, 2/DCF
        public DepthType WheelDepth;
        public AutoBendSettings AutoBend;  // same as portamento?
        public LFOSettings LFO;
        public VibratoSettings Vibrato;
        public DepthType PressureFreq;

        /// <value>The number of sources in a single patch.</value>
        public const int SourceCount = 4;

        public Source[] Sources;
        public Amplifier[] Amplifiers;
        public Filter Filter1;
        public Filter Filter2;

        /// <summary>
        /// Constructs a single patch from default values.
        /// </summary>
        public SinglePatch()
        {
            this.Volume = new LevelType(99);
            this.Effect = new EffectNumberType(1);
            this.Submix = SubmixType.A;

            this._name = "NewSound";

            SourceMode = SourceMode.Normal;
            PolyphonyMode = PolyphonyMode.Poly1;
            AM12 = false;
            AM34 = false;
            SourceMutes = new bool[] { false, false, false, false };
            PitchBendRange = new PitchBendRangeType(2);
            WheelAssign = WheelAssignType.Vibrato;
            WheelDepth = new DepthType(0);
            Vibrato = new VibratoSettings();
            LFO = new LFOSettings();
            AutoBend = new AutoBendSettings();
            PressureFreq = new DepthType();

            Sources = new Source[SourceCount];
            Amplifiers = new Amplifier[SourceCount];
            for (int i = 0; i < SourceCount; i++)
            {
                // Source mutes default to false (not muted)
                SourceMutes[i] = false;

                Sources[i] = new Source();
                Amplifiers[i] = new Amplifier();
            }

            Filter1 = new Filter();
            Filter2 = new Filter();
            Checksum = 0;
        }

        /// <summary>
        /// Constructs a single patch from binary System Exclusive data.
        /// </summary>
        /// <param name="data">System Exclusive data</param>
        /// <remarks>
        /// The no-argument constructor is used to initialize the data members.
        /// </remarks>
        public SinglePatch(byte[] data) : this()
        {
            List<byte> vibratoBytes = new List<byte>();
            List<byte> autoBendBytes = new List<byte>();
            List<byte> lfoBytes = new List<byte>();
            int totalSourceDataSize = Source.DataSize * SourceCount;
            byte[] sourceData = new byte[totalSourceDataSize];
            int totalAmpDataSize = Amplifier.DataSize * SourceCount;
            byte[] ampData = new byte[totalAmpDataSize];
            int totalFilterDataSize = Filter.DataSize * 2;
            byte[] filterData = new byte[totalFilterDataSize];

            byte b = 0;  // will be reused when getting the next byte

            // Process the SysEx data using a memory stream:
            using (MemoryStream mem = new MemoryStream(data))
            {
                // Read the patch name in s00...s09
                Console.Error.WriteLine($"{mem.Position}: name");
                byte[] nameBytes = new byte[Patch.NameLength];
                mem.Read(nameBytes, 0, Patch.NameLength);
                this._name = this.GetName(nameBytes);

                Console.Error.WriteLine($"{mem.Position}: volume");
                this.Volume = new LevelType(mem.ReadByte());

                // effect = s11 bits 0...4
                Console.Error.WriteLine($"{mem.Position}: effect");
                this.Effect = new EffectNumberType(mem.ReadByte());

                // output select = s12 bits 0...2
                Console.Error.WriteLine($"{mem.Position}: output");
                Submix = (SubmixType)(mem.ReadByte() & 0x07); // 0b00000111

                Console.Error.WriteLine($"{mem.Position}: source mode, polyphony mode, AM 1>2, AM 3>4");
                int v = mem.ReadByte();
                // source mode = s13 bits 0...1
                SourceMode = (SourceMode)(v & 0x03);
                PolyphonyMode = (PolyphonyMode)((v >> 2) & 0x03);
                AM12 = ((v >> 4) & 0x01) == 1;
                AM34 = ((v >> 5) & 0x01) == 1;

                Console.Error.WriteLine($"{mem.Position}: source mutes");
                b = (byte)mem.ReadByte();
                // the source mute bits are in s14:
                // S1 = b0, S2 = b1, S3 = b2, S4 = b3
                // The K4 MIDI spec says 0/mute, 1/not mute,
                // so we flip it to make this value actually mean muted.
                for (int i = 0; i < SourceCount; i++)
                {
                    SourceMutes[i] = !(b.IsBitSet(i));
                }

                // Save the first vibrato byte (the rest will come later)
                Console.Error.WriteLine($"{mem.Position}: vibrato shape");
                vibratoBytes.Add((byte)mem.ReadByte());

                Console.Error.WriteLine($"{mem.Position}: pitch bend, wheel assign");
                v = mem.ReadByte();
                // Pitch bend = s15 bits 0...3
                PitchBendRange = new PitchBendRangeType(v & 0b1111);
                // Wheel assign = s15 bits 4...5
                WheelAssign = (WheelAssignType)((v >> 4) & 0x03);

                // Vibrato speed = s16 bits 0...6
                Console.Error.WriteLine($"{mem.Position}: vibrato speed");
                vibratoBytes.Add((byte)mem.ReadByte());

                // Wheel depth = s17 bits 0...6
                Console.Error.WriteLine($"{mem.Position}: wheel depth");
                WheelDepth = new DepthType((byte)mem.ReadByte());  // constructor adjusts 0~100 to ±50

                // Construct the auto bend settings from collected bytes
                Console.Error.WriteLine($"{mem.Position}: auto bend");
                autoBendBytes.Add((byte)mem.ReadByte());
                autoBendBytes.Add((byte)mem.ReadByte());
                autoBendBytes.Add((byte)mem.ReadByte());
                autoBendBytes.Add((byte)mem.ReadByte());
                AutoBend = new AutoBendSettings(autoBendBytes.ToArray());

                Console.Error.WriteLine($"{mem.Position}: vibrato pressure depth");
                vibratoBytes.Add((byte)mem.ReadByte());

                Console.Error.WriteLine($"{mem.Position}: vibrato depth");
                vibratoBytes.Add((byte)mem.ReadByte());

                // Now we have all the bytes for the vibrato settings
                Vibrato = new VibratoSettings(vibratoBytes);

                Console.Error.WriteLine($"{mem.Position}: LFO shape");
                lfoBytes.Add((byte)mem.ReadByte());

                Console.Error.WriteLine($"{mem.Position}: LFO speed");
                lfoBytes.Add((byte)mem.ReadByte());

                Console.Error.WriteLine($"{mem.Position}: LFO delay");
                lfoBytes.Add((byte)mem.ReadByte());

                Console.Error.WriteLine($"{mem.Position}: LFO depth");
                lfoBytes.Add((byte)mem.ReadByte());

                Console.Error.WriteLine($"{mem.Position}: LFO pressure depth");
                lfoBytes.Add((byte)mem.ReadByte());

                this.LFO = new LFOSettings(lfoBytes);

                Console.Error.WriteLine($"{mem.Position}: pressure freq");
                this.PressureFreq = new DepthType((byte)mem.ReadByte()); // constructor adjusts 0~100 to ±50

                Console.Error.WriteLine($"{mem.Position}: sources");
                mem.Read(sourceData, 0, totalSourceDataSize);
                List<byte> sourceBytes = new List<byte>(sourceData);
                Sources = new Source[SourceCount]
                {
                    new Source(Util.EveryNthElement(sourceBytes, 4, 0).ToArray()),
                    new Source(Util.EveryNthElement(sourceBytes, 4, 1).ToArray()),
                    new Source(Util.EveryNthElement(sourceBytes, 4, 2).ToArray()),
                    new Source(Util.EveryNthElement(sourceBytes, 4, 3).ToArray())
                };

                Console.Error.WriteLine($"{mem.Position}: amplifiers");
                mem.Read(ampData, 0, totalAmpDataSize);
                List<byte> ampBytes = new List<byte>(ampData);
                Amplifiers = new Amplifier[SourceCount]
                {
                    new Amplifier(Util.EveryNthElement(ampBytes, 4, 0).ToArray()),
                    new Amplifier(Util.EveryNthElement(ampBytes, 4, 1).ToArray()),
                    new Amplifier(Util.EveryNthElement(ampBytes, 4, 2).ToArray()),
                    new Amplifier(Util.EveryNthElement(ampBytes, 4, 3).ToArray()),
                };

                Console.Error.WriteLine($"{mem.Position}: filters");
                mem.Read(filterData, 0, totalFilterDataSize);
                List<byte> filterBytes = new List<byte>(filterData);
                Filter1 = new Filter(Util.EveryNthElement(filterBytes, 2, 0).ToArray());
                Filter2 = new Filter(Util.EveryNthElement(filterBytes, 2, 1).ToArray());

                // "Check sum value (s130) is the sum of the A5H and s0 ~ s129".
                this.Checksum = (byte)mem.ReadByte(); // store the checksum as we got it from SysEx
            }
        }

        /// <summary>
        /// Generates a printable representation of this patch.
        /// </summary>
        /// <returns>
        /// String with patch parameter values.
        /// </returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append($"{Name}\n");
            builder.Append($"VOLUME     ={Volume:3}\nEFFECT PACH= {Effect:2}\nSUBMIX CH  =  {Submix}\n");

            builder.Append(string.Format("SOURCE MODE={0}\n", Enum.GetNames(typeof(SourceMode))[(int)SourceMode]));
            builder.Append(string.Format("AM 1>2     ={0}\nAM 3>4     ={1}\n", AM12 ? "ON" : "OFF", AM34 ? "ON" : "OFF"));
            builder.Append(string.Format("POLY MODE  ={0}\n", Enum.GetNames(typeof(PolyphonyMode))[(int)PolyphonyMode]));
            builder.Append(string.Format("BNDR RANGE = {0,2}\n", PitchBendRange));
            builder.Append(string.Format("PRESS>FREQ = {0,2}\n", PressureFreq));
            builder.Append(string.Format("WHEEL\nASSIGN     ={0}\nDEPTH      ={1,2}\n", Enum.GetNames(typeof(WheelAssignType))[(int)WheelAssign], WheelDepth));
            builder.Append($"AUTO BEND\n{AutoBend}\n");
            builder.Append($"Sources: {SourceMuteString}\n");
            builder.Append($"VIBRATO\n{Vibrato}\n");
            builder.Append($"LFO\n{LFO}\n");

            StringBuilder sourceString = new StringBuilder();
            StringBuilder ampString = new StringBuilder();
            for (int i = 0; i < SourceCount; i++)
            {
                sourceString.Append($"Source {i+1}:\n{Sources[i]}");
                ampString.Append($"DCA: {Amplifiers[i]}");
            }
            builder.Append(sourceString.ToString());
            builder.Append(ampString.ToString());

            builder.Append($"Filter 1: {Filter1}\n");
            builder.Append($"Filter 2: {Filter2}\n");
            return builder.ToString();
        }

        /// <summary>
        /// Collects the patch data for use in a System Exclusive message.
        /// </summary>
        /// <returns>
        /// Byte array with patch data.
        /// </returns>
        /// <remarks>
        /// The data does not include the checksum.
        /// </remarks>
        protected override byte[] CollectData()
        {
            List<byte> data = new List<byte>();

            //byte[] nameBytes = Encoding.ASCII.GetBytes(this.Name.PadRight(10));
            byte[] nameBytes = GetNameBytes(this.Name.PadRight(10));
            data.AddRange(nameBytes);

            data.Add(Volume.ToByte());
            data.Add(Effect.ToByte());
            data.Add((byte)(Submix));  // A...H = 0...7

            // s13 combines source mode, poly mode, and source AM into one byte.
            // Construct a bit string, then convert it to byte.
            StringBuilder b13 = new StringBuilder("00");
            b13.Append(AM34 ? "1" : "0");
            b13.Append(AM12 ? "1" : "0");
            b13.Append(Convert.ToString((byte)PolyphonyMode, 2).PadLeft(2, '0'));
            b13.Append(Convert.ToString((byte)SourceMode, 2).PadLeft(2, '0'));
            data.Add(Convert.ToByte(b13.ToString(), 2));

            // s14 combines vibrato shape and source mutes into one byte.
            StringBuilder b14 = new StringBuilder("00");
            b14.Append(Convert.ToString((byte)Vibrato.Shape, 2).PadLeft(2, '0'));

            // Our "SourceMutes" is true if the source is muted,
            // or false if it is not. The SysEx wants "0/mute, 1/not mute".
            b14.Append(SourceMutes[3] ? "1" : "0");
            b14.Append(SourceMutes[2] ? "1" : "0");
            b14.Append(SourceMutes[1] ? "1" : "0");
            b14.Append(SourceMutes[0] ? "1" : "0");
            data.Add(Convert.ToByte(b14.ToString(), 2));

            // s15 combines pitch bend and wheel assign into one byte.
            StringBuilder b15 = new StringBuilder("");
            b15.Append(Convert.ToString((byte)WheelAssign, 2).PadLeft(4, '0'));
            b15.Append(Convert.ToString(PitchBendRange.ToByte(), 2).PadLeft(4, '0'));
            data.Add(Convert.ToByte(b15.ToString(), 2));

            data.Add(Vibrato.Speed.ToByte());
            data.Add(WheelDepth.ToByte());

            data.AddRange(AutoBend.ToData());

            data.Add(Vibrato.Pressure.ToByte());
            data.Add(Vibrato.Depth.ToByte());

            data.AddRange(LFO.ToData());

            data.Add(PressureFreq.ToByte());

            // The source data are interleaved, with one byte from each first,
            // then the second, etc. That's why they are emitted in this slightly
            // inelegant way. The same applies for DCA and DCF data.

            byte[] source1Data = Sources[0].ToData();
            byte[] source2Data = Sources[1].ToData();
            byte[] source3Data = Sources[2].ToData();
            byte[] source4Data = Sources[3].ToData();

            for (int i = 0; i < Source.DataSize; i++)
            {
                data.Add(source1Data[i]);
                data.Add(source2Data[i]);
                data.Add(source3Data[i]);
                data.Add(source4Data[i]);
            }

            byte[] amp1Data = Amplifiers[0].ToData();
            byte[] amp2Data = Amplifiers[1].ToData();
            byte[] amp3Data = Amplifiers[2].ToData();
            byte[] amp4Data = Amplifiers[3].ToData();

            for (int i = 0; i < Amplifier.DataSize; i++)
            {
                data.Add(amp1Data[i]);
                data.Add(amp2Data[i]);
                data.Add(amp3Data[i]);
                data.Add(amp4Data[i]);
            }

            byte[] filter1Data = Filter1.ToData();
            byte[] filter2Data = Filter2.ToData();
            for (int i = 0; i < Filter.DataSize; i++)
            {
                data.Add(filter1Data[i]);
                data.Add(filter2Data[i]);
            }

            return data.ToArray();
        }

        private string SourceMuteString
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(SourceMutes[0] ? "1" : "-");
                builder.Append(SourceMutes[1] ? "2" : "-");
                builder.Append(SourceMutes[2] ? "3" : "-");
                builder.Append(SourceMutes[3] ? "4" : "-");
                return builder.ToString();
            }
        }
    }
}
