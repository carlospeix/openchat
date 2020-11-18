using System;

namespace OpenChat.Model
{
    public class Clock
    {
        public static Clock System => new Clock();
        public static Clock Simulated => new SimulatedClock();
        public static Clock ThrowsException => new ThrowsExceptionClock();

        protected Clock()
        {
        }

        public virtual DateTime Now => DateTime.Now;

        public virtual void Set(DateTime _)
        {
            throw new InvalidOperationException("Can't set system time. Please use the SimulatedClock instead.");
        }

        private class SimulatedClock : Clock
        {
            private DateTime fakeNow = DateTime.Now;

            public override DateTime Now => fakeNow;

            public override void Set(DateTime dateTime)
            {
                fakeNow = dateTime;
            }
        }

        private class ThrowsExceptionClock : Clock
        {
            public override DateTime Now => throw new Exception("What are you doing here?");

            public override void Set(DateTime dateTime)
            {
                throw new Exception("What are you doing here?");
            }
        }
    }
}
