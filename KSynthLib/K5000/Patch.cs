using System.Collections.Generic;

namespace KSynthLib.K5000
{
    /// <summary>
    /// Abstract base class providing subclasses customization points
    /// for computing checksums and collecting data for building System Exclusive
    /// messages.
    /// </summary>
    public abstract class Patch
    {
        protected byte _checksum;

        /// <value>
        /// Virtual property to compute a checksum for patch data.
        /// </value>
        /// <remarks>
        /// This property should be overridden by any subclass.
        /// </remarks>
        public virtual byte Checksum
        {
            get
            {
                byte[] data = CollectData();
                byte sum = 0;
                foreach (byte b in data)
                {
                    sum += b;
                }
                sum += 0xA5;
                return sum;
            }

            set => _checksum = value;
        }

        public Patch()
        {
            _checksum = 0x00;
        }

        protected abstract byte[] CollectData();

        /// <summary>
        /// Collects the data that makes up this patch in System Exclusive format.
        /// </summary>
        /// <returns>The SysEx data bytes.</returns>
        public byte[] ToData()
        {
            var allData = new List<byte>();
            byte[] data = CollectData();
            allData.AddRange(data);
            allData.Add(this.Checksum);
            return allData.ToArray();
        }
    }
}