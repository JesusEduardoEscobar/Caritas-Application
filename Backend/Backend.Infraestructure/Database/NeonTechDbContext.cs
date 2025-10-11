using Backend.Infraestructure.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;
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
        public DbSet<Bed> Beds { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        public DbSet<Car> Cars { get; set; }
        public DbSet<TransportRequest> TransportRequests { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.HasPostgresEnum<UserRole>();
            modelBuilder.HasPostgresEnum<EconomicLevel>();
            modelBuilder.HasPostgresEnum<ReservationStatus>();

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.HasKey(s => s.Id);

                entity.Property(s => s.Id).HasColumnName("id");
                entity.Property(s => s.Name).HasColumnName("name");
                entity.Property(s => s.Password).HasColumnName("password");
                entity.Property(s => s.Email).HasColumnName("email");
                entity.Property(s => s.Age).HasColumnName("age");
                entity.Property(s => s.EconomicLevel).HasColumnName("economic_level");
                entity.Property(s => s.Verified).HasColumnName("verified");
                entity.Property(s => s.ShelterId).HasColumnName("shelter_id");
                entity.Property(s => s.Role).HasColumnName("role");
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


            modelBuilder.Entity<Bed>(entity =>
            {
                entity.ToTable("beds");

                entity.HasKey(b => b.Id);

                entity.Property(b => b.Id).HasColumnName("id");
                entity.Property(b => b.ShelterId).HasColumnName("shelter_id");
                entity.Property(b => b.BedNumber).HasColumnName("bed_number");
                entity.Property(b => b.IsAvailable).HasColumnName("is_available");
            });

            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.ToTable("reservations");

                entity.HasKey(r => r.Id);

                entity.Property(r => r.Id).HasColumnName("id");
                entity.Property(r => r.UserId).HasColumnName("user_id");
                entity.Property(r => r.BedId).HasColumnName("bed_id");
                entity.Property(r => r.StartDate).HasColumnName("start_date");
                entity.Property(r => r.EndDate).HasColumnName("end_date");
                entity.Property(r => r.Status).HasColumnName("status");
                entity.Property(r => r.CreatedAt).HasColumnName("created_at");
            });

            modelBuilder.Entity<Car>(entity =>
            {
                entity.ToTable("reservations");

                entity.HasKey(c => c.Id);

                entity.Property(c => c.Id).HasColumnName("id");
                entity.Property(c => c.ShelterId).HasColumnName("shelter_id");
                entity.Property(c => c.Plate).HasColumnName("plate");
                entity.Property(c => c.Model).HasColumnName("model");
                entity.Property(c => c.Capacity).HasColumnName("capacity");
            });

            modelBuilder.Entity<TransportRequest>(entity =>
            {
                entity.ToTable("reservations");

                entity.HasKey(tr => tr.Id);

                entity.Property(tr => tr.Id).HasColumnName("id");
                entity.Property(tr => tr.UserId).HasColumnName("user_id");
                entity.Property(tr => tr.CarId).HasColumnName("car_id");
                entity.Property(tr => tr.PickupLocation).HasColumnName("pickup_location");
                entity.Property(tr => tr.DropoffLocation).HasColumnName("dropoff_location");
                entity.Property(tr => tr.RequestDate).HasColumnName("request_date");
                entity.Property(tr => tr.Status).HasColumnName("status");
            });

        }
    }

    




}
