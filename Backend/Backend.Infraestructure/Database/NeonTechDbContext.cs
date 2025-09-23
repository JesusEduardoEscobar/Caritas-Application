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

        // Ejemplo: tabla Users
        public DbSet<User> Users { get; set; }
    }

    // Entidad de ejemplo
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
