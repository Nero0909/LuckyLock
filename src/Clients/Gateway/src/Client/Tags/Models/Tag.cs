using System;

namespace Client.Tags.Models
{
    public class Tag 
    {
        public Guid Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }
    }
}
