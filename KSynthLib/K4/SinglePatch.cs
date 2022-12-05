using System;
using System.Text;
using System.Collections.Generic;

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
    public class SinglePatch : IPatch, ISystemExclusiveData
    {
        /// <value>System Exclusive data length.</value>
        public const int DataSize = 131;

        private PatchName _patchName;

        public Level Volume;
        public EffectNumber Effect; // 1~32 (on K4)
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

        public PitchBendRange PitchBendRange;

        public WheelAssignType WheelAssign; // 0/VIB, 1/LFO, 2/DCF
        public Depth WheelDepth;
        public AutoBendSettings AutoBend;  // same as portamento?
        public LFOSettings LFO;
        public VibratoSettings Vibrato;
        public Depth PressureFreq;

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
            this.Volume = new Level(99);
            this.Effect = new EffectNumber(1);
            this.Submix = SubmixType.A;

            this._patchName = new PatchName("NewSound");

            SourceMode = SourceMode.Normal;
            PolyphonyMode = PolyphonyMode.Poly1;
            AM12 = false;
            AM34 = false;
            SourceMutes = new bool[] { false, false, false, false };
            PitchBendRange = new PitchBendRange(2);
            WheelAssign = WheelAssignType.Vibrato;
            WheelDepth = new Depth(0);
            Vibrato = new VibratoSettings();
            LFO = new LFOSettings();
            AutoBend = new AutoBendSettings();
            PressureFreq = new Depth();

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
            var offset = 0;
            byte b = 0;  // will be reused when getting the next byte

            this._patchName = new PatchName(data, offset);
            offset += 10;  // name is S00 to S09
            //Console.Error.WriteLine(this.Name);

            (b, offset) = Util.GetNextByte(data, offset);
            this.Volume = new Level(b & 0x7f);

            // effect = s11 bits 0...4
            (b, offset) = Util.GetNextByte(data, offset);
            this.Effect = new EffectNumber(b & 0x1f);

            // output select = s12 bits 0...2
            (b, offset) = Util.GetNextByte(data, offset);
            int outputNameIndex = (int)(b & 0x07); // 0b00000111
            this.Submix = (SubmixType)outputNameIndex;

            // source mode = s13 bits 0...1
            (b, offset) = Util.GetNextByte(data, offset);
            this.SourceMode = (SourceMode)(b & 0x03);
            this.PolyphonyMode = (PolyphonyMode)((b >> 2) & 0x03);
            this.AM12 = ((b >> 4) & 0x01) == 1;
            this.AM34 = ((b >> 5) & 0x01) == 1;

            (b, offset) = Util.GetNextByte(data, offset);
            // the source mute bits are in s14:
            // S1 = b0, S2 = b1, S3 = b2, S4 = b3
            // The K4 MIDI spec says 0/mute, 1/not mute,
            // so we flip it to make this value actually mean muted.
            for (int i = 0; i < SourceCount; i++)
            {
                if (b.IsBitSet(i))
                {
                    SourceMutes[i] = false;
                }
                else
                {
                    SourceMutes[i] = true;
                }
            }

            // Collect the bytes that make up the vibrato settings,
            // to construct the object later from them.
            var vibratoBytes = new List<byte>();
            vibratoBytes.Add(b);

            (b, offset) = Util.GetNextByte(data, offset);
            // Pitch bend = s15 bits 0...3
            this.PitchBendRange = new PitchBendRange(b);
            // Wheel assign = s15 bits 4...5
            this.WheelAssign = (WheelAssignType)((b >> 4) & 0x03);

            (b, offset) = Util.GetNextByte(data, offset);
            // Vibrato speed = s16 bits 0...6
            vibratoBytes.Add(b);

            // Wheel depth = s17 bits 0...6
            (b, offset) = Util.GetNextByte(data, offset);
            this.WheelDepth = new Depth(b);  // constructor adjusts 0~100 to ±50

            // Construct the auto bend settings from collected bytes (s18...s21)
            var autoBendBytes = new List<byte>();
            (b, offset) = Util.GetNextByte(data, offset);
            autoBendBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            autoBendBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            autoBendBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            autoBendBytes.Add(b);
            this.AutoBend = new AutoBendSettings(autoBendBytes.ToArray());

            // s22 = vib prs>vib
            (b, offset) = Util.GetNextByte(data, offset);
            vibratoBytes.Add(b);

            // s23 = vibrato depth
            (b, offset) = Util.GetNextByte(data, offset);
            vibratoBytes.Add(b);

            // Now we have all the bytes for the vibrato settings
            this.Vibrato = new VibratoSettings(vibratoBytes);

            var lfoBytes = new List<byte>();
            (b, offset) = Util.GetNextByte(data, offset);
            lfoBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            lfoBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            lfoBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            lfoBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            lfoBytes.Add(b);
            LFO = new LFOSettings(lfoBytes);

            (b, offset) = Util.GetNextByte(data, offset);
            this.PressureFreq = new Depth(b); // constructor adjusts 0~100 to ±50

            int totalSourceDataSize = Source.DataSize * SourceCount;
            byte[] sourceData = new byte[totalSourceDataSize];
            Array.Copy(data, offset, sourceData, 0, totalSourceDataSize);
            List<byte> allSourceData = new List<byte>(sourceData);
            List<byte> source1Data = Util.EveryNthElement(allSourceData, 4, 0);
            List<byte> source2Data = Util.EveryNthElement(allSourceData, 4, 1);
            List<byte> source3Data = Util.EveryNthElement(allSourceData, 4, 2);
            List<byte> source4Data = Util.EveryNthElement(allSourceData, 4, 3);

            Sources = new Source[SourceCount];
            Sources[0] = new Source(source1Data.ToArray());
            Sources[1] = new Source(source2Data.ToArray());
            Sources[2] = new Source(source3Data.ToArray());
            Sources[3] = new Source(source4Data.ToArray());

            offset += totalSourceDataSize;

            int totalAmpDataSize = Amplifier.DataSize * SourceCount;
            var ampData = new byte[totalAmpDataSize];
            Array.Copy(data, offset, ampData, 0, totalAmpDataSize);
            List<byte> allAmpData = new List<byte>(ampData);
            List<byte> amp1Data = Util.EveryNthElement(allAmpData, 4, 0);
            List<byte> amp2Data = Util.EveryNthElement(allAmpData, 4, 1);
            List<byte> amp3Data = Util.EveryNthElement(allAmpData, 4, 2);
            List<byte> amp4Data = Util.EveryNthElement(allAmpData, 4, 3);

            Amplifiers = new Amplifier[SourceCount];
            Amplifiers[0] = new Amplifier(amp1Data.ToArray());
            Amplifiers[1] = new Amplifier(amp2Data.ToArray());
            Amplifiers[2] = new Amplifier(amp3Data.ToArray());
            Amplifiers[3] = new Amplifier(amp4Data.ToArray());

            offset += totalAmpDataSize;

            // DCF
            int totalFilterDataSize = Filter.DataSize * 2;
            byte[] filterData = new byte[totalFilterDataSize];
            Array.Copy(data, offset, filterData, 0, totalFilterDataSize);
            List<byte> allFilterData = new List<byte>(filterData);
            List<byte> filter1Data = Util.EveryNthElement(allFilterData, 2, 0);
            List<byte> filter2Data = Util.EveryNthElement(allFilterData, 2, 1);
            Filter1 = new Filter(filter1Data.ToArray());
            Filter2 = new Filter(filter2Data.ToArray());

            offset += totalFilterDataSize;

            (b, offset) = Util.GetNextByte(data, offset);
            // "Check sum value (s130) is the sum of the A5H and s0 ~ s129".
            //this.Checksum = b; // store the checksum as we got it from SysEx
        }

        /// <summary>
        /// Generates a printable representation of this patch.
        /// </summary>
        /// <returns>
        /// String with patch parameter values.
        /// </returns>
        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append($"{this.Name}\n");
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

            var sourceString = new StringBuilder();
            var ampString = new StringBuilder();
            for (var i = 0; i < SourceCount; i++)
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
        /// A list of bytes containing the patch data.
        /// </returns>
        /// <remarks>
        /// The data does not include the checksum.
        /// </remarks>
        private List<byte> CollectData()
        {
            var data = new List<byte>();

            data.AddRange(this._patchName.GetSystemExclusiveData());

            data.Add(Volume.ToByte());
            data.Add(Effect.ToByte());
            data.Add((byte)(Submix));  // A...H = 0...7

            // s13 combines source mode, poly mode, and source AM into one byte.
            // Construct a bit string, then convert it to byte.
            var b13 = new StringBuilder("00");
            b13.Append(AM34 ? "1" : "0");
            b13.Append(AM12 ? "1" : "0");
            b13.Append(Convert.ToString((byte)PolyphonyMode, 2).PadLeft(2, '0'));
            b13.Append(Convert.ToString((byte)SourceMode, 2).PadLeft(2, '0'));
            data.Add(Convert.ToByte(b13.ToString(), 2));

            // s14 combines vibrato shape and source mutes into one byte.
            var b14 = new StringBuilder("00");
            b14.Append(Convert.ToString((byte)Vibrato.Shape, 2).PadLeft(2, '0'));

            // Our "SourceMutes" is true if the source is muted,
            // or false if it is not. The SysEx wants "0/mute, 1/not mute".
            b14.Append(SourceMutes[3] ? "1" : "0");
            b14.Append(SourceMutes[2] ? "1" : "0");
            b14.Append(SourceMutes[1] ? "1" : "0");
            b14.Append(SourceMutes[0] ? "1" : "0");
            data.Add(Convert.ToByte(b14.ToString(), 2));

            // s15 combines pitch bend and wheel assign into one byte.
            var b15 = new StringBuilder("");
            b15.Append(Convert.ToString((byte)WheelAssign, 2).PadLeft(4, '0'));
            b15.Append(Convert.ToString(PitchBendRange.ToByte(), 2).PadLeft(4, '0'));
            data.Add(Convert.ToByte(b15.ToString(), 2));

            data.Add(Vibrato.Speed.ToByte());
            data.Add(WheelDepth.ToByte());

            data.AddRange(AutoBend.GetSystemExclusiveData());

            data.Add(Vibrato.Pressure.ToByte());
            data.Add(Vibrato.Depth.ToByte());

            data.AddRange(LFO.GetSystemExclusiveData());

            data.Add(PressureFreq.ToByte());

            // Collect the source data lists into one list, then interleave.
            var allSourceData = new List<List<byte>>();
            foreach (var source in Sources)
            {
                allSourceData.Add(new List<byte>(source.GetSystemExclusiveData()));
            }
            data.AddRange(Util.InterleaveBytes(allSourceData));

            // Similarly for amp data:
            var allAmplifierData = new List<List<byte>>();
            foreach (var amplifier in Amplifiers)
            {
                allAmplifierData.Add(amplifier.GetSystemExclusiveData());
            }
            data.AddRange(Util.InterleaveBytes(allAmplifierData));

            // And finally for filters:
            data.AddRange(
                Util.InterleaveBytes(
                    new List<byte>(Filter1.GetSystemExclusiveData()),
                    new List<byte>(Filter2.GetSystemExclusiveData())
                )
            );

            return data;
        }

        private string SourceMuteString
        {
            get
            {
                var builder = new StringBuilder();
                builder.Append(SourceMutes[0] ? "1" : "-");
                builder.Append(SourceMutes[1] ? "2" : "-");
                builder.Append(SourceMutes[2] ? "3" : "-");
                builder.Append(SourceMutes[3] ? "4" : "-");
                return builder.ToString();
            }
        }

        //
        // Implementation of ISystemExclusiveData interface
        //

        public List<byte> GetSystemExclusiveData()
        {
            var data = new List<byte>();

            data.AddRange(this.CollectData());
            data.Add(this.Checksum);

            return data;
        }

        //
        // Implementation of the IPatch interface
        //

        public string Name
        {
            get
            {
                return this._patchName.Value;
            }

            set
            {
                this._patchName = new PatchName(value);
            }
        }

        public byte Checksum
        {
            get
            {
                List<byte> data = this.CollectData();
                int sum = 0;
                foreach (byte b in data)
                {
                    sum = (sum + b) & 0xff;
                }
                sum += 0xA5;
                return (byte)(sum & 0x7f);
            }
        }
    }
}
