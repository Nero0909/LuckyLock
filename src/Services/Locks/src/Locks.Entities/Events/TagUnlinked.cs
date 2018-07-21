using System;

namespace Contracts.Events
{
    public interface TagUnlinked : BaseLockMessage
    {
        Guid TagId { get; }
    }

    public class TagUnlinkedMessage : TagUnlinked
    {
        public Guid EventId { get; set; }
        public DateTime EventCreatedDate { get; set; }
        public string UserId { get; set; }
        public Guid LockId { get; set; }
        public Guid TagId { get; set; }
    }
}