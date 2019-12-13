using System;
using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K5000
{
    public class SinglePatch : Patch
    {
        // Common is inherited from Patch

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

        // Initialize a single patch with default settings
        public SinglePatch() : base()
        {
            //Common = new CommonSettings();  // initialized by superclass constructor

            SingleCommon = new SingleCommonSettings();
            SingleCommon.NumSources = 1;

            Sources = new Source[SingleCommon.NumSources];
            for (int i = 0; i < SingleCommon.NumSources; i++)
            {
                Sources[i] = new Source();
            }
        }

        public SinglePatch(byte[] data) : base(data)
        {
            int offset = CommonSettings.DataSize;  // skip the common data parsed by superclass

            byte[] singleCommonData = new byte[SingleCommonSettings.DataSize];
            Buffer.BlockCopy(data, offset, singleCommonData, 0, SingleCommonSettings.DataSize);
            SingleCommon = new SingleCommonSettings(singleCommonData);

            offset += SingleCommonSettings.DataSize;

            Sources = new Source[SingleCommon.NumSources];
            for (int i = 0; i < SingleCommon.NumSources; i++)
            {
                byte[] sourceData = new byte[Source.DataSize];

                // BlockCopy argument list: Array src, int srcOffset, Array dst, int dstOffset, int count
                Buffer.BlockCopy(data, offset, sourceData, 0, Source.DataSize);
                Console.WriteLine(String.Format("Source {0} data:\n{1}", i + 1, Util.HexDump(sourceData)));
                Source source = new Source(sourceData);
                Sources[i] = source;
                offset += Source.DataSize;
                Console.WriteLine(String.Format("{0:X6} parsed {1} bytes of source data", offset, Source.DataSize));
            }

            for (int i = 0; i < SingleCommon.NumSources; i++)
            {
                Source source = Sources[i];
                if (source.IsAdditive)  // ADD source, so include wave kit size in calculation
                {
                    byte[] additiveData = new byte[AdditiveKit.DataSize];
                    //Console.WriteLine(String.Format("About to copy from data at offset {0:X4} to start of new buffer", offset));
                    Buffer.BlockCopy(data, offset, additiveData, 0, AdditiveKit.DataSize);
                    Console.WriteLine(String.Format("{0:X6} Source {1} ADD data:\n{2}", offset, i + 1, Util.HexDump(additiveData)));
                    source.ADD = new AdditiveKit(additiveData);
                    offset += AdditiveKit.DataSize;
                    Console.WriteLine(String.Format("{0:X6} parsed {1} bytes of ADD data", offset, AdditiveKit.DataSize));
                }
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(Common.ToString());
            builder.Append(SingleCommon.ToString());
            builder.Append("SOURCES:\n");
            for (int i = 0; i < SingleCommon.NumSources; i++)
            {
                builder.Append(String.Format("S{0}:\n{1}\n", i + 1, Sources[i].ToString()));
            }
            builder.Append("\n");
            return builder.ToString();
        }

        protected override byte[] CollectData()
        {
            List<byte> data = new List<byte>();

            data.AddRange(Common.ToData());
            data.AddRange(SingleCommon.ToData());

            for (int i = 0; i < SingleCommon.NumSources; i++)
            {
                byte[] sourceData = Sources[i].ToData();
                data.AddRange(sourceData);
            }

            return data.ToArray();
        }

        // Convert this single patch into SysEx data.
        // "BANK A, D, E, F: (check sum) + (COMMON) + (SOURCE)*(2~8)" (probably should be ~2*6?)

        protected override byte ComputeChecksum(byte[] data)
        {
            // BANK A, D, E, F: check sum = [(common sum) + (source1 sum) [ + (source2~8 sum)] + 0xa5) & 0x7f
            byte total = 0;

            // First, compute the sum of common data and add it to the total:
            byte[] commonData = Common.ToData();
            byte commonSum = 0;
            foreach (byte b in commonData)
            {
                commonSum += b;
            }
            total += commonSum;

            // For each source, compute the sum of source data and add it to the total:
            for (int i = 0; i < SingleCommon.NumSources; i++)
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
    }
}
