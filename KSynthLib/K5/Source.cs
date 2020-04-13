using System;
using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K5
{
    public enum KeyTracking
    {
        Track,
        Fixed
    }

    public class PitchEnvelopeSegment  // DFG ENV
    {
        private PositiveDepthType _rate; // 0~31
        public byte Rate
        {
            get => _rate.Value;
            set => _rate.Value = value;
        }

        private DepthType _level; // 0~±31
        public sbyte Level
        {
            get => _level.Value;
            set => _level.Value = value;
        }

        public PitchEnvelopeSegment()
        {
            _rate = new PositiveDepthType();
            _level = new DepthType();
        }

        public override string ToString()
        {
            return $"Rate={Rate} Level={Level}";
        }
    }

    public class PitchEnvelope
    {
        public PitchEnvelopeSegment[] Segments;
        public bool IsLooping;

        public PitchEnvelope()
        {

        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("*DFG ENV*\n");
            builder.Append("    SEG  | 1 | 2 | 3 | 4 | 5 | 6 |\n");
            builder.Append("    ------------------------------\n");
            builder.Append("    RATE |");
            for (int i = 0; i < Segments.Length; i++)
            {
                builder.Append($"{Segments[i].Rate,3}|");
            }
            builder.Append("\n");
            builder.Append("    LEVEL|");
            for (int i = 0; i < Segments.Length; i++)
            {
                builder.Append($"{Segments[i].Level,3}|");
            }
            builder.Append("\n\n");
            builder.Append("    LOOP<3-4>=");
            builder.Append(IsLooping ? "YES" : "--");
            builder.Append("\n\n");
            return builder.ToString();
        }
    }

    public struct EnvelopeSegment
    {
        public byte Rate;
        public byte Level;

        // true if this segment is the MAX in this envelope, false otherwise
        public bool IsMax;
        public bool IsMod;
    }
    

    public class PitchSettings
    {
        private CoarseType _coarse; // 0~±48
        public sbyte Coarse
        {
            get => _coarse.Value;
            set => _coarse.Value = value;
        }

        private DepthType _fine; // 0~±31
        public sbyte Fine
        {
            get => _fine.Value;
            set => _fine.Value = value;
        }

        public KeyTracking KeyTracking;  // enumeration
        public byte Key;  // the keytracking key, zero if not used
        public sbyte EnvelopeDepth; // 0~±24

        private DepthType _pressureDepth; // 0~±31
        public sbyte PressureDepth
        {
            get => _pressureDepth.Value;
            set => _pressureDepth.Value = value;
        }

        public byte BenderDepth; // 0~24

        private DepthType _velocityEnvelopeDepth; // 0~±31
        public sbyte VelocityEnvelopeDepth
        {
            get => _velocityEnvelopeDepth.Value;
            set => _velocityEnvelopeDepth.Value = value;
        }

        private PositiveDepthType _lfoDepth; // 0~31
        public byte LFODepth
        {
            get => _lfoDepth.Value;
            set => _lfoDepth.Value = value;
        }

        private DepthType _pressureLFODepth; // 0~±31
        public sbyte PressureLFODepth
        {
            get => _pressureLFODepth.Value;
            set => _pressureLFODepth.Value = value;
        }

        public PitchEnvelope PitchEnvelope;

        const int DataLength = 21;

        public PitchSettings()
        {
            _coarse = new CoarseType();
            _fine = new DepthType();
            _pressureDepth = new DepthType();
            _velocityEnvelopeDepth = new DepthType();
            _lfoDepth = new PositiveDepthType();
            _pressureLFODepth = new DepthType();
        }

        public override string ToString()
        {
            return string.Format(
                "*DFG*              \n\n" +
                "COARSE= {0,2}        <DEPTH>\n" + 
                "FINE  = {1,2}        ENV= {2,2}-VEL {3}\n" + 
                "                  PRS= {4}\n" + 
                "                  LFO= {5,2}-PRS= {6,3}\n" + 
                "KEY    ={7}     BND= {8}\n" + 
                "FIXNO  ={9}\n\n", 
                Coarse, Fine, EnvelopeDepth, VelocityEnvelopeDepth,
                PressureDepth, LFODepth, PressureLFODepth,
                KeyTracking, BenderDepth,
                Key) + 
                PitchEnvelope;
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();
            byte b = 0;
            data.Add(Coarse.ToByte());
            data.Add(Fine.ToByte());
            b = Key;  // the tracking key if fixed, 0 if track
            if (KeyTracking == KeyTracking.Fixed)
            {
                b = b.SetBit(7);
            }
            else
            {
                b = b.UnsetBit(7);
            }
            data.Add(b);
            data.Add(EnvelopeDepth.ToByte());
            data.Add(PressureDepth.ToByte());
            data.Add(BenderDepth);
            data.Add(VelocityEnvelopeDepth.ToByte());
            data.Add(LFODepth);
            data.Add(PressureLFODepth.ToByte());

            for (int i = 0; i < Source.PitchEnvelopeSegmentCount; i++)
            {
                b = PitchEnvelope.Segments[i].Rate;

                // Set the envelope looping bit for the first rate only:
                if (i == 0)
                {
                    if (PitchEnvelope.IsLooping)
                    {
                        b = b.SetBit(7);
                    }
                }
                data.Add(b);
            }

            for (int i = 0; i < Source.PitchEnvelopeSegmentCount; i++)
            {
                sbyte sb = PitchEnvelope.Segments[i].Level;
                data.Add(sb.ToByte());
            }

            if (data.Count != DataLength)
            {
                System.Console.WriteLine(String.Format("WARNING: DFG length, expected = {0}, actual = {1}", DataLength, data.Count));
            }

            return data.ToArray();
        }
    }

    public class Source
    {
        public const int EnvelopeSegmentCount = 6;
        public const int PitchEnvelopeSegmentCount = 6;
        public const int HarmonicCount = 63;
        public const int HarmonicEnvelopeCount = 4;
        public const int HarmonicEnvelopeSegmentCount = 6;
        public const int FilterEnvelopeSegmentCount = 6;
        public const int AmplifierEnvelopeSegmentCount = 7;
    
        public PitchSettings Pitch;
        public Harmonic[] Harmonics;
        public Harmonic Harmonic63bis;
        public HarmonicSettings HarmonicSettings;
        public Filter Filter;
        public Amplifier Amplifier;

        public int SourceNumber;

        public Source()
        {
            Pitch = new PitchSettings();
            Harmonics = new Harmonic[HarmonicCount];
            Harmonic63bis = new Harmonic();
            HarmonicSettings = new HarmonicSettings();
            Filter = new Filter();
            Amplifier = new Amplifier();
        }

        public Source(byte[] data, int number) : this()
        {
            SourceNumber = number;

            System.Console.WriteLine($"S{SourceNumber} data:");
            System.Console.WriteLine(Util.HexDump(data));

            int offset = 0;
            byte b = 0;  // reused when getting the next byte
            List<byte> buf = new List<byte>();

            // DFG
            Pitch = new PitchSettings();

            (b, offset) = Util.GetNextByte(data, offset);
            Pitch.Coarse = b.ToSignedByte();

            (b, offset) = Util.GetNextByte(data, offset);
            Pitch.Fine = b.ToSignedByte();

	        (b, offset) = Util.GetNextByte(data, offset);
	        if (b.IsBitSet(7))
            {
                Pitch.KeyTracking = KeyTracking.Fixed;
                Pitch.Key = (byte)(b & 0b01111111);
            }
            else 
            {
                Pitch.KeyTracking = KeyTracking.Track;
                Pitch.Key = (byte)(b & 0b01111111);
            }
            // TODO: Check that the SysEx spec gets the meaning of b7 right

            (b, offset) = Util.GetNextByte(data, offset);
            Pitch.EnvelopeDepth = b.ToSignedByte();

            (b, offset) = Util.GetNextByte(data, offset);
            Pitch.PressureDepth = b.ToSignedByte();

            (b, offset) = Util.GetNextByte(data, offset);
            Pitch.BenderDepth = b;

            (b, offset) = Util.GetNextByte(data, offset);
            Pitch.VelocityEnvelopeDepth = b.ToSignedByte();

            (b, offset) = Util.GetNextByte(data, offset);
            Pitch.LFODepth = b;

            (b, offset) = Util.GetNextByte(data, offset);
            Pitch.PressureLFODepth = b.ToSignedByte();

            Pitch.PitchEnvelope.Segments = new PitchEnvelopeSegment[PitchEnvelopeSegmentCount];
            for (int i = 0; i < PitchEnvelopeSegmentCount; i++)
            {
                (b, offset) = Util.GetNextByte(data, offset);
                buf.Add(b);
                if (i == 0)
                {
                    Pitch.PitchEnvelope.IsLooping = b.IsBitSet(7);
                    Pitch.PitchEnvelope.Segments[i].Rate = (byte)(b & 0x7f);
                }
                else
                {
                    Pitch.PitchEnvelope.Segments[i].Rate = b;
                }
            }

            for (int i = 0; i < PitchEnvelopeSegmentCount; i++)
            {
                (b, offset) = Util.GetNextByte(data, offset);
                buf.Add(b);
                Pitch.PitchEnvelope.Segments[i].Level = b.ToSignedByte();
            }

            // DHG

            Harmonics = new Harmonic[HarmonicCount];
            for (int i = 0; i < HarmonicCount; i++)
            {
                (b, offset) = Util.GetNextByte(data, offset);
                Harmonics[i].Level = b;
            }

            // The values are packed into 31 + 1 bytes. The first 31 bytes contain the settings
            // for harmonics 1 to 62. The solitary byte that follows has the harm 63 settings.
            List<byte> harmData = new List<byte>();
            int count = 0;
            byte lowNybble = 0;
            byte highNybble = 0;
            while (count < HarmonicCount - 1)
            {
		        (b, offset) = Util.GetNextByte(data, offset);
                (highNybble, lowNybble) = Util.NybblesFromByte(b);
                harmData.Add(lowNybble);
                harmData.Add(highNybble);
                count += 2;
            }

            // NOTE: Seems that for harmonics with zero level and modulation off, the envelope number
            // could be something else than 0...3 (1...4). For example, 12 is a typical value.
            // Probably doesn't matter.

	        (b, offset) = Util.GetNextByte(data, offset);
            (highNybble, lowNybble) = Util.NybblesFromByte(b);
            harmData.Add(highNybble);
            Harmonic63bis = new Harmonic();
            Harmonic63bis.IsModulationActive = lowNybble.IsBitSet(2);
            Harmonic63bis.EnvelopeNumber = (byte)(lowNybble + 1);
            Harmonic63bis.Level = 99;  // don't care really

            // OK, here's the thing: there might be an error in the Kawai K5 System Exclusive specification
            // regarding the last harmonic (63) and how it is packed into a byte. Since it is not a very prominent
            // harmonic, we leave it as it is, even though it means that the comparison between parsed and
            // emitted SysEx data will fail. But it is close enough for now.

	        // Now harmData should have data for all the 63 harmonics
	        for (int i = 0; i < Harmonics.Length; i++) 
            {
		        Harmonics[i].IsModulationActive = harmData[i].IsBitSet(2);
		        Harmonics[i].EnvelopeNumber = (byte)(harmData[i] + 1);  // add one to make env number 1...4
	        }

            // DHG harmonic settings (S253 ... S260)
            HarmonicSettings harmSet = new HarmonicSettings();

	        (b, offset) = Util.GetNextByte(data, offset);
        	harmSet.VelocityDepth = b.ToSignedByte();

	        (b, offset) = Util.GetNextByte(data, offset);
        	harmSet.PressureDepth = b.ToSignedByte();

            (b, offset) = Util.GetNextByte(data, offset);
        	harmSet.KeyScalingDepth = b.ToSignedByte();

	        (b, offset) = Util.GetNextByte(data, offset);
        	harmSet.LFODepth = b;

            // Harmonic envelope 1 - 4 settings (these will be augmented later in the process)
            harmSet.Envelopes = new HarmonicEnvelope[HarmonicEnvelopeCount];
            for (int i = 0; i < HarmonicEnvelopeCount; i++) 
            {
    	        (b, offset) = Util.GetNextByte(data, offset);
                harmSet.Envelopes[i].IsActive = b.IsBitSet(7);
			    harmSet.Envelopes[i].Effect = (byte)(b & 0x1f);
		    }

            // The master modulation setting is packed with the harmonic selection value 
    	    (b, offset) = Util.GetNextByte(data, offset);
	        harmSet.IsModulationActive = b.IsBitSet(7);

            HarmonicSelection selection = HarmonicSelection.All;
            byte v = (byte)(b & 0x03);
	        switch (v) {
	        case 0:
		        selection = HarmonicSelection.Live;
                break;
	        case 1:
		        selection = HarmonicSelection.Die;
                break;
	        case 2:
		        selection = HarmonicSelection.All;
                break;
	        default:
		        selection = HarmonicSelection.All;
                break;
	        }
	        harmSet.Selection = selection;

    	    (b, offset) = Util.GetNextByte(data, offset);
        	harmSet.RangeFrom = b;

    	    (b, offset) = Util.GetNextByte(data, offset);
        	harmSet.RangeTo = b;

            // Harmonic envelope selections = 0/1, 1/2, 2/3, 3/4
    	    (b, offset) = Util.GetNextByte(data, offset);
            (highNybble, lowNybble) = Util.NybblesFromByte(b);
            // odd and even are in the same byte
        	harmSet.Odd = new HarmonicModulation 
            { 
                IsOn = highNybble.IsBitSet(3), 
                EnvelopeNumber = (byte)((highNybble & 0b00000011) + 1) 
            };
	        harmSet.Even = new HarmonicModulation 
            {
		        IsOn = lowNybble.IsBitSet(3),
		        EnvelopeNumber = (byte)((lowNybble & 0b00000011) + 1)
	        };

    	    (b, offset) = Util.GetNextByte(data, offset);
            (highNybble, lowNybble) = Util.NybblesFromByte(b);
            // octave and fifth are in the same byte
        	harmSet.Octave = new HarmonicModulation 
            {
		        IsOn = highNybble.IsBitSet(3),
		        EnvelopeNumber = (byte)((highNybble & 0b00000011) + 1)
            };
        	harmSet.Fifth = new HarmonicModulation
            {
		        IsOn = lowNybble.IsBitSet(3),
		        EnvelopeNumber = (byte)((lowNybble & 0b00000011) + 1)
            };

    	    (b, offset) = Util.GetNextByte(data, offset);
            (highNybble, lowNybble) = Util.NybblesFromByte(b);
        	harmSet.All = new HarmonicModulation
            {
		        IsOn = highNybble.IsBitSet(3),
		        EnvelopeNumber = (byte)((highNybble & 0b00000011) + 1)
            };

    	    (b, offset) = Util.GetNextByte(data, offset);
            switch (b)
            {
            case 0:
                harmSet.Angle = HarmonicAngle.Negative;
                break;
            case 1:
                harmSet.Angle = HarmonicAngle.Neutral;
                break;
            case 2:
                harmSet.Angle = HarmonicAngle.Positive;
                break;
            default:
                harmSet.Angle = HarmonicAngle.Neutral;  // just to keep the compiler happy
                break;
            }

    	    (b, offset) = Util.GetNextByte(data, offset);
	        harmSet.HarmonicNumber = b;

            // Harmonic envelopes (S285 ... S380) - these were created earlier.
            // There are six segments for each of the four envelopes.
            int harmonicEnvelopeDataCount = 0;
            int desiredHarmonicEnvelopeDataCount = 6 * 4 * 2;  // 4 envs, 6 segs each, level + rate for each seg
            bool shadow = false;
            for (int ei = 0; ei < HarmonicEnvelopeCount; ei++)
            {
                HarmonicEnvelopeSegment[] segments = new HarmonicEnvelopeSegment[HarmonicEnvelopeSegmentCount];
                for (int si = 0; si < HarmonicEnvelopeSegmentCount; si++)
                {
            	    (b, offset) = Util.GetNextByte(data, offset);
                    harmonicEnvelopeDataCount++;
                    if (si == 0)
                    {
                        shadow = b.IsBitSet(7);
                    }
                    segments[si].IsMaxSegment = b.IsBitSet(6);
                    segments[si].Level = (byte)(b & 0b00111111);
                }

                for (int si = 0; si < HarmonicEnvelopeSegmentCount; si++)
                {
                    (b, offset) = Util.GetNextByte(data, offset);
                    harmonicEnvelopeDataCount++;
                    segments[si].Rate = (byte)(b & 0b00111111);
                }

                harmSet.Envelopes[ei].Segments = segments;
            }
            if (harmonicEnvelopeDataCount != desiredHarmonicEnvelopeDataCount)
            {
                Console.WriteLine($"Should have {desiredHarmonicEnvelopeDataCount} bytes of HE data, have {harmonicEnvelopeDataCount} bytes");
            }
            harmSet.IsShadowOn = shadow;

            /*
            for (int ei = 0; ei < HarmonicEnvelopeCount; ei++)
            {
                for (int si = 0; si < HarmonicEnvelopeSegmentCount; si++)
                {
                    Console.WriteLine(string.Format("env{0} seg{1} rate = {2} level = {3}{4}", ei + 1, si + 1, harmSet.Envelopes[ei].Segments[si].Rate, harmSet.Envelopes[ei].Segments[si].Level, harmSet.Envelopes[ei].Segments[si].IsMaxSegment ? "*": ""));
                }
            }
             */

	        HarmonicSettings = harmSet;  // finally we get to assign this to the source

            // DDF (S381 ... S426)
    	    (b, offset) = Util.GetNextByte(data, offset);
            Filter.Cutoff = (byte)(b & 0b01111111);
    	    (b, offset) = Util.GetNextByte(data, offset);
            Filter.CutoffModulation = (byte)(b & 0b00011111);
            (b, offset) = Util.GetNextByte(data, offset);
            Filter.Slope = (byte)(b & 0b00011111);
    	    (b, offset) = Util.GetNextByte(data, offset);
            Filter.SlopeModulation = (byte)(b & 0b00011111);
            (b, offset) = Util.GetNextByte(data, offset);
            Filter.FlatLevel = (byte)(b & 0b00011111);
    	    (b, offset) = Util.GetNextByte(data, offset);
            Filter.VelocityDepth = b.ToSignedByte();
    	    (b, offset) = Util.GetNextByte(data, offset);
            Filter.PressureDepth = b.ToSignedByte();
    	    (b, offset) = Util.GetNextByte(data, offset);
            Filter.KeyScalingDepth = b.ToSignedByte();
    	    (b, offset) = Util.GetNextByte(data, offset);
            Filter.EnvelopeDepth = b.ToSignedByte();
    	    (b, offset) = Util.GetNextByte(data, offset);
            Filter.VelocityEnvelopeDepth = b.ToSignedByte();
    	    (b, offset) = Util.GetNextByte(data, offset);
            Filter.IsActive = b.IsBitSet(7);
            Filter.IsModulationActive = b.IsBitSet(6);
            Filter.LFODepth = (byte)(b & 0b00011111);

            Filter.EnvelopeSegments = new FilterEnvelopeSegment[FilterEnvelopeSegmentCount];
            for (int i = 0; i < FilterEnvelopeSegmentCount; i++)
            {
        	    (b, offset) = Util.GetNextByte(data, offset);
                Filter.EnvelopeSegments[i].Rate = b;
            }
            for (int i = 0; i < FilterEnvelopeSegmentCount; i++)
            {
        	    (b, offset) = Util.GetNextByte(data, offset);
                Filter.EnvelopeSegments[i].IsMaxSegment = b.IsBitSet(6);
                Filter.EnvelopeSegments[i].Level = (byte)(b & 0x3f);
            }

            // DDA (S427 ... S468)
    	    (b, offset) = Util.GetNextByte(data, offset);
            Amplifier.AttackVelocityDepth = b.ToSignedByte();
            
    	    (b, offset) = Util.GetNextByte(data, offset);
            Amplifier.PressureDepth = b.ToSignedByte();

    	    (b, offset) = Util.GetNextByte(data, offset);
            Amplifier.KeyScalingDepth = b.ToSignedByte();

    	    (b, offset) = Util.GetNextByte(data, offset);
            Amplifier.IsActive = b.IsBitSet(7);
            Amplifier.LFODepth = (byte)(b & 0b01111111);

    	    (b, offset) = Util.GetNextByte(data, offset);
            Amplifier.AttackVelocityRate = b.ToSignedByte();

    	    (b, offset) = Util.GetNextByte(data, offset);
            Amplifier.ReleaseVelocityRate = b.ToSignedByte();

    	    (b, offset) = Util.GetNextByte(data, offset);
            Amplifier.KeyScalingRate = b.ToSignedByte();

            // Amplifier envelope segments:
            // Bit 7 is always zero, bit 6 is a boolean toggle, and bits 5...0 are the value
            Amplifier.Envelope = new AmplifierEnvelope();  // also creates the segments

            // First, the amp envelope rates:
            for (int i = 0; i < AmplifierEnvelopeSegmentCount; i++)
            {
        	    (b, offset) = Util.GetNextByte(data, offset);
                Amplifier.Envelope.Segments[i].IsRateModulationOn = b.IsBitSet(6);
                Amplifier.Envelope.Segments[i].Rate = (byte)(b & 0b00111111);
            }

            // Then, the amp envelope levels and max settings:
            for (int i = 0; i < AmplifierEnvelopeSegmentCount - 1; i++)
            {
        	    (b, offset) = Util.GetNextByte(data, offset);
                Amplifier.Envelope.Segments[i].IsMaxSegment = b.IsBitSet(6);
                Amplifier.Envelope.Segments[i].Level = (byte)(b & 0b00111111);
            }

            // Actually, S467 and S468 are marked as zero in the SysEx description:
            //   S467 00000000 s1
            //   S468 00000000 s2
            // But we'll use them as the max setting and level for the 7th amp env segment.
            // Not sure if this is an error in the spec, or my misinterpretation (maybe the
            // 7th segment doesn't have a level?)
        }

        public override string ToString()
        {
            /*
            StringBuilder harmonicBuilder = new StringBuilder();
            harmonicBuilder.Append("Harmonics:\n");
            for (int i = 0; i < HarmonicCount; i++)
            {
                harmonicBuilder.Append(String.Format("{0,2}: {1,2} {2}\n", i, Harmonics[i].Level, Harmonics[i].IsModulationActive ? "Y" : "N"));
            }
            harmonicBuilder.Append("\n");
             */

            return $"{Pitch}{HarmonicSettings}{Filter}{Amplifier}";
        }

        public byte[] ToData()
        {
            var buf = new List<byte>();

            buf.AddRange(Pitch.ToData());

            for (int i = 0; i < HarmonicCount; i++)
            {
                buf.Add(Harmonics[i].Level);
            }

            byte b = 0;
            byte lowNybble = 0, highNybble = 0;
            int count = 0;
            // Harmonics 1...62 (0...61)
            while (count < HarmonicCount - 2)
            {
                lowNybble = (byte)(Harmonics[count].EnvelopeNumber - 1);
                lowNybble = lowNybble.UnsetBit(2);
                if (Harmonics[count].IsModulationActive)
                {
                    lowNybble = lowNybble.SetBit(3);
                }

                count++;

                highNybble = (byte)(Harmonics[count].EnvelopeNumber - 1);
                highNybble = highNybble.UnsetBit(2);
                if (Harmonics[count].IsModulationActive)
                {
                    highNybble = highNybble.SetBit(3);
                }

                count++;

                b = Util.ByteFromNybbles(highNybble, lowNybble);
                buf.Add(b);
            }

            // harmonic 63 (count = 62)
            b = (byte)(Harmonics[count].EnvelopeNumber - 1);
            byte originalByte = b;
            b = b.UnsetBit(3);
            if (Harmonics[count].IsModulationActive)
            {
                b = b.SetBit(3);
            }

            byte extraByte = (byte)(Harmonic63bis.EnvelopeNumber - 1);
            if (Harmonic63bis.IsModulationActive)
            {
                extraByte = extraByte.SetBit(3);
            }
            byte finalByte = Util.ByteFromNybbles(b, extraByte);
            buf.Add(finalByte);

            buf.AddRange(HarmonicSettings.ToData());
            buf.AddRange(Filter.ToData());
            buf.AddRange(Amplifier.ToData());

            return buf.ToArray();
        }
    }
}