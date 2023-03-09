using System;
using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K5000
{
    public enum VelocitySwitchKind
    {
        Off,  // "notes play at all velocity levels"
        Loud, // "only loud notes will sound"
        Soft  // "only soft notes will sound"
    }

    public class VelocitySwitchSettings: ISystemExclusiveData
    {
        public VelocitySwitchKind SwitchKind;  // enumeration
        public byte Threshold; // MIDI velocity value

        public VelocitySwitchSettings()
        {
            SwitchKind = VelocitySwitchKind.Off;
            Threshold = 4; // lower bound of 4~127
        }

        public VelocitySwitchSettings(byte b)
        {
            SwitchKind = (VelocitySwitchKind)(b >> 5);

            var thresholdValue = 4 + ((b & 0x1F) * 4);  // convert from 0~31 to 4~128
            if (thresholdValue == 128)  // adjust to 127 if necessary
            {
                thresholdValue -= 1;
            }
            Threshold = (byte) thresholdValue;
            //Console.Error.WriteLine($"velo sw original value = {b:X2}");
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            switch (SwitchKind)
            {
                case VelocitySwitchKind.Off:
                    builder.Append("OFF");
                    break;

                case VelocitySwitchKind.Loud:
                    builder.Append("LOUD");
                    break;

                case VelocitySwitchKind.Soft:
                    builder.Append("SOFT");
                    break;
            }

            builder.Append(string.Format(" {0}", Threshold));

            return builder.ToString();
        }

        //
        // ISystemExclusiveData implementation
        //

        public List<byte> Data
        {
            get
            {
                var kindValue = (uint) this.SwitchKind;

                var velocityValue = (uint) this.Threshold;
                if (velocityValue == 127)
                {
                    velocityValue += 1;  // adjust to 128 for calculation
                }
                velocityValue = (velocityValue / 4) - 4;  // adjust to 0~31

                // Combine the values into one byte
                var outValue = (byte)((kindValue << 5) | velocityValue);

                return new List<byte> { outValue };
            }
        }

        public int DataLength => 1;
    }

    public class ModulationSettings: ISystemExclusiveData
    {
        public ControlDestination Destination;  // enumeration

        public ControlDepth Depth;

        public ModulationSettings()
        {
            Destination = ControlDestination.Level;
            Depth = new ControlDepth();
        }

        public ModulationSettings(byte destination, byte depth)
        {
            Destination = (ControlDestination)destination;
            Depth = new ControlDepth(depth);
        }

        //
        // ISystemExclusiveData implementation
        //

        public List<byte> Data
        {
            get
            {
                return new List<byte> {
                    (byte)Destination,
                    Depth.ToByte()
                };
            }
        }

        public int DataLength => 2;
    }

    public class ControllerSettings: ISystemExclusiveData
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

        //
        // ISystemExclusiveData implementation
        //

        public List<byte> Data
        {
            get
            {
                List<byte> data = new List<byte>();

                data.AddRange(Destination1.Data);
                data.AddRange(Destination2.Data);

                return data;
            }
        }

        public int DataLength => 2;
    }

    public class AssignableController: ISystemExclusiveData
    {
        public ControlSource Source;
        public ModulationSettings Target;

        public AssignableController()
        {
            Source = ControlSource.Bender;
            Target = new ModulationSettings();
        }

        //
        // ISystemExclusiveData implementation
        //

        public List<byte> Data
        {
            get
            {
                var data = new List<byte>();

                data.Add((byte)Source);
                data.AddRange(Target.Data);

                return data;
            }
        }

        public int DataLength => 1 + Target.DataLength;
    }

    public enum PanKind
    {
        Normal,
        KeyScaling,
        NegativeKeyScaling,
        Random
    }

    /// <summary>
    /// Represents a source in a single patch.
    /// </summary>
    public class Source: ISystemExclusiveData
    {
        public static int DataSize = 86;

        public PositiveLevel Volume;
        public Zone Zone;
        public VelocitySwitchSettings VelocitySwitch;
        public EffectPath EffectPath;
        public BenderPitch BenderPitch;
        public BenderCutoff BenderCutoff;
        public ControllerSettings Press;
        public ControllerSettings Wheel;
        public ControllerSettings Express;
        public AssignableController Assign1;
        public AssignableController Assign2;

        public PositiveLevel KeyOnDelay;

        public PanKind Pan;  // enumeration

        public SignedLevel PanValue;  // (63L)1 ~ (63R)127

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

            Volume = new PositiveLevel(120);
            KeyOnDelay = new PositiveLevel();
            EffectPath = EffectPath.Path1;

            BenderPitch = new BenderPitch();
            BenderCutoff = new BenderCutoff();

            Pan = PanKind.Normal;
            PanValue = new SignedLevel();

            Zone = new Zone(0, 127);

            VelocitySwitch = new VelocitySwitchSettings();  // defaults to OFF / 31

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
            var low = b;
            (b, offset) = Util.GetNextByte(data, offset);
            var high = b;
            Zone = new Zone(low, high);

            (b, offset) = Util.GetNextByte(data, offset);
            VelocitySwitch = new VelocitySwitchSettings(b);

            (b, offset) = Util.GetNextByte(data, offset);
            EffectPath = (EffectPath)b;

            (b, offset) = Util.GetNextByte(data, offset);
            Volume = new PositiveLevel(b);

            (b, offset) = Util.GetNextByte(data, offset);
            BenderPitch = new BenderPitch(b);

            (b, offset) = Util.GetNextByte(data, offset);
            BenderCutoff = new BenderCutoff(b);

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
            Assign1.Target.Depth = new ControlDepth(b);

            Assign2 = new AssignableController();
            (b, offset) = Util.GetNextByte(data, offset);
            Assign2.Source = (ControlSource)b;
            (b, offset) = Util.GetNextByte(data, offset);
            Assign2.Target.Destination = (ControlDestination)b;
            (b, offset) = Util.GetNextByte(data, offset);
            Assign2.Target.Depth = new ControlDepth(b);

            (b, offset) = Util.GetNextByte(data, offset);
            KeyOnDelay = new PositiveLevel(b);

            (b, offset) = Util.GetNextByte(data, offset);
            Pan = (PanKind)b;
            (b, offset) = Util.GetNextByte(data, offset);
            PanValue = new SignedLevel(b);  // (63L)1~(63R)127

            DCO = new DCOSettings(data, offset);
            offset += DCO.Data.Count;

            DCF = new DCFSettings(data, offset);
            offset += DCF.Data.Count;

            DCA = new DCASettings(data, offset);
            offset += DCA.Data.Count;

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
            builder.Append($"Zone: low = {Zone.Low.Value}, high = {Zone.High.Value}\n");
            builder.Append(string.Format("Vel. sw type = {0}, velocity = {1}\n", VelocitySwitch.SwitchKind, VelocitySwitch.Threshold));
            builder.Append(string.Format("Effect path = {0}\n", EffectPath));
            builder.Append(string.Format("Volume = {0}\n", Volume.Value));
            builder.Append(string.Format("Bender Pitch = {0}  Bender Cutoff = {1}\n", BenderPitch.Value, BenderCutoff.Value));
            builder.Append(string.Format("Key ON Delay = {0}\n", KeyOnDelay.Value));
            builder.Append(string.Format("Pan type = {0}, value = {1}\n", Pan, PanValue.Value));
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

        //
        // Implementation of ISystemExclusiveData interface
        //

        public List<byte> Data
        {
            get
            {
                var data = new List<byte>();

                data.AddRange(Zone.Data);

                data.AddRange(VelocitySwitch.Data);
                data.Add((byte)EffectPath);
                data.Add(Volume.ToByte());
                data.Add(BenderPitch.ToByte());
                data.Add(BenderCutoff.ToByte());

                data.AddRange(Press.Data);
                data.AddRange(Wheel.Data);
                data.AddRange(Express.Data);

                data.AddRange(Assign1.Data);
                data.AddRange(Assign2.Data);

                data.Add(KeyOnDelay.ToByte());
                data.Add((byte)Pan);
                data.Add(PanValue.ToByte());

                data.AddRange(DCO.Data);
                data.AddRange(DCF.Data);
                data.AddRange(DCA.Data);
                data.AddRange(LFO.Data);

                if (IsAdditive)
                {
                    data.AddRange(ADD.Data);
                }

                return data;
            }
        }

        public int DataLength => DataSize;
    }
}
