using Microsoft.EntityFrameworkCore;
using SPG_Fachtheorie.Aufgabe1.Model;

namespace SPG_Fachtheorie.Aufgabe1.Infrastructure
{
    public class AppointmentContext : DbContext
    {
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Manager> Managers => Set<Manager>();
        public DbSet<Cashier> Cashiers => Set<Cashier>();
        public DbSet<CashDesk> CashDesks => Set<CashDesk>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<PaymentItem> PaymentItems => Set<PaymentItem>();

        public AppointmentContext(DbContextOptions options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasDiscriminator<string>("Type")
                    .HasValue<Manager>("Manager")
                    .HasValue<Cashier>("Cashier")
                    .IsComplete(true);

                entity.Property(e => e.Type)
                    .HasMaxLength(50)
                    .IsRequired();
            });
        }
    }
}