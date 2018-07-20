using System;

namespace Contracts.Events
{
    public interface LockCreated : BaseLockMessage
    {  
        string Name { get; }

        string UniqueNumber { get; }
    }

    public class LockCreatedMessage : LockCreated
    {
        public Guid EventId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserId { get; set; }
        public Guid LockId { get; set; }
        public string Name { get; set; }
        public string UniqueNumber { get; set; }
    }
}