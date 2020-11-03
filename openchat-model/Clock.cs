using System;

namespace OpenChat.Model
{
    public class Clock
    {
        public static Clock Real => new Clock();
        public static Clock Fake => new FakeClock();

        protected Clock()
        {
        }

        public virtual DateTime Now => DateTime.Now;

        private class FakeClock : Clock
        {
            private readonly DateTime fakeNow = DateTime.Now;

            public override DateTime Now => fakeNow;
        }
    }
}
