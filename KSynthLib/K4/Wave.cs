using System;
using System.Text;

using KSynthLib.Common;

namespace KSynthLib.K4
{
    public class Wave
    {
        public const int WaveCount = 256;

        public static string[] Names = new string[]
        {
            "(not used)",  // just to bring the index in line with the one-based wave number

            // 1 ~ 96 = CYCLIC WAVE LIST
            "SIN 1ST",
            "SIN 2ND",
            "SIN 3RD",
            "SIN 4TH",
            "SIN 5TH",
            "SIN 6TH",
            "SIN 7TH",
            "SIN 8TH",
            "SIN 9TH",
            "SAW 1",
            "SAW 2",
            "SAW 3",
            "SAW 4",
            "SAW 5",
            "SAW 6",
            "SAW 7",
            "SAW 8",
            "PULSE",
            "TRIANGLE",
            "SQUARE",
            "RECTANGULAR 1",
            "RECTANGULAR 2",
            "RECTANGULAR 3",
            "RECTANGULAR 4",
            "RECTANGULAR 5",
            "RECTANGULAR 6",
            "PURE HORN L",
            "PUNCH BRASS 1",
            "OBOE 1",
            "OBOE 2",
            "CLASSIC GRAND",
            "EP 1",
            "EP 2",
            "EP 3",
            "E.ORGAN 1",
            "E.ORGAN 2",
            "POSITIF",
            "E.ORGAN 3",
            "E.ORGAN 4",
            "E.ORGAN 5",
            "E.ORGAN 6",
            "E.ORGAN 7",
            "E.ORGAN 8",
            "E.ORGAN 9",
            "CLASSIC GUITAR",
            "STEEL STRINGS",
            "HARP",
            "WOOD BASS",
            "SYN BASS 3",
            "DIGI BASS",
            "FINGER BASS",
            "MARIMBA",
            "SYN VOICE",
            "GLASS HARP 1",
            "CELLO",
            "XYLO",
            "EP 4",
            "SYN CLAVI 1",
            "EP 5",
            "E.ORGAN 10",
            "E.ORGAN 11",
            "E.ORGAN 12",
            "BIG PIPE",
            "GLASS HARP 2",
            "RANDOM",
            "EP 6",
            "SYN BASS 4",
            "SYN BASS 1",
            "SYN BASS 2",
            "QUENA",
            "OBOE 3",
            "PURE HORN H",
            "FAT BRASS",
            "PUNCH BRASS 2",
            "EP 7",
            "EP 8",
            "SYN CLAVI 2",
            "HARPSICHORD M",
            "HARPSICHORD L",
            "HARPSICHORD H",
            "E.ORGAN 13",
            "KOTO",
            "SITAR L",
            "SITAR H",
            "PICK BASS",
            "SYN BASS 5",
            "SYN BASS 6",
            "VIBRAPHONE ATTACK",
            "VIBRAPHONE 1",
            "HORN VIBE",
            "STEEL DRUM 1",
            "STEEL DRUM 2",
            "VIBRAPHONE 2",
            "MARIMBA ATTACK",
            "HARMONICA",
            "SYNTH",

            // 97 ~ 256 PCM WAVE LIST
            // DRUM & PERCUSSION GROUP
            "KICK",
            "GATED KICK",
            "SNARE TITE",
            "SNARE DEEP",
            "SNARE HI",
            "RIM SNARE",
            "RIM SHOT",
            "TOM",
            "TOM VR",
            "E.TOM",
            "HH CLOSED",
            "HH OPEN",
            "HH OPEN VR",
            "HH FOOT",
            "CRASH",
            "CRASH VR",
            "CRASH VR 2",
            "RIDE EDGE",
            "RIDE EDGE VR",
            "RIDE CUP",
            "RIDE CUP VR",
            "CLAPS",
            "COWBELL",
            "CONGA",
            "CONGA SLAP",
            "TAMBOURINE",
            "TAMBOURINE VR",
            "CLAVES",
            "TIMBALE",
            "SHAKER",
            "SHAKER VR",
            "TIMPANI",
            "TIMPANI VR",
            "SLEIBELL",
            "BELL",
            "METAL HIT",
            "CLICK",
            "POLE",
            "GLOCKEN",
            "MARIMBA",
            "PIANO ATTACK",
            "WATER DROP",
            "CHAR",

            // MULTI GROUP
            "PIANO NRML",
            "PIANO VR",
            "CELLO NRML",
            "CELLO VR1",
            "CELLO VR2",
            "CELLO 1 SHOT",
            "STRINGS NRML",
            "STRINGS VR",
            "SLAP BASS L NRML",
            "SLAP BASS L VR",
            "SLAP BASS L 1 SHOT",
            "SLAP BASS H NRML",
            "SLAP BASS H VR",
            "SLAP BASS H 1 SHOT",
            "PICK BASS NRML",
            "PICK BASS VR",
            "PICK BASS 1 SHOT",
            "WOOD BASS ATTACK",
            "WOOD BASS NRML",
            "WOOD BASS VR",
            "FRETLESS NRML",
            "FRETLESS VR",
            "SYN.BASS NRML",
            "SYN.BASS VR",
            "E.G MUTE NRML",
            "E.G MUTE VR",
            "E.G MUTE 1 SHOT",
            "DIST MUTE NRML",
            "DIST MUTE VR",
            "DIST MUTE 1 SHOT",
            "DIST LEAD NRML",
            "DIST LEAD VR",
            "E.GUITAR NRML",
            "GUT GUITAR NRML",
            "GUT GUITAR VR",
            "GUT GUITAR 1 SHOT",
            "FLUTE NRML",
            "FLUTE 1 SHOT",
            "BOTTLE BLOW NRML",
            "BOTTLE BLOW VR",
            "SAX NRML",
            "SAX VR 1",
            "SAX VR 2",
            "SAX 1 SHOT",
            "TRUMPET NRML",
            "TRUMPET VR 1",
            "TRUMPET VR 2",
            "TRUMPET 1 SHOT",
            "TROMBONE NRML",
            "TROMBONE VR",
            "TROMBONE 1 SHOT",
            "VOICE",
            "NOISE",

            // BLOCK GROUP
            "PIANO 1",
            "PIANO 2",
            "PIANO 3",
            "PIANO 4",
            "PIANO 5",
            "CELLO 1",
            "CELLO 2",
            "CELLO 3",
            "CELLO 4 1 SHOT",
            "CELLO 5 1 SHOT",
            "CELLO 6 1 SHOT",
            "STRINGS 1",
            "STRINGS 2",
            "SLAP BASS L",
            "SLAP BASS L 1 SHOT",
            "SLAP BASS H",
            "SLAP BASS H 1 SHOT",
            "PICK BASS 1",
            "PICK BASS 2 1 SHOT",
            "PICK BASS 3 1 SHOT",
            "E.G MUTE",
            "E.G MUTE 1 SHOT",
            "DIST LEAD 1",
            "DIST LEAD 2",
            "DIST LEAD 3",
            "GUT GUITAR 1",
            "GUT GUITAR 2",
            "GUT GUITAR 3 1 SHOT",
            "GUT GUITAR 4 1 SHOT",
            "FLUTE 1",
            "FLUTE 2",
            "SAX 1",
            "SAX 2",
            "SAX 3",
            "SAX 4 1 SHOT",
            "SAX 5 1 SHOT",
            "SAX 6 1 SHOT",
            "TRUMPET",
            "TRUMPET 1 SHOT",
            "VOICE 1",
            "VOICE 2",

            // REVERSE & LOOP
            "REVERSE 1",
            "REVERSE 2",
            "REVERSE 3",
            "REVERSE 4",
            "REVERSE 5",
            "REVERSE 6",
            "REVERSE 7",
            "REVERSE 8",
            "REVERSE 9",
            "REVERSE 10",
            "REVERSE 11",
            "LOOP 1",
            "LOOP 2",
            "LOOP 3",
            "LOOP 4",
            "LOOP 5",
            "LOOP 6",
            "LOOP 7",
            "LOOP 8",
            "LOOP 9",
            "LOOP 10",
            "LOOP 11",
            "LOOP 12"
        };

        private string _name;
        public string Name
        {
            get => _name;
        }

        private ushort _number;
        public ushort Number {
            get => _number;
        }

        public Wave()
        {
            this._number = 1;
        }

        public Wave(ushort number)
        {
            this._number = number;
            this._name = Names[number];
        }

        public Wave(byte high, byte low)
        {
            ushort waveNumber = Wave.NumberFrom(high, low);
            this._number = waveNumber;
            this._name = Names[waveNumber];
        }

        public (byte high, byte low) WaveSelect
        {
            get
            {
                byte waveNumber = (byte)(this._number - 1);  // bring into range 0~255 for SysEx
                byte highByte = waveNumber.IsBitSet(0) ? (byte)0x01 : (byte)0x00;
                string lowBitsString = ByteExtensions.ToBinaryString(waveNumber).Reversed().Substring(1);
                byte lowByte = Convert.ToByte(lowBitsString, 2);
                return (highByte, lowByte);
            }
        }

        public static ushort NumberFrom(byte high, byte low)
        {
            int h = high & 0x01;  // `wave select h` is b0 of s34/s35/s36/s37
            int l = low & 0x7f;    // `wave select l` is bits 0...6 of s38/s39/s40/s41
            //print("high = 0x\(String(high, radix: 16)), low = 0x\(String(low, radix: 16))")
            // Combine the h and l to one 8-bit value and make it 1~256
            return (ushort)(((h << 7) | l) + 1);
        }

        /// <summary>
        /// Generates a printable representation of this wave.
        /// </summary>
        /// <returns>
        /// String representing the wave.
        /// </returns>
        public override string ToString()
        {
            return $"{this.Number} {this.Name}";
        }
    }
}