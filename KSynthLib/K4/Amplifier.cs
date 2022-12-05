using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;


namespace KSynthLib.K4
{
    public class LevelModulation: ISystemExclusiveData
    {
        public Depth VelocityDepth;
        public Depth PressureDepth;
        public Depth KeyScalingDepth;

        public LevelModulation()
        {
            VelocityDepth = new Depth();
            PressureDepth = new Depth();
            KeyScalingDepth = new Depth();
        }

        public LevelModulation(int vel, int prs, int ks)
        {
            VelocityDepth = new Depth(vel);
            PressureDepth = new Depth(prs);
            KeyScalingDepth = new Depth(ks);
        }

        public LevelModulation(List<byte> data)
        {
            // The bytes passed in must be raw SysEx. The Depth(byte) constructor
            // adjusts them to the correct range, to avoid repetitive code here.
            VelocityDepth = new Depth(data[0]);
            PressureDepth = new Depth(data[1]);
            KeyScalingDepth = new Depth(data[2]);
        }

        public override string ToString()
        {
            return $"VEL DEP = {VelocityDepth}, PRS = {PressureDepth}, KS = {KeyScalingDepth}";
        }

        public List<byte> GetSystemExclusiveData()
        {
            var data = new List<byte>();

            data.Add(VelocityDepth.ToByte());
            data.Add(PressureDepth.ToByte());
            data.Add(KeyScalingDepth.ToByte());

            return data;
        }
    }

    public class TimeModulation: ISystemExclusiveData
    {
        public Depth AttackVelocity;
        public Depth ReleaseVelocity;
        public Depth KeyScaling;

        public TimeModulation()
        {
            AttackVelocity = new Depth();
            ReleaseVelocity = new Depth();
            KeyScaling = new Depth();
        }

        public TimeModulation(int a, int r, int ks)
        {
            AttackVelocity = new Depth(a);
            ReleaseVelocity = new Depth(r);
            KeyScaling = new Depth(ks);
        }

        public TimeModulation(List<byte> data)
        {
            AttackVelocity = new Depth(data[0]);
            ReleaseVelocity = new Depth(data[1]);
            KeyScaling = new Depth(data[2]);
        }

        public override string ToString()
        {
            return $"ATK VEL = {AttackVelocity}, RLS VEL = {ReleaseVelocity}, KS = {KeyScaling}";
        }

        public List<byte> GetSystemExclusiveData()
        {
            var data = new List<byte>();

            data.Add(AttackVelocity.ToByte());
            data.Add(ReleaseVelocity.ToByte());
            data.Add(KeyScaling.ToByte());

            return data;
        }
    }

    /// <summary>
    /// Source-specific amplifier settings
    /// <summary>
    public class Amplifier: ISystemExclusiveData
    {
        public const int DataSize = 11;

        public AmplifierEnvelope Env;
        public Level EnvelopeLevel;
        public LevelModulation LevelMod;
        public TimeModulation TimeMod;

        public Amplifier()
        {
            Env = new AmplifierEnvelope(0, 0, 0, 0);
            EnvelopeLevel = new Level();
            LevelMod = new LevelModulation();
            TimeMod = new TimeModulation();
        }

        public Amplifier(byte[] data) : this()
        {
            int offset = 0;
            byte b = 0;  // will be reused when getting the next byte

            (b, offset) = Util.GetNextByte(data, offset);
            EnvelopeLevel = new Level(b);

            var envBytes = new List<byte>();
            (b, offset) = Util.GetNextByte(data, offset);
            envBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            envBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            envBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            envBytes.Add(b);
            Env = new AmplifierEnvelope(envBytes);

            var levelModBytes = new List<byte>();
            (b, offset) = Util.GetNextByte(data, offset);
            levelModBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            levelModBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            levelModBytes.Add(b);
            LevelMod = new LevelModulation(levelModBytes);

            // Same goes for the time modulation values:

            var timeModBytes = new List<byte>();
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
            var builder = new StringBuilder();
            builder.Append($"envelope = {Env}, level = {EnvelopeLevel}\n");
            builder.Append($"level modulation: {LevelMod}\n");
            builder.Append($"time modulation: {TimeMod}\n");
            return builder.ToString();
        }

        public List<byte> GetSystemExclusiveData()
        {
            var data = new List<byte>();

            data.Add(EnvelopeLevel.ToByte());
            data.AddRange(Env.GetSystemExclusiveData());
            data.AddRange(LevelMod.GetSystemExclusiveData());
            data.AddRange(TimeMod.GetSystemExclusiveData());

            return data;
        }
    }
}
