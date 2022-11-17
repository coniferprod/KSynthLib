using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K5000
{

    public class DCASettings: ISystemExclusiveData
    {
        public VelocityCurve VelocityCurve;
        public AmplifierEnvelope Envelope;
        public KeyScalingControlEnvelope KeyScaling;
        public VelocityControlEnvelope VelocitySensitivity;

        public DCASettings()
        {
            KeyScaling = new KeyScalingControlEnvelope();
            VelocitySensitivity = new VelocityControlEnvelope();
            VelocityCurve = VelocityCurve.Curve6;
            Envelope = new AmplifierEnvelope(20, 95, 127, 110, 127, 11);
        }

        public DCASettings(byte[] data, int offset)
        {
            byte b = 0;  // will be reused when getting the next byte

            (b, offset) = Util.GetNextByte(data, offset);
            VelocityCurve = (VelocityCurve)b;

            var envBytes = new List<byte>();
            (b, offset) = Util.GetNextByte(data, offset);
            envBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            envBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            envBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            envBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            envBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            envBytes.Add(b);
            Envelope = new AmplifierEnvelope(envBytes);

            var ksEnvBytes = new List<byte>();
            KeyScaling = new KeyScalingControlEnvelope();
            (b, offset) = Util.GetNextByte(data, offset);
            ksEnvBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            ksEnvBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            ksEnvBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            ksEnvBytes.Add(b);
            KeyScaling = new KeyScalingControlEnvelope(ksEnvBytes);

            var velEnvBytes = new List<byte>();
            VelocitySensitivity = new VelocityControlEnvelope();
            (b, offset) = Util.GetNextByte(data, offset);
            velEnvBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            velEnvBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            velEnvBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            velEnvBytes.Add(b);
            VelocitySensitivity = new VelocityControlEnvelope(velEnvBytes);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append("                   DCA Envelope\n");
            builder.Append($"VelCrv      {VelocityCurve,3}   Dcy2 T    {Envelope.Decay2Time.Value,3}\n");
            builder.Append($"Atak T      {Envelope.AttackTime.Value,3}   Dcy2 L   {Envelope.Decay2Level.Value,3}\n");
            builder.Append($"Dcy1 T      {Envelope.Decay1Time.Value,3}   Rels T   {Envelope.ReleaseTime.Value,3}\n");
            builder.Append($"Dcy1 L      {Envelope.Decay1Level.Value,3}\n");

            builder.Append("                   DCA Modulation\n");
            builder.Append("  KS TO DCA ENV       VELO TO DCA ENV\n");
            builder.Append($"Level         {KeyScaling.Level.Value,3}     Level   {VelocitySensitivity.Level.Value,3}\n");
            builder.Append($"Attack Time   {KeyScaling.AttackTime.Value,3}    Attack Time    {VelocitySensitivity.AttackTime.Value,3}\n");
            builder.Append($"Decay1 Time   {KeyScaling.Decay1Time.Value,3}    Decay1 Time    {VelocitySensitivity.Decay1Time.Value,3}\n");
            builder.Append($"Release       {KeyScaling.ReleaseTime.Value,3}   Release        {VelocitySensitivity.ReleaseTime.Value,3}\n");

            return builder.ToString();
        }

        public List<byte> GetSystemExclusiveData()
        {
            var data = new List<byte>();

            data.Add((byte)(VelocityCurve - 1));  // adjust from 1~12 to 0~11

            data.AddRange(Envelope.ToData());
            data.AddRange(KeyScaling.ToData());
            data.AddRange(VelocitySensitivity.ToData());

            return data;
        }
    }
}
