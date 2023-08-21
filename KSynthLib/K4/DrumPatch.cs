using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using SyxPack;
using KSynthLib.Common;

namespace KSynthLib.K4
{

    public class DrumPatch : Patch, ISystemExclusiveData
    {
        public const int DataSize = 682;
        public const int NoteCount = 61;  // from C1 to C6

        public Channel ReceiveChannel;
        public Level Volume;
        public Level VelocityDepth;

        public List<DrumNote> Notes;

        public override byte Checksum
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
        }

        public byte[] OriginalData;

        public DrumPatch()
        {
            ReceiveChannel = new Channel(1);
            Volume = new Level(99);
            VelocityDepth = new Level(99);

            Notes = new List<DrumNote>();
            for (var i = 0; i < NoteCount; i++)
            {
                Notes.Add(new DrumNote());
            }

            OriginalData = null;
        }

        public DrumPatch(byte[] data)
        {
            byte b;  // will be reused when getting the next byte
            int offset = 0;

            (b, offset) = Util.GetNextByte(data, offset);
            ReceiveChannel = new Channel(b);

            (b, offset) = Util.GetNextByte(data, offset);
            Volume = new Level(b);

            (b, offset) = Util.GetNextByte(data, offset);
            VelocityDepth = new Level(b);

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

            OriginalData = new byte[DataSize];
            Array.Copy(data, OriginalData, DataSize);
        }

        protected override List<byte> CollectData()
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

            return data;
        }

        //
        // ISystemExclusiveData implementation
        //

        public List<byte> Data
        {
            get
            {
                var data = new List<byte>();

                data.AddRange(this.CollectData());

                // Add common checksum (gets computed by the property)
                data.Add(Checksum);

                // Add all the drum notes. Each one computes and adds its own checksum.
                foreach (var note in Notes)
                {
                    data.AddRange(note.ToData());
                }

                return data;
            }
        }

        public int DataLength => DataSize;

        /// <summary>
        /// Generates a printable representation of this patch.
        /// </summary>
        /// <returns>
        /// String with patch parameter values.
        /// </returns>
        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine($"DRUM rcv ch = {this.ReceiveChannel}, volume = {this.Volume}, vel.depth = {this.VelocityDepth}");

            var noteNumber = 36;
            foreach (var note in this.Notes)
            {
                builder.Append(string.Format($"{noteNumber} {note}"));
                noteNumber++;
            }

            return builder.ToString();
        }
    }
}
