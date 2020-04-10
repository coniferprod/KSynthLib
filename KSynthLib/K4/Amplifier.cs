using System;
using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K4
{
    public class LevelModulation
    {
        private DepthType velocityDepth;
        public int VelocityDepth
        {
            get => velocityDepth.Value;
            set => velocityDepth.Value = value;
        }

        private DepthType pressureDepth;
        public int PressureDepth // 0~100 (±50)
        {
            get => pressureDepth.Value;
            set => pressureDepth.Value = value;
        }

        private DepthType keyScalingDepth;
        public int KeyScalingDepth // 0~100 (±50)
        {
            get => keyScalingDepth.Value;
            set => keyScalingDepth.Value = value;
        }

        public LevelModulation()
        {
            velocityDepth = new DepthType();
            pressureDepth = new DepthType();
            keyScalingDepth = new DepthType();
        }

        public LevelModulation(int vel, int prs, int ks)
        {
            velocityDepth = new DepthType(vel);
            pressureDepth = new DepthType(prs);
            keyScalingDepth = new DepthType(ks);
        }

        public override string ToString()
        {
            return $"VEL DEP = {VelocityDepth}, PRS = {PressureDepth}, KS = {KeyScalingDepth}";
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();

            // The depth values are (±50), so they need to be scaled back to 0-100.
            data.Add((byte)(VelocityDepth + 50));
            data.Add((byte)(PressureDepth + 50));
            data.Add((byte)(KeyScalingDepth + 50));

            return data.ToArray();
        }
    }

    public class TimeModulation
    {
        private DepthType attackVelocity;
        public int AttackVelocity // 0~100 (±50)
        {
            get => attackVelocity.Value;
            set => attackVelocity.Value = value;
        }

        private DepthType releaseVelocity;
        public int ReleaseVelocity // 0~100 (±50)
        {
            get => releaseVelocity.Value;
            set => releaseVelocity.Value = value;
        }

        private DepthType keyScaling;
        public int KeyScaling // 0~100 (±50)
        {
            get => keyScaling.Value;
            set => keyScaling.Value = value;
        }

        public TimeModulation()
        {
            attackVelocity = new DepthType();
            releaseVelocity = new DepthType();
            keyScaling = new DepthType();
        }

        public TimeModulation(int a, int r, int ks)
        {
            attackVelocity = new DepthType(a);
            releaseVelocity = new DepthType(r);
            keyScaling = new DepthType(ks);
        }

        public override string ToString()
        {
            return $"ATK VEL = {AttackVelocity}, RLS VEL = {ReleaseVelocity}, KS = {KeyScaling}";
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();

            // The values are (±50), so they need to be scaled to 0-100.
            data.Add((byte)(AttackVelocity + 50));
            data.Add((byte)(ReleaseVelocity + 50));
            data.Add((byte)(KeyScaling + 50));

            return data.ToArray();
        }
    }

    // Source-specific amplifier settings
    public class Amplifier
    {
        public const int DataSize = 11;

        public Envelope Env;

        private LevelType envelopeLevel;
        public int EnvelopeLevel // 0~100
        {
            get => envelopeLevel.Value;
            set => envelopeLevel.Value = value;
        }

        public LevelModulation LevelMod;
        public TimeModulation TimeMod;

        public Amplifier()
        {
            Env = new Envelope(0, 0, 0, 0);
            envelopeLevel = new LevelType();
            LevelMod = new LevelModulation(0, 0, 0);
            TimeMod = new TimeModulation(0, 0, 0);
        }

        public Amplifier(byte[] data)
        {
            Env = new Envelope(0, 0, 0, 0);
            LevelMod = new LevelModulation(0, 0, 0);
            TimeMod = new TimeModulation(0, 0, 0);

            int offset = 0;
            byte b = 0;  // will be reused when getting the next byte

            (b, offset) = Util.GetNextByte(data, offset);
            envelopeLevel = new LevelType(b & 0x7f);

            (b, offset) = Util.GetNextByte(data, offset);
            Env.Attack = b & 0x7f;

            (b, offset) = Util.GetNextByte(data, offset);
            Env.Decay = b & 0x7f;

            (b, offset) = Util.GetNextByte(data, offset);
            Env.Sustain = b & 0x7f;

            (b, offset) = Util.GetNextByte(data, offset);
            Env.Release = b & 0x7f;

            //
            // Depth values come in as 0...100,
            // they need to be scaled to -50...50.
            //

            (b, offset) = Util.GetNextByte(data, offset);
            LevelMod.VelocityDepth = (b & 0x7f) - 50;

            (b, offset) = Util.GetNextByte(data, offset);
            LevelMod.PressureDepth = (b & 0x7f) - 50;

            (b, offset) = Util.GetNextByte(data, offset);
            LevelMod.KeyScalingDepth = (b & 0x7f) - 50;

            // Same goes for the time modulation values:

            (b, offset) = Util.GetNextByte(data, offset);
            TimeMod.AttackVelocity = (b & 0x7f) - 50;

            (b, offset) = Util.GetNextByte(data, offset);
            TimeMod.ReleaseVelocity = (b & 0x7f) - 50;

            (b, offset) = Util.GetNextByte(data, offset);
            TimeMod.KeyScaling = (b & 0x7f) - 50;
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
            data.Add((byte)EnvelopeLevel);
            data.AddRange(Env.ToData());
            data.AddRange(LevelMod.ToData());
            data.AddRange(TimeMod.ToData());
            return data.ToArray();
        }
    }
}