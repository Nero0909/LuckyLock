using System;

namespace Contracts.Events
{
    public interface LockDeleted : BaseLockMessage
    {
    }

    public class LockDeletedMessage : LockDeleted
    {
        public Guid EventId { get; set; }
        public DateTime EventCreatedDate { get; set; }
        public string UserId { get; set; }
        public Guid LockId { get; set; }
    }
}