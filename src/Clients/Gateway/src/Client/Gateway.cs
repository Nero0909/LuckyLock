using System;
using Client.Locks;
using Client.Tags;

namespace Client
{
    public class Gateway : IGateway
    {
        public Gateway(GatewaySettings settings)
        {
            if(settings == null) throw new ArgumentNullException(nameof(settings));

            Tags = new Tags.Tags(settings);
            Locks = new Locks.Locks(settings);
        }

        public void Dispose()
        {
            Tags?.Dispose();
            Locks?.Dispose();
        }

        public ITags Tags { get; }
        public ILocks Locks { get; }
    }
}
