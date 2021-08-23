using System;
using System.Collections.Generic;
using System.Text;

using KSynthLib.Common;

namespace KSynthLib.K4
{

    public class DrumPatch
    {
        public const int DataSize = 682;

        public const int NoteCount = 61;  // from C1 to C6

        private MidiChannelType _receiveChannel;
        public byte ReceiveChannel
        {
            get => _receiveChannel.Value;
            set => _receiveChannel.Value = value;
        }

        private LevelType _volume;
        public byte Volume
        {
            get => _volume.Value;
            set => _volume.Value = value;
        }

        private LevelType _velocityDepth;
        public byte VelocityDepth
        {
            get => _velocityDepth.Value;
            set => _velocityDepth.Value = value;
        }

        public List<DrumNote> Notes;

        private byte _checksum;
        public byte Checksum
        {
            get
            {
                List<byte> data = new List<byte>();

                data.Add(_receiveChannel.AsByte()); // 1~16 stored as 0~15
                data.Add(Volume);
                data.Add(VelocityDepth);

                byte[] bs = data.ToArray();
                byte sum = 0;
                foreach (byte b in bs)
                {
                    sum += b;
                }
                sum += 0xA5;
                return sum;
            }

            set => _checksum = value;
        }

        public DrumPatch()
        {
            _receiveChannel = new MidiChannelType();
            _volume = new LevelType(99);
            _velocityDepth = new LevelType(99);

            Notes = new List<DrumNote>();
            for (int i = 0; i < NoteCount; i++)
            {
                Notes.Add(new DrumNote());
            }
        }

        public DrumPatch(byte[] data) : this()
        {
            int offset = 0;
            byte b = 0;  // will be reused when getting the next byte

            (b, offset) = Util.GetNextByte(data, offset);
            _receiveChannel = new MidiChannelType(b);

            (b, offset) = Util.GetNextByte(data, offset);
            _volume = new LevelType(b);  // 0~100

            (b, offset) = Util.GetNextByte(data, offset);
            _velocityDepth = new LevelType(b);

            // Eat up the dummy bytes
            offset += 7;

            (b, offset) = Util.GetNextByte(data, offset);
            Checksum = b;

            for (int i = 0; i < NoteCount; i++)
            {
                byte[] noteData = new byte[DrumNote.DataSize];
                Array.Copy(data, offset, noteData, 0, DrumNote.DataSize);

                DrumNote note = new DrumNote(noteData);
                Notes.Add(note);

                offset += DrumNote.DataSize;
            }
        }

        public byte[] ToData()
        {
            List<byte> data = new List<byte>();

            data.Add(_receiveChannel.AsByte()); // 1~16 stored as 0~15
            data.Add(Volume);
            data.Add(VelocityDepth);

            // Add seven dummy bytes
            data.Add(0);
            data.Add(0);
            data.Add(0);
            data.Add(0);
            data.Add(0);
            data.Add(0);
            data.Add(0);

            // Add common checksum (gets computed by the property)
            data.Add(Checksum);

            // Add all the drum notes. Each one computes and adds its own checksum.
            foreach (DrumNote note in Notes)
            {
                data.AddRange(note.ToData());
            }

            return data.ToArray();
        }

        /// <summary>
        /// Generates a printable representation of this patch.
        /// </summary>
        /// <returns>
        /// String with patch parameter values.
        /// </returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append($"DRUM rcv ch = {this.ReceiveChannel}, volume = {this.Volume}, vel.depth = {this.VelocityDepth}\n");

            int noteNumber = 36;
            foreach (DrumNote note in this.Notes)
            {
                builder.Append(string.Format($"{noteNumber} {note.ToString()}"));
                noteNumber++;
            }

            return builder.ToString();
        }
    }
}
