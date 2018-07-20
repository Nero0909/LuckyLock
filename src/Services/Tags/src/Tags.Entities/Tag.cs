using System;
using System.Collections.Generic;
using System.Text;

namespace Tags.Entities
{
    public class Tag 
    {
        public Guid Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public string Name { get; set; }

        public string UniqueNumber { get; set; }
    }
}
