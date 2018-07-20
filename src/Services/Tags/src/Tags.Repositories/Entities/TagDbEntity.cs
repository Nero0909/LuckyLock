using System;
using System.Collections.Generic;
using System.Text;

namespace Tags.Repositories.Entities
{
    public class TagDbEntity : DbEntity
    {
        public string Name { get; set; }

        public string UniqueNumber { get; set; }
    }
}
