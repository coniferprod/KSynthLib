using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K5000
{
    /// <summary>
    /// Represents a K5000 multi patch (combi on K5000W/R).
    /// </summary>
    public class MultiPatch : IPatch, ISystemExclusiveData
    {
        public PatchName PatchName;

        /// <summary>
        /// Constructs a multi patch with default values.
        /// </summary>
        public MultiPatch() : base()
        {
            this.PatchName = new PatchName("NewMulti");

        }

        /// <summary>
        /// Constructs a multi patch from System Exclusive data.
        /// </summary>
        public MultiPatch(byte[] data) : base()
        {
            int offset = 0;
            byte b;
            (b, offset) = Util.GetNextByte(data, offset);

            // Ingest the checksum
            //_checksum = b;

            // TODO: Parse the multi from SysEx
        }

        /// <summary>
        /// Returns a string representation of this multi patch.
        /// </summary>
        /// <returns>
        /// String representation.
        /// </returns>
        public override string ToString()
        {
            var builder = new StringBuilder();

            // TODO: generate string representation

            return builder.ToString();
        }

        private List<byte> CollectData()
        {
            var data = new List<byte>();

            // TODO: Collect the data

            return data;
        }

        //
        // Implementation of the IPatch interface
        //

        public List<byte> GetSystemExclusiveData()
        {
            var data = new List<byte>();

            data.AddRange(this.CollectData());
            data.Add(this.Checksum);

            return data;
        }

        //
        // Implementation of the IPatch interface
        //

        public byte Checksum
        {
            get
            {
                List<byte> data = this.CollectData();
                byte sum = 0;
                foreach (byte b in data)
                {
                    sum += b;
                }
                sum += 0xA5;
                return sum;
            }
        }

        public string Name
        {
            get
            {
                return this.PatchName.Value;
            }

            set
            {
                this.PatchName = new PatchName(value);
            }
        }
    }
}
