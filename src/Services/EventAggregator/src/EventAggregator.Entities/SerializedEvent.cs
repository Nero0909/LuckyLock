using System;

namespace EventAggregator.Entities
{
    public class SerializedEvent
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public Guid AggregateId { get; set; }
        public string EventType { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Data { get; set; }
    }
}
