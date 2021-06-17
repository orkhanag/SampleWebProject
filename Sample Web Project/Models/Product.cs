using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sample_Web_Project.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }


        [Required]
        [Range(1f,float.MaxValue, ErrorMessage ="Price must be a positive number!")] 
        public float Price { get; set; }
    }
}
