using System;
using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K5000
{
    public class SinglePatch : Patch
    {
        public override byte Checksum
        {
            get
            {
                // BANK A, D, E, F: check sum = [(common sum) + (source1 sum) [ + (source2~8 sum)] + 0xa5) & 0x7f
                byte total = 0;

                // For each source, compute the sum of source data and add it to the total:
                for (int i = 0; i < SingleCommon.SourceCount; i++)
                {
                    byte[] sourceData = Sources[i].ToData();
                    byte sourceSum = 0;
                    foreach (byte b in sourceData)
                    {
                        sourceSum += b;
                    }
                    total += sourceSum;
                }

                total += 0xA5;

                return (byte)(total & 0x7f);
            }

            set
            {
                _checksum = value;
            }
        }

        public SingleCommonSettings SingleCommon;

        public Source[] Sources;

        public int DataSize
        {
            get
            {
                int sourcesSize = Sources.Length * Source.DataSize;
                return CommonSettings.DataSize + SingleCommonSettings.DataSize + sourcesSize;
            }
        }

        /// <summary>Constructs a single patch with default settings.</summary>
        public SinglePatch() : base()
        {
            SingleCommon = new SingleCommonSettings();
            SingleCommon.SourceCount = 1;

            Sources = new Source[SingleCommon.SourceCount];
            for (int i = 0; i < SingleCommon.SourceCount; i++)
            {
                Sources[i] = new Source();
            }
        }

        /// <summary>Constructs a single patch from System Exclusive data.</summary>
        public SinglePatch(byte[] data) : base()
        {
            int offset = 0;
            byte b;
            (b, offset) = Util.GetNextByte(data, offset);

            // Ingest the checksum
            _checksum = b;

            // Create single patch common settings from binary data:
            byte[] singleCommonData = new byte[SingleCommonSettings.DataSize];
            Buffer.BlockCopy(data, offset, singleCommonData, 0, SingleCommonSettings.DataSize);
            SingleCommon = new SingleCommonSettings(singleCommonData);

            offset += SingleCommonSettings.DataSize;

            // Create each of the sources, as many as indicated by the common data:
            Sources = new Source[SingleCommon.SourceCount];
            for (int i = 0; i < SingleCommon.SourceCount; i++)
            {
                byte[] sourceData = new byte[Source.DataSize];

                // BlockCopy argument list: Array src, int srcOffset, Array dst, int dstOffset, int count
                Buffer.BlockCopy(data, offset, sourceData, 0, Source.DataSize);
                string hex = Util.HexDump(sourceData);
                //Console.Error.WriteLine($"Source {i + 1} data:\n{hex}");
                Source source = new Source(sourceData);
                Sources[i] = source;
                offset += Source.DataSize;
                //Console.Error.WriteLine($"{offset:X6} parsed {Source.DataSize} bytes of source data");
            }

            for (int i = 0; i < SingleCommon.SourceCount; i++)
            {
                Source source = Sources[i];
                if (source.IsAdditive)  // ADD source, so include wave kit size in calculation
                {
                    byte[] additiveData = new byte[AdditiveKit.DataSize];
                    //Console.Error.WriteLine(string.Format("About to copy from data at offset {0:X4} to start of new buffer", offset));
                    Buffer.BlockCopy(data, offset, additiveData, 0, AdditiveKit.DataSize);
                    string hex = Util.HexDump(additiveData);
                    //Console.Error.WriteLine($"{offset:X6} Source {i + 1} ADD data:\n{hex}");
                    source.ADD = new AdditiveKit(additiveData);
                    offset += AdditiveKit.DataSize;
                    //Console.Error.WriteLine($"{offset:X6} parsed {AdditiveKit.DataSize} bytes of ADD data");
                }
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(SingleCommon);
            builder.Append("SOURCES:\n");
            for (int i = 0; i < SingleCommon.SourceCount; i++)
            {
                builder.Append($"S{i + 1}:\n{Sources[i]}\n");
            }
            builder.Append("\n");
            return builder.ToString();
        }

        protected override byte[] CollectData()
        {
            List<byte> data = new List<byte>();

            data.AddRange(SingleCommon.ToData());

            for (int i = 0; i < SingleCommon.SourceCount; i++)
            {
                byte[] sourceData = Sources[i].ToData();
                data.AddRange(sourceData);
            }

            return data.ToArray();
        }
    }
}
