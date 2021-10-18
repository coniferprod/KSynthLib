using System;
using System.Collections.Generic;
using System.Text;

using KSynthLib.Common;

namespace KSynthLib.K5000
{
    public class Wave
    {
        public const int WaveCount = 464;

        private string _name;
        public string Name => _name;

        private ushort _number;
        public ushort Number => _number;

        public Wave()
        {
            this._number = 1;
            this._name = Names[this._number];
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
                // Convert wave kit number to binary string with 10 digits
                var waveString = Convert.ToString(this.Number - 1, 2).PadLeft(10, '0');

                // Take the first three bits and convert them to a number
                var waveMSBString = waveString.Substring(0, 3);
                var msb = Convert.ToByte(waveMSBString);

                // Take the last seven bits and convert them to a number
                var waveLSBString = waveString.Substring(3);
                var lsb = Convert.ToByte(waveLSBString);

                return (msb, lsb);
            }
        }

        public static ushort NumberFrom(byte msb, byte lsb)
        {
            var waveMSBString = Convert.ToString(msb, 2).PadLeft(3, '0');
            var waveLSBString = Convert.ToString(lsb, 2).PadLeft(7, '0');
            var waveString = waveMSBString + waveLSBString;
            return (ushort)(Convert.ToUInt16(waveString) + 1);
        }

        public bool IsAdditive()
        {
            return this.Number == 512;
        }

        public byte[] ToData()
        {
            var data = new List<byte>();
            var (msb, lsb) = this.WaveSelect;
            data.Add(msb);
            data.Add(lsb);
            return data.ToArray();
        }

        public static string[] Names = new string[]
        {
            "(not used)",  // just to bring the index in line with the one-based wave number

            /*  1 */ "OldUprit1",
            /*  2 */ "OldUprit2",
            /*  3 */ "Gr.Piano",
            /*  4 */ "WidPiano",
            /*  5 */ "Br.Piano",
            /*  6 */ "Hnkytonk1",
            /*  7 */ "E.Grand1",
            /*  8 */ "Hnkytonk2",
            /*  9 */ "E.Grand2",
            /*  10 */ "E.Grand3",
            /*  11 */ "Metallic1",
            /*  12 */ "E.Piano1",
            /*  13 */ "60's EP",
            /*  14 */ "E.Piano2",
            /*  15 */ "E.Piano3",
            /*  16 */ "E.Piano4",
            /*  17 */ "Clavi 1",
            /*  18 */ "Drawbar1",
            /*  19 */ "Drawbar2",
            /*  20 */ "DetunOr1",
            /*  21 */ "Drawbar3",
            /*  22 */ "PercOrg1",
            /*  23 */ "PercOrg2",
            /*  24 */ "ChrcOrg1",
            /*  25 */ "ChrcOrg2",
            /*  26 */ "Celesta1",
            /*  27 */ "Vibe",
            /*  28 */ "Glocken1",
            /*  29 */ "Marimba",
            /*  30 */ "Glocken2",
            /*  31 */ "NewAge1",
            /*  32 */ "Xylophon",
            /*  33 */ "TubulBel",
            /*  34 */ "Stl Drum",
            /*  35 */ "Timpani1",
            /*  36 */ "CncertBD1",
            /*  37 */ "NylonGt1",
            /*  38 */ "Ukulele",
            /*  39 */ "NylonGt2",
            /*  40 */ "Nyln+Stl",
            /*  41 */ "Atmosphr1",
            /*  42 */ "SteelGt1",
            /*  43 */ "Sci-Fi1",
            /*  44 */ "Mandolin1",
            /*  45 */ "Mandolin2",
            /*  46 */ "SteelGt2",
            /*  47 */ "12strGtr1",
            /*  48 */ "12strGtr2",
            /*  49 */ "Dulcimer1",
            /*  50 */ "JazzGtr1",
            /*  51 */ "CleanGtr1",
            /*  52 */ "Hi.E.Gtr1",
            /*  53 */ "ChorusGt",
            /*  54 */ "TubeBass1",
            /*  55 */ "CleanGtr2",
            /*  56 */ "Hi.E.Gtr2",
            /*  57 */ "MuteGtr1",
            /*  58 */ "OvrDrive1",
            /*  59 */ "ResO.D.1",
            /*  60 */ "OvrDrive2",
            /*  61 */ "ResO.D.2",
            /*  62 */ "Distortd",
            /*  63 */ "Charang1",
            /*  64 */ "Charang2",
            /*  65 */ "FeedbkGt1",
            /*  66 */ "PowerGtr1",
            /*  67 */ "Res.Dist",
            /*  68 */ "RockOrgn1",
            /*  69 */ "PowerGtr2",
            /*  70 */ "Harmnics",
            /*  71 */ "Dulcimer2",
            /*  72 */ "Banjo",
            /*  73 */ "Sitar",
            /*  74 */ "Shamisen",
            /*  75 */ "Koto",
            /*  76 */ "TaishoKt1",
            /*  77 */ "TaishoKt2",
            /*  78 */ "Harp1",
            /*  79 */ "Harp2",
            /*  80 */ "Ac.Bass1",
            /*  81 */ "Ac.Bass2",
            /*  82 */ "Ac.Bass3",
            /*  83 */ "FngBass1",
            /*  84 */ "Ac.Bass4",
            /*  85 */ "FngBass2",
            /*  86 */ "TubeBass2",
            /*  87 */ "PickBass1",
            /*  88 */ "MutePick1",
            /*  89 */ "PickBass2",
            /*  90 */ "MutePick2",
            /*  91 */ "Fretless",
            /*  92 */ "SlapBas1",
            /*  93 */ "FunkGtr1",
            /*  94 */ "FunkGtr2",
            /*  95 */ "SlapBas2",
            /*  96 */ "SlapBas3",
            /*  97 */ "SlapBas4",
            /*  98 */ "SynBass1",
            /*  99 */ "SynBass2",
            /* 100 */ "SynBass3",
            /* 101 */ "SynBass4",
            /* 102 */ "HouseBass1",
            /* 103 */ "HouseBass2",
            /* 104 */ "SynBass5",
            /* 105 */ "Violn",
            /* 106 */ "Fiddle",
            /* 107 */ "SlwVioln",
            /* 108 */ "Viola",
            /* 109 */ "Cello",
            /* 110 */ "Contra1",
            /* 111 */ "Contra2",
            /* 112 */ "Strings1",
            /* 113 */ "Strings2",
            /* 114 */ "Orchstra1",
            /* 115 */ "Strings3",
            /* 116 */ "Strings4",
            /* 117 */ "Bright1",
            /* 118 */ "Atmosphr2",
            /* 119 */ "Sweep1",
            /* 120 */ "Pizzicto1",
            /* 121 */ "Pizzicto2",
            /* 122 */ "SynStrg1",
            /* 123 */ "SynBras1",
            /* 124 */ "SynStrg2",
            /* 125 */ "Poly Syn1",
            /* 126 */ "Rain1",
            /* 127 */ "Soundtrk1",
            /* 128 */ "Soundtrk2",
            /* 129 */ "SynBass5",
            /* 130 */ "SynStrg3",
            /* 131 */ "SynStrg4",
            /* 132 */ "SynBras2",
            /* 133 */ "SynBras3",
            /* 134 */ "Chiff1",
            /* 135 */ "Fifth1",
            /* 136 */ "Fifth2",
            /* 137 */ "Metallic2",
            /* 138 */ "Sci-Fi2",
            /* 139 */ "ChorAah1",
            /* 140 */ "Voice1",
            /* 141 */ "Halo Pad1",
            /* 142 */ "Echoes",
            /* 143 */ "ChorAah2",
            /* 144 */ "ChorAah3",
            /* 145 */ "Sweep2",
            /* 146 */ "RockOrgn2",
            /* 147 */ "Choir1",
            /* 148 */ "Halo Pad2",
            /* 149 */ "Chiff2",
            /* 150 */ "Bright2",
            /* 151 */ "Voi Ooh1",
            /* 152 */ "SynVoice",
            /* 153 */ "NewAge2",
            /* 154 */ "Choir2",
            /* 155 */ "Goblns1",
            /* 156 */ "Voi Ooh2",
            /* 157 */ "Orchstra2",
            /* 158 */ "Oct.Bras1",
            /* 159 */ "BrasSect1",
            /* 160 */ "Brass1",
            /* 161 */ "Oct.Bras2",
            /* 162 */ "Orch Hit1",
            /* 163 */ "Orch Hit2",
            /* 164 */ "WarmTrmp",
            /* 165 */ "Trumpet",
            /* 166 */ "Tuba1",
            /* 167 */ "DublBone",
            /* 168 */ "Tuba2",
            /* 169 */ "TromBone",
            /* 170 */ "BrasSect2",
            /* 171 */ "Mute Tp",
            /* 172 */ "FrenchHr1",
            /* 173 */ "FrenchHr2",
            /* 174 */ "SprnoSax",
            /* 175 */ "Bassoon1",
            /* 176 */ "AltoSax1",
            /* 177 */ "AltoSax2",
            /* 178 */ "TenorSax1",
            /* 179 */ "BrthTenr1",
            /* 180 */ "Brass2",
            /* 181 */ "Bari Sax",
            /* 182 */ "EnglHorn",
            /* 183 */ "Bassoon2",
            /* 184 */ "Oboe",
            /* 185 */ "Winds1",
            /* 186 */ "Winds2",
            /* 187 */ "Shanai1",
            /* 188 */ "Bag Pipe1",
            /* 189 */ "Clarinet1",
            /* 190 */ "Winds3",
            /* 191 */ "Flute1",
            /* 192 */ "Winds4",
            /* 193 */ "Calliope1",
            /* 194 */ "Flute2",
            /* 195 */ "Piccolo1",
            /* 196 */ "PanFlute1",
            /* 197 */ "Bottle",
            /* 198 */ "Calliope2",
            /* 199 */ "Voice2",
            /* 200 */ "Shakhach",
            /* 201 */ "Kalimba1",
            /* 202 */ "Agogo",
            /* 203 */ "WoodBlok",
            /* 204 */ "Melo.Tom",
            /* 205 */ "Syn.Drum",
            /* 206 */ "E.Percus",
            /* 207 */ "Scratch",
            /* 208 */ "E.Tom1",
            /* 209 */ "E.Tom2",
            /* 210 */ "Castanet",
            /* 211 */ "TaikoDrm",
            /* 212 */ "RevCymb1",
            /* 213 */ "WndChime",
            /* 214 */ "BrthNoiz",
            /* 215 */ "Flute3",
            /* 216 */ "Recorder1",
            /* 217 */ "PanFlute2",
            /* 218 */ "Ocarina1",
            /* 219 */ "Flute4",
            /* 220 */ "DrawBar4",
            /* 221 */ "Piccolo2",
            /* 222 */ "TenorSax2",
            /* 223 */ "BrthTenr2",
            /* 224 */ "Seashore",
            /* 225 */ "Wind",
            /* 226 */ "FretNoiz",
            /* 227 */ "GtCtNiz1",
            /* 228 */ "GtCtNiz2",
            /* 229 */ "StrgSlap",
            /* 230 */ "Rain2",
            /* 231 */ "Thunder",
            /* 232 */ "Stream1",
            /* 233 */ "Stream2",
            /* 234 */ "Bubble",
            /* 235 */ "Bird1",
            /* 236 */ "Bird2",
            /* 237 */ "Dog",
            /* 238 */ "HorseGalp",
            /* 239 */ "Tel1",
            /* 240 */ "DoorCreak",
            /* 241 */ "Door",
            /* 242 */ "Helicopter",
            /* 243 */ "CarEngine",
            /* 244 */ "CarStop",
            /* 245 */ "CarPass",
            /* 246 */ "CarCrash",
            /* 247 */ "Siren",
            /* 248 */ "Train",
            /* 249 */ "JetPlan",
            /* 250 */ "StarShip",
            /* 251 */ "Applause1",
            /* 252 */ "Applause2",
            /* 253 */ "Laughing",
            /* 254 */ "Screaming",
            /* 255 */ "Punch",
            /* 256 */ "HeartBeat",
            /* 257 */ "FootStep",
            /* 258 */ "Gun",
            /* 259 */ "MachinGun",
            /* 260 */ "LaserGun",
            /* 261 */ "Explosion",
            /* 262 */ "Omni1",
            /* 263 */ "Omni2",
            /* 264 */ "Rain3",
            /* 265 */ "MuteGtr2",
            /* 266 */ "MusicBox1",
            /* 267 */ "Sine",
            /* 268 */ "Bowed1",
            /* 269 */ "ConcrtBD2",
            /* 270 */ "FngBass3",
            /* 271 */ "FeedbkGt2",
            /* 272 */ "Timpani2",
            /* 273 */ "SawLead1",
            /* 274 */ "Dr.Solo1",
            /* 275 */ "Dr.Solo2",
            /* 276 */ "SawLead2",
            /* 277 */ "DistClav1",
            /* 278 */ "DistClav2",
            /* 279 */ "DstSawLd1",
            /* 280 */ "DstSawLd2",
            /* 281 */ "Bass&Ld1",
            /* 282 */ "Bass&Ld2",
            /* 283 */ "PolySyn2",
            /* 284 */ "SawLead3",
            /* 285 */ "SquarLd1",
            /* 286 */ "SquarLd2",
            /* 287 */ "SquarLd3",
            /* 288 */ "SquarLd4",
            /* 289 */ "Dist.Sqr1",
            /* 290 */ "Dist.Sqr2",
            /* 291 */ "E.Piano5",
            /* 292 */ "E.Piano6",
            /* 293 */ "E.Piano7",
            /* 294 */ "Clavi2",
            /* 295 */ "Hrpschrd1",
            /* 296 */ "Hrpschrd2",
            /* 297 */ "PercOrg3",
            /* 298 */ "Drawbar5",
            /* 299 */ "DetunOr2",
            /* 300 */ "DetunOr3",
            /* 301 */ "60'sOrg",
            /* 302 */ "CheseOrg",
            /* 303 */ "PercOrg4",
            /* 304 */ "ChrchOrg3",
            /* 305 */ "ReedOrgn1",
            /* 306 */ "ReedOrgn2",
            /* 307 */ "Accord.1",
            /* 308 */ "Accord.2",
            /* 309 */ "Accord.3",
            /* 310 */ "Accord.4",
            /* 311 */ "TangoAcd1",
            /* 312 */ "TangoAcd2",
            /* 313 */ "Harmnica",
            /* 314 */ "Celesta2",
            /* 315 */ "MusicBox2",
            /* 316 */ "Crystal1",
            /* 317 */ "Crystal2",
            /* 318 */ "Kalimba2",
            /* 319 */ "TnklBell1",
            /* 320 */ "TnklBell2",
            /* 321 */ "JazzGtr2",
            /* 322 */ "MelowGt1",
            /* 323 */ "Hawaiian",
            /* 324 */ "MelowGt2",
            /* 325 */ "SynBass6",
            /* 326 */ "SynBass7",
            /* 327 */ "SynBass8",
            /* 328 */ "SynBras4",
            /* 329 */ "SynBras5",
            /* 330 */ "Warm1",
            /* 331 */ "Warm2",
            /* 332 */ "Bowed2",
            /* 333 */ "Sweep3",
            /* 334 */ "Sweep4",
            /* 335 */ "Goblns2",
            /* 336 */ "Whistle1",
            /* 337 */ "Whistle2",
            /* 338 */ "Ocarina2",
            /* 339 */ "Recorder2",
            /* 340 */ "Bag Pipe2",
            /* 341 */ "Shanai2",

            // K5000S PCM wave names (with some obvious typos corrected)

            /* Inst Noise Attack */
            /* 342 */ "Piano Noise Attack",
            /* 343 */ "EP Noise Attack",
            /* 344 */ "Percus Noise Attack",
            /* 345 */ "Dist Gtr Noise Attack",
            /* 346 */ "Orch Noise Attack",
            /* 347 */ "Flanged Noise Attack",
            /* 348 */ "Saw Noise Attack",
            /* 349 */ "Zipper Noise Attack",

            /* Inst Noise Looped */
            /* 350 */ "Organ Noise Looped",
            /* 351 */ "Violin Noise Looped",
            /* 352 */ "Crystal Noise Looped",
            /* 353 */ "Sax Breath Looped",
            /* 354 */ "Panflute Noise Looped",
            /* 355 */ "Pipe Noise Looped",
            /* 356 */ "Saw Noise Looped",
            /* 357 */ "Gorgo Noise Looped",
            /* 358 */ "Enhancer Noise Looped",
            /* 359 */ "Tabla Spectrum Noise Looped",
            /* 360 */ "Cave Spectrum Noise Looped",
            /* 361 */ "White Noise Looped",

            /* Inst Attack */
            /* 362 */ "Clavi Attack",
            /* 363 */ "Digi EP Attack",
            /* 364 */ "Glocken Attack",
            /* 365 */ "Vibe Attack",
            /* 366 */ "Marimba Attack",
            /* 367 */ "Org Key Click",
            /* 368 */ "Slap Bass Attack",
            /* 369 */ "Folk Gtr Attack",
            /* 370 */ "Gut Gtr Attack",
            /* 371 */ "Dist Gtr Attack",
            /* 372 */ "Clean Gtr Attack",
            /* 373 */ "Muted Gtr Attack",
            /* 374 */ "Cello & Violin Attack",
            /* 375 */ "Pizz Violin Attack",
            /* 376 */ "Pizz Double Bass Attack",
            /* 377 */ "Doo Attack",
            /* 378 */ "Trombone Attack",
            /* 379 */ "Brass Attack",
            /* 380 */ "F.Horn1 Attack",
            /* 381 */ "F.Horn2 Attack",
            /* 382 */ "Flute Attack",
            /* 383 */ "T.Sax Attack",
            /* 384 */ "Shamisen Attack",

            /* Analog Attack */
            /* 385 */ "Voltage Attack",
            /* 386 */ "BBDigi Attack",
            /* 387 */ "BBDX Attack",
            /* 388 */ "BBBlip Attack",
            /* 389 */ "Techno Hit Attack",
            /* 390 */ "Techno Attack",
            /* 391 */ "X-Piano Attack",

            /* Analog Loop */
            /* 392 */ "Noisy Voice Looped",
            /* 393 */ "Noisy Human Looped",
            /* 394 */ "Ravoid Looped",
            /* 395 */ "Hyper Looped",
            /* 396 */ "Beef Looped",
            /* 397 */ "Texture Looped",
            /* 398 */ "MMBass Looped",

            /* Cyclic Loop */
            /* 399 */ "Syn PWM Cyc",
            /* 400 */ "Harpsichord Cyc",
            /* 401 */ "Digi EP Cyc",
            /* 402 */ "Soft EP Cyc",
            /* 403 */ "EP Bell Cyc",
            /* 404 */ "Bandoneon Cyc",
            /* 405 */ "Cheesy Organ Cyc",
            /* 406 */ "Organ Cyc",
            /* 407 */ "Oboe Cyc",
            /* 408 */ "Crystal Cyc",
            /* 409 */ "Syn Bass1 Cyc",
            /* 410 */ "Syn Bass2 Cyc",
            /* 411 */ "Syn Saw1 Cyc",
            /* 412 */ "Syn Saw2 Cyc",
            /* 413 */ "Syn Saw3 Cyc",
            /* 414 */ "Syn Square1 Cyc",
            /* 415 */ "Syn Square2 Cyc",
            /* 416 */ "Syn Pulse1 Cyc",
            /* 417 */ "Syn Pulse2 Cyc",
            /* 418 */ "Pulse20 Cyc",
            /* 419 */ "Pulse40 Cyc",
            /* 420 */ "Nasty Cyc",
            /* 421 */ "Mini Max Cyc",
            /* 422 */ "Bottom Cyc",
            /* 423 */ "Over 64th harmonics only Cyc",
            /* 424 */ "Over 64th harmonics only Cyc",

            /* Percus Attack */
            /* 425 */ "BD Attack",
            /* 426 */ "Ana Kick",
            /* 427 */ "SD Attack",
            /* 428 */ "Tiny SD Attack",
            /* 429 */ "Ana SD Attack",
            /* 430 */ "Ana HHD Attack",
            /* 431 */ "Simonzu Tom Attack",
            /* 432 */ "Ride Cup Attack",
            /* 433 */ "Cowbell Attack",
            /* 434 */ "Conga Attack",
            /* 435 */ "CongaMuted Attack",
            /* 436 */ "Agogo Attack",
            /* 437 */ "Castanet Attack",
            /* 438 */ "Claves Attack",
            /* 439 */ "Tambourine Attack",
            /* 440 */ "JingleBell Attack",
            /* 441 */ "BellTree Attack",
            /* 442 */ "WindowChime Attack",
            /* 443 */ "Atarigane Attack",
            /* 444 */ "Rama Attack",
            /* 445 */ "Udo Attack",
            /* 446 */ "TablaNa Attack",
            /* 447 */ "Voice Du Attack",
            /* 448 */ "HighQ Attack",
            /* 449 */ "Super Q Attack",
            /* 450 */ "Glass Attack",
            /* 451 */ "Metal Attack",
            /* 452 */ "Noise Attack",
            /* 453 */ "Pop Attack",

            /* S.E Loop */
            /* 454 */ "Crash Looped",
            /* 455 */ "Burner Looped",
            /* 456 */ "Jet Engine Looped",

            /* Omnibus Loop */
            /* 457 */ "Omnibus Loop 1",
            /* 458 */ "Omnibus Loop 2",
            /* 459 */ "Omnibus Loop 3",
            /* 460 */ "Omnibus Loop 4",
            /* 461 */ "Omnibus Loop 5",
            /* 462 */ "Omnibus Loop 6",
            /* 463 */ "Omnibus Loop 7",
            /* 464 */ "Omnibus Loop 8"
        };

        /// <summary>
        /// Generates a printable representation of this wave.
        /// </summary>
        /// <returns>
        /// String representing the wave.
        /// </returns>
        public override string ToString()
        {
            if (this.IsAdditive())
            {
                return "ADD";
            }

            return string.Format($"{this.Number} {this.Name}");
        }
    }
}
