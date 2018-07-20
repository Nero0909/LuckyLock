using System;
using System.Collections.Generic;
using System.Text;
using Contracts.Events;

namespace EventAggregator.Entities
{
    public class LockActivity
    {
        public Guid LockId { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Type { get; set; }

        public BaseLockMessage Data { get; set; }
    }
}
