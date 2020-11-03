using OpenChat.Model;
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
    }
}
