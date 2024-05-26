using System;

namespace Anomaleye.DirectClient.Models
{
    internal class EventRecord
    {
        public string type { get; set; }

        public DateTime timeUtc { get; set; }

        public string detailsJson { get; set; }
    }
}