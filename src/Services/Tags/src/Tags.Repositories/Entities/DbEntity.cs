using System;
using System.Collections.Generic;
using System.Text;

namespace Tags.Repositories.Entities
{
    public class DbEntity
    {
        public Guid Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string ModifiedBy { get; set; }
    }
}
