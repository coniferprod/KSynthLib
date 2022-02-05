using System;
using System.Collections.Generic;

using Xunit;

using KSynthLib.SystemExclusive;

namespace KSynthLib.Tests.SystemExclusive
{
    public class ManufacturerTests
    {
        public ManufacturerTests()
        {

        }

        [Fact]
        public void IsDevelopment()
        {
            var bytes = new List<byte> { 0xf0, 0x7d, 0x00, 0x00, 0xf7 };
            var message = Message.Create(bytes.ToArray());
            Assert.IsType<ManufacturerSpecificMessage>(message);
            Assert.Equal(ManufacturerKind.Development, ((ManufacturerSpecificMessage)message).Manufacturer.Kind);
        }

        [Fact]
        public void IsStandard()
        {
            var bytes = new List<byte> { 0xf0, 0x40, 0x00, 0x00, 0x00, 0xf7 };
            var message = Message.Create(bytes.ToArray());
            Assert.IsType<ManufacturerSpecificMessage>(message);
            var manufacturer = ((ManufacturerSpecificMessage) message).Manufacturer;
            Assert.Equal(ManufacturerKind.Standard, manufacturer.Kind);
            Assert.Single(manufacturer.Identifier);
            Assert.Equal(0x40, manufacturer.Identifier[0]);
        }

        [Fact]
        public void IsFound()
        {
            var identifier = new byte[] { 0x40 };
            var result = ManufacturerDefinition.Find(identifier);
            Assert.NotNull(result);
        }
    }
}
