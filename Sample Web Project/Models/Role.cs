using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sample_Web_Project.Models
{
    public class Role
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        public int Name { get; set; }
    }
}
