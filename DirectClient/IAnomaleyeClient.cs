using System.Collections.Generic;
using System.Threading.Tasks;
using Anomaleye.DirectClient.Models;

namespace Anomaleye.DirectClient
{
    /// <summary>
    /// Abstract means of communicating with the Anomaleye server.
    /// </summary>
    internal interface IAnomaleyeClient
    {
        Task RecordEventsAsync(string systemId, string systemVersionId, string recordingSessionId, List<EventRecord> events);
    }
}