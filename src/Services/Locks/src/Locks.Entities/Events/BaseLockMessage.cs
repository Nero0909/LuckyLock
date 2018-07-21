using System;

namespace Contracts.Events
{
    public interface BaseLockMessage
    {
        Guid EventId { get; }

        DateTime EventCreatedDate { get; }

        string UserId { get; }

        Guid LockId { get; }
    }
}