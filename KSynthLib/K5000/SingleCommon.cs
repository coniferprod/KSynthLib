using System;
using System.Text;
using System.Collections.Generic;

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
        public const int MaxSources = 6;

        public PolyphonyMode Poly;

        public int NumSources;

        public bool[] IsSourceMuted;  // 0=muted, 1=not muted - store the inverse here

        public byte AM;  // 0=off, value=1~5(src2~6)

        public EffectControl EffectControl1;
        public EffectControl EffectControl2;

        public bool IsPortamentoEnabled;  // 0=off, 1=on
        public int PortamentoSpeed;  // 0 ~ 127

        // TODO: Add macro controllers
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
            IsSourceMuted = new bool[MaxSources];
            EffectControl1 = new EffectControl();
            EffectControl2 = new EffectControl();
            Macro1 = new MacroController();
            Macro2 = new MacroController();
            Macro3 = new MacroController();
            Macro4 = new MacroController();
        }

        public SingleCommonSettings(byte[] data)
        {
            IsSourceMuted = new bool[MaxSources];
            EffectControl1 = new EffectControl();
            EffectControl2 = new EffectControl();
            Macro1 = new MacroController();
            Macro2 = new MacroController();
            Macro3 = new MacroController();
            Macro4 = new MacroController();

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

            (b, offset) = Util.GetNextByte(data, offset);
            EffectControl1.Source = (ControlSource)b;
            (b, offset) = Util.GetNextByte(data, offset);
            EffectControl1.Destination = (EffectDestination)b;
            (b, offset) = Util.GetNextByte(data, offset);
            EffectControl1.Depth = (sbyte)(b - 64);

            (b, offset) = Util.GetNextByte(data, offset);
            EffectControl2.Source = (ControlSource)b;
            (b, offset) = Util.GetNextByte(data, offset);
            EffectControl2.Destination = (EffectDestination)b;
            (b, offset) = Util.GetNextByte(data, offset);
            EffectControl2.Depth = (sbyte)(b - 64);

            (b, offset) = Util.GetNextByte(data, offset);
            IsPortamentoEnabled = (b == 1);

            (b, offset) = Util.GetNextByte(data, offset);
            PortamentoSpeed = b;

            (b, offset) = Util.GetNextByte(data, offset);
            Macro1.Param1.Type = (MacroControllerType)b;
            (b, offset) = Util.GetNextByte(data, offset);
            Macro1.Param2.Type = (MacroControllerType)b;

            (b, offset) = Util.GetNextByte(data, offset);
            Macro2.Param1.Type = (MacroControllerType)b;
            (b, offset) = Util.GetNextByte(data, offset);
            Macro2.Param2.Type = (MacroControllerType)b;

            (b, offset) = Util.GetNextByte(data, offset);
            Macro3.Param1.Type = (MacroControllerType)b;
            (b, offset) = Util.GetNextByte(data, offset);
            Macro3.Param2.Type = (MacroControllerType)b;

            (b, offset) = Util.GetNextByte(data, offset);
            Macro4.Param1.Type = (MacroControllerType)b;
            (b, offset) = Util.GetNextByte(data, offset);
            Macro4.Param2.Type = (MacroControllerType)b;

            (b, offset) = Util.GetNextByte(data, offset);
            Macro1.Param1.Depth = (sbyte)(b - 64);
            (b, offset) = Util.GetNextByte(data, offset);
            Macro1.Param2.Depth = (sbyte)(b - 64);

            (b, offset) = Util.GetNextByte(data, offset);
            Macro2.Param1.Depth = (sbyte)(b - 64);
            (b, offset) = Util.GetNextByte(data, offset);
            Macro2.Param2.Depth = (sbyte)(b - 64);

            (b, offset) = Util.GetNextByte(data, offset);
            Macro3.Param1.Depth = (sbyte)(b - 64);
            (b, offset) = Util.GetNextByte(data, offset);
            Macro3.Param2.Depth = (sbyte)(b - 64);

            (b, offset) = Util.GetNextByte(data, offset);
            Macro4.Param1.Depth = (sbyte)(b - 64);
            (b, offset) = Util.GetNextByte(data, offset);
            Macro4.Param2.Depth = (sbyte)(b - 64);

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

            data.Add((byte)Macro1.Param1.Type);
            data.Add((byte)Macro1.Param2.Type);
            data.Add((byte)Macro2.Param1.Type);
            data.Add((byte)Macro2.Param2.Type);
            data.Add((byte)Macro3.Param1.Type);
            data.Add((byte)Macro3.Param2.Type);
            data.Add((byte)Macro4.Param1.Type);
            data.Add((byte)Macro4.Param2.Type);

            data.Add((byte)(Macro1.Param1.Depth + 64));
            data.Add((byte)(Macro1.Param2.Depth + 64));
            data.Add((byte)(Macro2.Param1.Depth + 64));
            data.Add((byte)(Macro2.Param2.Depth + 64));
            data.Add((byte)(Macro3.Param1.Depth + 64));
            data.Add((byte)(Macro3.Param2.Depth + 64));
            data.Add((byte)(Macro4.Param1.Depth + 64));
            data.Add((byte)(Macro4.Param2.Depth + 64));

            data.Add((byte)Switch1);
            data.Add((byte)Switch2);
            data.Add((byte)FootSwitch1);
            data.Add((byte)FootSwitch2);

            return data.ToArray();
        }
    }
}