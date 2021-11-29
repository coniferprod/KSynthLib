using System;
using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K5000
{
    public enum PolyphonyMode
    {
        Poly,
        Solo1,
        Solo2
    }

    public enum AmplitudeModulation  // 0=off, value=1~5(src2~6)
    {
        Off,
        Source2,
        Source3,
        Source4,
        Source5,
        Source6
    }

    public class SingleCommonSettings
    {
        public const int DataSize = 81;
        public const int MaxSources = 6;
        public const int NameLength = 8;

        public EffectAlgorithm EffectAlgorithm; // 1~4 (in SysEx 0~3)
        public ReverbSettings Reverb;
        public EffectSettings Effect1;
        public EffectSettings Effect2;
        public EffectSettings Effect3;
        public EffectSettings Effect4;
        public GEQSettings GEQ;
        public bool DrumMark;  // dummy, always zero for single patch

        private string _name;  // eight characters
        public string Name
        {
            get => _name.Substring(0, NameLength);
            set => _name = value.Substring(0, NameLength);
        }

        public PositiveLevel Volume;
        public PolyphonyMode Poly;  // enumeration
        public bool IsPortamentoEnabled;  // 0=off, 1=on
        public PositiveLevel PortamentoSpeed;
        public int SourceCount;
        public bool[] IsSourceMuted;  // 0=muted, 1=not muted - store the inverse here
        public AmplitudeModulation AM;  // 0=off, value=1~5(src2~6)
        public EffectControl EffectControl1;
        public EffectControl EffectControl2;
        public MacroController Macro1;
        public MacroController Macro2;
        public MacroController Macro3;
        public MacroController Macro4;
        public Switch Switch1;
        public Switch Switch2;
        public Switch FootSwitch1;
        public Switch FootSwitch2;

        public SingleCommonSettings()
        {
            // 3.1.2.1 COMMON DATA (No. 1...38)

            EffectAlgorithm = EffectAlgorithm.Algorithm1;
            Reverb = new ReverbSettings();
            Effect1 = new EffectSettings();
            Effect2 = new EffectSettings();
            Effect3 = new EffectSettings();
            Effect4 = new EffectSettings();
            GEQ = new GEQSettings();

            Name = "NewSound";

            Volume = new PositiveLevel(79);

            Poly = PolyphonyMode.Poly; // No. 49

            SourceCount = 1;  // No. 51
            IsSourceMuted = new bool[MaxSources] { false, false, false, false, false, false }; // No. 52

            AM = AmplitudeModulation.Off;  // No. 53

            EffectControl1 = new EffectControl();  // No. 54-56
            EffectControl2 = new EffectControl();  // No. 57-59

            IsPortamentoEnabled = false;  // No. 60
            PortamentoSpeed = new PositiveLevel();  // No. 61

            // No. 62 ... 77
            Macro1 = new MacroController();
            Macro2 = new MacroController();
            Macro3 = new MacroController();
            Macro4 = new MacroController();

            // No. 78 ... 81
            Switch1 = Switch.Off;
            Switch2 = Switch.Off;
            FootSwitch1 = Switch.Off;
            FootSwitch2 = Switch.Off;
        }

        public SingleCommonSettings(byte[] data) : this()
        {
            int offset = 0;
            byte b = 0;

            (b, offset) = Util.GetNextByte(data, offset);
            // Seems like the K5000 might set the top bits of this, so mask them off
            EffectAlgorithm = (EffectAlgorithm)(b & 0x03);
            //Console.Error.WriteLine($"Effect Algorithm = {EffectAlgorithm}");

            Reverb = new ReverbSettings(data, offset);
            offset += ReverbSettings.DataSize;
            //Console.Error.WriteLine($"Reverb = {Reverb}");

            Effect1 = new EffectSettings(data, offset);
            offset += EffectSettings.DataSize;
            //Console.Error.WriteLine($"E1 = {Effect1}");

            Effect2 = new EffectSettings(data, offset);
            offset += EffectSettings.DataSize;
            //Console.Error.WriteLine($"E2 = {Effect2}");

            Effect3 = new EffectSettings(data, offset);
            offset += EffectSettings.DataSize;
            //Console.Error.WriteLine($"E3 = {Effect3}");

            Effect4 = new EffectSettings(data, offset);
            offset += EffectSettings.DataSize;
            //Console.Error.WriteLine($"E4 = {Effect4}");

            GEQ = new GEQSettings(data, offset);
            offset += GEQSettings.DataSize;
            //Console.Error.WriteLine($"GEQ = {GEQ}");

            (b, offset) = Util.GetNextByte(data, offset);
            DrumMark = (b == 1);

            Name = CollectName(data, offset);
            offset += NameLength;
            //Console.Error.WriteLine($"Name = {Name}");

            (b, offset) = Util.GetNextByte(data, offset);
            Volume = new PositiveLevel(b);
            //Console.Error.WriteLine($"Volume = {Volume}");

            (b, offset) = Util.GetNextByte(data, offset);
            Poly = (PolyphonyMode) b; // 0=POLY, 1=SOLO1, 2=SOLO2
            //Console.Error.WriteLine($"Polyphony = {Poly}");

            // No. 50 or "no use" can be ignored
            (b, offset) = Util.GetNextByte(data, offset);

            (b, offset) = Util.GetNextByte(data, offset);
            SourceCount = b;  // No. 51

            (b, offset) = Util.GetNextByte(data, offset);
            for (var i = 0; i < MaxSources; i++)
            {
                bool isNotMuted = b.IsBitSet(i);
                IsSourceMuted[i] = ! isNotMuted;
            }

            (b, offset) = Util.GetNextByte(data, offset);
            AM = (AmplitudeModulation) b;

            byte[] ec1Data = null;
            (ec1Data, offset) = Util.GetNextBytes(data, offset, 3);
            EffectControl1 = new EffectControl(ec1Data);

            byte[] ec2Data = null;
            (ec2Data, offset) = Util.GetNextBytes(data, offset, 3);
            EffectControl2 = new EffectControl(ec2Data);

            (b, offset) = Util.GetNextByte(data, offset);
            IsPortamentoEnabled = (b == 1);

            (b, offset) = Util.GetNextByte(data, offset);
            PortamentoSpeed = new PositiveLevel(b);

            // The four macro definitions are packed into 16 bytes like this:
            // m1 p1 t, m1 p2 t, m2 p1 t, m2 p2 t, m3 p1 t, m3 p2 t, m4 p1 t, m4 p2 t
            // m1 p1 d, m1 p2 d, m2 p1 d, m2 p2 d, m3 p1 d, m3 p2 d, m4 p1 d, m4 p2 d
            // m = macro, p = parameter, t = type, d = depth
            byte[] bs = null;
            (bs, offset) = Util.GetNextBytes(data, offset, 16);
            Macro1 = new MacroController(
                new MacroControllerParameter(bs[0], bs[8]),
                new MacroControllerParameter(bs[1], bs[9])
            );
            Macro2 = new MacroController(
                new MacroControllerParameter(bs[2], bs[10]),
                new MacroControllerParameter(bs[3], bs[11])
            );
            Macro3 = new MacroController(
                new MacroControllerParameter(bs[4], bs[12]),
                new MacroControllerParameter(bs[5], bs[13])
            );
            Macro4 = new MacroController(
                new MacroControllerParameter(bs[6], bs[14]),
                new MacroControllerParameter(bs[7], bs[15])
            );

            (b, offset) = Util.GetNextByte(data, offset);
            Switch1 = (Switch)b;
            (b, offset) = Util.GetNextByte(data, offset);
            Switch2 = (Switch)b;
            (b, offset) = Util.GetNextByte(data, offset);
            FootSwitch1 = (Switch)b;
            (b, offset) = Util.GetNextByte(data, offset);
            FootSwitch2 = (Switch)b;

            //Console.Error.WriteLine(string.Format("Common data parsed, offset = {0:X2} ({0})", offset));
        }

        private string CollectName(byte[] data, int offset)
        {
            byte[] bytes =
            {
                data[offset],
                data[offset + 1],
                data[offset + 2],
                data[offset + 3],
                data[offset + 4],
                data[offset + 5],
                data[offset + 6],
                data[offset + 7],
            };
            string name = Encoding.ASCII.GetString(bytes);
            return name;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append($"{Name}\n");

            builder.Append($"Volume       {Volume.Value,3}  Sources        {SourceCount}\n");

            string amSettingString = (AM == 0) ? "OFF" : Convert.ToString(AM);
            builder.Append($"Poly        {Poly, 4}  AM           {amSettingString}\n");

            string portamentoSettingString = IsPortamentoEnabled ? "ON" : "OFF";
            builder.Append($"Portamento   {portamentoSettingString}\n");
            builder.Append($"Porta Speed  {PortamentoSpeed.Value}\n");

            builder.Append("\nMacro Controller\n");
            builder.Append($"User 1: {Macro1}\n");
            builder.Append($"User 2: {Macro2}\n");
            builder.Append($"User 3: {Macro3}\n");
            builder.Append($"User 4: {Macro4}\n");

            builder.Append($"Switch1 = {Switch1}    FootSw1 = {FootSwitch1}\n");
            builder.Append($"Switch2 = {Switch2}    FootSw2 = {FootSwitch2}\n");
            builder.Append("Only effective if System FSW is \"Single\"\n");

            return builder.ToString();
        }

        public byte[] ToData()
        {
            var data = new List<byte>();

            data.Add((byte)EffectAlgorithm);  // 0...3
            data.AddRange(Reverb.ToData());
            data.AddRange(Effect1.ToData());
            data.AddRange(Effect2.ToData());
            data.AddRange(Effect3.ToData());
            data.AddRange(Effect4.ToData());
            data.AddRange(GEQ.ToData());
            data.Add(0);  // drum_mark

            string PaddedName = Name.PadRight(NameLength, ' ');
            data.AddRange(ASCIIEncoding.ASCII.GetBytes(PaddedName));

            data.Add(Volume.ToByte());
            data.Add((byte)Poly);
            data.Add(0); // "no use"

            data.Add((byte)SourceCount);
            byte sourceMute = 0;
            for (var i = 0; i < MaxSources; i++)
            {
                if (IsSourceMuted[i])
                {
                    sourceMute.SetBit(i);
                }
            }
            data.Add(sourceMute);

            data.Add((byte)AM);

            data.AddRange(EffectControl1.ToData());
            data.AddRange(EffectControl2.ToData());

            data.Add((byte)(IsPortamentoEnabled ? 1 : 0));  // only bit 0 is used for this
            data.Add(PortamentoSpeed.ToByte());

            var m1p1 = Macro1.Param1.Bytes;
            var m1p2 = Macro1.Param2.Bytes;
            var m2p1 = Macro2.Param1.Bytes;
            var m2p2 = Macro2.Param2.Bytes;
            var m3p1 = Macro3.Param1.Bytes;
            var m3p2 = Macro3.Param2.Bytes;
            var m4p1 = Macro4.Param1.Bytes;
            var m4p2 = Macro4.Param2.Bytes;

            data.AddRange(new List<byte>() {
                m1p1.Type, m1p2.Type, m2p1.Type, m2p2.Type,
                m3p1.Type, m3p2.Type, m4p1.Type, m4p2.Type
            });

            data.AddRange(new List<byte>() {
                m1p1.Depth, m1p2.Depth, m2p1.Depth, m2p2.Depth,
                m3p1.Depth, m3p2.Depth, m4p1.Depth, m4p2.Depth
            });

            data.AddRange(new List<byte>() {
                (byte)Switch1, (byte)Switch2,
                (byte)FootSwitch1, (byte)FootSwitch2
            });

            return data.ToArray();
        }
    }
}