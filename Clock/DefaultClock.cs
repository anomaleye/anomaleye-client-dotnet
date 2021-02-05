using System;

namespace Anomaleye
{
    public class DefaultClock : IClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}