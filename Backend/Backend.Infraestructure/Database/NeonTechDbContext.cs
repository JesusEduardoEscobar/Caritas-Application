using Backend.Infraestructure.Objects.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Backend.Infrastructure.Database
{
    public class NeonTechDbContext : DbContext
    {
        public NeonTechDbContext(DbContextOptions<NeonTechDbContext> options)
            : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }
    }
}
