using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
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
        private string _systemId;
        private string _systemVersionId;
        private string _recordingSessionId;
        private IClock _clock;
        private BlockingCollection<EventRecord> _queue = new BlockingCollection<EventRecord>();
        private List<Exception> _exceptions = new List<Exception>();
        private CancellationTokenSource _stopper = new();
        private CancellationToken _stopToken;
        private bool _running = true;

        public AnomaleyeImpl(
            IAnomaleyeClient client,
            string systemId,
            string systemVersionId,
            string recordingSessionId,
            IClock clock = null)
        {
            this._client = client;
            this._systemId = systemId;
            this._systemVersionId = systemVersionId;
            this._recordingSessionId = recordingSessionId;
            this._clock = clock ?? new DefaultClock();

            this._stopToken = this._stopper.Token;

            // TODO: dispose
            Task.Run(async () =>
            {
                DateTime timeLastSentBatch = DateTime.UtcNow;
                var batch = new List<EventRecord>(capacity: 21);
                int total = 0;
                bool done = false;
                while (batch.Count > 0 || !done)
                {
                    Console.WriteLine("poll coll again");
                    if (this._queue.TryTake(out EventRecord record, TimeSpan.FromSeconds(3)))
                    {
                        batch.Add(record);
                    }
                    else
                    {
                        if (this._stopToken.IsCancellationRequested)
                        {
                            done = true;
                        }
                    }

                    if (batch.Count != 0 && (batch.Count == 21 || DateTime.UtcNow - timeLastSentBatch > TimeSpan.FromSeconds(3)))
                    {
                        try
                        {
                            Console.WriteLine("sending batch");
                            // TODO: Give records GUIDs so we can retry and dedupe.
                            await this._client.RecordEventsAsync(this._systemId, this._systemVersionId, this._recordingSessionId, batch);
                            total += batch.Count;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);

                            this._exceptions.Add(e);

                            await Task.Delay(TimeSpan.FromSeconds(7));
                        }

                        timeLastSentBatch = DateTime.UtcNow;
                        batch.Clear();
                    }

                            Console.WriteLine($"I have sent {total} events");
                    

                }

                this._running = false;
            });
        }

        public void Dispose()
        {
            while (this._queue.Count > 0)
            {
                Console.WriteLine("Waiting for queue to drain");
                Thread.Sleep(500);
            }

            this._stopper.Cancel();

            while (this._running)
            {
                Thread.Sleep(500);
            }

            Console.WriteLine("Disposed");
        }

        public void Log(string eventType, object eventDetails, DateTime? eventTimeUtc = null)
        {
            if (this._exceptions.Count != 0)
            {
                throw new AggregateException(this._exceptions);
            }

            var evt = new EventRecord()
            {
                type = eventType,
                timeUtc = eventTimeUtc ?? this._clock.UtcNow,
                detailsJson = JsonConvert.SerializeObject(eventDetails),
            };

            this._queue.Add(evt);

            // Task.Run(async () =>
            // {
            //     for (int i = 0; i < 5; i++)
            //     {
            //         try
            //         {
            //             await this._client.RecordEventsAsync(this._systemId, this._systemVersionId, this._recordingSessionId, new List<EventRecord>(1) { evt });
            //             break;
            //         }
            //         catch (Exception e)
            //         {
            //             Console.WriteLine(e);

            //             await Task.Delay(TimeSpan.FromSeconds(7));
            //         }
            //     }
            // });
        }
    }
}