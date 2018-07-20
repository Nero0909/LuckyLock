using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Locks.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Locks.API.Controllers.V1.Models
{
    public class LockUpdateModel
    {
        [Required]
        public LockState? State { get; set; }
    }
}
