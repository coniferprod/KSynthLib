using System.Text;
using System.Collections.Generic;
using System.IO;

using KSynthLib.Common;

namespace KSynthLib.K5000
{
    /// <summary>
    /// Represents a K5000 multi patch (combi on K5000W/R).
    /// </summary>
    public class MultiPatch : IPatch, ISystemExclusiveData
    {
        public const int SectionCount = 4;

        public MultiCommon Common;
        public MultiSection[] Sections;

        /// <summary>
        /// Constructs a multi patch with default values.
        /// </summary>
        public MultiPatch()
        {
            this.Common = new MultiCommon();
            this.Sections = new MultiSection[SectionCount];
        }

        /// <summary>
        /// Constructs a multi patch from System Exclusive data.
        /// </summary>
        public MultiPatch(byte[] data) : base()
        {
            using (MemoryStream memory = new MemoryStream(data, false))
	        {
                using (BinaryReader reader = new BinaryReader(memory))
                {
                    var checksum = reader.ReadByte();

                    byte[] commonData = reader.ReadBytes(MultiCommon.DataSize);
                    this.Common = new MultiCommon(commonData);

                    for (int i = 0; i < SectionCount; i++)
                    {
                        byte[] sectionData = reader.ReadBytes(MultiSection.DataSize);
                        this.Sections[i] = new MultiSection(sectionData);
                    }
                }
            }
        }

        /// <summary>
        /// Returns a string representation of this multi patch.
        /// </summary>
        /// <returns>
        /// String representation.
        /// </returns>
        public override string ToString()
        {
            var builder = new StringBuilder();

            // TODO: generate string representation

            return builder.ToString();
        }

        private List<byte> CollectData()
        {
            var data = new List<byte>();

            data.AddRange(this.Common.Data);

            for (int i = 0; i < SectionCount; i++)
            {
                data.AddRange(this.Sections[i].Data);
            }

            return data;
        }

#region Implementation of the ISystemExclusiveData interface for MultiPatch

        public List<byte> Data
        {
            get
            {
                var data = new List<byte>();

                data.Add(this.Checksum);
                data.AddRange(this.CollectData());

                return data;
            }
        }

        public int DataLength
        {
            get
            {
                return 1 + this.Common.DataLength + SectionCount * MultiSection.DataSize;
            }
        }
#endregion

#region Implementation of the IPatch interface for MultiPatch

        public byte Checksum
        {
            get
            {
                List<byte> data = this.CollectData();
                byte sum = 0;
                foreach (byte b in data)
                {
                    sum += b;
                }
                sum += 0xA5;
                return sum;
            }
        }

        public string Name
        {
            get
            {
                return this.Common.Name.Value;
            }

            set
            {
                this.Common.Name = new PatchName(value);
            }
        }
    }
#endregion

}
