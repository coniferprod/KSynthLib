using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using KSynthLib.Common;

namespace KSynthLib.K5000
{
    public enum PolyphonyMode {
        Poly,
        Solo1,
        Solo2
    }

    public class SingleCommonSettings
    {
        public const int DataSize = 33;

        public const int MaxSources = 6;

        public PolyphonyMode Poly;

        public int NumSources;

        public bool[] IsSourceMuted;  // 0=muted, 1=not muted - store the inverse here

        public byte AM;  // 0=off, value=1~5(src2~6)

        public EffectControl EffectControl1;
        public EffectControl EffectControl2;

        public bool IsPortamentoEnabled;  // 0=off, 1=on
        public int PortamentoSpeed;  // 0 ~ 127

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
            EffectControl1 = new EffectControl();
            EffectControl2 = new EffectControl();
            Macro1 = new MacroController();
            Macro2 = new MacroController();
            Macro3 = new MacroController();
            Macro4 = new MacroController();
            Poly = PolyphonyMode.Poly;

            NumSources = 1;
            IsSourceMuted = new bool[MaxSources] { false, false, false, false, false, false };
            AM = 0;
            IsPortamentoEnabled = false;
            PortamentoSpeed = 0;
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
            Poly = (PolyphonyMode) b; // 0=POLY, 1=SOLO1, 2=SOLO2

            // "no use" can be ignored
            (b, offset) = Util.GetNextByte(data, offset);

            (b, offset) = Util.GetNextByte(data, offset);
            NumSources = b;

            (b, offset) = Util.GetNextByte(data, offset);
            for (int i = 0; i < MaxSources; i++)
            {
                bool isNotMuted = b.IsBitSet(i);
                IsSourceMuted[i] = ! isNotMuted;
            }

            (b, offset) = Util.GetNextByte(data, offset);
            AM = b;

            byte[] ec1Data = null;
            (ec1Data, offset) = Util.GetNextBytes(data, offset, 3);
            EffectControl1 = new EffectControl(ec1Data);

            byte[] ec2Data = null;
            (ec2Data, offset) = Util.GetNextBytes(data, offset, 3);
            EffectControl2 = new EffectControl(ec2Data);

            (b, offset) = Util.GetNextByte(data, offset);
            IsPortamentoEnabled = (b == 1);

            (b, offset) = Util.GetNextByte(data, offset);
            PortamentoSpeed = b;

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

            Console.WriteLine(String.Format("Common data parsed, offset = {0:X2} ({0})", offset));
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(String.Format("Poly = {0}\n", Poly));
            builder.Append(String.Format("Portamento = {0}, speed = {1}\n", IsPortamentoEnabled ? "ON" : "OFF", PortamentoSpeed));
            builder.Append(String.Format("Sources = {0}\n", NumSources));
            builder.Append(String.Format("AM = {0}\n", (AM == 0) ? "OFF" : Convert.ToString(AM)));
            builder.Append("Macro controllers:\n");
            builder.Append(String.Format("User 1: {0}\n", Macro1.ToString()));
            builder.Append(String.Format("User 2: {0}\n", Macro2.ToString()));
            builder.Append(String.Format("User 3: {0}\n", Macro3.ToString()));
            builder.Append(String.Format("User 4: {0}\n", Macro4.ToString()));
            builder.Append(String.Format("SW1 = {0}  SW2 = {1}  F.SW1 = {2}  F.SW2 = {3}\n", Switch1, Switch2, FootSwitch1, FootSwitch2));
            return builder.ToString();
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();

            data.Add((byte)Poly);
            data.Add(0); // "no use"
            data.Add((byte)NumSources);

            byte sourceMute = 0;
            for (int i = 0; i < MaxSources; i++)
            {
                if (IsSourceMuted[i])
                {
                    sourceMute.SetBit(i);
                }
            }
            data.Add(sourceMute);

            data.Add(AM);

            data.AddRange(EffectControl1.ToData());
            data.AddRange(EffectControl2.ToData());
            
            data.Add((byte)(IsPortamentoEnabled ? 1 : 0));  // only bit 0 is used for this
            data.Add((byte)PortamentoSpeed);

            var m1p1 = Macro1.Param1.Bytes;
            var m1p2 = Macro1.Param2.Bytes;
            var m2p1 = Macro2.Param1.Bytes;
            var m2p2 = Macro2.Param2.Bytes;
            var m3p1 = Macro3.Param1.Bytes;
            var m3p2 = Macro3.Param2.Bytes;
            var m4p1 = Macro4.Param1.Bytes;
            var m4p2 = Macro4.Param2.Bytes;

            data.Add(m1p1.Type);
            data.Add(m1p2.Type);
            data.Add(m2p1.Type);
            data.Add(m2p2.Type);
            data.Add(m3p1.Type);
            data.Add(m3p2.Type);
            data.Add(m4p1.Type);
            data.Add(m3p2.Type);

            data.Add(m1p1.Depth);
            data.Add(m1p2.Depth);
            data.Add(m2p1.Depth);
            data.Add(m2p2.Depth);
            data.Add(m3p1.Depth);
            data.Add(m3p2.Depth);
            data.Add(m4p1.Depth);
            data.Add(m4p2.Depth);

            data.Add((byte)Switch1);
            data.Add((byte)Switch2);
            data.Add((byte)FootSwitch1);
            data.Add((byte)FootSwitch2);

            return data.ToArray();
        }
    }
}