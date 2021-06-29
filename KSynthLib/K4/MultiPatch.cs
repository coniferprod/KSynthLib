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

        private Section[] sections;

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

            sections = new Section[SectionCount];
            for (int i = 0; i < SectionCount; i++)
            {
                sections[i] = new Section();
            }
        }

        public MultiPatch(byte[] data) : this()
        {
            sections = new Section[SectionCount];

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
                sections[i] = new Section(sectionData);
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
                builder.Append(sections[i].ToString());
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
                data.AddRange(sections[i].ToData());
            }

            return data.ToArray();
        }
    }
}