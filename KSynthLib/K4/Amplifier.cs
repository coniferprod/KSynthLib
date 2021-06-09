using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K4
{
    public class LevelModulation
    {
        private DepthType _velocityDepth;
        public sbyte VelocityDepth
        {
            get => _velocityDepth.Value;
            set => _velocityDepth.Value = value;
        }

        private DepthType _pressureDepth;
        public sbyte PressureDepth // 0~100 (±50)
        {
            get => _pressureDepth.Value;
            set => _pressureDepth.Value = value;
        }

        private DepthType _keyScalingDepth;
        public sbyte KeyScalingDepth // 0~100 (±50)
        {
            get => _keyScalingDepth.Value;
            set => _keyScalingDepth.Value = value;
        }

        public LevelModulation()
        {
            _velocityDepth = new DepthType();
            _pressureDepth = new DepthType();
            _keyScalingDepth = new DepthType();
        }

        public LevelModulation(sbyte vel, sbyte prs, sbyte ks)
        {
            _velocityDepth = new DepthType(vel);
            _pressureDepth = new DepthType(prs);
            _keyScalingDepth = new DepthType(ks);
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
        private DepthType _attackVelocity;
        public sbyte AttackVelocity // 0~100 (±50)
        {
            get => _attackVelocity.Value;
            set => _attackVelocity.Value = value;
        }

        private DepthType _releaseVelocity;
        public sbyte ReleaseVelocity // 0~100 (±50)
        {
            get => _releaseVelocity.Value;
            set => _releaseVelocity.Value = value;
        }

        private DepthType _keyScaling;
        public sbyte KeyScaling // 0~100 (±50)
        {
            get => _keyScaling.Value;
            set => _keyScaling.Value = value;
        }

        public TimeModulation()
        {
            _attackVelocity = new DepthType();
            _releaseVelocity = new DepthType();
            _keyScaling = new DepthType();
        }

        public TimeModulation(sbyte a, sbyte r, sbyte ks)
        {
            _attackVelocity = new DepthType(a);
            _releaseVelocity = new DepthType(r);
            _keyScaling = new DepthType(ks);
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

        public AmplifierEnvelope Env;

        private LevelType envelopeLevel;
        public byte EnvelopeLevel // 0~100
        {
            get => envelopeLevel.Value;
            set => envelopeLevel.Value = value;
        }

        public LevelModulation LevelMod;
        public TimeModulation TimeMod;

        public Amplifier()
        {
            Env = new AmplifierEnvelope(0, 0, 0, 0);
            envelopeLevel = new LevelType();
            LevelMod = new LevelModulation(0, 0, 0);
            TimeMod = new TimeModulation(0, 0, 0);
        }

        public Amplifier(byte[] data) : this()
        {
            int offset = 0;
            byte b = 0;  // will be reused when getting the next byte

            (b, offset) = Util.GetNextByte(data, offset);
            envelopeLevel = new LevelType((byte)(b & 0x7f));

            (b, offset) = Util.GetNextByte(data, offset);
            Env.Attack = (byte)(b & 0x7f);

            (b, offset) = Util.GetNextByte(data, offset);
            Env.Decay = (byte)(b & 0x7f);

            (b, offset) = Util.GetNextByte(data, offset);
            Env.Sustain = (byte)(b & 0x7f);

            (b, offset) = Util.GetNextByte(data, offset);
            Env.Release = (byte)(b & 0x7f);

            //
            // Depth values come in as 0...100,
            // they need to be scaled to -50...50.
            //

            (b, offset) = Util.GetNextByte(data, offset);
            LevelMod.VelocityDepth = (sbyte)((b & 0x7f) - 50);

            (b, offset) = Util.GetNextByte(data, offset);
            LevelMod.PressureDepth = (sbyte)((b & 0x7f) - 50);

            (b, offset) = Util.GetNextByte(data, offset);
            LevelMod.KeyScalingDepth = (sbyte)((b & 0x7f) - 50);

            // Same goes for the time modulation values:

            (b, offset) = Util.GetNextByte(data, offset);
            TimeMod.AttackVelocity = (sbyte)((b & 0x7f) - 50);

            (b, offset) = Util.GetNextByte(data, offset);
            TimeMod.ReleaseVelocity = (sbyte)((b & 0x7f) - 50);

            (b, offset) = Util.GetNextByte(data, offset);
            TimeMod.KeyScaling = (sbyte)((b & 0x7f) - 50);
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