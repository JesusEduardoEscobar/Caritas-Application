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

        // Reservaciones de servicios

        public DbSet<ServiceReservation> ServiceReservations { get; set; }


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

            modelBuilder.Entity<ServiceReservation>(entity =>
            {
                entity.ToTable("service_reservations");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.ShelterId).HasColumnName("shelter_id");
                entity.Property(e => e.ServiceId).HasColumnName("service_id");
                entity.Property(e => e.QrData).HasColumnName("qr_data");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Shelter>()
                      .WithMany()
                      .HasForeignKey(e => e.ShelterId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Service>()
                      .WithMany()
                      .HasForeignKey(e => e.ServiceId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // User
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.HasKey(u => u.Id);

                entity.Property(u => u.Id).HasColumnName("id");
                entity.Property(u => u.Name).HasColumnName("name");
                entity.Property(u => u.Password).HasColumnName("password");
                entity.Property(u => u.Email).HasColumnName("email");
                entity.Property(u => u.Age).HasColumnName("age");
                entity.Property(u => u.EconomicLevel).HasColumnName("economic_level");
                entity.Property(u => u.verificate).HasColumnName("verificate");
                entity.Property(u => u.shelter).HasColumnName("shelter");
                entity.Property(u => u.isAdmin).HasColumnName("is_admin");
            });

            base.OnModelCreating(modelBuilder);


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
