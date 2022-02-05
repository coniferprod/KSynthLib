#nullable enable

using System;
using System.Collections.Generic;
using System.Text;

using KSynthLib.Common;

namespace KSynthLib.SystemExclusive
{
    public enum MessageKind
    {
        UniversalNonRealTime,
        UniversalRealTime,
        ManufacturerSpecific,
    }

    public class UniversalHeader
    {
        public byte DeviceChannel { get; set; }
        public byte SubId1 { get; set; }
        public byte SubId2 { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append(string.Format("Device Channel = {0}\n", this.DeviceChannel + 1));
            builder.Append(string.Format("Sub Id 1 = {0:X2}H, Sub Id 2 = {1:X2}H\n", this.SubId1, this.SubId2));

            return builder.ToString();
        }
    }

    public abstract class Message
    {
        public List<byte>? Payload;

        /// Creates a message from System Exclusive data bytes.
        public static Message Create(byte[] data)
        {
            byte[] GetPayload(int startIndex = 2)
            {
                return data[startIndex .. ^1];
            }

            UniversalHeader GetUniversalHeader()
            {
                return new UniversalHeader { DeviceChannel = data[2], SubId1 = data[3], SubId2 = data[4] };
            }

            if (data.Length < 5)
            {
                throw new ArgumentException("Message too short!");
            }

            if (data[0] != Constants.Initiator)
            {
                throw new ArgumentException(string.Format("Message must start with {0}", Constants.Initiator));
            }

            if (data[^1] != Constants.Terminator)
            {
                throw new ArgumentException(string.Format("Message must end with {0}", Constants.Terminator));
            }

            switch (data[1])
            {
                case Constants.Development:
                    return new ManufacturerSpecificMessage
                    {
                        Manufacturer = ManufacturerDefinition.Development,
                        Payload = new List<byte>(GetPayload())
                    };

                case Constants.UniversalNonRealTime:
                    return new UniversalMessage
                    {
                        IsRealtime = false,
                        Header = GetUniversalHeader(),
                        Payload = new List<byte>(GetPayload(4))
                    };

                case Constants.UniversalRealTime:
                    return new UniversalMessage
                    {
                        IsRealtime = true,
                        Header = GetUniversalHeader(),
                        Payload = new List<byte>(GetPayload(4))
                    };

                case 0x00:  // Extended manufacturer
                    byte[] identifier = new byte[] { data[1], data[2], data[3] };
                    var manufacturer = ManufacturerDefinition.Find(identifier);
                    return new ManufacturerSpecificMessage
                    {
                        Manufacturer = manufacturer,
                        Payload = new List<byte>(GetPayload(4))
                    };

                default:  // Standard manufacturer
                    return new ManufacturerSpecificMessage
                    {
                        Manufacturer = ManufacturerDefinition.Find(new byte[] { data[1] }),
                        Payload = new List<byte>(GetPayload())
                    };
            }
        }
    }

    public class UniversalMessage : Message
    {
        public bool IsRealtime { get; set; }
        public UniversalHeader? Header { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append(string.Format("Universal System Exclusive Message, {0}\n", this.IsRealtime ? "Real-time" : "Non-Real-time"));
            builder.Append(this.Header);

            return builder.ToString();
        }
    }

    public class ManufacturerSpecificMessage : Message
    {
        public ManufacturerDefinition? Manufacturer { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append(string.Format("Manufacturer: {0}\n", this.Manufacturer));
            builder.Append(string.Format("Payload: {0} bytes", this.Payload?.Count));

            return builder.ToString();
        }

        public byte[] ToData()
        {
            var result = new List<byte>();
            result.Add(Constants.Initiator);
            if (this.Payload != null)
            {
                result.AddRange(this.Payload);
            }
            result.Add(Constants.Terminator);
            return result.ToArray();
        }
    }
}
