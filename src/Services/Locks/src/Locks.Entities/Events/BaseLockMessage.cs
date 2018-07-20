using System;

namespace Contracts.Events
{
    public interface BaseLockMessage
    {
        Guid EventId { get; }

        DateTime CreatedDate { get; }

        string UserId { get; }

        Guid LockId { get; }
    }
}