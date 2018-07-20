using System;
using Client.Locks;
using Client.Tags;

namespace Client
{
    public interface IGateway : IDisposable
    {
        ITags Tags { get; }

        ILocks Locks { get; }
    }
}