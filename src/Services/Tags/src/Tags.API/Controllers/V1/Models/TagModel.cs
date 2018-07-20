using System.ComponentModel.DataAnnotations;

namespace Tags.API.Controllers.V1.Models
{
    public class TagModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string UniqueNumer { get; set; }
    }
}
