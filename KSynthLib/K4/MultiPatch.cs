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

        private LevelType _volume;
        public byte Volume  // 0~100
        {
            get => _volume.Value;
            set => _volume.Value = value;
        }

        public Section[] Sections;

        private EffectNumberType _effectPatch;
        public byte EffectPatch  // 1~32 (on K4)
        {
            get => _effectPatch.Value;
            set =>_effectPatch.Value = value;
        }

        public MultiPatch()
        {
            this._name = "Init      ";
            this._volume = new LevelType(80);
            this._effectPatch = new EffectNumberType(1);

            this.Sections = new Section[SectionCount];
            for (int i = 0; i < SectionCount; i++)
            {
                this.Sections[i] = new Section();
            }
        }

        public MultiPatch(byte[] data) : this()
        {
            this.Sections = new Section[SectionCount];

            int offset = 0;
            byte b = 0;  // will be reused when getting the next byte
            _name = GetName(data, offset);
            offset += 10;  // name is M0 to M9

            (b, offset) = Util.GetNextByte(data, offset);
            Volume = b;

            (b, offset) = Util.GetNextByte(data, offset);
            this.EffectPatch = (byte)(b + 1);  // store 0~31 as 1~32

            for (int i = 0; i < SectionCount; i++)
            {
                byte[] sectionData = new byte[Section.DataSize];
                Array.Copy(data, offset, sectionData, 0, Section.DataSize);
                this.Sections[i] = new Section(sectionData);
                offset += Section.DataSize;
            }

            (b, offset) = Util.GetNextByte(data, offset);
            this.Checksum = b;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"name = {Name}, volume = {Volume}, effect = {EffectPatch}\n");
            for (int i = 0; i < SectionCount; i++)
            {
                builder.Append($"Section {i + 1}:\n");
                builder.Append(this.Sections[i].ToString());
                builder.Append("\n");
            }

            return builder.ToString();
        }

        protected override byte[] CollectData()
        {
            List<byte> data = new List<byte>();

            byte[] nameBytes = Encoding.ASCII.GetBytes(this.Name.PadRight(10));
            data.AddRange(nameBytes);

            data.Add((byte)Volume);
            data.Add((byte)(EffectPatch - 1));  // store 1~32 as 0~31

            for (int i = 0; i < SectionCount; i++)
            {
                data.AddRange(this.Sections[i].ToData());
            }

            return data.ToArray();
        }
    }
}
