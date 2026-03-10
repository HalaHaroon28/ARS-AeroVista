using Microsoft.EntityFrameworkCore;
using AeroVista.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AeroVista.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<City> Cities { get; set; }
        public DbSet<Flights> Flights { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Flights>()
                .HasOne(f => f.FromCity)
                .WithMany()
                .HasForeignKey(f => f.FromCityId)
                .OnDelete(DeleteBehavior.Restrict);

           
            modelBuilder.Entity<Flights>()
                .HasOne(f => f.ToCity)
                .WithMany()
                .HasForeignKey(f => f.ToCityId)
                .OnDelete(DeleteBehavior.Restrict);

           
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Flight)
                .WithMany()
                .HasForeignKey(b => b.FlightId)
                .OnDelete(DeleteBehavior.Restrict);

        
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}