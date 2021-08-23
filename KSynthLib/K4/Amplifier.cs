using System.Text;
using System.Collections.Generic;
using System.IO;

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

        public LevelModulation(List<byte> data)
        {
            // The bytes passed in must be raw SysEx. The DepthType(byte b) constructor
            // adjusts them to the correct range, to avoid repetitive code here.
            _velocityDepth = new DepthType(data[0]);
            _pressureDepth = new DepthType(data[1]);
            _keyScalingDepth = new DepthType(data[2]);
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

        public TimeModulation(List<byte> data)
        {
            _attackVelocity = new DepthType(data[0]);
            _releaseVelocity = new DepthType(data[1]);
            _keyScaling = new DepthType(data[2]);
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

    /// <summary>
    /// Source-specific amplifier settings
    /// <summary>
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

            //
            // Depth values come in as 0...100,
            // they need to be scaled to -50...50.
            //

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
            data.Add(EnvelopeLevel);
            data.AddRange(Env.ToData());
            data.AddRange(LevelMod.ToData());
            data.AddRange(TimeMod.ToData());
            return data.ToArray();
        }
    }
}