using System;

namespace Locks.Repository.Entities
{
    public class LockTagDbEntity : DbEntity
    {
        public Guid LockId { get; set; }

        public Guid TagId { get; set; }
    }
}
