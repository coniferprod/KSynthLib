using System.Text;
using System.Collections.Generic;

using KSynthLib.Common;

namespace KSynthLib.K5000
{
    public class MultiPatch : Patch
    {
        public MultiPatch() : base()
        {

        }

        /// <summary>Constructs a multi patch from System Exclusive data.</summary>
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
            List<byte> data = new List<byte>();

            // TODO: Collect the data

            return data.ToArray();
        }

        public override byte Checksum => 0;  // TODO: Calculate checksum for multi

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            // TODO: generate string representation

            return builder.ToString();
        }
    }
}