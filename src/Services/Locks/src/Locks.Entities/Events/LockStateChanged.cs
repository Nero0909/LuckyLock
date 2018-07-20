using System;

namespace Contracts.Events
{
    public interface LockStateChanged: BaseLockMessage
    {
        string PreviousState { get; }

        string CurrentState { get; }
    }

    public class LockStateChangedMessage : LockStateChanged
    {
        public Guid EventId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserId { get; set; }
        public Guid LockId { get; set; }
        public string PreviousState { get; set; }
        public string CurrentState { get; set; }
    }
}
