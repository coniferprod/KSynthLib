using System;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K5
{
    public enum KeyTracking
    {
        Track,
        Fixed
    }


    public struct EnvelopeSegment
    {
        public byte Rate;
        public byte Level;

        // true if this segment is the MAX in this envelope, false otherwise
        public bool IsMax;
        public bool IsMod;
    }

    public enum SourceMode
    {
        Twin,
        Full
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

        /// <summary>Constructs a Source from binary data.</summary>
        /// <param name="number">The source number, must be 1 or 2.</param>
        /// <remarks>Calls the no-argument constructor to initialize members.</remarks>
        public Source(byte[] data, int number) : this()
        {
            SourceNumber = number;

            //Console.Error.WriteLine($"S{SourceNumber} data:");
            //Console.Error.WriteLine(Util.HexDump(data));

            int offset = 0;
            byte b = 0;  // reused when getting the next byte
            //List<byte> buf = new List<byte>();

            // DFG (S21 ... S62)

            //Pitch = new PitchSettings();  // created by no-arg ctor

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

            Pitch.Envelope.Segments = new PitchEnvelopeSegment[PitchEnvelopeSegmentCount];
            for (int i = 0; i < PitchEnvelopeSegmentCount; i++)
            {
                (b, offset) = Util.GetNextByte(data, offset);
                //buf.Add(b);
                if (i == 0)
                {
                    Pitch.Envelope.IsLooping = b.IsBitSet(7);
                    Pitch.Envelope.Segments[i].Rate = (byte)(b & 0x7f);
                }
                else
                {
                    Pitch.Envelope.Segments[i].Rate = b;
                }
            }

            for (int i = 0; i < PitchEnvelopeSegmentCount; i++)
            {
                (b, offset) = Util.GetNextByte(data, offset);
                //buf.Add(b);
                Pitch.Envelope.Segments[i].Level = b.ToSignedByte();
            }

            // DHG (S63 ... S380)

            //Harmonics = new Harmonic[HarmonicCount]; // created by no-arg ctor
            for (int i = 0; i < HarmonicCount; i++)
            {
                Harmonic harm = new Harmonic();
                (b, offset) = Util.GetNextByte(data, offset);
                harm.Level = b;
                Harmonics[i] = harm;
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
            //Harmonic63bis = new Harmonic(); // created by no-arg ctor
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
                HarmonicEnvelope he = new HarmonicEnvelope();
    	        (b, offset) = Util.GetNextByte(data, offset);
                he.IsActive = b.IsBitSet(7);
			    he.Effect = (byte)(b & 0x1f);
                harmSet.Envelopes[i] = he;
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
            HarmonicModulation odd = new HarmonicModulation();
            odd.IsOn = highNybble.IsBitSet(3);
            odd.EnvelopeNumber = (byte)((highNybble & 0b00000011) + 1);
        	harmSet.Odd = odd;

            HarmonicModulation even = new HarmonicModulation();
            even.IsOn = lowNybble.IsBitSet(3);
            even.EnvelopeNumber = (byte)((lowNybble & 0b00000011) + 1);
            harmSet.Even = even;

    	    (b, offset) = Util.GetNextByte(data, offset);
            (highNybble, lowNybble) = Util.NybblesFromByte(b);

            // octave and fifth are in the same byte
            HarmonicModulation octave = new HarmonicModulation();
            octave.IsOn = highNybble.IsBitSet(3);
            octave.EnvelopeNumber = (byte)((highNybble & 0b00000011) + 1);
            harmSet.Octave = octave;

            HarmonicModulation fifth = new HarmonicModulation();
            fifth.IsOn = lowNybble.IsBitSet(3);
            fifth.EnvelopeNumber = (byte)((lowNybble & 0b00000011) + 1);
            harmSet.Fifth = fifth;

    	    (b, offset) = Util.GetNextByte(data, offset);
            (highNybble, lowNybble) = Util.NybblesFromByte(b);
            HarmonicModulation all = new HarmonicModulation();
            all.IsOn = highNybble.IsBitSet(3);
            all.EnvelopeNumber = (byte)((highNybble & 0b00000011) + 1);
        	harmSet.All = all;

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
                    HarmonicEnvelopeSegment segment = new HarmonicEnvelopeSegment();
            	    (b, offset) = Util.GetNextByte(data, offset);
                    harmonicEnvelopeDataCount++;
                    if (si == 0)
                    {
                        shadow = b.IsBitSet(7);
                    }
                    segment.IsMaxSegment = b.IsBitSet(6);
                    segment.Level = (byte)(b & 0b00111111);

                    segments[si] = segment;
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
                Console.Error.WriteLine($"WARNING: Should have {desiredHarmonicEnvelopeDataCount} bytes of HE data, have {harmonicEnvelopeDataCount} bytes");
            }
            harmSet.IsShadowOn = shadow;

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
                FilterEnvelopeSegment segment = new FilterEnvelopeSegment();
        	    (b, offset) = Util.GetNextByte(data, offset);
                segment.Rate = b;
                Filter.EnvelopeSegments[i] = segment;
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
                AmplifierEnvelopeSegment segment = new AmplifierEnvelopeSegment();
        	    (b, offset) = Util.GetNextByte(data, offset);
                segment.IsRateModulationOn = b.IsBitSet(6);
                segment.Rate = (byte)(b & 0b00111111);
                Amplifier.Envelope.Segments[i] = segment;
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

                highNybble = (byte)(Harmonics[count].EnvelopeNumber - 1); // 1~4 to 0~3
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
            b = (byte)(Harmonics[count].EnvelopeNumber - 1); // 1~4 to 0~3
            byte originalByte = b;
            b = b.UnsetBit(3);
            if (Harmonics[count].IsModulationActive)
            {
                b = b.SetBit(3);
            }

            byte extraByte = (byte)(Harmonic63bis.EnvelopeNumber - 1); // 1~4 to 0~3
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