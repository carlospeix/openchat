using OpenChat.Model;
using System;
using System.Threading;
using Xunit;

namespace OpenChat.Tests
{
    public class ClockTests
    {
        [Fact]
        public void ClockRuns()
        {
            var clock = Clock.System;

            var now = clock.Now;
            Thread.Sleep(5);

            Assert.NotEqual(now, clock.Now);
        }

        [Fact]
        public void FakeClockDoesntRun()
        {
            var clock = Clock.Fake;

            var now = clock.Now;
            Thread.Sleep(5);

            Assert.Equal(now, clock.Now);
        }

        [Fact]
        public void CanSetTheClockFromOutside()
        {
            var clock = Clock.Fake;
            var now = DateTime.Now;

            clock.Set(now);
            Thread.Sleep(5);
            Assert.Equal(now, clock.Now);

            clock.Set(now.AddHours(2));
            Thread.Sleep(5);
            Assert.Equal(now.AddHours(2), clock.Now);
        }

        [Fact]
        public void ThrowsIfUsingTheSetMethodInSystemClock()
        {
            var clock = Clock.System;

            _ = Assert.Throws<InvalidOperationException>(() => clock.Set(DateTime.Now));
        }
    }
}
