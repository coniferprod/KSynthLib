using System;
using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K4
{
    public class MultiPatch : Patch
    {
        public const int DataSize = 77;

        public static readonly int SectionCount = 8;

        private int volume;

        private Section[] sections;

        // The EffectPatch property handles the off-by-one
        private int _effectPatch;  // in SysEx as 0...31, stored as 1...32
        public int EffectPatch
        {
            get => this._effectPatch + 1;
            set => this._effectPatch = value + 1;
        }

        public MultiPatch()
        {
            this._name = "Init";
            this.volume = 80;
            this._effectPatch = 1;

            sections = new Section[SectionCount];
            for (int i = 0; i < SectionCount; i++)
            {
                sections[i] = new Section();
            }
        }

        public MultiPatch(byte[] data)
        {
            sections = new Section[SectionCount];

            int offset = 0;
            byte b = 0;  // will be reused when getting the next byte
            _name = GetName(data, offset);
            offset += 10;  // name is M0 to M9

            (b, offset) = Util.GetNextByte(data, offset);
            volume = b;

            (b, offset) = Util.GetNextByte(data, offset);
            this.EffectPatch = b;  // use property to handle 0/1 difference

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
            builder.Append($"name = {Name}, volume = {volume}, effect = {EffectPatch}\n");
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

            data.Add((byte)volume);
            data.Add((byte)_effectPatch);  // use the zero-based raw value here

            for (int i = 0; i < SectionCount; i++)
            {
                data.AddRange(sections[i].ToData());
            }

            return data.ToArray();
        }
    }
}