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
        public string Name { get; set; }

        public ManufacturerDefinition()
        {
            Kind = ManufacturerKind.Unknown;
            Identifier = new byte[] { };
            Group = ManufacturerGroup.Unknown;
            Name = string.Empty;
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

            builder.Append(string.Format("{0} (id={1}, {2})", this.Name, idString, group));

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
                Name = "Development/Non-commercial"
            };

            Unknown = new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Unknown,
                Identifier = new byte[] { },
                Group = ManufacturerGroup.Unknown,
                Name = "Unknown"
            };
        }

        public static readonly ManufacturerDefinition[] Manufacturers =
        {
            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x01 },
                Group = ManufacturerGroup.American,
                Name = "Sequential Circuits"
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x02 },
                Group = ManufacturerGroup.American,
                Name = "IDP"
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x03 },
                Group = ManufacturerGroup.American,
                Name = "Voyetra Turtle Beach, Inc."
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x04 },
                Group = ManufacturerGroup.American,
                Name = "Moog Music"
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x05 },
                Group = ManufacturerGroup.American,
                Name = "Passport Designs"
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x06 },
                Group = ManufacturerGroup.American,
                Name = "Lexicon Inc."
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x07 },
                Group = ManufacturerGroup.American,
                Name = "Kurzweil / Young Chang"
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x08 },
                Group = ManufacturerGroup.American,
                Name = "Fender"
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x09 },
                Group = ManufacturerGroup.American,
                Name = "MIDI9"
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x0A },
                Group = ManufacturerGroup.American,
                Name = "AKG Acoustics"
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x0B },
                Group = ManufacturerGroup.American,
                Name = "Voyce Music"
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x0C },
                Group = ManufacturerGroup.American,
                Name = "Waveframe (Timeline)"
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x0D },
                Group = ManufacturerGroup.American,
                Name = "ADA Signal Processors, Inc."
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x0E },
                Group = ManufacturerGroup.American,
                Name = "Garfield Electronics"
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x0F },
                Group = ManufacturerGroup.American,
                Name = "Ensoniq"
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x10 },
                Group = ManufacturerGroup.American,
                Name = "Oberheim / Gibson Labs"
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x11 },
                Group = ManufacturerGroup.American,
                Name = "Apple"
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x12 },
                Group = ManufacturerGroup.American,
                Name = "Gray Matter Response"
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x13 },
                Group = ManufacturerGroup.American,
                Name = "Digidesign Inc."
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x14 },
                Group = ManufacturerGroup.American,
                Name = "Palmtree Instruments"
            },

            // ...some items missing...

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x18 },
                Group = ManufacturerGroup.American,
                Name = "E-MU"
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x3A },
                Group = ManufacturerGroup.EuropeanOrOther,
                Name = "Steinberg Media Technologies GmbH"
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x3E },
                Group = ManufacturerGroup.EuropeanOrOther,
                Name = "Waldorf Electronics GmbH"
            },

            // Extended / American

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Extended,
                Identifier = new byte[] { 0x00, 0x00, 0x0E },
                Group = ManufacturerGroup.American,
                Name = "Alesis Studio Electronics"
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Extended,
                Identifier = new byte[] { 0x00, 0x00, 0x3B },
                Group = ManufacturerGroup.American,
                Name = "Mark Of the Unicorn"
            },

            // Extended / Japanese

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x40 },
                Group = ManufacturerGroup.Japanese,
                Name = "Kawai Musical Instruments MFG. CO. Ltd",
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x41 },
                Group = ManufacturerGroup.Japanese,
                Name = "Roland Corporation",
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x42 },
                Group = ManufacturerGroup.Japanese,
                Name = "Korg Inc."
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x43 },
                Group = ManufacturerGroup.Japanese,
                Name = "Yamaha Corporation"
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x44 },
                Group = ManufacturerGroup.Japanese,
                Name = "Casio Computer Co.Ltd"
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x46 },
                Group = ManufacturerGroup.Japanese,
                Name = "Kamiya Studio Co.Ltd"
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x47 },
                Group = ManufacturerGroup.Japanese,
                Name = "Akai Electric Co.Ltd."
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x48 },
                Group = ManufacturerGroup.Japanese,
                Name = "Victor Company of Japan, Ltd."
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x48 },
                Group = ManufacturerGroup.Japanese,
                Name = "Fujitsu Limited"
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x4C },
                Group = ManufacturerGroup.Japanese,
                Name = "Sony Corporation"
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x4E },
                Group = ManufacturerGroup.Japanese,
                Name = "Teac Corporation"
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x50 },
                Group = ManufacturerGroup.Japanese,
                Name = "Matsushita Electric Industrial Co., Ltd"
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x51 },
                Group = ManufacturerGroup.Japanese,
                Name = "Fostex Corporation"
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x52 },
                Group = ManufacturerGroup.Japanese,
                Name = "Zoom Corporation"
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x54 },
                Group = ManufacturerGroup.Japanese,
                Name = "Matsushita Communication Industrial Co., Ltd."
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x55 },
                Group = ManufacturerGroup.Japanese,
                Name = "Suzuki Musical Instruments MFG.Co., Ltd."
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x56 },
                Group = ManufacturerGroup.Japanese,
                Name = "Fuji Sound Corporation Ltd."
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x57 },
                Group = ManufacturerGroup.Japanese,
                Name = "Acoustic Technical Laboratory, Inc."
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x59 },
                Group = ManufacturerGroup.Japanese,
                Name = "Faith, Inc."
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x5A },
                Group = ManufacturerGroup.Japanese,
                Name = "Internet Corporation"
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x5C },
                Group = ManufacturerGroup.Japanese,
                Name = "Seekers Co.Ltd."
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Standard,
                Identifier = new byte[] { 0x5F },
                Group = ManufacturerGroup.Japanese,
                Name = "SD Card Association"
            },

            // Extended / Japanese

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Extended,
                Identifier = new byte[] { 0x00, 0x40, 0x00 },
                Group = ManufacturerGroup.Japanese,
                Name = "Crimson Technology Inc."
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Extended,
                Identifier = new byte[] { 0x00, 0x40, 0x01 },
                Group = ManufacturerGroup.Japanese,
                Name = "Softbank Mobile Corp"
            },

            new ManufacturerDefinition
            {
                Kind = ManufacturerKind.Extended,
                Identifier = new byte[] { 0x00, 0x40, 0x03 },
                Group = ManufacturerGroup.Japanese,
                Name = "D & M Holdings Inc."
            }
        };
    }
}
