using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K5000
{
    /// <summary>
    /// Represents a K5000 multi patch (combi on K5000W/R).
    /// </summary>
    public class MultiPatch : Patch
    {
        public PatchName Name;

        /// <summary>
        /// Constructs a multi patch with default values.
        /// </summary>
        public MultiPatch() : base()
        {
            Name = new PatchName("NewMulti");

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
            _checksum = b;

            // TODO: Parse the multi from SysEx
        }

        protected override byte[] CollectData()
        {
            var data = new List<byte>();

            // TODO: Collect the data

            return data.ToArray();
        }

        public override byte Checksum => 0;  // TODO: Calculate checksum for multi

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
    }
}
