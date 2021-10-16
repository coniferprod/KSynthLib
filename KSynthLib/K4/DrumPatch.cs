using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using KSynthLib.Common;

namespace KSynthLib.K4
{

    public class DrumPatch
    {
        public const int DataSize = 682;
        public const int NoteCount = 61;  // from C1 to C6

        public ChannelType ReceiveChannel;
        public LevelType Volume;
        public LevelType VelocityDepth;
        public List<DrumNote> Notes;

        private byte _checksum;
        public byte Checksum
        {
            get
            {
                var data = new List<byte>();

                data.Add(ReceiveChannel.ToByte());
                data.Add(Volume.ToByte());
                data.Add(VelocityDepth.ToByte());

                byte[] bs = data.ToArray();
                byte sum = 0;
                foreach (var b in bs)
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
            ReceiveChannel = new ChannelType(1);
            Volume = new LevelType(99);
            VelocityDepth = new LevelType(99);

            Notes = new List<DrumNote>();
            for (var i = 0; i < NoteCount; i++)
            {
                Notes.Add(new DrumNote());
            }
        }

        public DrumPatch(byte[] data)
        {
            int offset = 0;
            byte b = 0;  // will be reused when getting the next byte

            (b, offset) = Util.GetNextByte(data, offset);
            ReceiveChannel = new ChannelType(b);

            (b, offset) = Util.GetNextByte(data, offset);
            Volume = new LevelType(b);  // 0~100

            (b, offset) = Util.GetNextByte(data, offset);
            VelocityDepth = new LevelType(b);

            // Eat up the dummy bytes
            offset += 7;

            (b, offset) = Util.GetNextByte(data, offset);
            Checksum = b;

            Notes = new List<DrumNote>();

            for (var i = 0; i < NoteCount; i++)
            {
                byte[] noteData = new byte[DrumNote.DataSize];
                Array.Copy(data, offset, noteData, 0, DrumNote.DataSize);

                DrumNote note = new DrumNote(noteData);
                Notes.Add(note);

                offset += DrumNote.DataSize;
            }

            Debug.Assert(Notes.Count == NoteCount);
        }

        public byte[] ToData()
        {
            var data = new List<byte>();

            data.Add(ReceiveChannel.ToByte());
            data.Add(Volume.ToByte());
            data.Add(VelocityDepth.ToByte());

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
            foreach (var note in Notes)
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
            var builder = new StringBuilder();

            builder.Append($"DRUM rcv ch = {this.ReceiveChannel}, volume = {this.Volume}, vel.depth = {this.VelocityDepth}\n");

            var noteNumber = 36;
            foreach (var note in this.Notes)
            {
                builder.Append(string.Format($"{noteNumber} {note.ToString()}"));
                noteNumber++;
            }

            return builder.ToString();
        }
    }
}
