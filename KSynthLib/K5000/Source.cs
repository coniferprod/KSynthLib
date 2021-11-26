using System;
using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K5000
{
    public enum VelocitySwitchType
    {
        Off,
        Loud,
        Soft
    }

    public class VelocitySwitchSettings
    {
        public VelocitySwitchType SwitchType;  // enumeration

        // Get the value on input as conversionTable{n] (where n = bottom 5 bits of value),
        // and on output as IndexOf(threshold).
        private static byte[] conversionTable =
        {
            4, 8, 12, 16, 20, 24, 28, 32,
            36, 40, 44, 48, 52, 56, 60, 64,
            68, 72, 76, 80, 84, 88, 92, 96,
            100, 104, 108, 112, 116, 120, 124, 127
        };

        // The velocity switch threshold is specified like this:
        // "velo:0=4 ~ 31=127". What does that even mean?
        private VelocityThresholdType _threshold; // 31 ~ 127
        public byte Threshold
        {
            get => VelocitySwitchSettings.conversionTable[_threshold.Value];
            set => _threshold.Value = VelocitySwitchSettings.conversionTable[value];
        }

        public VelocitySwitchSettings()
        {
            SwitchType = VelocitySwitchType.Off;
            _threshold = new VelocityThresholdType(31);
        }
    }

    public class ModulationSettings
    {
        public ControlDestination Destination;  // enumeration

        private MacroDepthType _depth;
        public sbyte Depth
        {
            get => _depth.Value;
            set => _depth.Value = value;
        }

        public ModulationSettings()
        {
            Destination = ControlDestination.Level;
            _depth = new MacroDepthType();
        }

        public ModulationSettings(byte destination, byte depth)
        {
            Destination = (ControlDestination)destination;
            _depth = new MacroDepthType(depth);
        }

        public byte[] ToData() => new List<byte>() { (byte)Destination, _depth.AsByte() }.ToArray();
    }

    public class ControllerSettings
    {
        public ModulationSettings Destination1;
        public ModulationSettings Destination2;

        public ControllerSettings()
        {
            Destination1 = new ModulationSettings();
            Destination2 = new ModulationSettings();
        }

        public ControllerSettings(List<byte> data)
        {
            Destination1 = new ModulationSettings(data[0], data[1]);
            Destination2 = new ModulationSettings(data[2], data[3]);
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Destination1.ToData());
            data.AddRange(Destination2.ToData());
            return data.ToArray();
        }
    }

    public class AssignableController
    {
        public ControlSource Source;
        public ModulationSettings Target;

        public AssignableController()
        {
            Source = ControlSource.Bender;
            Target = new ModulationSettings();
        }

        public byte[] ToData()
        {
            var data = new List<byte>();
            data.Add((byte)Source);
            data.AddRange(Target.ToData());
            return data.ToArray();
        }
    }

    public enum PanType
    {
        Normal,
        KeyScaling,
        NegativeKeyScaling,
        Random
    }

    /// <summary>
    /// Represents a source in a single patch.
    /// </summary>
    public class Source
    {
        public static int DataSize = 86;

        private PositiveLevelType _volume;
        public byte Volume
        {
            get => _volume.Value;
            set => _volume.Value = value;
        }

        private PositiveLevelType _zoneLow;
        public byte ZoneLow
        {
            get => _zoneLow.Value;
            set => _zoneLow.Value = value;
        }

        private PositiveLevelType _zoneHigh;
        public byte ZoneHigh
        {
            get => _zoneHigh.Value;
            set => _zoneHigh.Value = value;
        }

        public VelocitySwitchSettings VelocitySwitch;

        private EffectPathType _effectPath;
        public byte EffectPath
        {
            get => _effectPath.Value;
            set => _effectPath.Value = value;
        }

        private BenderPitchType _benderPitch;
        public byte BenderPitch
        {
            get => _benderPitch.Value;
            set => _benderPitch.Value = value;
        }

        private BenderCutoffType _benderCutoff;
        public byte BenderCutoff
        {
            get => _benderCutoff.Value;
            set => _benderCutoff.Value = value;
        }

        public ControllerSettings Press;
        public ControllerSettings Wheel;
        public ControllerSettings Express;
        public AssignableController Assign1;
        public AssignableController Assign2;

        private PositiveLevelType _keyOnDelay;
        public byte KeyOnDelay
        {
            get => _keyOnDelay.Value;
            set => _keyOnDelay.Value = value;
        }

        public PanType Pan;  // enumeration

        private SignedLevelType _panValue;  // (63L)1 ~ (63R)127
        public sbyte PanValue
        {
            get => _panValue.Value;
            set => _panValue.Value = value;
        }

        public DCOSettings DCO;
        public DCFSettings DCF;
        public DCASettings DCA;
        public LFOSettings LFO;

        public AdditiveKit ADD;

        public bool IsAdditive => DCO.Wave.IsAdditive();

        public Source()
        {
            // The members are initialized here according to the
            // description in the Kawai K5000S user manual:

            _volume = new PositiveLevelType(120);
            _keyOnDelay = new PositiveLevelType();
            _effectPath = new EffectPathType();
            _benderPitch = new BenderPitchType();
            _benderCutoff = new BenderCutoffType();

            Pan = PanType.Normal;
            _panValue = new SignedLevelType();

            _zoneLow = new PositiveLevelType();
            _zoneHigh = new PositiveLevelType(127);

            VelocitySwitch = new VelocitySwitchSettings();
            VelocitySwitch.SwitchType = VelocitySwitchType.Off;
            VelocitySwitch.Threshold = 31;  // this is weirdly spec'd, need to check

            Press = new ControllerSettings();
            Wheel = new ControllerSettings();
            Express = new ControllerSettings();
            Assign1 = new AssignableController();
            Assign2 = new AssignableController();

            DCO = new DCOSettings();
            DCF = new DCFSettings();
            DCA = new DCASettings();
            LFO = new LFOSettings();
            ADD = new AdditiveKit();
        }

        public Source(byte[] data) : this()  // call other ctor to get all members initialized
        {
            int offset = 0;
            byte b = 0;  // will be reused when getting the next byte

            (b, offset) = Util.GetNextByte(data, offset);
            ZoneLow = b;

            (b, offset) = Util.GetNextByte(data, offset);
            ZoneHigh = b;

            (b, offset) = Util.GetNextByte(data, offset);
            VelocitySwitch = new VelocitySwitchSettings();
            VelocitySwitch.SwitchType = (VelocitySwitchType)(b >> 5);
            VelocitySwitch.Threshold = (byte)(b & 0x1F);
            //Console.Error.WriteLine($"velo sw original value = {b:X2}");

            (b, offset) = Util.GetNextByte(data, offset);
            EffectPath = b;

            (b, offset) = Util.GetNextByte(data, offset);
            Volume = b;

            (b, offset) = Util.GetNextByte(data, offset);
            BenderPitch = b;

            (b, offset) = Util.GetNextByte(data, offset);
            BenderCutoff = b;

            var pressBytes = new List<byte>();
            (b, offset) = Util.GetNextByte(data, offset);
            pressBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            pressBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            pressBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            pressBytes.Add(b);
            Press = new ControllerSettings(pressBytes);

            var wheelBytes = new List<byte>();
            (b, offset) = Util.GetNextByte(data, offset);
            wheelBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            wheelBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            wheelBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            wheelBytes.Add(b);
            Wheel = new ControllerSettings(wheelBytes);

            var expressBytes = new List<byte>();
            (b, offset) = Util.GetNextByte(data, offset);
            expressBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            expressBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            expressBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            expressBytes.Add(b);
            Express = new ControllerSettings(expressBytes);

            Assign1 = new AssignableController();
            (b, offset) = Util.GetNextByte(data, offset);
            Assign1.Source = (ControlSource)b;
            (b, offset) = Util.GetNextByte(data, offset);
            Assign1.Target.Destination = (ControlDestination)b;
            (b, offset) = Util.GetNextByte(data, offset);
            Assign1.Target.Depth = (sbyte)(b - 64); // (-31)33~(+31)~95

            Assign2 = new AssignableController();
            (b, offset) = Util.GetNextByte(data, offset);
            Assign2.Source = (ControlSource)b;
            (b, offset) = Util.GetNextByte(data, offset);
            Assign2.Target.Destination = (ControlDestination)b;
            (b, offset) = Util.GetNextByte(data, offset);
            Assign2.Target.Depth = (sbyte)(b - 64); // (-31)33~(+31)~95

            (b, offset) = Util.GetNextByte(data, offset);
            KeyOnDelay = b;

            (b, offset) = Util.GetNextByte(data, offset);
            Pan = (PanType)b;
            (b, offset) = Util.GetNextByte(data, offset);
            _panValue = new SignedLevelType(b);  // (63L)1~(63R)127

            DCO = new DCOSettings(data, offset);
            offset += DCO.ToData().Length;

            DCF = new DCFSettings(data, offset);
            offset += DCF.ToData().Length;

            DCA = new DCASettings(data, offset);
            offset += DCA.ToData().Length;

            var lfoBytes = new List<byte>();
            (b, offset) = Util.GetNextByte(data, offset);
            lfoBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            lfoBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            lfoBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            lfoBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            lfoBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            lfoBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            lfoBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            lfoBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            lfoBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            lfoBytes.Add(b);
            (b, offset) = Util.GetNextByte(data, offset);
            lfoBytes.Add(b);
            LFO = new LFOSettings(lfoBytes);

            ADD = new AdditiveKit();
            /*
            if (DCO.WaveNumber == AdditiveKit.WaveNumber)
            {
                byte[] additiveData = new byte[AdditiveKit.DataSize];
                Buffer.BlockCopy(data, offset, additiveData, 0, AdditiveKit.DataSize);
                ADD = new AdditiveKit(additiveData);
            }
             */
            // Don't automatically add the wave kit data, since we may not have it in our buffer
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append($"Zone: low = {ZoneLow}, high = {ZoneHigh}\n");
            builder.Append(string.Format("Vel. sw type = {0}, velocity = {1}\n", VelocitySwitch.SwitchType, VelocitySwitch.Threshold));
            builder.Append(string.Format("Effect path = {0}\n", EffectPath));
            builder.Append(string.Format("Volume = {0}\n", Volume));
            builder.Append(string.Format("Bender Pitch = {0}  Bender Cutoff = {1}\n", BenderPitch, BenderCutoff));
            builder.Append(string.Format("Key ON Delay = {0}\n", KeyOnDelay));
            builder.Append(string.Format("Pan type = {0}, value = {1}\n", Pan, PanValue));
            builder.Append($"DCO:\n{DCO}\n");
            builder.Append($"DCF:\n{DCF}\n");
            builder.Append($"DCA:\n{DCA}\n");
            builder.Append($"LFO:\n{LFO}\n");

            if (IsAdditive)
            {
                builder.Append(string.Format("ADD data:\n{0}", ADD.ToString()));
            }
            return builder.ToString();
        }

        public byte[] ToData()
        {
            var data = new List<byte>();

            data.Add(ZoneLow);
            data.Add(ZoneHigh);

            uint typeValue = (uint) VelocitySwitch.SwitchType;
            uint velocityValue = (uint) VelocitySwitch.Threshold;
            uint outValue = (typeValue << 5) | velocityValue;
            data.Add((byte)velocityValue);

            data.Add(EffectPath);
            data.Add(Volume);
            data.Add(BenderPitch);
            data.Add(BenderCutoff);

            data.AddRange(Press.ToData());
            data.AddRange(Wheel.ToData());
            data.AddRange(Express.ToData());

            data.AddRange(Assign1.ToData());
            data.AddRange(Assign2.ToData());

            data.Add(KeyOnDelay);
            data.Add((byte)Pan);
            data.Add(_panValue.Byte);

            data.AddRange(DCO.ToData());
            data.AddRange(DCF.ToData());
            data.AddRange(DCA.ToData());
            data.AddRange(LFO.ToData());

            if (IsAdditive)
            {
                data.AddRange(ADD.ToData());
            }

            return data.ToArray();
        }
    }
}
