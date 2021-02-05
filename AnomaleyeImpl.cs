using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Anomaleye.DirectClient;
using Anomaleye.DirectClient.Models;
using Newtonsoft.Json;

namespace Anomaleye
{
    internal class AnomaleyeImpl : IAnomaleye
    {
        // Blocking collection etc

        private IAnomaleyeClient _client;
        private string _systemVersionId;
        private string _recordingSessionId;
        private IClock _clock;

        public AnomaleyeImpl(
            IAnomaleyeClient client,
            string systemVersionId,
            string recordingSessionId,
            IClock clock = null)
        {
            this._client = client;
            this._systemVersionId = systemVersionId;
            this._recordingSessionId = recordingSessionId;
            this._clock = clock ?? new DefaultClock();
        }

        public void Log(string eventType, object eventDetails, DateTime? eventTimeUtc = null)
        {
            var evt = new EventRecord()
            {
                type = eventType,
                timeUtc = eventTimeUtc ?? this._clock.UtcNow,
                detailsJson = JsonConvert.SerializeObject(eventDetails),
            };

            Task.Run(() => this._client.RecordEventsAsync(this._systemVersionId, this._recordingSessionId, new List<EventRecord>(1) { evt }));
        }
    }
}