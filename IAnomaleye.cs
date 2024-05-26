using System;

namespace Anomaleye
{
    public interface IAnomaleye : IDisposable
    {
        /// <summary>
        /// Enqueues the event to be shipped to the Anomaleye server.
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="eventDetails"></param>
        /// <param name="eventTimeUtc"></param>
        void Log(string eventType, object eventDetails, DateTime? eventTimeUtc = null);
    }
}