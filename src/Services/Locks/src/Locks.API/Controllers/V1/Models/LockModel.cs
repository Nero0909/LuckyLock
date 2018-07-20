using System.ComponentModel.DataAnnotations;

namespace Locks.API.Controllers.V1.Models
{
    public class LockModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string UniqueNumer { get; set; }
    }
}
