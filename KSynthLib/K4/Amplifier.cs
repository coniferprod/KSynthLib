using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K4
{
    public class LevelModulation
    {
        public DepthType VelocityDepth;

        public DepthType PressureDepth;

        public DepthType KeyScalingDepth;

        public LevelModulation()
        {
            VelocityDepth = new DepthType();
            PressureDepth = new DepthType();
            KeyScalingDepth = new DepthType();
        }

        public LevelModulation(int vel, int prs, int ks)
        {
            VelocityDepth = new DepthType(vel);
            PressureDepth = new DepthType(prs);
            KeyScalingDepth = new DepthType(ks);
        }

        public LevelModulation(List<byte> data)
        {
            // The bytes passed in must be raw SysEx. The DepthType(byte b) constructor
            // adjusts them to the correct range, to avoid repetitive code here.
            VelocityDepth = new DepthType(data[0]);
            PressureDepth = new DepthType(data[1]);
            KeyScalingDepth = new DepthType(data[2]);
        }

        public override string ToString()
        {
            return $"VEL DEP = {VelocityDepth}, PRS = {PressureDepth}, KS = {KeyScalingDepth}";
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();

            data.Add(VelocityDepth.ToByte());
            data.Add(PressureDepth.ToByte());
            data.Add(KeyScalingDepth.ToByte());

            return data.ToArray();
        }
    }

    public class TimeModulation
    {
        public DepthType AttackVelocity;
        public DepthType ReleaseVelocity;
        public DepthType KeyScaling;

        public TimeModulation()
        {
            AttackVelocity = new DepthType();
            ReleaseVelocity = new DepthType();
            KeyScaling = new DepthType();
        }

        public TimeModulation(int a, int r, int ks)
        {
            AttackVelocity = new DepthType(a);
            ReleaseVelocity = new DepthType(r);
            KeyScaling = new DepthType(ks);
        }

        public TimeModulation(List<byte> data)
        {
            AttackVelocity = new DepthType(data[0]);
            ReleaseVelocity = new DepthType(data[1]);
            KeyScaling = new DepthType(data[2]);
        }

        public override string ToString()
        {
            return $"ATK VEL = {AttackVelocity}, RLS VEL = {ReleaseVelocity}, KS = {KeyScaling}";
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();

            data.Add(AttackVelocity.ToByte());
            data.Add(ReleaseVelocity.ToByte());
            data.Add(KeyScaling.ToByte());

            return data.ToArray();
        }
    }

    /// <summary>
    /// Source-specific amplifier settings
    /// <summary>
    public class Amplifier
    {
        public const int DataSize = 11;

        public AmplifierEnvelope Env;
        public LevelType EnvelopeLevel;
        public LevelModulation LevelMod;
        public TimeModulation TimeMod;

        public Amplifier()
        {
            Env = new AmplifierEnvelope(0, 0, 0, 0);
            EnvelopeLevel = new LevelType();
            LevelMod = new LevelModulation();
            TimeMod = new TimeModulation();
        }

        public Amplifier(byte[] data) : this()
        {
            int offset = 0;
            byte b = 0;  // will be reused when getting the next byte

            (b, offset) = Util.GetNextByte(data, offset);
            EnvelopeLevel = new LevelType(b);

            List<byte> envBytes = new List<byte>();
            (b, offset) = Util.GetNextByte(data, offset);
            envBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            envBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            envBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            envBytes.Add(b);
            Env = new AmplifierEnvelope(envBytes);

            List<byte> levelModBytes = new List<byte>();
            (b, offset) = Util.GetNextByte(data, offset);
            levelModBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            levelModBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            levelModBytes.Add(b);
            LevelMod = new LevelModulation(levelModBytes);

            // Same goes for the time modulation values:

            List<byte> timeModBytes = new List<byte>();
            (b, offset) = Util.GetNextByte(data, offset);
            timeModBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            timeModBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            timeModBytes.Add(b);
            TimeMod = new TimeModulation(timeModBytes);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"envelope = {Env}, level = {EnvelopeLevel}\n");
            builder.Append($"level modulation: {LevelMod}\n");
            builder.Append($"time modulation: {TimeMod}\n");
            return builder.ToString();
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();

            data.Add(EnvelopeLevel.ToByte());
            data.AddRange(Env.ToData());
            data.AddRange(LevelMod.ToData());
            data.AddRange(TimeMod.ToData());

            return data.ToArray();
        }
    }
}
