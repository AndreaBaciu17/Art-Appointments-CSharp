using Microsoft.EntityFrameworkCore;
using ArtConnect.Models;

namespace ArtConnect.Services
{
    public class ArtAppointmentsDbContext : DbContext
    {
        public DbSet<Owner> Owners { get; init; } = null!;

        public DbSet<Appointment> Appointments { get; init; } = null!;

        public ArtAppointmentsDbContext(DbContextOptions options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Owner>();
            modelBuilder.Entity<Appointment>();
        }
    }
}
