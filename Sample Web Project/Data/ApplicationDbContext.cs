using Microsoft.EntityFrameworkCore;
using Sample_Web_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample_Web_Project.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; } 
        public DbSet<User> Users { get; set; } 
        public DbSet<Role> Roles { get; set; } 
    }
}
 