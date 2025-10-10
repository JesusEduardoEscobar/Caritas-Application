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
        public DbSet<Shelter> Shelters { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ShelterService> ShelterServices { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Shelter>(entity =>
            {
                entity.ToTable("shelters");

                entity.HasKey(s => s.Id);

                entity.Property(s => s.Id).HasColumnName("id");
                entity.Property(s => s.Name).HasColumnName("name");
                entity.Property(s => s.Address).HasColumnName("address");
                entity.Property(s => s.Latitude).HasColumnName("latitude");
                entity.Property(s => s.Longitude).HasColumnName("longitude");
                entity.Property(s => s.Phone).HasColumnName("phone");
                entity.Property(s => s.Capacity).HasColumnName("capacity");
                entity.Property(s => s.Description).HasColumnName("description");
                entity.Property(s => s.CreatedAt).HasColumnName("created_at");
                entity.Property(s => s.Occupancy).HasColumnName("occupancy");
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.ToTable("services");

                entity.HasKey(s => s.Id);

                entity.Property(s => s.Id).HasColumnName("id");
                entity.Property(s => s.Name).HasColumnName("name");
                entity.Property(s => s.Description).HasColumnName("description");
                entity.Property(s => s.IconKey).HasColumnName("icon_key");
            });

            modelBuilder.Entity<ShelterService>(entity =>
            {
                entity.ToTable("shelter_services");

                entity.HasKey(ss => new { ss.ShelterId, ss.ServiceId });

                entity.Property(ss => ss.ShelterId).HasColumnName("shelter_id");
                entity.Property(ss => ss.ServiceId).HasColumnName("service_id");
                entity.Property(ss => ss.Price).HasColumnName("price");
                entity.Property(ss => ss.IsAvailable).HasColumnName("is_available");
                entity.Property(ss => ss.Description).HasColumnName("description");
                entity.Property(ss => ss.Capacity).HasColumnName("capacity");
            }); 

        }
    }

    public class Shelter
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Phone { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Occupancy { get; set; }
    }

    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IconKey { get; set; }
    }

    public class ShelterService
    {
        public int ShelterId { get; set; }
        public int ServiceId { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public string? Description { get; set; }
        public int Capacity { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public int EconomicLevel { get; set; }
        public bool verificate { get; set; }
        public string shelter { get; set; }
        public bool? isAdmin { get; set; }

    }


}
