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
        public UnsignedLevel Depth;
        public SignedLevel KeyScaling;

        public LFOControl()
        {
            Depth = new UnsignedLevel();
            KeyScaling = new SignedLevel();
        }

        public LFOControl(byte d, byte k)
        {
            Depth = new UnsignedLevel(d);
            KeyScaling = new SignedLevel(k);
        }

        public byte[] ToData() => new List<byte>() { Depth.ToByte(), KeyScaling.ToByte() }.ToArray();
    }

    public class LFOSettings
    {
        public LFOWaveform Waveform;
        public PositiveLevel Speed;
        public PositiveLevel DelayOnset;
        public PositiveLevel FadeInTime;
        public UnsignedLevel FadeInToSpeed;  // or "fade-in speed"?
        public LFOControl Vibrato;
        public LFOControl Growl;
        public LFOControl Tremolo;

        public LFOSettings()
        {
            Waveform = LFOWaveform.Triangle;
            Speed = new PositiveLevel();
            DelayOnset = new PositiveLevel();
            FadeInTime = new PositiveLevel();
            FadeInToSpeed = new UnsignedLevel();
            Vibrato = new LFOControl();
            Growl = new LFOControl();
            Tremolo = new LFOControl();
        }

        public LFOSettings(List<byte> data)
        {
            Waveform = (LFOWaveform)data[0];
            Speed = new PositiveLevel(data[1]);
            DelayOnset = new PositiveLevel(data[2]);
            FadeInTime = new PositiveLevel(data[3]);
            FadeInToSpeed = new UnsignedLevel(data[4]);
            Vibrato = new LFOControl(data[4], data[5]);
            Growl = new LFOControl(data[6], data[7]);
            Tremolo = new LFOControl(data[8], data[9]);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(string.Format("Waveform={0}  Speed={1}  Delay Onset={2}\n", Waveform, Speed.Value, DelayOnset.Value));
            builder.Append(string.Format("Fade In Time={0}  Fade In To Speed={1}\n", FadeInTime.Value, FadeInToSpeed.Value));
            builder.Append("LFO Modulation:\n");
            builder.Append(string.Format("Vibrato(DCO) = {0}   KS To Vibrato={1}\n", Vibrato.Depth.Value, Vibrato.KeyScaling.Value));
            builder.Append(string.Format("Growl(DCF) = {0}   KS To Growl={1}\n", Growl.Depth.Value, Growl.KeyScaling.Value));
            builder.Append(string.Format("Tremolo(DCA) = {0}   KS To Tremolo={1}\n", Tremolo.Depth.Value, Tremolo.KeyScaling.Value));
            return builder.ToString();
        }

        public byte[] ToData()
        {
            var data = new List<byte>();

            data.AddRange(new List<byte>() {
                (byte)Waveform,
                Speed.ToByte(),
                DelayOnset.ToByte(),
                FadeInTime.ToByte(),
                FadeInToSpeed.ToByte()
            });

            data.AddRange(Vibrato.ToData());
            data.AddRange(Growl.ToData());
            data.AddRange(Tremolo.ToData());

            return data.ToArray();
        }
    }
}
