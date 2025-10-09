using Backend.Infraestructure.Models;
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

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.HasKey(s => s.Id);

                entity.Property(s => s.Id).HasColumnName("id");
                entity.Property(s => s.Name).HasColumnName("name");
                entity.Property(s => s.Password).HasColumnName("password");
                entity.Property(s => s.Email).HasColumnName("email");
                entity.Property(s => s.Age).HasColumnName("age");
                //entity.Property(s => s.EconomicLevel).HasColumnName("economic_level");
                entity.Property(s => s.verificate).HasColumnName("verified");
                //entity.Property(s => s.shelter).HasColumnName("shelter_id");
                entity.Property(s => s.isAdmin).HasColumnName("role");
            });

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


}
