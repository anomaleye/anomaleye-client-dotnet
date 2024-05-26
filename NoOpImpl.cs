using System;

namespace Anomaleye
{
    internal class NoOpImpl : IAnomaleye
    {
        public void Dispose()
        {
        }

        public void Log(string eventType, object eventDetails, DateTime? eventTimeUtc = null)
        {
        }
    }
}