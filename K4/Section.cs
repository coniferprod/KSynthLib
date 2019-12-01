using System;
using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K4
{
    public class Section
    {
        public static readonly int DataSize = 8;
        private int singlePatch;
        private int zoneLow;
        private int zoneHigh;
        private int receiveChannel;
        private int velocitySwitch;
        private bool isMuted;
        private int output;

        private int playMode;
        private int level;
        private int transpose;
        private int tune;

        public Section(byte[] data)
        {
            int offset = 0;
            byte b = 0;  // will be reused when getting the next byte

            (b, offset) = Util.GetNextByte(data, offset);
            singlePatch = b;

            (b, offset) = Util.GetNextByte(data, offset);
            zoneLow = b;

            (b, offset) = Util.GetNextByte(data, offset);
            zoneHigh = b;

            (b, offset) = Util.GetNextByte(data, offset);
            // rcv ch = M15 bits 0...3
            receiveChannel = (int)(b & 0x0f);
            // velo sw = M15 bits 4..5
            velocitySwitch = (int)((b >> 4) & 0x03);
            // section mute = M15 bit 6
            isMuted = b.IsBitSet(6);

            (b, offset) = Util.GetNextByte(data, offset);
            // out select = M16 bits 0...2
            output = (int)(b & 0x07);
            // play mode = M16 bits 3...4
            playMode = (int)((b >> 3) & 0x03);

            (b, offset) = Util.GetNextByte(data, offset);
            level = b;

            (b, offset) = Util.GetNextByte(data, offset);
            transpose = b;

            (b, offset) = Util.GetNextByte(data, offset);
            tune = b;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(String.Format("single = {0}, recv ch = {1}, play mode = {2}\n", PatchUtil.GetPatchName(singlePatch), receiveChannel + 1, playMode));
            builder.Append(String.Format("zone = {0} to {1}, vel sw = {2}\n", GetNoteName(zoneLow), GetNoteName(zoneHigh), velocitySwitch));
            builder.Append(String.Format("level = {0}, transpose = {1}, tune = {2}\n", level, transpose, tune));
            builder.Append(String.Format("submix ch = {0}\n", output));
            return builder.ToString();
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();

            data.Add((byte)singlePatch);
            data.Add((byte)zoneLow);
            data.Add((byte)zoneHigh);

            // Combine rcv ch, velo sw and section mute into one byte for M15/M23 etc.
            byte vb = (byte)velocitySwitch;
            byte rb = (byte)receiveChannel;
            byte vbp = (byte)(vb << 4);
            byte m15 = (byte)(rb | vbp);
            if (isMuted)
            {
                m15.SetBit(6);
            }
            data.Add(m15);

            // TODO: Combine out select and mode into one byte for M16/M24 etc.
            byte os = (byte)output;
            byte m = (byte)playMode;
            byte m16 = (byte)(os | m);
            data.Add(m16);

            data.Add((byte)level);
            data.Add((byte)transpose);
            data.Add((byte)tune);

            return data.ToArray();
        }

        // This is bogus; should be 0 ~ 127 / C-2 ~ G8
        private string GetNoteName(int noteNumber) {
            //string[] notes = new string[] {"A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#"};
            string[] notes = new string[] { "C", "C#", "D", "Eb", "E", "F", "F#", "G", "G#", "A", "Bb", "B" };
            int octave = noteNumber / 12 + 1;
            string name = notes[noteNumber % 12];
            return name + octave;
        }
    }
}
