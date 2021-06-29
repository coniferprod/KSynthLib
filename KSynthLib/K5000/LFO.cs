using System.Text;
using System.Collections.Generic;

namespace KSynthLib.K5000
{
    public enum LFOWaveform
    {
        Triangle,
        Square,
        Sawtooth,
        Sine,
        Random
    }

    public class LFOControl
    {
        private UnsignedLevelType _depth;
        public byte Depth // 0 ~ 63
        {
            get => _depth.Value;
            set => _depth.Value = value;
        }

        private SignedLevelType _keyScaling;
        public sbyte KeyScaling // (-63)1 ~ (+63)127
        {
            get => _keyScaling.Value;
            set => _keyScaling.Value = value;
        }

        public LFOControl()
        {
            _depth = new UnsignedLevelType();
            _keyScaling = new SignedLevelType();
        }

        public LFOControl(byte d, byte k)
        {
            _depth = new UnsignedLevelType(d);
            _keyScaling = new SignedLevelType(k);
        }

        public byte[] ToData() => new List<byte>() { Depth, _keyScaling.Byte }.ToArray();
    }

    public class LFOSettings
    {
        public LFOWaveform Waveform;

        private PositiveLevelType _speed;
        public byte Speed
        {
            get => _speed.Value;
            set => _speed.Value = value;
        }

        private PositiveLevelType _delayOnset;
        public byte DelayOnset
        {
            get => _delayOnset.Value;
            set => _delayOnset.Value = value;
        }

        private PositiveLevelType _fadeInTime;
        public byte FadeInTime
        {
            get => _fadeInTime.Value;
            set => _fadeInTime.Value = value;
        }

        private UnsignedLevelType _fadeInToSpeed;  // or "fade-in speed"?
        public byte FadeInToSpeed
        {
            get => _fadeInToSpeed.Value;
            set => _fadeInToSpeed.Value = value;
        }

        public LFOControl Vibrato;
        public LFOControl Growl;
        public LFOControl Tremolo;

        public LFOSettings()
        {
            Waveform = LFOWaveform.Triangle;
            _speed = new PositiveLevelType();
            _delayOnset = new PositiveLevelType();
            _fadeInTime = new PositiveLevelType();
            _fadeInToSpeed = new UnsignedLevelType();

            Vibrato = new LFOControl();
            Growl = new LFOControl();
            Tremolo = new LFOControl();
        }

        public LFOSettings(List<byte> data)
        {
            Waveform = (LFOWaveform)data[0];
            _speed = new PositiveLevelType(data[1]);
            _delayOnset = new PositiveLevelType(data[2]);
            _fadeInTime = new PositiveLevelType(data[3]);
            _fadeInToSpeed = new UnsignedLevelType(data[4]);

            Vibrato = new LFOControl(data[4], data[5]);
            Growl = new LFOControl(data[6], data[7]);
            Tremolo = new LFOControl(data[8], data[9]);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(string.Format("Waveform={0}  Speed={1}  Delay Onset={2}\n", Waveform, Speed, DelayOnset));
            builder.Append(string.Format("Fade In Time={0}  Fade In To Speed={1}\n", FadeInTime, FadeInToSpeed));
            builder.Append("LFO Modulation:\n");
            builder.Append(string.Format("Vibrato(DCO) = {0}   KS To Vibrato={1}\n", Vibrato.Depth, Vibrato.KeyScaling));
            builder.Append(string.Format("Growl(DCF) = {0}   KS To Growl={1}\n", Growl.Depth, Growl.KeyScaling));
            builder.Append(string.Format("Tremolo(DCA) = {0}   KS To Tremolo={1}\n", Tremolo.Depth, Tremolo.KeyScaling));
            return builder.ToString();
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();

            data.Add((byte)Waveform);
            data.Add(Speed);
            data.Add(DelayOnset);
            data.Add(FadeInTime);
            data.Add(FadeInToSpeed);
            data.AddRange(Vibrato.ToData());
            data.AddRange(Growl.ToData());
            data.AddRange(Tremolo.ToData());

            return data.ToArray();
        }
    }
}
