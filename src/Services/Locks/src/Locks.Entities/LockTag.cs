using System;

namespace Locks.Entities
{
    public class LockTag
    {
        public Guid Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public Guid LockId { get; set; }

        public Guid TagId { get; set; }
    }
}
