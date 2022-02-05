#nullable enable

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using KSynthLib.Common;

namespace KSynthLib.SystemExclusive
{
    public enum ManufacturerKind
    {
        Development,
        Standard,
        Extended,
        Unknown
    }

    public enum ManufacturerGroup
    {
        American,
        EuropeanOrOther,
        Japanese,
        Other,
        Unknown
    }

    public class ManufacturerDefinition
    {
        public ManufacturerKind Kind { get; set; }
        public byte[] Identifier { get; set; }  // one or three bytes
        public ManufacturerGroup Group { get; set; }
        public string Name {get; set; }
        public string DisplayName { get; set; }

        public ManufacturerDefinition()
        {
            Kind = ManufacturerKind.Unknown;
            Identifier = new byte[] { };
            Group = ManufacturerGroup.Unknown;
            Name = string.Empty;
            DisplayName = string.Empty;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            var idString = "";
            foreach (byte b in this.Identifier)
            {
                idString += string.Format("{0:X2}H", b);
            }

            var group = "Unknown";
            switch (this.Group)
            {
                case ManufacturerGroup.American:
                    group = "American";
                    break;

                case ManufacturerGroup.EuropeanOrOther:
                    group = "European or other";
                    break;

                case ManufacturerGroup.Japanese:
                    group = "Japanese";
                    break;

                case ManufacturerGroup.Other:
                    group = "Other";
                    break;

                default:
                    group = "Unknown";
                    break;
            }

            builder.Append(string.Format("{0} (id={1}, {2})", this.DisplayName, idString, group));

            return builder.ToString();
        }

        public static ManufacturerDefinition Find(byte[] identifier)
        {
            // NOTE: Need to compare the contents of the two byte arrays; simply using Equals would
            // compare the references.
            var result = Array.Find(Manufacturers, element => element.Identifier.SequenceEqual(identifier));
            // If not found, returns default value for type -- so should be null?
            return result != null ? result : ManufacturerDefinition.Unknown;
        }

        public static readonly ManufacturerDefinition Development;
        public static readonly ManufacturerDefinition Unknown;

        static ManufacturerDefinition()
        {
            Development = new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Development,
                Identifier = new byte[] { Constants.Development },
                Group = ManufacturerGroup.Other,
                Name = "Development",
                DisplayName = "Development/Non-commercial"
            };

            Unknown = new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Unknown,
                Identifier = new byte[] { },
                Group = ManufacturerGroup.Unknown,
                Name = "Unknown",
                DisplayName = "Unknown"
            };
        }

        private static readonly ManufacturerDefinition[] Manufacturers =
        {
            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x01 },
                Group = ManufacturerGroup.American,
                Name = "Sequential Circuits",
                DisplayName = "Sequential Circuits"
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Extended,
                Identifier = new byte[] { 0x00, 0x00, 0x0E },
                Group = ManufacturerGroup.American,
                Name = "Alesis Studio Electronics",
                DisplayName = "Alesis"
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x40 },
                Group = ManufacturerGroup.Japanese,
                Name = "Kawai Musical Instruments MFG. CO. Ltd",
                DisplayName = "Kawai"
            }
        };

/*
    "00003B": ("MOTU", "Mark Of The Unicorn", .american),
    "40": ("Kawai", "Kawai Musical Instruments MFG. CO. Ltd", .japanese),
    "41": ("Roland", "Roland Corporation", .japanese),
    "42": ("KORG", "Korg Inc.", .japanese),
    "43": ("Yamaha", "Yamaha", .japanese),
*/

    }
}
