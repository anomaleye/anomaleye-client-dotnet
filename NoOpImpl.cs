using System;

namespace Anomaleye
{
    internal class NoOpImpl : IAnomaleye
    {
        public void Log(string eventType, object eventDetails, DateTime? eventTimeUtc = null)
        {
        }
    }
}