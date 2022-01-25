using System;
using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K4
{
    public class MultiPatch : Patch
    {
        public const int DataSize = 77;
        public const int SectionCount = 8;

        public LevelType Volume;
        public Section[] Sections;
        public EffectNumberType EffectPatch; // 1~32 (on K4)

        public MultiPatch()
        {
            this._name = "Init      ";
            this.Volume = new LevelType(80);
            this.EffectPatch = new EffectNumberType(1);

            this.Sections = new Section[SectionCount];
            for (var i = 0; i < SectionCount; i++)
            {
                this.Sections[i] = new Section();
            }
        }

        public MultiPatch(byte[] data) : this()
        {
            this.Sections = new Section[SectionCount];

            var offset = 0;
            byte b = 0;  // will be reused when getting the next byte
            _name = GetName(data, offset);
            offset += 10;  // name is M0 to M9

            (b, offset) = Util.GetNextByte(data, offset);
            Volume = new LevelType(b & 0x7f);

            (b, offset) = Util.GetNextByte(data, offset);
            this.EffectPatch = new EffectNumberType(b & 0x1f);

            for (var i = 0; i < SectionCount; i++)
            {
                var sectionData = new byte[Section.DataSize];
                Array.Copy(data, offset, sectionData, 0, Section.DataSize);
                this.Sections[i] = new Section(sectionData);
                offset += Section.DataSize;
            }

            (b, offset) = Util.GetNextByte(data, offset);
            this.Checksum = b;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append($"name = {Name}, volume = {Volume}, effect = {EffectPatch}\n");
            for (var i = 0; i < SectionCount; i++)
            {
                builder.Append($"Section {i + 1}:\n");
                builder.Append(this.Sections[i].ToString());
                builder.Append("\n");
            }

            return builder.ToString();
        }

        protected override byte[] CollectData()
        {
            var data = new List<byte>();

            byte[] nameBytes = Encoding.ASCII.GetBytes(this.Name.PadRight(10));
            data.AddRange(nameBytes);

            data.Add(Volume.ToByte());
            data.Add(EffectPatch.ToByte());

            for (var i = 0; i < SectionCount; i++)
            {
                data.AddRange(this.Sections[i].ToData());
            }

            return data.ToArray();
        }
    }
}
