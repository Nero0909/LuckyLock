using System;

namespace Contracts.Events
{
    public interface TagLinked: BaseLockMessage
    {
        Guid TagId { get; }
    }

    public class TagLinkedMessage : TagLinked
    {
        public Guid EventId { get; set; }
        public DateTime EventCreatedDate { get; set; }
        public string UserId { get; set; }
        public Guid LockId { get; set; }
        public Guid TagId { get; set; }
    }
}