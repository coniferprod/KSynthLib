using System;
using System.Collections.Generic;

using Xunit;

using KSynthLib.SystemExclusive;

namespace KSynthLib.Tests.SystemExclusive
{
    public class MessageTests
    {
        public MessageTests()
        {

        }

        [Fact]
        public void TooShortIsRejected()
        {
            var bytes = new List<byte> { 0xf0, 0xf7 };
            Assert.Throws<ArgumentException>(() => Message.Create(bytes.ToArray()));
        }

        [Fact]
        public void MissingInitiatorIsRejected()
        {
            var bytes = new List<byte> { 0x00, 0x00, 0x00, 0x00, 0x00, 0xf7 };
            Assert.Throws<ArgumentException>(() => Message.Create(bytes.ToArray()));
        }

        [Fact]
        public void MissingTerminatorIsRejected()
        {
            var bytes = new List<byte> { 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00 };
            Assert.Throws<ArgumentException>(() => Message.Create(bytes.ToArray()));
        }
    }
}
