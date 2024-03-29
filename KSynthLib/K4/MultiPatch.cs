using System;
using System.Text;
using System.Collections.Generic;

using SyxPack;
using KSynthLib.Common;

namespace KSynthLib.K4
{
    public class MultiPatch : Patch, ISystemExclusiveData
    {
        public const int DataSize = 77;
        public const int SectionCount = 8;

        public PatchName Name;
        public Level Volume;
        public Section[] Sections;
        public EffectNumber EffectPatch; // 1~32 (on K4)

        public byte[] OriginalData;

        public MultiPatch()
        {
            this.Name = new PatchName("InitMulti");
            this.Volume = new Level();
            this.EffectPatch = new EffectNumber(1);

            this.Sections = new Section[SectionCount];
            for (var i = 0; i < SectionCount; i++)
            {
                this.Sections[i] = new Section();
            }

            OriginalData = null;
        }

        public MultiPatch(byte[] data) : this()
        {
            this.Sections = new Section[SectionCount];

            byte b;  // will be reused when getting the next byte
            var offset = 0;

            this.Name = new PatchName(data);
            offset += 10;  // name is M0 to M9

            (b, offset) = Util.GetNextByte(data, offset);
            Volume = new Level(b & 0x7f);

            (b, offset) = Util.GetNextByte(data, offset);
            this.EffectPatch = new EffectNumber(b & 0x1f);

            for (var i = 0; i < SectionCount; i++)
            {
                var sectionData = new byte[Section.DataSize];
                Array.Copy(data, offset, sectionData, 0, Section.DataSize);
                this.Sections[i] = new Section(sectionData);
                offset += Section.DataSize;
            }

            (b, offset) = Util.GetNextByte(data, offset);
            this.Checksum = b;

            OriginalData = new byte[DataSize];
            Array.Copy(data, OriginalData, DataSize);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine($"name = {Name}, volume = {Volume}, effect = {EffectPatch}");
            for (var i = 0; i < SectionCount; i++)
            {
                builder.AppendLine($"Section {i + 1}:");
                builder.Append(this.Sections[i].ToString());
                builder.AppendLine();
            }

            return builder.ToString();
        }

        protected override List<byte> CollectData()
        {
            var data = new List<byte>();

            data.AddRange(Name.Data);

            data.Add(Volume.ToByte());
            data.Add(EffectPatch.ToByte());

            for (var i = 0; i < SectionCount; i++)
            {
                data.AddRange(this.Sections[i].Data);
            }

            return data;
        }

        //
        // Implementation of ISystemExclusiveData interface
        //

        public List<byte> Data
        {
            get
            {
                var data = new List<byte>();

                data.AddRange(this.CollectData());
                data.Add(this.Checksum);

                return data;
            }
        }

        public int DataLength => DataSize;

        public override byte Checksum
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

            set
            {
                _checksum = value;
            }
        }
    }
}
