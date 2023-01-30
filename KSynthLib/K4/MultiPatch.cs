using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using KSynthLib.Common;

namespace KSynthLib.K4
{
    public class MultiPatch : Patch
    {
        public const int DataSize = 77;
        public const int SectionCount = 8;

        // TODO: Custom validator for characters in the patch name?
        // Or just a setter that enforces it?

        private string _name;

        [Display(Name = "Patch Name")]
        [Required(ErrorMessage = "{0} must be present.")]
        [StringLength(10, ErrorMessage = "{0} must be exactly {1} characters")]
        public string Name
        {
            get => _name.PadRight(10, ' ').Substring(0, 10);
            set => _name = value.PadRight(10, ' ').Substring(0, Math.Min(value.Length, 10));
        }

        [Range(0, 100, ErrorMessage = "{0} must be between {1} and {2}")]
        public int Volume;

        public Section[] Sections;

        [Range(1, 32, ErrorMessage = "{0} must be between {1} and {2}")]
        public int EffectPatch; // 1~32 (on K4)

        public MultiPatch()
        {
            this.Name = "InitMulti";
            this.Volume = 80;
            this.EffectPatch = 1;

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

            this.Name = SystemExclusiveDataConverter.PatchNameFromBytes(
                new List<byte>(data));
            offset += 10;  // name is M0 to M9

            (b, offset) = Util.GetNextByte(data, offset);
            Volume = b & 0x7f;

            (b, offset) = Util.GetNextByte(data, offset);
            this.EffectPatch = b & 0x1f;

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

            builder.AppendLine($"name = {Name}, volume = {Volume}, effect = {EffectPatch}");
            for (var i = 0; i < SectionCount; i++)
            {
                builder.AppendLine($"Section {i + 1}:");
                builder.Append(this.Sections[i].ToString());
                builder.AppendLine();
            }

            return builder.ToString();
        }

        protected override byte[] CollectData()
        {
            var data = new List<byte>();

            data.AddRange(SystemExclusiveDataConverter.BytesFromPatchName(Name));
            data.Add((byte)Volume);
            data.Add(SystemExclusiveDataConverter.ByteFromEffect(EffectPatch));

            for (var i = 0; i < SectionCount; i++)
            {
                data.AddRange(this.Sections[i].GetSystemExclusiveData());
            }

            return data.ToArray();
        }
    }
}
