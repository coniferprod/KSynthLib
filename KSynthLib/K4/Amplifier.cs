using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using KSynthLib.Common;


namespace KSynthLib.K4
{
    public class LevelModulation: ISystemExclusiveData
    {
        [Range(-50, 50, ErrorMessage = "{0} must be between {1} and {2}")]
        public int VelocityDepth;

        [Range(-50, 50, ErrorMessage = "{0} must be between {1} and {2}")]
        public int PressureDepth;

        [Range(-50, 50, ErrorMessage = "{0} must be between {1} and {2}")]
        public int KeyScalingDepth;

        public LevelModulation()
        {
            VelocityDepth = 0;
            PressureDepth = 0;
            KeyScalingDepth = 0;
        }

        public LevelModulation(int velocity, int pressure, int keyScaling)
        {
            VelocityDepth = velocity;
            PressureDepth = pressure;
            KeyScalingDepth = keyScaling;
        }

        public LevelModulation(List<byte> data)
        {
            // The bytes passed in must be raw SysEx.
            // Use the appropriate SystemExclusiveDataConverter method
            // to adjust them to the correct range.
            VelocityDepth = SystemExclusiveDataConverter.DepthFromByte(data[0]);
            PressureDepth = SystemExclusiveDataConverter.DepthFromByte(data[1]);
            KeyScalingDepth = SystemExclusiveDataConverter.DepthFromByte(data[2]);
        }

        public override string ToString()
        {
            return $"VEL DEP = {VelocityDepth}, PRS = {PressureDepth}, KS = {KeyScalingDepth}";
        }

        public List<byte> GetSystemExclusiveData()
        {
            var data = new List<byte>();

            data.Add(SystemExclusiveDataConverter.ByteFromDepth(VelocityDepth));
            data.Add(SystemExclusiveDataConverter.ByteFromDepth(PressureDepth));
            data.Add(SystemExclusiveDataConverter.ByteFromDepth(KeyScalingDepth));

            return data;
        }
    }

    public class TimeModulation: ISystemExclusiveData
    {
        [Range(-50, 50, ErrorMessage = "{0} must be between {1} and {2}")]
        public int AttackVelocity;

        [Range(-50, 50, ErrorMessage = "{0} must be between {1} and {2}")]
        public int ReleaseVelocity;

        [Range(-50, 50, ErrorMessage = "{0} must be between {1} and {2}")]
        public int KeyScaling;

        public TimeModulation()
        {
            AttackVelocity = 0;
            ReleaseVelocity = 0;
            KeyScaling = 0;
        }

        public TimeModulation(int a, int r, int ks)
        {
            AttackVelocity = a;
            ReleaseVelocity = r;
            KeyScaling = ks;
        }

        public TimeModulation(List<byte> data)
        {
            AttackVelocity = SystemExclusiveDataConverter.DepthFromByte(data[0]);
            ReleaseVelocity = SystemExclusiveDataConverter.DepthFromByte(data[1]);
            KeyScaling = SystemExclusiveDataConverter.DepthFromByte(data[2]);
        }

        public override string ToString()
        {
            return $"ATK VEL = {AttackVelocity}, RLS VEL = {ReleaseVelocity}, KS = {KeyScaling}";
        }

        public List<byte> GetSystemExclusiveData()
        {
            var data = new List<byte>();

            data.Add(SystemExclusiveDataConverter.ByteFromDepth(AttackVelocity));
            data.Add(SystemExclusiveDataConverter.ByteFromDepth(ReleaseVelocity));
            data.Add(SystemExclusiveDataConverter.ByteFromDepth(KeyScaling));

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

        [Range(0, 100, ErrorMessage = "{0} must be between {1} and {2}")]
        public int EnvelopeLevel;

        public LevelModulation LevelMod;
        public TimeModulation TimeMod;

        public Amplifier()
        {
            Env = new AmplifierEnvelope(0, 0, 0, 0);
            EnvelopeLevel = 0;
            LevelMod = new LevelModulation();
            TimeMod = new TimeModulation();
        }

        public Amplifier(byte[] data)
        {
            int offset = 0;
            byte b = 0;  // will be reused when getting the next byte

            (b, offset) = Util.GetNextByte(data, offset);
            EnvelopeLevel = b;

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

            builder.AppendLine($"envelope = {Env}, level = {EnvelopeLevel}");
            builder.AppendLine($"level modulation: {LevelMod}");
            builder.AppendLine($"time modulation: {TimeMod}");

            return builder.ToString();
        }

        public List<byte> GetSystemExclusiveData()
        {
            var data = new List<byte>();

            data.Add((byte)EnvelopeLevel);
            data.AddRange(Env.GetSystemExclusiveData());
            data.AddRange(LevelMod.GetSystemExclusiveData());
            data.AddRange(TimeMod.GetSystemExclusiveData());

            return data;
        }
    }
}
