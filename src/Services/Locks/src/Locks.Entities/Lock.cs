using System;
using System.Collections.Generic;
using System.Text;

namespace Locks.Entities
{
    public class Lock
    {
        public Guid Id { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Name { get; set; }

        public string UniqueNumber { get; set; }

        public LockState State { get; set; }
    }
}
