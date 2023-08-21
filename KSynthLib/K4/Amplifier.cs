using System.Text;
using System.Collections.Generic;

using SyxPack;
using KSynthLib.Common;


namespace KSynthLib.K4
{
    public class LevelModulation : ISystemExclusiveData
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

        public LevelModulation(int velocity, int pressure, int keyScaling)
        {
            VelocityDepth = new Depth(velocity);
            PressureDepth = new Depth(pressure);
            KeyScalingDepth = new Depth(keyScaling);
        }

        public LevelModulation(List<byte> data) : this(data[0], data[1], data[2]) { }

        public override string ToString()
        {
            return $"VEL DEP = {VelocityDepth}, PRS = {PressureDepth}, KS = {KeyScalingDepth}";
        }

        //
        // ISystemExclusiveData implementation
        //

        public List<byte> Data
        {
            get
            {
                var data = new List<byte>();

                data.Add(this.VelocityDepth.ToByte());
                data.Add(this.PressureDepth.ToByte());
                data.Add(this.KeyScalingDepth.ToByte());

                return data;
            }
        }

        public int DataLength => 3;
    }

    public class TimeModulation : ISystemExclusiveData
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

        public TimeModulation(List<byte> data) : this(data[0], data[1], data[2]) { }

        public override string ToString()
        {
            return $"ATK VEL = {AttackVelocity}, RLS VEL = {ReleaseVelocity}, KS = {KeyScaling}";
        }

        //
        // ISystemExclusiveData implementation
        //

        public List<byte> Data
        {
            get
            {
                var data = new List<byte>();

                data.Add(this.AttackVelocity.ToByte());
                data.Add(this.ReleaseVelocity.ToByte());
                data.Add(this.KeyScaling.ToByte());

                return data;
            }
        }

        public int DataLength => 3;
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

        public Amplifier(byte[] data)
        {
            byte b;  // will be reused when getting the next byte
            int offset = 0;

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

            builder.AppendLine($"envelope = {Env}, level = {EnvelopeLevel}");
            builder.AppendLine($"level modulation: {LevelMod}");
            builder.AppendLine($"time modulation: {TimeMod}");

            return builder.ToString();
        }

        //
        // ISystemExclusiveData implementation
        //

        public List<byte> Data
        {
            get
            {
                var data = new List<byte>();

                data.Add(EnvelopeLevel.ToByte());
                data.AddRange(Env.Data);
                data.AddRange(LevelMod.Data);
                data.AddRange(TimeMod.Data);

                return data;
            }
        }

        public int DataLength => 1 + Env.DataLength + LevelMod.DataLength + TimeMod.DataLength;
    }
}
