using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sample_Web_Project.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [DisplayName("Full Name")]
        public string FullName { get; set; }

        [DisplayName("Date of Birth")]
        public DateTime BirthDate { get; set; }

        [Required]
        public DateTime AddDate { get; set; }
    }
}
