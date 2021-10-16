using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K5000
{

    public class DCASettings
    {
        private VelocityCurveType _velocityCurve; // values are 0 ~ 11, shown as 1 ~ 12
        public byte VelocityCurve
        {
            get => _velocityCurve.Value;
            set => _velocityCurve.Value = value;
        }

        public AmplifierEnvelope Envelope;
        public KeyScalingControlEnvelope KeyScaling;
        public VelocityControlEnvelope VelocitySensitivity;

        public DCASettings()
        {
            KeyScaling = new KeyScalingControlEnvelope();
            VelocitySensitivity = new VelocityControlEnvelope();

            _velocityCurve = new VelocityCurveType(5);

            Envelope = new AmplifierEnvelope
            {
                AttackTime = 20,
                Decay1Time = 95,
                Decay1Level = 127,
                Decay2Time = 110,
                Decay2Level = 127,
                ReleaseTime = 11
            };
            // Note that an object initializer invokes the default constructor,
            // not the one with six arguments
        }

        public DCASettings(byte[] data, int offset)
        {
            byte b = 0;  // will be reused when getting the next byte

            (b, offset) = Util.GetNextByte(data, offset);
            VelocityCurve = (byte)(b + 1);  // adjust from 0~11 to 1~12

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
            builder.Append($"VelCrv      {VelocityCurve,3}   Dcy2 T    {Envelope.Decay2Time,3}\n");
            builder.Append($"Atak T      {Envelope.AttackTime,3}   Dcy2 L   {Envelope.Decay2Level,3}\n");
            builder.Append($"Dcy1 T      {Envelope.Decay1Time,3}   Rels T   {Envelope.ReleaseTime,3}\n");
            builder.Append($"Dcy1 L      {Envelope.Decay1Level,3}\n");

            builder.Append("                   DCA Modulation\n");
            builder.Append("  KS TO DCA ENV       VELO TO DCA ENV\n");
            builder.Append($"Level         {KeyScaling.Level,3}     Level   {VelocitySensitivity.Level,3}\n");
            builder.Append($"Attack Time   {KeyScaling.AttackTime,3}    Attack Time    {VelocitySensitivity.AttackTime,3}\n");
            builder.Append($"Decay1 Time   {KeyScaling.Decay1Time,3}    Decay1 Time    {VelocitySensitivity.Decay1Time,3}\n");
            builder.Append($"Release       {KeyScaling.ReleaseTime,3}   Release        {VelocitySensitivity.ReleaseTime,3}\n");

            return builder.ToString();
        }

        public byte[] ToData()
        {
            var data = new List<byte>();

            data.Add((byte)(VelocityCurve - 1));  // adjust from 1~12 to 0~11

            data.AddRange(Envelope.ToData());
            data.AddRange(KeyScaling.ToData());
            data.AddRange(VelocitySensitivity.ToData());

            return data.ToArray();
        }
    }
}
