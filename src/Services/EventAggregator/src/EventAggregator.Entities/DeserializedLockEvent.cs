using System;
using System.Collections.Generic;
using System.Text;
using Contracts.Events;

namespace EventAggregator.Entities
{
    public class DeserializedLockEvent
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public Guid AggregateId { get; set; }
        public string EventType { get; set; }
        public DateTime CreatedDate { get; set; }
        public BaseLockMessage Data { get; set; }
    }
}
